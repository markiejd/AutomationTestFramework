using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class CheckboxStepHelper : StepHelper, ICheckboxStepHelper
    {
        private readonly ITargetForms targetForms;
        public CheckboxStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string elementType = "Button";

        public bool IsDisplayed(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"IsDisplayed {checkboxName}");
            if (ElementInteraction.IsElementDisplayed(CurrentPage, checkboxName, "CheckBox")) return true;
            return ElementInteraction.IsElementsParentElementDisplayed(CurrentPage, checkboxName, "CheckBox");
        }

        public bool IsSelected(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"IsSelected {checkboxName}");
            return ElementInteraction.IsElementSelected(CurrentPage, checkboxName, elementType);
        }

        public bool Select(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"Select {checkboxName}");
            if (ElementInteraction.ClickOnElement(CurrentPage, checkboxName, elementType)) return true;
            DebugOutput.Log($"Wee issue clicking on the check box....  Some people have the input of a checkbox hidden below another element!");
            return ElementInteraction.ClickOnElement_CheckBoxSub(CurrentPage, checkboxName, elementType);
        }

        public bool Selected(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"Selected {checkboxName}");            
            if (IsSelected(checkboxName)) return true;
            //Not selected, needs to be selected
            return Select(checkboxName, timeout);
        }

        public bool SelectedNot(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"SelectedNot {checkboxName}");
            if (!IsSelected(checkboxName)) return true;
            //Is selected, needs to be NOT selected, so we click on it!
            return Select(checkboxName, timeout);
        }
    }
}
