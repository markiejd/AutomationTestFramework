
using Core;
using Core.Logging;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    public static class GetDictionaryLocator
    {
        public static By? ByValue(FormBase CurrentPage, string value)
        {
            DebugOutput.Log($"Does the Current Page {CurrentPage.Name} contain the element {value}");
            if (CurrentPage.Elements.ContainsKey(value))
            {
                DebugOutput.Log($"IT DOES! Found it!");
                return CurrentPage.Elements[value];
            }
            DebugOutput.Log($"Nope - not in this page at least!");
            return null;
        }

        
        public static By? GetElementLocator(string name, FormBase CurrentPage, string type = "")
        {
            if (name != "ID") name = name.ToLower();
            DebugOutput.Log($"GetCorrectElementName {name} CURRENTPAGE {CurrentPage} and TYPE = {type}");
            if (CurrentPage.Elements.ContainsKey(name))
            {
                DebugOutput.Log($"Found the element name {name} on current page form!");
                return CurrentPage.Elements[name];
            }

            var nameAllLowerCase = name.ToLower();
            if (CurrentPage.Elements.ContainsKey(nameAllLowerCase))
            {
                DebugOutput.Log($"Found the element name {nameAllLowerCase} on current page form!");
                return CurrentPage.Elements[nameAllLowerCase];
            }

            var nameAllLowerCaseWithType = nameAllLowerCase + " " + type.ToLower();
            if (CurrentPage.Elements.ContainsKey(nameAllLowerCaseWithType))
            {
                DebugOutput.Log($"Found the element name {nameAllLowerCaseWithType} on current page form!");
                return CurrentPage.Elements[nameAllLowerCaseWithType];
            }

            var noSpaceName = nameAllLowerCase.Replace(" ", "");
            noSpaceName = noSpaceName.Replace("-", "");
            if (CurrentPage.Elements.ContainsKey(noSpaceName))
            {
                DebugOutput.Log($"Found the element name NO SPACE {noSpaceName} on current page form!");
                return CurrentPage.Elements[noSpaceName];
            }

            string noSpaceNameButAddedType = noSpaceName + " " + type.ToLower();
            if (CurrentPage.Elements.ContainsKey(noSpaceNameButAddedType))
            {
                DebugOutput.Log($"Found the element name NO SPACE BUT with added type {noSpaceNameButAddedType} on current page form!");
                return CurrentPage.Elements[noSpaceNameButAddedType];
            }

            string NoSpaceNameButAddedTypeWithNoSpace = noSpaceNameButAddedType.Replace(" ", "");
            if (CurrentPage.Elements.ContainsKey(NoSpaceNameButAddedTypeWithNoSpace))
            {
                DebugOutput.Log($"Found the element name NO SPACE BUT with added type {NoSpaceNameButAddedTypeWithNoSpace} on current page form!");
                return CurrentPage.Elements[NoSpaceNameButAddedTypeWithNoSpace];
            }

            DebugOutput.Log($"WE gotten to here - I have no traceablity of {name} on page {CurrentPage.Name} of type {type}");
            return null;
        }
        
    }
}