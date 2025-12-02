using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class ChipStepHelper : StepHelper, IChipStepHelper
    {
        private readonly ITargetForms targetForms;
        public ChipStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string elementType = "Chip";


        public bool ArraryContainsChip(string chipArraryName, string chipName)
        {
            chipName = StringValues.TextReplacementService(chipName);
            DebugOutput.Log($"ArraryContainsChip {chipArraryName} {chipName}");
            ElementInteraction.IsElementUnderElementByTextDisplayed(CurrentPage, chipArraryName, elementType, chipName);
            return ElementInteraction.IsElementUnderElementByTextDisplayed(CurrentPage, chipArraryName, elementType, chipName);
        }

        public bool ClickChip(string chipArrayName, string chipName)
        {
            DebugOutput.Log($"ClickChip {chipArrayName} {chipName}");
            return ElementInteraction.ClickOnSubElementByTextUnderElement(CurrentPage, chipArrayName, elementType, chipName);
        }

        public bool CloseChip(string chipArrayName, string chipName)
        {
            chipName = StringValues.TextReplacementService(chipName);
            DebugOutput.Log($"CloseChip {chipArrayName} {chipName}");
            return false;
            // return ElementInteraction.ClickOnSubElementByTagSubElementByClassByTextByTag(CurrentPage, chipArrayName, elementType, "chipName");
        }

        public bool IsDisplayed(string chipArrayName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, chipArrayName, "ChipArray");
        }


    }
}
