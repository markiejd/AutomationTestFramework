using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for managing span element interactions in automated tests.
    /// Handles link-related operations such as checking visibility and clicking on links.
    /// </summary>
    public class SpanStepHelper : StepHelper, ISpanStepHelper
    {
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the SpanStepHelper class.
        /// </summary>
        /// <param name="featureContext">The feature context for the current test execution.</param>
        /// <param name="targetForms">Service for managing target forms interactions.</param>
        public SpanStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Verifies whether a link with the specified name is displayed on the current page.
        /// </summary>
        /// <param name="linkName">The name of the link to check for visibility.</param>
        /// <returns>True if the link is displayed; otherwise, false.</returns>
        public bool LinkDisplayedByName(string linkName)
        {
            // Log the operation for debugging purposes
            DebugOutput.Log($"proc - LinkDisplayedByName {linkName}");
            
            // Check if the element with the specified name exists and is visible on the current page
            return ElementInteraction.IsElementDisplayed(CurrentPage, linkName, "link");
        }

        /// <summary>
        /// Clicks on a link with the specified name on the current page.
        /// </summary>
        /// <param name="linkName">The name of the link to click.</param>
        /// <returns>True if the click operation was successful; otherwise, false.</returns>
        public bool ClickOnLinkByName(string linkName)
        {
            // Log the operation for debugging purposes
            DebugOutput.Log($"proc - ClickOnLinkByName {linkName}");
            
            // Perform a click action on the element with the specified name
            return ElementInteraction.ClickOnElement(CurrentPage, linkName, "link");
        }
    }
}
