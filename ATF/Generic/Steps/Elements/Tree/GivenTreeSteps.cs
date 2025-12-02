using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Tree
{
    [Binding]
    public class GivenTreeSteps : StepsBase
    {
        public GivenTreeSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Node ""([^""]*)"" Is Selected In Tree ""([^""]*)""")]
        public void GivenNodeIsSelectedInTree(string node, string tree)
        {
            string proc = $"Given Node {node} Is Selected In Tree {tree}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tree.SelectNodeFromTree(node, tree))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
        
        [Then(@"Node ""([^""]*)"" Exists In Tree ""([^""]*)""")]
        [Given(@"Node ""([^""]*)"" Exists In Tree ""([^""]*)""")]
        public void GivenNodeExistsInTree(string node, string treeName)
        {
            string proc = $"Given Node {node} Exiss In Tree {treeName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Tree.ConfirmNode(node, treeName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
