using Core;
using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Window
{
    [Binding]
    public class WhenWindowSteps : StepsBase
    {
        public WhenWindowSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Close Window ""([^""]*)""")]
        public void WhenICloseWindow(string windowsName)
        {
            string proc = $"When I Close Window {windowsName}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Window.CloseWindow(windowsName))
                {
                    if (Helpers.Button.IsDisplayed("Don't Save"))
                    {
                        if(!Helpers.Button.ClickButton("Don't Save"))
                        {
                            DebugOutput.Log($"Problem with popup Save Changes?");
                            return;
                        }
                    }
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Close Window")]
        public void WhenICloseWindow()
        {
            string proc = $"When I Close Window";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Window.CloseTopElement())
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
