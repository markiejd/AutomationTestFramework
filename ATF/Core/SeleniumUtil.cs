
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Drawing;
using Core.FileIO;
using Core.Images;
using System;
using System.IO;
using System.Web;
using System.Diagnostics;

namespace Core
{
    public static class SeleniumUtil
    {
        public static IWebDriver? webDriver = null;
        // public static WindowsDriver<WindowsElement>? winDriver = null;
        public static string outputFolder = @"\AppSpecFlow\TestOutput\";
        public static string compareFolder = @"\AppSpecFlow\TestCompare\";
        public static string failedFindElement = "Failed to find element!";
        public static int test = TargetConfiguration.Configuration.Debug;
        public static int DefaultPositiveTimeOut = TargetConfiguration.Configuration.PositiveTimeout;
        public static string currentDirectory = Directory.GetCurrentDirectory();

        /// <summary>
        /// Get the Alert Element displayed on a web site
        /// </summary>
        /// <returns>IAlert if found, null if not found</returns>
        private static IAlert? GetAlert()
        {
            DebugOutput.OutputMethod($"Selenium - GetAlert");
            // DebugOutput.Log($"Sel - GetAlert");
            IAlert? alert = null;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return alert;
            try
            {
                if (webDriver == null)
                {
                    return null;
                }
                Thread.Sleep(500);
                alert = webDriver.SwitchTo().Alert();
                return alert;
            }
            catch
            {
                DebugOutput.Log($"Failed to Navigate To Alert");
            }
            return alert;
        }

        public static By GetParentXPathLocator()
        {
            DebugOutput.OutputMethod($"Selenium - GetParentXPathLocator");
            return By.XPath($"./..");
        }

        public static bool AlertLogin(string username, string password)
        {
            DebugOutput.OutputMethod($"Selenium - AlertLogin", $"{username} {password}");
            // DebugOutput.Log($"Sel - AlertLogin {username} {password}");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            IAlert? alert_win = GetAlert();
            if (alert_win == null)
            {
                DebugOutput.Log($"FAILED TO FIND ALERT WINDOW!");
                try
                {
                    if (webDriver == null) return success;
                    var x = webDriver.WindowHandles;
                    DebugOutput.Log($"We have {x.Count} windows currently available to us!");
                    var elements = webDriver.FindElements(By.XPath($"//*"));
                    DebugOutput.Log($"AND we have {elements.Count} ELEMENTS TO PLAY WITH");
                    if (!EnterText(elements[0], "JUST WORK!")) DebugOutput.Log($"NOPE NOTHING I CAN DO HERE!");
                }
                catch
                {
                    DebugOutput.Log("Failed to Find Alerts!");
                }
                return success;
            }
            try
            {
                alert_win.SendKeys(username + Keys.Tab + password);
                alert_win.Accept();
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failure entering username and password {username}");
                return success;
            }
        }


        /// <summary>
        /// Get Alert and Send Keys
        /// </summary>
        /// <param name="text"></param>
        /// <returns>true if successful</returns>
        public static bool AlertInput(string text)
        {
            DebugOutput.OutputMethod($"Selenium - AlterInput", $"{text}");
            // DebugOutput.Log($"Sel - AlterInput {text}");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            IAlert? alert_win = GetAlert();
            if (alert_win == null) return success;
            try
            {
                alert_win.SendKeys(text);
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed to send {text} to Alert");
            }
            return success;
        }

        /// <summary>
        /// Get an Alert and confirm the message 
        /// </summary>
        /// <param name="alertMessage"></param>
        /// <returns>true if alert message is displayed</returns>
        public static bool AlertDisplayed(string alertMessage)
        {
            DebugOutput.OutputMethod($"Selenium - AlertDisplayed", $"{alertMessage}");
            // DebugOutput.Log($"Sel - AlertDisplayed {alertMessage}");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            IAlert? alert_win = GetAlert();
            if (alert_win == null) return success;
            if (alertMessage.ToLower() == alert_win.Text.ToLower())
            {
                success = true;
                return success;
            }
            DebugOutput.Log($"We have an alert, but Not the message, we got '{alert_win.Text.ToLower()}' was expecting '{alertMessage.ToLower()}'");
            if (alert_win.Text.ToLower().Contains(alertMessage.ToLower()))
            {
                DebugOutput.Log($"message is found in message!");
                success = true;
                return success;
            }
            return success;
        }

        /// <summary>
        /// Get Alert, then dismiss it (cancel)
        /// </summary>
        /// <returns>true if alert message dismissed</returns>
        public static bool AlertClickCancel()
        {
            DebugOutput.OutputMethod($"Selenium - AlertClickCancel");
            // DebugOutput.Log($"Sel - AlertClickAccept");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            IAlert? alert_win = GetAlert();
            if (alert_win == null) return success;
            try
            {
                alert_win.Dismiss();
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed on the accept of alert");
            }
            return success;
        }

        /// <summary>
        /// Get Alert Message and Accept
        /// </summary>
        /// <returns>true if alert message accepted</returns>
        public static bool AlertClickAccept()
        {
            DebugOutput.OutputMethod($"Selenium - AlertClickAccept");
            // DebugOutput.Log($"Sel - AlertClickAccept");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            IAlert? alert_win = GetAlert();
            if (alert_win == null) return success;
            try
            {
                alert_win.Accept();
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed on the accept of alert");
            }
            return success;
        }


        public static bool ClickBackButtonInBrowser()
        {
            DebugOutput.OutputMethod($"Selenium - ClickBackButtonInBrowser ");
            var driver = webDriver;
            if (driver == null) return false;
            try
            {
                driver.Navigate().Back();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to click on browser back button!");
                return false;
            }
        }

        /// <summary>
        /// Pass in Element, move to it then click
        /// We use movetoelement rather than just element.click as there maybe an element on top which blocks a human from clicking on it
        /// It moves to the centre point of the element
        /// </summary>
        /// <param name="element"></param>
        /// <returns>true if successfully clicked</returns>
        public static bool Click(IWebElement element)
        {
            DebugOutput.OutputMethod("Selenium - Click", $"{element}");
            // DebugOutput.Log($"Sel - Click {element}");
            bool success = false;
            var action = GetActions();
            if (action == null) return success;
            try
            {
                action.MoveToElement(element);
                action.Build();
                action.Perform();
                action.Click();
                action.Build();
                action.Perform();
                success = true;
                return success;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {element} {ex}");
                return success;
            }
        }


        public static bool ClickCoordinates(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Selenium - ClickCoordinates", $"{x} {y}");
            // DebugOutput.Log($"Sel - ClickCoordinates {x} {y}");
            var action = GetActions();
            if (action == null) return false;
            if (webDriver == null) return false;
            try
            {
                action.MoveToElement(webDriver.FindElement(By.TagName("body")), x, y); 
                action.Click();
                action.Build().Perform();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to click or failed to find body!");
                return false;
            }
        }

        /// <summary>
        /// Pass in an element, move to its centre, then moves x and y from that centre point, then clicks
        /// </summary>
        /// <param name="element"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if successfully clicked</returns>
        public static bool ClickCoordinatesWithElement(IWebElement element, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Selenium - ClickCoordinatesWithElement", $"{element} {x} {y}");
            bool success = false;
            var action = GetActions();
            if (action == null) return success;
            try
            {
                action.MoveToElement(element);
                action.MoveByOffset(x, y);
                action.Click();
                action.Build();
                action.Perform();
                success = true;
                return success;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {element} {ex}");
                return success;
            }
        }

        /// <summary>
        /// Pass in element, move to its centre then double click
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns true if successful</returns>
        public static bool DoubleClick(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - DoubleClick", $"{element}");
            bool success = false;
            var action = GetActions();
            if (action == null) return success;
            try
            {
                action.MoveToElement(element);
                action.DoubleClick();
                action.Build();
                action.Perform();
                success = true;
                return success;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {element} {ex}");
                return success;
            }
        }

        /// <summary>
        /// Pass in element, move to its centre then right clicks
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns true if successful</returns>
        public static bool RightClick(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - RightClick", $"{element}");
            bool success = false;
            var action = GetActions();
            if (action == null) return success;
            try
            {
                action.MoveToElement(element);
                action.ContextClick();
                action.Build();
                action.Perform();
                success = true;
                return success;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {element} {ex}");
                return success;
            }
        }

        public static IWebElement? GetNthElementBy(By locator, string nTh)
        {
            DebugOutput.OutputMethod($"Selenium - ClickNthElementBy", $"{locator} {nTh}");
            var elements = GetElements(locator);
            if (elements == null) return null;
            {
                if (elements.Count == 0) return null;
            }
            DebugOutput.Log($"We have {elements.Count} elements of locator {locator}");
            switch (nTh.ToLower())
            {
                default:
                case "1st":
                {
                    try
                    {
                        return elements[0];
                    }
                    catch
                    {
                        DebugOutput.Log($"Failed to return the {nTh} element - it may not exist with {locator}");
                        return null;
                    }
                }
                case "2nd":
                {
                    try
                    {
                        return elements[1];
                    }
                    catch
                    {
                        DebugOutput.Log($"Failed to return the {nTh} element - it may not exist with {locator}");
                        return null;
                    }
                }
                case "3rd":
                {
                    try
                    {
                        return elements[2];
                    }
                    catch
                    {
                        DebugOutput.Log($"Failed to return the {nTh} element - it may not exist with {locator}");
                        return null;
                    }
                }
            }

        }

        public static bool ClearThenEnterText(IWebElement element, string text, string key = "")
        {
            DebugOutput.OutputMethod($"Selenium - ClearThenEnterText", $"{element} {text} {key}");
            Click(element);
            SendKey(element, "clear");
            return EnterText(element, text, key);
        }

        /// <summary>
        /// Pass in an element, send the keys in text, and then press keyboard key
        /// Expected keys are 'enter', 'tab'
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns>returns true if successful</returns>
        public static bool EnterText(IWebElement element, string text, string key = "")
        {
            DebugOutput.OutputMethod($"Selenium - EnterText", $"{element} {text} {key}");
            text = StringValues.TextReplacementService(text);
            DebugOutput.Log($"NEW TEXT = '{text}'");
            if (text != "")
            {
                try
                {
                    element.SendKeys(text);
                    if (string.IsNullOrEmpty(key)) return true;
                }
                catch
                {
                    DebugOutput.Log($"Failed to Enter Text Directly {element} {text} going to try finding an input box inside the element");
                    // find an input box inside the element
                    var inputs = element.FindElements(By.TagName("input"));
                    if (inputs.Count > 0)
                    {
                        DebugOutput.Log($"We found {inputs.Count} input boxes inside the element, going to try the first one");
                        try
                        {
                            inputs[0].SendKeys(text);
                            element = inputs[0];
                        }
                        catch
                        {
                            DebugOutput.Log($"Failed to Enter Text into input box {element} {text}");
                            return false;
                        }
                    }
                    else
                    {
                        DebugOutput.Log($"Failed to find an input box inside the element {element} {text}");
                        return false;
                    }
                }
            }
            if (key == "") return true;
            DebugOutput.Log($"Now to send the key {key}");
            if (SendKey(element, key)) return true;

            DebugOutput.Log($"Failed to send key {key}");
            var inputs2 = element.FindElements(By.TagName("input"));
            if (inputs2.Count > 0)
            {
                DebugOutput.Log($"We found {inputs2.Count} input boxes inside the element, going to try the first one");
                try
                {
                    if (SendKey(inputs2[0], key)) return true;
                }
                catch
                {
                    DebugOutput.Log($"Failed to Enter Text into input box {element} {text}");
                    return false;
                }
            }
            else
            {
                DebugOutput.Log($"Failed to find an input box inside the element {element} {text}");
                return false;
            }
            DebugOutput.Log($"Failed to send key {key} AGAIN");
            return false;
        }

        /// <summary>
        /// Pass in 2 elements, click and hold centre point of the first element, them move to centre of the second element and release
        /// </summary>
        /// <param name="elementA"></param>
        /// <param name="elementB"></param>
        /// <returns>returns true if successful</returns>
        public static bool DragElementToElement(IWebElement elementA, IWebElement elementB)
        {
            DebugOutput.OutputMethod($"Selenium - DragElementToElement", $"{elementA} {elementB} ");
            bool success = false;
            var action = GetActions();
            if (action == null) return success;
            try
            {
                DebugOutput.Log($"Starting action");
                var dragAndDrop = action.ClickAndHold(elementA).MoveToElement(elementB, 10, 10).Release(elementB).Build();
                dragAndDrop.Perform();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to drag!");
                return false;
            }
        }
        
        /// <summary>
        /// An element can be loaded but not displayed - Selenium gets the element and reports instantly on is visabilit!
        /// In the real world, we need to wait for this to display!
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns true if displayed, false if timed out postive timeout</returns>
        public static bool IsDisplayed(IWebElement element, bool not = false, int timeOut = 0)
        {
            DebugOutput.OutputMethod($"Selenium - IsDisplayed", $"{element} {not} {timeOut} ");

            // if the element is null we have to return a false
            if (element == null) return false;

            // if not is equal to true we are waiting for it to NOT be displayed
            if (not)
            {
                if (timeOut == 0) timeOut = TargetConfiguration.Configuration.NegativeTimeout;
                try
                {
                    DebugOutput.Log($"Testing that something is NOT displayed");
                    if (!element.Displayed)
                    {
                        DebugOutput.Log($"Element is NOT displayed already!");
                        return true;
                    }
                    DebugOutput.Log($"Element IS displayed, we need to wait for it to go NOT displayed");
                    DateTime now = DateTime.Now;
                    DateTime endTime = now.AddSeconds(timeOut);
                    int counter = 0;
                    while (DateTime.Now < endTime)
                    {
                        if (!element.Displayed)
                        {
                            DebugOutput.Log($"Element is now NOT displayed! after {counter} seconds!");
                            return true;
                        }
                        Thread.Sleep(1000);
                        DebugOutput.Log($"Still displayed after {counter} seconds!");
                        counter++;
                    }
                    DebugOutput.Log($"TIMED OUT waiting for element to be NOT displayed after {timeOut} seconds!");
                    return true;
                }
                catch
                {
                    DebugOutput.Log($"The reaons for try catch, is the element maybe GONE! what is called a stale element, it existed once, but no more! Querying a stale element restults in an exception - NEVER want those!");
                    DebugOutput.Log($"If its stale, it cant be displayed!");
                    return true;
                }
            }
            // If Not = false we are waiting for it to be displayed
            if (timeOut != 0) timeOut = TargetConfiguration.Configuration.PositiveTimeout;
            if (!element.Displayed)
            {
                DebugOutput.Log($"DISPLAYED WE DO NOT HAVE the element yet! waiting up to {timeOut} seconds for it to appear!");
                DateTime now = DateTime.Now;
                DateTime endTime = now.AddSeconds(timeOut);
                int counter = 0;
                while (DateTime.Now < endTime)
                {
                    if (element.Displayed)
                    {
                        DebugOutput.Log($"Element is now displayed! after {counter} seconds!");
                        return true;
                    }
                    Thread.Sleep(1000);
                    DebugOutput.Log($"Still not displayed after {counter} seconds!");
                    counter++;
                }
                if (element.Displayed)
                {
                    DebugOutput.Log($"Element is now displayed! after {counter} seconds!");
                    return true;
                }
                DebugOutput.Log($"TIMED OUT waiting for element to be displayed after {timeOut} seconds!");
                return false;
            }
            DebugOutput.Log($"Element is displayed! Instantly");
            return true;
        }

        /// <summary>
        /// Pass in an element, and click on centre point of element, and hold
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns true if successful</returns>
        public static bool MouseDown(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - MouseDown", $"{element} ");
            bool success = false;
            var action = GetActions();
            if (action == null) return success;
            try
            {
                action.ClickAndHold(element);
                action.Perform();
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed to click and hold / mouse down!");
                return success;
            }
        }

        /// <summary>
        /// Pass in element and move mouse over elements centre point
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns ture if successful</returns>
        public static bool MoveToElement(IWebElement element, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Selenium - MoveToElement", $"{element} {x} {y} ");
            bool success = false;
            if (element == null)
            {
                DebugOutput.Log($"No element!");
                return success;
            }    
            var action = GetActions();
            if (action == null) return success;
            try
            {
                action.MoveToElement(element);
                action.Perform();
                DebugOutput.Log($"First move!");
                if (y != 0)
                {
                    y = y + 2;
                    if (!ScrollDownPage(element, y)) return success;
                }    
                DebugOutput.Log($"MOVED!");
                success = true;
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to move!");
                return success;
            }
        }

        private static bool ScrollDownPage(IWebElement elem, int y = 0)
        {
            DebugOutput.OutputMethod($"Selenium - ScrollDownPage", $"{elem} {y} ");
            try
            {
                if (webDriver == null) return false;
                IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
                string script = $"window.scrollBy(0,{y})";
                js.ExecuteScript(script);
                //js.ExecuteScript("arguments[0].scrollIntoView(false)", elem);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Not here!");
                return false;
            }
        }

        public static bool ScrollToElement(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - ScrollToElement", $"{element} ");
            bool success = false;
            if (element == null) return success;
            try
            {
                if (webDriver == null) return success;
                IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
                js.ExecuteScript("arguments[0].scrollIntoView(false)", element);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Not here!");
                return success;
            }
        }

        /// <summary>
        /// Pass in an element, moves the slider to 0, then moves it to offset% of slider
        /// </summary>
        /// <param name="slider"></param>
        /// <param name="offSet"></param>
        /// <returns>returns true if movement of the slider is successful</returns>
        public static bool MoveSliderElement(IWebElement slider, int offSet)
        {
            DebugOutput.OutputMethod($"Selenium - MoveSliderElement", $"{slider} {offSet} ");
            bool success = false;
            if (offSet > 100)
            {
                DebugOutput.Log($"It only handles 100%");
                return success;
            }
            var width = slider.Size.Width;
            DebugOutput.Log($"Width of Element {width}");
            //The default will be 50% as moveToElement will take it to the middle of the element
            float onePercent = (float)width / 100;
            DebugOutput.Log($"One Percent =  {onePercent}");
            float zeroPoint = width - (onePercent * 50);
            var intZeroPoint = (int)Math.Round(zeroPoint);
            DebugOutput.Log($"intzeropoint = {intZeroPoint}");
            var action = GetActions();
            if (action == null) return success;
            try
            {
                DebugOutput.Log($"0 Click");
                action.MoveToElement(slider, intZeroPoint, 0);
                action.Click();
                action.Build().Perform();
                Thread.Sleep(100);
            }
            catch
            {
                DebugOutput.Log($"Failed to zero slider");
                return success;
            }
            //onePercent is the width split into 100 parts (50 to the left, 50 to the right)
            float floatMovement = onePercent * offSet;
            DebugOutput.Log($"Will be clicking on of Element FLOAT {floatMovement}");
            int movement = (int)Math.Round(floatMovement);
            try
            {
                action.MoveToElement(slider, movement, 0);
                action.Click();
                action.Build().Perform();
                Thread.Sleep(100);
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed to slide!");
            }
            return false;
        }

        /// <summary>
        /// Pass in a url to the webdriver and automaticlly navigate to url
        /// beaware some urls will forward you on!
        /// </summary>
        /// <param name="url"></param>
        /// <returns>returns true if able to send url to browser, returns false if unable to pass url to browser, or if not using a browser</returns>
        public static bool NavigateToURL(string url)
        {
            DebugOutput.OutputMethod($"Selenium - NavigateToURL", $"{url} ");
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType == "web")
            {
                try
                {
                    if (webDriver == null)
                    {
                        DebugOutput.Log($"We do not have a web driver!");
                        return false;
                    }
                    webDriver.Navigate().GoToUrl(url);
                    return true;
                }
                catch
                {
                    DebugOutput.Log($"Failed to Navigate GoToUrl");
                    return false;
                }
            }
            if (appType == "windows")
            {
                DebugOutput.Log($"It is a windows app?  If you need to navigate to a URL you will need to manually enter this in the app!");
                return false;
            }
            return false;
        }

        /// <summary>
        /// Navigate a URL that requires a username and password via a popup
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="url"></param>
        /// <returns>returns true if successful</returns>
        public static bool NavigateToURLAsUserWithPassword(string userName, string password, string url)
        {
            DebugOutput.OutputMethod($"Selenium - NavigateToURLAsUserWithPassword", $"{userName} {password} {url} ");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            url = url.ToLower();
            var theWebAddress = url.Replace("http://", "");
            theWebAddress = url.Replace("https://", "");
            var combinedurl = "http://" + userName + ":" + password + "@" + theWebAddress;
            DebugOutput.Log($"Navigation is equal to {combinedurl}");
            try
            {
                if (webDriver == null)
                {
                    DebugOutput.Log($"We do not have a web driver!");
                    return success;
                }
                webDriver.Navigate().GoToUrl(combinedurl);
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed navigation");
                return success;
            }
        }

        public static Bitmap? GetImageOfElementOnPage(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetImageOfElementOnPage", $"{element} ");
            Bitmap? img;
            try
            {
                if (webDriver == null)
                {
                    DebugOutput.Log($"This is web only at the moment!");
                    return null;
                }
                img = GetElementScreenShot(element);
                return img;
            }
            catch
            {
                DebugOutput.Log($"FAILED TO IMAGE {element}");
            }
            return null;
        }


        /// <summary>
        /// Takes a screengrab of the page displayed
        /// saves it as TEMP.png supplied
        /// Convert that file into a Bitmap and return
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns>returns bitmap of screen shot</returns>
        public static bool? GetCurrentPageScreenShot(string id = "")
        {
            DebugOutput.OutputMethod($"Selenium - GetCurrentPageScreenShot", $"{id} ");
            if (webDriver == null)
            {
                DebugOutput.Log($"We do not have a web driver!");
                return false;
            }
            try
            {
                Screenshot ss = ((ITakesScreenshot)webDriver).GetScreenshot();
                var EPOCH = EPOCHControl.Epoch;
                var fileName = string.IsNullOrEmpty(id) ? "TEMPEPOCH.png" : $"{id}EPOCH.png";
                var newFileName = StringValues.TextReplacementService(fileName);
                var fullFileName = FileUtils.GetImagesErrorDirectory() + "/" + newFileName;
                DebugOutput.Log($"SAVING IMAGE IN {fullFileName}");
                ss.SaveAsFile(fullFileName);
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to take page image: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Takes a screengrab of the page displayed
        /// saves it as pageName.png supplied
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns>returns true if successful</returns>
        public static bool SaveCurrentPageImage(string pageName)
        {
            DebugOutput.OutputMethod($"Selenium - GetCurrentPageImage", $" {pageName} ");
            if (pageName.Length > 30) pageName = pageName.Substring(0,30);
            try
            {
                if (webDriver == null)
                {
                    DebugOutput.Log($"We do not have a web driver!");
                    return false;
                }
                Screenshot sc = ((ITakesScreenshot)webDriver).GetScreenshot();
                Bitmap? img;
                if (OperatingSystem.IsWindows())
                {
                    img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap; DebugOutput.Log($"Current directory = {currentDirectory}");
                }
                else img = null;
                var fullfileName = currentDirectory + outputFolder + pageName + ".png";
                DebugOutput.Log($"putting  file {fullfileName}");
                if (img == null)
                {
                    DebugOutput.Log($"We failed in the image from stream");
                    return false;
                }
                if (OperatingSystem.IsWindows()) img.Save(fullfileName, System.Drawing.Imaging.ImageFormat.Png);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to take page image");
                return false;
            }
        }

        /// <summary>
        /// Takes a screengrab of the page displayed
        /// but unlike GetCurrentPageImage it does not care what page is displayed
        /// This is more useful for fails
        /// </summary>
        /// <returns>returns true if succesful</returns>
        public static bool ScreenShotPage()
        {
            DebugOutput.OutputMethod($"Selenium - ScreenShotPage ");
            try
            {
                if (webDriver == null)
                {
                    DebugOutput.Log($"We do not have a web driver!");
                    return false;
                }
                Screenshot sc = ((ITakesScreenshot)webDriver).GetScreenshot();
                Bitmap? img;
                if (OperatingSystem.IsWindows())
                {
                    img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap; DebugOutput.Log($"Current directory = {currentDirectory}");
                }
                else
                {
                    img = null;
                }
                var fullfileName = currentDirectory + outputFolder + "currentPage.png";
                DebugOutput.Log($"putting  file {fullfileName}");
                if (img == null)
                {
                    DebugOutput.Log($"We failed in the image from stream");
                    return false;
                }
                if (OperatingSystem.IsWindows()) img.Save(fullfileName, System.Drawing.Imaging.ImageFormat.Png);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to take page image");
                return false;
            }
        }
        

       /// <summary>
       /// Pass in the name of the element and page name
       /// says it as elementName.png
       /// </summary>
       /// <param name="elementName"></param>
       /// <returns>returns true if successful in finding file of elementName</returns>
        public static bool ScreenShotElementAlreadyExists(string elementName, string pageName = "")
        {
            DebugOutput.OutputMethod($"Selenium - ScreenShotElementAlreadyExists", $"'{elementName}' '{pageName}'");
            FileUtils.SetCurrentDirectoryToTop();
            DebugOutput.Log($"ScreenShotElementAlreadyExists Current directory = {currentDirectory}");
            string fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + elementName + ".png";
            return FileUtils.OSFileCheck(fullFileName);
        }


        public static bool ScreenShotElementAndCompare(IWebElement element, string elementName, string pageName = "", string outputDir = "")
        {
            DebugOutput.OutputMethod($"Selenium - ScreenShotElementAndCompare", $"{element} '{elementName}' '{pageName}' '{outputDir}'");
            try
            {
                if (webDriver == null && TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
                {
                    DebugOutput.Log($"We do not have a web driver!");
                    return false;
                }
                // if (winDriver == null && TargetConfiguration.Configuration.ApplicationType.ToLower() == "windows")
                // {
                //     DebugOutput.Log($"We do not have a WINDOWS driver!");
                //     return false;
                // }
                var fullFileName = "";
                var project = TargetConfiguration.Configuration.AreaPath;
                FileUtils.SetCurrentDirectoryToTop();
                DebugOutput.Log($"ScreenShotElementAndCompare Current directory = {currentDirectory}");
                var directory = FileUtils.GetImagePageDirectory(outputDir) + @"\" + pageName; 
                fullFileName = directory + "\\" + elementName + ".png";
                DebugOutput.Log($"getting  file {fullFileName}");
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        var bitmap = new Bitmap(@fullFileName);
                        if (bitmap == null)
                        {
                            DebugOutput.Log($"Failed to read bitmap!");
                            return false;
                        }
                        DebugOutput.Log($"We have the bitmap! want to compare bitmap to img");
                        var img = GetElementScreenShot(element);
                        if (img == null)
                        {
                            DebugOutput.Log($"Can not img screen shot, if no screen shot!");
                            return false;
                        }

                    }
                    catch
                    {
                        DebugOutput.Log($"Something went wrong with the bitmap file!");
                        return false;
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Something over arching went wrong!");
                return false;
            }
            return true;
        }


       /// <summary>
       /// Pass in an element and take a photo of it!
       /// saves it as elementName.png
       /// </summary>
       /// <param name="element"></param>
       /// <param name="elementName"></param>
       /// <returns>returns true if successful in finding and taking image of element</returns>
        public static bool ScreenShotElement(IWebElement element, string elementName, string pageName = "")
        { 
            DebugOutput.OutputMethod($"Selenium - ScreenShotElement", $" {element} '{elementName}' '{pageName}' ");
            try
            {
                if (webDriver == null && TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
                {
                    DebugOutput.Log($"We do not have a web driver!");
                    return false;
                }
                // if (winDriver == null && TargetConfiguration.Configuration.ApplicationType.ToLower() == "windows")
                // {
                //     DebugOutput.Log($"We do not have a WINDOWS driver!");
                //     return false;
                // }
                var img = GetElementScreenShot(element);
                if (img == null)
                {
                    DebugOutput.Log($"Can not save screen shot, if no screen shot!");
                    return false;
                }
                FileUtils.SetCurrentDirectoryToTop();
                DebugOutput.Log($"Current directory = {currentDirectory}");
                var fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + elementName + ".png";
                DebugOutput.Log($"putting  file {fullFileName}");
                if (File.Exists(fullFileName))
                {
                    try
                    {
                        File.Delete(fullFileName);
                    }
                    catch
                    {
                        DebugOutput.Log($"failed to delete - will hopefully overwrite");
                    }
                }
                if (OperatingSystem.IsWindows()) DebugOutput.Log($"Clean and ready to save {img.Size.Width} x {img.Size.Height}");
                if (OperatingSystem.IsWindows()) img.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Png);
                DebugOutput.Log($"Saved file");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int GetWidthOfElement(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetWidthOfElement", $" {element} ");
            return element.Size.Width;
        }

        public static int GetHeightOfElement(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetHeightOfElement", $" {element} ");
            return element.Size.Height;
        }

        public static bool ListOfElementNamesContains(string partualName)
        {
            DebugOutput.OutputMethod($"Selenium - ListOfElementNamesContains", $" {partualName} ");
            List<string> elementNames = GetAllEmenetsNamesAsList();
            foreach(string elementName in elementNames)
            {
                DebugOutput.Log($"NAME = {elementName}");   
                if (elementName.Contains(partualName)) return true;
            }
            DebugOutput.Log($"Looked through {elementNames.Count} element names - did not find!");
            return false;
        }

        public static string? GetTabTitle()
        {
            DebugOutput.OutputMethod($"Selenium - GetTabTitle ");
            if (webDriver == null) return null;
            return webDriver.Title;
        }

        private static List<string> GetAllEmenetsNamesAsList()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllEmenetsNamesAsList ");
            var elements = GetAllElements();
            List<string> result = new List<string>();
            foreach (var element in elements)
            {
                var name = GetElementAttributeValue(element, "Name");
                if (name != null) result.Add(name);
            }
            return result;
        }

        /// <summary>
        /// Create a bitmap image of an element passed in
        /// not the same as ScreenShotElement as that creates and saves the image, this just creates it
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns the bitmap, if fails returns null</returns>
        public static Bitmap? GetElementScreenShot(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementScreenShot", $" {element} ");
            Bitmap? img = null;
            try
            {
                if (TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
                {
                    DebugOutput.Log($"WEB");
                    if (webDriver == null) return null;
                    var driver = webDriver;
                    Screenshot sc = ((ITakesScreenshot)driver).GetScreenshot();
                    DebugOutput.Log($"SCREEN SHOT TAKEN");
                    if (OperatingSystem.IsWindows())
                    {
                        DebugOutput.Log($"Windows change");
                        img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap;
                        DebugOutput.Log($"Windows change DONE");
                    }
                    else img = null;
                    if (img == null)
                    {
                        DebugOutput.Log($"Issue grabbing image Bitmap!");
                        return null;
                    }
                }
                else
                {
                    // if (winDriver == null) return null;
                    // var driver = winDriver;
                    // Screenshot sc = ((ITakesScreenshot)driver).GetScreenshot();
                    // if (OperatingSystem.IsWindows()) img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap;                    
                    // if (img == null)
                    // {
                    //     DebugOutput.Log($"Issue grabbing image Bitmap!");
                    //     return null;
                    // }
                    return null;
                }
                DebugOutput.Log($"We have our image {img}");
                var rect = new Rectangle(element.Location, element.Size);
                DebugOutput.Log($"We have a rect {element} {element.Size}");
                if (element.Size.Width == 0 || element.Size.Height == 0) return img;
                if (OperatingSystem.IsWindows()) return img.Clone(rect, img.PixelFormat);
                return null;
            }
            catch
            {
                DebugOutput.Log($"Failed to capture image!");
                return null;
            }
        }

        public static bool SetWindowSize(string size)
        {
            DebugOutput.OutputMethod($"Selenium - SetWindowSize", $" {size} ");
            switch(size.ToLower())
            {
                default:
                {
                    var defaultScreenSize = TargetConfiguration.Configuration.ScreenSize;
                    var listOfSize = StringValues.BreakUpByDelimitedToList(defaultScreenSize,"x");
                    var defaultx = int.Parse(listOfSize[0]);
                    var defaulty = int.Parse(listOfSize[1]);
                    return SetWindowSize(defaultx, defaulty);
                }
                case "large":
                {
                    var x = 4000;
                    var y = 4000;
                    return SetWindowSize(x, y);
                }
                case "small":
                {
                    var x = 800;
                    var y = 600;
                    return SetWindowSize(x, y);
                }
            }

        }

        /// <summary>
        /// Set the windows size
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>returns true if successful</returns>
        public static bool SetWindowSize(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Selenium - SetWindowSize", $" {x}x{y} ");
            if (x == 0 || y == 0)
            {
                var size = TargetConfiguration.Configuration.ScreenSize;
                var listOfSize = StringValues.BreakUpByDelimitedToList(size,"x");
                try
                {
                    x = Int32.Parse(listOfSize[0]);
                    y = Int32.Parse(listOfSize[1]);
                }
                catch
                {
                    DebugOutput.Log($"Failed to convert {listOfSize} to x and y");
                    return false;
                }
            }

            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType == "web")
            {
                try
                {
                    if (webDriver == null) return false;
                    webDriver.Manage().Window.Size = new System.Drawing.Size(x, y);
                    return true;
                }
                catch
                {
                    DebugOutput.Log($"Failed to set it!");
                    return false;
                }
            }
            if (appType == "windows")
            {
                // if (winDriver == null) return false;
                // try
                // {
                //     winDriver.Manage().Window.Size = new System.Drawing.Size(x, y);
                //     return true;
                // }
                // catch (Exception ex)
                // {
                //     DebugOutput.Log($"{ex}");
                //     return false;
                // }
            }
            return false;
        }


        public static bool DownKeyAndClick(IWebElement element, string key)
        {
            DebugOutput.OutputMethod($"Selenium - DownKeyAndClick", $" {element} {key}");
            bool success = false;
            var action = GetActions();

            if (action == null) return success;
            try
            {
                action.MoveToElement(element);
                action.Build();
                action.Perform();
                if (key.ToLower() == "ctrl")
                {
                    action.KeyDown(Keys.LeftControl);
                }
                action.Click();
                if (key.ToLower() == "ctrl")
                {
                    action.KeyUp(Keys.LeftControl);
                }
                action.Build();
                action.Perform();
                success = true;
                return success;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {element} {ex}");
                return success;
            }

        }


        /// <summary>
        /// Send single or multiple keypresses to an element, by name
        /// default just sends the key(s) supplied
        /// down, down arrow = Keys.Down
        /// clear = Ctl+A then delete
        /// close = Alt+F4
        /// enter = Keys.Return
        /// escape = Keys.Escape
        /// tab = Keys.Tab
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns>returns true if successful</returns>
        public static bool SendKey(IWebElement element, string key)
        {
            DebugOutput.OutputMethod($"Selenium - SendKey", $" {element} {key} ");
            if (string.IsNullOrEmpty(key)) return false;
            key = key.ToLower();    
            try
            {
                switch (key)
                {
                    default:
                        {
                            element.SendKeys(key);
                            return true;
                        }
                    case "clear":
                        {
                            element.SendKeys(Keys.Control + "a");
                            element.SendKeys(Keys.Delete);
                            return true;
                        }
                    case "close":
                        {
                            element.SendKeys(Keys.Alt + Keys.F4);
                            return true;
                        }
                    case "down":
                    case "down arrow":
                        {
                            element.SendKeys(Keys.Down);
                            return true;
                        }
                    case "enter":
                    case "return":
                        {
                            DebugOutput.Log($"Sending enter key");
                            element.SendKeys(Keys.Return);
                            return true;
                        }
                    case "escape":
                        {
                            element.SendKeys(Keys.Escape);
                            return true;
                        }
                    case "page down":
                    case "pagedown":
                        {
                            element.SendKeys(Keys.PageDown);
                            return true;
                        }
                    case "page up":
                    case "pageup":
                        {
                            element.SendKeys(Keys.PageUp);
                            return true;
                        }
                    case "tab":
                        {
                            element.SendKeys(Keys.Tab);
                            return true;
                        }
                }
            }
            catch
            {
                DebugOutput.Log($"problem sending key!");
                return false;
            }
        }

        /// <summary>
        /// Supply an element, is it enabled (i.e. can it be interacted with)
        /// </summary>
        /// <param name="element"></param>
        /// <returns>return true if the element can be interacted with, false if no interaction is possible</returns>
        public static bool IsEnabled(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - IsEnabled", $" {element}");
            try
            {
                // need a wee check for a disable attribute not picked up in element.Enabled!
                if (element.Enabled)
                {
                    DebugOutput.Log($"CHECKING DISABLED ATTRIBUTE");
                    var attribute = GetElementAttributeValue(element, "disabled");
                    if (attribute != null)
                    {
                        DebugOutput.Log($"We have a disabled attribute! {attribute}");
                        return false;
                    }

                }
                return element.Enabled;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Enabled flag! {ex}");
                return false;
            }
        }

        public static bool? IsSelectedSwitch(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - IsSelectedSwitch", $" {element}");
            try
            {
                // quickest check is the aria-checked attribute
                var ariaChecked = GetElementAttributeValue(element, "aria-checked");
                if (ariaChecked != "" || ariaChecked != null)
                {
                    DebugOutput.Log($"We have an aria-checked attribute! {ariaChecked}");
                    if (ariaChecked.ToLower() == "true") return true;
                    if (ariaChecked.ToLower() == "false") return false;
                }
                // Switches have an element (buttton) with different class if on or of
                var buttonElement = GetElementUnderElement(element, By.ClassName("mdc-switch--unselected"), 1);
                if (buttonElement != null)
                {
                    DebugOutput.Log($"We have the class mdc-switch--unselected found! thus unselected!");
                    return false;
                }
                buttonElement = GetElementUnderElement(element, By.ClassName("mdc-switch--selected"));
                if (buttonElement != null)
                {
                    DebugOutput.Log($"We have the class mdc-switch--selected found! thus selected!");
                    return true;
                }
                DebugOutput.Log($"These are the only ways I know of currently! I return NULL");
                return null;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Selected flag! {ex}");
                return null;
            }
        }

        /// <summary>
        /// Supply an element, is it selected?  Normally used for checkboxes and radio buttons
        /// </summary>
        /// <param name="element"></param>
        /// <returns>return true if the element is selected, false if not</returns>
        public static bool IsSelected(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - IsSelected", $" {element}");
            try
            {
                DebugOutput.Log($"IT IS {element.Selected}");
                var select = element.Selected;
                DebugOutput.Log($"WE have a the selected element {select}");
                if (select) return select;
                DebugOutput.Log($"Still using class, can we check the text?");
                var classTitle = SeleniumUtil.GetElementAttributeValue(element, "class");
                DebugOutput.Log($"We have {classTitle} which may say ACTIVE?");
                if (classTitle.ToLower().Contains("active")) return true;
                DebugOutput.Log($"Problem is it is 'active' not selected! so neg to the selected");
                if (element.GetAttribute("class").Contains("active"))
                {
                    DebugOutput.Log($"We can check the class and it contains the class active!");
                    return true;
                }
                DebugOutput.Log($"If we have a space in the class name, we don't see the active part! so CSS attempt");
                var cSSText = element.GetCssValue("class");
                DebugOutput.Log($"CSS has given us {cSSText}");
                if (cSSText.ToLower().Contains("active")) return true;
                DebugOutput.Log($"Tried every way but loose! I wonder if it has a child that has the actual active flag?");
                return ChildIsSelected(element);
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Selected flag! {ex}");
                return false;
            }
        }

        private static bool ChildIsSelected(IWebElement parentElement)
        {
            DebugOutput.OutputMethod($"Selenium - ChildIsSelected", $"{parentElement}");
            try
            {
                var allChildElements = SeleniumUtil.GetAllElementsUnderElement(parentElement);
                foreach (var childElement in allChildElements)
                {
                    var select = childElement.Selected;
                    DebugOutput.Log($"WE have a the selected element {select}");
                    if (select) return select;
                    DebugOutput.Log($"Still using class, can we check the text?");
                    var classTitle = SeleniumUtil.GetElementAttributeValue(childElement, "class");
                    DebugOutput.Log($"We have {classTitle} which may say ACTIVE?");
                    if (classTitle.ToLower().Contains("active")) return true;
                }
                DebugOutput.Log($"Still no active flag - so it must not be active!");
                return false;
            }
            catch
            {
                DebugOutput.Log($"We have hit an issue when checking on Child Elements if they are selected!");
            }
            return false;
        }

        /// <summary>
        /// Supply an element, and return its parent element
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns the parent element of the supplied element, or null if top element supplied</returns>
        public static IWebElement? GetElementParent(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementParent", $" {element}");
            //DebugOutput.Log($"Sel - GetElementParent {element} ");
            try
            {
                return element.FindElement(By.XPath("./parent::*"));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// By using a locator, find and return an element within the timeout supplied,
        /// if no timeout supplied or timeout 0, will use app default
        /// This is used by web and windows applications
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns>returns the element found, or null if no element found</returns>
        public static IWebElement? GetElement(By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetElement", $" {locator} {timeout}");
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType == "web")
            {
                return GetWebElement(locator, timeout);
            }
            if (appType == "windows")
            {
                return GetWindowsElement(locator, timeout);
            }
            return null;
        }

        /// <summary>
        /// Get the CSS Attributes, and return 1 value based on attribute supplied
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string? GetElementCSSAttribute(IWebElement element, string attribute)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementsCSSAttribute", $" {attribute}");
            switch(attribute.ToLower())
            {
                default: return null;

                case "background color":
                case "background colour": return element.GetCssValue("background-color");

                case "color":
                case "colour": return element.GetCssValue("color");
            }
        }

        public static string? GetBackGroundColourOfElement(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetBackGroundColourOfElement", $"{element}");

            // WE NEED TO FIND OUR ELEMENT COLOUR
            var elementColour = GetElementCSSAttribute(element, "colour");
            if (elementColour == null) return null;
            var elementColourRGBA = ImageWorkings.GetRGBADetailsFromString(elementColour);

            //  WE HAVE OUR ELEMENT - IT MAY HAVE A BACKGROUND COLOUR IF IT DOES WE RETURN THAT AS A STRING!
            var elementBackGroundColour = GetElementCSSAttribute(element, "background colour");
            if (elementBackGroundColour != null)
            {
                DebugOutput.Log($"You can have a null background colour - but we don't!");
                if (elementColour != elementBackGroundColour)
                {
                    DebugOutput.Log($"We have different colours for our element and background of element {elementColour} {elementBackGroundColour}");
                    var elementBackGroundColourRGBA = ImageWorkings.GetRGBADetailsFromString(elementBackGroundColour);
                    DebugOutput.Log($"Transparent = {elementBackGroundColourRGBA.Alpha}");
                    if (elementBackGroundColourRGBA.Alpha > 0)
                    {
                        DebugOutput.Log($"We have different colours AND the background is NOT transparent!  we have a winner! {elementColour} {elementBackGroundColour}");
                        return elementBackGroundColour;
                    }
                    DebugOutput.Log($"But its transparent!  so No background!");
                }
            }
            DebugOutput.Log($"We have looked at the element background and it is not the background we see on screen - its transparent, or the same!");
            bool different = false;
            while (!different)
            {
                DebugOutput.Log($"Lets check its parent of {element}!");
                var parentElement = GetElementParent(element);
                if (parentElement == null)
                {
                    DebugOutput.Log($"We have an element with NO parent - we as far as we can go!  And no background colour!");
                    return null;
                }
                var parentColour = GetElementCSSAttribute(parentElement, "colour");
                if (elementColour != parentColour)
                {
                    if (parentColour != null)
                    {
                        DebugOutput.Log($"The parent element colour is different from the elements! {parentColour} {elementColour} ");
                        var parentElementColourRGBA = ImageWorkings.GetRGBADetailsFromString(parentColour);
                        if (parentElementColourRGBA.Alpha != 0)
                        {
                            DebugOutput.Log($"A different colour in the parent and NOT transparent!");
                            return parentColour;
                        }
                    }
                }
                DebugOutput.Log($"So the colour of the parent is no use, but IT may have a background!");
                var parentBackGroundColour =  GetElementCSSAttribute(parentElement, "background colour");
                if (parentBackGroundColour != null)
                {
                    DebugOutput.Log($"Parent DOES have a background colour {parentBackGroundColour}");
                    if (elementColour != parentBackGroundColour)
                    {
                        DebugOutput.Log($"And that is differetn from the element colour {elementColour}");
                        var parentBackGroundColourRGBA = ImageWorkings.GetRGBADetailsFromString(parentBackGroundColour);
                        if (parentBackGroundColourRGBA.Alpha > 0)
                        {
                            DebugOutput.Log($"We have a parent with different background colour non transparent! {parentBackGroundColour} ");
                            return parentBackGroundColour;
                        }
                    }
                }
                DebugOutput.Log($"We have check the elements parent - WHAT ABOUT ITS PARENT?");
                element = parentElement;
            }
            DebugOutput.Log($"we should be out by here!");
            return null;
        }


        public static string? GetElementIDAsString(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementIDAsString", $" {element}");
            return element.ToString();
        }


        private static List<IWebElement>? GetWindowsElements(By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetWindowsElements", $" {locator} {timeout}");
            return null;
            // timeout = GetTimeout(timeout);
            // try
            // {
            //     if (winDriver == null) return null;
            //     var list = winDriver.FindElements(locator);
            //     var listOfElements = new List<IWebElement>();
            //     foreach( var singleElement in list)
            //     {
            //         listOfElements.Add(singleElement);
            //     }
            //     return listOfElements;
            // }
            // catch
            // {                
            //     DebugOutput.Log("FAILED GET ANY ELEMENTs Sucessfully");
            //     string locatorXPath = locator.ToString();
            //     if (locatorXPath.Contains("By.Id: "))
            //     {
            //         locatorXPath = locatorXPath.Replace("By.Id: ", "");
            //         DebugOutput.Log($"Using Accesibiluy = {locatorXPath}");
            //         try
            //         {
            //             if (winDriver == null) return null;
            //             var list = winDriver.FindElementsByAccessibilityId(locatorXPath);
            //             var listOfElements = new List<IWebElement>();
            //             foreach( var singleElement in list)
            //             {
            //                 listOfElements.Add(singleElement);
            //             }
            //             return listOfElements;
            //         }
            //         catch
            //         {
            //             DebugOutput.Log($"Not even accessibility");
            //         }
            //     }
            //     DebugOutput.Log($"LOCATOR = {locator}");
            // }
            // return null;
        }

        /// <summary>
        /// By using a locator, find and return an element within the timeout supplied,
        /// if no timeout supplied or timeout 0, will use app default
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns>returns element if found, null if not</returns>
        private static IWebElement? GetWindowsElement(By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetWindowsElement", $" {locator} {timeout}");
            return null;
            // timeout = GetTimeout(timeout);
            // try
            // {
            //     var wait = new WebDriverWait(winDriver, TimeSpan.FromSeconds(timeout));
            //     return wait.Until(drv => drv.FindElement(locator));
            // }
            // catch
            // {
            //     DebugOutput.Log("FAILED GET ELEMENT");
            //     string locatorXPath = locator.ToString();
            //     if (locatorXPath.Contains("By.Id: "))
            //     {
            //         locatorXPath = locatorXPath.Replace("By.Id: ", "");
            //         DebugOutput.Log($"Using Accesibiluy = {locatorXPath}");
            //         try
            //         {
            //             if (winDriver == null) return null;
            //             return winDriver.FindElementByAccessibilityId(locatorXPath);
            //         }
            //         catch
            //         {
            //             DebugOutput.Log($"Not even accessibility");
            //         }
            //     }
            //     DebugOutput.Log($"LOCATOR = {locator}");
            //     return null;
            // }
        }

        /// <summary>
        /// By using a locator, find and return an element within the timeout supplied,
        /// if no timeout supplied or timeout 0, will use app default
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static IWebElement? GetWebElement(By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetWebElement", $" {locator} {timeout}");
            if (webDriver == null) return null;
            timeout= GetTimeout(timeout);
            try
            {
                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout));
                return wait.Until(drv => drv.FindElement(locator));
            }
            catch
            {
                DebugOutput.Log("FAILED GET ELEMENT");
                return null;
            }
        }

        public static int GetPageWidth()
        {
            DebugOutput.OutputMethod($"Selenium - GetPageWidth");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
            {
                if (webDriver == null) return 0;
                return webDriver.Manage().Window.Size.Width;
            }
            // if (winDriver == null) return 0;
            // return winDriver.Manage().Window.Size.Width;
            return 0;
        }

        public static int GetPageHeight()
        {
            DebugOutput.OutputMethod($"Selenium - GetPageHeight");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
            {
                if (webDriver == null) return 0;
                return webDriver.Manage().Window.Size.Height;
            }
            // if (winDriver == null) return 0;
            // return winDriver.Manage().Window.Size.Height;
            return 0;
        }

        public static int GetTimeout(int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetTimeout", $"{timeout}");
            if (timeout == 0) timeout = TargetConfiguration.Configuration.PositiveTimeout;
            timeout *= TargetConfiguration.Configuration.TimeoutMultiplie;
            DebugOutput.Log($"Timeout set to {timeout}");
            return timeout;
        }

        /// <summary>
        /// Find the very top element of the driver
        /// in a PC this COULD be above the application in question
        /// </summary>
        /// <returns>returns the top element</returns>
        public static IWebElement? GetTopElement()
        {
            DebugOutput.OutputMethod($"Selenium - GetTopElement ");
            var locator = By.XPath("//*");
            var element = GetElement(locator);
            DebugOutput.Log($" WE HAVE THE ELEMENT = {element}");
            return element;
        }

        /// <summary>
        /// Get all the elements and put them in a list
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>returns a list of all elements found</returns>
        public static List<IWebElement> GetAllElements(int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetAllElements", $" {timeout} ");
            var element = GetTopElement();
            var locator = By.XPath("//*");
            List<IWebElement> elements = new List<IWebElement>();
            if (element == null) return elements;
            elements = GetElementsUnder(element, locator);
            DebugOutput.Log($"WE have {elements.Count} elements IN TOTAL");
            return elements;
        }

        /// <summary>
        /// Get all the elements but only return those that are visiable!
        /// </summary>
        /// <returns>returns a list of all elements found but only if visible</returns>
        public static List<IWebElement> GetAllVisibleElements()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllVisibleElements  ");
            var allElements = GetAllElements();
            var visibleElements = new List<IWebElement>();
            foreach (var element in allElements)
            {
                if (element.Displayed) visibleElements.Add(element);
            }
            return visibleElements;
        }


        /// <summary>
        /// Get every visible element, and make a list of those element that have no children!
        /// </summary>
        /// <returns></returns>
        public static List<IWebElement> GetAllChildlessElements()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllChildlessElements");
            var returnElements = new List<IWebElement>();
            var allElements = GetAllVisibleElements();
            foreach (var element in allElements)
            {
                IReadOnlyList<IWebElement> childs = element.FindElements(By.XPath(".//*"));
                Int32 numofChildren = childs.Count;
                if (numofChildren == 0) returnElements.Add(element);
            }
            return returnElements;
        }


        public static List<IWebElement> GetAllChildlessElementsWithText()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllChildlessElementsWithText");
            var returnElements = new List<IWebElement>();
            var allChildElements = GetAllChildlessElements();
            foreach (var element in allChildElements)
            {
                var text = SeleniumUtil.GetElementtextDirect(element);
                if (text != null)
                {
                    var size = text.Length;
                    if (text != "" || text != null || text != string.Empty || size != 0)
                    {
                        DebugOutput.Log($"TEXT IS '{text}'");
                        returnElements.Add(element);
                    }

                }
            }
            return returnElements;
        }

        /// <summary>
        /// Get all Elements
        /// </summary>
        /// <returns>Only elements currently displayed</returns>
        public static List<IWebElement> GetAllDisplayedElements()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllDisplayedElements");
            var returnElements = new List<IWebElement>();
            var allElements = GetAllElements();
            foreach (var element in allElements)
            {
                if (element.Displayed)
                {
                    returnElements.Add(element);
                }
            }
            return returnElements;
        }


        public static List<IWebElement> GetAllCurrentlyLoadedButHiddenElements()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllCurrentlyLoadedButHiddenElements");
            var returnElements = new List<IWebElement>();
            var allElements = GetAllElements();
            foreach (var element in allElements)
            {
                if (!element.Displayed)
                {
                    returnElements.Add(element);
                }
            }
            return returnElements;
        }


        public static List<IWebElement> GetAllHeadersDisplayed()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllHeadersDisplayed");
            var returnElements = new List<IWebElement>();
            var allElements = GetAllElements();
            foreach (var element in allElements)
            {
                if (element.Displayed)
                {
                    // var headerLocators = new List<By>{By.TagName("h1"), By.TagName("h2"), By.TagName("h3"), By.TagName("h4"), By.TagName("h5"), By.TagName("h6"), By.TagName("h7"), By.TagName("h8"), By.TagName("h9"), By.TagName("h10")};
                    var headerTags = new List<string>{"h1","h2","h3","h4","h5","h6","h7","h8","h9","h10"};
                    var elementTag = SeleniumUtil.GetElementTagName(element);
                    if (headerTags.Contains(elementTag)) returnElements.Add(element);                    
                }
            }
            return returnElements;
        }
        

        public static List<IWebElement> GetAllImageElementsDisplayed()
        {
            DebugOutput.OutputMethod($"Selenium - GetAllImageElementsDisplayed");
            var locator = By.TagName("img");
            var returnElements = new List<IWebElement>();
            var allElements = GetElements(locator, 1);
            if (allElements == null) return returnElements;
            foreach (var element in allElements)
            {
                if (element.Displayed) returnElements.Add(element);
            }
            return returnElements;
        }


        public static IWebElement? GetAllInputElementsByText(string text)
        {
            DebugOutput.OutputMethod($"Selenium - GetAllInputElementsByText", $" {text}  ");
            var locator = By.TagName("input");
            var allInputElements = GetElements(locator, 1);
            if (allInputElements == null) return null;
            DebugOutput.Log($"We have {allInputElements.Count} INPUT elements found!");
            foreach (var element in allInputElements)
            {
                var elementText = GetElementText(element);
                if (elementText.ToLower() == text.ToLower())
                {
                    DebugOutput.Log($"FOUND AN INPUT ELEMENT USING THE TEXT {text} we return the first found!");
                    return element;
                }
            }
            DebugOutput.Log($"Failed to find any element with that text!");
            return null;
        }

        public static IWebElement? GetInputElementByParentText(string text)
        {
            DebugOutput.OutputMethod($"Selenium - GetInputElementByParentText", $" {text}  ");
            var locator = By.TagName("input");
            var allInputElements = GetElements(locator, 1);
            if (allInputElements == null) return null;
            DebugOutput.Log($"We have {allInputElements.Count} INPUT elements found! Which fit for our location");
            foreach (var element in allInputElements)
            {
                var parentOfElement = GetElementParent(element);
                if (parentOfElement != null)
                {
                    var elementText = GetElementText(parentOfElement);
                    if (elementText.ToLower() == text.ToLower())
                    {
                        DebugOutput.Log($"FOUND AN INPUT ELEMENT USING THE TEXT OF ITS PARENT! {text} we return the first found!");
                        return element;
                    }
                }
            }
            DebugOutput.Log($"Failed to find any element with that text even the inputs parent!!");
            return null;
        }

        public static List<IWebElement> GetAllElementsUnderElement(IWebElement parent)
        {
            DebugOutput.OutputMethod($"Selenium - GetAllElementsUnderElement", $" {parent}  ");
            // var locator = By.XPath($"//*");
            var locator = By.CssSelector("*");
            var allElements = GetElementsUnder(parent, locator);
            DebugOutput.Log($"WE have {allElements.Count} elements IN TOTAL UNDER THE PARENT {parent}");
            return allElements;
        }

        /// <summary>
        /// Get all the elements found by using a locator
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns>returns list of elements</returns>
        public static List<IWebElement>? GetElements(By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetElements {locator}", $" {timeout}  ");
            var elementList = new List<IWebElement>();
            // if (winDriver != null)
            // {
            //     return GetWindowsElements(locator, timeout);
            // }
            timeout = GetTimeout(timeout);
            try
            {
                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout));
                var elements = wait.Until(drv => drv.FindElements(locator));
                foreach (var element in elements)
                {
                    elementList.Add(element);
                }
                DebugOutput.Log($"We have {elementList.Count} elements found");
                return elementList;
            }
            catch
            {
                DebugOutput.Log("FAILED GET ELEMENTSSSS");
                return elementList;
            }
        }

        /// <summary>
        /// Get first element using a locator found UNDER a supplied element - Web ONLY
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns>returns element found, or null if not found</returns>
        public static IWebElement? GetElementUnderElement(IWebElement parentElement, By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementUnderElement", $" {parentElement} {locator} {timeout}  ");
            if (webDriver == null) return null;
            timeout = GetTimeout(timeout);
            try
            {
                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout));
                return wait.Until(driv => parentElement.FindElement(locator));
            }
            catch
            {
                DebugOutput.Log("failed to find element");
                return null;
            }
        }


        public static string? GetCurrentWebPageURL()
        {
            DebugOutput.OutputMethod($"Selenium - GetCurrentWebPageURL ");
            if (webDriver == null) return null;
            return webDriver.Url;
        }


        /// <summary>
        /// Get list of elements using a locator found UNDER a supplied element
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static List<IWebElement> GetWebElementsUnder(IWebElement parent, By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetWebElementsUnder", $" {parent} {locator} {timeout}  ");

            var elementList = new List<IWebElement>();

            var elementsQuick = parent.FindElements(locator);
            foreach(var element in elementsQuick)
            {
                DebugOutput.Log($"adding - {element}");
                elementList.Add(element);
            }    
            DebugOutput.Log($"returning {elementList.Count()} elements found under {parent} using {locator}");
            return elementList;
        }

        /// <summary>
        /// Get List of Elements found under a supplied element by locator
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static List<IWebElement> GetWindowsElementsUnder(IWebElement parent, By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetWindowsElementsUnder", $" {parent} {locator} {timeout}  ");
            var elementList = new List<IWebElement>();
            try
            {
                var windowElements = parent.FindElements(locator);
                foreach(var element in windowElements)
                {
                    elementList.Add((IWebElement)element);
                }
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"FAILED GET ELEMENTSSsssss {ex}");
            }
            DebugOutput.Log($"FOUND {elementList.Count} windows elements under {parent}");
            return elementList;
        }

        /// <summary>
        /// Get all elements found element by locator - web or windows
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="locator"></param>
        /// <param name="timeout"></param>
        /// <returns>returns list of elements</returns>
        public static List<IWebElement> GetElementsUnder(IWebElement parent, By locator, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementsUnder", $" {parent} {locator} {timeout}  ");
            var elementList = new List<IWebElement>();
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType == "web")
            {
                return GetWebElementsUnder(parent, locator, timeout);
            }
            if (appType == "windows")
            {
                return GetWindowsElementsUnder(parent, locator, timeout);
            }
            return elementList;
        }
        

        public static string GetElementTagName(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementTagName", $" {element} ");
            return element.TagName;
        }

        public static string? GetElementTextDirectOneLine(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementtextDirect", $" {element} ");
            // Get the full HTML of the element
            string htmlContent = element.GetAttribute("outerHTML");

            string cleanedText = Regex.Replace(htmlContent, "<sup>.*?</sup>", string.Empty, RegexOptions.Singleline);

            // Use a regular expression to remove the HTML tags
            string textWithChildrenRemoved = Regex.Replace(cleanedText, "<[^>]*>", "");

            // Remove any remaining HTML entities
            string newCleanedText = HttpUtility.HtmlDecode(textWithChildrenRemoved);

            return newCleanedText;
        }


        public static string? GetElementtextDirect(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementtextDirect", $" {element} ");
            try
            {
                var text = element.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    DebugOutput.Log($"WE GOT A VALUE FROM .Text = '{text}'");
                    return text;
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to read direct");
            }
            return null;
        }

        /// <summary>
        /// Get the text of an element
        /// Some elements store text as text, value or textcontent.  Will try in that order and return the first that is not null
        /// </summary>
        /// <param name="element"></param>
        /// <returns>returns a string of text or if none found in the 3 attributes, return ""</returns>
        public static string GetElementText(IWebElement element)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementText", $" {element} ");
            if (element == null) return failedFindElement;
            string? returnText = null; 
            if (TargetConfiguration.Configuration.ApplicationType == "windows")
            {
                DebugOutput.Log($"For WINDOWS Will try Direct first!");
                var direct = GetElementtextDirect(element);
                if (!string.IsNullOrEmpty(direct)) return direct;
                DebugOutput.Log($"For WINDOWS Will try Name Second - it has to have a name!");
                DebugOutput.Log($"Name sometimes gets the value");
                if (!string.IsNullOrEmpty(GetElementAttributeValue(element, "Name"))) return SeleniumUtil.GetElementAttributeValue(element, "textContent");
            }
            try
            {
                returnText = element.Text;
                if (returnText != "0" || returnText != null)
                {
                    DebugOutput.Log($"We have THE TEXT  '{returnText}' from the element {element}");
                    if (returnText != "")
                    {
                        DebugOutput.Log($"We have the non null text being returned of '{returnText}");  
                        return returnText;
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to get text in direct query!");
            }
            DebugOutput.Log($"Attribute Text");
            if (!string.IsNullOrEmpty(GetElementAttributeValue(element, "text"))) return SeleniumUtil.GetElementAttributeValue(element, "text");
            DebugOutput.Log($"Attribute Value");
            if (!string.IsNullOrEmpty(GetElementAttributeValue(element, "value"))) return SeleniumUtil.GetElementAttributeValue(element, "value");
            DebugOutput.Log($"Attribute textContent");
            if (!string.IsNullOrEmpty(GetElementAttributeValue(element, "textContent"))) return SeleniumUtil.GetElementAttributeValue(element, "textContent");
            DebugOutput.Log($"Attribute original title");
            if (!string.IsNullOrEmpty(GetElementAttributeValue(element, "data-original-title"))) return SeleniumUtil.GetElementAttributeValue(element, "textContent");
            DebugOutput.Log($"FAILED ALL TEXT KNOWN TO MAN, i.e. me!");
            return "";
        }

        /// <summary>
        /// In web you find a tag by tag, but in windows its tagName.  same with Class and Id
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns>returns a string of the correct attribute name for type of test</returns>
        private static string GetCorrectAttributeName(string attributeName)
        {
            DebugOutput.OutputMethod($"Selenium - GetCorrectAttributeName", $" {attributeName} ");
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (attributeName.ToLower() == "tag")
            {
                return "TagName";
            }
            if (attributeName.ToLower() == "class" || attributeName.ToLower() == "classname" || attributeName.ToLower() == "tagClass")
            {
                if (appType == "windows")
                {
                    return "ClassName";
                }
            }
            if (attributeName.ToLower() == "id")
            {
                if (appType == "windows")
                {
                    return "AutomationId";
                }
            }
            return attributeName;
        }

        /// <summary>
        /// Return a list of expected attributes.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributes"></param>
        /// <returns>return a list of expected attributes.</returns>
        public static List<string> GetElementMultipleAttributesValues(IWebElement element, List<string> attributes)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementMultipleAttributesValues", $" {element} {attributes.Count} ");
            var attributeValueList = new List<string>();
            string modifiedAttributeName;
            foreach (var attribute in attributes)
            {
                modifiedAttributeName = GetCorrectAttributeName(attribute);
                var value = "";
                //Everything has a tag so is not classed as an element
                if (modifiedAttributeName == "TagName")
                {
                    value = element.TagName;
                }
                else
                {
                    value = GetElementAttributeValue(element, modifiedAttributeName);
                }
                if (value.Length < 1)
                {
                    attributeValueList.Add("NULL");
                }
                else
                {
                    attributeValueList.Add(value);
                }
            }
            return attributeValueList;
        }


        /// <summary>
        /// Given an element return a full XPath
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string? GetFullXPathOfElement(IWebElement? element)
        {
            DebugOutput.OutputMethod($"Selenium - GetFullXPathOfElement", $" {element}  ");
            if (element == null) return null;            
            string xpath = "";
            while (element != null)
            {
                var index = 0;
                var siblings = element.FindElements(By.XPath("preceding-sibling::*"));
                foreach (var sibling in siblings)
                {
                    if (sibling.TagName == element.TagName) index++;
                }
                xpath = "/" + element.TagName + "[" + (index + 1) + "]" + xpath;
                try
                {
                    element = element.FindElement(By.XPath("./parent::*"));
                    var elementid = GetElementAttributeValue(element, "id");
                    int letterCounter = Regex.Matches(elementid,@"[a-zA-Z]").Count;
                    if (letterCounter > 0)
                    {
                        DebugOutput.Log($"We have an id of {elementid} and tag of {element.TagName}");
                        // //div[@id='footerPanel']
                        var startOfXPath = @"//" + element.TagName + "[@id='"+ elementid + "']";
                        xpath = startOfXPath + xpath;
                        return xpath;
                    }                    
                }
                catch
                {
                    element = null;
                }
            }
            return xpath;



            // IWebElement? parentElement;
            // string fullXPath = "";
            // bool gotFullXPath = false;
            // var currentElementTag = GetElementAttributeValue(element, "tag");
            // fullXPath = $"{currentElementTag}";
            // while (!gotFullXPath)
            // {
            //     parentElement = GetElementParent(element);
            //     if (parentElement != null)
            //     {
            //         var parentTag = GetElementAttributeValue(parentElement, "tag");
            //         fullXPath = parentTag + @"/" + fullXPath;
            //         element = parentElement;
            //     }
            //     else
            //     {
            //         fullXPath = @"(//" + fullXPath + ")";
            //         gotFullXPath = true;    
            //     }
            // }
            // return fullXPath;
        }


        public static bool SwapTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod($"Selenium - SwapTabByNumber", $" {tabNumber}  ");
            if (webDriver == null) return false;
            int counter = 1;
            foreach (string window in webDriver.WindowHandles)
            {
                webDriver.SwitchTo().Window(window);
                if (counter == tabNumber) return true;
                counter ++;
            }
            DebugOutput.Log($"Failed to switch window/tab {tabNumber}");
            return false;
        }


        public static bool CloseTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod($"Selenium - CloseTabByNumber", $" {tabNumber}  ");
            DebugOutput.Log($"Sel - CloseTabByNumber {tabNumber}");
            if (webDriver == null) return false;
            if (!SwapTabByNumber(tabNumber)) return false;
            webDriver.Close();
            var numberOfTabs = GetNumberOfTabsOpenInBrowser();
            if (numberOfTabs < 1) return false;
            return SwapTabByNumber(1);
        }


        public static int GetNumberOfTabsOpenInBrowser()
        {
            DebugOutput.OutputMethod($"Selenium - GetNumberOfTabsOpenInBrowser  ");
            if (webDriver == null) return 0;
            return webDriver.WindowHandles.Count;
        }

        /// <summary>
        /// Supply an element and what attribute you want the value of
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <returns>return string value of attribute of given element</returns>
        public static string GetElementAttributeValue(IWebElement element, string attribute)
        {
            DebugOutput.OutputMethod($"Selenium - GetElementAttributeValue", $" {element} {attribute}  ");
            string blank = "";
            if (string.IsNullOrEmpty(attribute)) return blank;
            if (attribute.ToLower() == "elementid")
            {
                if (element == null) return blank;
                var str = element.ToString();
                if (str == null) return blank;
                return str;
            }
            if (attribute.ToLower() == "locatorname")
            {
                attribute = "Name";
            }
            if (attribute.ToLower() == "tagname" || attribute.ToLower() == "tag")
            {
                string value = blank;
                try
                {
                    value = element.TagName;
                }
                catch
                {
                    DebugOutput.Log($"There should be no element EVERY with no tag, but we've found one!");
                }
                if (value.Length > 0) return value;
            }
            if (attribute.ToLower() == "tagclass")
            {
                attribute = "class";
            }
            if (attribute.ToLower() == "fullxpath")
            {
                //Get the element - get every element above it
                var xpath = GetFullXPathOfElement(element);
                if (xpath == null) return "";
                return xpath;
            }
            try
            {
                var attributeValue = element.GetAttribute(attribute);
                if (attributeValue == null)
                {
                    DebugOutput.Log($"We read the attribute {attribute} and got a null return! so need to return that!");
                    return blank;
                }
                DebugOutput.Log($"We read the attribute {attribute} and got '{attributeValue}' Which is better than nothing!");
                return attributeValue;
            }
            catch
            {
                DebugOutput.Log($"Failed to read attribute {attribute}");
                return blank;
            }
        }

        /// <summary>
        /// Get the Actions for each type of Driver
        /// </summary>
        /// <returns>Correct action driver for type of application under test</returns>
        private static Actions? GetActions()
        {
            DebugOutput.OutputMethod($"Selenium - GetActions ");
            var type = TargetConfiguration.Configuration.ApplicationType.ToLower();
            DebugOutput.Log($"GetActions = {type} ");
            if (type == "web")
            {
                Actions action = new Actions(webDriver);
                return action;
            }
            // if (type == "windows")
            // {
            //     Actions action = new Actions(winDriver);
            //     return action;
            // }
            DebugOutput.Log($"Failed to get ACTION!");
            return null;
        }


    }
}
