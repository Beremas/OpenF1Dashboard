
namespace OpenF1Dashboard.Shared.Views
{
    public class QualiSummary
    {
        public List<double?>? LapTime { get; set; }
        public int? Position { get; set; }
    }

    public class RaceSummary
    {
        public int? Position { get; set; }
        public double? BestLapTime { get; set; }
        public string BestLapTimeFormatted
        {
            get
            {
                if (!BestLapTime.HasValue)
                    return "";

                TimeSpan ts = TimeSpan.FromSeconds(BestLapTime.Value);
                return $"{ts.Minutes}:{ts.Seconds:D2}.{ts.Milliseconds:D3}";
            }
        }
        public Dictionary<string, List<int>>? CompoundEndLap { get; set; }
        public List<double?>? LapTimes { get; set; }
        public bool Dnf { get; set; }
        public bool Dns { get; set; }
        public bool Dsq { get; set; }
        public int PosGained { get; set; }
        public int PosLost { get; set; }
        public int Overtakes { get; set; }
        public bool IsFinisher => !(Dnf || Dns || Dsq);
        public bool IsFastest { get; set; }
        public int? Pits { get; set; }
        public int? Stints { get; set; }
    }

    public class DriverSummary
    {
        public string? FullName { get; set; }
        public RaceSummary? Race { get; set; }     
        public QualiSummary? Qualifiying { get; set; }
       
    }

    public class AnalyzerSummary
    {
        public List<DriverSummary>? Drivers { get; set; }
        public DriverSummary? FastestDriver { get; set; }
        public string? CircuitGeojson { get; set; }
    }
}
