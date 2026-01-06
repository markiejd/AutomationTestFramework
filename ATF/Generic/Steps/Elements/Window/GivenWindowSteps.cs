using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Window
{
    [Binding]
    public class GivenWindowSteps : StepsBase
    {
        public GivenWindowSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Window ""([^""]*)"" Is Displayed")]
        public void GivenWindowIsDisplayed(string windowName)
        {
            string proc = $"Given Window {windowName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Window.IsDisplayed(windowName))
                {
                    DebugOutput.Log($"Is Displayed setting current page");
                    Helpers.Page.SetCurrentPage(windowName);
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
        
        [Given(@"Window Size ""(.*)""")]
        public void GivenWindowSize(string compositeSize)
        {
            string proc = $"Given Window Size {compositeSize}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Window.SizeOfWindowString(compositeSize))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
