using Core;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class AlertStepHelper : StepHelper, IAlertStepHelper
    {
        private readonly ITargetForms targetForms;
        public AlertStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool IsDisplayed(string alertMessage)
        {
            return ElementInteraction.AlertIsDisplayed(alertMessage);
        }

        public bool Accept()
        {
            return ElementInteraction.AlertClickAccept();
        }

        public bool Cancel()
        {
            return ElementInteraction.AlertClickCancel();
        }

        public bool SendKeys(string text)
        {
            return ElementInteraction.AlertSendKeys(text);
        }


    }


}
