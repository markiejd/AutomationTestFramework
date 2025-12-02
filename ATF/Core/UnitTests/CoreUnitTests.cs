
using Core.Logging;
using Core.NLM;
using Core.Transformations;


namespace Core.UnitTests
{
    public static class CoreUnitTests
    {

        private static bool Result(string method, string reason, bool pass)
        {
            if (!pass) DebugOutput.Log($" ***********  FAILURE *************");
            DebugOutput.Log(method + " >>> " + reason);
            return pass;
        }

        public static string Hello()
        {
            return "hello";
        }

        public static bool PassAddJsonStringToListOfAnalusedAnswersSupplied_NotNull_Count()
        {
            var method = "PassAddJsonStringToListOfAnalusedAnswersSupplied_NotNull_Count";
            DebugOutput.Log($"STARTING {method} METHOD");
            var jsonString = "[{\"Id\": 1, \"Text\": \"Answer 1\"}, {\"Id\": 2, \"Text\": \"Answer 2\"}]";
            var currentListOfAnalysesAnswers = new List<AnalysedAnswer>();
            var result = Analyse.AddJsonStringToListOfAnalusedAnswersSupplied(jsonString, currentListOfAnalysesAnswers);
            if (result == null) return Result(method, $"PassAddJsonStringToListOfAnalusedAnswersSuppliedNot_Null should never be null!", false);
            if (result.Count != 2) return Result(method, $"PassAddJsonStringToListOfAnalusedAnswersSupplied_Count should be 2! {result.Count}", false);
            return Result(method, "Result is NOT NULL - correct", true);
        }


        //  TRANSFORMATIONS

            // DateValues
            public static bool Pass_ReturnNowDateAsString_Default()
            {
                var method = "Pass_ReturnNowDateAsString_Default";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.ReturnNowDateAsString();
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTime();
                var date = now.ToString("MM/dd/yyyy");
                if (date != returnValue) return Result(method, $"We did NOT get expected date {returnValue} back {date}", false);
                return Result(method,"All Passed", true);
            }            
            public static bool Pass_ReturnFirstOfThisMonth_Default()
            {
                var method = "Pass_ReturnFirstOfThisMonth_Default";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.ReturnFirstOfThisMonth();
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTime();
                var date = now.ToString("MM/01/yyyy");
                if (date != returnValue) return Result(method ,$"We did NOT get expected date {returnValue} back {date}", false);
                return Result(method,"All Passed", true);
            }     
            public static bool Pass_GetDateOnlyFromDateTime()
            {
                var method = "Pass_GetDateOnlyFromDateTime";
                DebugOutput.Log($"STARTING {method} METHOD");
                var now = DateValues.GetDateTime();
                var returnValue = DateValues.GetDateOnlyFromDateTime(now);
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTime();
                var date = now.ToString("dd/MMM/yyyy");
                if (date != returnValue) return Result(method ,$"We did NOT get expected date {returnValue} back {date}", false);
                return Result(method ,"All Passed", true);
            }
            public static bool Pass_GetDateTimeFromDateString()
            {
                var method = "Pass_GetDateTimeFromDateString";
                DebugOutput.Log($"STARTING {method} METHOD");
                var now = "03/18/2018 07:22:16";
                var returnValue = DateValues.GetDateTimeFromDateString(now);
                if (returnValue == null) return Result(method, "Failed to parse date!", false);
                DebugOutput.Log($"The return value = {returnValue}");
                var expectedString = "3/18/2018 7:22:16 AM";
                var expectedDate = DateValues.GetDateTimeFromParse(expectedString);
                if (expectedDate != returnValue) return Result(method ,$"We were expecting {expectedDate} but got {returnValue}", false);
                return Result(method ,"All Passed", true);
            }
            public static bool Pass_GetDateTimeFromDateString_Format_Default()
            {
                var method = "Pass_GetDateTimeFromDateString_Format_Default";
                DebugOutput.Log($"STARTING {method} METHOD");
                var now = "Sat 08 Jan 2013 8:30 AM -06:00";
                var returnValue = DateValues.GetDateTimeFromDateString(now, "");
                DebugOutput.Log($"The return value = {returnValue}");
                var expectedString = "6/1/2013 3:30:00 PM";
                var expectedDate = DateValues.GetDateTimeFromParse(expectedString);
                if (expectedDate != returnValue) return Result(method ,$"We were expecting {expectedDate} but got {returnValue}", false);
                return Result(method ,"All Passed", true);
            }
            public static bool Pass_GetDateTimeFromStringX_Now()
            {
                var method = "Pass_GetDateTimeFromStringX_Now";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.GetDateTimeFromStringX("NOW");
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTimeUTC();
                var seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method, "Not expected difference?!", false);
                return Result("Pass_GetDateTimeFromStringX_Now","All Passed", true);
            }
            public static bool Pass_GetDateTimeFromStringX_Today()
            {
                var method = "Pass_GetDateTimeFromStringX_Today";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.GetDateTimeFromStringX("TODAY");
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTimeUTC();
                var seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method ,"Not expected difference?!", false);
                return Result(method ,"All Passed", true);
            }            
            public static bool Pass_GetDateTimeFromStringX_Today_Minus_Minute()
            {
                var method = "Pass_GetDateTimeFromStringX_Today_Minus_Minute";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.GetDateTimeFromStringX("TODAY-1m");
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTimeUTC();
                now = now.AddMinutes(-1);
                var seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method, $"Not expected difference?! {now} {returnValue} {10}", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-1mm");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddMinutes(-1);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method, $"Not expected difference?! {now} {returnValue} {10}", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-60minute");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddMinutes(-60);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method, $"Not expected difference?! {now} {returnValue} {10}", false);

                return Result(method, "All Passed", true);
            }                        
            public static bool Pass_GetDateTimeFromStringX_Today_Minus_Hour()
            {
                var method = "Pass_GetDateTimeFromStringX_Today_Minus_Hour";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.GetDateTimeFromStringX("TODAY-1h");
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTimeUTC();
                now = now.AddHours(-1);
                var seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method ,"Not expected difference?!", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-1hour");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddHours(-1);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method ,"Not expected difference?!", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-12hours");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddHours(-12);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method ,"Not expected difference?!", false);

                return Result("Pass_GetDateTimeFromStringX_Today_Minus_Hour","All Passed", true);
            }
            public static bool Pass_GetDateTimeFromStringX_Today_Minus_Day()
            {
                var method = "Pass_GetDateTimeFromStringX_Today_Minus_Day";
                DebugOutput.Log($"STARTING {method} METHOD");
                var returnValue = DateValues.GetDateTimeFromStringX("TODAY-1d");
                DebugOutput.Log($"The return value = {returnValue}");
                var now = DateValues.GetDateTimeUTC();
                now = now.AddDays(-1);
                var seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method ,"Not expected difference?!", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-1day");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddDays(-1);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method ,"Not expected difference?!", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-1days");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddDays(-1);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method,"Not expected difference?!", false);
                
                returnValue = DateValues.GetDateTimeFromStringX("TODAY-1 ");
                DebugOutput.Log($"The return value = {returnValue}");
                now = DateValues.GetDateTimeUTC();
                now = now.AddDays(-1);
                seconds = DateValues.GetSecondsBetweenDateTimes(now, returnValue);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, returnValue, 10)) return Result(method,"Not expected difference?!", false);

                return Result(method ,"All Passed", true);
            }
            public static bool Pass_GetSecondsBetweenDateTimes()
            {
                var method = "Pass_GetSecondsBetweenDateTimes";
                DebugOutput.Log($"STARTING {method} METHOD");
                var change = 100;
                var now = DateValues.GetDateTimeUTC();
                var nowChanged = now.AddSeconds(change);
                var diff = DateValues.GetSecondsBetweenDateTimes(now, nowChanged);
                if (diff != change) return Result(method, $"Difference here {diff} expected {change}?!", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_IsTwoDateTimesWithinSeconds()
            {
                var method = "Pass_IsTwoDateTimesWithinSeconds";
                DebugOutput.Log($"STARTING {method} METHOD");
                var change = 10;
                var now = DateValues.GetDateTimeUTC();
                var nowChanged = now.AddSeconds(change);
                if (!DateValues.IsTwoDateTimesWithinSeconds(now, nowChanged, change)) return Result(method, $"Pass_IsTwoDateTimesWithinSeconds Difference here {change} IS more than expected?!", false);
                
                now = DateTime.UtcNow;
                nowChanged = now.AddSeconds(change * 2);
                if (DateValues.IsTwoDateTimesWithinSeconds(now, nowChanged, change)) return Result(method, $"Pass_IsTwoDateTimesWithinSeconds Difference here {change * 2}?! this should be testing the false", false);

                return Result(method, "All Passed", true);
            }

            public static bool Pass_BreakUpDateAndTime()
            {
                var method = "Pass_BreakUpDateAndTime";
                DebugOutput.Log($"STARTING {method} METHOD");
                var stringDate = "16/01/1973 16:03:59";
                var returnValue = DateValues.BreakUpDateAndTime(stringDate);
                if (returnValue.Count < 1) return Result(method, "Did not work out the date format!", false);
                var returnValueDay = "";
                var returnValueMonth = "";
                var returnValueYear = "";
                var returnValueHour = "";
                var returnValueMinute = "";
                var returnValueSecond = "";
                try
                {
                    returnValueDay = returnValue[0];
                    returnValueMonth = returnValue[1];
                    returnValueYear = returnValue[2];
                    returnValueHour = returnValue[3];
                    returnValueMinute = returnValue[4];
                    returnValueSecond = returnValue[5];
                }
                catch
                {
                    return Result(method, $"Failed to populate from return string {returnValue}", false);
                }
                if (returnValueDay != "16") return Result(method, $"Failed to populate from return DAY string {returnValue}", false);
                if (returnValueMonth != "01") return Result(method, $"Failed to populate from return returnValueMonth string {returnValue}", false);
                if (returnValueYear != "1973") return Result(method, $"Failed to populate from return returnValueYear string {returnValue}", false);
                if (returnValueHour != "16") return Result(method, $"Failed to populate from return returnValueHour string {returnValue}", false);
                if (returnValueMinute != "03") return Result(method, $"Failed to populate from return returnValueMinute string {returnValue}", false);
                if (returnValueSecond != "59") return Result(method, $"Failed to populate from return returnValueSecond string {returnValue}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_Format_Default()
            {
                var method = "Pass_Format_Default";
                DebugOutput.Log($"STARTING {method} METHOD");
                var dateTimeString = DateValues.GetDateTimeFromDateString("2011-03-21 13:26");
                if (dateTimeString == null) return Result(method, "Failed to parse date", false);
                var returnValue = DateValues.Format(dateTimeString.Value);
                DebugOutput.Log($"Return value = {returnValue}");
                var match = "03/21/2011";
                if (returnValue != match) return Result(method, $"Failed to match {match} gptten {returnValue}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_Format_1()
            {
                var method = "Pass_Format_1";
                DebugOutput.Log($"STARTING {method} METHOD");
                var dateTimeString = DateValues.GetDateTimeFromDateString("2011-03-21 13:26");
                if (dateTimeString == null) return Result(method, "Failed to parse date", false);
                var returnValue = DateValues.Format(dateTimeString.Value, "1");
                DebugOutput.Log($"Returning value = {returnValue}");
                var match = "03/21/11";
                if (returnValue != match) return Result(method, $"Failed to match {match} gptten {returnValue}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_Format_2()
            {
                var method = "Pass_Format_2";
                DebugOutput.Log($"STARTING {method} METHOD");
                var dateTimeString = DateValues.GetDateTimeFromDateString("2011-03-21 13:26");
                if (dateTimeString == null) return Result(method, "Failed to parse date", false);
                var returnValue = DateValues.Format(dateTimeString.Value, "2");
                DebugOutput.Log($"Returning value = {returnValue}");
                var match = "11/03/21";
                if (returnValue != match) return Result(method, $"Failed to match {match} gptten {returnValue}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_Format_23()
            {
                var method = "Pass_Format_23";
                DebugOutput.Log($"STARTING {method} METHOD");
                var dateTimeString = DateValues.GetDateTimeFromDateString("2011-03-21 13:26");
                if (dateTimeString == null) return Result(method, "Failed to parse date", false);
                var returnValue = DateValues.Format(dateTimeString.Value, "2");
                DebugOutput.Log($"Returning value = {returnValue}");
                var match = "11/03/21";
                if (returnValue != match) return Result(method, $"Failed to match {match} gptten {returnValue}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_MathsToDate()
            {
                var method = "Pass_MathsToDate";
                DebugOutput.Log($"STARTING {method} METHOD");
                var now = DateValues.GetDateTimeUTC();
                var changedNow = DateValues.MathsToDate("3","+","101");
                var nowChanged = now.AddDays(3).ToString("MM/dd/yyyy");
                // Changed fown = 05/19/2024 verse 5/19/2024 2:27:07 PM&#xD;
                DebugOutput.Log($"Changed fown = {changedNow} verse {nowChanged}");
                if (nowChanged != changedNow) return Result(method, $"Failed to match these dates {changedNow} and {nowChanged}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_TurnStringDateAround()
            {
                var method = "Pass_TurnStringDateAround";
                DebugOutput.Log($"STARTING {method} METHOD");
                var date = "16/01/1973";
                var newDate = DateValues.TurnStringDateAround(date);
                var expectedReturn = "01/16/1973";
                if (newDate != expectedReturn) return Result(method, $"Failed to move day and month around {expectedReturn} -> {newDate}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_GetTimeInUnix()
            {                
                var method = "Pass_GetTimeInUnix";
                DebugOutput.Log($"STARTING {method} METHOD");
                var expectedString = "8/18/2018 7:22:16 AM";
                var expectedDate = DateValues.GetDateTimeFromParse(expectedString) ?? DateValues.GetDateTime();
                var expectedUnixTime = DateValues.GetTimeInUnix(expectedDate);
                DebugOutput.Log($"The unix time for {expectedString} is {expectedUnixTime}");
                var expected = 1534573336;
                if (expected != expectedUnixTime) return Result(method, $"Unix numbers not equal, we expected {expected.ToString()} but got {expectedUnixTime.ToString()}", false);

                return Result(method, "All Passed", true);
            }
            public static bool Pass_GetStaticUnix()
            {
                var method = "Pass_GetTimeInUnix";
                DebugOutput.Log($"STARTING {method} METHOD");
                var period = "MIN";
                var returnValue = DateValues.GetStaticUnix(period);
                var expectedSeconds = 60;
                if (returnValue != expectedSeconds) return Result(method, $"Thats unexpected - For {period} we expect {expectedSeconds} to equal {returnValue} seconds",false);
                period = "minute";
                returnValue = DateValues.GetStaticUnix(period);
                expectedSeconds = 60;
                if (returnValue != expectedSeconds) return Result(method, $"Thats unexpected - For {period} we expect {expectedSeconds} to equal {returnValue} seconds",false);
                period = "hour";
                returnValue = DateValues.GetStaticUnix(period);
                expectedSeconds = 3600;
                if (returnValue != expectedSeconds) return Result(method, $"Thats unexpected - For {period} we expect {expectedSeconds} to equal {returnValue} seconds",false);
                period = "day";
                returnValue = DateValues.GetStaticUnix(period);
                expectedSeconds = 86400;
                if (returnValue != expectedSeconds) return Result(method, $"Thats unexpected - For {period} we expect {expectedSeconds} to equal {returnValue} seconds",false);
                period = "week";
                returnValue = DateValues.GetStaticUnix(period);
                expectedSeconds = 604800;
                if (returnValue != expectedSeconds) return Result(method, $"Thats unexpected - For {period} we expect {expectedSeconds} to equal {returnValue} seconds",false);
                period = "year";
                returnValue = DateValues.GetStaticUnix(period);
                expectedSeconds = 31536000;
                if (returnValue != expectedSeconds) return Result(method, $"Thats unexpected - For {period} we expect {expectedSeconds} to equal {returnValue} seconds",false);

                return Result(method, "All Passed", true);
            }

            public static bool Pass_ChangeDateDivider()
            {
                var method = "Pass_ChangeDateDivider";
                DebugOutput.Log($"STARTING {method} METHOD");
                string date = "16/01/1973";
                var changeTo = "-";
                var dateReturned = DateValues.ChangeDateDivider(date, changeTo);
                var expectedDate = "16-01-1973";
                if (expectedDate != dateReturned) return Result(method, $"We expected {expectedDate} but got {dateReturned} changing {changeTo}", false);
                date = "16-01-1973";
                changeTo = "/";
                dateReturned = DateValues.ChangeDateDivider(date, changeTo);
                expectedDate = "16/01/1973";
                if (expectedDate != dateReturned) return Result(method, $"We expected {expectedDate} but got {dateReturned} changing {changeTo}", false);

                return Result(method, "All Passed", true);
            }
            

    }
}