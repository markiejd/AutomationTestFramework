using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Elements.Switch
{
    [Binding]
    public class GivenSwitchSteps : StepsBase
    {
        public GivenSwitchSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [Given(@"Switch ""(.*)"" Is ""(.*)""")]
        public void GivenSwitchIs(string switchName,string status)
        {
            string proc = $"Given Switch {switchName} Is {status}";
            if (CombinedSteps.OuputProc(proc))
            {
                bool? expectedStatus = GetStatusAsBool(status);
                if (expectedStatus == null)
                {
                    CombinedSteps.Failure($"Invalid status {status}");
                    return;
                }
                if (Helpers.Switch.Status(switchName) == expectedStatus)
                {
                    DebugOutput.Log($"All good here no change required.");
                    return;
                }
                if (Helpers.Switch.Click(switchName))
                {
                    DebugOutput.Log($"I've clicked to change the status");
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
