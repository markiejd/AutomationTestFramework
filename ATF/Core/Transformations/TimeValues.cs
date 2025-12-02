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

		public static bool IsTimeIsh(string gotTime, string expectedTime, int range = 1)
		{
			DebugOutput.Log($"Proc - IsTimeNowIsh {gotTime} {expectedTime} {range}minute");
			var dateTimeGot = DateValues.GetDateTimeFromDateString(gotTime);
			if (dateTimeGot == null) return false;
			var dateTimeInUnix = DateValues.GetTimeInUnix(dateTimeGot.Value);
			DebugOutput.Log($"Unix time = {dateTimeInUnix}");
			var rangeUnix = DateValues.GetStaticUnix("min") * range;
			DebugOutput.Log($"Range in UNIX time = {rangeUnix}");
			var dateTimeExpected = DateValues.GetDateTimeFromDateString(expectedTime);
			if (dateTimeExpected == null) return false;
			var expectedTimeInUnix = DateValues.GetTimeInUnix(dateTimeExpected.Value);
			var diff = expectedTimeInUnix - dateTimeInUnix;
			if (diff < 0) diff = diff * -1;
			if (diff > rangeUnix) return false;
			return true;
		}


	}
}
