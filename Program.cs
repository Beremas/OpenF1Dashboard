using OpenF1Dashboard.API;
using OpenF1Dashboard.Enum;
using OpenF1Dashboard.Models;
using System;
using System.Threading.Tasks;

namespace OpenF1Dashboard
{
	class Program
	{
		static async Task Main()
		{
			var f1Client = new DriverController(); 
			int selection = -1;

			while (selection != 4)
			{
				Console.Clear();
				ShowMenu();

				int input = Utility.ReadInt("\nSelect an option (1 to 4): ");
				selection = input;
				if (selection >= 1 && selection <= 4)
				{
					Console.Clear();
					switch (selection)
					{
						case 1:
							await ShowDriversAtPosition(f1Client);
							break;
						case 2:
							await ShowDriversStanding(f1Client);
							break;
						case 3:
							await ShowWorldDriverChampion(f1Client);
							break;
						case 4:
							await ShowDriversList(f1Client);
							break;
						case 9:
							break;
					}
				}
				else
				{
					Utility.DisplayMessage("Invalid input. Please enter a number between 1 and 4.");
				}

				if (selection != 9)
				{
					Utility.DisplayMessageAndPressKey("\nPress any key to return to the menu...");
				}
			}
		}

		static void ShowMenu()
		{

			Utility.DisplayMessages([
				"===== F1 Statistics Menu =====",
				//"1. Driver chequed flag position",
				"2. Drivers' standing", // rework this point to make => Drivers' standing 
				"3. WDC for year",
				//"4. Drivers list for year",
				"9. Exit"
			]);
		}
		static async Task ShowWorldDriverChampion(DriverController f1Client)
		{
			int year = Utility.ReadInt("Enter the year: ");

			var response = await f1Client.GetWorldDriverChampionInfoByYear(year);

			string message;
			if (response.error_message == "")
			{
				message = $"The WDC of the {year} was {response.full_name} ({response.nationality}), driving for {response.team}, scored {response.points} points with car number {response.number}.";
			}
			else
			{
				message = response.error_message;
			}

			Utility.DisplayMessage(message);
		}


		static async Task ShowDriversStanding(DriverController f1Client)
		{
			var year = Utility.ReadInt("Enter the year: ");

			var standings = await f1Client.GetDriversStandingByYear(year);

			Console.WriteLine($"Driver	-	nationality	-	points	-	team");
			for (int i = 0; i < standings?.Count; i++)
			{
				Console.WriteLine($"{standings[i].full_name}\t{standings[i].nationality}\t{standings[i].points}\t{standings[i].team}\t");
			}
		}


		static async Task ShowDriversAtPosition(DriverController f1Client)
		{
			var input = Utility.ReadStringAndSeparateBy("Enter: year, country-code, race-type, position (comma separated)", ',');
			if (input.Length != 4
				|| !int.TryParse(input[0].Trim(), out int year)
				|| RaceType.TryParse<RaceType>(input[2], ignoreCase: true, out var race_type)
				|| !int.TryParse(input[3].Trim(), out int position))
			{
				Utility.DisplayMessage("Invalid input.");
				return;
			}
			string country_code = input[1].Trim();

			//var result = await f1Client.DriversAtPosition(year,country_code,race_type,position);

			//Utility.DisplayMessage($"Drivers at position {position} in {country_code} ({year}): {result}");
		}


		static async Task ShowDriversList(DriverController f1Client)
		{
			var input = Utility.ReadStringAndSeparateBy("Enter: year, country code (comma separated)",',');
			if (input.Length != 2 || !int.TryParse(input[0].Trim(), out int year))
			{
				Utility.DisplayMessage("Invalid input.");
				return;
			}
			string country_code = input[1].Trim();

			//List<Driver>? driverInfo = await f1Client.DriversInfo(year,country_code);

			//if (driverInfo == null || driverInfo.Count == 0)
			//{
			//	Utility.DisplayMessage($"No drivers info for year {year} in country {country_code}");
			//}
			//else
			//{
			//	foreach(var driver in driverInfo)
			//	{
			//		Utility.DisplayMessage($"{driver.full_name} - {driver.team_name} - {driver.country_code} - {driver.driver_number}");
			//	}
			//}

		}
	}
}
