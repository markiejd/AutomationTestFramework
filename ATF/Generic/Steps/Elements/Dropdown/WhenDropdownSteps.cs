using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Dropdown
{
    [Binding]
    public class WhenDropdownSteps : StepsBase
    {
        public WhenDropdownSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        [When(@"I Select ""([^""]*)"" From DropDown ""([^""]*)""")]
        [When(@"I Select ""([^""]*)"" From Dropdown ""([^""]*)""")]
        public void WhenISelectFromDropdown(string selecting, string dropdownName)
        {
            string proc = $"When I Select {selecting} From Dropdown {dropdownName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Dropdown.SelectingFromWithoutText(selecting, dropdownName, 0, false))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
        
        [When(@"I Change Selected ""(.*)"" From DropDown ""(.*)""")]
        public void WhenIChangeSelectedFromDropDown(string selecting,string dropdownName)
        {
            string proc = $"When I Change Selected {selecting} From Dropdown {dropdownName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Dropdown.SelectingFromWithoutText(selecting, dropdownName, 0, true))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Enter And Select ""([^""]*)"" From DropDown ""([^""]*)""")]
        public void WhenIEnterAndSelectFromDropDown(string selecting, string dropdownName)
        {
            string proc = $"When I Enter And Select {selecting} From Dropdown {dropdownName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Dropdown.EnterThenSelectFrom(selecting, dropdownName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [When(@"I Click On DropDown ""(.*)""")]
        public void WhenIClickOnDropDown(string dropdownName)
        {            
            string proc = $"When I Click On DropDown {dropdownName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Dropdown.Click(dropdownName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }





    }
}
