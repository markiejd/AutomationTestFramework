using Core;
using Core.Configuration;
using Core.Logging;
using Generic.Steps.Helpers.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Generic.Steps
{
    public class CombinedSteps
    {
        public static bool Failed { get; set; }
        public static int FailedCount { get; set; }

        public static void Failure(string message)
        {
            DebugOutput.Log($"***** FAILURE *****");
            ElementInteraction.GetErrorScreenShotOfPage(message);
            FailedCount += 1;
            Failed = true;
            Assert.Fail(message + " FAILED");
        }

        public static bool OutputProc(string proc, int timeOut = 0)
        {
            if (TargetConfiguration.Configuration == null) return false;
            if (TargetConfiguration.Configuration.OutputOnly)
            {
                DebugOutput.debugLevel = 2;
                DebugOutput.Log(proc);
                Assert.IsTrue(true);
                return false;
            }
            var dubugLevel = DebugOutput.debugLevel;
            if (dubugLevel != 0)
            {
                DebugOutput.Log(proc);
            }
            //If any previous step has failed and Skip On Failure is true
            //DebugOutput.Log($"FAILED = {Failed}");
            //DebugOutput.Log($"Skip = {TargetConfiguration.Configuration.SkipOnFailure}");
            if (Failed && TargetConfiguration.Configuration.SkipOnFailure)
            {
                Assert.Inconclusive();
                return false;
            }
            return true;
        }

    }
}
