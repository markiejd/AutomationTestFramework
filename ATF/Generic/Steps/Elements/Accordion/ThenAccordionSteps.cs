using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Accordion
{
    [Binding]
    public class ThenAccordionSteps : StepsBase
    {
        public ThenAccordionSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [Then(@"Accordion Is Displayed")]
        public void ThenAccordionIsDisplayed()
        {
            ThenAccordionIsDisplayed("Accordion");
        }


        [Then(@"Accordion ""([^""]*)"" Is Displayed")]
        public void ThenAccordionIsDisplayed(string accordianName)
        {
            string proc = $"Then Accordion {accordianName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.IsDisplayed(accordianName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Accordion ""([^""]*)"" Is Expanded")]
        public void ThenAccordionIsExpanded(string accordianName)
        {
            string proc = $"Then Accordion {accordianName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.IsAccordionExpanded(accordianName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Group ""([^""]*)"" In Accordion ""([^""]*)"" Is Expanded")]
        public void ThenGroupInAccordionIsExpanded(string groupName, string accordianName)
        {
            string proc = $"Then Group {groupName} In Accordion {accordianName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.GroupIsExpanded(accordianName, groupName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"Group ""(.*)"" Contained In Accordion ""(.*)""")]
        public void ThenGroupContainedInAccordion(string groupName, string accordianName)
        {
            string proc = $"Then Group {groupName} Contained In Accordion {accordianName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.GroupIsDisplayed(accordianName, groupName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Group ""([^""]*)"" In Accordion ""([^""]*)"" Is Not Expanded")]
        public void ThenGroupInAccordionIsNotExpanded(string groupName, string accordianName)
        {
            string proc = $"Then Group {groupName} In Accordion {accordianName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.GroupIsNotExpanded(accordianName, groupName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Button ""([^""]*)"" In Accordion ""([^""]*)"" Displayed")]
        public void ThenButtonInAccordionDisplayed(string buttonName, string accordianName)
        {
            string proc = $"Then Button {buttonName} In Accordion {accordianName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.IsButtonDisplayed(accordianName, buttonName))    
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
