using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class SpinnerStepHelper : StepHelper, ISpinnerStepHelper
    {
        private readonly ITargetForms targetForms;
        public SpinnerStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool StillDisplayed()
        {
            DebugOutput.Log($"Proc - StillDisplayed");
            return ElementInteraction.WaitForElementToBeDisplayed(CurrentPage, "spinner", "spinner");
        }

        public bool SpinnerIsGone()
        {
            DebugOutput.Log($"Proc - SpinnerIsGone");
            return ElementInteraction.WaitForElementToNotBeDisplayed(CurrentPage, "spinner", "spinner");
        }

    }
}
