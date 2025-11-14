using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenF1Dashboard.Models
{
	public class Driver
	{
		public string broadcast_name { get; set; }
		public string country_code { get; set; }
		public int driver_number { get; set; }
		public string first_name { get; set; }
		public string full_name { get; set; }
		public string headshot_url { get; set; }
		public string last_name { get; set; }
		public int meeting_key { get; set; }
		public string name_acronym { get; set; }
		public int session_key { get; set; }
		public string team_colour { get; set; }
		public string team_name { get; set; }
	}
}
