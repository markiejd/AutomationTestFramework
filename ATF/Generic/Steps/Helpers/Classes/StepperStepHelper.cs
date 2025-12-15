using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class StepperStepHelper : StepHelper, IStepperStepHelper
    {
        private readonly ITargetForms targetForms;
        public StepperStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }


        public bool IsDisplayed(string stepperName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, stepperName, "Stepper");
        }

        public bool IsStepDisplayed(string stepperName, string stepName)
        {
            DebugOutput.Log($"IsStepDisplayed {stepperName} {stepName}");
            return ElementInteraction.IsSubElementDisplayed(CurrentPage, stepperName, "Stepper", stepName);
        }

        public int GetNumberOfSteps(string stepperName)
        {
            DebugOutput.Log($"IsDisplayed {stepperName}");
            return ElementInteraction.GetTheNumberOfSubElementsOfElement(CurrentPage, stepperName, "Stepper") ?? 0;
        }

        public string? GetStatusOfStep(string stepperName, string stepName)
        {
            DebugOutput.Log($"IsDisplayed {stepperName}");
            return null;
            
            // var classReturn = ElementInteraction.GetAttributeValueOfSubElementByNameOfElement(CurrentPage, stepperName, "stepper", stepName, "class");
            // if (classReturn == null) return null;
            // DebugOutput.Log($"WE have the class OF '{classReturn}   '");
            // var actualStatus = FixStatusByClass(classReturn);
            // return actualStatus;
        }

        private string? FixStatusByClass(string classText)
        {
            DebugOutput.Log($"GetStepElements {classText} ");
            classText = classText.ToLower();
            if (classText.Contains("invalid")) return "invalid";
            if (classText.Contains("valid")) return "valid";
            if (classText.Contains("empty")) return "invalid";
            if (classText.Contains("disabled")) return "disabled";
            if (classText.Contains("progress")) return "progress";
            return null;
        }

    }
}
