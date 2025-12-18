using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for handling chip element interactions in the UI automation framework.
    /// Provides methods to interact with chip components and chip arrays.
    /// </summary>
    public class ChipStepHelper : StepHelper, IChipStepHelper
    {
        private readonly ITargetForms targetForms;
        
        public ChipStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// The UI element type identifier for chip elements.
        /// </summary>
        private string elementType = "Chip";


        /// <summary>
        /// Checks if a specific chip is contained within a chip array.
        /// </summary>
        /// <param name="chipArraryName">The name of the chip array container to search within</param>
        /// <param name="chipName">The name/text of the chip to locate</param>
        /// <returns>True if the chip is found and displayed in the array; otherwise false</returns>
        public bool ArraryContainsChip(string chipArraryName, string chipName)
        {
            // Apply any necessary text transformations or replacements to the chip name
            chipName = StringValues.TextReplacementService(chipName);
            DebugOutput.Log($"ArraryContainsChip {chipArraryName} {chipName}");
            
            // Check if the chip element is displayed under the specified array
            return ElementInteraction.IsElementUnderElementByTextDisplayed(CurrentPage, chipArraryName, elementType, chipName);
        }

        /// <summary>
        /// Clicks on a specific chip within a chip array.
        /// </summary>
        /// <param name="chipArrayName">The name of the chip array container</param>
        /// <param name="chipName">The name/text of the chip to click</param>
        /// <returns>True if the click action was successful; otherwise false</returns>
        public bool ClickChip(string chipArrayName, string chipName)
        {
            DebugOutput.Log($"ClickChip {chipArrayName} {chipName}");
            
            // Locate and click the chip element by its text within the specified array
            return ElementInteraction.ClickOnSubElementByTextUnderElement(CurrentPage, chipArrayName, elementType, chipName);
        }

        /// <summary>
        /// Closes or removes a specific chip from a chip array.
        /// </summary>
        /// <param name="chipArrayName">The name of the chip array container</param>
        /// <param name="chipName">The name/text of the chip to close</param>
        /// <returns>True if the close action was successful; otherwise false</returns>
        public bool CloseChip(string chipArrayName, string chipName)
        {
            // Apply any necessary text transformations or replacements to the chip name
            chipName = StringValues.TextReplacementService(chipName);
            DebugOutput.Log($"CloseChip {chipArrayName} {chipName}");
            
            // TODO: Implement the close chip functionality
            // The actual implementation is commented out and needs to be completed
            return false;
            // return ElementInteraction.ClickOnSubElementByTagSubElementByClassByTextByTag(CurrentPage, chipArrayName, elementType, "chipName");
        }

        /// <summary>
        /// Verifies if a chip array is currently displayed in the UI.
        /// </summary>
        /// <param name="chipArrayName">The name of the chip array container to check</param>
        /// <returns>True if the chip array is visible; otherwise false</returns>
        public bool IsDisplayed(string chipArrayName)
        {
            // Check if the chip array element is displayed on the current page
            return ElementInteraction.IsElementDisplayed(CurrentPage, chipArrayName, "ChipArray");
        }


    }
}
