using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using OpenQA.Selenium;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Step helper that encapsulates high-level interactions with TextBox elements on the current page.
    /// Delegates low-level work to ElementInteraction and centralizes logging for traceability.
    /// </summary>
    public class TextBoxStepHelper : StepHelper, ITextBoxStepHelper
    {
        // DI dependency kept for potential future use (e.g., resolving page-specific form contexts)
        private readonly ITargetForms _targetForms;

        /// <summary>
        /// Initializes a new instance of <see cref="TextBoxStepHelper"/>.
        /// </summary>
        /// <param name="featureContext">The current feature context provided by the test framework.</param>
        /// <param name="targetForms">Form targeting helper injected for future extensibility.</param>
        public TextBoxStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            _targetForms = targetForms; // Reserved for future use (e.g., dynamic form targeting)
        }

        // Element type constant used by the ElementInteraction layer for locator resolution
        private const string ElementType = "TextBox";

        /// <summary>
        /// Gets the visible text value from the specified TextBox.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <returns>The current text value, or an empty string if not found.</returns>
        public string GetText(string textBoxName)
        {
            // Log the action for debug/trace output
            DebugOutput.Log($"GetText {textBoxName}");
            // Delegate to the interaction layer; normalize null to empty string
            return ElementInteraction.GetTextFromElement(CurrentPage, textBoxName, ElementType) ?? "";
        }

        /// <summary>
        /// Gets the label text associated with the specified TextBox.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <param name="labelLocator">The Selenium locator used to find the label element.</param>
        /// <returns>The label text, or an empty string if not found.</returns>
        public string? GetLabelOfTextBox(string textBoxName, By labelLocator)
        {
            DebugOutput.Log($"GetLabelOfTextBox {textBoxName}");
            // Normalize null to empty string for convenience
            return ElementInteraction.GetLabelFromElement(CurrentPage, textBoxName, ElementType, labelLocator) ?? "";
        }

        /// <summary>
        /// Clears all text from the specified TextBox.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool Clear(string textBoxName)
        {
            DebugOutput.Log($"Clear {textBoxName}");
            return ElementInteraction.ClearTextFromElement(CurrentPage, textBoxName, ElementType);
        }

        /// <summary>
        /// Clears the TextBox and then enters the provided text.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <param name="text">The text to enter after clearing.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool ClearThenEnterText(string textBoxName, string text)
        {
            DebugOutput.Log($"ClearThenEnterText {textBoxName} {text}");
            return ElementInteraction.ClearTextThenEnterTextToElement(CurrentPage, textBoxName, ElementType, text);
        }

        /// <summary>
        /// Enters text into the specified TextBox, optionally followed by a key press.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <param name="text">The text to enter.</param>
        /// <param name="key">Optional key to press after entering text (e.g., Enter). Defaults to empty string.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool EnterText(string textBoxName, string text, string key = "")
        {
            DebugOutput.Log($"EnterText {textBoxName} {text} {key}");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, textBoxName, ElementType, text, key);
        }

        /// <summary>
        /// Enters text into the specified TextBox and presses a specific key.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <param name="text">The text to enter.</param>
        /// <param name="key">The key to press after entering text (e.g., Enter).</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool EnterTextAndKey(string textBoxName, string text, string key)
        {
            DebugOutput.Log($"EnterTextAndKey {textBoxName} {text} {key}");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, textBoxName, ElementType, text, key);
        }

        /// <summary>
        /// Determines whether the specified TextBox is displayed within an optional timeout.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <param name="timeOut">Optional timeout in seconds; 0 uses the default.</param>
        /// <returns>True if displayed; otherwise, false.</returns>
        public bool IsDisplayed(string textBoxName, int timeOut = 0)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, textBoxName, ElementType, timeOut);
        }

        /// <summary>
        /// Clicks the specified TextBox (sets focus or triggers click events as applicable).
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool Click(string textBoxName)
        {
            DebugOutput.Log($"Click {textBoxName}");
            return ElementInteraction.ClickOnElement(CurrentPage, textBoxName, ElementType);
        }

        /// <summary>
        /// Determines whether the specified TextBox is read-only.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <returns>True if read-only; otherwise, false.</returns>
        public bool IsReadOnly(string textBoxName)
        {
            DebugOutput.Log($"Proc - IsReadOnly {textBoxName}");
            return ElementInteraction.IsElementReadOnly(CurrentPage, textBoxName, ElementType);
        }

        /// <summary>
        /// Gets the rendered width (in pixels) of the specified TextBox.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <returns>The width in pixels, or 0 if unavailable.</returns>
        public int GetWidthOfTextBox(string textBoxName)
        {
            DebugOutput.Log($"Proc - GetWidthOfTextBox {textBoxName}");
            return ElementInteraction.GetWidthOfElement(CurrentPage, textBoxName, ElementType) ?? 0;
        }

        /// <summary>
        /// Gets the placeholder attribute text for the specified TextBox.
        /// </summary>
        /// <param name="textBoxName">The logical name of the TextBox.</param>
        /// <returns>The placeholder text, or an empty string if none exists.</returns>
        public string GetPlaceholderText(string textBoxName)
        {
            DebugOutput.Log($"GetPlaceholderText {textBoxName}");
            return ElementInteraction.GetPlaceholderText(CurrentPage, textBoxName, ElementType) ?? "";
        }
    }
}
