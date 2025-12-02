using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ITabStepHelper : IStepHelper
    {
        int GetNumberOfTabsInTabs(string tabsName);
        bool IsDisplayed(string tabName);
        bool SelectTab(string tabs, string tabName);
        bool TabContainedInTabs(string tabs, string tabName);
        string WhatTabIsSelected(string tabName);
    }
}
