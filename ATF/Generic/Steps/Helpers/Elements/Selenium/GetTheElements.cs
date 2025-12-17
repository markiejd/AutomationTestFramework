using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Configuration;
using Core.Logging;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper methods for locating and returning collections of IWebElement instances using Selenium.
    /// Contains convenience wrappers around SeleniumUtil and dictionary-based locators.
    /// </summary>
    public static class GetTheElements
    {
        /// <summary>
        /// Returns a list of elements from the page dictionary locator for the provided element name/type.
        /// Returns null when no locator is found or when parameters are invalid.
        /// </summary>
        /// <param name="currentPage">Current page object containing metadata for locators.</param>
        /// <param name="elementName">Logical name of the element to find.</param>
        /// <param name="elementType">Type/category of the element for dictionary lookup.</param>
        /// <param name="timeout">Optional timeout in seconds; when 0 the default PositiveTimeout is used.</param>
        public static List<IWebElement>? GetElements(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            if (currentPage == null)
            {
                DebugOutput.Log("GetElements called with null currentPage");
                return null;
            }

            if (string.IsNullOrWhiteSpace(elementName) || string.IsNullOrWhiteSpace(elementType))
            {
                DebugOutput.Log("GetElements called with empty elementName or elementType");
                return null;
            }

            timeout = ResolveTimeout(timeout);

            var locator = GetDictionaryLocator.GetElementLocator(elementName, currentPage, elementType);
            if (locator == null)
            {
                DebugOutput.Log($"NO LOCATOR FOUND IN PAGE {currentPage.Name} FOR NAME {elementName} OF TYPE {elementType}");
                return null;
            }

            return GetElements(locator, timeout);
        }
        
        /// <summary>
        /// Returns a list of elements for a given Selenium By locator.
        /// </summary>
        /// <param name="locator">Selenium By locator to use.</param>
        /// <param name="timeout">Optional timeout in seconds; when 0 the default PositiveTimeout is used.</param>
        public static List<IWebElement>? GetElements(By locator, int timeout = 0)
        {
            if (locator == null)
            {
                DebugOutput.Log("GetElements called with null locator");
                return null;
            }

            timeout = ResolveTimeout(timeout);
            return SeleniumUtil.GetElements(locator, timeout);
        }

        /// <summary>
        /// Returns the Nth element (1-based index) matching the dictionary locator on the current page.
        /// Returns null when element not found, parameters invalid, or index out of range.
        /// </summary>
        /// <param name="currentPage">Current page used for dictionary lookup.</param>
        /// <param name="elementName">Logical element name.</param>
        /// <param name="elementType">Logical element type.</param>
        /// <param name="nth">1-based index of the element to return.</param>
        /// <param name="timeout">Optional timeout in seconds; when 0 the default PositiveTimeout is used.</param>
        public static IWebElement? GetNthElement(FormBase currentPage, string elementName, string elementType, int nth, int timeout = 0)
        {
            if (nth <= 0)
            {
                DebugOutput.Log($"GetNthElement called with invalid nth value: {nth}");
                return null;
            }

            var elements = GetElements(currentPage, elementName, elementType, timeout);
            if (elements == null || elements.Count == 0)
            {
                DebugOutput.Log($"No elements found for {elementName} of type {elementType} on page {currentPage?.Name}");
                return null;
            }

            if (elements.Count < nth)
            {
                DebugOutput.Log($"Requested nth element ({nth}) is out of range. Found {elements.Count} elements.");
                return null;
            }

            // Convert 1-based to 0-based index
            return elements[nth - 1];
        }
        
        /// <summary>
        /// Returns all elements with the provided tag at the document/root level.
        /// </summary>
        /// <param name="tag">Tag name to search for (e.g. "div", "input").</param>
        /// <param name="timeout">Optional timeout in seconds; when 0 the default PositiveTimeout is used.</param>
        public static List<IWebElement>? ByTag(string tag, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                DebugOutput.Log("ByTag called with empty tag");
                return null;
            }

            timeout = ResolveTimeout(timeout);
            By tagLocator = By.TagName(tag);
            return SeleniumUtil.GetElements(tagLocator, timeout);
        }

        /// <summary>
        /// Returns all descendant elements with the provided tag under the given parent element.
        /// </summary>
        /// <param name="element">Parent IWebElement under which to search.</param>
        /// <param name="tag">Tag name to search for.</param>
        /// <param name="timeout">Optional timeout in seconds; not all SeleniumUtil overloads use this parameter.</param>
        public static List<IWebElement>? ByTag(IWebElement element, string tag, int timeout = 0)
        {
            if (element == null)
            {
                DebugOutput.Log("ByTag(parent) called with null parent element");
                return null;
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                DebugOutput.Log("ByTag(parent) called with empty tag");
                return null;
            }

            // Some SeleniumUtil.GetElementsUnder overloads may not accept a timeout; delegate to the existing utility.
            By tagLocator = By.TagName(tag);
            return SeleniumUtil.GetElementsUnder(element, tagLocator);
        }

        /// <summary>
        /// Resolve the timeout parameter, defaulting to the configured PositiveTimeout when 0 is supplied.
        /// </summary>
        private static int ResolveTimeout(int timeout) => timeout == 0 ? TargetConfiguration.Configuration.PositiveTimeout : timeout;
    }
}