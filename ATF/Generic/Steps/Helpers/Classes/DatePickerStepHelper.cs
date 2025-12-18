using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for handling date picker interactions in automated tests.
    /// Provides methods to interact with date picker elements on the UI.
    /// </summary>
    public class DatePickerStepHelper : StepHelper, IDatePickerStepHelper
    {
        private readonly ITargetForms targetForms;
        
        // private readonly By[] DatePickerText = LocatorValues.locatorParser(TargetLocator.Configuration.DatePickerText);

        /// <summary>
        /// Initializes a new instance of the DatePickerStepHelper class.
        /// </summary>
        /// <param name="featureContext">The Reqnroll feature context for the current test execution.</param>
        /// <param name="targetForms">Service for interacting with target forms.</param>
        public DatePickerStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Checks if the date picker element is currently displayed on the page.
        /// </summary>
        /// <param name="datePickerName">The name/identifier of the date picker element.</param>
        /// <param name="timeout">Optional timeout in milliseconds for the visibility check. Defaults to 0.</param>
        /// <returns>True if the date picker is displayed; otherwise, false.</returns>
        public bool IsDisplayed(string datePickerName, int timeout = 0)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, datePickerName, "DatePicker");
        }

        /// <summary>
        /// Sets the date value in the date picker by clicking on the year picker and navigating to the desired date.
        /// </summary>
        /// <param name="datePickerName">The name/identifier of the date picker element.</param>
        /// <param name="date">The date string to be set. If invalid or empty, defaults to current date/time.</param>
        /// <param name="dateFormat">Optional date format string. Currently unused but reserved for future implementation.</param>
        /// <param name="timeOut">Optional timeout in milliseconds for the operation. Defaults to 0.</param>
        /// <returns>True if the date was successfully set; otherwise, false.</returns>
        public bool SetDateValue(string datePickerName, string date, string dateFormat = "", int timeOut = 0)
        {
            DebugOutput.Log($"SetDateValue {datePickerName} {date}");
            
            // Parse the date string into a DateTime object; fall back to current date/time if parsing fails
            var dateTime = DateValues.GetDateTimeFromDateString(date) ?? DateTime.Now;
            DebugOutput.Log($"SetDateValue dateTime {dateTime}");
            
            // Click on the year picker and select the desired date
            return ElementInteraction.ClickOnSelectYearPickerOfDatePicker(CurrentPage, datePickerName, "datepicker", dateTime);
        }

        /// <summary>
        /// Enters a date value directly into the date picker input field.
        /// </summary>
        /// <param name="datePickerName">The name/identifier of the date picker element.</param>
        /// <param name="date">The date string to be entered.</param>
        /// <param name="timeOut">Timeout in milliseconds for the operation.</param>
        /// <returns>True if the value was successfully entered; otherwise, false. Currently returns false as implementation is pending.</returns>
        public bool EnterValueInDatePicker(string datePickerName, string date, int timeOut)
        {
            DebugOutput.Log($"EnterValueInDatePicker {datePickerName} {date}");
            
            // TODO: Implement direct text input to date picker field
            return false;
        }

        /// <summary>
        /// Retrieves the current date value displayed in the date picker.
        /// </summary>
        /// <param name="datePickerName">The name/identifier of the date picker element.</param>
        /// <param name="timeOut">Timeout in milliseconds for the operation.</param>
        /// <returns>The current date value as a string. Currently returns an empty string as implementation is pending.</returns>
        public string GetCurrentValue(string datePickerName, int timeOut)
        {
            DebugOutput.Log($"GetCurrentValue {datePickerName}");
            
            // TODO: Implement retrieval of current date picker value
            return "";
        }

    }
}
