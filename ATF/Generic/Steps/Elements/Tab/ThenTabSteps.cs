using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Tab
{
    [Binding]
    public class ThenTabSteps : StepsBase
    {
        public ThenTabSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Tabs ""([^""]*)"" Is Displayed")]
        public void ThenTabsIsDisplayed(string tabs)
        {
            tabs = tabs.ToLower();
            string proc = $"Then Tabs {tabs} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.IsDisplayed(tabs)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Tabs ""([^""]*)"" Contains (.*) Tabs")]
        public void ThenTabsContainsTabs(string tabs, int expecteNumberOfTabs)
        {
            tabs = tabs.ToLower();
            string proc = $"Then Tabs {tabs} Contains {expecteNumberOfTabs} Tabs";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.GetNumberOfTabsInTabs(tabs) == expecteNumberOfTabs) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"Tabs ""(.*)"" Contains Tab ""(.*)""")]
        public void ThenTabsContainsTab(string tabs,string tabName)
        {
            tabs = tabs.ToLower();
            string proc = $"Then Tabs {tabs} Contains {tabName} Tab";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.TabContainedInTabs(tabs, tabName)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"Tabs ""(.*)"" Does Not Contain Tab ""(.*)""")]
        public void ThenTabsDoesNotContainTab(string tabs,string tabName)
        {
            tabs = tabs.ToLower();
            string proc = $"Then Tabs {tabs} Does Not Contain Tab {tabName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Tab.TabContainedInTabs(tabs, tabName)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }



        [Then(@"Tab ""([^""]*)"" Is Selected In Tabs ""([^""]*)""")]
        public void ThenTabIsSelectedInTabs(string tab, string tabs)
        {
            tabs = tabs.ToLower();
            string proc = $"Then Tab {tab} Is Selected In Tabs {tabs}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tab.WhatTabIsSelected(tabs) == tab)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }







    }
}
