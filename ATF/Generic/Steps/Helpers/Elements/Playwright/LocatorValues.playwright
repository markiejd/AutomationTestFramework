
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    //Json can only store locators as strings
    public static class LocatorValues
    {
        public static By[] locatorParser(string[] locators)
        {
            By [] result = new By[locators.Length];
            for (int i = 0; i < locators.Length; i++)
            {
                var stringLocator = locators[i];
                var locator = locatorParser(stringLocator);
                result[i] = locator;
            }
            return result;
        }

        public static By locatorParser(string locator)
        {
            By returnLocator = By.Id("unknown");
            string locatorLoweCase = locator.ToLower();
            string theThing = "";
            if (locatorLoweCase.Contains("by.tagname"))
            {
                //"By.TagName(\"li\")"
                theThing = locator;
                theThing = theThing.Replace("//By.TagName(\"", "");
                theThing = theThing.Replace(@"//By.TagName(", "");
                theThing = theThing.Replace("By.TagName(\"", "");
                theThing = theThing.Replace("\")", "");
                //DebugOutput.Log($"TAGNAME = {theThing}");
                returnLocator = By.TagName(theThing);
            }
            if (locatorLoweCase.Contains("by.xpath"))
            {
                //By.XPath(\"//button[contains(text(),'Next')]\")
                //   //span
                By.XPath("//span");
                theThing = locator;
                theThing = theThing.Replace("//By.XPath(\"", "");
                theThing = theThing.Replace(@"//By.XPath(", "");
                theThing = theThing.Replace("By.XPath(\"", "");
                theThing = theThing.Replace("\")", "");
                //DebugOutput.Log($"!!!!      {theThing}");
                //theThing = "\"" + theThing + "\"";
                returnLocator =By.XPath(theThing);
            }
            if (locatorLoweCase.Contains("by.id"))
            {
                //By.Id(\"loading\")
                theThing = locator;
                theThing = theThing.Replace("By.Id(\"", "");
                theThing = theThing.Replace(@"By.Id(", "");
                theThing = theThing.Replace("\")", "");
                returnLocator = By.Id(theThing);
            }
            if (locatorLoweCase.Contains("by.name"))
            {
                theThing = locator;
                theThing = theThing.Replace("By.Name(\"", "");
                theThing = theThing.Replace(@"By.Name(", "");
                theThing = theThing.Replace("\")", "");
                returnLocator = By.Name(theThing);
            }
            if (locatorLoweCase.Contains("by.classname"))
            {
                theThing = locator;
                theThing = theThing.Replace("By.ClassName(\"", "");
                theThing = theThing.Replace(@"By.ClassName(", "");
                theThing = theThing.Replace("\")", "");
                returnLocator = By.ClassName(theThing);
            }
            return returnLocator;
        }
    }
}
