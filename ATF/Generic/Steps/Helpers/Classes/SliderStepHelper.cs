using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class SliderStepHelper : StepHelper, ISliderStepHelper
    {
        private readonly ITargetForms targetForms;
        public SliderStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool IsDisplayed(string sliderName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, sliderName, "Slider");
        }

        public bool SetSliderValue(string sliderName, string value)
        {
            DebugOutput.Log($"proc - SetSliderValue {sliderName}");
            int move = 0;
            try
            {
                move = int.Parse(value);
            }
            catch
            {
                DebugOutput.Log($"Failed to convert {value} to int!");
                return false;
            }
            return ElementInteraction.MoveSliderElement(CurrentPage, sliderName, "slider", move);
        }

        public bool EnterSliderValue(string sliderName, string value)
        {
            DebugOutput.Log($"proc - EnterSliderValue {sliderName}");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, sliderName, "slider", value);
        }
        
    }
}
