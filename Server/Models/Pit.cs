using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenF1Dashboard.Shared.Models
{
    public class Pit
    {
        public DateTime date { get; set; }
        public int driver_number { get; set; }
        public int? lap_number { get; set; }
        public int meeting_key { get; set; }
        public int session_key { get; set; }
        public double? pit_duration { get; set; }
    }
}
