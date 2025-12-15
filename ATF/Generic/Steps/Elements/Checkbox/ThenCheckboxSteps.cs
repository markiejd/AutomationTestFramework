using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.Checkbox
{
    [Binding]
    public class ThenCheckboxSteps : StepsBase
    {
        public ThenCheckboxSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"CheckBox ""([^""]*)"" Is Displayed")]
        public void ThenCheckBoxIsDisplayed(string checkboxName)
        {
            string proc = $"Then CheckBox {checkboxName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Checkbox.IsDisplayed(checkboxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



        [Then(@"CheckBox ""([^""]*)"" Is Selected")]
        public void ThenCheckBoxIsSelected(string checkboxName)
        {
            string proc = $"Then CheckBox {checkboxName} Is Selected";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Checkbox.IsSelected(checkboxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"CheckBox ""([^""]*)"" Is Not Selected")]
        public void ThenCheckBoxIsNotSelected(string checkboxName)
        {
            string proc = $"Then CheckBox {checkboxName} Is Selected";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Checkbox.IsSelected(checkboxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
