namespace OpenF1Dashboard.API.Models
{
	public class DriverResponse
	{
		public string error_message { get; set; }
		public string full_name { get; set; }
		public string nationality { get; set; }
		public int number { get; set; }
		public string team { get; set; }
		public decimal points { get; set; }
	}
}
