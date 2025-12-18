using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for handling dropdown/select element interactions in automated tests.
    /// Provides methods for clicking, selecting values, validating options, and retrieving dropdown data.
    /// </summary>
    public class DropdownStepHelper : StepHelper, IDropdownStepHelper
    {
        private readonly ITargetForms targetForms;
        
        public DropdownStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }        

        /// <summary>
        /// Element type constant used to identify dropdown elements in the DOM.
        /// </summary>
        private string elementType = "Button";

        /// <summary>
        /// Clicks on a dropdown element to open it or trigger its action.
        /// </summary>
        /// <param name="dropdownName">The name or identifier of the dropdown element to click</param>
        /// <returns>True if the click action was successful; otherwise, false</returns>
        public bool Click(string dropdownName)
        {
            DebugOutput.Log($"Click {dropdownName}");
            return ElementInteraction.ClickOnElement(CurrentPage, dropdownName, elementType);
        }

        /// <summary>
        /// Verifies if a dropdown element is displayed on the page.
        /// </summary>
        /// <param name="dropdownName">The name or identifier of the dropdown element</param>
        /// <param name="timeout">Optional timeout in milliseconds to wait for element visibility (default: 0)</param>
        /// <returns>True if the dropdown is displayed; otherwise, false</returns>
        public bool IsDisplayed(string dropdownName, int timeout = 0)
        {
            DebugOutput.Log($"IsDisplayed {dropdownName}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, dropdownName, "DropDown");
        }

        /// <summary>
        /// Types text into a dropdown and selects an option by pressing Enter.
        /// Useful for searchable/filterable dropdown implementations.
        /// </summary>
        /// <param name="selecting">The text value to enter and select</param>
        /// <param name="dropDownName">The name or identifier of the dropdown element</param>
        /// <param name="timeOut">Optional timeout in milliseconds (default: 0)</param>
        /// <returns>True if the text entry and selection was successful; otherwise, false</returns>
        public bool EnterThenSelectFrom(string selecting, string dropDownName, int timeOut = 0)
        {
            // Apply any necessary string transformations/replacements before entering text
            selecting = StringValues.TextReplacementService(selecting);
            DebugOutput.Log($"Proc - EnterThenSelectFrom {selecting} {dropDownName}");
            return ElementInteraction.ClickOnElementEnterTextSendKey(CurrentPage, dropDownName, elementType, selecting, "enter");
        }

        /// <summary>
        /// Selects a value from a dropdown without typing text into it.
        /// This method is useful for dropdowns that do not support text input.
        /// </summary>
        /// <param name="selecting">The value to select from the dropdown options</param>
        /// <param name="dropdownName">The name or identifier of the dropdown element</param>
        /// <param name="timeout">Optional timeout in milliseconds (default: 0)</param>
        /// <param name="topOptionAlreadySelected">Flag indicating if the top option is already selected (default: false)</param>
        /// <returns>True if the selection was successful; otherwise, false</returns>
        public bool SelectingFromWithoutText(string selecting, string dropdownName, int timeout = 0, bool topOptionAlreadySelected = false)
        {
            // Apply any necessary string transformations/replacements to the selection value
            selecting = StringValues.TextReplacementService(selecting);
            DebugOutput.Log($"Proc - SelectingFromWithoutText {selecting} {dropdownName} {timeout} {topOptionAlreadySelected}");
            // Delegate to SelectingFrom with textEntry disabled
            return SelectingFrom(selecting, dropdownName, timeout, topOptionAlreadySelected, false);
        }

        /// <summary>
        /// Selects a value from a dropdown with optional text entry support.
        /// Flexible method that can handle both typed and click-based selection.
        /// </summary>
        /// <param name="selecting">The value to select from the dropdown options</param>
        /// <param name="dropdownName">The name or identifier of the dropdown element</param>
        /// <param name="timeout">Optional timeout in milliseconds (default: 0)</param>
        /// <param name="topOptionAlreadySelected">Flag indicating if the top option is already selected (default: false)</param>
        /// <param name="textEntry">Flag indicating if text entry should be attempted before selection (default: true)</param>
        /// <returns>True if the selection was successful; otherwise, false</returns>
        public bool SelectingFrom(string selecting, string dropdownName, int timeout = 0, bool topOptionAlreadySelected = false, bool textEntry = true)
        {
            // Apply any necessary string transformations/replacements to the selection value
            selecting = StringValues.TextReplacementService(selecting);
            DebugOutput.Log($"Proc - SelectingFrom {selecting} {dropdownName}");
            return ElementInteraction.SelectingFrom(CurrentPage, dropdownName, elementType, selecting, topOptionAlreadySelected, textEntry, timeout);
        }

        /// <summary>
        /// Retrieves the currently selected value from a dropdown element.
        /// </summary>
        /// <param name="dropdownName">The name or identifier of the dropdown element</param>
        /// <param name="timeout">Optional timeout in milliseconds (default: 0)</param>
        /// <returns>The currently selected value as a string, or null if no value is selected</returns>
        public string? GetCurrentValue(string dropdownName, int timeout = 0)
        {
            DebugOutput.Log($"Proc - GetCurrentValue {dropdownName}");
            return ElementInteraction.GetSelectionValue(CurrentPage, dropdownName, elementType);
        }

        /// <summary>
        /// Checks if a specific value exists in the dropdown's list of available options.
        /// </summary>
        /// <param name="dropdownName">The name or identifier of the dropdown element</param>
        /// <param name="value">The value to search for in the dropdown options</param>
        /// <param name="timeout">Optional timeout in milliseconds (default: 0)</param>
        /// <returns>True if the value exists in the dropdown; otherwise, false</returns>
        public bool ContainsValue(string dropdownName, string value, int timeout = 0)
        {
            // Apply any necessary string transformations/replacements to the search value
            value = StringValues.TextReplacementService(value);
            DebugOutput.Log($"Proc - ContainsValue {dropdownName} {value}");
            
            // Retrieve all available options from the dropdown
            var listOfOptions = ElementInteraction.GetSelectionValues(CurrentPage, dropdownName, elementType);
            
            // Return false if the dropdown has no options or retrieval failed
            if (listOfOptions == null) return false;
            if (listOfOptions.Count == 0) return false;
            
            // Search for the target value in the list of options
            foreach (var option in listOfOptions)
            {
                if (option == value) return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves all available values/options from a dropdown element.
        /// </summary>
        /// <param name="dropdownName">The name or identifier of the dropdown element</param>
        /// <param name="timeout">Optional timeout in milliseconds (default: 0)</param>
        /// <returns>A list of all available dropdown options; returns an empty list if none are found</returns>
        public List<string> GetAllValues(string dropdownName, int timeout = 0)
        {
            DebugOutput.Log($"Proc - GetCurrentValue {dropdownName}");
            // Initialize an empty list as fallback if retrieval returns null
            var listOfValues = new List<string>();
            // Return the retrieved values or the empty fallback list
            return ElementInteraction.GetSelectionValues(CurrentPage, dropdownName, elementType) ?? listOfValues;
        }

    }
}
