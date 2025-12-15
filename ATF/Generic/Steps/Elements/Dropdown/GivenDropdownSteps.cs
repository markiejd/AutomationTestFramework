using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Dropdown
{
    [Binding]
    public class GivenDropdownSteps : StepsBase
    {
        public GivenDropdownSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Dropdown ""([^""]*)"" Is Displayed")]
        public void GivenDropdownIsDisplayed(string dropdownName)
        {
            string proc = $"Given Dropdown {dropdownName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Dropdown.IsDisplayed(dropdownName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
        
        [Given(@"DropDown ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void GivenDropDownIsEqualTo(string dropdownName, string value)
        {
            string proc = $"Given Dropdown {dropdownName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Dropdown.GetCurrentValue(dropdownName) == value)
                {
                    return;
                }
                if (Helpers.Dropdown.SelectingFrom(value, dropdownName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
