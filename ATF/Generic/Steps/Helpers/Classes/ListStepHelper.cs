using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for list-related step operations in automated testing.
    /// Provides methods to interact with and validate list elements on web pages.
    /// </summary>
    public class ListStepHelper : StepHelper, IListStepHelper
    {
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the ListStepHelper class.
        /// </summary>
        /// <param name="featureContext">The feature context containing test execution context.</param>
        /// <param name="targetForms">The target forms interface for form interactions.</param>
        public ListStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Verifies if a specified list element is displayed on the current page.
        /// </summary>
        /// <param name="list">The identifier of the list element to check.</param>
        /// <returns>True if the list is displayed; otherwise, false.</returns>
        public bool IsDisplayed(string list)
        {
            DebugOutput.Log($"proc - IsDisplayed {list}");
            // Check if the list element is visible on the current page
            return ElementInteraction.IsElementDisplayed(CurrentPage, list, "list");
        }

        /// <summary>
        /// Checks if a list contains a specific value among its options.
        /// </summary>
        /// <param name="list">The identifier of the list element.</param>
        /// <param name="value">The value to search for in the list.</param>
        /// <returns>True if the list contains the specified value; otherwise, false.</returns>
        public bool ListContainsValue(string list, string value)
        {
            DebugOutput.Log($"proc - ListContainsValue {list} {value}");
            
            // Retrieve all text values from the list elements
            var listOfOptions = ElementInteraction.GetSubElementsTextOfElement(CurrentPage, list, "list");
            
            // Return false if the list is empty or null
            if (listOfOptions == null) 
                return false;
            
            // Iterate through all options and check for an exact match
            foreach (var option in listOfOptions)
            {
                if (option == value) 
                    return true;
            }
            
            // Value was not found in the list
            return false;
        }
    }
}
