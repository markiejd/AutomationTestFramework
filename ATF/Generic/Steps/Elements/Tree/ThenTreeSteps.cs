using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Tree
{
    [Binding]
    public class ThenTreeSteps : StepsBase
    {
        public ThenTreeSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Tree ""([^""]*)"" Is Displayed")]
        public void ThenTreeIsDisplayed(string treeName)
        {
            string proc = $"Then Tree {treeName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tree.IsDisplayed(treeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"Tree ""(.*)"" Is Not Displayed")]
        public void ThenTreeIsNotDisplayed(string treeName)
        {
            string proc = $"Then Tree {treeName} Is Not Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Tree.IsDisplayed(treeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"Tree ""(.*)"" Does Not Contain Node ""(.*)""")]
        public void ThenTreeDoesNotContainNode(string treeName,string nodeName)
        {
            string proc = $"Then Tree {treeName} Does Not Contain Node {nodeName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Tree.IsNodeExist(treeName, nodeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



        
        [Then(@"Exists Tree ""(.*)"" Node ""(.*)""")]   
        [Then(@"Tree ""([^""]*)"" Node ""([^""]*)"" Exists")]
        public void ThenTreeNodeExists(string treeName, string nodeName)
        {
            string proc = $"Then Tree {treeName} Node {nodeName} Exists";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tree.IsNodeExist(treeName, nodeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
