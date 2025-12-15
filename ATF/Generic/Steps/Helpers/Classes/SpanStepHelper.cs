using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class SpanStepHelper : StepHelper, ISpanStepHelper
    {
        private readonly ITargetForms targetForms;
        public SpanStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }


        public bool LinkDisplayedByName(string linkName)
        {
            DebugOutput.Log($"proc - LinkDisplayedByName {linkName}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, linkName, "link");
        }

        public bool ClickOnLinkByName(string linkName)
        {
            DebugOutput.Log($"proc - ClickOnLinkByName {linkName}");
            return ElementInteraction.ClickOnElement(CurrentPage, linkName, "link");
        }

    }
}
