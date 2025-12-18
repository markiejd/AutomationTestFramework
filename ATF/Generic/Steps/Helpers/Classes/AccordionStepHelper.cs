using Core;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper for interacting with Accordion components on the current page.
    /// Provides methods to check state and perform actions on accordions, groups and items.
    /// </summary>
    public class AccordionStepHelper : StepHelper, IAccordionStepHelper
    {
        private readonly ITargetForms targetForms;
        public AccordionStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        // Constant used when locating accordion elements via ElementInteraction helpers.
        private const string elementType = "Accordion";

        /// <summary>
        /// Returns true when the named accordion is visible on the current page.
        /// </summary>
        public bool IsDisplayed(string accordionName)
        {
            // Delegate visibility check to the ElementInteraction helper.
            return ElementInteraction.IsElementDisplayed(CurrentPage, accordionName, elementType);
        }

        /// <summary>
        /// Click the accordion header (toggles expansion for the accordion).
        /// </summary>
        public bool AccordionClick(string accordionName)
        {
            DebugOutput.Log($"AccordionClick {accordionName}");
            // Click the mat-expansion-panel-header tag inside the accordion element.
            return ElementInteraction.ClickOnTagElementUnderElement(CurrentPage, accordionName, elementType, "mat-expansion-panel-header");
        }

        /// <summary>
        /// Returns true if the accordion is currently expanded.
        /// </summary>
        public bool IsAccordionExpanded(string accordionName)
        {
            DebugOutput.Log($"IsAccordionExpanded {accordionName}");
            return ElementInteraction.IsElementExpanded(CurrentPage, accordionName, elementType);
        }

        /// <summary>
        /// Returns true if the specified group exists within the accordion and is displayed.
        /// </summary>
        public bool GroupIsDisplayed(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupIsDisplayed {accordionName} {groupName}");
            return ElementInteraction.IsElementGroupDisplayed(CurrentPage, accordionName, elementType, groupName);
        }

        /// <summary>
        /// Returns true if the specified group within the accordion is expanded.
        /// </summary>
        public bool GroupIsExpanded(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupIsExpanded {accordionName} {groupName}");
            return ElementInteraction.IsElementGroupExpanded(CurrentPage, accordionName, elementType, groupName);
        }

        /// <summary>
        /// Returns true if the specified group within the accordion is not expanded.
        /// </summary>
        public bool GroupIsNotExpanded(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupIsNotExpanded {accordionName} {groupName}");
            return ElementInteraction.IsElementGroupNotExpanded(CurrentPage, accordionName, elementType, groupName);
        }

        /// <summary>
        /// Click the header for a named group inside the accordion.
        /// </summary>
        public bool GroupClick(string accordionName, string groupName)
        {
            DebugOutput.Log($"GroupClick {accordionName} {groupName}");
            return ElementInteraction.ClickOnGroupElement(CurrentPage, accordionName, elementType, groupName);
        }

        /// <summary>
        /// Click a button tag contained within the accordion element.
        /// </summary>
        public bool ButtonClick(string accordionName, string buttonName)
        {
            DebugOutput.Log($"ButtonClick {accordionName} {buttonName}");
            // Use tag-based click for button elements under the accordion.
            return ElementInteraction.ClickOnTagElementUnderElement(CurrentPage, accordionName, elementType, "button");
        }

        /// <summary>
        /// Click on a list item (li) inside the accordion by its visible text.
        /// </summary>
        public bool AccordionItemClick(string accordionName, string item)
        {
            // Apply configured text replacements (if any) before performing the click.
            item = StringValues.TextReplacementService(item);
            DebugOutput.Log($"AccordionItemClick {accordionName} {item}");
            return ElementInteraction.ClickOnTextInTagInElement(CurrentPage, accordionName, elementType, "li", item);
        }

        /// <summary>
        /// Returns true when a button with the given visible text exists under the accordion element.
        /// </summary>
        public bool IsButtonDisplayed(string accordionName, string buttonName)
        {
            DebugOutput.Log($"IsButtonDisplayed {accordionName} {buttonName}");
            // Check for a button tag under the accordion element that matches the provided text.
            return ElementInteraction.IsElementUnderElementByTagByTextDisplayed(CurrentPage, accordionName, elementType, "button", buttonName);
        }
    }
}
