using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Link
{
    [Binding]
    public class WhenLinkSteps : StepsBase
    {
        public WhenLinkSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Click Link ""([^""]*)""")]
        public void WhenIClickLink(string linkName)
        {
            string proc = $"When I Click Link {linkName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Link.ClickLink(linkName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
