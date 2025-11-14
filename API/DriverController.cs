using OpenF1Dashboard.API.Models;
using OpenF1Dashboard.Enum;
using OpenF1Dashboard.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using System.Security.AccessControl;
using System.Text.Json;

namespace OpenF1Dashboard.API
{

	public enum DebugLevel
	{
		None = 0,
		Verbose
	}

	public class DriverController
	{
		private readonly HttpApiClient  _httpClient;
		private readonly Dictionary<string,string> _openf1APIs;

		public DriverController()
		{
			_httpClient = new();
			_openf1APIs = _httpClient.endpoints ?? [];
		}
		

		public async Task<DriverResponse> GetWorldDriverChampionInfoByYear(int year)
		{
			try
			{
				if (DateTime.Now.Year == year)
				{
					return new DriverResponse { error_message = $"{year} F1 season is not yet completed." };
				}
				var race_sessions = await _httpClient.GetAsync<List<Session>>(_openf1APIs["SESSIONS"], $"year={year}");
				if (race_sessions == null || race_sessions.Count == 0)
				{
					return new DriverResponse { error_message = $"~/openf1.org/v1/sessions didn't return anything" };
				}
				else
				{
					Dictionary<int, decimal> driver_points = [];
					foreach (var race in race_sessions)
					{
						var positions = await _httpClient.GetAsync<List<Position>>(_openf1APIs["SESSION_RESULT"], $"session_key={race.session_key}", false, 200);
						if (positions == null)
						{
							continue;
						}

						foreach (var pos in positions)
						{
							var driver_number = pos.driver_number;
							var points = pos.points ?? 0;

							if (driver_points.ContainsKey(driver_number))
							{
								driver_points[driver_number] += points;
							}
							else
							{
								driver_points[driver_number] = points;
							}
						}
					}

					var session_key = race_sessions.Select(t => t.session_key).FirstOrDefault();
					var drivers_info = await _httpClient.GetAsync<List<Driver>>(_openf1APIs["DRIVERS"], $"session_key={session_key}");
					if (drivers_info == null || drivers_info.Count == 0)
					{
						return new DriverResponse { error_message = $"~/openf1.org/v1/drivers didn't return anything" };
					}

					var world_driver_champion = driver_points.OrderByDescending(kvp => kvp.Value).First();
					var full_name = Utility.GetNameByDriverNumber(world_driver_champion.Key);
					var full_points = world_driver_champion.Value;
					var nationality = drivers_info.Where(d => d.full_name.ToLower().Trim() == full_name.ToLower().Trim()).Select(x => x.country_code).First();
					var car_number = drivers_info.Where(d => d.full_name.ToLower().Trim() == full_name.ToLower().Trim()).Select(x => x.driver_number).First();
					var team = drivers_info.Where(d => d.full_name.ToLower().Trim() == full_name.ToLower().Trim()).Select(x => x.team_name).First();

					return new DriverResponse
					{
						error_message = "",
						full_name = full_name,
						points = (int)full_points,
						nationality = nationality,
						number = car_number,
						team = team
					};
				}
			}
			catch (Exception ex)
			{
				return new DriverResponse
				{
					error_message = ex.ToString(),
				};
			}
		}

		public async Task<List<DriverResponse>?> GetDriversStandingByYear(int year)
		{
			try
			{
				var session_races = await _httpClient.GetAsync<List<Session>>(_openf1APIs["SESSIONS"], $"year={year}");
				if (session_races == null || session_races.Count == 0)
				{
					return [];
				}
				var all_drivers = await _httpClient.GetAsync<List<Driver>>(_openf1APIs["DRIVERS"]);
				if (all_drivers == null ||  all_drivers.Count == 0) 
				{
					return [];
				}
				var race_session_keys = session_races.Select( x => x.session_key ).ToList();
				if (race_session_keys == null || race_session_keys.Count == 0)
				{
					return [];
				}
				var sessions_drivers = all_drivers.Where(x => race_session_keys.Contains(x.session_key)).ToList();
				if (sessions_drivers == null || sessions_drivers.Count == 0) 
				{
					return [];
				}

				var standings = new List<DriverResponse>();

				var all_results = await _httpClient.GetAsync<List<Position>>(_openf1APIs["SESSION_RESULT"], "", false, 300);
				foreach (var driver in sessions_drivers)
				{
					decimal points = 0;
					foreach (var race in session_races)
					{
						points += all_results?.Where( r => r.session_key == race.session_key )?.FirstOrDefault( r => r.driver_number == driver.driver_number)?.points ?? 0;
					}
					var driver_index = standings.FindIndex(existing_driver => existing_driver.full_name.ToLower().Trim() == driver.full_name.ToLower().Trim());

					if (driver_index == -1)
					{
						standings.Add(new DriverResponse
						{
							full_name = driver.full_name,
							points = points,
							nationality = driver.country_code,
							team = driver.team_name
						});
					}
					else
					{
						standings[driver_index].points += points;
					}
				}

				return standings.OrderByDescending( x => x.points).ToList();

			} 
			catch (Exception ex)
			{
				return [];
			}
		}



		//public async Task<APIResponse> DriversAtPosition(APIParameters par)
		//{
		//	var race_sessions = await _httpClient.GetAsync<List<Session>>(_openf1APIs["SESSIONS"], $"session_name={par.race_type}&year={par.year}&country_code={par.country_code}");
		//	if (race_sessions == null || race_sessions.Count == 0)
		//	{
		//		return new APIResponse { errorMessage = "No race sessions found for given parameters." };
		//	}

		//	var session_id = race_sessions?.Select(t => t.session_key).FirstOrDefault();
		//	if (session_id == null)
		//	{
		//		return new APIResponse { errorMessage = "No session ID found." };
		//	}

		//	var endRacePositions = await _httpClient.GetAsync<List<Position>>(_openf1APIs["SESSION_RESULT"], $"session_key={session_id}&position={par.position}");
		//	if (endRacePositions == null || endRacePositions.Count == 0)
		//	{
		//		return new APIResponse { errorMessage = $"No drivers found at position {par.position}." };
		//	}

		//	var winner_driverNumber = endRacePositions?.Select(t => t.driver_number).FirstOrDefault();
		//	if (winner_driverNumber == null)
		//	{
		//		return new APIResponse { errorMessage = "No driver number found at the specified position." };
		//	}

		//	var driver_name = Utility.GetNameByDriverNumber(winner_driverNumber);


		//	return new APIResponse
		//	{
		//		errorMessage = "",
		//		response = JsonSerializer.Serialize(new { driver_name })
		//	};
		//}



		//public async Task<List<Driver>?> DriversInfo(APIParameters par)
		//{
		//	var race_sessions = await _httpClient.GetAsync<List<Session>>(_openf1APIs["SESSIONS"], $"year={par.year}&country_code={par.country_code}");
		//	var session_id = race_sessions?.Select(t => t.session_key).FirstOrDefault();
		//	var drivers_info = await _httpClient.GetAsync<List<Driver>>(_openf1APIs["DRIVERS"], $"session_key={session_id}");
		//	return drivers_info?.OrderBy( x => x.team_name).ToList();
		//}

	}
}
