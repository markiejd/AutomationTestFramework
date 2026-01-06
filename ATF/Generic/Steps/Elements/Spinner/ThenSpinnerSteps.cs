using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Spinner
{
    [Binding]
    public class ThenSpinnerSteps : StepsBase
    {
        public ThenSpinnerSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Spinner Is Displayed")]
        public void ThenSpinnerIsDisplayed()
        {
            string proc = $"Then Spinner Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Spinner.StillDisplayed())
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
    }
}
