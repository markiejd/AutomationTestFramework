using Core;
using Core.Configuration;
using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Page
{
    [Binding]
    public class GivenPageSteps : StepsBase
    {
        public GivenPageSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"URL is ""([^""]*)""")]
        public void GivenURLIs(string url)
        {
            string proc = $"Given URL Is {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (ElementInteraction.NavigateToURL(url))
                {
                    DebugOutput.Log($"Have navigated");
                    return;
                }
                Assert.Fail($"Could not navigate to {url}");
            }           
        }




        [Given(@"Page Size (.*) x (.*)")]
        public void GivenPageSizeX(int widthPixels, int heightPixels)
        {
            string proc = $"Given Page Size {widthPixels} x {heightPixels}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (ElementInteraction.SetWindowSize(widthPixels, heightPixels))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Given(@"Images From Elements On Page ""([^""]*)"" Captured")]
        [When(@"I Capture Images Of All Elements On page ""(.*)""")]
        public void GivenImagesFromElementsOnPageCaptured(string pageName)
        {
            string proc = $"Given Image From Elements On Page {pageName} Captured";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!TargetConfiguration.Configuration.HandleImages)
                {
                    Assert.Inconclusive("Images are switched OFF in the TargetConfiguration! Can not test images!");
                    return;
                }
                if (Helpers.Page.GetImagesOfAllElementsInPageFile(pageName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
        
        [Given(@"Images From Elements On Page ""(.*)"" Compared")]
        public void GivenImagesFromElementsOnPageCompared(string pageName)
        {
            string proc = $"Given Image From Elements On Page {pageName} Compared";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!TargetConfiguration.Configuration.HandleImages)
                {
                    Assert.Inconclusive("Images are switched OFF in the TargetConfiguration! Can not test images!");
                    return;
                }
                if (Helpers.Page.GetImagesOfAllElementsInPageFile(pageName, true))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
