using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Tab
{
    [Binding]
    public class GivenTabSteps : StepsBase
    {
        public GivenTabSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Tabs ""([^""]*)"" Is Displayed")]
        [Given(@"Tab ""([^""]*)"" Is Displayed")]
        public void GivenTabIsDisplayed(string tabName)
        {
            string proc = $"Given Tab {tabName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.IsDisplayed(tabName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Given(@"Tab ""([^""]*)"" Is Selected In Tabs ""([^""]*)""")]
        public void GivenTabIsSelectedInTabs(string tabName, string tabs)
        {
            tabs = tabs.ToLower();
            string proc = $"Given Tab {tabName} Is Selected In Tabs {tabs}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.WhatTabIsSelected(tabs) == tabName)
                {
                    return;
                }
                DebugOutput.Log($"Tab {tabName} is NOT selected!");
                if (Helpers.Tab.SelectTab(tabs, tabName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
