using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class ButtonStepHelper : StepHelper, IButtonStepHelper
    {
        private readonly ITargetForms targetForms;
        public ButtonStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string elementType = "Button";

        public string GetText(string buttonName)
        {
            DebugOutput.Log($"GetText {buttonName}");
            return ElementInteraction.GetTextFromElement(CurrentPage, buttonName, elementType) ?? "";
        }

        public bool IsSelected(string buttonName, int timeout =0)
        {
            DebugOutput.OutputMethod("IsSelected", buttonName);
            return ElementInteraction.IsElementSelected(CurrentPage, buttonName, elementType);
        }

        public bool IsDisplayed(string buttonName, int timeout = 0)
        {
            DebugOutput.OutputMethod("IsDisplayed", buttonName);
            return ElementInteraction.IsElementDisplayed(CurrentPage, buttonName, elementType);
        }

        public bool IsNotDisplayed(string buttonName, int timeout = 30)
        {
            DebugOutput.Log($"IsNotDisplayed {buttonName}");
            return ElementInteraction.IsElementNotDisplayed(CurrentPage, buttonName, elementType);
        }

        public bool IsEnabled(string buttonName)
        {
            DebugOutput.Log($"IsEnabled {buttonName}");
            return ElementInteraction.IsElementEnabled(CurrentPage, buttonName, elementType);
        }

        public bool ClickButton(string buttonName)
        {
            DebugOutput.Log($"ClickButton {buttonName}");
            return ElementInteraction.ClickOnElement(CurrentPage, buttonName, elementType);
        }

        public bool DoubleClick(string buttonName)
        {
            DebugOutput.Log($"ClickButton {buttonName}");
            return ElementInteraction.DoubleClickOnElement(CurrentPage, buttonName, elementType);
        }

        public bool RightClick(string buttonName)
        {
            DebugOutput.Log($"ClickButton {buttonName}");
            return ElementInteraction.RightClickElement(CurrentPage, buttonName, elementType);
        }

        public bool MouseOver(string buttonName)
        {
            DebugOutput.Log($"MouseOver {buttonName}");
            return ElementInteraction.MouseOverElement(CurrentPage, buttonName, elementType);
        }

        public bool ClickNthButton(string buttonName, string nTh)
        {
            DebugOutput.Log($"ClickNthButton {buttonName} {nTh}");
            return ElementInteraction.ClickNthElement(CurrentPage, buttonName, elementType, nTh);
        }

        public bool DragAToB(string buttonAName, string buttonBName)
        {
            DebugOutput.Log($"DragAToB {buttonAName} {buttonBName}");
            return ElementInteraction.ClickNthElement(CurrentPage, buttonAName, buttonBName, elementType);
        }
    }
}
