using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class TimePickerStepHelper : StepHelper, ITimePickerStepHelper
    {
        private readonly ITargetForms targetForms;
        public TimePickerStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }



        // private readonly By[] TimePickerButtonOpen = LocatorValues.locatorParser(TargetLocator.Configuration.TimePickerButtonOpen);
        // private readonly By[] TimePickerText = LocatorValues.locatorParser(TargetLocator.Configuration.TimePickerText);


        public bool IsDisplayed(string timePickerName, int timeout = 0)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, timePickerName, "TimePicker");
        }

        public bool SetTimeValue(string timePickerName, string time, int timeOut = 0)
        {
            DebugOutput.Log($"SetTimeValue {timePickerName} {time}");
            return false;
        }

        public bool EnterValueInTimePicker(string timePickerName, string time, int timeOut)
        {
            DebugOutput.Log($"EnterValueInDatePicker {timePickerName} {time}");
            return false;
        }

        public string? GetCurrentValue(string timePickerName, int timeOut)
        {
            DebugOutput.Log($"GetCurrentValue {timePickerName}");
            return null;

        }

    }
}
