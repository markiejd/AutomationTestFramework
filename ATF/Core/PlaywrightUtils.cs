using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Core.FileIO;
using Core.Images;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Core
{
    public static class PlaywrightUtils
    {
        public static IBrowser? playwrightBrowser = null;
        public static IContext? playwrightContext = null;
        public static IPage? playwrightPage = null;
        public static string outputFolder = @"\AppSpecFlow\TestOutput\";
        public static string compareFolder = @"\AppSpecFlow\TestCompare\";
        public static string failedFindElement = "Failed to find element!";
        public static int test = TargetConfiguration.Configuration.Debug;
        public static int DefaultPositiveTimeOut = TargetConfiguration.Configuration.PositiveTimeout;
        public static string currentDirectory = Directory.GetCurrentDirectory();

        /// <summary>
        /// Handle JavaScript alerts/dialogs
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true if successful</returns>
        public static bool AlertLogin(string username, string password)
        {
            DebugOutput.OutputMethod($"Playwright - AlertLogin", $"{username} {password}");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            if (playwrightPage == null) return success;

            try
            {
                playwrightPage.OnDialog += async dialog =>
                {
                    if (dialog.Type == DialogType.Alert)
                    {
                        await dialog.DismissAsync();
                    }
                };
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
        /// Send keys to an alert
        /// </summary>
        /// <param name="text"></param>
        /// <returns>true if successful</returns>
        public static bool AlertInput(string text)
        {
            DebugOutput.OutputMethod($"Playwright - AlertInput", $"{text}");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            if (playwrightPage == null) return success;

            try
            {
                playwrightPage.OnDialog += async dialog =>
                {
                    await dialog.AcceptAsync(text);
                };
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
        /// Check if alert message is displayed
        /// </summary>
        /// <param name="alertMessage"></param>
        /// <returns>true if alert message matches</returns>
        public static bool AlertDisplayed(string alertMessage)
        {
            DebugOutput.OutputMethod($"Playwright - AlertDisplayed", $"{alertMessage}");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            if (playwrightPage == null) return success;

            try
            {
                string? dialogMessage = null;
                playwrightPage.OnDialog += dialog =>
                {
                    dialogMessage = dialog.Message;
                };

                if (dialogMessage != null && alertMessage.ToLower() == dialogMessage.ToLower())
                {
                    success = true;
                    return success;
                }

                if (dialogMessage != null && dialogMessage.ToLower().Contains(alertMessage.ToLower()))
                {
                    DebugOutput.Log($"message is found in message!");
                    success = true;
                    return success;
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to check alert message");
            }
            return success;
        }

        /// <summary>
        /// Dismiss an alert (cancel)
        /// </summary>
        /// <returns>true if alert dismissed</returns>
        public static bool AlertClickCancel()
        {
            DebugOutput.OutputMethod($"Playwright - AlertClickCancel");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            if (playwrightPage == null) return success;

            try
            {
                playwrightPage.OnDialog += async dialog =>
                {
                    await dialog.DismissAsync();
                };
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed to dismiss alert");
            }
            return success;
        }

        /// <summary>
        /// Accept an alert
        /// </summary>
        /// <returns>true if alert accepted</returns>
        public static bool AlertClickAccept()
        {
            DebugOutput.OutputMethod($"Playwright - AlertClickAccept");
            bool success = false;
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return success;
            if (playwrightPage == null) return success;

            try
            {
                playwrightPage.OnDialog += async dialog =>
                {
                    await dialog.AcceptAsync();
                };
                success = true;
                return success;
            }
            catch
            {
                DebugOutput.Log($"Failed to accept alert");
            }
            return success;
        }

        /// <summary>
        /// Click back button in browser
        /// </summary>
        /// <returns>true if successful</returns>
        public static bool ClickBackButtonInBrowser()
        {
            DebugOutput.OutputMethod($"Playwright - ClickBackButtonInBrowser");
            if (playwrightPage == null) return false;
            try
            {
                playwrightPage.GoBackAsync().Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to click on browser back button!");
                return false;
            }
        }

        /// <summary>
        /// Click on an element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if successful</returns>
        public static bool Click(string locator)
        {
            DebugOutput.OutputMethod("Playwright - Click", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.ClickAsync(locator).Wait();
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {locator} {ex}");
                return false;
            }
        }

        /// <summary>
        /// Click at specific coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if successful</returns>
        public static bool ClickCoordinates(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Playwright - ClickCoordinates", $"{x} {y}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.ClickAsync("body", new PageClickOptions { Position = new Point { X = x, Y = y } }).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to click at coordinates!");
                return false;
            }
        }

        /// <summary>
        /// Click with offset from element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if successful</returns>
        public static bool ClickCoordinatesWithElement(string locator, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Playwright - ClickCoordinatesWithElement", $"{locator} {x} {y}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.ClickAsync(locator, new PageClickOptions { Position = new Point { X = x, Y = y } }).Wait();
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {locator} {ex}");
                return false;
            }
        }

        /// <summary>
        /// Double click on an element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if successful</returns>
        public static bool DoubleClick(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - DoubleClick", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.DblClickAsync(locator).Wait();
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to double click {locator} {ex}");
                return false;
            }
        }

        /// <summary>
        /// Right click on an element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if successful</returns>
        public static bool RightClick(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - RightClick", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.ClickAsync(locator, new PageClickOptions { Button = MouseButton.Right }).Wait();
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to right click {locator} {ex}");
                return false;
            }
        }

        /// <summary>
        /// Get the Nth element matching a selector
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="nTh"></param>
        /// <returns>locator string or null</returns>
        public static string? GetNthElementBy(string selector, string nTh)
        {
            DebugOutput.OutputMethod($"Playwright - GetNthElementBy", $"{selector} {nTh}");
            if (playwrightPage == null) return null;

            try
            {
                var elements = playwrightPage.QuerySelectorAllAsync(selector).Result;
                if (elements == null || elements.Count == 0) return null;

                int index = nTh.ToLower() switch
                {
                    "1st" => 0,
                    "2nd" => 1,
                    "3rd" => 2,
                    _ => 0
                };

                if (index < elements.Count)
                {
                    return $"({selector}):nth-match(n+{index + 1})";
                }
                return null;
            }
            catch
            {
                DebugOutput.Log($"Failed to get {nTh} element");
                return null;
            }
        }

        /// <summary>
        /// Clear and enter text into an element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns>true if successful</returns>
        public static bool ClearThenEnterText(string locator, string text, string key = "")
        {
            DebugOutput.OutputMethod($"Playwright - ClearThenEnterText", $"{locator} {text} {key}");
            Click(locator);
            playwrightPage?.PressAsync(locator, "Control+A").Wait();
            playwrightPage?.PressAsync(locator, "Backspace").Wait();
            return EnterText(locator, text, key);
        }

        /// <summary>
        /// Enter text into an element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns>true if successful</returns>
        public static bool EnterText(string locator, string text, string key = "")
        {
            DebugOutput.OutputMethod($"Playwright - EnterText", $"{locator} {text} {key}");
            if (playwrightPage == null) return false;

            text = StringValues.TextReplacementService(text);
            DebugOutput.Log($"NEW TEXT = '{text}'");

            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    playwrightPage.FillAsync(locator, text).Wait();
                }

                if (!string.IsNullOrEmpty(key))
                {
                    return SendKey(locator, key);
                }

                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to enter text {locator} {text}");
                return false;
            }
        }

        /// <summary>
        /// Drag one element to another
        /// </summary>
        /// <param name="sourceLocator"></param>
        /// <param name="targetLocator"></param>
        /// <returns>true if successful</returns>
        public static bool DragElementToElement(string sourceLocator, string targetLocator)
        {
            DebugOutput.OutputMethod($"Playwright - DragElementToElement", $"{sourceLocator} {targetLocator}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.DragAndDropAsync(sourceLocator, targetLocator).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to drag!");
                return false;
            }
        }

        /// <summary>
        /// Check if element is displayed
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="not"></param>
        /// <param name="timeOut"></param>
        /// <returns>true if displayed</returns>
        public static bool IsDisplayed(string locator, bool not = false, int timeOut = 0)
        {
            DebugOutput.OutputMethod($"Playwright - IsDisplayed", $"{locator} {not} {timeOut}");
            if (playwrightPage == null) return false;

            timeOut = GetTimeout(timeOut);

            try
            {
                if (not)
                {
                    playwrightPage.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden, Timeout = timeOut * 1000 }).Wait();
                    return true;
                }
                else
                {
                    playwrightPage.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = timeOut * 1000 }).Wait();
                    return true;
                }
            }
            catch
            {
                DebugOutput.Log($"Element not in expected state");
                return false;
            }
        }

        /// <summary>
        /// Move to element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if successful</returns>
        public static bool MoveToElement(string locator, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Playwright - MoveToElement", $"{locator} {x} {y}");
            if (playwrightPage == null) return false;

            try
            {
                if (x == 0 && y == 0)
                {
                    playwrightPage.HoverAsync(locator).Wait();
                }
                else
                {
                    playwrightPage.HoverAsync(locator, new PageHoverOptions { Position = new Point { X = x, Y = y } }).Wait();
                }
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to move!");
                return false;
            }
        }

        /// <summary>
        /// Scroll to element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if successful</returns>
        public static bool ScrollToElement(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - ScrollToElement", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.EvaluateAsync($"() => document.querySelector('{locator}').scrollIntoView()").Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to scroll!");
                return false;
            }
        }

        /// <summary>
        /// Move slider element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="offset"></param>
        /// <returns>true if successful</returns>
        public static bool MoveSliderElement(string locator, int offset)
        {
            DebugOutput.OutputMethod($"Playwright - MoveSliderElement", $"{locator} {offset}");
            if (playwrightPage == null) return false;

            if (offset > 100)
            {
                DebugOutput.Log($"It only handles 100%");
                return false;
            }

            try
            {
                playwrightPage.ClickAsync(locator).Wait();
                var offsetPixels = (offset * 100) / 100;
                playwrightPage.PressAsync(locator, "ArrowRight", new PagePressOptions { Delay = 100 }).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to slide!");
                return false;
            }
        }

        /// <summary>
        /// Navigate to URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true if successful</returns>
        public static bool NavigateToURL(string url)
        {
            DebugOutput.OutputMethod($"Playwright - NavigateToURL", $"{url}");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.GotoAsync(url).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to navigate");
                return false;
            }
        }

        /// <summary>
        /// Navigate to URL with credentials
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="url"></param>
        /// <returns>true if successful</returns>
        public static bool NavigateToURLAsUserWithPassword(string userName, string password, string url)
        {
            DebugOutput.OutputMethod($"Playwright - NavigateToURLAsUserWithPassword", $"{userName} {password} {url}");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            if (playwrightPage == null) return false;

            try
            {
                var uri = new Uri(url);
                var urlWithCredentials = $"{uri.Scheme}://{userName}:{password}@{uri.Host}{uri.PathAndQuery}";
                playwrightPage.GotoAsync(urlWithCredentials).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed navigation");
                return false;
            }
        }

        /// <summary>
        /// Get screenshot of current page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true if successful</returns>
        public static bool? GetCurrentPageScreenShot(string id = "")
        {
            DebugOutput.OutputMethod($"Playwright - GetCurrentPageScreenShot", $"{id}");
            if (playwrightPage == null) return false;

            try
            {
                var fileName = string.IsNullOrEmpty(id) ? "TEMPEPOCH.png" : $"{id}EPOCH.png";
                var newFileName = StringValues.TextReplacementService(fileName);
                var fullFileName = FileUtils.GetImagesErrorDirectory() + "/" + newFileName;
                DebugOutput.Log($"SAVING IMAGE IN {fullFileName}");
                playwrightPage.ScreenshotAsync(new PageScreenshotOptions { Path = fullFileName }).Wait();
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to take page image: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Save current page screenshot
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns>true if successful</returns>
        public static bool SaveCurrentPageImage(string pageName)
        {
            DebugOutput.OutputMethod($"Playwright - SaveCurrentPageImage", $"{pageName}");
            if (playwrightPage == null) return false;

            if (pageName.Length > 30) pageName = pageName.Substring(0, 30);

            try
            {
                var fullFileName = currentDirectory + outputFolder + pageName + ".png";
                DebugOutput.Log($"putting file {fullFileName}");
                playwrightPage.ScreenshotAsync(new PageScreenshotOptions { Path = fullFileName }).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to take page image");
                return false;
            }
        }

        /// <summary>
        /// Screenshot page
        /// </summary>
        /// <returns>true if successful</returns>
        public static bool ScreenShotPage()
        {
            DebugOutput.OutputMethod($"Playwright - ScreenShotPage");
            if (playwrightPage == null) return false;

            try
            {
                var fullFileName = currentDirectory + outputFolder + "currentPage.png";
                DebugOutput.Log($"putting file {fullFileName}");
                playwrightPage.ScreenshotAsync(new PageScreenshotOptions { Path = fullFileName }).Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to take page image");
                return false;
            }
        }

        /// <summary>
        /// Check if screenshot of element already exists
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="pageName"></param>
        /// <returns>true if file exists</returns>
        public static bool ScreenShotElementAlreadyExists(string elementName, string pageName = "")
        {
            DebugOutput.OutputMethod($"Playwright - ScreenShotElementAlreadyExists", $"'{elementName}' '{pageName}'");
            FileUtils.SetCurrentDirectoryToTop();
            string fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + elementName + ".png";
            return FileUtils.OSFileCheck(fullFileName);
        }

        /// <summary>
        /// Screenshot element and compare
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="elementName"></param>
        /// <param name="pageName"></param>
        /// <param name="outputDir"></param>
        /// <returns>true if successful</returns>
        public static bool ScreenShotElementAndCompare(string locator, string elementName, string pageName = "", string outputDir = "")
        {
            DebugOutput.OutputMethod($"Playwright - ScreenShotElementAndCompare", $"{locator} '{elementName}' '{pageName}' '{outputDir}'");
            if (playwrightPage == null) return false;

            try
            {
                FileUtils.SetCurrentDirectoryToTop();
                DebugOutput.Log($"ScreenShotElementAndCompare Current directory = {currentDirectory}");
                var directory = FileUtils.GetImagePageDirectory(outputDir) + @"\" + pageName;
                var fullFileName = directory + "\\" + elementName + ".png";
                DebugOutput.Log($"getting file {fullFileName}");

                if (File.Exists(fullFileName))
                {
                    var baseline = new Bitmap(fullFileName);
                    if (baseline == null)
                    {
                        DebugOutput.Log($"Failed to read bitmap!");
                        return false;
                    }
                    DebugOutput.Log($"We have the baseline bitmap!");
                    return true;
                }
                return false;
            }
            catch
            {
                DebugOutput.Log($"Something went wrong!");
                return false;
            }
        }

        /// <summary>
        /// Screenshot element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="elementName"></param>
        /// <param name="pageName"></param>
        /// <returns>true if successful</returns>
        public static bool ScreenShotElement(string locator, string elementName, string pageName = "")
        {
            DebugOutput.OutputMethod($"Playwright - ScreenShotElement", $"{locator} '{elementName}' '{pageName}'");
            if (playwrightPage == null) return false;

            try
            {
                FileUtils.SetCurrentDirectoryToTop();
                DebugOutput.Log($"Current directory = {currentDirectory}");
                var fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + elementName + ".png";
                DebugOutput.Log($"putting file {fullFileName}");

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

                var handle = playwrightPage.QuerySelectorAsync(locator).Result;
                if (handle != null)
                {
                    handle.ScreenshotAsync(new ElementHandleScreenshotOptions { Path = fullFileName }).Wait();
                }

                DebugOutput.Log($"Saved file");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get width of element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>width in pixels</returns>
        public static int GetWidthOfElement(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetWidthOfElement", $"{locator}");
            if (playwrightPage == null) return 0;

            try
            {
                var box = playwrightPage.QuerySelectorAsync(locator).Result?.BoundingBoxAsync().Result;
                return (int)(box?.Width ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get height of element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>height in pixels</returns>
        public static int GetHeightOfElement(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetHeightOfElement", $"{locator}");
            if (playwrightPage == null) return 0;

            try
            {
                var box = playwrightPage.QuerySelectorAsync(locator).Result?.BoundingBoxAsync().Result;
                return (int)(box?.Height ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Check if list of element names contains text
        /// </summary>
        /// <param name="partialName"></param>
        /// <returns>true if found</returns>
        public static bool ListOfElementNamesContains(string partialName)
        {
            DebugOutput.OutputMethod($"Playwright - ListOfElementNamesContains", $"{partialName}");
            var elementNames = GetAllElementNamesAsList();
            foreach (string elementName in elementNames)
            {
                DebugOutput.Log($"NAME = {elementName}");
                if (elementName.Contains(partialName)) return true;
            }
            DebugOutput.Log($"Looked through {elementNames.Count} element names - did not find!");
            return false;
        }

        /// <summary>
        /// Get page title
        /// </summary>
        /// <returns>page title</returns>
        public static string? GetTabTitle()
        {
            DebugOutput.OutputMethod($"Playwright - GetTabTitle");
            if (playwrightPage == null) return null;
            return playwrightPage.TitleAsync().Result;
        }

        /// <summary>
        /// Get all element names as list
        /// </summary>
        /// <returns>list of element names</returns>
        private static List<string> GetAllElementNamesAsList()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllElementNamesAsList");
            var elements = GetAllElements();
            List<string> result = new List<string>();
            foreach (var locator in elements)
            {
                var name = GetElementAttributeValue(locator, "name");
                if (!string.IsNullOrEmpty(name)) result.Add(name);
            }
            return result;
        }

        /// <summary>
        /// Set window size
        /// </summary>
        /// <param name="size"></param>
        /// <returns>true if successful</returns>
        public static bool SetWindowSize(string size)
        {
            DebugOutput.OutputMethod($"Playwright - SetWindowSize", $"{size}");
            switch (size.ToLower())
            {
                case "large":
                    return SetWindowSize(4000, 4000);
                case "small":
                    return SetWindowSize(800, 600);
                default:
                {
                    var defaultScreenSize = TargetConfiguration.Configuration.ScreenSize;
                    var listOfSize = StringValues.BreakUpByDelimitedToList(defaultScreenSize, "x");
                    var defaultx = int.Parse(listOfSize[0]);
                    var defaulty = int.Parse(listOfSize[1]);
                    return SetWindowSize(defaultx, defaulty);
                }
            }
        }

        /// <summary>
        /// Set window size by dimensions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if successful</returns>
        public static bool SetWindowSize(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod($"Playwright - SetWindowSize", $"{x}x{y}");
            if (playwrightPage == null) return false;

            if (x == 0 || y == 0)
            {
                var size = TargetConfiguration.Configuration.ScreenSize;
                var listOfSize = StringValues.BreakUpByDelimitedToList(size, "x");
                try
                {
                    x = int.Parse(listOfSize[0]);
                    y = int.Parse(listOfSize[1]);
                }
                catch
                {
                    DebugOutput.Log($"Failed to convert to x and y");
                    return false;
                }
            }

            try
            {
                playwrightPage.ViewportSizeAsync = new ViewportSize { Width = x, Height = y };
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to set window size!");
                return false;
            }
        }

        /// <summary>
        /// Press key while clicking element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="key"></param>
        /// <returns>true if successful</returns>
        public static bool DownKeyAndClick(string locator, string key)
        {
            DebugOutput.OutputMethod($"Playwright - DownKeyAndClick", $"{locator} {key}");
            if (playwrightPage == null) return false;

            try
            {
                if (key.ToLower() == "ctrl")
                {
                    playwrightPage.ClickAsync(locator, new PageClickOptions { Modifiers = new[] { KeyboardModifier.Control } }).Wait();
                }
                else
                {
                    playwrightPage.ClickAsync(locator).Wait();
                }
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {locator} {ex}");
                return false;
            }
        }

        /// <summary>
        /// Send key to element
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="key"></param>
        /// <returns>true if successful</returns>
        public static bool SendKey(string locator, string key)
        {
            DebugOutput.OutputMethod($"Playwright - SendKey", $"{locator} {key}");
            if (string.IsNullOrEmpty(key)) return false;
            if (playwrightPage == null) return false;

            key = key.ToLower();
            try
            {
                var keyMap = new Dictionary<string, string>
                {
                    { "clear", "Control+A" },
                    { "close", "Alt+F4" },
                    { "down", "ArrowDown" },
                    { "down arrow", "ArrowDown" },
                    { "enter", "Enter" },
                    { "return", "Enter" },
                    { "escape", "Escape" },
                    { "page down", "PageDown" },
                    { "pagedown", "PageDown" },
                    { "page up", "PageUp" },
                    { "pageup", "PageUp" },
                    { "tab", "Tab" }
                };

                if (keyMap.TryGetValue(key, out var mappedKey))
                {
                    playwrightPage.PressAsync(locator, mappedKey).Wait();
                    if (key == "clear")
                    {
                        playwrightPage.PressAsync(locator, "Delete").Wait();
                    }
                }
                else
                {
                    playwrightPage.TypeAsync(locator, key).Wait();
                }
                return true;
            }
            catch
            {
                DebugOutput.Log($"problem sending key!");
                return false;
            }
        }

        /// <summary>
        /// Check if element is enabled
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if enabled</returns>
        public static bool IsEnabled(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - IsEnabled", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                var isEnabled = playwrightPage.IsEnabledAsync(locator).Result;
                if (isEnabled)
                {
                    var disabled = GetElementAttributeValue(locator, "disabled");
                    if (!string.IsNullOrEmpty(disabled))
                    {
                        DebugOutput.Log($"We have a disabled attribute! {disabled}");
                        return false;
                    }
                }
                return isEnabled;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Enabled flag! {ex}");
                return false;
            }
        }

        /// <summary>
        /// Check if element is selected (switch)
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if selected, false if not, null if unknown</returns>
        public static bool? IsSelectedSwitch(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - IsSelectedSwitch", $"{locator}");
            if (playwrightPage == null) return null;

            try
            {
                var ariaChecked = GetElementAttributeValue(locator, "aria-checked");
                if (!string.IsNullOrEmpty(ariaChecked))
                {
                    DebugOutput.Log($"We have an aria-checked attribute! {ariaChecked}");
                    if (ariaChecked.ToLower() == "true") return true;
                    if (ariaChecked.ToLower() == "false") return false;
                }

                var hasUnselectedClass = playwrightPage.QuerySelectorAsync($"{locator} .mdc-switch--unselected").Result != null;
                if (hasUnselectedClass)
                {
                    DebugOutput.Log($"Element is unselected!");
                    return false;
                }

                var hasSelectedClass = playwrightPage.QuerySelectorAsync($"{locator} .mdc-switch--selected").Result != null;
                if (hasSelectedClass)
                {
                    DebugOutput.Log($"Element is selected!");
                    return true;
                }

                DebugOutput.Log($"Unknown selection state");
                return null;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Selected flag! {ex}");
                return null;
            }
        }

        /// <summary>
        /// Check if element is selected
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if selected</returns>
        public static bool IsSelected(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - IsSelected", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                var isChecked = playwrightPage.IsCheckedAsync(locator).Result;
                if (isChecked) return true;

                var classAttr = GetElementAttributeValue(locator, "class");
                if (classAttr.ToLower().Contains("active")) return true;

                DebugOutput.Log($"Element is not selected!");
                return false;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Selected flag! {ex}");
                return false;
            }
        }

        /// <summary>
        /// Get element parent locator
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>parent locator or null</returns>
        public static string? GetElementParent(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementParent", $"{locator}");
            try
            {
                return locator + "/.."  ;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get CSS attribute value
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="attribute"></param>
        /// <returns>attribute value</returns>
        public static string? GetElementCSSAttribute(string locator, string attribute)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementCSSAttribute", $"{attribute}");
            if (playwrightPage == null) return null;

            try
            {
                return attribute.ToLower() switch
                {
                    "background color" or "background colour" => playwrightPage.EvaluateAsync<string>($"window.getComputedStyle(document.querySelector('{locator}')).backgroundColor").Result,
                    "color" or "colour" => playwrightPage.EvaluateAsync<string>($"window.getComputedStyle(document.querySelector('{locator}')).color").Result,
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get background color of element
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>background color or null</returns>
        public static string? GetBackGroundColourOfElement(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetBackGroundColourOfElement", $"{locator}");
            if (playwrightPage == null) return null;

            try
            {
                var bgColor = GetElementCSSAttribute(locator, "background colour");
                return bgColor;
            }
            catch
            {
                DebugOutput.Log($"Failed to get background color");
                return null;
            }
        }

        /// <summary>
        /// Get page width
        /// </summary>
        /// <returns>page width in pixels</returns>
        public static int GetPageWidth()
        {
            DebugOutput.OutputMethod($"Playwright - GetPageWidth");
            if (playwrightPage == null) return 0;
            return playwrightPage.ViewportSize?.Width ?? 0;
        }

        /// <summary>
        /// Get page height
        /// </summary>
        /// <returns>page height in pixels</returns>
        public static int GetPageHeight()
        {
            DebugOutput.OutputMethod($"Playwright - GetPageHeight");
            if (playwrightPage == null) return 0;
            return playwrightPage.ViewportSize?.Height ?? 0;
        }

        /// <summary>
        /// Get timeout with multiplier
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>adjusted timeout in seconds</returns>
        public static int GetTimeout(int timeout = 0)
        {
            DebugOutput.OutputMethod($"Playwright - GetTimeout", $"{timeout}");
            if (timeout == 0) timeout = TargetConfiguration.Configuration.PositiveTimeout;
            timeout *= TargetConfiguration.Configuration.TimeoutMultiplie;
            DebugOutput.Log($"Timeout set to {timeout}");
            return timeout;
        }

        /// <summary>
        /// Get all elements
        /// </summary>
        /// <returns>list of element locators</returns>
        public static List<string> GetAllElements(int timeout = 0)
        {
            DebugOutput.OutputMethod($"Playwright - GetAllElements", $"{timeout}");
            if (playwrightPage == null) return new List<string>();

            try
            {
                var elements = playwrightPage.QuerySelectorAllAsync("*").Result;
                var locators = new List<string>();
                for (int i = 0; i < elements.Count; i++)
                {
                    locators.Add($"nth-match(*,{i + 1})");
                }
                DebugOutput.Log($"WE have {locators.Count} elements IN TOTAL");
                return locators;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get all visible elements
        /// </summary>
        /// <returns>list of visible element locators</returns>
        public static List<string> GetAllVisibleElements()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllVisibleElements");
            if (playwrightPage == null) return new List<string>();

            try
            {
                var elements = playwrightPage.QuerySelectorAllAsync("*:visible").Result;
                var locators = new List<string>();
                for (int i = 0; i < elements.Count; i++)
                {
                    locators.Add($"nth-match(*:visible,{i + 1})");
                }
                return locators;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get all displayed elements
        /// </summary>
        /// <returns>list of displayed element locators</returns>
        public static List<string> GetAllDisplayedElements()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllDisplayedElements");
            return GetAllVisibleElements();
        }

        /// <summary>
        /// Get all header elements
        /// </summary>
        /// <returns>list of header element locators</returns>
        public static List<string> GetAllHeadersDisplayed()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllHeadersDisplayed");
            if (playwrightPage == null) return new List<string>();

            try
            {
                var headers = new List<string> { "h1", "h2", "h3", "h4", "h5", "h6" };
                var locators = new List<string>();
                foreach (var header in headers)
                {
                    var elements = playwrightPage.QuerySelectorAllAsync(header).Result;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        locators.Add($"{header}:nth-of-type({i + 1})");
                    }
                }
                return locators;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get all image elements
        /// </summary>
        /// <returns>list of image element locators</returns>
        public static List<string> GetAllImageElementsDisplayed()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllImageElementsDisplayed");
            if (playwrightPage == null) return new List<string>();

            try
            {
                var elements = playwrightPage.QuerySelectorAllAsync("img").Result;
                var locators = new List<string>();
                for (int i = 0; i < elements.Count; i++)
                {
                    locators.Add($"img:nth-of-type({i + 1})");
                }
                return locators;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get current page URL
        /// </summary>
        /// <returns>page URL</returns>
        public static string? GetCurrentWebPageURL()
        {
            DebugOutput.OutputMethod($"Playwright - GetCurrentWebPageURL");
            if (playwrightPage == null) return null;
            return playwrightPage.Url;
        }

        /// <summary>
        /// Get element tag name
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>tag name</returns>
        public static string GetElementTagName(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementTagName", $"{locator}");
            if (playwrightPage == null) return "";

            try
            {
                return playwrightPage.EvaluateAsync<string>($"document.querySelector('{locator}').tagName").Result ?? "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Get element text (one line)
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>element text</returns>
        public static string? GetElementTextDirectOneLine(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementTextDirectOneLine", $"{locator}");
            if (playwrightPage == null) return null;

            try
            {
                var text = playwrightPage.TextContentAsync(locator).Result;
                if (!string.IsNullOrEmpty(text))
                {
                    return Regex.Replace(text, @"\s+", " ").Trim();
                }
                return text;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get element text direct
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>element text</returns>
        public static string? GetElementtextDirect(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementtextDirect", $"{locator}");
            if (playwrightPage == null) return null;

            try
            {
                var text = playwrightPage.TextContentAsync(locator).Result;
                if (!string.IsNullOrEmpty(text))
                {
                    DebugOutput.Log($"WE GOT A VALUE FROM .TextContent = '{text}'");
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
        /// Get element text
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>element text</returns>
        public static string GetElementText(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementText", $"{locator}");
            if (playwrightPage == null) return failedFindElement;

            try
            {
                var text = playwrightPage.TextContentAsync(locator).Result;
                if (!string.IsNullOrEmpty(text))
                {
                    DebugOutput.Log($"We have THE TEXT  '{text}' from the element {locator}");
                    return text;
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to get text in direct query!");
            }

            // Try attribute values
            var textAttr = GetElementAttributeValue(locator, "text");
            if (!string.IsNullOrEmpty(textAttr)) return textAttr;

            var valueAttr = GetElementAttributeValue(locator, "value");
            if (!string.IsNullOrEmpty(valueAttr)) return valueAttr;

            var titleAttr = GetElementAttributeValue(locator, "data-original-title");
            if (!string.IsNullOrEmpty(titleAttr)) return titleAttr;

            DebugOutput.Log($"FAILED ALL TEXT KNOWN TO MAN, i.e. me!");
            return "";
        }

        /// <summary>
        /// Get element attribute value
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="attribute"></param>
        /// <returns>attribute value</returns>
        public static string GetElementAttributeValue(string locator, string attribute)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementAttributeValue", $"{locator} {attribute}");
            if (string.IsNullOrEmpty(attribute)) return "";
            if (playwrightPage == null) return "";

            try
            {
                if (attribute.ToLower() == "tagname" || attribute.ToLower() == "tag")
                {
                    return GetElementTagName(locator);
                }

                var value = playwrightPage.GetAttributeAsync(locator, attribute).Result;
                if (value == null) return "";

                DebugOutput.Log($"We read the attribute {attribute} and got '{value}'");
                return value;
            }
            catch
            {
                DebugOutput.Log($"Failed to read attribute {attribute}");
                return "";
            }
        }

        /// <summary>
        /// Get element multiple attributes
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="attributes"></param>
        /// <returns>list of attribute values</returns>
        public static List<string> GetElementMultipleAttributesValues(string locator, List<string> attributes)
        {
            DebugOutput.OutputMethod($"Playwright - GetElementMultipleAttributesValues", $"{locator} {attributes.Count}");
            var attributeValueList = new List<string>();

            foreach (var attribute in attributes)
            {
                var value = GetElementAttributeValue(locator, attribute);
                attributeValueList.Add(string.IsNullOrEmpty(value) ? "NULL" : value);
            }

            return attributeValueList;
        }

        /// <summary>
        /// Swap tab by number
        /// </summary>
        /// <param name="tabNumber"></param>
        /// <returns>true if successful</returns>
        public static bool SwapTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod($"Playwright - SwapTabByNumber", $"{tabNumber}");
            if (playwrightBrowser == null) return false;

            try
            {
                var pages = playwrightContext?.Pages;
                if (pages != null && tabNumber > 0 && tabNumber <= pages.Count)
                {
                    playwrightPage = pages[tabNumber - 1];
                    return true;
                }
                return false;
            }
            catch
            {
                DebugOutput.Log($"Failed to switch to tab {tabNumber}");
                return false;
            }
        }

        /// <summary>
        /// Close tab by number
        /// </summary>
        /// <param name="tabNumber"></param>
        /// <returns>true if successful</returns>
        public static bool CloseTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod($"Playwright - CloseTabByNumber", $"{tabNumber}");
            if (playwrightBrowser == null) return false;

            try
            {
                if (!SwapTabByNumber(tabNumber)) return false;
                playwrightPage?.CloseAsync().Wait();
                var numberOfTabs = GetNumberOfTabsOpenInBrowser();
                if (numberOfTabs < 1) return false;
                return SwapTabByNumber(1);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get number of tabs open
        /// </summary>
        /// <returns>number of tabs</returns>
        public static int GetNumberOfTabsOpenInBrowser()
        {
            DebugOutput.OutputMethod($"Playwright - GetNumberOfTabsOpenInBrowser");
            if (playwrightContext == null) return 0;
            return playwrightContext.Pages.Count;
        }

        /// <summary>
        /// Get mouse down (element hold)
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true if successful</returns>
        public static bool MouseDown(string locator)
        {
            DebugOutput.OutputMethod($"Playwright - MouseDown", $"{locator}");
            if (playwrightPage == null) return false;

            try
            {
                playwrightPage.MouseAsync.DownAsync().Wait();
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to click and hold / mouse down!");
                return false;
            }
        }

        /// <summary>
        /// Get all input elements by text
        /// </summary>
        /// <param name="text"></param>
        /// <returns>locator or null</returns>
        public static string? GetAllInputElementsByText(string text)
        {
            DebugOutput.OutputMethod($"Playwright - GetAllInputElementsByText", $"{text}");
            if (playwrightPage == null) return null;

            try
            {
                var locator = $"input[value='{text}']";
                var element = playwrightPage.QuerySelectorAsync(locator).Result;
                if (element != null)
                {
                    DebugOutput.Log($"FOUND AN INPUT ELEMENT USING THE TEXT {text}");
                    return locator;
                }
                return null;
            }
            catch
            {
                DebugOutput.Log($"Failed to find any element with that text!");
                return null;
            }
        }

        /// <summary>
        /// Get input element by parent text
        /// </summary>
        /// <param name="text"></param>
        /// <returns>locator or null</returns>
        public static string? GetInputElementByParentText(string text)
        {
            DebugOutput.OutputMethod($"Playwright - GetInputElementByParentText", $"{text}");
            if (playwrightPage == null) return null;

            try
            {
                var locator = $"input:has-text('{text}')";
                var element = playwrightPage.QuerySelectorAsync(locator).Result;
                if (element != null)
                {
                    DebugOutput.Log($"FOUND AN INPUT ELEMENT WITH PARENT TEXT {text}");
                    return locator;
                }
                return null;
            }
            catch
            {
                DebugOutput.Log($"Failed to find any element with that text!");
                return null;
            }
        }

        /// <summary>
        /// Get all childless elements
        /// </summary>
        /// <returns>list of locators</returns>
        public static List<string> GetAllChildlessElements()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllChildlessElements");
            if (playwrightPage == null) return new List<string>();

            try
            {
                var elements = playwrightPage.QuerySelectorAllAsync("*:not(:has(*))").Result;
                var locators = new List<string>();
                for (int i = 0; i < elements.Count; i++)
                {
                    locators.Add($"nth-match(*:not(:has(*)),{i + 1})");
                }
                return locators;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get all childless elements with text
        /// </summary>
        /// <returns>list of locators</returns>
        public static List<string> GetAllChildlessElementsWithText()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllChildlessElementsWithText");
            var returnLocators = new List<string>();
            var childlessElements = GetAllChildlessElements();

            foreach (var locator in childlessElements)
            {
                var text = GetElementtextDirect(locator);
                if (!string.IsNullOrEmpty(text) && text.Trim().Length > 0)
                {
                    DebugOutput.Log($"TEXT IS '{text}'");
                    returnLocators.Add(locator);
                }
            }
            return returnLocators;
        }

        /// <summary>
        /// Get all hidden elements
        /// </summary>
        /// <returns>list of locators</returns>
        public static List<string> GetAllCurrentlyLoadedButHiddenElements()
        {
            DebugOutput.OutputMethod($"Playwright - GetAllCurrentlyLoadedButHiddenElements");
            if (playwrightPage == null) return new List<string>();

            try
            {
                var elements = playwrightPage.QuerySelectorAllAsync("*:hidden").Result;
                var locators = new List<string>();
                for (int i = 0; i < elements.Count; i++)
                {
                    locators.Add($"nth-match(*:hidden,{i + 1})");
                }
                return locators;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}