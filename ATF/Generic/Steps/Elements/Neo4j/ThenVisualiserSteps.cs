using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.List
{
    [Binding]
    public class ThenVisualiserSteps : StepsBase
    {
        public ThenVisualiserSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [Then(@"Visualiser Is Displayed")]
        public void ThenVisualiserIsDisplayed()
        {
            string proc = $"Then Visualiser Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Visualiser.IsDisplayed())
                {
                    CombinedSteps.Failure(proc);
                    return;
                }
                return;
            }
        }

        
        [Then(@"Visualiser Contains (.*) Nodes")]
        public bool ThenVisualiserContainsNodes(int expectedNodes)
        {
            string proc = $"Then Visualiser Contains {expectedNodes} Nodes";
            if (CombinedSteps.OutputProc(proc))
            {
                var actualNodes = Helpers.Visualiser.GetHowManyNodesInVisualiser();
                if (actualNodes == expectedNodes) return true;
                DebugOutput.Log($"Wanted this many nodes {expectedNodes} but I see {actualNodes} that many nodes!");
                CombinedSteps.Failure(proc);
            }
            return false;
        }

        
        [Then(@"Visualiser Contains (.*) Links")]
        public bool ThenVisualiserContainsLinks(int expectedNumberOfLinks)
        {
            string proc = $"Then Visualiser Contains {expectedNumberOfLinks} Links";
            if (CombinedSteps.OutputProc(proc))
            {
                if (expectedNumberOfLinks == Helpers.Visualiser.GetHowManyLinksInVisualiser()) return true;
                CombinedSteps.Failure(proc);
            }
            return false;
        }





    }
}

