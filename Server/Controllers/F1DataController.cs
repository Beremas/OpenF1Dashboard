using Microsoft.AspNetCore.Mvc;
using OpenF1Dashboard.Server.DataSource;
using OpenF1Dashboard.Server.Models;
using OpenF1Dashboard.Shared.Enums;
using OpenF1Dashboard.Shared.Models;
using OpenF1Dashboard.Shared.Utilities;
using OpenF1Dashboard.Shared.Views;

namespace OpenF1Dashboard.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class F1DataController : ControllerBase
    {
        #region CONSTRUCTOR
        public F1DataController(IHttpClientFactory httpFactory) => _source = new OpenF1(httpFactory);
        #endregion

        #region PUBLIC (Methods)
        [HttpGet("analyzer")]
        public async Task<IActionResult> GetAnalysisAsync(int year, string circuitShortName, int numDriversToShow, SessionTypes sessionType)
        {
            try
            {
                AnalyzerSummary response = new();

                List<Session>                  sessions             = await _source.GetSessionsAsync(null, null, null, sessionType.ToString(), year, circuitShortName);
                if (sessions.Count == 0) { 
                    return NotFound();
                }
                var sessionKey                                       = sessions.Select(s => s.session_key).FirstOrDefault();
                Dictionary<int, List<double?>>  driversNoLapTimes    = await GetDriversLapTimes(year, sessionKey);
                Dictionary<int, double?>        driversno_bestlap    = GetDriverBestLapTime(driversNoLapTimes);
                List<SessionResult>             sessionResult;
                List<Driver>                    drivers              = await _source.GetDriversAsync(sessionKey);
                List<Pit>                       pits                 = await _source.GetPitsAsync(sessionKey);
                List<Stint>                     stints               = await _source.GetStintsAsync(sessionKey);
                List<Overtake>                  overtakes            = await _source.GetOvertakesAsync(sessionKey);


                if (sessionType == SessionTypes.Race)
                {
                    sessionResult = await _source.GetSessionResultAsync(sessionKey);

                    if (sessionResult.Count == 0)
                    {
                        return NotFound();
                    }

                    response.Drivers = [.. Enumerable.Range(0, Math.Min(numDriversToShow, sessionResult?.Count ?? 0)).Select(index =>
                    {
                        var driverno = sessionResult?.ElementAtOrDefault(index)?.driver_number ?? 0;
                        var driverName = drivers?.FirstOrDefault(d => d.driver_number == driverno)?.full_name ?? "N/A";
                        var driverStints = stints?.Where(s => s.driver_number == driverno).ToList();
                        var driverBestLapTime = driversno_bestlap?.GetValueOrDefault(driverno) ?? 0.0;
                        var totalLapTimes = driversNoLapTimes?.GetValueOrDefault(driverno);
                        var totalPits = pits?.Count(p => p.driver_number == driverno) ?? 0;
                        var totalStints = stints?.Count(s => s.driver_number == driverno) ?? 0;
                        var dnf = sessionResult?.Where(sr => sr.driver_number == driverno).Select(sr => sr.dnf).FirstOrDefault();
                        var dns = sessionResult?.Where(sr => sr.driver_number == driverno).Select(sr => sr.dns).FirstOrDefault();
                        var dsq = sessionResult?.Where(sr => sr.driver_number == driverno).Select(sr => sr.dsq).FirstOrDefault();
                        var position = sessionResult?.Where(sr => sr.driver_number == driverno).Select(sr => sr.position).FirstOrDefault();
                        var posGained = overtakes.Where(o => o.overtaking_driver_number == driverno)?.Count() ?? 0;
                        var posLost = overtakes.Where(o => o.overtaken_driver_number == driverno)?.Count() ?? 0;
                        var compoundEndLap = driverStints?.Any() == true
                                    ? driverStints
                                        .GroupBy(s => StringHelper.Capitalize(s.compound ?? "Unknown"))
                                        .ToDictionary(
                                            g => g.Key,
                                            g => g.Select(s => s.lap_end ?? 0).ToList()
                                        )
                                    : new Dictionary<string, List<int>>();
                        var isFastest = IsFastestDriver(driversno_bestlap, driverno);

                        return new DriverSummary
                        {
                            FullName = driverName,
                            Race = new RaceSummary
                            {
                                Position = position,
                                IsFastest = isFastest,
                                BestLapTime = driverBestLapTime,
                                LapTimes = totalLapTimes,
                                Pits = totalPits,
                                Stints = totalStints,
                                Dnf = dnf ?? false,
                                Dns = dns ?? false,
                                Dsq = dsq ?? false,
                                PosGained = posGained,
                                PosLost = posLost,
                                CompoundEndLap = compoundEndLap,
                            }
                        };
                    })];

                    var fastestDriverNo = driversno_bestlap?.ElementAt(0).Key ?? 0;
                    response.FastestDriver = sessionResult?.FirstOrDefault(sr => sr.driver_number == fastestDriverNo) is var fastest && fastest != null
                          ? new DriverSummary
                          {
                              FullName = drivers?.FirstOrDefault(d => d.driver_number == fastestDriverNo)?.full_name ?? "N/A",
                              Race = new RaceSummary
                              {
                                  Position = sessionResult?.FirstOrDefault(sr => sr.driver_number == fastestDriverNo)?.position,
                                  IsFastest = IsFastestDriver(driversno_bestlap,fastestDriverNo),
                                  BestLapTime = driversno_bestlap?.GetValueOrDefault(fastestDriverNo) ?? 0.0,
                                  LapTimes = driversNoLapTimes?.GetValueOrDefault(fastestDriverNo),
                                  Pits = pits?.Count(p => p.driver_number == fastestDriverNo) ?? 0,
                                  Stints = stints?.Count(s => s.driver_number == fastestDriverNo) ?? 0
                              }
                          }
                          : null;
                    response.CircuitGeojson = GetGeoJsonBy(circuitShortName);
                }
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("401"))
                    return StatusCode(401, $"Unauthorized: {ex.Message}");
                if (ex.Message.Contains("403"))
                    return StatusCode(403, $"Forbidden: {ex.Message}");
                return StatusCode(503, $"API request failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
        #endregion

        #region PRIVATE (Methods)

        private static string GetGeoJsonBy(string circuitShortName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Circuits", $"{circuitShortName}.geojson");
            return System.IO.File.ReadAllText(path);
        }
        private static bool IsFastestDriver(Dictionary<int, double?>? drivernoLaptime, int driverNo)
        {
            return drivernoLaptime?.ElementAt(0).Key == driverNo;
        }
        private async Task<Dictionary<int,List<double?>>> GetDriversLapTimes(int year, int? sessionKey)
        {
            try
            {
                Dictionary<int, List<double?>> driversno_lapsduration = [];
                if (sessionKey != null)
                {
                    var laps = await _source.GetLapsAsync(sessionKey);
                    //var driver = await _source.GetDriversAsync(session_key, null);
                    foreach (var lap in laps)
                    {
                        //var driver_full_name = driver?.Where(x => x.driver_number == lap.driver_number).Select(x => x.full_name).FirstOrDefault() ?? "N/A";
                        if (!driversno_lapsduration.ContainsKey(lap.driver_number))
                        {
                            List<double?> lap_duration = new() { lap.lap_duration };
                            driversno_lapsduration[lap.driver_number] = lap_duration;
                        }
                        else
                        {
                            driversno_lapsduration.TryGetValue(lap.driver_number, out var existing_laps_duration);
                            if (existing_laps_duration != null)
                            {
                                existing_laps_duration.Add(lap.lap_duration);
                                driversno_lapsduration[lap.driver_number] = existing_laps_duration;
                            }
                        }
                    }
                }
                return driversno_lapsduration;
            }
            catch (Exception ex)
            {
                return [];
            }
        }
        private static Dictionary<int,double?> GetDriverBestLapTime(Dictionary<int, List<double?>> driversNoLapTimes)
        {
            return driversNoLapTimes
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Any(l => l.HasValue)
                            ? kvp.Value.Where(l => l.HasValue).Min()
                            : (double?)null
                    )
                    .OrderBy(kvp => kvp.Value ?? double.MaxValue)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        #endregion

        #region ATTRIBUTES    
        private readonly OpenF1 _source;
        #endregion
    }
}
