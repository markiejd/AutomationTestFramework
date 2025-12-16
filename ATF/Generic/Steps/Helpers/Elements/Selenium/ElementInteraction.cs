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

        public static bool AlertIsDisplayed(string alertMessage)
        {
            return SeleniumUtil.AlertDisplayed(alertMessage);
        }

        public static bool AlertClickAccept()
        {
            return SeleniumUtil.AlertClickAccept();
        }

        public static bool AlertClickCancel()
        {
            return SeleniumUtil.AlertClickCancel();
        }

        public static bool AlertSendKeys(string text)
        {
            return SeleniumUtil.AlertInput(text);
        }

        // ------------------ Click / Interaction helpers ------------------
        // Group: methods that perform clicks, selections, dragging, navigation

        public static bool ClearTextFromElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("ClearTextFromElement", $"{currentPage.Name}, {elementName}, {elementType}");
            return EnterTextAndKeyIntoElement(currentPage, elementName, elementType, "", "clear");
        }

        public static bool ClearTextThenEnterTextToElement(FormBase currentPage, string elementName, string elementType, string text, string key = "")
        {
            DebugOutput.OutputMethod("ClearTextThenEnterTextToElement", $"{currentPage.Name}, {elementName}, {elementType}");
            if (!ClearTextFromElement(currentPage, elementName, elementType)) return Failure($"Failed to clear element");
            return EnterTextAndKeyIntoElement(currentPage, elementName, elementType, text, key);
        }

        public static bool ClickBackButtonInBrowser()
        {
            DebugOutput.OutputMethod("ClickBackButtonInBrowser", $"");
            return SeleniumUtil.ClickBackButtonInBrowser();
        }

        public static bool ClickOnCooClickCoordinates(int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod("ClickOnCooClickCoordinates", $" {x} {y}");
            return SeleniumUtil.ClickCoordinates(x, y);
        }

        public static bool ClickOnElement(FormBase currentPage, string elementName, string elementType, int x = 0, int y = 0)
        {
            DebugOutput.OutputMethod("ClickOnElement", $"{currentPage.Name}, {elementName}, {elementType} {x} {y}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            return SeleniumUtil.ClickCoordinatesWithElement(element, x, y);
        }

        public static bool ClickOnElement_CheckBoxSub(FormBase currentPage, string elementName, string elementType)
        {
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

        public static bool ClickOnElementEnterTextSendKey(FormBase currentPage, string elementName, string elementType, string text, string key)
        {
            DebugOutput.OutputMethod("ClickOnElementCheckBoxSub", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            if (!SeleniumUtil.Click(element)) return Failure($"Failed to click on the top element!");
            if (!SeleniumUtil.EnterText(element, text)) return Failure($"Clicked - but can not enter text");
            if (!SeleniumUtil.SendKey(element, "enter")) return Failure($"Clicked, sent text - but its the send key I fail at!");
            return true;
        }

        public static bool ClickOnGroupElement(FormBase currentPage, string elementName, string elementType, string groupName)
        {
            DebugOutput.OutputMethod("ClickOnGroupElement", $"{currentPage.Name}, {elementName}, {elementType} {groupName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var groupElement = GetTheElement.GetGroup(element, groupName);
            if (groupElement == null) return Failure($"Failed to get the group element under element!");
            return SeleniumUtil.Click(groupElement);
        }

        public static bool ClickOnSelectYearPickerOfDatePicker(FormBase currentPage, string elementName, string elementType, DateTime date)
        {
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

        public static bool ClickOnSubElementByTagSubElementByClassByTextByTag(FormBase currentPage, string elementName, string elementType, string text, int timeout = 0)
        {
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

        public static bool ClickOnSubSubElementByNumberUnderElement(FormBase currentPage, string elementName, string elementType, string subElementType, int number)
        {
            DebugOutput.OutputMethod("ClickOnSubElementByTextUnderElement", $"{currentPage.Name}, {elementName}, {elementType}, {subElementType}, {number}");
            var subSubElement = GetTheSubSubElementsOfElement(currentPage, elementName, elementType, subElementType);
            if (subSubElement == null) return Failure($"Failed to get sub sub element!");
            if (number > subSubElement.Count) return Failure($"Can not click on the {number} sub sub element, when there is only {subSubElement.Count}");
            number--;
            return SeleniumUtil.Click(subSubElement[number]);
        }

        public static bool ClickOnSubElementByTextUnderElement(FormBase currentPage, string elementName, string elementType, string subElementText)
        {
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

        public static bool ClickOnSubElementByTypeUnderElement(FormBase currentPage, string elementName, string elementType, string subElementText, string subElementType)
        {
            DebugOutput.OutputMethod("ClickOnSubElementByTypeUnderElement", $"{currentPage.Name}, {elementName}, {elementType}, {subElementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            var elements = GetSubElementsOfElement(element, "", subElementType);
            if (elements == null) return Failure($"No sub elements found!");
            return SeleniumUtil.Click(elements[0]);
        }

        public static bool ClickOnTagElementUnderElement(FormBase currentPage, string elementName, string elementType, string tag)
        {
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

        public static bool ClickOnTextInTagInElement(FormBase currentPage, string elementName, string elementType, string tag, string item)
        {
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

        public static bool ClickNthElement(FormBase currentPage, string elementName, string elementType, string whichElement)
        {
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

        public static bool CloseTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod("CloseTabByNumber", $"{tabNumber}");
            return SeleniumUtil.CloseTabByNumber(tabNumber);
        }

        public static void CloseWebBrowser()
        {
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

        public static bool CreateSelfHealModel(FormBase currentPage, IWebElement element, string elementName, string elementType)
        {
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

        public static bool DoubleClickOnElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("ClickOnElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so can not click on it!");
            return SeleniumUtil.DoubleClick(element);
        }

        public static bool DragElementAToElementB(FormBase currentPage, string element1Name, string element2Name, string elementType)
        {
            DebugOutput.OutputMethod("DragAToB", $"{currentPage.Name}, {element1Name}, {element2Name}, {elementType}");
            var element1 = GetTheElement.GetElement(currentPage, element1Name, elementType);
            if (element1 == null) return Failure($"Failed to find 1 {element1Name}");
            var element2 = GetTheElement.GetElement(currentPage, element2Name, elementType);
            if (element2 == null) return Failure($"Failed to find 2 {element2Name}");
            return SeleniumUtil.DragElementToElement(element1, element2);
        }

        public static bool EnterTextAndKeyIntoElement(FormBase currentPage, string elementName, string elementType, string text, string key = "")
        {
            DebugOutput.OutputMethod("EnterTextAndKeyIntoElement", $"{currentPage.Name}, {elementName}, {elementType} {text} {key}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            return SeleniumUtil.EnterText(element, text, key);
        }

        // ------------------ Getters / Query helpers ------------------
        // Group: methods that retrieve text, attributes, counts, selection

        public static string? GetAttributeValueOfSubElementByNameOfElement(FormBase currentPage, string elementName, string elementType, string subElementName, string attribute)
        {
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

        public static string? GetLabelFromElement(FormBase currentPage, string elementName, string elementType, By labelLocator)
        {
            DebugOutput.OutputMethod("GetLabelFromElement", $"{currentPage.Name}, {elementName}, {elementType} {labelLocator}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureString($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            var labelElement = SeleniumUtil.GetElementUnderElement(element, labelLocator, 0);
            if (labelElement == null) return FailureString($"Failed to find the parent or its label! by {labelLocator}");
            return SeleniumUtil.GetElementText(labelElement);
        }

        public static string? GetPlaceholderText(FormBase currentPage, string elementName, string elementType)
        {
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

        public static int? GetTheNumberOfTabsOpenInBrowser()
        {
            DebugOutput.OutputMethod("GetNumberOfTabsOpenInBrowser", $"");
            return SeleniumUtil.GetNumberOfTabsOpenInBrowser();
        }

        public static string? GetSelectionValue(FormBase currentPage, string elementName, string elementType)
        {
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

        public static List<string>? GetSelectionValues(FormBase currentPage, string elementName, string elementType)
        {
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

        private static int? GetSubElementCountFromElement(IWebElement element, string elementType, string name)
        {
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

        public static string? GetSubElementTextFromNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth)
        {
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

        public static string? GetTextFromElement(FormBase currentPage, string elementName, string elementType)
        {
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

        public static string? GetTextFromLastElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("GetTextFromLastElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType, 0);
            if (elements == null) return FailureString($"Never found ANY elements using {elementName} of {elementType} in page {currentPage}");
            return SeleniumUtil.GetElementText(elements[elements.Count() - 1]);
        }

        public static string? GetTextFromNthElement(FormBase currentPage, string elementName, string elementType, int nth)
        {
            DebugOutput.OutputMethod("GetTextFromNthElement", $"{currentPage.Name}, {elementName}, {elementType} {nth}");
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType, 0);
            if (elements == null) return FailureString($"Never found ANY elements using {elementName} of {elementType} in page {currentPage}");
            if (elements.Count() < nth) return FailureString($"Your looking for the {nth}th element, but I only found {elements.Count()}");
            if (nth < 1) return FailureString($"0 is fine for an array, but humans count 1, 2, 3...");
            nth--;  // becuase it IS an array
            return SeleniumUtil.GetElementText(elements[nth]);
        }

        public static bool GetScreenShotOfElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("GetScreenshotOfElement", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"Failed to take screen shot {currentPage.Name} {elementName}  {elementType}");
            // if (!SeleniumUtil.ScrollToElement(element)) return Failure($"Failed to SCROLL to the element! {elementName}");
            Thread.Sleep(200);
            return SeleniumUtil.ScreenShotElement(element, elementName);
        }

        public static bool GetScreenShotOfPage(FormBase currentPage)
        {
            DebugOutput.OutputMethod("GetScreenShotOfPage", $"{currentPage.Name}");
            return SeleniumUtil.SaveCurrentPageImage(currentPage.Name);
        }

        public static bool GetErrorScreenShotOfPage(string errorMessage)
        {
            DebugOutput.OutputMethod("GetErrorScreenShotOfPage", $"{errorMessage}");
            return SeleniumUtil.GetCurrentPageScreenShot(errorMessage) ?? false;
        }

        private static List<IWebElement>? GetSubElementOfElement(IWebElement element, By[] locators)
        {
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

        public static List<string>? GetSubElementsTextOfElement(FormBase currentPage, string elementName, string elementType)
        {
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

        private static List<IWebElement>? GetSubElementsOfElement(IWebElement element, string elementType, string subType = "")
        {
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

        private static List<IWebElement>? GetSubElementsOfElement(IWebElement element, By[] locators)
        {
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

        private static List<IWebElement>? GetSubSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
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

        public static string? GetTextFromSubElementSelectedOfElement(FormBase currentPage, string elementName, string elementType)
        {
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

        public static List<string>? GetTextListFromSubSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0, string attribute = "")
        {
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

        private static List<IWebElement>? GetTheListOfSubElementsofNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth = 0)
        {
            var element = GetTheNthElement(currentPage, elementName, elementType);
            if (element == null) return null;

            var sublocator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (sublocator == null) return null;

            return SeleniumUtil.GetElementsUnder(element, sublocator);
        }

        private static IWebElement? GetTheNthElement(FormBase currentPage, string elementName, string elementType, int nth = 0)
        {
            var elements = GetTheElements.GetElements(currentPage, elementName, elementType);
            if (elements == null) return null;
            var numberOfElements = elements.Count();
            if (nth == 0) return elements[numberOfElements - 1];
            nth--;
            return elements[numberOfElements - nth];
        }

        public static int? GetTheNumberOfSubElementsOfElement(FormBase currentPage, string elementName, string elementType)
        {
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return null;
            var subElements = GetSubElementsOfElement(element, elementType);
            if (subElements == null) return null;
            return subElements.Count();
        }

        public static int? GetTheNumberOfSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth = 0)
        {
            var listOfElements = GetTheListOfSubElementsofNthElement(currentPage, elementName, elementType, subElementName, subElementType, nth);
            if (listOfElements == null) return FailureInt($"Failed to get list of elements!");
            return listOfElements.Count();
        }

        public static int? GetTheNumberOfSubSubElementsOfElement(FormBase currentPage, string elementName, string elementType, string subType)
        {
            var subSubElement = GetTheSubSubElementsOfElement(currentPage, elementName, elementType, subType);
            if (subSubElement == null) return FailureInt($"Failed to find any sub sub element!");
            return subSubElement.Count;
        }

        public static int? GetTheNumberOfSubSubElementsOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
            var listOfElements = GetTheListOfSubSubElementsofNthElement(currentPage, elementName, elementType, subElementName, subElementType, subSubElementName, subSubElementType, nth);
            if (listOfElements == null) return FailureInt($"Failed to get list of elements!");
            return listOfElements.Count();
        }

        private static IWebElement? GetTheSubElementOfNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, int nth = 0)
        {
            var element = GetTheNthElement(currentPage, elementName, elementType);
            if (element == null) return null;

            var locator = GetDictionaryLocator.GetElementLocator(subElementName, currentPage, subElementType);
            if (locator == null) return null;

            return SeleniumUtil.GetElementUnderElement(element, locator);
        }

        private static List<IWebElement>? GetTheSubSubElementsOfElement(FormBase currentPage, string elementName, string elementType, string subType)
        {
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return null;
            var subElements = GetSubElementsOfElement(element, elementType, subType);
            if (subElements == null) return null;
            if (subElements.Count > 1) return null;
            var subSubElement = GetSubElementsOfElement(subElements[0], elementType);
            if (subSubElement == null) return null;
            return subSubElement;
        }

        private static IWebElement? GetTheSubSubElementofNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
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

        private static List<IWebElement>? GetTheListOfSubSubElementsofNthElement(FormBase currentPage, string elementName, string elementType, string subElementName, string subElementType, string subSubElementName, string subSubElementType, int nth = 0)
        {
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

        public static int? GetWidthOfElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("GetText", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return FailureInt($"Failed to get element! {elementName} of type {elementType} in page {currentPage.Name}");
            return SeleniumUtil.GetWidthOfElement(element);
        }

        public static bool IsElementDisplayed(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
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

        public static bool IsElementExpanded(FormBase currentPage, string elementName, string elementType)
        {
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

        public static bool IsElementExists(FormBase currentPage, string elementName, string elementType, int timeout)
        {
            DebugOutput.OutputMethod("IsElementDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {timeout}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType, timeout);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            return true;
        }

        public static bool IsElementGroupDisplayed(FormBase currentPage, string elementName, string elementType, string groupName)
        {
            DebugOutput.OutputMethod("IsElementGroupDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {groupName}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its display status");
            var groupElement = GetTheElement.GetGroup(element, groupName);
            if (groupElement == null) return Failure($"Failed to get the group element under element!");
            return SeleniumUtil.IsDisplayed(groupElement);
        }

        public static bool IsElementGroupExpanded(FormBase currentPage, string elementName, string elementType, string groupName)
        {            
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

        public static bool IsElementNotDisplayed(FormBase currentPage, string elementName, string elementType, int timeout = 0)
        {
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

        public static bool IsElementEnabled(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("IsEnabled", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know if enabled or not!");
            return SeleniumUtil.IsEnabled(element);
        }

        public static bool IsElementGroupNotExpanded(FormBase currentPage, string elementName, string elementType, string groupName)
        {
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

        public static bool IsElementReadOnly(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("IsReadOnly", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know if enabled or not!");
            return !SeleniumUtil.IsEnabled(element);
        }

        public static bool IsElementSelected(FormBase currentPage, string elementName, string elementType)
        {
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

        public static bool IsElementsParentElementDisplayed(FormBase currentPage, string elementName, string elementType)
        {
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

        public static bool IsElementUnderElementByTagByTextDisplayed(FormBase currentPage, string elementName, string elementType, string tag, string text)
        {
            DebugOutput.OutputMethod("IsElementUnderElementByTagDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {tag} {text}");
            var locator = By.TagName(tag);
            return IsElementUnderElementByLocatorByTextDisplayed(currentPage, elementName, elementType, locator, text);
        }

        public static bool IsElementUnderElementByClassByTextDisplayed(FormBase currentPage, string elementName, string elementType, string classWanted, string text)
        {
            DebugOutput.OutputMethod("IsElementUnderElementByTagDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {classWanted} {text}");
            var locator = By.ClassName(classWanted);
            return IsElementUnderElementByLocatorByTextDisplayed(currentPage, elementName, elementType, locator, text);
        }        

        public static bool IsElementUnderElementByLocatorByTextDisplayed(FormBase currentPage, string elementName, string elementType, By locator, string text)
        {
            DebugOutput.OutputMethod("IsElementUnderElementByTagDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {locator} {text}");
            var subElement = GetTheElement.GetSubElementByText(currentPage, elementName, elementType, locator, text, 0);
            if (subElement == null) return Failure($"Failed to find the sub element by {locator} with text {text} under {elementName}");
            return SeleniumUtil.IsDisplayed(subElement);
        }

        public static bool IsElementUnderElementByTextDisplayed(FormBase currentPage, string elementName, string elementType, string subElementText)
        {
            DebugOutput.OutputMethod("IsElementUnderElementByTextDisplayed", $"{currentPage.Name}, {elementName}, {elementType} {subElementText}");
            By locator = By.XPath($".//*[text()='{subElementText}']");
            var subElement= GetTheElement.GetSubElementByText(currentPage, elementName, elementType, locator, subElementText, 0);
            if (subElement == null) return Failure($"Failed to find the sub element!");
            return SeleniumUtil.IsDisplayed(subElement);
        }

        public static bool IsDriverActive()
        {
            DebugOutput.OutputMethod("IsDriverNull", $"");
            if (SeleniumUtil.webDriver == null) return false;
            return true;            
        }

        public static bool IsSubElementDisplayed(FormBase currentPage, string elementName, string elementType, string subElementName)
        {
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

        public static bool MouseOverElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("MouseOver", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            return SeleniumUtil.MoveToElement(element);
        }

        public static bool MoveSliderElement(FormBase currentPage, string elementName, string elementType, int value)
        {
            DebugOutput.OutputMethod("MoveSliderElement", $"{currentPage.Name}, {elementName}, {elementType} {value}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            return SeleniumUtil.MoveSliderElement(element, value);
        }

        public static bool NavigateToURL(string url)
        {
            DebugOutput.OutputMethod("NavigateToURL", $"{url}");
            return SeleniumUtil.NavigateToURL(url);
        }

        public static bool RightClickElement(FormBase currentPage, string elementName, string elementType)
        {
            DebugOutput.OutputMethod("RightClick", $"{currentPage.Name}, {elementName}, {elementType}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"No element found called {elementName} of type {elementType} in page {currentPage} so do not know its selected or not!");
            return SeleniumUtil.RightClick(element);
        }

        public static bool SelectingFrom(FormBase currentPage, string elementName, string elementType, string selecting, bool topOptionAlreadySelected = false, bool textEntry = true, int timeout = 0)
        {
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

        public static bool SetWindowSize(string size)
        {
            return SeleniumUtil.SetWindowSize(size);
        } 

        public static bool SetWindowSize(int length, int height)
        {
            DebugOutput.OutputMethod("SetWindowSize", $"{length}, {height}");         
            return SeleniumUtil.SetWindowSize(length, height);
        }

        public static bool SwapTabByNumber(int tabNumber)
        {
            DebugOutput.OutputMethod("SwapTabByNumber", $"{tabNumber}"); 
            return  SeleniumUtil.SwapTabByNumber(tabNumber);
        }

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

        public static bool WaitForTextNotToChange(FormBase currentPage, string elementName, string elementType, string generationString = "", int waitTime = 0)
        {
            if (waitTime == 0) waitTime = TargetConfiguration.Configuration.PositiveTimeout;
            DebugOutput.OutputMethod("WaitForTextNotToChange", $"{currentPage.Name} {elementName} {elementType} {generationString} {waitTime}");
            var element = GetTheElement.GetElement(currentPage, elementName, elementType);
            if (element == null) return Failure($"We failed even findind the element!");
            return WaitForTextNotToChange(element, generationString, waitTime);
        }

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
