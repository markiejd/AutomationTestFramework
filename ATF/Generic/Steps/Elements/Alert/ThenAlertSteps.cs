using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;


namespace Generic.Steps.Elements.Alert
{
    [Binding]
    public class ThenAlertSteps : StepsBase
    {
        public ThenAlertSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Alert ""([^""]*)"" Is Displayed")]
        public void ThenAlertIsDisplayed(string alertMessage)
        {
            string proc = $"Then Alert {alertMessage} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Alert.IsDisplayed(alertMessage))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }            
        }


    }
}
