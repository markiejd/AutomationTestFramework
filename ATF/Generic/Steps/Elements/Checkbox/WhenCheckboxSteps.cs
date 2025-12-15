using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.Checkbox
{
    [Binding]
    public class WhenCheckboxSteps : StepsBase
    {
        public WhenCheckboxSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Click On CheckBox ""([^""]*)""")]
        public void WhenIClickOnCheckBox(string checkboxName)
        {
            string proc = $"When I Click On CheckBox {checkboxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Checkbox.Select(checkboxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
