using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Accordion
{
    [Binding]
    public class GivenAccordionSteps : StepsBase
    {
        public GivenAccordionSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Accordion ""([^""]*)"" Is Expanded")]
        public void GivenAccordionIsExpanded(string accordionName)
        {
            string proc = $"Given Accordion {accordionName} Is Expanded";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Accordion.IsAccordionExpanded(accordionName)) return;
                DebugOutput.Log($"Need to expand accordion {accordionName}");
                if (Helpers.Accordion.AccordionClick(accordionName)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }





    }
}
