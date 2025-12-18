using System;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    // Json can only store locators as strings
    public static class LocatorValues
    {
        /// <summary>
        /// Parse an array of locator strings into an array of Selenium By objects.
        /// Safe for null inputs (returns empty array) and preserves the original order.
        /// </summary>
        public static By[] locatorParser(string[] locators)
        {
            if (locators == null)
            {
                return Array.Empty<By>();
            }

            By[] result = new By[locators.Length];
            for (int i = 0; i < locators.Length; i++)
            {
                var stringLocator = locators[i];
                var locator = locatorParser(stringLocator);
                result[i] = locator;
            }
            return result;
        }

        /// <summary>
        /// Parse a single locator string into a Selenium By object.
        /// Expected input forms include:
        /// "By.Id(\"value\")", "By.XPath(//div)", "By.TagName(\"li\")", etc.
        /// The method is robust against extra wrappers or missing quotes and trims whitespace.
        /// </summary>
        public static By locatorParser(string locator)
        {
            // Default sentinel value to indicate an unknown locator.
            By returnLocator = By.Id("unknown");

            if (string.IsNullOrWhiteSpace(locator))
            {
                // nothing to parse
                return returnLocator;
            }

            string locatorLowerCase = locator.ToLowerInvariant();
            // Extract the inner argument once and reuse for all cases.
            string argument = ExtractArgument(locator);

            // Determine the By type by inspecting the original (lowercased) locator string.
            // Each branch documents the expected behavior.
            if (locatorLowerCase.Contains("by.tagname"))
            {
                // e.g. By.TagName("li") or By.TagName(li)
                returnLocator = By.TagName(argument);
            }
            else if (locatorLowerCase.Contains("by.xpath"))
            {
                // e.g. By.XPath("//button[text()='Next']") or By.XPath(//span)
                returnLocator = By.XPath(argument);
            }
            else if (locatorLowerCase.Contains("by.id"))
            {
                // e.g. By.Id("loading")
                returnLocator = By.Id(argument);
            }
            else if (locatorLowerCase.Contains("by.name"))
            {
                // e.g. By.Name("username")
                returnLocator = By.Name(argument);
            }
            else if (locatorLowerCase.Contains("by.classname"))
            {
                // e.g. By.ClassName("btn-primary")
                returnLocator = By.ClassName(argument);
            }

            return returnLocator;
        }

        // Helper: extract the argument part from a locator string.
        // Handles forms like:
        //   By.Type("value")
        //   By.Type(value)
        //   //div/xpath-style (no parentheses)
        // The result is trimmed and surrounding quotes (single or double) are removed.
        private static string ExtractArgument(string locator)
        {
            if (string.IsNullOrEmpty(locator))
            {
                return string.Empty;
            }

            // Try to find the first opening parenthesis and the last closing one.
            int openParen = locator.IndexOf('(');
            int closeParen = locator.LastIndexOf(')');
            string raw;

            if (openParen >= 0 && closeParen > openParen)
            {
                // Extract between parentheses
                raw = locator.Substring(openParen + 1, closeParen - openParen - 1);
            }
            else
            {
                // No parentheses - treat the whole string as the value (common with raw xpaths like "//span")
                raw = locator;
            }

            // Trim whitespace and surrounding quotes
            raw = raw.Trim();
            if ((raw.StartsWith("\"") && raw.EndsWith("\"")) || (raw.StartsWith("'") && raw.EndsWith("'")))
            {
                raw = raw.Substring(1, raw.Length - 2);
            }

            return raw.Trim();
        }
    }
}
