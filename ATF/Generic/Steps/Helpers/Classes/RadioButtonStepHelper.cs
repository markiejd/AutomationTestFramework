using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for radio button interactions and validations in test automation.
    /// Provides methods to check display state, enabled state, selected state, and to select radio buttons.
    /// </summary>
    public class RadioButtonStepHelper : StepHelper, IRadioButtonStepHelper
    {
        // Reference to target forms for radio button operations
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the RadioButtonStepHelper class.
        /// </summary>
        /// <param name="featureContext">The Reqnroll feature context for test execution</param>
        /// <param name="targetForms">The target forms interface for radio button interactions</param>
        public RadioButtonStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Determines whether a radio button element is displayed on the current page.
        /// </summary>
        /// <param name="radioButtonName">The name identifier of the radio button to check</param>
        /// <returns>True if the radio button is displayed; otherwise, false</returns>
        public bool IsDisplayed(string radioButtonName)
        {
            // Log the method call for debugging purposes
            DebugOutput.Log($"proc - IsDisplayed {radioButtonName}");
            // Check if the element is visible on the current page using ElementInteraction utility
            return ElementInteraction.IsElementDisplayed(CurrentPage, radioButtonName, "radiobutton");
        }

        /// <summary>
        /// Determines whether a radio button element is enabled and can be interacted with.
        /// </summary>
        /// <param name="radioButtonName">The name identifier of the radio button to check</param>
        /// <returns>True if the radio button is enabled; otherwise, false</returns>
        public bool IsEnabled(string radioButtonName)
        {
            // Log the method call for debugging purposes
            DebugOutput.Log($"proc - IsEnabled {radioButtonName}");
            // Check if the element is enabled on the current page using ElementInteraction utility
            return ElementInteraction.IsElementEnabled(CurrentPage, radioButtonName, "radiobutton");
        }

        /// <summary>
        /// Determines whether a radio button element is currently selected.
        /// </summary>
        /// <param name="radioButtonName">The name identifier of the radio button to check</param>
        /// <returns>True if the radio button is selected; otherwise, false</returns>
        public bool IsSelected(string radioButtonName)
        {
            // Log the method call for debugging purposes
            DebugOutput.Log($"proc - IsSelected {radioButtonName}");
            // Check if the element is in the selected state using ElementInteraction utility
            return ElementInteraction.IsElementSelected(CurrentPage, radioButtonName, "radiobutton");
        }

        /// <summary>
        /// Selects a radio button element on the current page.
        /// </summary>
        /// <param name="radioButtonName">The name identifier of the radio button to select</param>
        /// <returns>True if the radio button was successfully selected; otherwise, false</returns>
        public bool Select(string radioButtonName)
        {
            // Log the selection action for debugging purposes
            DebugOutput.Log($"Select {radioButtonName}");
            // Click on the radio button element to select it using ElementInteraction utility
            return ElementInteraction.ClickOnElement(CurrentPage, radioButtonName, "radiobutton");
        }

    }
}
