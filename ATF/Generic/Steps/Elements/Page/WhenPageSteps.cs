using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Page
{
    [Binding]
    public class WhenPageSteps : StepsBase
    {
        public WhenPageSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"I Navigate To ""(.*)""")]
        [When(@"I Navigate To ""([^""]*)""")]
        public void WhenINavigateTo(string url)
        {
            if (ElementInteraction.NavigateToURL(url))
            {
                DebugOutput.Log($"Have navigated");
                return;
            }
            Assert.Fail($"Could not navigate to {url}");
        }

        
        [When(@"I Compare Images In Page Object ""(.*)"" In Directory ""(.*)""")]
        public void WhenICompareImagesInPageObjectInDirectory(string pageName,string directory)
        {
            string proc = $"When I Compare Images In Page Object {pageName} In Directory{directory} ";
            if (CombinedSteps.OutputProc(proc))
            {
                var newPageName = directory + "" + pageName + " Page";
                newPageName = StringValues.GetTextInCase(newPageName);
                // var allElements = Helpers.Page.GetAllPageElements(pageName);
                // DebugOutput.Log($"WE have {allElements.Count} elements in page object!");

            }
        }



    }
}
