using System;
using System.Diagnostics;
using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Centralized Selenium interaction layer.
    /// THIS IS THE BLOCK OF SELENIUM - Never pass selenium out of here!
    /// Keep direct WebDriver/IWebElement usage inside this class so the rest
    /// of the framework remains Selenium-agnostic.
    /// </summary>
    public static class ElementInteraction
    {
        // ------------------ Alert & dialog helpers ------------------
        // Group: methods that interact with browser alerts and dialogs

        /// <summary>
        /// Check whether an alert with the specified message is displayed.
        /// </summary>
        /// <param name="alertMessage">Expected alert message text.</param>
        /// <returns>True if alert is displayed, otherwise false.</returns>
        public static bool AlertIsDisplayed(string alertMessage)
        {
            // Delegates to SeleniumUtil to check for alert presence.
            return SeleniumUtil.AlertDisplayed(alertMessage);
        }

        /// <summary>
        /// Accept the currently displayed alert.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        public static bool AlertClickAccept()
        {
            // Delegates to Selenium util to accept the alert.
            return SeleniumUtil.AlertClickAccept();
        }

        /// <summary>
        /// Dismiss or cancel the currently displayed alert.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        public static bool AlertClickCancel()
        {
            // Delegates to Selenium util to cancel the alert.
            return SeleniumUtil.AlertClickCancel();
        }

        /// <summary>
        /// Send keys/text into an active alert prompt.
        /// </summary>
        /// <param name="text">Text to input into the alert prompt.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool AlertSendKeys(string text)
        {
            // Delegates text input into browser alert.
            return SeleniumUtil.AlertInput(text);
        }

        // ------------------ Click / Interaction helpers ------------------
        // Group: methods that perform clicks, selections, dragging, navigation

        /// <summary>
        /// Clear text from a named element on the current page.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True if cleared, false otherwise.</returns>
        public static bool ClearTextFromElement(FormBase currentPage, string elementName, string elementType)
        {
            // Log and call helper that clears text by sending the clear key action.
            DebugOutput.OutputMethod("ClearTextFromElement", $"{currentPage.Name}, {elementName}, {elementType}");
            return EnterTextAndKeyIntoElement(currentPage, elementName, elementType, "", "clear");
        }

        /// <summary>
        /// Clear then enter text into the element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="text">Text to enter.</param>
        /// <param name="key">Optional key to send after entering text.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClearTextThenEnterTextToElement(FormBase currentPage, string elementName, string elementType, string text, string key = "")
        {
            // Clear first and then enter text using existing helper.
            DebugOutput.OutputMethod("ClearTextThenEnterTextToElement", $"{currentPage.Name}, {elementName}, {elementType}");
            if (!ClearTextFromElement(currentPage, elementName, elementType)) return Failure($"Failed to clear element");
            return EnterTextAndKeyIntoElement(currentPage, elementName, elementType, text, key);
        }

        /// <summary>
        /// Click the browser's back button.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickBackButtonInBrowser()
        {
            // Navigate back via Selenium util.
            DebugOutput.OutputMethod("ClickBackButtonInBrowser", "");
            return SeleniumUtil.ClickBackButtonInBrowser();
        }

        /// <summary>
        /// Click coordinates in the current viewport.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnCooClickCoordinates(int x = 0, int y = 0)
        {
            // Direct coordinate click via selenium util.
            DebugOutput.OutputMethod("ClickOnCooClickCoordinates", $" {x} {y}");
            return SeleniumUtil.ClickCoordinates(x, y);
        }

        /// <summary>
        /// Click the specified element (by name/type) and optionally offset inside it.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="x">X offset.</param>
        /// <param name="y">Y offset.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnElement(FormBase currentPage, string elementName, string elementType, int x = 0, int y = 0)
        {
            // Resolve the element and click by coordinates with element reference.
            DebugOutput.OutputMethod("ClickOnElement", $"{currentPage.Name}, {elementName}, {elementType} {x} {y}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            return SeleniumUtil.ClickCoordinatesWithElement(element, x, y);
        }

        /// <summary>
        /// Click a checkbox sub-element by finding the label inside the parent.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnElement_CheckBoxSub(FormBase currentPage, string elementName, string elementType)
        {
            // Locate closest parent and label to click the checkbox representation.
            DebugOutput.OutputMethod("ClickOnElementCheckBoxSub", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            var parentElement = GetTheElement.GetParent(element);
            if (parentElement == null) return Failure($"Failed, it has no parent?");
            By labelElementLocator = GetSubLocators.CheckBoxLabelElementLocator;
            var labelElement = SeleniumUtil.GetElementUnderElement(parentElement, labelElementLocator, 1);
            if (labelElement == null) return Failure($"Even the parents, sub, {labelElementLocator} nope, doesn't exist!");
            return SeleniumUtil.Click(labelElement);
        }

        /// <summary>
        /// Click on element then enter text and send a key (e.g., Enter).
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="text">Text to enter.</param>
        /// <param name="key">Key to send.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnElementEnterTextSendKey(FormBase currentPage, string elementName, string elementType, string text, string key)
        {
            // Click element then enter text and send key via Selenium util.
            DebugOutput.OutputMethod("ClickOnElementCheckBoxSub", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            if (!SeleniumUtil.Click(element)) return Failure($"Failed to click on the top element!");
            if (!SeleniumUtil.EnterText(element, text)) return Failure($"Clicked - but can not enter text");
            if (!SeleniumUtil.SendKey(element, "enter")) return Failure($"Clicked, sent text - but its the send key I fail at!");
            return true;
        }

        /// <summary>
        /// Click an element inside a group contained by a parent element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="groupName">Name of the group to click.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnGroupElement(FormBase currentPage, string elementName, string elementType, string groupName)
        {
            // Find group element and click it.
            DebugOutput.OutputMethod("ClickOnGroupElement", $"{currentPage.Name}, {elementName}, {elementType} {groupName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var groupElement = GetTheElement.GetGroup(element, groupName);
            if (groupElement == null) return Failure($"Failed to get the group element under element!");
            return SeleniumUtil.Click(groupElement);
        }

        /// <summary>
        /// Interact with a date picker to select year, month and day.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Date picker element name key.</param>
        /// <param name="elementType">Date picker element type string.</param>
        /// <param name="date">Date to select.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnSelectYearPickerOfDatePicker(FormBase currentPage, string elementName, string elementType, DateTime date)
        {
            // Open the year picker within the date picker and select year/month/day.
            DebugOutput.OutputMethod("ClickOnSubElementOfDatePicker", $"{currentPage.Name}, {elementName}, {elementType}, {date} ");
            var datePicker = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (datePicker == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it XZ!");
            // open the year picker
            DebugOutput.Log($"Now for the YEAR");
            var openYearPicker = By.XPath("(//button[@aria-label='Choose month and year'])[1]");
            var yearPickerElement = SeleniumUtil.GetElementUnderElement(datePicker, openYearPicker, 1);
            if (yearPickerElement == null) return Failure($"Failed to find the button to open the year picker!");
            if (!SeleniumUtil.Click(yearPickerElement)) return Failure($"Failed to click on the button to open the year picker!");
            // now click on the year
            DebugOutput.Log($"Now to pick the YEAR {date.Year}");
            var yearToClick = By.XPath($"//button[@aria-label='{date.Year}']");
            var yearElement = SeleniumUtil.GetElementUnderElement(datePicker, yearToClick, 1);
            // find the first button to see what years are displayed
            if (yearElement == null)
            {
                DebugOutput.Log("STILL TO DO!");
                return false;
            }
            if (!SeleniumUtil.Click(yearElement)) return Failure($"Failed to click on the year {date.Year}");
            DebugOutput.Log($"Now for the MONTH");
            var monthName = DateValues.Get3MonthWordFromMonthNumber(date);
            if (monthName == null) return Failure($"Failed to get the month name from the number {date.Month}");
            DebugOutput.Log($"Month name is {monthName}");
            var monthToClick = By.XPath($"//button[span[normalize-space(text())='{monthName.ToUpper()}']]");
            var monthElement = SeleniumUtil.GetElementUnderElement(datePicker, monthToClick, 1);
            if (monthElement == null) return Failure($"Failed to find the month element {monthName}");
            if (!SeleniumUtil.Click(monthElement)) return Failure($"Failed to click on the month {monthName}");
            DebugOutput.Log($"Now for the DAY");
            var dayLocator = By.XPath($"//button[span[normalize-space(text())='{date.Day}']]");
            var dayElement = SeleniumUtil.GetElementUnderElement(datePicker, dayLocator, 1);
            if (dayElement == null) return Failure($"Failed to find the day element {date.Day}");
            if (!SeleniumUtil.Click(dayElement)) return Failure($"Failed to click on the day {date.Day}");
            return true;
        }

        /// <summary>
        /// Click a chip-like sub-element by matching text and clicking its close element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="text">Text to match in chip.</param>
        /// <param name="timeout">Optional timeout seconds.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnSubElementByTagSubElementByClassByTextByTag(FormBase currentPage, string elementName, string elementType, string text, int timeout = 0)
        {
            // Iterate chips, find matching text, click the close 'x' element.
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            // all chips in the chip array
            var subElements = SeleniumUtil.GetElementsUnder(element, GetSubLocators.ChipLocator);
            if (subElements == null) return Failure($"No sub elements under the element found by {GetSubLocators.ChipLocator}");
            foreach (var subSubElement in subElements)
            {
                var textElement = SeleniumUtil.GetElementUnderElement(subSubElement, GetSubLocators.ChipTextLocator);
                if (textElement != null)
                {
                    var elementText = SeleniumUtil.GetElementText(textElement);
                    if (elementText.Contains(text))
                    {
                        var closeElement = SeleniumUtil.GetElementUnderElement(subSubElement, GetSubLocators.ChipCloseLocator);
                        if (closeElement == null) return Failure($"We get the right chip, but no close x");
                        return SeleniumUtil.Click(closeElement);
                    }
                }
            }
            return Failure($"Failed to find it - not really surprising was it?!");
        }

        /// <summary>
        /// Click the nth sub-sub element under a given element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="subElementType">Sub-element type string.</param>
        /// <param name="number">1-based index of sub-sub element.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnSubSubElementByNumberUnderElement(FormBase currentPage, string elementName, string elementType, string subElementType, int number)
        {
            // Resolve list and click by index (1-based input).
            DebugOutput.OutputMethod("ClickOnSubElementByTextUnderElement", $"{currentPage.Name}, {elementName}, {elementType}, {subElementType}, {number}");
            var subSubElement = GetTheSubSubElementsOfElement(currentPage, elementName, elementType, subElementType);
            if (subSubElement == null) return Failure($"Failed to get sub sub element!");
            if (number > subSubElement.Count) return Failure($"Can not click on the {number} sub sub element, when there is only {subSubElement.Count}");
            number--;
            return SeleniumUtil.Click(subSubElement[number]);
        }

        /// <summary>
        /// Click a sub-element under an element by matching visible text.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="subElementText">Text to find in sub-element.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnSubElementByTextUnderElement(FormBase currentPage, string elementName, string elementType, string subElementText)
        {
            // Attempt to click the element with the text; fallback to parent click.
            DebugOutput.OutputMethod("ClickOnSubElementByTextUnderElement", $"{currentPage.Name}, {elementName}, {elementType}, {subElementText}");

            By locator = By.XPath($".//*[text()='{subElementText}']");
            var subElement = GetTheElement.GetSubElementByText(currentPage, elementName, elementType, locator, subElementText, 0);
            if (subElement == null) return Failure($"Failed to find the sub element!");
            if (SeleniumUtil.Click(subElement)) return true;
            DebugOutput.Log($"failed to click but it is there.. maybe the parent is the clickable part as we found the text!");
            var parentOfSub = SeleniumUtil.GetElementParent(subElement);
            if (parentOfSub == null) return Failure($"Not really sure HOW there is no parent... ?");
            if (SeleniumUtil.Click(parentOfSub)) return true;
            return Failure($"Even the parent isn't clickable!");
        }

        /// <summary>
        /// Click the first sub-element of a specific sub-type under an element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="subElementText">Sub-element name or descriptor.</param>
        /// <param name="subElementType">Sub-element type to find.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnSubElementByTypeUnderElement(FormBase currentPage, string elementName, string elementType, string subElementText, string subElementType)
        {
            // Find sub-elements by subtype and click the first one.
            DebugOutput.OutputMethod("ClickOnSubElementByTypeUnderElement", $"{currentPage.Name}, {elementName}, {elementType}, {subElementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            var elements = GetSubElementsOfElement(element, "", subElementType);
            if (elements == null) return Failure($"No sub elements found!");
            return SeleniumUtil.Click(elements[0]);
        }

        /// <summary>
        /// Click an element by searching for a tag under the parent element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="tag">Tag name to search for.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnTagElementUnderElement(FormBase currentPage, string elementName, string elementType, string tag)
        {
            // Try tag name and fallbacks like mat-expansion-panel-header, else click parent.
            DebugOutput.OutputMethod("ClickOnButtonInElement", $"{currentPage.Name}, {elementName}, {elementType}, {tag}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            var buttonLocator = By.TagName(tag);
            var buttonElement = SeleniumUtil.GetElementUnderElement(element, buttonLocator, 1);
            if (buttonElement == null)
            {
                DebugOutput.Log($"Failed to find a button by TAG {tag} under Element {elementName} now try finding an element using mat-expansion-panel-header");
                buttonLocator = By.TagName("mat-expansion-panel-header");
                buttonElement = SeleniumUtil.GetElementUnderElement(element, buttonLocator, 1);
                if (buttonElement == null)
                {
                    DebugOutput.Log($"Still failing - so I'm just going to click on element!");
                    return SeleniumUtil.Click(element);
                }
            }
            return SeleniumUtil.Click(buttonElement);
        }

        /// <summary>
        /// Click an item within elements of a given tag that contains the specified text.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="tag">Tag name to search within.</param>
        /// <param name="item">Text to search for.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickOnTextInTagInElement(FormBase currentPage, string elementName, string elementType, string tag, string item)
        {
            // Iterate elements with tag and click the one containing the text.
            DebugOutput.OutputMethod("ClickOnTextInTagInElement", $"{currentPage.Name}, {elementName}, {elementType}, {tag}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            var itemElements = GetTheElements.ByTag(element, tag);
            if (itemElements == null) return Failure($"Failed to find the sub elements by tag {tag}!");
            DebugOutput.Log($"There are {itemElements.Count} elements currently under the accordion");
            foreach (var itemElement in itemElements)
            {
                if (SeleniumUtil.IsDisplayed(itemElement))
                {
                    if (SeleniumUtil.GetElementText(itemElement).Contains(item))
                    {
                        return SeleniumUtil.Click(itemElement);
                    }
                }
            }
            return Failure($"Never found an element by tag {tag} containing text {item} under {elementName}");
        }

        /// <summary>
        /// Click the nth element from a collection identified by elementName/type.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element collection name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="whichElement">1-based index as string to select which element.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool ClickNthElement(FormBase currentPage, string elementName, string elementType, string whichElement)
        {
            // Parse 1-based index string, locate elements by locator and click the specified one.
            DebugOutput.OutputMethod("ClickNthElement", $"{currentPage.Name}, {elementName}, {elementType} {whichElement} ");
            int toBeClicked = 0;
            try
            {
                toBeClicked = Int32.Parse(whichElement);
            }
            catch
            {
                return Failure($"Failed to convert the string {whichElement} into an integer!");
            }
            if (toBeClicked < 1)
            {
                return Failure($"Although arrays start a 0, we have to assume the user will count 1, 2, 3... so the first element will be 1.  Hence no zero possible!");
            }
            var elementsLocator = GetDictionaryLocator.GetElementLocator(elementName, currentPage, elementType);
            if (elementsLocator == null) return Failure($"We did not find the locator for {elementName} of type {elementType} in page {currentPage}");
            var elements = SeleniumUtil.GetElements(elementsLocator);
            if (elements == null) return Failure($"We have a locator {elementsLocator} but no elements found - not even 1!");
            toBeClicked--;
            var elementToBeClicked = elements[toBeClicked];
            return SeleniumUtil.Click(elementToBeClicked);
        }

        /// <summary>
        /// Close browser tab by index.
        /// </summary>
        /// <param name="tabNumber">Index of tab to close.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool CloseTabByNumber(int tabNumber)
        {
            // Delegate to selenium util to close specified tab.
            DebugOutput.OutputMethod("CloseTabByNumber", $"{tabNumber}");
            return SeleniumUtil.CloseTabByNumber(tabNumber);
        }

        /// <summary>
        /// Close the web browser safely.
        /// </summary>
        public static void CloseWebBrowser()
        {
            // Gracefully close and quit driver if present.
            DebugOutput.OutputMethod("CloseWebBrowser");
            if (SeleniumUtil.webDriver == null) return;
            SeleniumUtil.webDriver.Close();
            SeleniumUtil.webDriver.Quit();
            SeleniumUtil.webDriver = null;
        }

        /// <summary>
        /// Gracefully shutdown the web driver and clear references.
        /// Returns true on success, false on failure.
        /// </summary>
        public static bool WebShutdown()
        {
            // Attempt to quit the driver; catch and report any exceptions.
            DebugOutput.OutputMethod("WebShutdown");
            try
            {
                if (SeleniumUtil.webDriver != null)
                {
                    // attempt to quit the driver
                    SeleniumUtil.webDriver.Quit();
                    SeleniumUtil.webDriver = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"WebShutdown failed: {ex.Message}");
                return Failure($"WebShutdown failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a model useful for self-healing element location strategies.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="element">Found IWebElement instance.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True if model written to file successfully, false otherwise.</returns>
        public static bool CreateSelfHealModel(FormBase currentPage, IWebElement element, string elementName, string elementType)
        {
            // Collect metadata about element and persist model via SelfHeal.WriteToFile.
            DebugOutput.OutputMethod($"CreateSelfHealModel", $"{currentPage.Name} {element} {elementName} {elementType}");
            if (elementName == null || elementType == null) return false;
            var model = new SelfHealModel();
            model.PageName = currentPage.Name;
            model.ElementName = elementName;
            model.ElementType = elementType;
            model.ElementKnownLocator = GetDictionaryLocator.GetElementLocator(elementName, currentPage, elementType);
            model.ElementXPathString = SeleniumUtil.GetFullXPathOfElement(element);
            model.ElementText = SeleniumUtil.GetElementText(element);
            model.ElementTag = element.TagName;
            model.ElementEnabled = element.Enabled;

            return SelfHeal.WriteToFile(model);
        }

        /// <summary>
        /// Double click on the specified element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool DoubleClickOnElement(FormBase currentPage, string elementName, string elementType)
        {
            // Locate element and perform double-click via Selenium util.
            DebugOutput.OutputMethod("ClickOnElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            return SeleniumUtil.DoubleClick(element);
        }

        /// <summary>
        /// Drag element A onto element B.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="element1Name">Source element name.</param>
        /// <param name="element2Name">Target element name.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool DragElementAToElementB(FormBase currentPage, string element1Name, string element2Name, string elementType)
        {
            // Resolve both elements and delegate drag operation.
            DebugOutput.OutputMethod("DragAToB", $"{currentPage.Name}, {element1Name}, {element2Name}, {elementType}");
            var element1 = GetTheElement.GetElement(currentPage, element1Name, elementType);
            if (element1 == null) return Failure($"Failed to find 1 {element1Name}");
            var element2 = GetTheElement.GetElement(currentPage, element2Name, elementType);
            if (element2 == null) return Failure($"Failed to find 2 {element2Name}");
            return SeleniumUtil.DragElementToElement(element1, element2);
        }

        /// <summary>
        /// Enter text into an element and optionally send a key.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="text">Text to enter.</param>
        /// <param name="key">Optional key to send after text.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool EnterTextAndKeyIntoElement(FormBase currentPage, string elementName, string elementType, string text, string key = "")
        {
            // Resolve element and call SeleniumUtil.EnterText with optional key.
            DebugOutput.OutputMethod("EnterTextAndKeyIntoElement", $"{currentPage.Name}, {elementName}, {elementType} {text} {key}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            return SeleniumUtil.EnterText(element, text, key);
        }

        // ------------------ Getters / Query helpers ------------------
        // Group: methods that retrieve text, attributes, counts, selection

        /// <summary>
        /// Get the attribute value of a named sub-element (by visible name) under an element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="subElementName">Visible text of sub-element to match.</param>
        /// <param name="attribute">Attribute name to return.
        /// </param>
        /// <returns>Attribute value or null on failure.</returns>
        public static string? GetAttributeValueOfSubElementByNameOfElement(FormBase currentPage, string elementName, string elementType, string subElementName, string attribute)
        {
            // Iterate sub-elements and return requested attribute for the matching sub-element.
            DebugOutput.OutputMethod("GetAttributeValueOfSubElementByNameOfElement", $"{currentPage.Name}, {elementName}, {elementType} {subElementName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return FailureString($"FAiled to find the sub elements!");
            IWebElement? wantedElement = null;
            foreach (var subElement in subElements)
            {
                var text = SeleniumUtil.GetElementText(subElement);
                if (text == subElementName)
                {
                    wantedElement = subElement;
                    break;
                }
            }
            if (wantedElement == null) return FailureString($"FAiled to find a sub element, of which we found some, that equaled {subElementName}");
            return SeleniumUtil.GetElementAttributeValue(wantedElement, attribute);
        }

        /// <summary>
        /// Get label text from an element using a provided locator for the label.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="labelLocator">Locator for the label element under the parent.</param>
        /// <returns>Label text or null on failure.</returns>
        public static string? GetLabelFromElement(FormBase currentPage, string elementName, string elementType, By labelLocator)
        {
            // Find the parent element, then find the label under it and return its text.
            DebugOutput.OutputMethod("GetLabelFromElement", $"{currentPage.Name}, {elementName}, {elementType} {labelLocator}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            var labelElement = SeleniumUtil.GetElementUnderElement(element, labelLocator, 0);
            if (labelElement == null) return FailureString($"Failed to find the parent or its label! by {labelLocator}");
            return SeleniumUtil.GetElementText(labelElement);
        }

        /// <summary>
        /// Get the placeholder text associated with an input element by reading its parent label.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>Placeholder text or null on failure.</returns>
        public static string? GetPlaceholderText(FormBase currentPage, string elementName, string elementType)
        {
            // Get the element's parent and read the label tag text as placeholder.
            DebugOutput.OutputMethod("GetPlaceholderText", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            //  We want selenium to get the parent - then get the label tag element below - then get its text
            var elementParent = SeleniumUtil.GetElementParent(element);
            if (elementParent == null) return FailureString($"Failed to get the parent of the element {elementName} of type {elementType} in page {currentPage.Name}");
            By labelLocator = By.TagName("label");
            var labelElement = SeleniumUtil.GetElementUnderElement(elementParent, labelLocator, 0);
            if (labelElement == null) return FailureString($"Failed to find the its label! by {labelLocator}");
            var placeHolderText = SeleniumUtil.GetElementtextDirect(labelElement);
            if (placeHolderText == null) return FailureString($"Failed to get the placeholder text from the label element {labelElement}");
            DebugOutput.Log($"We have a placeholder text of '{placeHolderText}'");
            return placeHolderText;
        }

        /// <summary>
        /// Return the number of tabs currently open in the browser.
        /// </summary>
        /// <returns>Number of open tabs or null on failure.</returns>
        public static int? GetTheNumberOfTabsOpenInBrowser()
        {
            // Delegate to Selenium util to count tabs.
            DebugOutput.OutputMethod("GetNumberOfTabsOpenInBrowser", "");
            return SeleniumUtil.GetNumberOfTabsOpenInBrowser();
        }

        /// <summary>
        /// Get the selected value text of a selector element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>Selected text value or null if not found.</returns>
        public static string? GetSelectionValue(FormBase currentPage, string elementName, string elementType)
        {
            // Try direct text, then inspect option items to find selected one.
            DebugOutput.OutputMethod("GetSelectorValue", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            var text = SeleniumUtil.GetElementtextDirect(element);
            if (text != null)
            {
                DebugOutput.Log($"We have a text value of '{text}'");
                return text;
            }
            DebugOutput.Log($"We have a null value FOR TEXT of the element - lets check for selectors!");
            foreach (var optionLocator in GetSubLocators.DropDownItemLocators)
            {
                var optionElements = SeleniumUtil.GetElementsUnder(element, optionLocator);
                if (optionElements.Count > 0)
                {
                    foreach (var option in optionElements)
                    {
                        if (SeleniumUtil.IsSelected(option)) return SeleniumUtil.GetElementText(option);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get all possible selection values under a dropdown-like element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>List of option texts or null on failure.</returns>
        public static List<string>? GetSelectionValues(FormBase currentPage, string elementName, string elementType)
        {
            // Collect text from all found option elements under the parent.
            DebugOutput.OutputMethod("GetSelectorValue", $"{currentPage.Name}, {elementName}, {elementType}");
            var listOfPossibleValues = new List<string>();
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureListString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            foreach (var optionLocator in GetSubLocators.DropDownItemLocators)
            {
                var optionElements = SeleniumUtil.GetElementsUnder(element, optionLocator);
                if (optionElements.Count > 0)
                {
                    foreach (var option in optionElements)
                    {
                        var text = SeleniumUtil.GetElementText(option);
                        listOfPossibleValues.Add(text);
                    }
                }
            }
            return listOfPossibleValues;
        }

        // Private helper: get sub-element index by visible name
        private static int? GetSubElementCountFromElement(IWebElement element, string elementType, string name)
        {
            // Find matching sub-element by comparing its text and return index.
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return null;
            int counter = 0;
            foreach (var subElement in subElements)
            {
                var text = SeleniumUtil.GetElementText(subElement);
                if (text == name) return counter;
                counter++;
            }
            DebugOutput.Log($"FAiled to find {name} in the sub elements of type {elementType}");
            return null;
        }

        /// <summary>
        /// Get the text of a sub-element found under the nth element of a list.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element list name.</param>
        /// <param name="elementType">Parent element type.</param>
        /// <param name="subElementName">Sub-element name key.</param>
        /// <param name="subElementType">Sub-element type string.</param>
        /// <param name="nth">1-based index of which parent element to inspect.
        /// </param>
        /// <returns>Text of the sub-element or null on failure.</returns>
        public static string? GetSubElementTextFromNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth)
        {
            // Retrieve nth parent element then fetch its sub-element by locator and return text.
            DebugOutput.OutputMethod("GetSubElementTextFromNthElement", $"{currentPage.Name}, {elementName}, {elementType} {subElementName} {subElementType} {nth}");
            // get the nth Element
            var element = GetTheElements.GetNthElement(currentPage, elementName, elementType, nth, 0);
            if (element == null) return FailureString($"Failed to get the {nth}th element!");
            // now to get sub            
            var locator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (locator == null)
            {
                DebugOutput.Log($"NO LOCATOR FOUND IN PAGE {currentPage.Name} FOR NAME {elementName} OF TYPE {elementType} - unless the element is in the ");
                return null;
            }
            var subElement = SeleniumUtil.GetElementUnderElement(element, locator, 0);
            if (subElement == null) return FailureString($"No sub element found {locator}");
            return SeleniumUtil.GetElementText(subElement);
        }

        /// <summary>
        /// Get text content from a single element identified by name/type.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>Text or null on failure.</returns>
        public static string? GetTextFromElement(FormBase currentPage, string elementName, string elementType)
        {
            // Attempt to fetch text, retrying once after a short delay if null.
            DebugOutput.OutputMethod("GetTextFromElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            var text = SeleniumUtil.GetElementText(element);
            if (text == null)
            {
                DebugOutput.Log($"This first time, I'm going to wait 1 second.. try again!");
                Thread.Sleep(1000);
                text = SeleniumUtil.GetElementText(element);
            }
            if (text == null) return FailureString($"Failed to get any text from element! {elementName} of type {elementType} in page {currentPage.Name}");
            return text;
        }

        /// <summary>
        /// Get text from last element in a collection.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Collection name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>Text of last element or null on failure.</returns>
        public static string? GetTextFromLastElement(FormBase currentPage, string elementName, string elementType)
        {
            // Fetch all elements and return text of final one.
            DebugOutput.OutputMethod("GetTextFromLastElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType, 0);
            if (elements == null) return FailureString($"Never found ANY elements using {elementName} of {elementType} in page {currentPage}");
            return SeleniumUtil.GetElementText(elements[elements.Count() - 1]);
        }

        /// <summary>
        /// Get text from the nth element in a collection (1-based).
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Collection name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="nth">1-based index of element to read.</param>
        /// <returns>Text of the nth element or null on failure.</returns>
        public static string? GetTextFromNthElement(FormBase currentPage, string elementName, string elementType, int nth)
        {
            // Validate index and return text of specified element.
            DebugOutput.OutputMethod("GetTextFromNthElement", $"{currentPage.Name}, {elementName}, {elementType} {nth}");
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType, 0);
            if (elements == null) return FailureString($"Never found ANY elements using {elementName} of {elementType} in page {currentPage}");
            if (elements.Count() < nth) return FailureString($"Your looking for the {nth}th element, but I only found {elements.Count()}");
            if (nth < 1) return FailureString($"0 is fine for an array, but humans count 1, 2, 3...");
            nth--;  // becuase it IS an array
            return SeleniumUtil.GetElementText(elements[nth]);
        }

        /// <summary>
        /// Save a screenshot of the specified element to disk.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key used for file naming.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool GetScreenShotOfElement(FormBase currentPage, string elementName, string elementType)
        {
            // Get IWebElement and call Selenium util to capture it.
            DebugOutput.OutputMethod("GetScreenshotOfElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"Failed to take screen shot {currentPage.Name} {elementName}  {elementType}");
            // if (!SeleniumUtil.ScrollToElement(element)) return Failure($"Failed to SCROLL to the element! {elementName}");
            Thread.Sleep(200);
            return SeleniumUtil.ScreenShotElement(element, elementName);
        }

        /// <summary>
        /// Save a screenshot of the current page.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool GetScreenShotOfPage(FormBase currentPage)
        {
            // Delegate to Selenium util to save page image with page name.
            DebugOutput.OutputMethod("GetScreenShotOfPage", $"{currentPage.Name}");
            return SeleniumUtil.SaveCurrentPageImage(currentPage.Name);
        }

        /// <summary>
        /// Save an error screenshot with an associated message.
        /// </summary>
        /// <param name="errorMessage">Error message used for naming/logging.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool GetErrorScreenShotOfPage(string errorMessage)
        {
            // Attempt to capture screenshot of current page and return success state.
            DebugOutput.OutputMethod("GetErrorScreenShotOfPage", $"{errorMessage}");
            return SeleniumUtil.GetCurrentPageScreenShot(errorMessage) ?? false;
        }

        // Private helper to get sub-elements matching an array of locators
        private static List<IWebElement>? GetSubElementOfElement(IWebElement element, By[] locators)
        {
            // Try each locator in order until any matching elements are found.
            foreach (var locator in locators)
            {
                DebugOutput.Log($"Checking LOCATOR {locator} ");
                var subElements = SeleniumUtil.GetElementsUnder(element, locator, 1);
                if (subElements != null)
                {
                    if (subElements.Count > 0) return subElements;
                }
            }
            DebugOutput.Log("Nothing returned!");
            return null;
        }

        /// <summary>
        /// Get visible texts of sub-elements under a parent element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <returns>List of sub-element texts or null on failure.</returns>
        public static List<string>? GetSubElementsTextOfElement(FormBase currentPage, string elementName, string elementType)
        {
            // Resolve the parent and collect texts for each sub-element.
            DebugOutput.OutputMethod("GetSubElementsOfElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return null;
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return FailureListString($"Failed to get sub elements! but did get element!");
            var returnList = new List<string>();
            foreach (var e in subElements)
            {
                var text = SeleniumUtil.GetElementText(e);
                if (text != null) returnList.Add(text);
            }
            return returnList;
        }

        // Overloaded private helper for sub-elements selection logic
        private static List<IWebElement>? GetSubElementsOfElement(IWebElement element, string elementType, string subType = "")
        {
            // Decide which locator arrays to use based on elementType and subtype.
            DebugOutput.OutputMethod("GetSubElementsOfElement", $"{element} {elementType} {subType}");
            if (element == null) return null;
            if (subType != "")
            {
                switch (subType.ToLower())
                {
                    default: return null;

                    case "table.body": return GetSubElementOfElement(element, GetSubLocators.TableBodyLocator);
                    case "table.nextpage": return GetSubElementOfElement(element, GetSubLocators.TableNextPageButton);
                }
            }
            switch (elementType.ToLower())
            {
                default:
                case "list": return GetSubElementsOfElement(element, GetSubLocators.ListItemLocator);

                case "step":
                case "stepper":
                case "steps": return GetSubElementsOfElement(element, GetSubLocators.StepperStepLabelLocator);

                case "tab": return GetSubElementsOfElement(element, GetSubLocators.TabLocator);

                case "table": return GetSubElementsOfElement(element, GetSubLocators.TableBodyRowLocator);

                case "tree": return GetSubElementsOfElement(element, GetSubLocators.TreeNodeLocator);
            }
        }

        // Private helper that tries multiple locators to find sub-elements
        private static List<IWebElement>? GetSubElementsOfElement(IWebElement element, By[] locators)
        {
            // Iterate locator candidates and return the first set of elements found.
            foreach (var locator in locators)
            {
                DebugOutput.Log($"Checking LOCATOR {locator} ");
                var subElements = SeleniumUtil.GetElementsUnder(element, locator, 1);
                if (subElements != null)
                {
                    if (subElements.Count > 0) return subElements;
                }
            }
            DebugOutput.Log("Nothing returned!");
            return null;
        }

        // private helper to get sub-sub elements under nth element
        private static List<IWebElement>? GetSubSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
            // Find the nth parent element then its sub-element and return matching sub-sub elements.
            DebugOutput.OutputMethod("GetSubSubElementsOfNthElement", $"{currentPage.Name}, {elementName}, {elementType}");
            // get list elements - ANSWER 
            var nthElement = GetTheNthElement(currentPage, elementName, elementType, nth);
            if (nthElement == null)
            {
                DebugOutput.Log($"Failed to find ANY nth element {currentPage.Name} {elementName} {elementType}");
                return null;
            }
            // answers citation wrapper
            var sublocator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (sublocator == null)
            {
                DebugOutput.Log($"Failed to get LOCATOR for sub locator {subElementName} {subElementType}");
                return null;
            }
            var subElement = SeleniumUtil.GetElementUnderElement(nthElement, sublocator);
            if (subElement == null)
            {
                DebugOutput.Log($"o sub element found {subElementName} {subElementType}");
                return null;
            }
            // answers citation
            var subSubLocator = GetDictionaryLocator.GetElementLocator(subSubElementName, currentPage, subSubElementType);
            if (subSubLocator == null)
            {
                DebugOutput.Log($"FAiled to get the SUB SUB locator! {subElementName} {subElementType}");
                return null;
            }
            var subSubElements = SeleniumUtil.GetElementsUnder(subElement, subSubLocator);
            if (subSubElements == null)
            {
                DebugOutput.Log($"Failed at getting the sub sub element! {subElementName} {subElementType}");
                return null;
            }
            return subSubElements;
        }

        /// <summary>
        /// Get text of the sub-element that is currently selected under a parent element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <returns>Text of selected sub-element or null if none selected.</returns>
        public static string? GetTextFromSubElementSelectedOfElement(FormBase currentPage, string elementName, string elementType)
        {
            // Iterate sub-elements and return text of the one with selected state.
            DebugOutput.OutputMethod("GetTextFromSubElementSelectedOfElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString("FAiled to even get the parent element!");
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return FailureString($"failed to get the sub elements!");
            foreach (var e in subElements)
            {
                if (SeleniumUtil.IsSelected(e)) return SeleniumUtil.GetElementText(e);
            }
            return null;
        }

        /// <summary>
        /// Get list of texts (or attribute values) from sub-sub elements under the nth parent element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="subElementName">Sub-element name key.</param>
        /// <param name="subElementType">Sub-element type string.</param>
        /// <param name="subSubElementName">Sub-sub element name key.</param>
        /// <param name="subSubElementType">Sub-sub element type string.</param>
        /// <param name="nth">Index of parent element to inspect.</param>
        /// <param name="attribute">Optional attribute name to read instead of text.
        /// </param>
        /// <returns>List of strings collected or null on failure.</returns>
        public static List<string>? GetTextListFromSubSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0, string attribute = "")
        {
            // Collect text or attribute for each matching sub-sub element.
            DebugOutput.OutputMethod("GetTextFromSubSubElementOfLastElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var subSubElements = GetSubSubElementsOfNthElement(currentPage, elementName, elementType, subElementName, subElementType, subSubElementName, subSubElementType, nth);
            if (subSubElements == null) return FailureListString($"Failed at getting the sub sub element!");
            var returnList = new List<string>();
            foreach (var subsubElement in subSubElements)
            {
                string? text = null;
                if (attribute == "") text = SeleniumUtil.GetElementText(subsubElement);
                else
                {
                    text = SeleniumUtil.GetElementAttributeValue(subsubElement, attribute);
                }
                if (text != null) returnList.Add(text);
            }
            return returnList;
        }

        // Private helper: get list of sub-elements of nth element via locator lookup
        private static List<IWebElement>? GetTheListOfSubElementsofNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth = 0)
        {
            // Use GetTheNthElement and dictionary locator to return elements under that nth element.
            var element = GetTheNthElement(currentPage, elementName, elementType);
            if (element == null) return null;

            var sublocator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (sublocator == null) return null;

            return SeleniumUtil.GetElementsUnder(element, sublocator);
        }

        // Private helper: return the nth element (or last if nth==0) from a named collection
        private static IWebElement? GetTheNthElement(FormBase currentPage, string elementName, string elementType, int nth = 0)
        {
            // Get all elements for the collection then pick nth or last.
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType);
            if (elements == null) return null;
            var numberOfElements = elements.Count();
            if (nth == 0) return elements[numberOfElements - 1];
            nth--;
            return elements[numberOfElements - nth];
        }

        /// <summary>
        /// Get the count of sub-elements under a given element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <returns>Count of sub-elements or null on failure.</returns>
        public static int? GetTheNumberOfSubElementsOfElement(FormBase currentPage, string elementName, string elementType)
        {
            // Resolve element and return number of sub-elements.
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return null;
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return null;
            return subElements.Count();
        }

        /// <summary>
        /// Get count of sub-elements for the nth element in a collection.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Collection name key.</param>
        /// <param name="elementType">Collection element type string.</param>
        /// <param name="subElementName">Sub-element name key.</param>
        /// <param name="subElementType">Sub-element type string.</param>
        /// <param name="nth">Index (1-based) of parent element.</param>
        /// <returns>Count or null on failure.</returns>
        public static int? GetTheNumberOfSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth = 0)
        {
            // Use helper to get list and return its count.
            var listOfElements = GetTheListOfSubElementsofNthElement(currentPage, elementName, elementType, subElementName, subElementType, nth);
            if (listOfElements == null) return FailureInt($"Failed to get list of elements!");
            return listOfElements.Count();
        }

        /// <summary>
        /// Get number of sub-sub elements under an element when there is a single sub-element containing them.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="subType">Sub-type identifier.
        /// </param>
        /// <returns>Count or null on failure.</returns>
        public static int? GetTheNumberOfSubSubElementsOfElement(FormBase currentPage, string elementName, string elementType, string subType)
        {
            // Delegate to GetTheSubSubElementsOfElement and return the count.
            var subSubElement = GetTheSubSubElementsOfElement(currentPage, elementName, elementType, subType);
            if (subSubElement == null) return FailureInt($"Failed to find any sub sub element!");
            return subSubElement.Count;
        }

        /// <summary>
        /// Get count of sub-sub elements under the nth element in a collection.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Collection name key.</param>
        /// <param name="elementType">Collection element type string.</param>
        /// <param name="subElementName">Sub-element name key.
        /// </param>
        /// <param name="subElementType">Sub-element type string.
        /// </param>
        /// <param name="subSubElementName">Sub-sub element name key.</param>
        /// <param name="subSubElementType">Sub-sub element type string.</param>
        /// <param name="nth">Index of parent element.</param>
        /// <returns>Count or null on failure.</returns>
        public static int? GetTheNumberOfSubSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
            // Get list of sub-sub elements for nth parent and return count.
            var listOfElements = GetTheListOfSubSubElementsofNthElement(currentPage, elementName, elementType, subElementName, subElementType, subSubElementName, subSubElementType, nth);
            if (listOfElements == null) return FailureInt($"Failed to get list of elements!");
            return listOfElements.Count();
        }

        // Private helper to get a specific sub-element for nth parent
        private static IWebElement? GetTheSubElementOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth = 0)
        {
            // Resolve parent and sub-element locator and return single element.
            var element = GetTheNthElement(currentPage, elementName, elementType);
            if (element == null) return null;

            var locator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (locator == null) return null;

            return SeleniumUtil.GetElementUnderElement(element, locator);
        }

        // Private helper to get sub-sub elements under a parent element
        private static List<IWebElement>? GetTheSubSubElementsOfElement(FormBase currentPage, string elementName, string elementType, string subType)
        {
            // Retrieve the first sub-element then return the list of its child sub-elements.
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return null;
            var subElements = GetSubElementsOfElement(element, elementType, subType);
            if (subElements == null) return null;
            if (subElements.Count > 1) return null;
            var subSubElement = GetSubElementsOfElement(subElements[0], elementType);
            if (subSubElement == null) return null;
            return subSubElement;
        }

        // Private helper to get a single sub-sub element under the nth element
        private static IWebElement? GetTheSubSubElementofNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
            // Resolve nth parent, sub-element locator and sub-sub locator to return single element.
            var element = GetTheNthElement(currentPage, elementName, elementType);
            if (element == null) return null;

            var sublocator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (sublocator == null) return null;

            var subElement = SeleniumUtil.GetElementUnderElement(element, sublocator);
            if (subElement == null) return null;

            var subSubLocator = GetDictionaryLocator.GetElementLocator(subSubElementName, currentPage, subSubElementType);
            if (subSubLocator == null) return null;

            return SeleniumUtil.GetElementUnderElement(subElement, subSubLocator);
        }

        // Private helper to get list of sub-sub elements under nth parent
        private static List<IWebElement>? GetTheListOfSubSubElementsofNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
            // Resolve nth parent, sub-element and sub-sub locator and return list.
            var element = GetTheNthElement(currentPage, elementName, elementType);
            if (element == null) return null;

            var sublocator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (sublocator == null) return null;

            var subElement = SeleniumUtil.GetElementUnderElement(element, sublocator);
            if (subElement == null) return null;

            var subSubLocator = GetDictionaryLocator.GetElementLocator(subSubElementName, currentPage, subSubElementType);
            if (subSubLocator == null) return null;

            return SeleniumUtil.GetElementsUnder(subElement, subSubLocator);
        }

        /// <summary>
        /// Get the width (in pixels) of an element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>Width in pixels or null on failure.</returns>
        public static int? GetWidthOfElement(FormBase currentPage, string elementName, string elementType)
        {
            // Resolve element and ask Selenium util for its width.
            DebugOutput.OutputMethod("GetText", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureInt($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            return SeleniumUtil.GetWidthOfElement(element);
        }

        /// <summary>
        /// Check whether an element is displayed (visible) within an optional timeout.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="timeout">Timeout in seconds to wait for element to exist.
        /// </param>
        /// <returns>True if displayed, false otherwise.</returns>
        public static bool IsElementDisplayed(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            // Try to find the element and confirm it is displayed; optionally create self-heal model.
            DebugOutput.OutputMethod("IsElementDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {timeout}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
            if (element == null) return false;
            SeleniumUtil.MoveToElement(element);
            if (SeleniumUtil.IsDisplayed(element, false, timeout))
            {
                if (TargetConfiguration.Configuration.SelfHeal) CreateSelfHealModel(currentPage, element, elementName, elementType);
                return true;
            }
            DebugOutput.Log($"Element exists - just not displayed!");
            return false;
        }        

        /// <summary>
        /// Determine whether an expandable element is in an expanded state.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True if expanded, false otherwise.</returns>
        public static bool IsElementExpanded(FormBase currentPage, string elementName, string elementType)
        {
            // Inspect child's aria-expanded attribute to determine expansion state.
            DebugOutput.OutputMethod("IsElementExpanded", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var expansionElement = GetTheElement.GetExpansion(element);
            if (expansionElement == null) return Failure($"Failed to find expansion element under {element}");
            var attributeValue = SeleniumUtil.GetElementAttributeValue(expansionElement, "aria-expanded");
            if (attributeValue == null) return Failure($"Failed to get attribute from expansion element {expansionElement}");
            DebugOutput.Log($"We got attribute value of {attributeValue}");
            if (attributeValue.ToLower() == "true") return true;
            return false;
        }

        /// <summary>
        /// Check whether an element exists (regardless of visibility), with an optional timeout.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="timeout">Timeout seconds to wait for existence.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public static bool IsElementExists(FormBase currentPage, string elementName, string elementType, int timeout)
        {
            // Resolve element with timeout; return false if null.
            DebugOutput.OutputMethod("IsElementDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {timeout}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            return true;
        }

        /// <summary>
        /// Determine whether a named group of a parent element is displayed.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.
        /// </param>
        /// <param name="groupName">Group key to find under parent.
        /// </param>
        /// <returns>True if displayed, false otherwise.</returns>
        public static bool IsElementGroupDisplayed(FormBase currentPage, string elementName, string elementType, string groupName)
        {
            // Get the group element under parent and verify display state.
            DebugOutput.OutputMethod("IsElementGroupDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {groupName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var groupElement = GetTheElement.GetGroup(element, groupName);
            if (groupElement == null) return Failure($"Failed to get the group element under element!");
            return SeleniumUtil.IsDisplayed(groupElement);
        }

        /// <summary>
        /// Determine whether a named group is expanded by checking its expansion element.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.
        /// </param>
        /// <param name="groupName">Group key to find under parent.
        /// </param>
        /// <returns>True if expanded, false otherwise.</returns>
        public static bool IsElementGroupExpanded(FormBase currentPage, string elementName, string elementType, string groupName)
        {            
            // Find the group's expansion element and inspect aria-expanded attribute.
            DebugOutput.OutputMethod("IsElementGroupDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {groupName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var groupElement = GetTheElement.GetGroup(element, groupName);
            if (groupElement == null) return Failure($"Failed to get the group element under element!");
            DebugOutput.Log($"WE HAVE THE GROUP {groupName}");
            var groupExpandedElement = GetTheElement.GetGroupExpanded(groupElement);
            if (groupExpandedElement == null) return Failure($"Failed to find the group expanded elemtent");
            var expandedString = SeleniumUtil.GetElementAttributeValue(groupExpandedElement, "aria-expanded");
            if (expandedString == null) return Failure($"Failed to get the attribute value of the group expanded element");
            var expanded = StringValues.ConvertStringToBool(expandedString);
            if (expanded == null) return Failure($"Failed to convert the string to a bool!");
            return expanded ?? false;
        }        

        /// <summary>
        /// Wait until an element is not displayed (becomes hidden) within a timeout.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <param name="timeout">Timeout in seconds to wait for disappearance.</param>
        /// <returns>True if not displayed, false if still displayed after timeout.</returns>
        public static bool IsElementNotDisplayed(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            // Immediate check then loop with negative timeout until element not displayed.
            DebugOutput.OutputMethod("IsElementDisplayedNot", $"{currentPage.Name}, {elementName}, {elementType}, {timeout}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return true;
            DebugOutput.Log($"We have an element!");
            if (!SeleniumUtil.IsDisplayed(element)) return true;
            DebugOutput.Log($"The element is displayed, but this is INSTANT");
            int counter = 0;
            int negativeTimeout = timeout;
            if (negativeTimeout == 0) negativeTimeout = TargetConfiguration.Configuration.NegativeTimeout;
            while (SeleniumUtil.IsDisplayed(element))
            {
                if (counter >= negativeTimeout) return Failure($"We have waited {negativeTimeout} seconds - and elementName {elementName} is still displayed!");
                Thread.Sleep(1000);
                counter++;
            }
            DebugOutput.Log($"Finally gone, waited {counter} seconds");
            return true;
        }

        /// <summary>
        /// Check whether an element is enabled (interactive).
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True if enabled, false otherwise.</returns>
        public static bool IsElementEnabled(FormBase currentPage, string elementName, string elementType)
        {
            // Locate element and query enabled state from Selenium util.
            DebugOutput.OutputMethod("IsEnabled", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know if enabled or not!");
            return SeleniumUtil.IsEnabled(element);
        }

        /// <summary>
        /// Determine whether a group is not expanded.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="groupName">Group key under the parent.
        /// </param>
        /// <returns>True if not expanded, false otherwise.</returns>
        public static bool IsElementGroupNotExpanded(FormBase currentPage, string elementName, string elementType, string groupName)
        {
            // Inspect group's aria-expanded attribute and return negated state.
            DebugOutput.OutputMethod("IsElementGroupDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {groupName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var groupElement = GetTheElement.GetGroup(element, groupName);
            if (groupElement == null) return Failure($"Failed to get the group element under element!");
            DebugOutput.Log($"WE HAVE THE GROUP {groupName}");
            var groupExpandedElement = GetTheElement.GetGroupExpanded(groupElement);
            if (groupExpandedElement == null) return Failure($"Failed to find the group expanded elemtent");
            var expandedString = SeleniumUtil.GetElementAttributeValue(groupExpandedElement, "aria-expanded");
            if (expandedString == null) return Failure($"Failed to get the attribute value of the group expanded element");
            var expanded = StringValues.ConvertStringToBool(expandedString);
            if (expanded == null) return Failure($"Failed to convert the string to a bool!");
            DebugOutput.Log($"We doing a negative! {expanded}");
            if (expanded ?? true) return false;
            return true;
        }        

        /// <summary>
        /// Determine whether an element is read-only (not enabled).
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.</param>
        /// <returns>True if read-only, false otherwise.</returns>
        public static bool IsElementReadOnly(FormBase currentPage, string elementName, string elementType)
        {
            // Return negation of IsEnabled check.
            DebugOutput.OutputMethod("IsReadOnly", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know if enabled or not!");
            return !SeleniumUtil.IsEnabled(element);
        }

        /// <summary>
        /// Determine whether an element is selected; special handling for switches.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Element name key.</param>
        /// <param name="elementType">Element type string.
        /// </param>
        /// <returns>True if selected, false otherwise.</returns>
        public static bool IsElementSelected(FormBase currentPage, string elementName, string elementType)
        {
            // Use special switch detection, otherwise check selected state.
            DebugOutput.OutputMethod("IsSelected", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            if (elementType.ToLower() == "switch")
            {
                var status = SeleniumUtil.IsSelectedSwitch(element);
                if (status == null) DebugOutput.Log($"I can only return true or false - im making an assumption - FALSE");
                return status ?? false;
            } 
            return SeleniumUtil.IsSelected(element);
        }

        /// <summary>
        /// Determine whether the parent element of a named element is displayed.
        /// </summary>
        public static bool IsElementsParentElementDisplayed(FormBase currentPage, string elementName, string elementType)
        {
            // Get parent via helper and check its Displayed property.
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var parentElement = GetTheElement.GetParent(element);
            if (parentElement == null) return Failure($"No PARENT element found above {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            if (parentElement.Displayed)
            {
                // if (TargetConfiguration.Configuration.SelfHeal) CreateSelfHealModel(currentPage, element, elementName, elementType);
                return true;
            }
            DebugOutput.Log($"PARENT Element exists - just not displayed!");
            return false;
        }

        /// <summary>
        /// Check whether an element under a parent with a given tag and matching text is displayed.
        /// </summary>
        public static bool IsElementUnderElementByTagByTextDisplayed(FormBase currentPage, string elementName, string elementType, string tag, string text)
        {
            // Build tag locator and call common locator/text checker.
            DebugOutput.OutputMethod("IsElementUnderElementByTagDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {tag} {text}");
            var locator = By.TagName(tag);
            return IsElementUnderElementByLocatorByTextDisplayed(currentPage, elementName, elementType, locator, text);
        }

        /// <summary>
        /// Check whether an element under a parent with a given class and matching text is displayed.
        /// </summary>
        public static bool IsElementUnderElementByClassByTextDisplayed(FormBase currentPage, string elementName, string elementType, string classWanted, string text)
        {
            // Build class locator and use generic checker.
            DebugOutput.OutputMethod("IsElementUnderElementByTagDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {classWanted} {text}");
            var locator = By.ClassName(classWanted);
            return IsElementUnderElementByLocatorByTextDisplayed(currentPage, elementName, elementType, locator, text);
        }        

        /// <summary>
        /// Check whether a sub-element under a parent located by a locator with given text is displayed.
        /// </summary>
        public static bool IsElementUnderElementByLocatorByTextDisplayed(FormBase currentPage, string elementName, string elementType, By locator, string text)
        {
            // Use helper to find sub-element by locator-text then check display state.
            DebugOutput.OutputMethod("IsElementUnderElementByTagDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {locator} {text}");
            var subElement = GetTheElement.GetSubElementByText(currentPage, elementName, elementType, locator, text, 0);
            if (subElement == null) return Failure($"Failed to find the sub element by {locator} with text {text} under {elementName}");
            return SeleniumUtil.IsDisplayed(subElement);
        }

        /// <summary>
        /// Check whether a sub-element under a parent (located by matching text anywhere) is displayed.
        /// </summary>
        public static bool IsElementUnderElementByTextDisplayed(FormBase currentPage, string elementName, string elementType, string subElementText)
        {
            // Build XPath for exact text and use helper to find and check display.
            DebugOutput.OutputMethod("IsElementUnderElementByTextDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {subElementText}");
            By locator = By.XPath($".//*[text()='{subElementText}']");
            var subElement= GetTheElement.GetSubElementByText(currentPage, elementName, elementType, locator, subElementText, 0);
            if (subElement == null) return Failure($"Failed to find the sub element!");
            return SeleniumUtil.IsDisplayed(subElement);
        }

        /// <summary>
        /// Check whether the Selenium web driver is active.
        /// </summary>
        /// <returns>True if webDriver not null, otherwise false.</returns>
        public static bool IsDriverActive()
        {
            // Simple null-check for driver.
            DebugOutput.OutputMethod("IsDriverNull", "");
            if (SeleniumUtil.webDriver == null) return false;
            return true;            
        }

        /// <summary>
        /// Determine whether a named sub-element (by visible name) is displayed under a parent element.
        /// </summary>
        public static bool IsSubElementDisplayed(FormBase currentPage, string elementName, string elementType, string subElementName)
        {
            // Iterate sub-elements to find matching visible name then return its display state.
            DebugOutput.OutputMethod("IsSubElementDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {subElementName} ");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return Failure($"FAiled to get sub elements if this!");
            foreach (var e in subElements)
            {
                var text = SeleniumUtil.GetElementText(e);
                if (text == subElementName) return SeleniumUtil.IsDisplayed(e);
            }
            DebugOutput.Log($"To get here - no sub element found!");
            return false;
        }

        /// <summary>
        /// Move mouse cursor over an element (hover).
        /// </summary>
        public static bool MouseOverElement(FormBase currentPage, string elementName, string elementType)
        {
            // Use Selenium util to move to element (hover behaviour).
            DebugOutput.OutputMethod("MouseOver", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            return SeleniumUtil.MoveToElement(element);
        }

        /// <summary>
        /// Move a slider element to a specified value.
        /// </summary>
        public static bool MoveSliderElement(FormBase currentPage, string elementName, string elementType, int value)
        {
            // Locate the slider element and instruct Selenium util to change its value.
            DebugOutput.OutputMethod("MoveSliderElement", $"{currentPage.Name}, {elementName}, {elementType} {value}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            return SeleniumUtil.MoveSliderElement(element, value);
        }

        /// <summary>
        /// Navigate the browser to a specified URL.
        /// </summary>
        public static bool NavigateToURL(string url)
        {
            // Delegate to Selenium util's navigation.
            DebugOutput.OutputMethod("NavigateToURL", $"{url}");
            return SeleniumUtil.NavigateToURL(url);
        }

        /// <summary>
        /// Perform a right-click (context click) on an element.
        /// </summary>
        public static bool RightClickElement(FormBase currentPage, string elementName, string elementType)
        {
            // Locate element and perform right-click via Selenium util.
            DebugOutput.OutputMethod("RightClick", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            return SeleniumUtil.RightClick(element);
        }

        /// <summary>
        /// Select an option from a dropdown-like element using multiple strategies.
        /// </summary>
        /// <param name="currentPage">Page context.</param>
        /// <param name="elementName">Parent element name key.</param>
        /// <param name="elementType">Parent element type string.</param>
        /// <param name="selecting">Text to select.</param>
        /// <param name="topOptionAlreadySelected">Whether top option is already selected (affects navigation).
        /// </param>
        /// <param name="textEntry">If true, try typing into the control to narrow options.
        /// </param>
        /// <param name="timeout">Optional timeout for locating dropdown options.
        /// </param>
        /// <returns>True if selection succeeded, false otherwise.</returns>
        public static bool SelectingFrom(FormBase currentPage, string elementName, string elementType, string selecting, bool topOptionAlreadySelected = false, bool textEntry = true, int timeout = 0)
        {
            // Open dropdown if needed, try text entry, look through option locators and fall back to generic searches.
            DebugOutput.OutputMethod("SelectingFrom", $"{currentPage.Name}, {elementName}, {elementType}, {selecting}, {topOptionAlreadySelected}, {textEntry}, {timeout}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            bool alreadyOpen = false;
            foreach (var locator in GetSubLocators.DropDownItemLocators)
            {
                DebugOutput.Log($"11 Checking if DROPDOWN is already open with locator {locator}");
                var allOptions = SeleniumUtil.GetElementsUnder(element, locator, 5);
                if (allOptions.Count > 0) alreadyOpen = true;
            }
            if (!alreadyOpen)
            {
                if (!SeleniumUtil.Click(element)) return Failure($"It is not open, so we click - but failed!");
                Thread.Sleep(250);
            }
            if (textEntry)
            {
                if (SeleniumUtil.EnterText(element, selecting))
                {
                    return SeleniumUtil.SendKey(element, "enter");
                }
            }
            var howManyWaysToFindOptions = GetSubLocators.DropDownItemLocators.Length;
            DebugOutput.Log($"There are {howManyWaysToFindOptions} ways to find options! Lets look through them!");
            foreach (var locator in GetSubLocators.DropDownItemLocators)
            {
                DebugOutput.Log($"22 Checking if DROPDOWN is already open with locator {locator}");
                if (locator == By.TagName("mat-option"))
                {
                    DebugOutput.Log($"mat-option - is not related to the dropdown!");
                    // lets get all the mat-options on the page
                    var allMatOptions = SeleniumUtil.GetElements(locator);
                    if (allMatOptions == null) continue;
                    if (allMatOptions.Count() == 0 || allMatOptions == null)
                    {
                        DebugOutput.Log($"No mat-options found on the page!");
                        continue;
                    }
                    DebugOutput.Log($"There are {allMatOptions.Count()} mat-options on the page!");
                    foreach (var matOption in allMatOptions)
                    {
                        var text = SeleniumUtil.GetElementText(matOption);
                        if (selecting == text)
                        {
                            DebugOutput.Log($"Match on TEXT for mat-option");
                            if (SeleniumUtil.Click(matOption)) return true;
                        }
                    }
                }
                var allOptions = SeleniumUtil.GetElementsUnder(element, locator, 5);
                DebugOutput.Log($"There are {allOptions.Count()} options under this dropdown! {locator}");
                if (allOptions.Count > 0)
                {
                    int optionCounter = 0;
                    if (!topOptionAlreadySelected)
                    {
                        optionCounter = 1;
                    }
                    bool found = false;
                    foreach (var option in allOptions)
                    {
                        var text = SeleniumUtil.GetElementAttributeValue(option, "Name");
                        if (selecting == text)
                        {
                            DebugOutput.Log($"Match on NAME");
                            found = true;
                            if (SeleniumUtil.Click(option)) return true;
                        }
                        text = SeleniumUtil.GetElementText(option);
                        if (selecting == text)
                        {
                            DebugOutput.Log($"Match on TEXT");
                            found = true;
                            if (SeleniumUtil.Click(option)) return true;
                        }
                        if (found)
                        {
                            DebugOutput.Log($"We have a match at option {optionCounter} but failed to click on it (sometimes the option is displayed behind 1 element)");
                            for (int i = 0; i < optionCounter; i++)
                            {
                                SeleniumUtil.SendKey(element, "down arrow");
                                Thread.Sleep(100);
                            }
                            return SeleniumUtil.SendKey(element, "enter");
                        }
                        optionCounter++;
                    }
                }
                else
                {
                    DebugOutput.Log($"no options to select for locator {locator}!");
                }
            }
            By subLocator = By.XPath($"//*[contains(., '{selecting}')]");
            var allElements = SeleniumUtil.GetWebElementsUnder(element, subLocator);
            if (allElements.Count > 0)
            {
                DebugOutput.Log($"WE have something! {allElements.Count}");
                return SeleniumUtil.Click(allElements[allElements.Count-1]);
            }
            allElements = SeleniumUtil.GetAllElementsUnderElement(element);
            DebugOutput.Log($"FOUND {allElements.Count()} elements under the dropdown!");
            foreach(var newElement in allElements)
            {
                var newElementName = SeleniumUtil.GetElementAttributeValue(newElement, "Name");
                DebugOutput.Log($"NAME = {newElementName}");
                if (elementName == selecting)
                {
                    DebugOutput.Log($"Found by name");
                    return SeleniumUtil.Click(newElement);
                }
            }
            DebugOutput.Log($"The option {selecting} was simply not found!");
            return false;
        }

        /// <summary>
        /// Set the browser window size using a size string (e.g., "1024x768").
        /// </summary>
        public static bool SetWindowSize(string size)
        {
            // Delegate to Selenium util to set window size using string.
            return SeleniumUtil.SetWindowSize(size);
        } 

        /// <summary>
        /// Set the browser window size with specific dimensions.
        /// </summary>
        public static bool SetWindowSize(int length, int height)
        {
            DebugOutput.OutputMethod("SetWindowSize", $"{length}, {height}");         
            return SeleniumUtil.SetWindowSize(length, height);
        }

        /// <summary>
        /// Switch focus to a different tab by number.
        /// </summary>
        public static bool SwapTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod("SwapTabByNumber", $"{tabNumber}"); 
            return  SeleniumUtil.SwapTabByNumber(tabNumber);
        }

        /// <summary>
        /// Wait for an element to be present and displayed within a timeout.
        /// </summary>
        public static bool WaitForElementToBeDisplayed(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            DebugOutput.OutputMethod($"WaitForElementToBeDisplayed", $"{currentPage.Name} {elementName} {elementType} {timeout} ");
            if (timeout == 0) timeout = TargetConfiguration.Configuration.PositiveTimeout;
            var element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
            int counter = 0;
            if (element == null)
            {
                while (element == null)
                {
                    if (counter > timeout) return Failure($"Failed, even after waiting {timeout} seconds - no element {elementName} let alone displayed");
                    Thread.Sleep(1000);
                    element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
                    counter++;
                }
            }
            if (SeleniumUtil.IsDisplayed(element)) return true;
            while (!SeleniumUtil.IsDisplayed(element))
            {
                if (counter > timeout) return Failure($"Got the element for {elementName} but its still not displayed! even after {counter} seconds");
                Thread.Sleep(1000);
                counter++;
            }
            return true;
        }

        /// <summary>
        /// Wait for an element to not be displayed (disappear) within a timeout.
        /// </summary>
        public static bool WaitForElementToNotBeDisplayed(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
            if (timeout == 0) timeout = TargetConfiguration.Configuration.NegativeTimeout;
            var element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
            if (element == null)
            {
                Thread.Sleep(1000);
                element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
                if (element == null)
                {
                    DebugOutput.Log($"We waited 1 seconds - and still no element evenfound!");
                    return true;
                }
            }
            int counter = 0;
            while (SeleniumUtil.IsDisplayed(element))
            {
                if (counter > timeout) return Failure($"The element {elementName} is still displayed, even after {timeout} seconds");
                Thread.Sleep(1000);
                counter++;
            }
            DebugOutput.Log($"Its gone!");
            return true;
        }

        /// <summary>
        /// Wait until the text of an element stops changing and does not contain the generation string.
        /// </summary>
        public static bool WaitForTextNotToChange(FormBase currentPage, string elementName, string elementType, string generationString = "", int waitTime = 0)
        {
            if (waitTime == 0) waitTime = TargetConfiguration.Configuration.PositiveTimeout;
            DebugOutput.OutputMethod("WaitForTextNotToChange", $"{currentPage.Name} {elementName} {elementType} {generationString} {waitTime}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"We failed even findind the element!");
            return WaitForTextNotToChange(element, generationString, waitTime);
        }

        /// <summary>
        /// Wait until the text of a given IWebElement stops changing and does not contain the generation string.
        /// </summary>
        public static bool WaitForTextNotToChange(IWebElement element, string generationString = "", int waitTime = 0)
        {
            if (waitTime == 0) waitTime = TargetConfiguration.Configuration.PositiveTimeout;
            DebugOutput.OutputMethod("WaitForTextNotToChange", $"{element} {generationString} {waitTime}");
            var answerText = SeleniumUtil.GetElementText(element);
            int timeoutCounter = 0;
            while (answerText == null)
            {
                if (timeoutCounter >= waitTime) return Failure($"Timed out waiting with null!");
                Thread.Sleep(1000);
                answerText = SeleniumUtil.GetElementtextDirect(element);
                timeoutCounter++;
            }
            while (answerText?.Contains(generationString) == true)
            {
                if (timeoutCounter >= waitTime) return Failure($"Timed out waiting with {generationString} text displayed {answerText}!");
                Thread.Sleep(1000);
                answerText = SeleniumUtil.GetElementtextDirect(element);
                timeoutCounter++;
            }
            var tempAnswer = answerText;
            Thread.Sleep(1000);
            answerText = SeleniumUtil.GetElementtextDirect(element);
            timeoutCounter = 0;
            while (tempAnswer != answerText)
            {
                if (timeoutCounter >= waitTime) return Failure($"Timed out waiting STILL CHANGING {tempAnswer} {answerText}!");
                tempAnswer = answerText;
                Thread.Sleep(1000);
                answerText = SeleniumUtil.GetElementtextDirect(element);
                if (tempAnswer == answerText) return true;
                timeoutCounter++;
            }
            DebugOutput.Log($"It never changed in 1 second");
            return true;
        }

        /// <summary>
        /// Wait for the text of the last element in a collection to stop changing.
        /// </summary>
        public static bool WaitForTextNotToChangeInLastElement(FormBase currentPage, string elementName, string elementType, string generationString = "", int waitTime = 0)
        {
            if (waitTime == 0) waitTime = TargetConfiguration.Configuration.PositiveTimeout;
            DebugOutput.OutputMethod("WaitForTextNotToChangeInLastElement", $"{currentPage.Name} {elementName} {elementType} {generationString} {waitTime}");
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType, waitTime);
            if (elements == null) return Failure($"No elments found with {currentPage.Name} {elementName} {elementType}");
            return WaitForTextNotToChange(elements[elements.Count() - 1]);
        }

        // ------------------ Failure handling helpers ------------------
        // Centralized failure reporting methods that return typed defaults

        /// <summary>
        /// Send a failure with a boolean
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pass"></param>
        /// <returns>false is a fail</returns>
        private static bool Failure(string message, bool pass = false)
        {
            CombinedSteps.Failure("***FAILURE IN ElementInteraction*** > " + message);
            return pass;
        }

        /// <summary>
        /// Send a failure with a string
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pass"></param>
        /// <returns>null is a fail</returns>
        private static string? FailureString(string message, string? pass = null)
        {
            CombinedSteps.Failure("***FAILURE IN ElementInteraction FailureString*** > " + message);
            return pass;
        }

        /// <summary>
        /// Send a failure with a number
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pass"></param>
        /// <returns>null is a fail</returns>
        private static int? FailureInt(string message, int? pass = null)
        {
            CombinedSteps.Failure("***FAILURE IN ElementInteraction FailureInt*** > " + message);
            return pass;
        }

        /// <summary>
        /// Send a failure with an empty list
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pass"></param>
        /// <returns>null is a fail</returns>
        private static List<string>? FailureListString(string message, List<string>? pass = null)
        {
            CombinedSteps.Failure("***FAILURE IN ElementInteraction FailureListString*** > " + message);
            if (pass == null) return null;
            if (pass.Count() == 0) return null;
            return pass;
        }
    }
}
