using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.Checkbox
{
    [Binding]
    public class GivenCheckboxSteps : StepsBase
    {
        public GivenCheckboxSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"CheckBox ""([^""]*)"" Is Selected")]
        public void GivenCheckBoxIsSelected(string checkboxName)
        {
            string proc = $"Given CheckBox {checkboxName} Is Selected";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Checkbox.Selected(checkboxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Given(@"CheckBox ""([^""]*)"" Is Not Selected")]
        public void GivenCheckBoxIsNotSelected(string checkboxName)
        {
            string proc = $"Given CheckBox {checkboxName} Is Selected";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Checkbox.SelectedNot(checkboxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
