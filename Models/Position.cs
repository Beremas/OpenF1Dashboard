using OpenF1Dashboard.CustomConverter;
using System.Text.Json.Serialization;

namespace OpenF1Dashboard.Models
{
	public class Position
	{
		public bool dnf {  get; set; }
        public bool dns { get; set; }
        public bool dsq { get; set; }
        public int driver_number {  get; set; }
		[JsonConverter(typeof(FlexibleDecimalListConverter))]
		public List<decimal?> duration {  get; set; }

		[JsonConverter(typeof(FlexibleDecimalListConverter))]
		public List<decimal?> gap_to_leader { get; set; }
        public int? number_of_laps { get; set; }
        public int? meeting_key { get; set; }
        public int? position {  get; set; }
        public int? session_key {  get; set; }
        public decimal? points { get; set; }
	}
}
