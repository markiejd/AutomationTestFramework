using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Elements.Switch
{
    [Binding]
    public class ThenSwitchSteps : StepsBase
    {
        public ThenSwitchSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [Then(@"Switch ""(.*)"" Is Displayed")]
        [Then(@"Switch ""([^""]*)"" Is Displayed")]
        public void ThenSwitchIsDisplayed(string switchName)
        {
            string proc = $"Then Switch {switchName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                DebugOutput.Log($"Do I get here?");
                if (Helpers.Switch.IsDisplayed(switchName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Switch ""([^""]*)"" Is Not Displayed")]
        public void ThenSwitchIsNotDisplayed(string switchName)
        {
            string proc = $"Then Switch {switchName} Is Not Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Switch.IsDisplayed(switchName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Switch ""(.*)"" Is ""(.*)""")]
        public void ThenSwitchIs(string switchName,string status)
        {
            bool? expectedStatus = GetStatusAsBool(status);
            if (expectedStatus == null)
            {
                CombinedSteps.Failure($"Invalid status {status}");
                return;
            }
            string proc = $"Then Switch {switchName} Is {status}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Switch.Status(switchName) == expectedStatus)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        ///   PRIVATE

        /// <summary>
        ///  Convert the string to a status boolean
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>

        private bool? GetStatusAsBool(string status)
        {
            bool expectedStatus = false;
            switch(status.ToLower())
            {
                default:
                {
                    DebugOutput.Log($"Invalid status {status}");
                    return null;
                }
                case "true":
                case "t":
                case "1":
                case "on":
                {
                    expectedStatus = true;
                    break;
                }
                case "false":
                case "f":
                case "0":
                case "off":
                {
                    expectedStatus = false;
                    break;
                }
            }
            return expectedStatus;
        }


    }
}
