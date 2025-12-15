using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class RadioButtonStepHelper : StepHelper, IRadioButtonStepHelper
    {
        private readonly ITargetForms targetForms;
        public RadioButtonStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }


        public bool IsDisplayed(string radioButtonName)
        {
            DebugOutput.Log($"proc - IsDisplayed {radioButtonName}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, radioButtonName, "radiobutton");
        }

        public bool IsEnabled(string radioButtonName)
        {
            DebugOutput.Log($"proc - IsEnabled {radioButtonName}");
            return ElementInteraction.IsElementEnabled(CurrentPage, radioButtonName, "radiobutton");
        }

        public bool IsSelected(string radioButtonName)
        {
            DebugOutput.Log($"proc - IsSelected {radioButtonName}");
            return ElementInteraction.IsElementSelected(CurrentPage, radioButtonName, "radiobutton");
        }

        public bool Select(string radioButtonName)
        {
            DebugOutput.Log($"Select {radioButtonName}");
            return ElementInteraction.ClickOnElement(CurrentPage, radioButtonName, "radiobutton");
        }

    }
}
