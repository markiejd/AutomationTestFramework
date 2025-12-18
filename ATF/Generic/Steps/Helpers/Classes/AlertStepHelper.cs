using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for managing alert interactions in automated tests.
    /// Provides methods to handle browser alerts including verification, acceptance, cancellation, and text input.
    /// </summary>
    public class AlertStepHelper : StepHelper, IAlertStepHelper
    {
        // Dependency for accessing target forms (currently unused but kept for potential future use)
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the AlertStepHelper class.
        /// </summary>
        /// <param name="featureContext">The Reqnroll feature context for managing test state</param>
        /// <param name="targetForms">Interface for target forms interaction</param>
        public AlertStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Verifies if an alert with the specified message is currently displayed.
        /// </summary>
        /// <param name="alertMessage">The expected alert message text</param>
        /// <returns>True if the alert is displayed; otherwise, false</returns>
        public bool IsDisplayed(string alertMessage)
        {
            // Delegate to ElementInteraction to check alert visibility
            return ElementInteraction.AlertIsDisplayed(alertMessage);
        }

        /// <summary>
        /// Clicks the "Accept" button on the currently displayed alert.
        /// </summary>
        /// <returns>True if the action succeeded; otherwise, false</returns>
        public bool Accept()
        {
            // Perform accept action on the alert
            return ElementInteraction.AlertClickAccept();
        }

        /// <summary>
        /// Clicks the "Cancel" button on the currently displayed alert.
        /// </summary>
        /// <returns>True if the action succeeded; otherwise, false</returns>
        public bool Cancel()
        {
            // Perform cancel action on the alert
            return ElementInteraction.AlertClickCancel();
        }

        /// <summary>
        /// Sends text to a prompt alert.
        /// </summary>
        /// <param name="text">The text to send to the alert</param>
        /// <returns>True if the text was sent successfully; otherwise, false</returns>
        public bool SendKeys(string text)
        {
            // Send text input to the alert prompt
            return ElementInteraction.AlertSendKeys(text);
        }
    }
}
