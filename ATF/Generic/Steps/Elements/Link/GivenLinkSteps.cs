using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Link
{
    [Binding]
    public class GivenLinkSteps : StepsBase
    {
        public GivenLinkSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Link ""([^""]*)"" Is Displayed")]
        public void GivenLinkIsDisplayed(string linkName)
        {
            string proc = $"Given Link {linkName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Link.IsDisplayed(linkName))
                {
                    return;
                }
                Assert.Fail(proc + "FAILED");
                return;
            }
            Assert.Inconclusive();
        }


    }
}
