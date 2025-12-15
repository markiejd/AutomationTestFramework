using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Slider
{
    [Binding]
    public class ThenSliderSteps : StepsBase
    {
        public ThenSliderSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Slider ""([^""]*)"" Is Displayed")]
        public void ThenSliderIsDisplayed(string slider)
        {
            string proc = $"Then Slider {slider} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Slider.IsDisplayed(slider))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



}
}
