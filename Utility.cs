using System;
using System.Text.RegularExpressions;
namespace OpenF1Dashboard
{
	public static class Utility
	{
		public static int ReadInt(string message)
		{
			Console.Write(message);
			int value;
			while (!int.TryParse(Console.ReadLine(), out value))
			{
				Console.Write("Invalid number.");
			}
			return value;
		}

		public static string[] ReadStringAndSeparateBy(string message, char separator)
		{
			Console.Write(message);
			return Console.ReadLine()?.Split(separator) ?? [];
		}

		public static void DisplayMessage(string message)
		{
			Console.WriteLine($"\n{message}");
		}

		public static void DisplayMessageAndPressKey(string message)
		{
			Console.WriteLine(message);
			Console.ReadKey();
		}

		public static void DisplayMessages(List<string> messages)
		{
			foreach (var message in messages)
			{
				Console.WriteLine(message);
			}
		}

		

		public static string GetNameByDriverNumber(int? diver_number)
		{
			return diver_number switch
			{
				1 => "Max Verstappen",
				4 => "Lando Norris",
				5 => "Gabriel Bortoleto",
				6 => "Isack Hadjar",
				7 => "Jack Doohan",
				10 => "Pierre Gasly",
				12 => "Andrea Kimi Antonelli",
				14 => "Fernando Alonso",
				16 => "Charles Leclerc",
				18 => "Lance Stroll",
				22 => "Yuki Tsunoda",
				23 => "Alex Albon",
				27 => "Nico Hulkenberg",
				30 => "Liam Lawson",
				31 => "Esteban Ocon",
				44 => "Lewis Hamilton",
				55 => "Carlos Sainz",
				63 => "George Russell",
				81 => "Oscar Piastri",
				87 => "Oliver Bearman",
				_ => "Unknown Driver"
			};
		}
	}
}
