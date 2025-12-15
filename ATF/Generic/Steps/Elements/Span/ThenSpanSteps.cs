using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Span
{
    [Binding]
    public class ThenSpanSteps : StepsBase
    {
        public ThenSpanSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Span ""([^""]*)"" Is Displayed")]
        public void ThenSpanIsDisplayed(string spanText)
        {
            string proc = $"Then Span {spanText} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Span.LinkDisplayedByName(spanText))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
