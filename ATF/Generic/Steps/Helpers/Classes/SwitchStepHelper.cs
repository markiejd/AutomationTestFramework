using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.Extensions.DependencyModel.Resolution;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class SwitchStepHelper : StepHelper, ISwitchStepHelper
    {
        private readonly ITargetForms targetForms;
        public SwitchStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool Click(string switchName)
        {
            DebugOutput.Log($"Clicking on switch {switchName}");
            return ElementInteraction.ClickOnElement(CurrentPage, switchName, "Switch");
        }

        public bool IsDisplayed(string switchName)
        {
            DebugOutput.Log($"Checking if switch {switchName} is displayed");
            return ElementInteraction.IsElementDisplayed(CurrentPage, switchName, "Switch");
        }

        public bool Status(string switchName)
        {
            DebugOutput.Log($"Checking if switch {switchName} is on");
            return ElementInteraction.IsElementSelected(CurrentPage, switchName, "Switch");
        }

    }
}