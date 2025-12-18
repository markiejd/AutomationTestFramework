using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for slider-related step operations in automation tests.
    /// Provides methods to interact with slider elements such as checking visibility and setting values.
    /// </summary>
    public class SliderStepHelper : StepHelper, ISliderStepHelper
    {
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the SliderStepHelper class.
        /// </summary>
        /// <param name="featureContext">The Reqnroll feature context used for test execution.</param>
        /// <param name="targetForms">The target forms interface for form-related operations.</param>
        public SliderStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Checks if a slider element is displayed on the current page.
        /// </summary>
        /// <param name="sliderName">The name of the slider element to check.</param>
        /// <returns>True if the slider is displayed; otherwise, false.</returns>
        public bool IsDisplayed(string sliderName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, sliderName, "Slider");
        }

        /// <summary>
        /// Sets the slider value by moving it to a specified position.
        /// </summary>
        /// <param name="sliderName">The name of the slider element to move.</param>
        /// <param name="value">The distance (in pixels or units) to move the slider.</param>
        /// <returns>True if the slider was moved successfully; otherwise, false.</returns>
        public bool SetSliderValue(string sliderName, string value)
        {
            DebugOutput.Log($"proc - SetSliderValue {sliderName}");
            int move = 0;
            try
            {
                // Attempt to convert the string value to an integer for slider movement
                move = int.Parse(value);
            }
            catch
            {
                // Log and return false if conversion fails (invalid input format)
                DebugOutput.Log($"Failed to convert {value} to int!");
                return false;
            }
            
            // Move the slider element by the calculated amount
            return ElementInteraction.MoveSliderElement(CurrentPage, sliderName, "slider", move);
        }

        /// <summary>
        /// Enters a text value into a slider element and triggers a keyboard action.
        /// </summary>
        /// <param name="sliderName">The name of the slider element to interact with.</param>
        /// <param name="value">The text value to enter into the slider element.</param>
        /// <returns>True if the text was entered and key was pressed successfully; otherwise, false.</returns>
        public bool EnterSliderValue(string sliderName, string value)
        {
            DebugOutput.Log($"proc - EnterSliderValue {sliderName}");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, sliderName, "slider", value);
        }
    }
}
