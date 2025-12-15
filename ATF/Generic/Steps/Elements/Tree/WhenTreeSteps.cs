using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Tree
{
    [Binding]
    public class WhenTreeSteps : StepsBase
    {
        public WhenTreeSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [When(@"I Expand Node ""(.*)"" In Tree ""(.*)""")]
        public void WhenIExpandNodeInTree(string nodeName,string treeName)
        {
            string proc = $"When I Expand Node {nodeName} In Tree {treeName}";
            DebugOutput.Log(proc + "jweojoeij");
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tree.ExpandNodeInTree(treeName, nodeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



        [When(@"I Select Node ""([^""]*)"" From Tree ""([^""]*)""")]
        public void WhenISelectNodeFromTree(string nodeName, string treeName)
        {
            string proc = $"When I Select {nodeName} From Tree {treeName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tree.SelectNodeFromTree(nodeName, treeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
