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
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Core
{
    public static class PlaywrightUtils
    {
        public static IPlaywright? playwrightDriver = null;
        public static IBrowser? browser = null;
        public static IBrowserContext? browserContext = null;
        public static IPage? page = null;
        private static IDialog? lastDialog = null;
        private static bool dialogHandlerAttached = false;

        public static string outputFolder = @"\AppSpecFlow\TestOutput\";
        public static string compareFolder = @"\AppSpecFlow\TestCompare\";
        public static string failedFindElement = "Failed to find element!";
        public static int test = TargetConfiguration.Configuration.Debug;
        public static int DefaultPositiveTimeOut = TargetConfiguration.Configuration.PositiveTimeout;
        public static string currentDirectory = Directory.GetCurrentDirectory();

        public static void SetPage(IPage? currentPage)
        {
            page = currentPage;
            AttachDialogHandlerIfNeeded();
        }

        private static void AttachDialogHandlerIfNeeded()
        {
            if (page == null || dialogHandlerAttached) return;
            page.Dialog += (_, dialog) =>
            {
                lastDialog = dialog;
            };
            dialogHandlerAttached = true;
        }

        private static T? RunSync<T>(Func<Task<T>> action, T? fallback = default)
        {
            try
            {
                return action().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Playwright exception: {ex.Message}");
                return fallback;
            }
        }

        private static bool RunSync(Func<Task> action)
        {
            try
            {
                action().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Playwright exception: {ex.Message}");
                return false;
            }
        }

        private static string NormalizeSelector(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector)) return selector;
            if (selector.StartsWith("xpath=", StringComparison.OrdinalIgnoreCase)) return selector;
            if (selector.StartsWith("css=", StringComparison.OrdinalIgnoreCase)) return selector;
            if (selector.StartsWith("//") || selector.StartsWith("(//")) return $"xpath={selector}";
            return selector;
        }

        /// <summary>
        /// Get the Alert dialog displayed on a web site
        /// </summary>
        /// <returns>IDialog if found, null if not found</returns>
        private static IDialog? GetAlert()
        {
            DebugOutput.OutputMethod("Playwright - GetAlert");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return null;
            if (page == null) return null;
            AttachDialogHandlerIfNeeded();
            Thread.Sleep(500);
            return lastDialog;
        }

        public static string GetParentXPathLocator()
        {
            DebugOutput.OutputMethod("Playwright - GetParentXPathLocator");
            return "xpath=..";
        }

        public static bool AlertLogin(string username, string password)
        {
            DebugOutput.OutputMethod("Playwright - AlertLogin", $"{username} {password}");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            var alertWin = GetAlert();
            if (alertWin == null)
            {
                DebugOutput.Log("FAILED TO FIND ALERT WINDOW!");
                return false;
            }
            try
            {
                return RunSync(() => alertWin.AcceptAsync(username + "\t" + password));
            }
            catch
            {
                DebugOutput.Log($"Failure entering username and password {username}");
                return false;
            }
        }

        public static bool AlertInput(string text)
        {
            DebugOutput.OutputMethod("Playwright - AlertInput", text);
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            var alertWin = GetAlert();
            if (alertWin == null) return false;
            return RunSync(() => alertWin.AcceptAsync(text));
        }

        public static bool AlertDisplayed(string alertMessage)
        {
            DebugOutput.OutputMethod("Playwright - AlertDisplayed", alertMessage);
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            var alertWin = GetAlert();
            if (alertWin == null) return false;
            var message = alertWin.Message;
            if (message.Equals(alertMessage, StringComparison.OrdinalIgnoreCase)) return true;
            DebugOutput.Log($"We have an alert, but Not the message, we got '{message}' was expecting '{alertMessage}'");
            if (message.Contains(alertMessage, StringComparison.OrdinalIgnoreCase))
            {
                DebugOutput.Log("message is found in message!");
                return true;
            }
            return false;
        }

        public static bool AlertClickCancel()
        {
            DebugOutput.OutputMethod("Playwright - AlertClickCancel");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            var alertWin = GetAlert();
            if (alertWin == null) return false;
            return RunSync(() => alertWin.DismissAsync());
        }

        public static bool AlertClickAccept()
        {
            DebugOutput.OutputMethod("Playwright - AlertClickAccept");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            var alertWin = GetAlert();
            if (alertWin == null) return false;
            return RunSync(() => alertWin.AcceptAsync());
        }

        public static bool ClickBackButtonInBrowser()
        {
            DebugOutput.OutputMethod("Playwright - ClickBackButtonInBrowser");
            if (page == null) return false;
            return RunSync(async () => { await page.GoBackAsync(); });
        }

        public static bool Click(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - Click", $"{element}");
            return RunSync(() => element.ClickAsync());
        }

        public static bool ClickCoordinates(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod("Playwright - ClickCoordinates", $"{x} {y}");
            if (page == null) return false;
            return RunSync(() => page.Mouse.ClickAsync(x, y));
        }

        public static bool ClickCoordinatesWithElement(ILocator element, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod("Playwright - ClickCoordinatesWithElement", $"{element} {x} {y}");
            if (page == null) return false;
            var box = RunSync(() => element.BoundingBoxAsync());
            if (box == null) return false;
            var clickX = box.X + (box.Width / 2) + x;
            var clickY = box.Y + (box.Height / 2) + y;
            return RunSync(() => page.Mouse.ClickAsync(clickX, clickY));
        }

        public static bool DoubleClick(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - DoubleClick", $"{element}");
            return RunSync(() => element.DblClickAsync());
        }

        public static bool RightClick(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - RightClick", $"{element}");
            return RunSync(() => element.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right }));
        }

        public static ILocator? GetNthElementBy(string selector, string nTh)
        {
            DebugOutput.OutputMethod("Playwright - ClickNthElementBy", $"{selector} {nTh}");
            var elements = GetElements(selector);
            if (elements == null || elements.Count == 0) return null;
            DebugOutput.Log($"We have {elements.Count} elements of locator {selector}");
            switch (nTh.ToLower())
            {
                default:
                case "1st":
                    return elements.Count > 0 ? elements[0] : null;
                case "2nd":
                    return elements.Count > 1 ? elements[1] : null;
                case "3rd":
                    return elements.Count > 2 ? elements[2] : null;
            }
        }

        public static bool ClearThenEnterText(ILocator element, string text, string key = "")
        {
            DebugOutput.OutputMethod("Playwright - ClearThenEnterText", $"{element} {text} {key}");
            Click(element);
            SendKey(element, "clear");
            return EnterText(element, text, key);
        }

        public static bool EnterText(ILocator element, string text, string key = "")
        {
            DebugOutput.OutputMethod("Playwright - EnterText", $"{element} {text} {key}");
            text = StringValues.TextReplacementService(text);
            DebugOutput.Log($"NEW TEXT = '{text}'");
            if (!string.IsNullOrEmpty(text))
            {
                var success = RunSync(() => element.FillAsync(text));
                if (!success)
                {
                    DebugOutput.Log("Failed to Enter Text Directly, trying input inside the element");
                    var input = element.Locator("input").First;
                    if (RunSync(() => input.FillAsync(text)))
                    {
                        element = input;
                    }
                    else
                    {
                        DebugOutput.Log("Failed to Enter Text into input box");
                        return false;
                    }
                }
            }
            if (string.IsNullOrEmpty(key)) return true;
            DebugOutput.Log($"Now to send the key {key}");
            if (SendKey(element, key)) return true;
            DebugOutput.Log($"Failed to send key {key}");
            var input2 = element.Locator("input").First;
            if (SendKey(input2, key)) return true;
            DebugOutput.Log($"Failed to send key {key} AGAIN");
            return false;
        }

        public static bool DragElementToElement(ILocator elementA, ILocator elementB)
        {
            DebugOutput.OutputMethod("Playwright - DragElementToElement", $"{elementA} {elementB}");
            return RunSync(() => elementA.DragToAsync(elementB));
        }

        public static bool IsDisplayed(ILocator element, bool not = false, int timeOut = 0)
        {
            DebugOutput.OutputMethod("Playwright - IsDisplayed", $"{element} {not} {timeOut}");
            if (element == null) return false;
            if (not)
            {
                if (timeOut == 0) timeOut = TargetConfiguration.Configuration.NegativeTimeout;
                var timeoutMs = timeOut * 1000;
                try
                {
                    element.WaitForAsync(new LocatorWaitForOptions
                    {
                        Timeout = timeoutMs,
                        State = WaitForSelectorState.Hidden
                    }).GetAwaiter().GetResult();
                    return true;
                }
                catch
                {
                    DebugOutput.Log($"TIMED OUT waiting for element to be NOT displayed after {timeOut} seconds!");
                    return true;
                }
            }

            if (timeOut == 0) timeOut = TargetConfiguration.Configuration.PositiveTimeout;
            var positiveTimeoutMs = timeOut * 1000;
            try
            {
                element.WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = positiveTimeoutMs,
                    State = WaitForSelectorState.Visible
                }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                DebugOutput.Log($"TIMED OUT waiting for element to be displayed after {timeOut} seconds!");
                return false;
            }
        }

        public static bool MouseDown(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - MouseDown", $"{element}");
            if (page == null) return false;
            var box = RunSync(() => element.BoundingBoxAsync());
            if (box == null) return false;
            return RunSync(async () =>
            {
                await page.Mouse.MoveAsync(box.X + box.Width / 2, box.Y + box.Height / 2);
                await page.Mouse.DownAsync();
            });
        }

        public static bool MoveToElement(ILocator element, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod("Playwright - MoveToElement", $"{element} {x} {y}");
            if (element == null) return false;
            if (page == null) return false;
            var box = RunSync(() => element.BoundingBoxAsync());
            if (box == null) return false;
            var targetX = box.X + (box.Width / 2) + x;
            var targetY = box.Y + (box.Height / 2) + y;
            var moveResult = RunSync(() => page.Mouse.MoveAsync(targetX, targetY));
            if (!moveResult) return false;
            if (y != 0)
            {
                y = y + 2;
                if (!ScrollDownPage(y)) return false;
            }
            return true;
        }

        private static bool ScrollDownPage(int y = 0)
        {
            DebugOutput.OutputMethod("Playwright - ScrollDownPage", $"{y}");
            if (page == null) return false;
            return RunSync(async () => { await page.EvaluateAsync("y => window.scrollBy(0, y)", y); });
        }

        public static bool ScrollToElement(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - ScrollToElement", $"{element}");
            if (element == null) return false;
            return RunSync(() => element.ScrollIntoViewIfNeededAsync());
        }

        public static bool MoveSliderElement(ILocator slider, int offSet)
        {
            DebugOutput.OutputMethod("Playwright - MoveSliderElement", $"{slider} {offSet}");
            if (offSet > 100) return false;
            if (page == null) return false;
            var box = RunSync(() => slider.BoundingBoxAsync());
            if (box == null) return false;
            var onePercent = box.Width / 100f;
            var zeroPoint = box.Width - (onePercent * 50);
            var intZeroPoint = (int)Math.Round(zeroPoint);
            var startX = box.X + intZeroPoint;
            var startY = box.Y + (box.Height / 2);
            if (!RunSync(() => page.Mouse.ClickAsync(startX, startY))) return false;
            Thread.Sleep(100);
            var movement = (int)Math.Round(onePercent * offSet);
            var moveX = box.X + movement;
            var moveY = startY;
            if (!RunSync(() => page.Mouse.ClickAsync(moveX, moveY))) return false;
            Thread.Sleep(100);
            return true;
        }

        public static bool NavigateToURL(string url)
        {
            DebugOutput.OutputMethod("Playwright - NavigateToURL", url);
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType != "web") return false;
            if (page == null) return false;
            return RunSync(async () => { await page.GotoAsync(url); });
        }

        public static bool NavigateToURLAsUserWithPassword(string userName, string password, string url)
        {
            DebugOutput.OutputMethod("Playwright - NavigateToURLAsUserWithPassword", $"{userName} {password} {url}");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return false;
            url = url.ToLower();
            var webAddress = url.Replace("http://", "").Replace("https://", "");
            var combinedUrl = "http://" + userName + ":" + password + "@" + webAddress;
            if (page == null) return false;
            return RunSync(async () => { await page.GotoAsync(combinedUrl); });
        }

        public static Bitmap? GetImageOfElementOnPage(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetImageOfElementOnPage", $"{element}");
            return GetElementScreenShot(element);
        }

        public static bool? GetCurrentPageScreenShot(string id = "")
        {
            DebugOutput.OutputMethod("Playwright - GetCurrentPageScreenShot", id);
            if (page == null) return false;
            try
            {
                var epoch = EPOCHControl.Epoch;
                var fileName = string.IsNullOrEmpty(id) ? "TEMPEPOCH.png" : $"{id}EPOCH.png";
                var newFileName = StringValues.TextReplacementService(fileName);
                var fullFileName = FileUtils.GetImagesErrorDirectory() + "/" + newFileName;
                return RunSync(async () => { await page.ScreenshotAsync(new PageScreenshotOptions { Path = fullFileName, FullPage = true }); });
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to take page image: {ex.Message}");
                return false;
            }
        }

        public static bool SaveCurrentPageImage(string pageName)
        {
            DebugOutput.OutputMethod("Playwright - GetCurrentPageImage", pageName);
            if (pageName.Length > 30) pageName = pageName.Substring(0, 30);
            if (page == null) return false;
            var fullFileName = currentDirectory + outputFolder + pageName + ".png";
            return RunSync(async () => { await page.ScreenshotAsync(new PageScreenshotOptions { Path = fullFileName, FullPage = true }); });
        }

        public static bool ScreenShotPage()
        {
            DebugOutput.OutputMethod("Playwright - ScreenShotPage");
            if (page == null) return false;
            var fullFileName = currentDirectory + outputFolder + "currentPage.png";
            return RunSync(async () => { await page.ScreenshotAsync(new PageScreenshotOptions { Path = fullFileName, FullPage = true }); });
        }

        public static bool ScreenShotElementAlreadyExists(string elementName, string pageName = "")
        {
            DebugOutput.OutputMethod("Playwright - ScreenShotElementAlreadyExists", $"'{elementName}' '{pageName}'");
            FileUtils.SetCurrentDirectoryToTop();
            string fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + elementName + ".png";
            return FileUtils.OSFileCheck(fullFileName);
        }

        public static bool ScreenShotElementAndCompare(ILocator element, string elementName, string pageName = "", string outputDir = "")
        {
            DebugOutput.OutputMethod("Playwright - ScreenShotElementAndCompare", $"{element} '{elementName}' '{pageName}' '{outputDir}'");
            try
            {
                if (page == null && TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
                {
                    DebugOutput.Log("We do not have a page!");
                    return false;
                }
                var directory = FileUtils.GetImagePageDirectory(outputDir) + @"\" + pageName;
                var fullFileName = directory + "\\" + elementName + ".png";
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        var bitmap = new Bitmap(@fullFileName);
                        if (bitmap == null)
                        {
                            DebugOutput.Log("Failed to read bitmap!");
                            return false;
                        }
                        var img = GetElementScreenShot(element);
                        if (img == null)
                        {
                            DebugOutput.Log("Can not img screen shot, if no screen shot!");
                            return false;
                        }
                    }
                    catch
                    {
                        DebugOutput.Log("Something went wrong with the bitmap file!");
                        return false;
                    }
                }
            }
            catch
            {
                DebugOutput.Log("Something over arching went wrong!");
                return false;
            }
            return true;
        }

        public static bool ScreenShotElement(ILocator element, string elementName, string pageName = "")
        {
            DebugOutput.OutputMethod("Playwright - ScreenShotElement", $" {element} '{elementName}' '{pageName}' ");
            try
            {
                if (page == null && TargetConfiguration.Configuration.ApplicationType.ToLower() == "web")
                {
                    DebugOutput.Log("We do not have a page!");
                    return false;
                }
                var img = GetElementScreenShot(element);
                if (img == null)
                {
                    DebugOutput.Log("Can not save screen shot, if no screen shot!");
                    return false;
                }
                FileUtils.SetCurrentDirectoryToTop();
                var fullFileName = FileUtils.GetImagePageDirectory(pageName) + "\\" + elementName + ".png";
                if (File.Exists(fullFileName))
                {
                    try
                    {
                        File.Delete(fullFileName);
                    }
                    catch
                    {
                        DebugOutput.Log("failed to delete - will hopefully overwrite");
                    }
                }
                if (OperatingSystem.IsWindows()) img.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Png);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int GetWidthOfElement(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetWidthOfElement", $" {element} ");
            var box = RunSync(() => element.BoundingBoxAsync());
            return box == null ? 0 : (int)Math.Round(box.Width);
        }

        public static int GetHeightOfElement(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetHeightOfElement", $" {element} ");
            var box = RunSync(() => element.BoundingBoxAsync());
            return box == null ? 0 : (int)Math.Round(box.Height);
        }

        public static bool ListOfElementNamesContains(string partualName)
        {
            DebugOutput.OutputMethod("Playwright - ListOfElementNamesContains", $" {partualName} ");
            List<string> elementNames = GetAllEmenetsNamesAsList();
            foreach (string elementName in elementNames)
            {
                if (elementName.Contains(partualName)) return true;
            }
            return false;
        }

        public static string? GetTabTitle()
        {
            DebugOutput.OutputMethod("Playwright - GetTabTitle");
            if (page == null) return null;
            return RunSync(() => page.TitleAsync());
        }

        private static List<string> GetAllEmenetsNamesAsList()
        {
            DebugOutput.OutputMethod("Playwright - GetAllEmenetsNamesAsList");
            var elements = GetAllElements();
            List<string> result = new List<string>();
            foreach (var element in elements)
            {
                var name = GetElementAttributeValue(element, "Name");
                if (!string.IsNullOrEmpty(name)) result.Add(name);
            }
            return result;
        }

        public static Bitmap? GetElementScreenShot(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementScreenShot", $" {element} ");
            try
            {
                var bytes = RunSync(() => element.ScreenshotAsync());
                if (bytes == null) return null;
                if (!OperatingSystem.IsWindows()) return null;
                using var ms = new MemoryStream(bytes);
                using var bmp = new Bitmap(ms);
                return new Bitmap(bmp);
            }
            catch
            {
                DebugOutput.Log("Failed to capture image!");
                return null;
            }
        }

        public static bool SetWindowSize(string size)
        {
            DebugOutput.OutputMethod("Playwright - SetWindowSize", $" {size} ");
            switch (size.ToLower())
            {
                default:
                    var defaultScreenSize = TargetConfiguration.Configuration.ScreenSize;
                    var listOfSize = StringValues.BreakUpByDelimitedToList(defaultScreenSize, "x");
                    var defaultx = int.Parse(listOfSize[0]);
                    var defaulty = int.Parse(listOfSize[1]);
                    return SetWindowSize(defaultx, defaulty);
                case "large":
                    return SetWindowSize(4000, 4000);
                case "small":
                    return SetWindowSize(800, 600);
            }
        }

        public static bool SetWindowSize(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod("Playwright - SetWindowSize", $" {x}x{y} ");
            if (x == 0 || y == 0)
            {
                var size = TargetConfiguration.Configuration.ScreenSize;
                var listOfSize = StringValues.BreakUpByDelimitedToList(size, "x");
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
            if (appType != "web") return false;
            if (page == null) return false;
            return RunSync(() => page.SetViewportSizeAsync(x, y));
        }

        public static bool DownKeyAndClick(ILocator element, string key)
        {
            DebugOutput.OutputMethod("Playwright - DownKeyAndClick", $" {element} {key}");
            if (page == null) return false;
            try
            {
                if (key.Equals("ctrl", StringComparison.OrdinalIgnoreCase))
                {
                    RunSync(() => page.Keyboard.DownAsync("Control"));
                }
                var clicked = RunSync(() => element.ClickAsync());
                if (key.Equals("ctrl", StringComparison.OrdinalIgnoreCase))
                {
                    RunSync(() => page.Keyboard.UpAsync("Control"));
                }
                return clicked;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to click {element} {ex}");
                return false;
            }
        }

        public static bool SendKey(ILocator element, string key)
        {
            DebugOutput.OutputMethod("Playwright - SendKey", $" {element} {key} ");
            if (string.IsNullOrEmpty(key)) return false;
            key = key.ToLower();
            try
            {
                switch (key)
                {
                    default:
                        return RunSync(() => element.PressSequentiallyAsync(key));
                    case "clear":
                        return RunSync(async () =>
                        {
                            await element.PressAsync("Control+A");
                            await element.PressAsync("Delete");
                        });
                    case "close":
                        return RunSync(() => element.PressAsync("Alt+F4"));
                    case "down":
                    case "down arrow":
                        return RunSync(() => element.PressAsync("ArrowDown"));
                    case "enter":
                    case "return":
                        return RunSync(() => element.PressAsync("Enter"));
                    case "escape":
                        return RunSync(() => element.PressAsync("Escape"));
                    case "page down":
                    case "pagedown":
                        return RunSync(() => element.PressAsync("PageDown"));
                    case "page up":
                    case "pageup":
                        return RunSync(() => element.PressAsync("PageUp"));
                    case "tab":
                        return RunSync(() => element.PressAsync("Tab"));
                }
            }
            catch
            {
                DebugOutput.Log("problem sending key!");
                return false;
            }
        }

        public static bool IsEnabled(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - IsEnabled", $" {element}");
            try
            {
                var enabled = RunSync(() => element.IsEnabledAsync());
                if (enabled)
                {
                    var attribute = GetElementAttributeValue(element, "disabled");
                    if (!string.IsNullOrEmpty(attribute)) return false;
                }
                return enabled;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Enabled flag! {ex}");
                return false;
            }
        }

        public static bool? IsSelectedSwitch(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - IsSelectedSwitch", $" {element}");
            try
            {
                var ariaChecked = GetElementAttributeValue(element, "aria-checked");
                if (!string.IsNullOrEmpty(ariaChecked))
                {
                    if (ariaChecked.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
                    if (ariaChecked.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
                }
                var buttonElement = element.Locator(".mdc-switch--unselected");
                if (RunSync(() => buttonElement.CountAsync()) > 0) return false;
                buttonElement = element.Locator(".mdc-switch--selected");
                if (RunSync(() => buttonElement.CountAsync()) > 0) return true;
                return null;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Selected flag! {ex}");
                return null;
            }
        }

        public static bool IsSelected(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - IsSelected", $" {element}");
            try
            {
                var isChecked = RunSync(() => element.IsCheckedAsync(), false);
                if (isChecked) return true;
                var classTitle = GetElementAttributeValue(element, "class");
                if (!string.IsNullOrEmpty(classTitle) && classTitle.ToLower().Contains("active")) return true;
                var cssClass = GetElementCSSAttribute(element, "class");
                if (!string.IsNullOrEmpty(cssClass) && cssClass.ToLower().Contains("active")) return true;
                return ChildIsSelected(element);
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had a failed returning elements Selected flag! {ex}");
                return false;
            }
        }

        private static bool ChildIsSelected(ILocator parentElement)
        {
            DebugOutput.OutputMethod("Playwright - ChildIsSelected", $"{parentElement}");
            try
            {
                var children = GetAllElementsUnderElement(parentElement);
                foreach (var child in children)
                {
                    var isChecked = RunSync(() => child.IsCheckedAsync(), false);
                    if (isChecked) return true;
                    var classTitle = GetElementAttributeValue(child, "class");
                    if (!string.IsNullOrEmpty(classTitle) && classTitle.ToLower().Contains("active")) return true;
                }
                return false;
            }
            catch
            {
                DebugOutput.Log("We have hit an issue when checking on Child Elements if they are selected!");
            }
            return false;
        }

        public static ILocator? GetElementParent(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementParent", $" {element}");
            try
            {
                return element.Locator("xpath=..");
            }
            catch
            {
                return null;
            }
        }

        public static ILocator? GetElement(string selector, int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetElement", $" {selector} {timeout}");
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType != "web") return null;
            if (page == null) return null;
            selector = NormalizeSelector(selector);
            timeout = GetTimeout(timeout);
            var locator = page.Locator(selector).First;
            try
            {
                locator.WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = timeout * 1000,
                    State = WaitForSelectorState.Attached
                }).GetAwaiter().GetResult();
                return locator;
            }
            catch
            {
                DebugOutput.Log("FAILED GET ELEMENT");
                return null;
            }
        }

        public static string? GetElementCSSAttribute(ILocator element, string attribute)
        {
            DebugOutput.OutputMethod("Playwright - GetElementsCSSAttribute", $" {attribute}");
            switch (attribute.ToLower())
            {
                default: return null;
                case "background color":
                case "background colour":
                    return RunSync(() => element.EvaluateAsync<string>("el => getComputedStyle(el).getPropertyValue('background-color')"));
                case "color":
                case "colour":
                    return RunSync(() => element.EvaluateAsync<string>("el => getComputedStyle(el).getPropertyValue('color')"));
                case "class":
                    return RunSync(() => element.EvaluateAsync<string>("el => el.className"));
            }
        }

        public static string? GetBackGroundColourOfElement(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetBackGroundColourOfElement", $"{element}");
            var elementColour = GetElementCSSAttribute(element, "colour");
            if (elementColour == null) return null;
            var elementBackGroundColour = GetElementCSSAttribute(element, "background colour");
            if (elementBackGroundColour != null)
            {
                if (elementColour != elementBackGroundColour)
                {
                    var elementBackGroundColourRGBA = ImageWorkings.GetRGBADetailsFromString(elementBackGroundColour);
                    if (elementBackGroundColourRGBA.Alpha > 0) return elementBackGroundColour;
                }
            }
            while (true)
            {
                var parentElement = GetElementParent(element);
                if (parentElement == null) return null;
                var parentColour = GetElementCSSAttribute(parentElement, "colour");
                if (elementColour != parentColour && parentColour != null)
                {
                    var parentElementColourRGBA = ImageWorkings.GetRGBADetailsFromString(parentColour);
                    if (parentElementColourRGBA.Alpha != 0) return parentColour;
                }
                var parentBackGroundColour = GetElementCSSAttribute(parentElement, "background colour");
                if (parentBackGroundColour != null && elementColour != parentBackGroundColour)
                {
                    var parentBackGroundColourRGBA = ImageWorkings.GetRGBADetailsFromString(parentBackGroundColour);
                    if (parentBackGroundColourRGBA.Alpha > 0) return parentBackGroundColour;
                }
                element = parentElement;
            }
        }

        public static string? GetElementIDAsString(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementIDAsString", $" {element}");
            return element.ToString();
        }

        public static int GetPageWidth()
        {
            DebugOutput.OutputMethod("Playwright - GetPageWidth");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return 0;
            if (page == null) return 0;
            var viewport = page.ViewportSize;
            if (viewport != null) return viewport.Width;
            var width = RunSync(() => page.EvaluateAsync<int>("() => window.innerWidth"), 0);
            return width;
        }

        public static int GetPageHeight()
        {
            DebugOutput.OutputMethod("Playwright - GetPageHeight");
            if (TargetConfiguration.Configuration.ApplicationType.ToLower() != "web") return 0;
            if (page == null) return 0;
            var viewport = page.ViewportSize;
            if (viewport != null) return viewport.Height;
            var height = RunSync(() => page.EvaluateAsync<int>("() => window.innerHeight"), 0);
            return height;
        }

        public static int GetTimeout(int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetTimeout", $"{timeout}");
            if (timeout == 0) timeout = TargetConfiguration.Configuration.PositiveTimeout;
            timeout *= TargetConfiguration.Configuration.TimeoutMultiplie;
            DebugOutput.Log($"Timeout set to {timeout}");
            return timeout;
        }

        public static ILocator? GetTopElement()
        {
            DebugOutput.OutputMethod("Playwright - GetTopElement");
            if (page == null) return null;
            var locator = page.Locator("xpath=//*").First;
            return locator;
        }

        public static List<ILocator> GetAllElements(int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetAllElements", $" {timeout} ");
            if (page == null) return new List<ILocator>();
            var selector = "xpath=//*";
            var elements = RunSync(() => page.Locator(selector).AllAsync(), new List<ILocator>());
            return elements?.ToList() ?? new List<ILocator>();
        }

        public static List<ILocator> GetAllVisibleElements()
        {
            DebugOutput.OutputMethod("Playwright - GetAllVisibleElements");
            var allElements = GetAllElements();
            var visibleElements = new List<ILocator>();
            foreach (var element in allElements)
            {
                if (RunSync(() => element.IsVisibleAsync(), false)) visibleElements.Add(element);
            }
            return visibleElements;
        }

        public static List<ILocator> GetAllChildlessElements()
        {
            DebugOutput.OutputMethod("Playwright - GetAllChildlessElements");
            var returnElements = new List<ILocator>();
            var allElements = GetAllVisibleElements();
            foreach (var element in allElements)
            {
                var count = RunSync(() => element.Locator("*").CountAsync(), 0);
                if (count == 0) returnElements.Add(element);
            }
            return returnElements;
        }

        public static List<ILocator> GetAllChildlessElementsWithText()
        {
            DebugOutput.OutputMethod("Playwright - GetAllChildlessElementsWithText");
            var returnElements = new List<ILocator>();
            var allChildElements = GetAllChildlessElements();
            foreach (var element in allChildElements)
            {
                var text = GetElementtextDirect(element);
                if (!string.IsNullOrEmpty(text)) returnElements.Add(element);
            }
            return returnElements;
        }

        public static List<ILocator> GetAllDisplayedElements()
        {
            DebugOutput.OutputMethod("Playwright - GetAllDisplayedElements");
            return GetAllVisibleElements();
        }

        public static List<ILocator> GetAllCurrentlyLoadedButHiddenElements()
        {
            DebugOutput.OutputMethod("Playwright - GetAllCurrentlyLoadedButHiddenElements");
            var returnElements = new List<ILocator>();
            var allElements = GetAllElements();
            foreach (var element in allElements)
            {
                if (!RunSync(() => element.IsVisibleAsync(), true)) returnElements.Add(element);
            }
            return returnElements;
        }

        public static List<ILocator> GetAllHeadersDisplayed()
        {
            DebugOutput.OutputMethod("Playwright - GetAllHeadersDisplayed");
            var returnElements = new List<ILocator>();
            if (page == null) return returnElements;
            var selector = "h1,h2,h3,h4,h5,h6,h7,h8,h9,h10";
            var elements = RunSync(() => page.Locator(selector).AllAsync(), new List<ILocator>()) ?? new List<ILocator>();
            foreach (var element in elements)
            {
                if (RunSync(() => element.IsVisibleAsync(), false)) returnElements.Add(element);
            }
            return returnElements;
        }

        public static List<ILocator> GetAllImageElementsDisplayed()
        {
            DebugOutput.OutputMethod("Playwright - GetAllImageElementsDisplayed");
            var returnElements = new List<ILocator>();
            if (page == null) return returnElements;
            var elements = RunSync(() => page.Locator("img").AllAsync(), new List<ILocator>()) ?? new List<ILocator>();
            foreach (var element in elements)
            {
                if (RunSync(() => element.IsVisibleAsync(), false)) returnElements.Add(element);
            }
            return returnElements;
        }

        public static ILocator? GetAllInputElementsByText(string text)
        {
            DebugOutput.OutputMethod("Playwright - GetAllInputElementsByText", $" {text} ");
            if (page == null) return null;
            var allInputs = RunSync(() => page.Locator("input").AllAsync(), new List<ILocator>()) ?? new List<ILocator>();
            foreach (var element in allInputs)
            {
                var elementText = GetElementText(element);
                if (elementText.Equals(text, StringComparison.OrdinalIgnoreCase)) return element;
            }
            return null;
        }

        public static ILocator? GetInputElementByParentText(string text)
        {
            DebugOutput.OutputMethod("Playwright - GetInputElementByParentText", $" {text} ");
            if (page == null) return null;
            var allInputs = RunSync(() => page.Locator("input").AllAsync(), new List<ILocator>()) ?? new List<ILocator>();
            foreach (var element in allInputs)
            {
                var parent = GetElementParent(element);
                if (parent != null)
                {
                    var elementText = GetElementText(parent);
                    if (elementText.Equals(text, StringComparison.OrdinalIgnoreCase)) return element;
                }
            }
            return null;
        }

        public static List<ILocator> GetAllElementsUnderElement(ILocator parent)
        {
            DebugOutput.OutputMethod("Playwright - GetAllElementsUnderElement", $" {parent} ");
            var elements = RunSync(() => parent.Locator("*").AllAsync(), new List<ILocator>());
            return elements?.ToList() ?? new List<ILocator>();
        }

        public static List<ILocator>? GetElements(string selector, int timeout = 0)
        {
            DebugOutput.OutputMethod($"Playwright - GetElements {selector}", $" {timeout} ");
            if (page == null) return new List<ILocator>();
            selector = NormalizeSelector(selector);
            timeout = GetTimeout(timeout);
            try
            {
                var locator = page.Locator(selector);
                locator.WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = timeout * 1000,
                    State = WaitForSelectorState.Attached
                }).GetAwaiter().GetResult();
                var elements = RunSync(() => locator.AllAsync(), new List<ILocator>());
                return elements?.ToList() ?? new List<ILocator>();
            }
            catch
            {
                DebugOutput.Log("FAILED GET ELEMENTS");
                return new List<ILocator>();
            }
        }

        public static ILocator? GetElementUnderElement(ILocator parentElement, string selector, int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetElementUnderElement", $" {parentElement} {selector} {timeout} ");
            selector = NormalizeSelector(selector);
            timeout = GetTimeout(timeout);
            try
            {
                var locator = parentElement.Locator(selector).First;
                locator.WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = timeout * 1000,
                    State = WaitForSelectorState.Attached
                }).GetAwaiter().GetResult();
                return locator;
            }
            catch
            {
                DebugOutput.Log("failed to find element");
                return null;
            }
        }

        public static string? GetCurrentWebPageURL()
        {
            DebugOutput.OutputMethod("Playwright - GetCurrentWebPageURL");
            if (page == null) return null;
            return page.Url;
        }

        public static List<ILocator> GetWebElementsUnder(ILocator parent, string selector, int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetWebElementsUnder", $" {parent} {selector} {timeout} ");
            selector = NormalizeSelector(selector);
            var elements = RunSync(() => parent.Locator(selector).AllAsync(), new List<ILocator>());
            return elements?.ToList() ?? new List<ILocator>();
        }

        public static List<ILocator> GetWindowsElementsUnder(ILocator parent, string selector, int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetWindowsElementsUnder", $" {parent} {selector} {timeout} ");
            return new List<ILocator>();
        }

        public static List<ILocator> GetElementsUnder(ILocator parent, string selector, int timeout = 0)
        {
            DebugOutput.OutputMethod("Playwright - GetElementsUnder", $" {parent} {selector} {timeout} ");
            var appType = TargetConfiguration.Configuration.ApplicationType.ToLower();
            if (appType == "web") return GetWebElementsUnder(parent, selector, timeout);
            if (appType == "windows") return GetWindowsElementsUnder(parent, selector, timeout);
            return new List<ILocator>();
        }

        public static string GetElementTagName(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementTagName", $" {element} ");
            return RunSync(() => element.EvaluateAsync<string>("el => el.tagName"), string.Empty) ?? string.Empty;
        }

        public static string? GetElementTextDirectOneLine(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementtextDirect", $" {element} ");
            var htmlContent = RunSync(() => element.EvaluateAsync<string>("el => el.outerHTML"), "");
            if (htmlContent == null) return null;
            string cleanedText = Regex.Replace(htmlContent, "<sup>.*?</sup>", string.Empty, RegexOptions.Singleline);
            string textWithChildrenRemoved = Regex.Replace(cleanedText, "<[^>]*>", "");
            string newCleanedText = HttpUtility.HtmlDecode(textWithChildrenRemoved);
            return newCleanedText;
        }

        public static string? GetElementtextDirect(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementtextDirect", $" {element} ");
            try
            {
                var text = RunSync(() => element.InnerTextAsync());
                if (!string.IsNullOrEmpty(text)) return text;
            }
            catch
            {
                DebugOutput.Log("Failed to read direct");
            }
            return null;
        }

        public static string GetElementText(ILocator element)
        {
            DebugOutput.OutputMethod("Playwright - GetElementText", $" {element} ");
            if (element == null) return failedFindElement;
            var direct = GetElementtextDirect(element);
            if (!string.IsNullOrEmpty(direct)) return direct;
            var attributeText = GetElementAttributeValue(element, "text");
            if (!string.IsNullOrEmpty(attributeText)) return attributeText;
            var attributeValue = GetElementAttributeValue(element, "value");
            if (!string.IsNullOrEmpty(attributeValue)) return attributeValue;
            var attributeTextContent = GetElementAttributeValue(element, "textContent");
            if (!string.IsNullOrEmpty(attributeTextContent)) return attributeTextContent;
            var attributeOriginalTitle = GetElementAttributeValue(element, "data-original-title");
            if (!string.IsNullOrEmpty(attributeOriginalTitle)) return attributeOriginalTitle;
            return "";
        }

        private static string GetCorrectAttributeName(string attributeName)
        {
            DebugOutput.OutputMethod("Playwright - GetCorrectAttributeName", $" {attributeName} ");
            if (attributeName.ToLower() == "tag")
            {
                return "TagName";
            }
            if (attributeName.ToLower() == "class" || attributeName.ToLower() == "classname" || attributeName.ToLower() == "tagclass")
            {
                return "class";
            }
            if (attributeName.ToLower() == "id")
            {
                return "id";
            }
            return attributeName;
        }

        public static List<string> GetElementMultipleAttributesValues(ILocator element, List<string> attributes)
        {
            DebugOutput.OutputMethod("Playwright - GetElementMultipleAttributesValues", $" {element} {attributes.Count} ");
            var attributeValueList = new List<string>();
            foreach (var attribute in attributes)
            {
                var modifiedAttributeName = GetCorrectAttributeName(attribute);
                var value = "";
                if (modifiedAttributeName == "TagName")
                {
                    value = GetElementTagName(element);
                }
                else
                {
                    value = GetElementAttributeValue(element, modifiedAttributeName);
                }
                attributeValueList.Add(value.Length < 1 ? "NULL" : value);
            }
            return attributeValueList;
        }

        public static string? GetFullXPathOfElement(ILocator? element)
        {
            DebugOutput.OutputMethod("Playwright - GetFullXPathOfElement", $" {element} ");
            if (element == null) return null;
            const string script = "(el) => {\n" +
                                  "  if (!el) return '';\n" +
                                  "  if (el.id) return '//' + el.tagName.toLowerCase() + '[@id=\"' + el.id + '\"]';\n" +
                                  "  const parts = [];\n" +
                                  "  while (el && el.nodeType === Node.ELEMENT_NODE) {\n" +
                                  "    let index = 1;\n" +
                                  "    let sibling = el.previousSibling;\n" +
                                  "    while (sibling) {\n" +
                                  "      if (sibling.nodeType === Node.ELEMENT_NODE && sibling.tagName === el.tagName) index++;\n" +
                                  "      sibling = sibling.previousSibling;\n" +
                                  "    }\n" +
                                  "    parts.unshift(el.tagName.toLowerCase() + '[' + index + ']');\n" +
                                  "    if (el.parentElement && el.parentElement.id) {\n" +
                                  "      parts.unshift(el.parentElement.tagName.toLowerCase() + '[@id=\"' + el.parentElement.id + '\"]');\n" +
                                  "      return '//' + parts.join('/');\n" +
                                  "    }\n" +
                                  "    el = el.parentElement;\n" +
                                  "  }\n" +
                                  "  return '/' + parts.join('/');\n" +
                                  "}";
            return RunSync(() => element.EvaluateAsync<string>(script));
        }

        public static bool SwapTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod("Playwright - SwapTabByNumber", $" {tabNumber} ");
            if (browserContext == null) return false;
            if (tabNumber <= 0 || tabNumber > browserContext.Pages.Count) return false;
            var newPage = browserContext.Pages[tabNumber - 1];
            SetPage(newPage);
            return RunSync(() => newPage.BringToFrontAsync());
        }

        public static bool CloseTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod("Playwright - CloseTabByNumber", $" {tabNumber} ");
            if (browserContext == null) return false;
            if (!SwapTabByNumber(tabNumber)) return false;
            if (page == null) return false;
            RunSync(() => page.CloseAsync());
            return SwapTabByNumber(1);
        }

        public static int GetNumberOfTabsOpenInBrowser()
        {
            DebugOutput.OutputMethod("Playwright - GetNumberOfTabsOpenInBrowser");
            return browserContext?.Pages.Count ?? 0;
        }

        public static string GetElementAttributeValue(ILocator element, string attribute)
        {
            DebugOutput.OutputMethod("Playwright - GetElementAttributeValue", $" {element} {attribute} ");
            string blank = "";
            if (string.IsNullOrEmpty(attribute)) return blank;
            if (attribute.Equals("elementid", StringComparison.OrdinalIgnoreCase))
            {
                return element.ToString() ?? blank;
            }
            if (attribute.Equals("locatorname", StringComparison.OrdinalIgnoreCase))
            {
                attribute = "Name";
            }
            if (attribute.Equals("tagname", StringComparison.OrdinalIgnoreCase) || attribute.Equals("tag", StringComparison.OrdinalIgnoreCase))
            {
                var value = GetElementTagName(element);
                return string.IsNullOrEmpty(value) ? blank : value;
            }
            if (attribute.Equals("tagclass", StringComparison.OrdinalIgnoreCase))
            {
                attribute = "class";
            }
            if (attribute.Equals("fullxpath", StringComparison.OrdinalIgnoreCase))
            {
                return GetFullXPathOfElement(element) ?? "";
            }
            try
            {
                if (attribute.Equals("textcontent", StringComparison.OrdinalIgnoreCase))
                {
                    return RunSync(() => element.EvaluateAsync<string>("el => el.textContent"), blank) ?? blank;
                }
                if (attribute.Equals("text", StringComparison.OrdinalIgnoreCase))
                {
                    return RunSync(() => element.InnerTextAsync(), blank) ?? blank;
                }
                if (attribute.Equals("value", StringComparison.OrdinalIgnoreCase))
                {
                    return RunSync(() => element.EvaluateAsync<string>("el => el.value"), blank) ?? blank;
                }
                var attributeValue = RunSync(() => element.GetAttributeAsync(attribute));
                return attributeValue ?? blank;
            }
            catch
            {
                DebugOutput.Log($"Failed to read attribute {attribute}");
                return blank;
            }
        }
    }
}