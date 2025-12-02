using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Transformations
{
    public static class StringValues
    {
		/// <summary>
		/// Given Text we will replace certain TEXT with other 
		/// </summary>
		/// <param name="value"></param>
		/// <returns>text supplied changed</returns>
		public static string TextReplacementService(string value)
        {
			DebugOutput.Log($"TextReplacementService {value}");
			if (value.Contains("ATFVARIABLE"))
			{
				// find the next char after VARIABLE don't use REGEX as we want the actual text
				var regex = new Regex(@"ATFVARIABLE(\d+)");
				var match = regex.Match(value);
				if (match.Success)
				{
					var variableNumber = match.Groups[1].Value;
					DebugOutput.Log($"We have found ATFVARIABLE {variableNumber}");
					// convert variableNumber to int and get the value from the TargetConfiguration.Configuration.ATFVariableArray
					int variableCounter = -1;
					if (int.TryParse(match.Groups[1].Value, out int parsedNumber) && parsedNumber >= 0 && parsedNumber <= 9)
					{
						variableCounter = parsedNumber;
						DebugOutput.Log($"Converted ATFVARIABLE to number: {variableNumber}");
						// get this value from the TargetConfiguration.Configuration.ATFVariableArray
						var replacement = TargetConfiguration.Configuration.ATFVariableArray[variableCounter];
						// in the orginal text 'value' replace VARIABLEx with the value from the array, even if null
						value = value.Replace($"ATFVARIABLE{variableNumber}", replacement);
					}
					else
					{
						DebugOutput.Log($"Invalid ATFVARIABLE number: {match.Groups[1].Value} we only do 0 to 9 inclusive.  keeping ATFVARIABLE as is.");
					}
				}
			}
			if (value.Contains("MYREPO"))
				{
					var repoDir = FileUtils.GetRepoDirectory();
					value = value.Replace("MYREPO", repoDir);
				}
			if (value.Contains("%APPDATA"))
			{
				var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				value = value.Replace("%APPDATA", appData);
			}
			if (value.Contains("<DATEACTION>"))
			{
				//103 is dd/MM/yyyy
				var dateChange = DateValues.ReturnNowDateAsString("103");
				value = value.Replace("<DATEACTION>", dateChange);
			}
			if (value.Contains("DATEACTION"))
            {
				//103 is dd/MM/yyyy
				var dateChange = DateValues.ReturnNowDateAsString("103");
				value = value.Replace("DATEACTION", dateChange);
			}
			if (value.Contains("<DATEREVERSE>"))
			{
				//23 is yyyy-mm-dd
				var dateChange = DateValues.ReturnNowDateAsString("23");
				value = value.Replace("<DATEREVERSE>", dateChange);
			}
			if (value.Contains("DATEREVERSE"))
			{
				//23 is yyyy-mm-dd
				var dateChange = DateValues.ReturnNowDateAsString("23");
				value = value.Replace("DATEREVERSE", dateChange);
			}
			if (value.Contains("<DATEWARRANT>"))
            {
				var dateChange = DateValues.ReturnFirstOfThisMonth("23");
				value = value.Replace("<DATEWARRANT>", dateChange);
			}
			if (value.Contains("DATEWARRANT"))
			{
				var dateChange = DateValues.ReturnFirstOfThisMonth("23");
				value = value.Replace("DATEWARRANT", dateChange);
			}
			if (value.Contains("<EPOCH>"))
			{
				var featureEpoch = EPOCHControl.Epoch;
				DebugOutput.Log($"Replacing EPOCH with Epoch number {featureEpoch} ");
				value = value.Replace("<EPOCH>", featureEpoch);
			}
			if (value.Contains("EPOCH"))
            {
				var featureEpoch = EPOCHControl.Epoch;
				if (featureEpoch == null) featureEpoch = "000001";
				if (value.Contains("|"))
				{
					string[] brokenUp = value.Split("|");					
					DebugOutput.Log($"We don't want the WHOLE of the EPOCH");
					int number = 0;
					try
					{
						number = Int32.Parse(brokenUp[1]);
						DebugOutput.Log($"We only want the last {number} chars of EPOCH ");
						var toBeReturned = featureEpoch.Substring(featureEpoch.Length - number);
						if (toBeReturned != null)
						{
							return toBeReturned;
						} 
					}
					catch
					{
						DebugOutput.Log($"Tried to break up EPOCH - FAILED!");
					}
				}
				DebugOutput.Log($"Replacing EPOCH with Epoch number {featureEpoch} ");
				value = value.Replace("EPOCH", featureEpoch);
			}
			if (value.Contains("FIRSTOFMOTH") || value.Contains("FIRSTOFTHEMONTH"))
			{
				var date = DateValues.ReturnFirstOfThisMonth("103");
				DebugOutput.Log($"Replacing FIRSTOFTHEMONTH with {date} ");
				return date;
			}
			if (value.Contains("NOW"))
			{
				if (value.Contains("ISH"))
				{
					var timeChange = TimeValues.ReturnNowTimeAsString();
					value = value.Replace("NOW", timeChange);					
				}
				else
				{
					var timeChange = TimeValues.ReturnNowTimeAsString();
					value = value.Replace("NOW", timeChange);
				}
			}
			if (value.Contains("CURRENTHOUR"))
			{
				var timeChange = TimeValues.ReturnNowTimeAsString("HH");
				value = value.Replace("CURRENTHOUR", timeChange);
			}
			if (value.Contains("CURRENTMINUTE"))
			{
				var timeChange = TimeValues.ReturnNowTimeAsString("mm");
				value = value.Replace("CURRENTMINUTE", timeChange);
			}
			if (value.Contains("TODAY"))
            {
				var countryCode = "101";
				var dateFormatCountry = TargetConfiguration.Configuration.DateFormat;
				if (dateFormatCountry == "UK") countryCode = "103";
				if (value.Contains("+") || value.Contains("-"))
                {
					DebugOutput.Log($"We have some maths to do!");
					bool plus = false;
					if (value.Contains("+")) plus = true;
					/// splits TODAY+10 into TODAY and 10,  TOMORROW-2 into TOMORROW and 2
					string[] brokenUpText;
					string returnDate;
					if (plus)
                    {
						brokenUpText = value.Split("+");
						returnDate = DateValues.MathsToDate(brokenUpText[1], "+");
					}
					else
					{
						brokenUpText = value.Split("-");
						returnDate = DateValues.MathsToDate(brokenUpText[1], "-");
					}
					value = returnDate;
					return returnDate;
				}
				var dateChange = DateValues.ReturnNowDateAsString(countryCode);
				value = value.Replace("TODAY", dateChange);
			}
			DebugOutput.Log($"Size = {value.Length}");
			if (value.Length < 1000)
			{
				DebugOutput.Log($"Returning after TextReplacement {value}");
			}
			return value;
        }
		
		public static string[] ConvertCSVStringToArray(string csvString)
		{
			DebugOutput.Log($"Proc - ConvertCSVStringToArray {csvString}");
			if (string.IsNullOrEmpty(csvString))
			{
				return Array.Empty<string>();
			}
			// Split the string by commas, keeping empty entries
			var result = csvString.Split(new[] { ',' }, StringSplitOptions.None);
			// Trim each entry in the resulting array
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = result[i].Trim();
			}
			return result;
		}

		/// <summary>
		/// Take sime string, and capalize all words *must have spaces!
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string CapitalizeWords(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			string[] words = input.Split(' ');
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i].Length > 0)
				{
					words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
				}
			}
			return string.Join(" ", words);
		}	

		/// <summary>
		///     Take a value string and change everything to lowercase
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetAllCharToLower(string value)
		{
			DebugOutput.Log($"Proc - GetAllCharToLower {value}");
			return string.IsNullOrEmpty(value) ? string.Empty : value.ToLower();
		}
		

		public static string? GetLastNumberOfCharsFromAString(string value, int lastNumberOfChars, bool inclusive = true)
		{
			DebugOutput.Log($"Proc - GetLastNumberOfCharsFromAString {value} {lastNumberOfChars} {inclusive} ");
			if (value.Length > lastNumberOfChars)
			{
				var left = value.Length - lastNumberOfChars;
				DebugOutput.Log($"Not enough chars in {value} will return {left} chars!");
				return null;
			}
			var result = value.Substring(value.Length - lastNumberOfChars);
			return result;
		}


		public static string ProjectSpecificTextChanges(string value)
		{
			DebugOutput.Log($"Proc - ProjectSpecificTextChanges {value}");
			return "";
        }


		/// <summary>
		/// Convert a list into a delimited string
		/// </summary>
		/// <param name="number"></param>
		/// <returns>string of all items in a list</returns>
		public static string ListToString(List<string> listIn, string delimiter = "|")
		{
			DebugOutput.Log($"Proc - ListToString ");
			var returnString = "";
			int counter = 0;
			foreach (var item in listIn)
			{
				if (counter == 0)
				{
					returnString = item;
					counter++;
				}
				else
				{
					returnString = delimiter + item;
				}
			}
			DebugOutput.Log($"returing '{returnString}'");
			return returnString;
		}

		/// <summary>
		/// count how many times a sub string is found in a long string
		/// </summary>
		/// <param name="number"></param>
		/// <returns>the count of how many!</returns>
		public static int HowManyTimesSubStringInString(string longString, string subString)
		{
			int n = new Regex(Regex.Escape(subString)).Matches(longString).Count;
			DebugOutput.Log($"WE have {n} matches");
			return n;
		}

		/// <summary>
		/// Add tabs (5 spaces) to BEFORE a string
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static string Tabs(int number, string message)
		{
            string tabs = "";
            for (int i = 0; i < number; i++)
            {
                tabs = tabs + "     ";
            }
            return tabs + message;
        }

		/// <summary>
		/// take a string and replace all the slahses with what you want to replace with
		/// </summary>
		/// <param name=""></param>
		/// <returns>New String slashes replaceD</returns>
		public static string ReplaceAllSlashes(string value, string replaceWith)
        {
			value = ReplaceAllForwardSlashes(value, replaceWith);
			value = ReplaceAllBackSlashes(value, replaceWith);
			return value;
        }

		public static string ReplaceAllBackSlashes(string value, string replaceWith)
		{
			//DebugOutput.Log($"Proc - ReplaceAllForwardSlashes {value} {replaceWith}");
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value.Replace(@"\", replaceWith);
		}

		public static string ReplaceAllForwardSlashes(string value, string replaceWith)
		{
			//DebugOutput.Log($"Proc - ReplaceAllForwardSlashes {value} {replaceWith}");
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value.Replace(@"/",replaceWith);
		}

		/// <summary>
		/// Take a string and remove any hidden HTML chars and return
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveHtmlFromEnd(string value)
		{
			//DebugOutput.Log($"Proc - RemoveHtmlFromEnd {value}");

			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			value = Regex.Replace(value, @"<[^>]+>|&nbsp|\n;", "").Trim();

			return value;
		}

		/// <summary>
		/// Certain chars are fine on screen, but in comparison.. BAD
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveReserveredChars(string value)
		{

			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			value = value.Replace("$", "");
			value = value.Replace("-", "");
			value = value.Replace("(", "");
			value = value.Replace(")", "");
			value = value.Replace("%", "");
			value = value.Replace(" ", "");
			return value;
		}

		/// <summary>
		/// Remove everything bar upper and lower case chars and numbers!
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveAllNonAlphaNumericChars(string value)
		{

			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			value = Regex.Replace(value, "[^a-zA-Z0-9]", String.Empty);
			return value;
		}

		/// <summary>
		/// Certain chars are fine on screen, but in comparison.. BAD
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveReserveredCharsExceptSpace(string value)
		{

			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			value = value.Replace("$", "");
			value = value.Replace("-", "");
			value = value.Replace("(", "");
			value = value.Replace(")", "");
			value = value.Replace("%", "");
			value = value.Replace("\"", "");
			return RemoveHtmlFromEnd(value);
		}

		public static string RemoveRequiredFieldAstrixFromHeader(string value)
		{
			DebugOutput.Log($"Proc - RemoveRequiredFieldAstrixFromHeader {value}");
			value = value.Replace(" *", "");
			return value;
		}

		/// <summary>
		/// Take a string and remove all spaces from it.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveAllSpaces(string value)
		{
			DebugOutput.Log($"Proc - RemoveAllSpaces {value}");
			var returnedValue = value.Replace(" ", "");
			return returnedValue;
		}

		/// <summary>
		/// Remove number chars from a string.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public static string RemoveLastNChars(string value, int number)
		{
			DebugOutput.Log($"Proc - RemoveLastNChars {value}");
			return string.IsNullOrEmpty(value) ? value : value.Substring(0, value.Length - number);
		}

		/// <summary>
		/// pass in string using bang ! as delimited and replace with numbers
		/// </summary>
		/// <param name="xpath"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string FillDynamicXPathWildcardBang(string xpath, params int[] values)
		{
			DebugOutput.Log($"Proc - FillDynamicXPathWildcardBang {xpath}");
			const char delimiter = '!';
			return BreakStringUpByDelmited(xpath, delimiter, values);
		}

		/// <summary>
		/// pass in a string using astrix * as demited and replace with numbers
		/// </summary>
		/// <param name="xpath"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string FillDynamicXPathWildcards(string xpath, params int[] values)
		{
			DebugOutput.Log($"Proc - FillDynamicXPathWildcardBang {xpath}");
			const char delimiter = '*';
			DebugOutput.Log($"Proc - FillDynamicXPathWildcards {xpath} with {delimiter} ");
			return BreakStringUpByDelmited(xpath, delimiter, values);
		}

		public static string[] BreakUpByDelimited(string value, string delimiter)
		{
			DebugOutput.Log($"Proc - BreakUpByDelimited {value} with {delimiter} ");
			return value.Split(delimiter);
		}

		public static string[] BreakUpStringByLine(string value)
		{
			DebugOutput.Log($"Proc - BreakUpStringByLine {value} ");
			string[] lines = value.Split(new string[] { "\n" }, StringSplitOptions.None);
			return lines;
		}

		public static string? ReturnIntAsStringWithPaddingZeros(int number, int howManyChars)
		{
            DebugOutput.Log($"ReturnIntAsStringWithSoManyChars {number} {howManyChars}");
			if (number < 0)
			{
				DebugOutput.Log($"Can not pad negative numbers!");
				return "";
			}
			return number.ToString().PadLeft(howManyChars, '0');
		}

		public static string? GetTextInCase(string value, string textCase = "camel")
		{
			textCase = textCase.ToLower();
			DebugOutput.Log($"GetTextInCase {value} {textCase}");
            if (textCase.ToLower() == "lower") return value.ToLower();
            if (textCase.ToLower() == "upper") return value.ToUpper();
			if (textCase.ToLower() == "camel")
			{
				var returnText = "";
				var brokenUpText = BreakUpByDelimitedToList(value, " ");
				foreach (var word in brokenUpText)
				{
					var firstLetter = word.Substring(0,1);
					var restOfWord = word.Substring(1,word.Length-1);
					firstLetter = firstLetter.ToUpper();
					returnText = firstLetter + restOfWord;
					return returnText;
				}
			}
			DebugOutput.Log($"Failed to do case!");
			return null;
		}

		public static List<string> BreakUpStringByNewLineToList(string value)
		{
            DebugOutput.Log($"BreakUpByNewLineToList {value} ");
			var array = BreakUpStringByLine(value);
			var oneValueList = new List<string>();
			foreach (var item in array)
			{
				oneValueList.Add(item);
			}
			return oneValueList;
		}

			
		public static List<string> BreakUpByDelimitedToListComma(string value, string delimiter = ",")
		{
			var oneValueList = new List<string>();
			if (string.IsNullOrEmpty(value))
			{
				return oneValueList;
			}

			// Regular expression to split by delimiter but ignore delimiters inside quotes
			string pattern = $"(?<=^|{delimiter})(\"(?:[^\"]|\"\")*\"|[^{delimiter}]*)";

			var matches = Regex.Matches(value, pattern);
			foreach (Match match in matches)
			{
				string item = match.Value.Trim();
				if (item.StartsWith("\"") && item.EndsWith("\""))
				{
					// Remove surrounding quotes and replace escaped quotes
					item = item.Substring(1, item.Length - 2).Replace("\"\"", "\"");
				}
				oneValueList.Add(item);
			}
			return oneValueList;
		}
		

		public static List<string> BreakUpByDelimitedToList(string value, string delimiter = "|")
		{
            DebugOutput.Log($"BreakUpByDelimitedToList {value} {delimiter}");
			var oneValueList = new List<string>();
			if (!value.Contains(delimiter))
			{
				oneValueList.Add(value);
				return oneValueList;
			}
			var arrayReturn = BreakUpByDelimited(value, delimiter);
			return ConvertArrayToList(arrayReturn);
		}

		public static bool? ConvertStringToBool(string boolAsAString)
		{
			DebugOutput.Log($"ConvertStringToBool {boolAsAString}");
			if (boolAsAString.ToLower() == "true") return true;
			if (boolAsAString.ToLower() == "false") return false;
			DebugOutput.Log($"Unable to parse '{boolAsAString}'");
			return null;
		}

		public static int? ConvertStringToInt(string intAsAString)
		{
            DebugOutput.Log($"ConvertStringToInt {intAsAString}");
			try
			{
				int result = Int32.Parse(intAsAString);
				return result;
			}
			catch (FormatException)
			{
				DebugOutput.Log($"Unable to parse '{intAsAString}'");
				return null;
			}
		}

		public static List<string> ConvertArrayToList(string[] array)
        {
            DebugOutput.Log($"ConvertArrayToList {array.Count()}");
            List<string> returnList = new List<string>();
            foreach (var item in array)
            {
                returnList.Add(item);
            }
            return returnList;
        }

		public static IEnumerable<string> SplitIntoParagraphs(string text)
		{
			return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string? GetStringOfLengthFromInt( int number, int stringLength)
		{
            DebugOutput.Log($"GetStringOfLengthFromInt {number} {stringLength}");
			if (number < 0)
			{
				DebugOutput.Log($"Can not have leading zeros of a negative number");
				return null;
			}
			int digits = 1;
			if (number < 10) digits = 1;
			else if (number < 100) digits = 2;
			else if (number < 1000) digits = 3;
			else if (number < 10000) digits = 4; /// 9999  and want 0009999 (string length of 7)
			else if (number < 100000) digits = 5;
			else if (number < 1000000) digits = 6;
			else if (number < 10000000) digits = 7;
			else if (number < 100000000) digits = 8;  
			if (digits == stringLength) return number.ToString();
			if (digits > stringLength)
			{
				DebugOutput.Log($"You have a higher number than the number of digits allowed!");
				return null;
			}
			string leadingZero = "";
			for (int counter = 0; counter < stringLength - digits; counter ++)
			{
				leadingZero = leadingZero + "0";
			}
			var returnString = leadingZero + number.ToString();
			DebugOutput.Log($"Returing {returnString}");
			return returnString;
		}


		public static int? GetInt32FromString(string text)
		{
            DebugOutput.Log($"GetInt32FromString {text}");
			text = text.Trim();
			try
			{
				int result = Int32.Parse(text);
				DebugOutput.Log($"Returning number {result}");
				return result;
			}
			catch (FormatException)
			{
				DebugOutput.Log($"Unable to parse '{text}'");
				return null;
			}
		}

		

		/// <summary>
		/// Break up a string passing in the delimiter value
		/// </summary>
		/// <param name="xpath"></param>
		/// <param name="delimiter"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		private static string BreakStringUpByDelmited(string xpath, char delimiter, params int[] values)
		{
			DebugOutput.Log($"Proc - BreakStringUpByDelmited {xpath} with {delimiter} ");
			var wildcardCount = xpath.Count(x => x == delimiter);
			var valueCount = values.Length;

			if (wildcardCount != valueCount)
			{
				throw new ArgumentException(
					"Wrong number of integer parameters supplied. If you're surprised that you're seeing this message, then check that you aren't using a dynamic element selector in an `id`-based XPath!");
			}

			string[] XPathBuilder = xpath.Split(delimiter);
			var finalXPath = "";
			int counter = 0;

			foreach (var value in values)
			{
				DebugOutput.Log($"{counter} = {value}");
				finalXPath = finalXPath + XPathBuilder[counter] + value.ToString();
				counter++;
			}

			finalXPath = finalXPath + XPathBuilder[counter];
			DebugOutput.Log(finalXPath);
			return finalXPath;
		}
	}
}
