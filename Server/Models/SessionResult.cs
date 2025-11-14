using OpenF1Dashboard.Server.Utilities;
using System.Text.Json.Serialization;

namespace OpenF1Dashboard.Shared.Models
{
    public class SessionResult
    {
        public int? position { get; set; }
        public int driver_number { get; set; }
        public int? number_of_laps { get; set; }
        public decimal? points { get; set; }
        public bool dnf { get; set; }
        public bool dns { get; set; }
        public bool dsq { get; set; }

        [JsonConverter(typeof(RaceOrQualiConverter))]
        public List<double?>? gaps_to_leader { get; set; }
        [JsonConverter(typeof(RaceOrQualiConverter))]
        public List<double?>? durations { get; set; }
        public int? meeting_key { get; set; }
        public int? session_key { get; set; }
    }
}
