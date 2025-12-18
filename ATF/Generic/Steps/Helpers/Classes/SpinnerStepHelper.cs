using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for handling spinner UI element interactions in automated tests.
    /// Provides methods to check if a spinner is displayed or has disappeared from the page.
    /// </summary>
    public class SpinnerStepHelper : StepHelper, ISpinnerStepHelper
    {
        // Dependency for managing target forms in the application
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the SpinnerStepHelper class.
        /// </summary>
        /// <param name="featureContext">The feature context for the current test scenario</param>
        /// <param name="targetForms">The target forms interface for form interactions</param>
        public SpinnerStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Checks if the spinner element is still being displayed on the current page.
        /// Waits for the spinner to be displayed with a configured timeout.
        /// </summary>
        /// <returns>
        /// True if the spinner is displayed; otherwise false.
        /// </returns>
        public bool StillDisplayed()
        {
            // Log the method execution for debugging purposes
            DebugOutput.Log($"Proc - StillDisplayed");
            
            // Wait for spinner element to appear on the current page
            return ElementInteraction.WaitForElementToBeDisplayed(CurrentPage, "spinner", "spinner");
        }

        /// <summary>
        /// Checks if the spinner element has disappeared from the current page.
        /// Waits for the spinner to no longer be displayed with a configured timeout.
        /// </summary>
        /// <returns>
        /// True if the spinner is no longer displayed; otherwise false.
        /// </returns>
        public bool SpinnerIsGone()
        {
            // Log the method execution for debugging purposes
            DebugOutput.Log($"Proc - SpinnerIsGone");
            
            // Wait for spinner element to disappear from the current page
            return ElementInteraction.WaitForElementToNotBeDisplayed(CurrentPage, "spinner", "spinner");
        }

    }
}
