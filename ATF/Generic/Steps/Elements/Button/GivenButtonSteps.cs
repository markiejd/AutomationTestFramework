using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Button
{
    [Binding]
    public class GivenButtonSteps : StepsBase
    {
        public GivenButtonSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Button ""([^""]*)"" Is Displayed")]
        public void GivenButtonIsDisplayed(string buttonName)
        {
            string proc = $"Then Button {buttonName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.IsDisplayed(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Given(@"Button ""([^""]*)"" Is Active")]
        public void GivenButtonIsActive(string buttonName)
        {
            string proc = $"Given Button {buttonName} Is Active";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.IsSelected(buttonName))
                {
                    return;
                }
                if (Helpers.Button.ClickButton(buttonName)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Given(@"Button ""([^""]*)"" Is Not Active")]
        public void GivenButtonIsNotActive(string buttonName)
        {
            string proc = $"Given Button {buttonName} Is Not Active";
            if (Helpers.Button.IsSelected(buttonName))
            {
                if (Helpers.Button.ClickButton(buttonName)) return;                
            }
            CombinedSteps.Failure(proc);
            return;
        }





    }
}
