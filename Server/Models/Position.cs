using System;

namespace OpenF1Dashboard.Shared.Models
{
    public class Position
    {
        public DateTime date { get; set; }
        public int driver_number { get; set; }
        public int meeting_key { get; set; }
        public int position { get; set; }
        public int session_key { get; set; }
    }
}
