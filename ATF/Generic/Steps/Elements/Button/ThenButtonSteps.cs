using System.Diagnostics;
using Core.Configuration;
using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Button
{
    [Binding]
    public class ThenButtonSteps : StepsBase
    {
        public ThenButtonSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Button ""([^""]*)"" Is Displayed")]
        public bool ThenButtonIsDisplayed(string buttonName)
        {
            string proc = $"Then Button {buttonName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Button.IsDisplayed(buttonName))
                {
                    return true;
                }
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }


        [Then(@"Wait For Button ""(.*)"" To Not Be Displayed")]
        [Then(@"Button ""([^""]*)"" Is Not Displayed")]
        public bool ThenButtonIsNotDisplayed(string buttonName)
        {
            string proc = $"Then Button {buttonName} Is Not Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                Thread.Sleep(500);
                if (!Helpers.Button.IsDisplayed(buttonName))
                { 
                    return true;
                }
                int timeout = TargetConfiguration.Configuration.PositiveTimeout;
                if (Helpers.Button.IsNotDisplayed(buttonName)) return true;
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }
        

        [Then(@"Button ""([^""]*)"" Is Enabled")]
        public void ThenButtonIsEnabled(string buttonName)
        {
            string proc = $"Then Button {buttonName} Is Enabled";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Button.IsEnabled(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
            Assert.Inconclusive();
        }

        [Then(@"Button ""([^""]*)"" Is Disabled")]
        public void ThenButtonIsDisabled(string buttonName)
        {
            string proc = $"Then Button {buttonName} Is Disabled";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Button.IsEnabled(buttonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
            Assert.Inconclusive();
        }

        [Then(@"Button ""([^""]*)"" Is Selected")]
        public void GivenButtonIsSelected(string buttonName)
        {
            string proc = $"Then Button {buttonName} Is Selected";
            if (Helpers.Button.IsSelected(buttonName))
            {
                return;
            }
            CombinedSteps.Failure(proc);
            return;
        }


    }
}
