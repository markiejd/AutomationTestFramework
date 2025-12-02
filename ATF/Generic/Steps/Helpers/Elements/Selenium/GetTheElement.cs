
using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    public static class GetTheElement
    {
        
        /// <summary>
        /// Get Element - using Self Heal if POM Locator Fails
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="elementName"></param>
        /// <param name="elementType"></param>
        /// <returns>element if found, null if not, even after self heal</returns>
        public static IWebElement? GetElement(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            DebugOutput.Log($"GetElement {currentPage.Name} {elementName} {elementType} {timeout}");
            IWebElement? element = null;
            var locator = GetDictionaryLocator.GetElementLocator(elementName, currentPage, elementType);
            if (locator == null)
            {
                switch(elementType.ToLower())
                {
                    default:
                    {
                        DebugOutput.Log($"We dont' have anything specific for {elementType}");
                        break;
                    }
                    case "datepicker":
                    {
                        DebugOutput.Log($"We are looking for a datepicker - so we will try Tag name mat-datepicker-content");
                        locator = By.TagName("mat-datepicker-content");
                        element =  SeleniumUtil.GetElement(locator, timeout);
                        break;                        
                    }
                    case "link":
                    {
                        var stringXPath = $"//*[text()='{elementName}']";
                        locator = By.XPath(stringXPath);
                        element =  SeleniumUtil.GetElement(locator, timeout);
                        break;
                    }
                    case "spinner":
                    {
                        element = GetElementMultipleLocators(GetSubLocators.SpinnerLocators);
                        if (element != null) locator = By.LinkText("SpinnerLocators");
                        break;
                    }
                    case "stepper":
                    {
                        element = GetElementMultipleLocators(GetSubLocators.StepperStepLocator);
                        if (element != null) locator = By.LinkText("StepperStepLocator");
                        break;
                    }
                    case "tab":
                    {
                        element = GetElementMultipleLocators(GetSubLocators.TabLocator);
                        if (element != null) locator = By.LinkText("TabLocator");
                        break;
                    }
                    case "window":
                    {
                        var stringXPath = $"//Window[contains(@Name,'{elementName}')]";
                        locator = By.XPath("//Window/");
                        element =  SeleniumUtil.GetElement(locator, timeout);
                        break;
                    }
                }
            }
            if (locator == null)
            {
                DebugOutput.Log($"NO LOCATOR FOUND IN PAGE {currentPage.Name} FOR NAME {elementName} OF TYPE {elementType} - unless the element is in the ");
                return null;
            }
            if (element == null)
            {
                DebugOutput.Log($"START GET ELEMENT {DateTime.UtcNow} {locator}");
                element =  SeleniumUtil.GetElement(locator, timeout);
                DebugOutput.Log($"END GET ELEMENT {DateTime.UtcNow}");
            }
            if (element != null) return element;

            DebugOutput.Log($"Element {elementName} of type {elementType} was not found using POM Locators {locator}");
            if (!TargetConfiguration.Configuration.SelfHeal) return null;
            DebugOutput.Log($"We have tried the lot - out of ideas! SELF HEAL ATTEMPT!");
            var selfHealModel = SelfHeal.GetSelfHealModel(currentPage.Name, elementName, elementType);
            if (selfHealModel == null)
            {
                DebugOutput.Log($"Have no record of this element being displayed, so no details stored!");
                return null;
            }
            DebugOutput.Log($"We do have a record of this element, so a chance for self healing!");
            DebugOutput.Log($"We going to try TAG and TEXT");
            element = GetElementUsingTagAndText(selfHealModel.ElementTag, selfHealModel.ElementText);
            if (element != null) return element;
            element = GetElementUsingXPath(selfHealModel.ElementXPathString);
            if (element != null) return element;
            element = GetElementUsingText(selfHealModel.ElementText);
            if (element != null) return element;
            DebugOutput.Log($"To get to here, we have tried, and failed SELF HEALING!");
            return null;
        }

        private static IWebElement? GetElementMultipleLocators(By[] locators, int timeout = 0)
        {
            foreach (var locator in locators)
            {
                var element = SeleniumUtil.GetElement(locator, timeout);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
        }

        public static IWebElement? ByXPathSring(string xPath, int timeout)
        {
            By xPathLocator = By.XPath(xPath);
            return SeleniumUtil.GetElement(xPathLocator, timeout);
        }

        public static IWebElement? GetExpansion(IWebElement element)
        {
            By expansionElementLocator = By.ClassName("accordion-toggle");
            var expansionElement = SeleniumUtil.GetElementUnderElement(element, expansionElementLocator, 1);
            return expansionElement;
        }

        public static IWebElement? GetGroup(IWebElement element, string groupName)
        {
            var groupXPath = GetSubLocators.AccordionSubElementXPathStart[GetSubLocators.AccordionVersionNumber] + groupName + GetSubLocators.AccordionSubElementXPathEnd[GetSubLocators.AccordionVersionNumber];
            var groupLocator = By.XPath(groupXPath);
            return SeleniumUtil.GetElementUnderElement(element, groupLocator);
        }

        public static IWebElement? GetGroupExpanded(IWebElement groupElement)
        {            
            var groupExpandedXPath = GetSubLocators.AccordionSubElementExpanded[GetSubLocators.AccordionVersionNumber];
            By groupExpandedLocator = By.ClassName(groupExpandedXPath);
            if (groupExpandedXPath.Contains($"/")) groupExpandedLocator = By.XPath(groupExpandedXPath);
            return SeleniumUtil.GetElementUnderElement(groupElement, groupExpandedLocator, 1);
        }

        public static IWebElement? GetParent(IWebElement element)
        {
            return SeleniumUtil.GetElementParent(element);
        }

        public static IWebElement? GetSubElementByText(FormBase currentPage, string elementName, string elementType, By locator, string text, int timeout = 0)
        {
            var element = GetElement(currentPage, elementName, elementType);
            if (element == null) return null;
            DebugOutput.Log($"Gotten parent, now looking for sub!");
            var subElements = SeleniumUtil.GetElementsUnder(element, locator, timeout);
            if (subElements == null) return null;
            foreach (var subElement in subElements)
            {
                if (SeleniumUtil.GetElementText(subElement) == text) return subElement;
            }
            return null;
        }

        private static IWebElement? GetElementUsingTagAndText(string? elementTag, string? elementText)
        {
            if (elementTag == null) return null;
            By tagLocator = By.TagName(elementTag);
            var elements = SeleniumUtil.GetElements(tagLocator, 1);
            if (elements == null) return null;
            if (elements.Count == 1) return elements[0];
            if (elementText == null) return null;
            foreach (var e in elements)
            {
                var text = SeleniumUtil.GetElementText(e);
                if (text == elementText) return e;
            }
            return null;
        }

        private static IWebElement? GetElementUsingText(string? text)
        {
            if (text == null) return null;
            var xPathString = $"//*[text() = '{text}']";
            By xPathLocator = By.XPath(text);
            return SeleniumUtil.GetElement(xPathLocator, 1);
        }

        private static IWebElement? GetElementUsingXPath(string? xPath)
        {
            if (xPath == null) return null;
            By xPathLocator = By.XPath(xPath);
            return SeleniumUtil.GetElement(xPathLocator, 1);
        }

    }
}