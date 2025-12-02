using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Button
{
    [Binding]
    public class WhenSliderSteps : StepsBase
    {
        public WhenSliderSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Set Slider ""([^""]*)"" To ""([^""]*)""")]
        public void WhenISetSliderTo(string slider, string value)
        {
            string proc = $"When I Set Slider {slider} To {value}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Slider.SetSliderValue(slider, value))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Enter ""([^""]*)"" In Slider ""([^""]*)""")]
        public void WhenIEnterInSlider(string value, string slider)
        {
            string proc = $"When I Enter {value} In Slider {slider}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Slider.EnterSliderValue(slider, value)) 
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
