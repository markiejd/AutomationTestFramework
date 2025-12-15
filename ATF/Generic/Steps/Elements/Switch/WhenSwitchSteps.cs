using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Elements.Switch
{
    [Binding]
    public class WhenSwitchSteps : StepsBase
    {
        public WhenSwitchSteps(IStepHelpers helpers) : base(helpers)
        {
        }

                
        [When(@"I Click On Switch ""(.*)""")]
        public void WhenIClickOnSwitch(string switchName)
        {
            string proc = $"When I Click On Switch {switchName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Switch.Click(switchName))
                {
                    return;
                }
                DebugOutput.Log($"Failed to click!");
                return;
            }
        }

    }
}
