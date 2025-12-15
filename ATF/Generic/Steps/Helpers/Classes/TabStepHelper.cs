using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class TabStepHelper : StepHelper, ITabStepHelper
    {
        private readonly ITargetForms targetForms;
        public TabStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string elementType = "Tab";


        public bool IsDisplayed(string tabName)
        {
            DebugOutput.Log($"Proc - IsDisplayed {tabName}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, tabName, elementType);
        }

        public bool TabContainedInTabs(string tabs, string tabName)
        {
            DebugOutput.Log($"Proc - TabContainedInTabs {tabs} {tabName}");
            return ElementInteraction.IsSubElementDisplayed(CurrentPage, tabs, elementType, tabName);
        }



        public bool SelectTab(string tabs, string tabName)
        {
            DebugOutput.Log($"Proc - SelectTab {tabs} {tabName}");
            return ElementInteraction.ClickOnSubElementByTextUnderElement(CurrentPage, tabs, elementType, tabName);
        }

        public string WhatTabIsSelected(string tabName)
        {
            DebugOutput.Log($"Proc - WhatTabIsSelected {tabName}");
            return ElementInteraction.GetTextFromSubElementSelectedOfElement(CurrentPage, tabName, elementType) ?? "";
        }

        public int GetNumberOfTabsInTabs(string tabsName)
        {
            DebugOutput.Log($"Proc - GetNumberOfTabsInTabs {tabsName}");
            return ElementInteraction.GetTheNumberOfSubElementsOfElement(CurrentPage, tabsName, elementType) ?? 0;
        }

    }
}
