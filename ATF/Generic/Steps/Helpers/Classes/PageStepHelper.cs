
using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using OpenQA.Selenium;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class PageElements
    {
        // public List<string>? ElementKeys { get; set; }  
        // public List<By>? ElementLocators { get; set; }
    }

    public class PageStepHelper : StepHelper, IPageStepHelper
    {
        private readonly ITargetForms targetForms;
        public PageStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        // int versionNumber = 0;

        // private readonly string[] messageLocatorStart = { $"//*[contains(text(),'" };
        // private readonly string imageOutputDir = @"AppSpecFlow\TestOutput\PageImages";

        public string GetCurrentPageName()
        {
            DebugOutput.Log($"proc - GetCurrentPageName");
            if (CurrentPage.Name == null) return "NO PAGE REGISTERED";
            return CurrentPage.Name;
        }


        public bool ComparePageImage(string pageName)
        {
            DebugOutput.Log($"proc - ComparePageImage '{pageName}'");
            return false;
            // pageName = GetPageName(pageName);
            // DebugOutput.Log($"proc - ComparePageImage AFTER GetPageName '{pageName}' ");
            // // Get the current screenshot as bit1
            // Bitmap? bit1 = SeleniumUtil.GetCurrentPageScreenShot(pageName); 
            // if (bit1 == null) return false;

            // // Is there a old screen shot?
            // var fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + "PAGE" + pageName + ".png";
            // if (FileUtils.OSFileCheck(fullFileName))
            // {
            //     DebugOutput.Log($"The page already has an image we can compare to! {fullFileName}");
            //     // Get the old screenshot
            //     Bitmap? bit2 = ImageValues.GetBitMapFromFile(fullFileName);
            //     if (bit2 == null) return false;
            //     var compare = ImageValues.CompareBitMaps(bit1, bit2);
            //     if (compare) DebugOutput.Log($"THEY MATCH");
            //     else
            //     {
            //         DebugOutput.Log("THEY DO NOT MATCH");
            //         FileUtils.UpdateImageWarningFile($"Image for page {pageName} {fullFileName} do noot match");
            //     } 
            // }
            // else
            // {
            //     DebugOutput.Log($"The page does not have an image!  Nothing to compare so saving the temp image to the right place!");
            //     return ImageValues.SaveBitMapAs(bit1, fullFileName);
            // }            
            // return true;
        }

        public List<string> GetAllPageKeys(string pageName)
        {
            DebugOutput.OutputMethod($"GetAllPageKeys", pageName);
            var listOfKeys = new List<string>();
            return listOfKeys;
            // pageName = GetPageName(pageName);
            // var expectedPage = targetForms[pageName];
            // if (expectedPage == null) return listOfKeys;
            // DebugOutput.Log($"Have page {pageName}. Now looking for ALL the elements");
            // var pageElements = expectedPage.Elements;
            // foreach (var element in pageElements)
            // {
            //     listOfKeys.Add(element.Key);
            // }
            // return listOfKeys;
        }


        // public List<IWebElement> GetAllPageElements(string pageName, int timeOut = 0)
        // {
        //     DebugOutput.Log($"proc - GetAllPageElements '{pageName}'");
        //     pageName = GetPageName(pageName);
        //     DebugOutput.Log($"proc - GetAllPageElements AFTER GetPageName '{pageName}' ");
        //     var listOfElements = new List<IWebElement>();
        //     try
        //     {
        //         var expectedPage = targetForms[pageName];
        //         if (expectedPage == null) return listOfElements;
        //         DebugOutput.Log($"Have page {pageName}. Now looking for ALL the elements");
        //         var pageElements = expectedPage.Elements;
        //         foreach (KeyValuePair<string, By> entry in pageElements)
        //         {
        //             DebugOutput.Log($"Key {entry.Key} | Locator {entry.Value}  ");    
        //             //MUCH MORE TO DO HERE! 
        //         }
        //     }
        //     catch
        //     {

        //     }
        //     return listOfElements;
        // }

        // public By? GetCurrentPageLocators(string elementName)
        // {
        //     DebugOutput.Log($"proc - GetCurrentPageLocators {elementName}");
        //     var pageName = GetCurrentPageName();
        //     try
        //     {
        //         var expectedPage = targetForms[pageName];
        //         if (expectedPage == null) return null;
        //         DebugOutput.Log($"Have page {pageName}. Now looking for the elements {elementName}");
        //         var pageLocator = expectedPage.Elements[elementName];
        //         if (pageLocator == null) return null;
        //         DebugOutput.Log($"We have the LOCATOR {pageLocator}");
        //         return pageLocator;
        //     }
        //     catch
        //     {
        //         DebugOutput.Log($"Failed Finding the locator in page object! {elementName}");
        //         return null;
        //     }
        // }

        // public IWebElement? GetCurrentPageElement(string elementName, int timeOut = 0)
        // {
        //     DebugOutput.Log($"proc - GetCurrentPageElement {elementName}");
        //     var pageName = GetCurrentPageName();
        //     try
        //     {
        //         var expectedPage = targetForms[pageName];
        //         if (expectedPage == null) return null;
        //         DebugOutput.Log($"Have page {pageName}. Now looking for the elements {elementName}");
        //         var pageLocator = expectedPage.Elements[elementName];
        //         if (pageLocator == null) return null;
        //         DebugOutput.Log($"We have the LOCATOR {pageLocator}");
        //         var element = SeleniumUtil.GetElement(pageLocator, timeOut);
        //         return element;
        //     }
        //     catch
        //     {
        //         DebugOutput.Log($"Failed in the GetPageElement! {elementName}");
        //         return null;
        //     }
        // }

        // public List<IWebElement>? GetCurrentPageElements(string elementName, int timeout = 0)
        // {
        //     DebugOutput.Log($"proc - GetPageElements {elementName}");
        //     var pageName = GetCurrentPageName();
        //     try
        //     {
        //         var expectedPage = targetForms[pageName];
        //         if (expectedPage == null) return null;
        //         DebugOutput.Log($"Have page {pageName}. Now looking for the elements {elementName}");
        //         var pageLocator = expectedPage.Elements[elementName];
        //         if (pageLocator == null) return null;
        //         DebugOutput.Log($"We have the LOCATOR {pageLocator}");
        //         var elements = SeleniumUtil.GetElements(pageLocator, timeout);
        //         return elements;
        //     }
        //     catch
        //     {
        //         DebugOutput.Log($"Failed in the GetPageElements! {elementName}");
        //         return null;
        //     }
        // }

        // public IWebElement? GetPageElement(string pageName, string elementName, int timeOut = 0)
        // {
        //     DebugOutput.Log($"proc - GetPageElement '{pageName}' {elementName}");
        //     pageName = GetPageName(pageName);
        //     if (elementName != "ID") elementName = elementName.ToLower();
        //     DebugOutput.Log($"proc - GetPageElement AFTER GetPageName '{pageName}' {elementName}");
        //     try
        //     {
        //         var expectedPage = targetForms[pageName];
        //         if (expectedPage == null) return null;
        //         DebugOutput.Log($"Have page {pageName}. Now looking for the element {elementName}");
        //         var pageLocator = expectedPage.Elements[elementName];
        //         if (pageLocator == null) return null;
        //         DebugOutput.Log($"We have the LOCATOR {pageLocator}");
        //         var element = SeleniumUtil.GetElement(pageLocator, timeOut);
        //         if (element == null) return null;
        //         return element;
        //     }
        //     catch
        //     {
        //         DebugOutput.Log($"FAILED SOMEWHERE AROUND PAGE {pageName} looking for {elementName}");
        //         return null;
        //     }
        // }

        public bool IsExists(string pageName)
        {
            DebugOutput.Log($"proc - IsExists {pageName}");
            pageName = GetPageName(pageName);
            DebugOutput.Log($"proc - IsExists {pageName}");
            var pageElement = GetForm(pageName);
            if (pageElement == null) return false;
            return true;
        }

        public bool IsDisplayed(string pageName, int timeOut = 0)
        {
            DebugOutput.Log($"proc - IsDisplayed {pageName}");
            if (timeOut == 0)
            {
                timeOut = TargetConfiguration.Configuration.PositiveTimeout;
            }
            pageName = GetPageName(pageName);
            DebugOutput.Log($"proc - IsDisplayed {pageName}");
            
            var expectedPage = targetForms[pageName];
            if (ElementInteraction.IsElementExists(expectedPage, "ID", "textbox", TargetConfiguration.Configuration.PositiveTimeout))
            {
                DebugOutput.Log($"ID Element exists");
                CurrentPage = expectedPage;
                return ElementInteraction.WaitForElementToBeDisplayed(CurrentPage, "ID", "textbox");
            } 
            DebugOutput.Log($"Not able to find Page displayed");
            return false;
        }

        public bool IsNotDisplayed(string pageName, int timeOut = 0)
        {
            DebugOutput.Log($"proc - IsNotDisplayed {pageName} {timeOut}");
            if (timeOut == 0)
            {
                timeOut = TargetConfiguration.Configuration.NegativeTimeout;
            }
            return ElementInteraction.IsElementNotDisplayed(CurrentPage, "ID", "textbox");
        }


        public bool IsMessageDisplayed(string message)
        {
            message = StringValues.TextReplacementService(message);
            DebugOutput.Log($"proc - IsMessageDisplayed {message}");
            By locator = By.XPath($"//*[contains(text(),'{message}')]");
            var element = SeleniumUtil.GetElement(locator, 1);
            if (element == null)
            {
                DebugOutput.Log($"Message {message} not found by XPATH");
                return false;
            }
            DebugOutput.Log($"Message {message} found by XPATH");
            return SeleniumUtil.IsDisplayed(element, false, 2);
        }

        public void SetCurrentPage(string pageName)
        {
            pageName = GetPageName(pageName);
            FormBase? expectedPage;
            string pageNameNew = "";
            try
            {
                expectedPage = targetForms[pageName];
                CurrentPage = expectedPage;
            }
            catch
            {
                try
                {
                    pageNameNew = TargetConfiguration.Configuration.AreaPath + " " + pageName;
                    expectedPage = targetForms[pageNameNew];
                    CurrentPage = expectedPage;
                }
                catch
                {
                    DebugOutput.Log($"THIS HAS NOT SET THE PAGE NAME _ CHECK THERE IS A PAGE MODEL FOR {pageName} OR {pageNameNew}");
                }
            }
            DebugOutput.Log($"Current page now set to {CurrentPage}");
        }

        public bool SetCurrentPageSize(int width = 800, int height = 600)
        {
            return ElementInteraction.SetWindowSize(width, height);
        }



        //PRIVATE METHODS BELOW

        private FormBase? GetForm(string pageName)
        {
            pageName = GetPageName(pageName);
            FormBase? expectPage = null;
            DebugOutput.Log($"Proc - GetForm {pageName}");
            try
            {
                expectPage = targetForms[pageName];
            }
            catch
            {
                DebugOutput.Log($"Issue with targetForm {pageName}");
            }
            return expectPage;
        }

        public bool GetImagesOfAllElementsInPageFile(string pageName, bool update = false)
        {
            DebugOutput.Log($"Sel - GetImagesOfAllElementsInPageFile {pageName} {update}");
            return false;
            // pageName = GetPageName(pageName);
            // PageElements pageElements = GetAllPageElements(pageName);
            // if (pageElements.ElementKeys == null) return false;
            // bool allImagesTaken = true;
            // int counter = 0;
            // int x = 0;
            // int y = 0;
            // x = SeleniumUtil.GetPageWidth();
            // y = SeleniumUtil.GetPageHeight();
            // foreach (var elementName in pageElements.ElementKeys)
            // {
            //     DebugOutput.Log($"ELEMENT NAME =  {elementName}");
            //     if (pageElements.ElementLocators == null) return false;
            //     By elementLocator = pageElements.ElementLocators[counter];
            //     var element = SeleniumUtil.GetElement(elementLocator, 1);
            //     if (element == null) return false;
            //     if (element.Displayed)
            //     {
            //         if (TargetConfiguration.Configuration == null) return false;    
            //         if (TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
            //         {
            //             //SeleniumUtil.SetWindowSize(3000, 3000);
            //             var elementHeight = element.Size.Height;
            //             if (!SeleniumUtil.ScreenShotElementAlreadyExists(elementName, pageName))
            //             {
            //                 DebugOutput.Log($"New element image required! {elementName}");
            //                 if (!SeleniumUtil.MoveToElement(element)) return false;
            //                 if (!SeleniumUtil.ScreenShotElement(element, elementName, pageName))
            //                 {
            //                     DebugOutput.Log($"Failed, scroll down slightly! Try again!");
            //                     int moveY = element.Size.Height;
            //                     if (!SeleniumUtil.MoveToElement(element, 0, moveY)) return false;
            //                     if (!SeleniumUtil.ScreenShotElement(element, elementName, pageName)) return false;
            //                 }
            //             }
            //             else
            //             {
            //                 DebugOutput.Log($"Element Image on page already exists!");
            //                 if (update)
            //                 {
            //                     DebugOutput.Log($"Want to compare! And Update!"); 
            //                     var image1 = SeleniumUtil.GetElementScreenShot(element);
            //                     if (image1 == null) return false;
            //                     FileUtils.SetCurrentDirectoryToTop();
            //                     var directory = Directory.GetCurrentDirectory();
            //                     var fullFileLocation = directory + "\\" + imageOutputDir+ "\\" + TargetConfiguration.Configuration.AreaPath + "\\" + pageName + "\\" + elementName + ".png";
            //                     var image2 = ImageValues.GetBitMapFromFile(fullFileLocation);
            //                     if (image2 == null) return false;
            //                     if (image1 == image2) return true;
            //                     var match = ImageValues.CompareBitMaps(image1, image2);
            //                     if (!match) FileUtils.UpdateImageWarningFile($"Comparing the element {elementName}'s image - it does not match!  {fullFileLocation}");
            //                 }
            //             }
            //         }
            //     }
            //     counter++;
            // }
            // SeleniumUtil.SetWindowSize(x, y);

            // DebugOutput.Log($"WE have {pageElements.ElementKeys.Count} elements on Page!");
            // return allImagesTaken;
        }

        // private PageElements GetAllPageElements(string pageName)
        // {
        //     DebugOutput.Log($"Proc - GetAllPageElements {pageName}");
        //     pageName = GetPageName(pageName);
        //     int counter = 0;
        //     var expectedPage = targetForms[pageName];
        //     List<string> elementKeys = new List<string>();
        //     List<By> elementLocators = new List<By>();
        //     foreach (var item in expectedPage.Elements)
        //     {
        //         string elementKey = item.Key;
        //         elementKeys.Add(elementKey);
        //         By elementLocator = item.Value;
        //         elementLocators.Add(elementLocator);
        //         counter++;
        //     }
        //     return new PageElements { ElementKeys = elementKeys, ElementLocators = elementLocators };
        // }

        private string GetPageName(string pageName)
        {
            DebugOutput.Log($"Proc - GetCorrectPageName {pageName}");  
            var app = TargetConfiguration.Configuration.AreaPath;
            if (pageName.Contains(app)) pageName = pageName.Replace(app, "");
            if (pageName.Contains(app.ToLower())) pageName = pageName.Replace(app.ToLower(), "");
            if (pageName.Contains(app.ToUpper())) pageName = pageName.Replace(app.ToUpper(), "");
            DebugOutput.Log($"Leaves us with {pageName}");
            var newPageName = app.ToLower() + pageName;
            newPageName = newPageName.Replace(" Page","");
            newPageName = newPageName.Replace(" page","");
            DebugOutput.Log($"NEW PAGE RETURNED Is {newPageName}");
            return newPageName;
        }

        private string? GetApplication(bool lowerCase = false)
        {
            if (TargetConfiguration.Configuration.AreaPath == null) return null;
            var app = TargetConfiguration.Configuration.AreaPath;
            if (lowerCase) return app.ToLower();
            return app;
        }


        private bool IsMessageDisplayedName(string message)
        {
            DebugOutput.Log($"proc - IsMessageDisplayedName {message}");
            return false;
            // var locator = By.Name(message);
            // var messageElement = SeleniumUtil.GetElement(locator, 1);
            // if (messageElement == null) return IsMessageDisplayedPartOfName(message);
            // DebugOutput.Log($"Found by NAME!");
            // return true;
        }


        private bool IsMessageDisplayedPartOfName(string message)
        {
            DebugOutput.Log($"proc - IsMessageDisplayedPartOfName {message}");
            return false;
            // var cssLocator = By.CssSelector("[Name*='" + message + "']");
            // var messageElement = SeleniumUtil.GetElement(cssLocator, 1);
            // if (messageElement == null) return IsMessageDisplayedPartOfAnyName(message);
            // DebugOutput.Log($"Found by PART of NAME!");
            // return true;
        }


        private bool IsMessageDisplayedPartOfAnyName(string message)
        {
            return false;
            // return SeleniumUtil.ListOfElementNamesContains(message);
        }

    }
}
