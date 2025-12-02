using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class LinkStepHelper : StepHelper, ILinkStepHelper
    {
        private readonly ITargetForms targetForms;
        public LinkStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool ClickLink(string linkName)
        {
            DebugOutput.Log($"proc - IsDisplayed {linkName}");
            return ElementInteraction.ClickOnElement(CurrentPage, linkName, "Link");
        }

        public bool IsDisplayed(string linkName)
        {
            DebugOutput.Log($"proc - IsDisplayed {linkName}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, linkName, "Link");
        }

    }
}
