using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.List
{
    [Binding]
    public class ThenListSteps : StepsBase
    {
        public ThenListSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [Then(@"List ""(.*)"" Is Displayed")]
        public void ThenListIsDisplayed(string listName)
        {
            string proc = $"Then List {listName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.List.IsDisplayed(listName)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"List ""([^""]*)"" Contains ""([^""]*)""")]
        public void ThenListContains(string listName, string value)
        {
            string proc = $"Then List {listName} Contains {value}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.List.ListContainsValue(listName, value))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}

