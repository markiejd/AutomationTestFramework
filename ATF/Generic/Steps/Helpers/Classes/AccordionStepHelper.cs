using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class AccordionStepHelper : StepHelper, IAccordionStepHelper
    {
        private readonly ITargetForms targetForms;
        public AccordionStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string elementType = "Accordion";

        public bool IsDisplayed(string accordionName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, accordionName, elementType);
        }

        public bool AccordionClick(string accordionName)
        {
            DebugOutput.Log($"IsAccordionExpanded {accordionName}");
            return ElementInteraction.ClickOnTagElementUnderElement(CurrentPage, accordionName, elementType, "mat-expansion-panel-header");
        }

        public bool IsAccordionExpanded(string accordionName)
        {
            DebugOutput.Log($"IsAccordionExpanded {accordionName}");
            return ElementInteraction.IsElementExpanded(CurrentPage, accordionName, elementType);
        }

        public bool GroupIsDisplayed(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupIsDisplayed {accordionName} {groupName}");
            return ElementInteraction.IsElementGroupDisplayed(CurrentPage, accordionName, elementType, groupName);
        }

        public bool GroupIsExpanded(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupIsExpanded {accordionName} {groupName}");
            return ElementInteraction.IsElementGroupExpanded(CurrentPage, accordionName, elementType, groupName);
        }

        public bool GroupIsNotExpanded(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupIsExpanded {accordionName} {groupName}");
            return ElementInteraction.IsElementGroupNotExpanded(CurrentPage, accordionName, elementType, groupName);
        }

        public bool GroupClick(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupClick {accordionName} {groupName}");
            return ElementInteraction.ClickOnGroupElement(CurrentPage, accordionName, elementType, groupName);
        }

        public bool ButtonClick(string accordionName, string buttonName)
        {
            DebugOutput.Log($"GroupClick {accordionName} {buttonName}");
            return ElementInteraction.ClickOnTagElementUnderElement(CurrentPage, accordionName, elementType, "button");
        }

        public bool AccordionItemClick(string accordionName, string item)
        {
            item = StringValues.TextReplacementService(item);
            DebugOutput.Log($"AccordionItemClick {accordionName} {item}");
            return ElementInteraction.ClickOnTextInTagInElement(CurrentPage, accordionName, elementType, "li", item);
        }

        public bool IsButtonDisplayed(string accordionName, string buttonName)
        {
            DebugOutput.Log($"IsButtonDisplayed {accordionName} {buttonName}");
            return false;
            // return ElementInteraction.IsElementUnderElementByTagByTextDisplayed(CurrentPage, accordionName, elementType, "button", buttonName);
        }


    }

}
