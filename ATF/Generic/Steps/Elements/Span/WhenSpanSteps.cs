using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Span
{
    [Binding]
    public class WhenSpanSteps : StepsBase
    {
        public WhenSpanSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Click On Span ""([^""]*)""")]
        public void WhenIClickOnSpan(string text)
        {
            string proc = $"When I Click On Span {text}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Span.ClickOnLinkByName(text))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
