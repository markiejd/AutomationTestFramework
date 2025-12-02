using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;


namespace Generic.Steps.Elements.Alert
{
    [Binding]
    public class WhenAlertSteps : StepsBase
    {
        public WhenAlertSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Accept Alert")]
        public void WhenIAcceptAlert()
        {
            string proc = $"When I Accept Alert";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Alert.Accept())
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Cancel Alert")]
        public void ThenICancelAlert()
        {
            string proc = $"When I Cancel Alert";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Alert.Cancel())
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Enter ""([^""]*)"" In Alert")]
        public void WhenIEnterInAlert(string text)
        {
            string proc = $"When I Enter {text} In Alert";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Alert.SendKeys(text))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
