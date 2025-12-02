
using Core;
using Core.Configuration;
using Core.Logging;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    public static class GetTheElements
    {

        public static List<IWebElement>? GetElements(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            if (timeout == 0) timeout = TargetConfiguration.Configuration.PositiveTimeout;
            var locator = GetDictionaryLocator.GetElementLocator(elementName, currentPage, elementType);
            if (locator == null)
            {
                DebugOutput.Log($"NO LOCATOR FOUND IN PAGE {currentPage.Name} FOR NAME {elementName} OF TYPE {elementType} - unless the element is in the ");
                return null;
            }
            return GetElements(locator, timeout);
        }
        
        public static List<IWebElement>? GetElements(By locator, int timeout = 0)
        {
            return SeleniumUtil.GetElements(locator, timeout);
        }

        public static IWebElement? GetNthElement(FormBase currentPage, string elementName, string elementType, int nth, int timeout)
        {
            var elements = GetElements(currentPage, elementName, elementType, timeout);
            if (elements == null) return null;
            if (elements.Count < nth) return null;
            nth--;
            return elements[nth];
        }
        
        public static List<IWebElement>? ByTag(string tag, int timeout = 0)
        {
            By tagLocator = By.TagName(tag);
            return SeleniumUtil.GetElements(tagLocator, timeout);
        }

        public static List<IWebElement>? ByTag(IWebElement element, string tag, int timeout = 0)
        {
            By tagLocator = By.TagName(tag);
            return SeleniumUtil.GetElementsUnder(element, tagLocator);
        }
    }
}