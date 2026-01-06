using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Dropdown
{
    [Binding]
    public class ThenDropdownSteps : StepsBase
    {
        public ThenDropdownSteps(IStepHelpers helpers) : base(helpers)
        {
        }



        [Then(@"DropDown ""(.*)"" Value Is ""(.*)""")]
        [Then(@"DropDown ""(.*)"" Is Equal To ""(.*)""")]
        [Then(@"Dropdown ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void ThenDropdownIsEqualTo(string dropdownName, string value)
        {
            string proc = $"Then Dropdown {dropdownName} Is Equal To {value}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Dropdown.GetCurrentValue(dropdownName) == value)
                {
                    return;
                }
                if (value == "")
                {
                    if (Helpers.Dropdown.GetCurrentValue(dropdownName) == null)
                    {
                        DebugOutput.Log($"Check this - null does not equal empty string but it should");
                        return;
                    }
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"DropDown ""([^""]*)"" Is Displayed")]
        [Then(@"Dropdown ""([^""]*)"" Is Displayed")]
        public void ThenDropdownIsDisplayed(string dropdownName)
        {
            string proc = $"Then Dropdown {dropdownName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Dropdown.IsDisplayed(dropdownName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"DropDown ""(.*)"" Contains Option ""(.*)""")]
        [Then(@"DropDown ""([^""]*)"" Contains Option ""([^""]*)""")]
        public void ThenDropDownContainsOption(string dropdownName,string value)
        {
            string proc = $"Then Dropdown {dropdownName} Contains Option {value}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Dropdown.ContainsValue(dropdownName, value))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"DropDown ""([^""]*)"" Is Not Displayed")]
        [Then(@"Dropdown ""([^""]*)"" Is Not Displayed")]
        public void ThenDropdownIsNotDisplayed(string dropdownName)
        {
            string proc = $"Then Dropdown {dropdownName} Is Not Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Dropdown.IsDisplayed(dropdownName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
