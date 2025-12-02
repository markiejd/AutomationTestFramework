using Core.Configuration;
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Transformations
{
    public static class Dates
    {
        public static String GetRandomBirthDateForAges(int minimumAge = 18, int maximumAge = 99, string division = "-")
		{
			DebugOutput.Log($"Proc - GetRandomDate {minimumAge} {maximumAge}");
			var actualAge = Numbers.GetRandomNumberBetween(minimumAge, maximumAge);
			var todayDate = DateTime.Now.Year;
			var birthYear = todayDate - actualAge;
			var birthYearString = birthYear.ToString();

			var month = Numbers.GetRandomNumberBetween(1, 12);
			var monthString = month.ToString();
			if (month < 10)
			{
				monthString = "0" + monthString;
			}

			var day = Numbers.GetRandomNumberBetween(1, 28);
			var dayString = day.ToString();
			if (day < 10)
			{
				dayString = "0" + dayString;
			}
			var returnString = birthYearString + division + monthString + division + dayString;
			DebugOutput.Log($"RETURNING RANDOM BIRTHDAY OF {returnString}");
			return returnString;
		}

		public static String GetRandomDateBetween(int startYear, int endYear)
		{
			DebugOutput.Log($"Proc - GetRandomDateBetween {startYear} {endYear}");
			var todayYear = DateTime.Now.Year;
			var maxAge = todayYear - endYear;
			var minAge = todayYear - startYear;

			return GetRandomBirthDateForAges(minAge, maxAge);
		}

    }
}
