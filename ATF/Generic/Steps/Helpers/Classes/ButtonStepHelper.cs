using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for button-related test step operations.
    /// Provides methods for interacting with button elements in UI automation tests.
    /// </summary>
    public class ButtonStepHelper : StepHelper, IButtonStepHelper
    {
        private readonly ITargetForms targetForms;
        
        // Element type identifier for logging and element lookup
        private string elementType = "Button";

        /// <summary>
        /// Initializes a new instance of the ButtonStepHelper class.
        /// </summary>
        /// <param name="featureContext">The Reqnroll feature context for the current test.</param>
        /// <param name="targetForms">Interface for managing target forms in the application.</param>
        public ButtonStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Retrieves the text content from a specified button element.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <returns>The button text, or empty string if not found.</returns>
        public string GetText(string buttonName)
        {
            DebugOutput.Log($"GetText {buttonName}");
            // Fetch text from the element, defaulting to empty string if null
            return ElementInteraction.GetTextFromElement(CurrentPage, buttonName, elementType) ?? "";
        }

        /// <summary>
        /// Checks if a button element is in a selected state.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <param name="timeout">Optional timeout in seconds (currently unused).</param>
        /// <returns>True if the button is selected; otherwise, false.</returns>
        public bool IsSelected(string buttonName, int timeout = 0)
        {
            DebugOutput.OutputMethod("IsSelected", buttonName);
            return ElementInteraction.IsElementSelected(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Checks if a button element is displayed/visible on the page.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <param name="timeout">Optional timeout in seconds (currently unused).</param>
        /// <returns>True if the button is displayed; otherwise, false.</returns>
        public bool IsDisplayed(string buttonName, int timeout = 0)
        {
            DebugOutput.OutputMethod("IsDisplayed", buttonName);
            return ElementInteraction.IsElementDisplayed(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Checks if a button element is not displayed/hidden on the page.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <param name="timeout">Optional timeout in seconds (default: 30 seconds).</param>
        /// <returns>True if the button is not displayed; otherwise, false.</returns>
        public bool IsNotDisplayed(string buttonName, int timeout = 30)
        {
            DebugOutput.Log($"IsNotDisplayed {buttonName}");
            return ElementInteraction.IsElementNotDisplayed(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Checks if a button element is enabled and can be interacted with.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <returns>True if the button is enabled; otherwise, false.</returns>
        public bool IsEnabled(string buttonName)
        {
            DebugOutput.Log($"IsEnabled {buttonName}");
            return ElementInteraction.IsElementEnabled(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Performs a single left-click action on the specified button element.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <returns>True if the click was successful; otherwise, false.</returns>
        public bool ClickButton(string buttonName)
        {
            DebugOutput.Log($"ClickButton {buttonName}");
            return ElementInteraction.ClickOnElement(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Performs a double-click action on the specified button element.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <returns>True if the double-click was successful; otherwise, false.</returns>
        public bool DoubleClick(string buttonName)
        {
            DebugOutput.Log($"DoubleClick {buttonName}");
            return ElementInteraction.DoubleClickOnElement(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Performs a right-click (context menu) action on the specified button element.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <returns>True if the right-click was successful; otherwise, false.</returns>
        public bool RightClick(string buttonName)
        {
            DebugOutput.Log($"RightClick {buttonName}");
            return ElementInteraction.RightClickElement(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Performs a mouse hover action over the specified button element.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <returns>True if the mouse over action was successful; otherwise, false.</returns>
        public bool MouseOver(string buttonName)
        {
            DebugOutput.Log($"MouseOver {buttonName}");
            return ElementInteraction.MouseOverElement(CurrentPage, buttonName, elementType);
        }

        /// <summary>
        /// Performs a click action on the Nth occurrence of a button element with the specified name.
        /// </summary>
        /// <param name="buttonName">The name/identifier of the button element.</param>
        /// <param name="nTh">The index (1-based) of the button occurrence to click.</param>
        /// <returns>True if the click was successful; otherwise, false.</returns>
        public bool ClickNthButton(string buttonName, string nTh)
        {
            DebugOutput.Log($"ClickNthButton {buttonName} {nTh}");
            return ElementInteraction.ClickNthElement(CurrentPage, buttonName, elementType, nTh);
        }

        /// <summary>
        /// Performs a drag-and-drop action from one button element to another.
        /// </summary>
        /// <param name="buttonAName">The name/identifier of the source button element.</param>
        /// <param name="buttonBName">The name/identifier of the target button element.</param>
        /// <returns>True if the drag-and-drop action was successful; otherwise, false.</returns>
        public bool DragAToB(string buttonAName, string buttonBName)
        {
            DebugOutput.Log($"DragAToB {buttonAName} {buttonBName}");
            return ElementInteraction.ClickNthElement(CurrentPage, buttonAName, buttonBName, elementType);
        }
    }
}
