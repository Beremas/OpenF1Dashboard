namespace OpenF1Dashboard.Server.Models
{
    public class Overtake
    {
        public DateTime date { get; set; }
        public int meeting_key { get; set; }
        public int overtaken_driver_number { get; set; }
        public int overtaking_driver_number { get; set; }
        public int position { get; set; }
        public int session_key { get; set; }
    }
}
