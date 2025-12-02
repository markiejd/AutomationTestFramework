using Core.Configuration;
using Core.Logging;
using System;

namespace Core.Transformations
{
    public static class DateValues
	{
		/// <summary>
		/// Return the current date (as in when this is called/ran) in a format
		/// Using SQL FormatNumbers
		/// https://www.mssqltips.com/sqlservertip/1145/date-and-time-conversions-using-sql-server/
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ReturnNowDateAsString(string format = "101")
		{
			DebugOutput.Log($"Proc - ReturnNowDateAsString");

			// format = At some point we will want to override the default format 101 with a read from a config file
			DateTime now = DateTime.Now;
			var date = Format(now, format);
			DebugOutput.Log($"sending back date as  {date}");
			return date;
		}

		public static DateTime GetDateTime()
		{
			return DateTime.Now;
		}

		public static DateTime GetDateTimeUTC()
		{
			return DateTime.UtcNow;
		}

		public static DateTime? GetDateTimeFromParse(string text)
		{
			try
			{
				return DateTime.Parse(text);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Return the the first of the current month
		/// Using SQL FormatNumbers
		/// </summary>
		/// <param name="date"></param>
		/// <returns>01/*</returns>
		public static string ReturnFirstOfThisMonth(string format = "101")
		{
			DebugOutput.Log($"Proc - ReturnFirstOfThisMonth");
			DateTime now = DateTime.Now;
			DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
			var date = Format(firstDayOfMonth, format);
			DebugOutput.Log($"First of the month is {date}");
			return date;
		}

		public static String GetDateOnlyFromDateTime(DateTime dateTime)
		{
			DebugOutput.Log($"Proc - GetDateOnlyFromDateTime {dateTime}");
			var y = dateTime.ToString("dd/MMM/yyyy");
			return y;
		}

		public static string Get3MonthWordFromMonthNumber(DateTime date)
		{
			DebugOutput.Log($"Proc - GetMonthWordFromMonthNumber {date}");
			var monthNumber = date.Month;
			DebugOutput.Log($"Month number is {monthNumber}");
			// for some reason SEP is SEPT 
			if (monthNumber == 9) return "SEPT";
			var month = date.ToString("MMM");
			DebugOutput.Log($"Month is {month}");
			return month;
		}

		public static string GetFullMonthWordFromMonthNumber(DateTime date)
		{
			DebugOutput.Log($"Proc - GetFullMonthWordFromMonthNumber {date}");
			var month = date.ToString("MMMM");
			DebugOutput.Log($"Month is {month}");
			return month;
		}

		public static DateTime? GetDateTimeFromDateString(string text)
		{
			DebugOutput.Log($"Proc - GetDateTimeFromDateString {text}");
			DateTime parsedDate;
			try
			{
				// Specify the exact format and culture
				parsedDate = DateTime.ParseExact(text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
				return parsedDate;
			}
			catch
			{
				DebugOutput.Log($"FAILED to parse {text} using dd/MM/yyyy");
				return null;
			}					
		}

		public static DateTime? GetDateTimeFromDateString(string text, string format = "")
		{
			DebugOutput.Log($"Proc - GetDateTimeFromDateString {text} {format}");
			if (format == "") return GetDateTimeFromDateString(text);
			DateTime returnDateTime = DateTime.MinValue;
			var parsedDate = DateTime.TryParseExact(text, format, null, System.Globalization.DateTimeStyles.AssumeUniversal, out returnDateTime);
			if (!parsedDate) return null;
			return returnDateTime;
		}

		public static DateTime GetDateTimeFromStringX(string text)
		{
			DebugOutput.Log($"Proc - GetDateFromString {text}");
			text = text.ToLower();
			if (text.Contains("now"))
			{
				text = text.Replace("now", "today");
			}
			DebugOutput.Log($"GetDateFromString {text}");
			DateTime returnDate = DateTime.UtcNow;
			if (text.Contains("today"))
			{
					DebugOutput.Log($"USING TODAY (may have used NOW)");
				if (text.Contains("-"))
				{
					var delimitDate = StringValues.BreakUpByDelimitedToList(text,"-");
					var numberToSubtract = delimitDate[1];
					DebugOutput.Log($"NUMBER TO SUBTRACT = {numberToSubtract}");
					int howMuchToSubtract = 0;
					if (numberToSubtract.Contains("day") || numberToSubtract.Contains("days") || numberToSubtract.Contains("d"))
					{
						if (numberToSubtract.Contains("days")) numberToSubtract = numberToSubtract.Replace("days","");
						if (numberToSubtract.Contains("day")) numberToSubtract = numberToSubtract.Replace("day","");
						if (numberToSubtract.Contains("d")) numberToSubtract = numberToSubtract.Replace("d","");
						if (numberToSubtract.Contains(" ")) numberToSubtract = numberToSubtract.Replace(" ","");
						try
						{
							howMuchToSubtract = int.Parse(numberToSubtract);
							howMuchToSubtract = howMuchToSubtract * -1;
							DebugOutput.Log($"MODIFY BY DAYS {howMuchToSubtract}");
							returnDate = returnDate.AddDays(howMuchToSubtract);
						}
						catch
						{
							DebugOutput.Log($"FAILED TO CONVERT DATE {howMuchToSubtract}");
							return returnDate;
						}
					}
					else if (numberToSubtract.Contains("hour") || numberToSubtract.Contains("hours") || numberToSubtract.Contains("h"))
					{
						howMuchToSubtract = 0;
						if (numberToSubtract.Contains("hours")) numberToSubtract = numberToSubtract.Replace("hours","");
						if (numberToSubtract.Contains("hour")) numberToSubtract = numberToSubtract.Replace("hour","");
						if (numberToSubtract.Contains("h")) numberToSubtract = numberToSubtract.Replace("h","");
						if (numberToSubtract.Contains(" ")) numberToSubtract = numberToSubtract.Replace(" ","");
						try
						{
							howMuchToSubtract = int.Parse(numberToSubtract);
							howMuchToSubtract = howMuchToSubtract * -1;
							DebugOutput.Log($"DEFAU:T MODIFY BY HOURS {howMuchToSubtract}");
							returnDate = returnDate.AddHours(howMuchToSubtract);
						}
						catch
						{
							DebugOutput.Log($"FAILED TO CONVERT HOUR {howMuchToSubtract}");
							return returnDate;
						}
					}
					else if (numberToSubtract.Contains("minute") || numberToSubtract.Contains("mm") || numberToSubtract.Contains("m"))
					{
						if (numberToSubtract.Contains("minute")) numberToSubtract = numberToSubtract.Replace("minute","");
						if (numberToSubtract.Contains("mm")) numberToSubtract = numberToSubtract.Replace("mm","");
						if (numberToSubtract.Contains("m")) numberToSubtract = numberToSubtract.Replace("m","");
						if (numberToSubtract.Contains(" ")) numberToSubtract = numberToSubtract.Replace(" ","");
						try
						{
							howMuchToSubtract = int.Parse(numberToSubtract);
							howMuchToSubtract = howMuchToSubtract * -1;
							DebugOutput.Log($"MODIFY BY MINUTES {howMuchToSubtract}");
							returnDate = returnDate.AddMinutes(howMuchToSubtract);
						}
						catch
						{
							DebugOutput.Log($"FAILED TO CONVERT MINUTE {howMuchToSubtract}");
							return returnDate;
						}
					}
					else if (numberToSubtract.Contains("second") || numberToSubtract.Contains("ss") || numberToSubtract.Contains("s"))
					{
						if (numberToSubtract.Contains("second")) numberToSubtract = numberToSubtract.Replace("minute","");
						if (numberToSubtract.Contains("ss")) numberToSubtract = numberToSubtract.Replace("mm","");
						if (numberToSubtract.Contains("s")) numberToSubtract = numberToSubtract.Replace("m","");
						if (numberToSubtract.Contains(" ")) numberToSubtract = numberToSubtract.Replace(" ","");
						try
						{
							howMuchToSubtract = int.Parse(numberToSubtract);
							howMuchToSubtract = howMuchToSubtract * -1;
							DebugOutput.Log($"MODIFY BY MINUTES {howMuchToSubtract}");
							returnDate = returnDate.AddSeconds(howMuchToSubtract);
						}
						catch
						{
							DebugOutput.Log($"FAILED TO CONVERT MINUTE {howMuchToSubtract}");
							return returnDate;
						}
					}
					else
					{
							DebugOutput.Log($"DEFAU:T MODIFY BY DAYS {howMuchToSubtract}");
							howMuchToSubtract = int.Parse(numberToSubtract);
							howMuchToSubtract = howMuchToSubtract * -1;
							returnDate = returnDate.AddDays(howMuchToSubtract);
					}
				}
			}
			DebugOutput.Log($"RETURNING returnDate");
			return returnDate;
		}

		public static double GetSecondsBetweenDateTimes(DateTime dateTime1, DateTime dateTime2)
		{
			DebugOutput.Log($"GetSecondsBetweenDateTimes {dateTime1} {dateTime2}");
			var diffInSeconds = (dateTime1 - dateTime2).TotalSeconds;
			if (diffInSeconds < 0) diffInSeconds *= -1;
			return diffInSeconds;
		}

		public static bool IsTwoDateTimesWithinSeconds(DateTime dateTime1, DateTime dateTime2, double seconds)
		{
			DebugOutput.Log($"GetSecondsBetweenDateTimes {dateTime1} {dateTime2} {seconds}");
			var diffInSeconds = (dateTime1 - dateTime2).TotalSeconds;
			if (diffInSeconds < 0) diffInSeconds *= -1;
			if (diffInSeconds <= seconds) return true;
			DebugOutput.Log($"Was expecting {dateTime1} and {dateTime2} to be within {seconds} seconds but actually {diffInSeconds} seconds difference!");
			return false;
		}		

		/// <summary>
		/// True or False, is there a chance this is a date?
		/// </summary>
		/// <param name="text"></param>
		/// <returns>true if it contains a special char normally seen in dates</returns>
		private static bool DoesStringPossiblyContainDate(string text)
		{
			DebugOutput.Log($"Proc - DoesStringContainDate {text}");
			DebugOutput.Log($"A date must have a forward or backwards slash, or a hyphen or a dot");
			DebugOutput.Log($"Spaces are not possible, as that makes it multiple string sets!");
			bool success = false;
			if (text.Contains("-")) success = true;
			if (text.Contains(".")) success = true;
			if (text.Contains("/")) success = true;
			if (text.Contains(@"\")) success = true;
			return success;
		}

		/// <summary>
		/// Full Date and Time Supplied give dd, mm, yyyy, hh, mm, ss
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static List<string> BreakUpDateAndTime(string dateTime)
		{
			dateTime = ChangeDateDivider(dateTime,"-");
			DebugOutput.Log($"Proc - Format {dateTime}");
			var emptyDateBrokenUp = new List<string> ();
			string day = "";
			string month = "";
			string year = "";
			string hour = "";
			string minutes = "";
			string seconds = "";
			if (!dateTime.Contains($"-"))
			{
				DebugOutput.Log($"Your full date time MUST have its date as a forward slash be dd/mm/yy");
				return emptyDateBrokenUp;
			}
			if (!dateTime.Contains($" "))
			{
				DebugOutput.Log($"Your full date time MUST have its date and time seperated by a space dd-mm-yyyy hh:mm");
				return emptyDateBrokenUp;
			}
			if (!dateTime.Contains($":"))
			{
				DebugOutput.Log($"Your full date time MUST have its time as HH:MM:SS");
				return emptyDateBrokenUp;
			}
			var delimitedByHyphen = StringValues.BreakUpByDelimited(dateTime, "-");
			if (delimitedByHyphen.Count() != 3)
			{
				DebugOutput.Log($"WE split up {dateTime} by - and expected 3 parts, but got {delimitedByHyphen.Length} parts!");
				return emptyDateBrokenUp;
			}
			if (delimitedByHyphen[0].Length > 2)
			{
				DebugOutput.Log($"We have a date of {delimitedByHyphen[0]} that can't be right");
				return emptyDateBrokenUp;
			}
			if (delimitedByHyphen[0].Length < 2)
			{
				DebugOutput.Log($"We have a date of {delimitedByHyphen[0]} we going to add 0 to it!");
				day = "0" + delimitedByHyphen[0];
			}
			else
			{
				day = delimitedByHyphen[0];
			}
			if (delimitedByHyphen[1].Length > 2)
			{
				DebugOutput.Log($"We have a date of {delimitedByHyphen[1]} that can't be right");
				return emptyDateBrokenUp;
			}
			if (delimitedByHyphen[1].Length < 2)
			{
				DebugOutput.Log($"We have a date of {delimitedByHyphen[1]} we going to add 0 to it!");
				month = "0" + delimitedByHyphen[1];
			}
			else
			{
				month = delimitedByHyphen[1];
			}
			DebugOutput.Log($"WE have DAY of {delimitedByHyphen[0]} and a MONTH of {delimitedByHyphen[1]}");
			var yearAndTimeDelimited = StringValues.BreakUpByDelimited(delimitedByHyphen[2]," ");
			DebugOutput.Log($"We delimited the rest and got year of {yearAndTimeDelimited[0]} and the time of {yearAndTimeDelimited[1]}");
			if (yearAndTimeDelimited[0].Length < 4)
			{
				DebugOutput.Log($"We want YYYY but you gave us {yearAndTimeDelimited[0]}");
				if (yearAndTimeDelimited[0].Length == 2)
				{
					var newValue = yearAndTimeDelimited[0];
					yearAndTimeDelimited[0] = "20" + newValue;
				}
			}
			if (yearAndTimeDelimited[0].Length != 4)
			{
				DebugOutput.Log($"Failed to fix the date!");
				return emptyDateBrokenUp;
			}
			year = yearAndTimeDelimited[0];
			DebugOutput.Log($"We have gotten the YEAR as {year}");
			var timeDelimited = StringValues.BreakUpByDelimited(yearAndTimeDelimited[1],":");
			if (timeDelimited.Length > 2)
			{
				DebugOutput.Log($"You have supplied seconds, that is NICE!");
				seconds = timeDelimited[2];
			}
			else
			{
				seconds = "00";
			}
			if (timeDelimited[0].Length < 2)
			{
				DebugOutput.Log($"Hour CAN be single digit");
				timeDelimited[0] = "0" + timeDelimited[0];
			}
			hour = timeDelimited[0];
			if (timeDelimited[1].Length < 2)
			{
				DebugOutput.Log($"Minutes CAN be single digit");
				timeDelimited[1] = "0" + timeDelimited[1];
			}
			minutes = timeDelimited[1];

			DebugOutput.Log($"Completed! {day} {month} {year} {hour} {minutes} {seconds}");
			var FinishedTimeList = emptyDateBrokenUp;
			FinishedTimeList.Add(day);
			FinishedTimeList.Add(month);
			FinishedTimeList.Add(year);
			FinishedTimeList.Add(hour);
			FinishedTimeList.Add(minutes);
			FinishedTimeList.Add(seconds);
			return FinishedTimeList;
		}

		/// <summary>
		/// Format the date supplied in a given format
		/// https://www.mssqltips.com/sqlservertip/1145/date-and-time-conversions-using-sql-server/
		/// </summary>
		/// <param name="dateSupplied"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(DateTime dateSupplied, string format = "101")
		{
			DebugOutput.Log($"Proc - Format {dateSupplied} {format}");
			string date = "";

			switch (format)
			{
				case "1":
				default:
					{
						date = dateSupplied.ToString("MM/dd/yy");
						break;
					}
				case "2":
					{
						date = dateSupplied.ToString("yy/MM/dd");
						break;
					}
				case "23":
					{
						date = dateSupplied.ToString("yyyy-MM-dd");
						break;
					}
				case "101":
					{
						date = dateSupplied.ToString("MM/dd/yyyy");
						break;
					}
				case "103":
					{
						date = dateSupplied.ToString("dd/MM/yyyy");
						break;
					}
				case "104":
					{
						date = dateSupplied.ToString("dd.MM.yyyy");
						break;
					}
			}

			return date;
		}

		/// <summary>
		/// Use maths to a date and return the date as a string
		/// </summary>
		/// <param name="number"></param>
		/// <param name="maths"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string MathsToDate(string number, string maths, string format = "0")
		{
			DebugOutput.Log($"Proc - MathsToDate {number} {maths} {format}");
			if (format == "0")
            {
				format = GetDateFormat();
			}
			double doubleDays;
			var days = double.TryParse(number, out doubleDays);
			var date = "";

			if (maths == "+")
			{
				var newDate = DateTime.Now.AddDays(doubleDays);
				DebugOutput.Log($"NEW DATE UNFORMATED = '{newDate}'");
				date = Format(newDate, format);
				DebugOutput.Log($"DATE FORMATED = '{date}'");
			}
			else
            {
				doubleDays = doubleDays * -1;
				date = Format(DateTime.Now.AddDays(doubleDays), format);
			}

			return date;
		}


		/// <summary>
		/// What is the current date format? Returning SQL Number
		/// </summary>
		/// <param name="None"></param>
		/// <returns>SQL number in a string</returns>
		public static string GetDateFormat()
        {
			var country = TargetConfiguration.Configuration.DateFormat;
			switch(country.ToLower())
            {
				default:
				case "uk":
                    {
						return "103";
                    }
            }

        }


		/// <summary>
		/// Turn a string date around
		/// </summary>
		/// <param name="date"></param>
		/// <returns>Swap the dd/mm/yyyy into mm/dd/yyyy ruddy yanks</returns>
		public static string TurnStringDateAround(string date)
		{
			DebugOutput.Log($"Proc - TurnStringDateAround {date}");
			DebugOutput.Log($"Some dates are right, but wrong...  dd/mm/yyyy changed to mm/dd/yyyy");
			string dd = date.Substring(0, 2);
			DebugOutput.Log(dd);
			string mm = date.Substring(3, 2);
			DebugOutput.Log(mm);
			string yyyy = date.Substring(6, 4);
			DebugOutput.Log(yyyy);
			string newDate = $"{mm}/{dd}/{yyyy}";
			DebugOutput.Log($"changed {date} to {newDate}");
			return newDate;
		}


		/// <summary>
		/// Get the long of current seconds since 01/01/1970 00:00:00
		/// </summary>
		/// <param name="none"></param>
		/// <returns>Long number of seconds since EPOCH</returns>
		public static long GetTimeInUnix()
		{
			DebugOutput.Log($"Returing UNIX time (seconds from 1st Jan 1970");
			DateTime foo = DateTime.Now;
			long unixTime = GetTimeInUnix(foo); 
			return unixTime;
		}

		public static long GetTimeInUnix(DateTime date)
		{
			DebugOutput.Log($"Returing UNIX time (seconds from 1st Jan 1970 for {date.ToString()}");
			long unixTime = ((DateTimeOffset)date).ToUnixTimeSeconds(); 
			return unixTime;
		}

		public static long GetStaticUnix(string period)
		{
			DebugOutput.Log($"Get the number of seconds of {period}");
			switch(period.ToLower())
			{
				default: return 1;
				case "min": return 60;
				case "minute": return 60;
				case "hour": return 3600;
				case "day": return 86400;
				case "week": return 604800;
				case "year": return 31536000;
			}
		}

		/// <summary>
		/// Take a date and change the divider - like a / to a . or visa versa
		/// </summary>
		/// <param name="date"></param>
		/// <param name="changeTo"></param>
		/// <returns></returns>
		public static string ChangeDateDivider(string date, string changeTo)
		{
			var returned = "";

			if (date.Contains("/"))
			{
				returned = date.Replace("/", changeTo);
			}

			if (date.Contains("."))
			{
				returned = date.Replace(".", changeTo);
			}

			return returned;
		}

	}
}
