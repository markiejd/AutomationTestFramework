using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for handling link-related step operations in automated tests.
    /// Provides methods to interact with link elements on the current page.
    /// </summary>
    public class LinkStepHelper : StepHelper, ILinkStepHelper
    {
        private readonly ITargetForms targetForms;
        
        /// <summary>
        /// Initializes a new instance of the LinkStepHelper class.
        /// </summary>
        /// <param name="featureContext">The Reqnroll feature context for the current scenario.</param>
        /// <param name="targetForms">The target forms service for form interactions.</param>
        public LinkStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Attempts to click on a link element identified by the specified name.
        /// </summary>
        /// <param name="linkName">The name or identifier of the link to click.</param>
        /// <returns>True if the click action was successful; otherwise, false.</returns>
        public bool ClickLink(string linkName)
        {
            // Log the operation for debugging purposes
            DebugOutput.Log($"proc - IsDisplayed {linkName}");
            
            // Perform the click action on the element and return the result
            return ElementInteraction.ClickOnElement(CurrentPage, linkName, "Link");
        }

        /// <summary>
        /// Checks if a link element identified by the specified name is displayed on the current page.
        /// </summary>
        /// <param name="linkName">The name or identifier of the link to check.</param>
        /// <returns>True if the link is displayed; otherwise, false.</returns>
        public bool IsDisplayed(string linkName)
        {
            // Log the operation for debugging purposes
            DebugOutput.Log($"proc - IsDisplayed {linkName}");
            
            // Verify the element visibility and return the result
            return ElementInteraction.IsElementDisplayed(CurrentPage, linkName, "Link");
        }

    }
}
