using Core.Configuration;
using Core.Logging;
using System;

namespace Core.Transformations
{
    public static class TimeValues
    {
		public static string ReturnNowTimeAsString(string format = "HH:mm")
		{
			DebugOutput.Log($"Proc - ReturnNowTimeAsString");
			DateTime now = DateTime.Now;
			var time = now.ToString(format);
			DebugOutput.Log($"sending back time as  {time}");
			return time;
		}

		// A method to check if two times are within a certain MINUTE range of each other
		public static bool IsTimeIsh(string time1, string time2, int rangeInMinutes)
		{
			try
			{
				// Parse both datetime strings
				if (!DateTime.TryParseExact(time1, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime1))
					return false;
				if (!DateTime.TryParseExact(time2, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime2))
					return false;
				// Calculate the absolute difference in minutes
				var timeDifference = Math.Abs((dateTime1 - dateTime2).TotalMinutes);
				// Check if within range
				return timeDifference <= rangeInMinutes;
			}
			catch
			{
				return false;
			}
		}



	}
}
