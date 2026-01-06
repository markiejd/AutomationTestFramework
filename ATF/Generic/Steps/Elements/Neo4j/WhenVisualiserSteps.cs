using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.List
{
    [Binding]
    public class WhenVisualiserSteps : StepsBase
    {
        public WhenVisualiserSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        [When(@"I Find Entities In Visualiser")]
        public void WhenIFindEntitiesInVisualiser()
        {
            string proc = $"When I Find Entities In Visualiser";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Visualiser.FindEntity())
                {
                    CombinedSteps.Failure(proc);
                    return;
                }
                return;
            }
        }

        
        [When(@"I Press Key ""(.*)"" And Click On Node ""(.*)"" In Visualiser")]
        public bool WhenIPressKeyAndClickOnNodeInVisualiser(string keyDown,string node)
        {
            string proc = $"When I Press Key {keyDown} And Click On Node {node} In Visualiser";
            node = StringValues.TextReplacementService(node);
            proc = $"When I Press Key {keyDown} And Click On Node {node} In Visualiser";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Visualiser.KeyClickOnNode(keyDown, node)) return true;
                CombinedSteps.Failure(proc);
            }
            return false;
        }

        
        [When(@"I Right Click On Node ""(.*)"" In Visualiser")]
        public bool WhenIRightClickOnNodeInVisualiser(string node)
        {
            string proc = $"When I Right Click On Node {node} In Visualiser";
            node = StringValues.TextReplacementService(node);
            proc = $"When I Right Click On Node {node} In Visualiser";
            if (CombinedSteps.OutputProc(proc))
            {
                // if (Helpers.Visualiser.RightClickOnNode(node)) return true;
                CombinedSteps.Failure(proc);
            }
            return false;
        }

        
        [When(@"I Create Link Between Node ""(.*)"" And Node ""(.*)"" In Visualiser")]
        public bool WhenICreateLinkBetweenNodeAndNodeInVisualiser(string node1Text,string node2Text)
        {
            string proc = $"When I Create Link Between Node {node1Text} And Node {node2Text} In Visualiser";
            node1Text = StringValues.TextReplacementService(node1Text);
            node2Text = StringValues.TextReplacementService(node2Text);
            proc = $"When I Create Link Between Node {node1Text} And Node {node2Text} In Visualiser";
            if (CombinedSteps.OutputProc(proc))
            {
                WhenIPressKeyAndClickOnNodeInVisualiser("Ctrl", "Ford");
                WhenIPressKeyAndClickOnNodeInVisualiser("Ctrl", "DuffyEPOCH");
                WhenIRightClickOnNodeInVisualiser("DuffyEPOCH");
                if (Helpers.Visualiser.CreateLinkBetweenNodes(node1Text, node2Text)) return true;
                CombinedSteps.Failure(proc);
            }
            return false;
        }

        
        [When(@"I In Visualiser Click Button ""(.*)""")]
        public bool WhenIInVisualiserClickButton(string visualiserButton)
        {
            string proc = $"When I In Visualiser Click Button {visualiserButton}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Visualiser.ClickVisualiserButton(visualiserButton))
                {
                    Thread.Sleep(1000);
                    return true;
                } 
                CombinedSteps.Failure(proc);
            }
            return false;
        }







    }
}
