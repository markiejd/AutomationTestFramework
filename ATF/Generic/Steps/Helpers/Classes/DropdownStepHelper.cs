using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class DropdownStepHelper : StepHelper, IDropdownStepHelper
    {
        private readonly ITargetForms targetForms;
        public DropdownStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }        

        private string elementType = "Button";


        public bool Click(string dropdownName)
        {
            DebugOutput.Log($"Click {dropdownName}");
            return ElementInteraction.ClickOnElement(CurrentPage, dropdownName, elementType);
        }

        public bool IsDisplayed(string dropdownName, int timeout = 0)
        {
            DebugOutput.Log($"IsDisplayed {dropdownName}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, dropdownName, "DropDown");
        }

        public bool EnterThenSelectFrom(string selecting, string dropDownName, int timeOut = 0)
        {
            selecting = StringValues.TextReplacementService(selecting);
            DebugOutput.Log($"Proc - EnterThenSelectFrom {selecting} {dropDownName}");
            return ElementInteraction.ClickOnElementEnterTextSendKey(CurrentPage, dropDownName, elementType, selecting, "enter");
        }

        public bool SelectingFromWithoutText(string selecting, string dropdownName, int timeout = 0, bool topOptionAlreadySelected = false)
        {
            selecting = StringValues.TextReplacementService(selecting);
            DebugOutput.Log($"Proc - SelectingFromWithoutText {selecting} {dropdownName} {timeout} {topOptionAlreadySelected}");
            return SelectingFrom(selecting, dropdownName, timeout, topOptionAlreadySelected, false);
        }

        public bool SelectingFrom(string selecting, string dropdownName, int timeout = 0, bool topOptionAlreadySelected = false, bool textEntry = true)
        {
            selecting = StringValues.TextReplacementService(selecting);
            DebugOutput.Log($"Proc - SelectingFrom {selecting} {dropdownName}");
            return ElementInteraction.SelectingFrom(CurrentPage, dropdownName, elementType, selecting, topOptionAlreadySelected, textEntry, timeout);
        }

        public string? GetCurrentValue(string dropdownName, int timeout = 0)
        {
            DebugOutput.Log($"Proc - GetCurrentValue {dropdownName}");
            return ElementInteraction.GetSelectionValue(CurrentPage, dropdownName, elementType);
        }

        public bool ContainsValue(string dropdownName, string value, int timeout = 0)
        {
            value = StringValues.TextReplacementService(value);
            DebugOutput.Log($"Proc - ContainsValue {dropdownName} {value}");
            var listOfOptions = ElementInteraction.GetSelectionValues(CurrentPage, dropdownName, elementType);
            if (listOfOptions == null) return false;
            if (listOfOptions.Count == 0) return false;
            foreach (var option in listOfOptions)
            {
                if (option == value) return true;
            }
            return false;
        }

        public List<string> GetAllValues(string dropdownName, int timeout = 0)
        {
            DebugOutput.Log($"Proc - GetCurrentValue {dropdownName}");
            var listOfValues = new List<string>();
            return ElementInteraction.GetSelectionValues(CurrentPage, dropdownName, elementType) ?? listOfValues;
        }

    }
}
