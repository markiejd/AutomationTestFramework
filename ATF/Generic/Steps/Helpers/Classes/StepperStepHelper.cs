using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for interacting with Stepper UI components in automation tests.
    /// Provides methods to check stepper visibility, step status, and retrieve stepper metadata.
    /// </summary>
    public class StepperStepHelper : StepHelper, IStepperStepHelper
    {
        private readonly ITargetForms targetForms;
        
        /// <summary>
        /// Initializes a new instance of the StepperStepHelper class.
        /// </summary>
        /// <param name="featureContext">The feature context for the current test scenario</param>
        /// <param name="targetForms">Service for interacting with target forms</param>
        public StepperStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Checks if a stepper component is displayed on the current page.
        /// </summary>
        /// <param name="stepperName">The name of the stepper component to check</param>
        /// <returns>True if the stepper is displayed; otherwise, false</returns>
        public bool IsDisplayed(string stepperName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, stepperName, "Stepper");
        }

        /// <summary>
        /// Checks if a specific step within a stepper is displayed.
        /// </summary>
        /// <param name="stepperName">The name of the parent stepper component</param>
        /// <param name="stepName">The name of the step to check</param>
        /// <returns>True if the step is displayed; otherwise, false</returns>
        public bool IsStepDisplayed(string stepperName, string stepName)
        {
            DebugOutput.Log($"IsStepDisplayed {stepperName} {stepName}");
            return ElementInteraction.IsSubElementDisplayed(CurrentPage, stepperName, "Stepper", stepName);
        }

        /// <summary>
        /// Retrieves the total number of steps in a stepper component.
        /// </summary>
        /// <param name="stepperName">The name of the stepper component</param>
        /// <returns>The number of steps in the stepper; 0 if no steps are found</returns>
        public int GetNumberOfSteps(string stepperName)
        {
            DebugOutput.Log($"IsDisplayed {stepperName}");
            // Returns 0 if the element count is null (no steps found)
            return ElementInteraction.GetTheNumberOfSubElementsOfElement(CurrentPage, stepperName, "Stepper") ?? 0;
        }

        /// <summary>
        /// Gets the current status of a specific step within a stepper component.
        /// </summary>
        /// <param name="stepperName">The name of the parent stepper component</param>
        /// <param name="stepName">The name of the step</param>
        /// <returns>The status of the step (e.g., "valid", "invalid", "disabled", "progress"); null if status cannot be determined</returns>
        /// <remarks>
        /// Note: This method currently returns null. The implementation logic has been commented out 
        /// and should be reviewed and uncommented when the status retrieval functionality is needed.
        /// </remarks>
        public string? GetStatusOfStep(string stepperName, string stepName)
        {
            DebugOutput.Log($"IsDisplayed {stepperName}");
            return null;
            
            // TODO: Uncomment the following lines when ready to retrieve step status from CSS class attributes
            // var classReturn = ElementInteraction.GetAttributeValueOfSubElementByNameOfElement(CurrentPage, stepperName, "stepper", stepName, "class");
            // if (classReturn == null) return null;
            // DebugOutput.Log($"WE have the class OF '{classReturn   }'");
            // var actualStatus = FixStatusByClass(classReturn);
            // return actualStatus;
        }

        /// <summary>
        /// Determines the step status based on CSS class names.
        /// </summary>
        /// <param name="classText">The CSS class string to parse</param>
        /// <returns>The normalized status string ("invalid", "valid", "disabled", "progress"); null if no recognized status is found</returns>
        /// <remarks>
        /// This method performs case-insensitive matching on common status indicators in CSS classes.
        /// "empty" class is treated as "invalid" for status reporting purposes.
        /// </remarks>
        private string? FixStatusByClass(string classText)
        {
            DebugOutput.Log($"GetStepElements {classText} ");
            
            // Convert to lowercase for case-insensitive matching
            classText = classText.ToLower();
            
            // Check for status indicators in order of priority
            if (classText.Contains("invalid")) return "invalid";
            if (classText.Contains("valid")) return "valid";
            if (classText.Contains("empty")) return "invalid"; // Treat empty as invalid
            if (classText.Contains("disabled")) return "disabled";
            if (classText.Contains("progress")) return "progress";
            
            // Return null if no recognized status is found
            return null;
        }

    }
}
