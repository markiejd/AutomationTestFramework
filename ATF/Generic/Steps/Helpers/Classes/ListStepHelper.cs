using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class ListStepHelper : StepHelper, IListStepHelper
    {
        private readonly ITargetForms targetForms;
        public ListStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool IsDisplayed(string list)
        {
            DebugOutput.Log($"proc - IsDisplayed {list}");
            return ElementInteraction.IsElementDisplayed(CurrentPage, list, "list");
        }



        public bool ListContainsValue(string list, string value)
        {
            DebugOutput.Log($"proc - ListContainsValue {list} {value}");
            var listOfOptions = ElementInteraction.GetSubElementsTextOfElement(CurrentPage, list, "list");
            if (listOfOptions == null) return false;
            foreach (var option in listOfOptions)
            {
                if (option == value) return true;
            }
            return false;
        }

    }
}
