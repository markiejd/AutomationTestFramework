using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class DatePickerStepHelper : StepHelper, IDatePickerStepHelper
    {
        private readonly ITargetForms targetForms;
        public DatePickerStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        // private readonly By[] DatePickerText = LocatorValues.locatorParser(TargetLocator.Configuration.DatePickerText);

        public bool IsDisplayed(string datePickerName, int timeout = 0)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, datePickerName, "DatePicker");
        }

        public bool SetDateValue(string datePickerName, string date, string dateFormat = "", int timeOut = 0)
        {
            DebugOutput.Log($"SetDateValue {datePickerName} {date}");
            var dateTime = DateValues.GetDateTimeFromDateString(date) ?? DateTime.Now;
            DebugOutput.Log($"SetDateValue dateTime {dateTime}");
            return ElementInteraction.ClickOnSelectYearPickerOfDatePicker(CurrentPage, datePickerName, "datepicker", dateTime);
        }

        public bool EnterValueInDatePicker(string datePickerName, string date, int timeOut)
        {
            DebugOutput.Log($"EnterValueInDatePicker {datePickerName} {date}");
            return false;
        }

        public string GetCurrentValue(string datePickerName, int timeOut)
        {
            DebugOutput.Log($"GetCurrentValue {datePickerName}");
            return "";
        }

    }
}
