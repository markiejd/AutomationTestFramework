using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.Extensions.DependencyModel.Resolution;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for handling switch/toggle element interactions in test steps.
    /// Provides methods to click, check visibility, and retrieve the status of switch elements.
    /// </summary>
    public class SwitchStepHelper : StepHelper, ISwitchStepHelper
    {
        // Dependency for accessing target forms
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the SwitchStepHelper class.
        /// </summary>
        /// <param name="featureContext">The current Reqnroll feature context for test execution tracking</param>
        /// <param name="targetForms">Service for accessing target form definitions</param>
        public SwitchStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Clicks on a switch element identified by the specified name.
        /// </summary>
        /// <param name="switchName">The name of the switch element to click</param>
        /// <returns>True if the click operation was successful; otherwise, false</returns>
        public bool Click(string switchName)
        {
            // Log the action being performed for debugging purposes
            DebugOutput.Log($"Clicking on switch {switchName}");
            
            // Perform the click action on the switch element using ElementInteraction helper
            return ElementInteraction.ClickOnElement(CurrentPage, switchName, "Switch");
        }

        /// <summary>
        /// Checks whether a switch element is currently visible on the page.
        /// </summary>
        /// <param name="switchName">The name of the switch element to check</param>
        /// <returns>True if the switch is displayed; otherwise, false</returns>
        public bool IsDisplayed(string switchName)
        {
            // Log the visibility check for debugging purposes
            DebugOutput.Log($"Checking if switch {switchName} is displayed");
            
            // Verify the element is visible on the current page
            return ElementInteraction.IsElementDisplayed(CurrentPage, switchName, "Switch");
        }

        /// <summary>
        /// Retrieves the current status (on/off state) of a switch element.
        /// </summary>
        /// <param name="switchName">The name of the switch element to check</param>
        /// <returns>True if the switch is in the "on" state; otherwise, false</returns>
        public bool Status(string switchName)
        {
            // Log the status check for debugging purposes
            DebugOutput.Log($"Checking if switch {switchName} is on");
            
            // Check if the switch element is selected (on state)
            return ElementInteraction.IsElementSelected(CurrentPage, switchName, "Switch");
        }

    }
}