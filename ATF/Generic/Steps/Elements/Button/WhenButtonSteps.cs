using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Button
{
    [Binding]
    public class WhenButtonSteps : StepsBase
    {
        public WhenButtonSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Click Button ""([^""]*)""")]
        [When(@"I Click On Button ""([^""]*)""")]
        public void WhenIClickOnButton(string buttonName)
        {
            string proc = $"When I Click On Button {buttonName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.ClickButton(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Double Click On Button ""([^""]*)""")]
        public void WhenIDoubleClickOnButton(string buttonName)
        {
            string proc = $"When I Double Click On Button {buttonName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.DoubleClick(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Right Click On Button ""([^""]*)""")]
        public void WhenIRightClickOnButton(string buttonName)
        {
            string proc = $"When I Right Click On Button {buttonName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.RightClick(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Mouse Over Button ""([^""]*)""")]
        public void WhenIMouseOverButton(string buttonName)
        {
            string proc = $"When I Mouse Over Button {buttonName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.MouseOver(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [When(@"I Click On The ""(.*)"" Button ""(.*)""")]
        public void WhenIClickOnThenThButton(string which,string buttonName)
        {
            string proc = $"When I Click On The {which} Button {buttonName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.ClickNthButton(buttonName, which))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }

        }



        [When(@"I Drag Button ""([^""]*)"" To Button ""([^""]*)""")]
        public void WhenIDragButtonToButton(string elementA, string elementB)
        {
            string proc = $"When I Drag Button {elementA} To Button {elementB}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Button.DragAToB(elementA, elementB))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
