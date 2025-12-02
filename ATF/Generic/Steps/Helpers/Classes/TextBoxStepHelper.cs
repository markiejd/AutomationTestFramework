using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class TextBoxStepHelper : StepHelper, ITextBoxStepHelper
    {
        private readonly ITargetForms targetForms;
        public TextBoxStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string elementType = "TextBox";

        public string GetText(string textBoxName)
        {
            DebugOutput.Log($"GetText {textBoxName}");
            return ElementInteraction.GetTextFromElement(CurrentPage, textBoxName, elementType) ?? "";
        }

        public string? GetLabelOfTextBox(string textBoxName, By labelLocator)
        {
            DebugOutput.Log($"GetLabelOfTextBox {textBoxName}");
            return ElementInteraction.GetLabelFromElement(CurrentPage, textBoxName, elementType, labelLocator) ?? "";
        }

        public bool Clear(string textBoxName)
        {
            DebugOutput.Log($"Clear {textBoxName}");
            return ElementInteraction.ClearTextFromElement(CurrentPage, textBoxName, elementType);
        }

        public bool ClearThenEnterText(string textBoxName, string text)
        {
            DebugOutput.Log($"ClearThenEnterText {textBoxName} {text}");
            return ElementInteraction.ClearTextThenEnterTextToElement(CurrentPage, textBoxName, elementType, text);
        }

        public bool EnterText(string textBoxName, string text, string key = "")
        {
            DebugOutput.Log($"EnterText {textBoxName} {text} {key}");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, textBoxName, elementType, text, key);
        }

        public bool EnterTextAndKey(string textBoxName, string text, string key)
        {
            DebugOutput.Log($"EnterTextAndKey {textBoxName} {text} {key}");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, textBoxName, elementType, text, key);
        }

        public bool IsDisplayed(string textBoxName, int timeOut = 0)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, textBoxName, "TextBox", timeOut);
        }

        public bool Click(string textBoxName)
        {
            DebugOutput.Log($"Click {textBoxName}");
            return ElementInteraction.ClickOnElement(CurrentPage, textBoxName, elementType);
        }

        public bool IsReadOnly(string textBoxName)
        {
            DebugOutput.Log($"Proc - IsReadOnly {textBoxName}");
            return ElementInteraction.IsElementReadOnly(CurrentPage, textBoxName, elementType);
        }

        public int GetWidthOfTextBox(string textBoxName)
        {
            DebugOutput.Log($"Proc - GetWidthOfTextBox {textBoxName}");
            return ElementInteraction.GetWidthOfElement(CurrentPage, textBoxName, elementType) ?? 0;
        }

        public string GetPlaceholderText(string textBoxName)
        {
            DebugOutput.Log($"GetPlaceholderText {textBoxName}");
            return ElementInteraction.GetPlaceholderText(CurrentPage, textBoxName, elementType) ?? "";
        }



    }
}
