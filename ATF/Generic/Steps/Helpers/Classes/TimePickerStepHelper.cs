using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper for interacting with TimePicker controls in target forms.
    /// Provides visibility checks, value entry, and retrieval.
    /// </summary>
    public class TimePickerStepHelper : StepHelper, ITimePickerStepHelper
    {
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of <see cref="TimePickerStepHelper"/>.
        /// </summary>
        /// <param name="featureContext">The current feature context (SpecFlow/Reqnroll).</param>
        /// <param name="targetForms">The target forms abstraction used to resolve and interact with controls.</param>
        public TimePickerStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            // Store dependency for later use when interacting with forms/controls
            this.targetForms = targetForms ?? throw new ArgumentNullException(nameof(targetForms));
        }

        // private readonly By[] TimePickerButtonOpen = LocatorValues.locatorParser(TargetLocator.Configuration.TimePickerButtonOpen);
        // private readonly By[] TimePickerText = LocatorValues.locatorParser(TargetLocator.Configuration.TimePickerText);

        /// <summary>
        /// Checks whether a TimePicker control is displayed on the current page.
        /// </summary>
        /// <param name="timePickerName">The logical name/key of the TimePicker control.</param>
        /// <param name="timeout">Optional timeout (ms) to wait for the control to appear.</param>
        /// <returns>True if displayed; otherwise false.</returns>
        public bool IsDisplayed(string timePickerName, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(timePickerName))
            {
                // Defensive check: invalid control name
                DebugOutput.Log("IsDisplayed called with empty timePickerName");
                return false;
            }

            // Delegate to ElementInteraction for standardized locator resolution and visibility check
            return ElementInteraction.IsElementDisplayed(CurrentPage, timePickerName, "TimePicker");
        }

        /// <summary>
        /// Sets the value of a TimePicker control (e.g., "14:30").
        /// </summary>
        /// <param name="timePickerName">The logical name/key of the TimePicker control.</param>
        /// <param name="time">The time value to set (expected format determined by the app under test).</param>
        /// <param name="timeOut">Optional timeout (ms) for setting the value.</param>
        /// <returns>True if the value was set; otherwise false.</returns>
        public bool SetTimeValue(string timePickerName, string time, int timeOut = 0)
        {
            DebugOutput.Log($"SetTimeValue {timePickerName} {time}");

            if (string.IsNullOrWhiteSpace(timePickerName) || string.IsNullOrWhiteSpace(time))
            {
                // Invalid inputs; cannot proceed
                return false;
            }

            // TODO: Implement interaction with TimePicker control via targetForms/ElementInteraction
            // Example approach:
            // 1. Focus the control
            // 2. Clear existing value
            // 3. Enter the new time string
            // 4. Confirm/blur to trigger bindings
            return false;
        }

        /// <summary>
        /// Types a time value directly into the TimePicker text input.
        /// </summary>
        /// <param name="timePickerName">The logical name/key of the TimePicker control.</param>
        /// <param name="time">The time value to enter (e.g., "09:45").</param>
        /// <param name="timeOut">Optional timeout (ms) for typing the value.</param>
        /// <returns>True if the value was entered; otherwise false.</returns>
        public bool EnterValueInTimePicker(string timePickerName, string time, int timeOut)
        {
            DebugOutput.Log($"EnterValueInDatePicker {timePickerName} {time}");

            if (string.IsNullOrWhiteSpace(timePickerName) || string.IsNullOrWhiteSpace(time))
            {
                // Invalid inputs; cannot proceed
                return false;
            }

            // TODO: Use ElementInteraction to enter keys into the TimePicker's editable field
            // Consider sending TAB/ENTER to commit the value depending on control behavior
            return false;
        }

        /// <summary>
        /// Gets the current value from a TimePicker control.
        /// </summary>
        /// <param name="timePickerName">The logical name/key of the TimePicker control.</param>
        /// <param name="timeOut">Optional timeout (ms) for retrieval.</param>
        /// <returns>The current time value as a string, or null if not available.</returns>
        public string? GetCurrentValue(string timePickerName, int timeOut)
        {
            DebugOutput.Log($"GetCurrentValue {timePickerName}");

            if (string.IsNullOrWhiteSpace(timePickerName))
            {
                // Defensive check: invalid control name
                return null;
            }

            // TODO: Query the TimePicker's value attribute/text via ElementInteraction
            // Return the parsed/normalized time string if available
            return null;
        }

    }
}
