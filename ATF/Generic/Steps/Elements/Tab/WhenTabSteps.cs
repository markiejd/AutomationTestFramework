using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Tab
{
    [Binding]
    public class WhenTabSteps : StepsBase
    {
        public WhenTabSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Click On Tab ""([^""]*)"" In Tabs ""([^""]*)""")]
        public void WhenIClickOnTabInTabs(string tabNmae, string tabs)
        {
            tabs = tabs.ToLower();
            string proc = $"Given Tab {tabNmae} Is Selected In Tabs {tabs}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.SelectTab(tabs, tabNmae))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
