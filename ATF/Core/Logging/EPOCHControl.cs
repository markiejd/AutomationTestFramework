

namespace Core.Logging
{
    public class EPOCHControl
    {
        public static string? Epoch { get; set; }   

        public static bool SetEpoch()
        {
            var epochNumber = DateTime.UtcNow.Ticks / 10000000 - 63082281600;
            var epoch = epochNumber.ToString();
            EPOCHControl.Epoch = epoch;
            DebugOutput.Log($"Mid Test EPOCH SET TO  {epoch}");
            return true;
        }

        public static DateTime GetDateTimeFromEPOCH(string EPOCH)
        {
            long number;
            if (long.TryParse(EPOCH, out number))
            {
                return GetDateTimeFromEPOCH(number);
            }
            else
            {
                DebugOutput.Log($"Failed to convert {EPOCH} to a long!");
                var defaultFail = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return defaultFail;
            }
        }

        public static DateTime GetDateTimeFromEPOCH(long EPOCH)
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epochStart.AddSeconds(EPOCH).ToLocalTime();
        }

        public static int GetCurrentDateTimeInEPOCH()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;
            // Convert to UTC
            DateTime utcNow = now.ToUniversalTime();
            // Calculate the EPOCH timestamp
            double epochTime = (utcNow - DateTime.UnixEpoch).TotalSeconds;
            var epochInt = Convert.ToInt32(epochTime);
            DebugOutput.Log($"NOW EPOCH = {epochTime} {epochInt}");
            return epochInt;
        }

    }
}
