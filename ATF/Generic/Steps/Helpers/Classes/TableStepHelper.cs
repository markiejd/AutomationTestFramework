using System.Diagnostics;
using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using OpenQA.Selenium;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class TableStepHelper : StepHelper, ITableStepHelper
    {
        private readonly ITargetForms targetForms;
        public TableStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        // int versionNumber = 0;

        // int primaryColumnForTable = 1;

        //Everything below this is part of the table
        private readonly int TablePrimaryColumnNumber = TargetLocator.Configuration.TablePrimaryColumnNumber;  //what column can never be empty
        //The header row
        //All the column headers
        private readonly By[] TableHeadLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableHeadLocator);
        //The indivdual header cells
        private readonly By[] TableHeadCellsLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableHeadCellsLocator);

        //The data rows
        //All the data rows
        private readonly By[] TableBodyLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyLocator);
        //The individual row 
        private readonly By[] TableBodyRowLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyRowLocator);
        //The elment under a individual row that contains the highlighter
        private readonly By[] TableBodyRowSubLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyRowSubLocator);
        //The Cells in the row
        private readonly By[] TableBodyCellsLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyCellsLocator);


        private readonly By[] TableNextPageButton = LocatorValues.locatorParser(TargetLocator.Configuration.TableNextPageButton);
        private readonly By[] TablePreviousPageButton = LocatorValues.locatorParser(TargetLocator.Configuration.TablePreviousPageButton);

        private readonly By[] TableFilterLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableFilterLocator);

        private readonly string[] TableActions = TargetLocator.Configuration.TableActions;
        private readonly By[] TableActionsLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableActionsLocator);

        private readonly string[] EmptyTableText = {"No data to display", "Nobody found for the specified search criteria"};

        private readonly By LinkLocator = By.TagName("a");

        private readonly By ButtonLocator = By.TagName("button");

        public bool DoesRowContainAction(string tableName, string action, string columnName, string value)
        {
            DebugOutput.Log($"Proc - ActionRowByValueInColumnName {tableName} {action} {columnName} {value}");
            if (!TableActions.Contains(action.ToLower()))
            {
                DebugOutput.Log($"We do not know of action {action}");
                return false;
            }
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
            if (columnNumber < 0) return false;
            var rowNumber = GetRowNumberWhereValueIsFoundInColumnNumber(tableElement, columnNumber, value);
            if (rowNumber < 0) return false;
            DebugOutput.Log($"Found row number {rowNumber}");
            var allColumnElements = GetAllRows(tableElement);
            var rowElement = allColumnElements[rowNumber - 1];
            DebugOutput.Log($"Find the locator for action");
            int counter = 0;
            By locator;
            foreach (var doAction in TableActions)
            {
                if (doAction == action)
                {
                    locator = TableActionsLocator[counter];
                    var element = SeleniumUtil.GetElementUnderElement(rowElement, locator, 1);
                    if (element != null)
                    {
                        DebugOutput.Log($"");
                        return element.Displayed;
                    }
                    By test = By.ClassName("RadButton");
                    var allElements = SeleniumUtil.GetElementsUnder(rowElement, test, 1);
                    DebugOutput.Log($"That is {allElements.Count} ELEMENT FOUND under ROW {rowElement}");
                    foreach (var eachElement in allElements)
                    {
                        var actionWord = TableActions[counter];
                        var name = eachElement.GetAttribute("Name");
                        DebugOutput.Log(eachElement + " " + name);
                        if (name.ToLower() == actionWord)
                        {
                            DebugOutput.Log($"MATCH!");
                            return eachElement.Displayed;
                        }
                    }
                    return false;
                }
                counter++;
            }
            DebugOutput.Log($"Failed badly here!");
            return false;
        }

        public bool DoesRowColumnNumberContainsLink(string tableName, int rowNumber, int columnNumber)
        {
            DebugOutput.Log($"DoesRowColumnNumberContainsLink {tableName} {rowNumber} {columnNumber} ");
            var cellElement = GetCellElement(tableName, rowNumber, columnNumber);
            if (cellElement == null) return false;
            if (GetCellLink(cellElement) != null)
            {
                DebugOutput.Log($"There is a link element in this table at {rowNumber} x {columnNumber}");
                return true;
            }
            DebugOutput.Log($"Failed to find a link!");
            return false;
        }

        private IWebElement? GetCellLink(IWebElement cellElement)
        {
            DebugOutput.Log($"GetCellLink {cellElement}");
            var linkElement = SeleniumUtil.GetElementUnderElement(cellElement, LinkLocator, 1);
            if (linkElement == null)
            {
                DebugOutput.Log($"Failed to find a link element using {LinkLocator} ");
                return null;
            }
            return linkElement;
        }

        private IWebElement? GetCellButton(IWebElement cellElement)
        {
            DebugOutput.Log($"GetCellButton {cellElement}");
            var buttonElement = SeleniumUtil.GetElementUnderElement(cellElement, ButtonLocator, 1);
            if (buttonElement == null)
            {
                DebugOutput.Log($"Failed to find a button element using {ButtonLocator} ");
                return null;
            }
            return buttonElement;
        }

        private IWebElement? GetCellElement(string tableName, int rowNumber, int columnNumber)
        {
            DebugOutput.Log($"GetCellElement {tableName} {rowNumber} {columnNumber} ");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return null;
            var rowElements = GetAllVisibleRows(tableElement);
            if (rowElements.Count < 1) return null;
            DebugOutput.Log($"We have {rowElements.Count} rows in this table");
            int computerRowNumber = rowNumber - 1;
            var allCellElementsInARow = GetAllCellsInRow(rowElements[computerRowNumber]);
            DebugOutput.Log($"We have {allCellElementsInARow} cells in this row");
            int computerColumnNumber = columnNumber - 1;
            DebugOutput.Log($"We are taking cell {computerColumnNumber} from the row,  remember that I count from 0!");
            var cellElement = allCellElementsInARow[computerColumnNumber];
            return cellElement;
        }

        public bool ActionRowByValueInColumnName(string tableName, string action, string columnName, string value)
        {
            DebugOutput.Log($"ActionRowByValueInColumnName {tableName} {action} {columnName} {value}");
            value = StringValues.TextReplacementService(value);
            DebugOutput.Log($"Get table");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            DebugOutput.Log($"Get colum number");
            var columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
            if (columnNumber < 0) return false;
            DebugOutput.Log($"We are looking in column {columnNumber}");
            var allRows = GetAllRows(tableElement);
            DebugOutput.Log($"We have all these rows {allRows.Count()}");
            var rowNumber = GetRowNumberWhereValueIsFoundInColumnNumber(allRows, columnNumber, value);
            DebugOutput.Log($"Found in row number {rowNumber}");
            var rowElement = allRows[rowNumber - 1];
            return DoActionInRow(rowElement, action);
        }

        public bool ActionRowByNumber(string tableName, string action, int rowNumber)
        {
            DebugOutput.Log($"ActionRowByNumber {tableName} {action} {rowNumber}");
            DebugOutput.Log($"We take 1 away from our row number as our element list starts at 0 => {rowNumber}");
            rowNumber--;
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var allRows = GetAllRows(tableElement);
            DebugOutput.Log($"We have {allRows.Count()} rows returned! we want element number {rowNumber}");
            var rowElement = allRows[rowNumber];
            return DoActionInRow(rowElement, action);
        }

        private bool DoActionInRow(IWebElement rowElement, string action)
        {
            DebugOutput.Log($"DoActionInRow {rowElement} {action} ");
            int counter = 0;
            foreach (var doAction in TableActions)
            {
                if (doAction.ToLower() == action.ToLower())
                {
                    DebugOutput.Log($"We have the action '{doAction.ToLower()}'");
                    By locator = TableActionsLocator[counter];
                    var element = SeleniumUtil.GetElementUnderElement(rowElement, locator, 1);
                    if (element == null)
                    {
                        By test = By.ClassName("RadButton");
                        var allElements = SeleniumUtil.GetElementsUnder(rowElement, test, 1);
                        DebugOutput.Log($"That is {allElements.Count} ELEMENT FOUND under ROW {rowElement}");
                        int newCounter = 1;
                        foreach (var eachElement in allElements)
                        {
                            var actionWord = TableActions[counter];
                            var name = eachElement.GetAttribute("Name");
                            DebugOutput.Log(newCounter + " " + eachElement + " " + name);
                            if (name.ToLower() == actionWord)
                            {
                                DebugOutput.Log($"MATCH!");
                                return SeleniumUtil.Click(eachElement);
                                //eachElement.Click();
                            }
                            newCounter++;
                        }
                        return false;
                    }
                    return SeleniumUtil.Click(element);
                }
                counter++;
            }
            DebugOutput.Log($"Failed to find action!");
            return false;             
        }

        public bool ClikcOnLinkInTableColumnRow(string tableName, int columnNumber, int rowNumber)
        {
            DebugOutput.Log($"Proc - ClikcOnLinkInTableColumnRow {tableName} {columnNumber} {rowNumber}");
            var cellElement = GetCellElement(tableName, rowNumber, columnNumber);
            if (cellElement == null) return false;
            var linkElement = GetCellLink(cellElement);
            if (linkElement == null) return false;
            return SeleniumUtil.Click(linkElement);
        }

        public bool ClickOnButtonInTableColumnRow(string tableName, int columnNumber, int rowNumber)
        {
            DebugOutput.Log($"Proc - ClickOnButtonInTableColumnRow {tableName} {columnNumber} {rowNumber}");
            var cellElement = GetCellElement(tableName, rowNumber, columnNumber);
            if (cellElement == null) return false;
            var buttonElement = GetCellButton(cellElement);
            if (buttonElement == null) return false;
            return SeleniumUtil.Click(buttonElement);
        }

        public bool ClickOnCheckBoxInTableColumnRow(string tableName, int rowNumber, int columnNumber)
        {
            DebugOutput.Log($"Proc - ClickOnCheckBoxInTableColumnRow {tableName} {columnNumber} {rowNumber}");
            var cellElement = GetCellElement(tableName, rowNumber, columnNumber);
            if (cellElement == null) return false;
            var checkboxElement = SeleniumUtil.GetElementUnderElement(cellElement, By.TagName("input"), 1);
            if (checkboxElement == null) return false;
            return SeleniumUtil.Click(checkboxElement);
        }

        public bool ClickOnRow(int rowNumber, string tableName)
        {
            DebugOutput.Log($"Proc - GetRowNumberWhereValueIsFoundInColumnNumber {tableName} {rowNumber}");
            return ElementInteraction.ClickOnSubSubElementByNumberUnderElement(CurrentPage, tableName, "table", "table.body", rowNumber);
        }

        private int GetRowNumberWhereValueIsFoundInColumnNumber(List<IWebElement> allRowElements, int columnNumber, string value)
        {
            DebugOutput.Log($"Proc - GetRowNumberWhereValueIsFoundInColumnNumber LIST {allRowElements.Count} {columnNumber} {value}");
            int counter = 1;
            foreach (var row in allRowElements)
            {
                var getAllCellsInRow = GetAllCellsInRow(row);
                DebugOutput.Log($"There are {getAllCellsInRow.Count} columns in this row! We only want the {columnNumber} th");
                try
                {
                    var cellInColumn = getAllCellsInRow[columnNumber];
                    var text = SeleniumUtil.GetElementText(cellInColumn);
                    if (text == value)
                    {
                        DebugOutput.Log($"We have a match!");
                        return counter;
                    }
                    else
                    {
                        DebugOutput.Log($"NOT MATCH IN COLUMN {counter} {text} with {value}");
                    }
                }
                catch
                {
                    DebugOutput.Log($"had an issue Getting elmenet text!");
                }
                counter++;
            }
            return -1;

        }

        private int GetRowNumberWhereValueIsFoundInColumnNumber(IWebElement tableElement, int columnNumber, string value)
        {
            DebugOutput.Log($"Proc - GetRowNumberWhereValueIsFoundInColumnNumber {tableElement} {columnNumber} {value}");
            var getAllRows = GetAllRows(tableElement);
            return GetRowNumberWhereValueIsFoundInColumnNumber(getAllRows, columnNumber, value);
        }



        // / <summary>
        // / A cell allowing text to be entered, that filters the table
        // / </summary>
        // / <param name="tableName"></param>
        // / <param name="value"></param>
        // / <returns></returns>
        public bool Filter(string tableName, string value)
        {
            DebugOutput.Log($"Filter {tableName} {value}");
            IWebElement? filterTextBox = null;
            var numberOfPossibleFilters = TableFilterLocator.Count();
            int counter = 0;
            while (counter < numberOfPossibleFilters && filterTextBox is null)
            {
                filterTextBox = SeleniumUtil.GetElement(TableFilterLocator[counter],1);
                counter++;
            }
            if (filterTextBox == null) return false;
            return SeleniumUtil.EnterText(filterTextBox, value);
        }

        // / <summary>
        // / Is the table displayed
        // / </summary>
        // / <param name="tableName"></param>
        // / <returns></returns>
        public bool IsDisplayed(string tableName, int timeoOut = 0)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, tableName, "Table", timeoOut);
        }


        public bool IsColumnExist(string tableName, string columnName)
        {
            DebugOutput.Log($"Proc - IsColumnExist {tableName} {columnName}");
            return IsColumnExistInLocation(tableName, columnName);
        }

        public bool IsColumnExistInLocation(string tableName, string columnName, int count = 0)
        {
            DebugOutput.Log($"Proc - IsColumnExistInLocation {tableName} {columnName} {count}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
            DebugOutput.Log($"We have column number {columnNumber}");
            if (count == -1)
            {
                DebugOutput.Log($"Not found column name {columnName}");
                columnName = columnName.Trim();
                columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
                if (columnNumber == -1) return false;
                DebugOutput.Log($"We have column number {columnNumber} from trimmed column name {columnName}");
            }
            return columnNumber == count;
        }



        // / <summary>
        // / Does the given column contain a value?
        // / </summary>
        // / <param name="tableName"></param>
        // / <param name="columnName"></param>
        // / <param name="value"></param>
        // / <returns></returns>
        public bool IsColumnContainValue(string tableName, string columnName, string value)
        {
            DebugOutput.Log($"Proc - IsColumnContainValue {tableName} {columnName} {value}");
            value = StringValues.TextReplacementService(value);
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            int columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
            if (columnNumber == -1) return false;
            DebugOutput.Log($"Checking in Column {columnNumber}");
            var numberofDisplayedRows = GetNumberOfRowsDisplayed(tableName);
            DebugOutput.Log($"We have the number of displayed rows as {numberofDisplayedRows} ");
            var counter = 1;
            while (counter <= numberofDisplayedRows)
            {
                var text = GetValueOfGridBox(tableName, counter, columnNumber);
                if (text.ToLower().Contains(value.ToLower()))
                {
                    DebugOutput.Log($"IT EXISTS in {columnNumber} in row {counter}");
                    return true;
                }
                counter++;
            }
            DebugOutput.Log($"Cycled - not found!");
            return false;
        }

        public int GetNumberOfRowsTotal(string tableName)
        {
            DebugOutput.Log($"Proc - GetNumberOfRowsTotal {tableName}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return -1;
            // I need to find the parent element
            var parentElement = SeleniumUtil.GetElementParent(tableElement);
            if (parentElement == null) return -1;
            // find the pagination element
            By paginationLocator = By.TagName("mat-paginator");
            var paginationElement = SeleniumUtil.GetElementUnderElement(parentElement, paginationLocator, 1);
            if (paginationElement == null) return -1;
            // find the counter element
            By counterLocator = By.ClassName("mat-mdc-paginator-range-label");
            var counterElement = SeleniumUtil.GetElementUnderElement(paginationElement, counterLocator, 1);
            if (counterElement == null) return -1;
            var counterText = SeleniumUtil.GetElementText(counterElement);
            DebugOutput.Log($"Got counter text {counterText}");
            // the text is something like 1 - 10 of 47999 I want the 47999
            var parts = counterText.Split(" of ");
            if (parts.Length != 2) return -1;
            var totalPart = parts[1].Trim();
            if (int.TryParse(totalPart, out int totalNumber))
            {
                DebugOutput.Log($"Total number of rows is {totalNumber}");
                return totalNumber;
            } 
            return -1;
        }

        public int GetNumberOfRowsDisplayed(string tableName)
        {
            DebugOutput.Log($"Proc - NumberOfRowsDisplayed {tableName}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return -1;

            // how many rows
                // get the body 
            By tableBodyLocator = By.TagName("tbody");
            var tableBodyElement = SeleniumUtil.GetElementUnderElement(tableElement, tableBodyLocator, 1);
            if (tableBodyElement == null) return -1;

            By tableRowLocator = By.TagName("tr");
            var rowElements = SeleniumUtil.GetElementsUnder(tableBodyElement, tableRowLocator);
            DebugOutput.Log($"I have {rowElements.Count} rows found - but it could be a no data available row!");
            if (rowElements.Count == 0) return 0;
            if (rowElements.Count == 1)
            {
                // get all the text for this row element
                var rowText = SeleniumUtil.GetElementText(rowElements[0]);
                if (rowText.Contains("No data available") || EmptyTableText.Any(x => rowText.Contains(x)))
                {
                    DebugOutput.Log($"That row says no data available! so we have a row - but it is an empty row");
                    return 0;
                }
            }
            // how many rows displayed
            var numberofDisplayedRows = 0;
            foreach (var row in rowElements)
            {
                if (row.Displayed)
                {
                    var classAttribute = row.GetAttribute("class");
                    if (classAttribute != null && !classAttribute.Contains("expanded-row"))
                    {
                        numberofDisplayedRows++;
                    }
                } 
            }
            DebugOutput.Log($"of the {rowElements.Count} rows only {numberofDisplayedRows} are displayed");

            // return that value
            return  numberofDisplayedRows;
        }

        public bool ClickOnNextPageInPagination(string tableName, string button = "")
        {
            DebugOutput.Log($"Proc - ClickOnNextPageInPagination {tableName} {button}");
            return ElementInteraction.ClickOnSubElementByTypeUnderElement(CurrentPage, tableName, "table", "", "table.nextpage");
        }

        public int GetNumberOfRowsWithLocatorFound(string tableName, By locator)
        {
            DebugOutput.Log($"Proc - GetNumberOfRowsWithLocatorFound {tableName} {locator}");
            int numberOfRowsContainingLocator = 0;
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return 0;
            var rowElements = GetAllRows(tableElement);
            if (rowElements.Count == 0)
            {
                DebugOutput.Log($"Failed to find ANY rows!");
                return -1;
            }
            foreach (var rowElement in rowElements)
            {
                var elements = SeleniumUtil.GetElementsUnder(rowElement, locator, 1);
                if (elements.Count > 1)
                {
                    DebugOutput.Log($"Have multiple counts of that locator!");
                    return -1;
                }
                if (elements.Count == 1)
                {
                    DebugOutput.Log($"is element {elements[0]} displayed!");
                    if (elements[0].Displayed) numberOfRowsContainingLocator++;
                }
            }
            return numberOfRowsContainingLocator;
        }

        public int GetNumberOfRowsPerPage(string tableName)
        {
            DebugOutput.Log($"Proc - GetNumberOfRowsPerPage {tableName}");
            // // // we need the table
            // // // then the pagnation element
            // // // then the current value

            return -1;

        }

        public int GetNumberOfPopulatedRowsDisplayed(string tableName)
        {
            DebugOutput.Log($"Proc - GetNumberOfPopulatedRowsDisplayed {tableName}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return -1;
            var rowElements = GetAllRowsInBody(tableElement);
            if (rowElements.Count() == 0) return 0;
            DebugOutput.Log($"There are {rowElements.Count} rows displayed");
            int displayedRowCount = 0;
            foreach (var rowElement in rowElements)
            {
                if (rowElement.Displayed)
                {
                    DebugOutput.Log($"Row is displayed");
                    var cellElements = GetAllCellsInRow(rowElement);
                    displayedRowCount++;
                }
                else
                {
                    DebugOutput.Log($"Row is NOT displayed");
                }
            }
            if (displayedRowCount > 0) return displayedRowCount;
            DebugOutput.Log($"That means 0 rows are populated");
            return 0;
        }

        public string GetValueOfGridBoxUsingColumnTitle(string tableName, string columnTitle, int rowNumber)
        {
            DebugOutput.Log($"GetValueOfGridBoxUsingColumnTitle {tableName} {columnTitle} {rowNumber}");
            //row includes headerincudes something?
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return "";
            DebugOutput.Log($"Table element = {tableElement}");
            var columnNumber = GetColumnNumberFromTitle(tableElement, columnTitle);
            if (columnNumber == -1) return "";
            DebugOutput.Log($"We have {columnNumber} as the column number");
            return GetValueOfGridBox(tableName, rowNumber, columnNumber);
        }

        private List<IWebElement> GetAllCellsInRow(IWebElement rowElement)
        {
            DebugOutput.Log($"GetAllCellsInRow {rowElement}");

            var cells = new List<IWebElement>();
            foreach (var locator in TableBodyCellsLocator)
            {
                DebugOutput.Log($"TRYING to get all cells using {locator}");
                cells = SeleniumUtil.GetElementsUnder(rowElement, locator, 1);
                if (cells.Count() > 0) return cells;
            }
            DebugOutput.Log($"FAILED to see anything!");
            return cells;
        }

        public string GetValueOfGridBox(string tableName, int rowNumber, int columnNumber, bool header = false)
        {
            DebugOutput.Log($"GetValueOfGridBox {tableName} {rowNumber} {columnNumber} {header}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return "";
            //Get All The Row Elements 
            var rowElements = GetAllRows(tableElement);
            DebugOutput.Log($"We have {rowElements.Count} rows currently in table!");
            if (rowElements.Count == 0)
            {
                DebugOutput.Log($"Failed to get any rows of tableElement {tableElement}");
                return "";
            }
            var text = "";
            try
            {
                DebugOutput.Log($"Get single row element {rowNumber - 1}");
                var rowElement = rowElements[rowNumber - 1];
                DebugOutput.Log($"Have row element {rowElement}");
                var cellElements = GetAllCellsInRow(rowElement);
                DebugOutput.Log($"Got this number of cells in row {cellElements.Count()} we want column {columnNumber}");
                if (cellElements.Count < 1)
                {
                    DebugOutput.Log($"Failed to get any cells in row {rowElement}");
                    return "";
                }
                var cellElement = cellElements[columnNumber-1];
                DebugOutput.Log($"We have a CELL element {cellElement}");
                DebugOutput.Log($"Getting text of cell");
                text = SeleniumUtil.GetElementText(cellElement);
                DebugOutput.Log($"Returning {text} for row and column {rowNumber} {columnNumber}");
                return text;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Had an issue in GetValueOfGridBox {ex} ");
            }
            return text;
        }


        // // // PRIVATE

        private List<string> GetAllColumnTitles(IWebElement tableElement)
        {
            DebugOutput.Log($"Proc - GetAllColumnTitles {tableElement} ");
            List<string> columnTitles = new List<string>();
            foreach (var columnTitleLocator in TableHeadCellsLocator)
            {
                var allHeaderElements = SeleniumUtil.GetElementsUnder(tableElement, columnTitleLocator, 1);
                DebugOutput.Log($"Found {allHeaderElements.Count()} element headers using {columnTitleLocator}");
                if (allHeaderElements.Count > 0)
                {
                    foreach (var headerElement in allHeaderElements)
                    {
                        var textValue = SeleniumUtil.GetElementText(headerElement);
                        DebugOutput.Log($"Column Header = {textValue}");
                        if (textValue == null)
                        {
                            //Do nothing but can not test != as gives CS8604 warning
                        }
                        else
                        {
                            columnTitles.Add(textValue);
                        }
                    }
                }
            }
            return columnTitles;
        }

        public bool IsRowHighlighted(string tableName, int rowNumber)
        {
            DebugOutput.Log($"Proc - IsRowHighlighted {tableName} {rowNumber}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var rowElements = GetAllRows(tableElement);
            if (rowElements.Count == 0) return false;
            rowNumber = rowNumber - 1;  //Humans start from 1, rows start from 0
            var rowElement = rowElements[rowNumber];
            if (rowElement == null) return false;
            if (SeleniumUtil.IsSelected(rowElement)) return true;
            DebugOutput.Log($"The row itself is not selected, lets check for a sub element!");
            var numberOfSubHighlights = TableBodyRowSubLocator.Count();
            int counter = 0;
            while (counter < numberOfSubHighlights)
            {
                By subRowHighlighterLocator = TableBodyRowSubLocator[counter];
                DebugOutput.Log($"Trying sub element {subRowHighlighterLocator}");
                var subElement = SeleniumUtil.GetElementsUnder(rowElement, subRowHighlighterLocator, 1);
                if (subElement.Count() > 0)
                {
                    DebugOutput.Log($"CHECKING {counter} {subRowHighlighterLocator}");
                    if (SeleniumUtil.IsSelected(subElement[0])) return true;
                }
                counter ++;
            }
            DebugOutput.Log($"Failed to find ANY highlight");
            return false;
        }

        public bool OrderTableByColumn(string tableName, string columnName)
        {
            DebugOutput.Log($"Proc - GetColumnNumberFromTitle {tableName} {columnName}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
            DebugOutput.Log($"COLUMN NUMBER = {columnNumber}");
            if (columnNumber < 0) return false;
            return ClickOnHeader(tableElement, columnNumber);
        }

        public bool OrderTableByColumnDesc(string tableName, string columnName)
        {
            DebugOutput.Log($"Proc - OrderTableByColumnDesc {tableName} {columnName}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var columnHeaderElements = GetAllColumnElements(tableElement);
            if (columnHeaderElements.Count == 0) return false;
            var columnHeaderElement = GetColumnElementByName(columnHeaderElements, columnName);
            if (columnHeaderElement == null) return false;
            var classText = SeleniumUtil.GetElementAttributeValue(columnHeaderElement, "class");
            if (classText.Contains("sort-desc ")) return true;
            return SeleniumUtil.Click(columnHeaderElement);
        }

        public bool ClickOnLinkInTableColumnNameRow(string tableName, string columnName, int rowNumber)
        {
            DebugOutput.Log($"Proc - ClickOnLinkInTableColumnNameRow {tableName} {columnName} {rowNumber}");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var columnNumber = GetColumnNumberFromTitle(tableElement, columnName);
            if (columnNumber < 0) return false;
            return ClikcOnLinkInTableColumnRow(tableName, columnNumber, rowNumber);
        }

        private bool ClickOnHeader(IWebElement tableElement, int columnNumber)
        {
            DebugOutput.Log($"Proc - ClickOnHeader {tableElement} {columnNumber}");
            var columnHeaderElements = GetAllColumnElements(tableElement);
            var countNumberOfColumns = columnHeaderElements.Count;
            DebugOutput.Log($"The first element in the column elements is 0 but that is column 1, so we take away 1 from {columnNumber}");
            columnNumber--;
            if (columnNumber > countNumberOfColumns) 
            {
                DebugOutput.Log($"Can not check column {columnNumber} when I only get {countNumberOfColumns} back!");
                return false;
            }
            return SeleniumUtil.Click(columnHeaderElements[columnNumber]);
        }

        // / <summary>
        // / Given a name of a column, we will report back what column number
        // / </summary>
        // / <param name="tableElement"></param>
        // / <param name="columnTitle"></param>
        // / <returns></returns>
        private int GetColumnNumberFromTitle(IWebElement tableElement, string columnTitle)
        {
            columnTitle = columnTitle.Replace("'","");
            DebugOutput.Log($"Proc - GetColumnNumberFromTitle {tableElement} {columnTitle}");
            var allColumnTitles = GetAllColumnTitles(tableElement);
            int counter = 0;
            foreach (string theColumnTitle in allColumnTitles)
            {
                var gottenColumnTitle = theColumnTitle;
                gottenColumnTitle = gottenColumnTitle.Replace("'", "");
                DebugOutput.Log($"We received '{gottenColumnTitle}' we looking for column title '{columnTitle}'");
                if (gottenColumnTitle == columnTitle) return counter + 1;
                gottenColumnTitle = gottenColumnTitle.Trim();
                DebugOutput.Log($"We have trimmed it");
                if (gottenColumnTitle == columnTitle) return counter + 1;
                counter++;
            }
            return -1;
        }

        private List<IWebElement> GetAllColumnElements(IWebElement tableElement)
        {
            DebugOutput.Log($"Proc - GetAllColumnElements {tableElement}");
            var columnElements = new List<IWebElement>();
            foreach (var columnLocator in TableHeadCellsLocator)
            {
                DebugOutput.Log($"Using locator {columnLocator}");
                columnElements = SeleniumUtil.GetElementsUnder(tableElement, columnLocator);
                if (columnElements.Count() > 0)
                {
                    DebugOutput.Log($"We have column elements found by {columnLocator}");
                    return columnElements;
                }
            }
            DebugOutput.Log($"Failed to find any column elements using current TableHeadCellLocator s");
            return columnElements;
        }

        private IWebElement? GetColumnElementByName(List<IWebElement> columnElements, string columnName)
        {
            DebugOutput.Log($"Proc - GetColumnElementByName {columnElements} {columnName}");
            foreach (var columnElement in columnElements)
            {
                var textValue = SeleniumUtil.GetElementText(columnElement);
                DebugOutput.Log($"Column Header = {textValue}");
                if (textValue.ToLower() == columnName.ToLower()) return columnElement;
            }
            DebugOutput.Log($"Failed to find a column {columnName}");
            return null;
        }

        private bool EmptyRowDisplayed(IWebElement tableElement)
        {
            DebugOutput.Log($"Proc - EmptyRowDisplayed {tableElement}");
            foreach (var emptyText in EmptyTableText)
            {
                DebugOutput.Log($"Checking for an element that says '{emptyText}'");
                var emptyRowLocatorText = $"//*[contains(text(),'{emptyText}')]";
                var emptyRowLocator = By.XPath(emptyRowLocatorText);
                var childElement = SeleniumUtil.GetElementUnderElement(tableElement, emptyRowLocator, 1);
                if (childElement != null)
                {
                    DebugOutput.Log($"We have found an element that has that text.  We return if visiable!");
                    return childElement.Displayed;
                }
            }
            DebugOutput.Log($"Failed to find any emptry row! Which might be what you want!");
            return false;
        }

        private List<IWebElement> GetAllVisibleRows(IWebElement tableElement)
        {
            DebugOutput.Log($"Proc - GetAllVisibleRows {tableElement}");
            var rows = new List<IWebElement>();
            var allRowElements = GetAllRows(tableElement);
            if (allRowElements.Count == 0) return rows;
            foreach (var rowElement in allRowElements)
            {
                if (rowElement.Displayed)
                {
                    DebugOutput.Log($"Row is displayed but is it expanded?");
                    var classAttribute = rowElement.GetAttribute("class");
                    if (classAttribute != null && !classAttribute.Contains("expanded-row"))
                    {
                        DebugOutput.Log($"Row is displayed and not expanded!");
                        rows.Add(rowElement);
                    }
                }
            }
            return rows;
        }

        private List<IWebElement> GetAllRowsInBody(IWebElement tableElement)
        {
            DebugOutput.Log($"Proc - GetAllRowsInBody {tableElement}");
            List<IWebElement> rowElements = new List<IWebElement>();
            foreach (var locator in TableBodyLocator)
            {
                var tableBodyLocator = locator;
                var tableBodyElementTest = SeleniumUtil.GetElementsUnder(tableElement, tableBodyLocator, 1);
                DebugOutput.Log($"Found {tableBodyElementTest.Count()} elements using {tableBodyLocator}");
                if (tableBodyElementTest.Count > 0)
                {
                    DebugOutput.Log($"We have a table body element using {tableBodyLocator} FOUND {tableBodyElementTest.Count}");
                    foreach (var tableRowElement in tableBodyElementTest)
                    {
                        if (SeleniumUtil.IsDisplayed(tableRowElement))
                        {
                            rowElements.Add(tableRowElement);
                        }
                        else
                        {
                            DebugOutput.Log($"Table ROW element is NOT displayed");
                        }
                    }
                    return rowElements;
                }
            }
            return rowElements;
        }

        private List<IWebElement> GetAllRows(IWebElement tableElement)
        {
            DebugOutput.Log($"Proc - GetAllRows {tableElement}");
            var rows = new List<IWebElement>();
            if (EmptyRowDisplayed(tableElement)) return rows;
            //locator to find rows ----
            foreach (var rowLocator in TableBodyRowLocator)
            {
                var returnedRows = SeleniumUtil.GetElementsUnder(tableElement, rowLocator, 1);
                if (returnedRows.Count > 0)
                {
                    // I only want the rows displayed!
                    foreach(var row in returnedRows)
                    {
                        if (row.Displayed)
                        {
                            DebugOutput.Log($"Row is displayed");
                            rows.Add(row);
                        }
                        else
                        {
                            DebugOutput.Log($"Row is NOT displayed");
                        }
                    }
                    DebugOutput.Log($"Found using {rowLocator} returning {rows.Count()}");
                    return rows;
                }
            }
            DebugOutput.Log($"We not able to find using TableBodyRowLocator");
            return rows;
        }

        public bool IsRowForAction(string tableName, int rowNumber, string doing)
        {
            DebugOutput.OutputMethod("PrepareRowForAction");
            var tableElement = GetTableElement(tableName);
            if (tableElement == null) return false;
            var rowElements = GetAllRows(tableElement);
            if (rowElements.Count == 0) return false;
            DebugOutput.Log($"Looking for row number {rowNumber} of {rowElements.Count} rows DISPLAYED");
            rowNumber = rowNumber - 1;  //Humans start from 1,
            var rowElement = rowElements[rowNumber];
            if (rowElement == null) return false;
            if (SeleniumUtil.ScrollToElement(rowElement))
            {
                DebugOutput.Log($"Scrolled to row element");
            }
            // we have our row element I need the locator for the text
            var viewLocator = By.XPath($"//*[contains(text(),'{doing}')]");
            DebugOutput.Log($"Using locator {viewLocator} to find action element");
            var actionElement = SeleniumUtil.GetElementUnderElement(rowElement, viewLocator, 1);
            if (actionElement == null)
            {
                DebugOutput.Log($"Failed to find action element!");
                return false;
            }
            // we know it exists!
            DebugOutput.Log($"Found action element {actionElement} THIS the thing we looking for exists!");
            return true;
        }

        private IWebElement? GetTableElement(string tableName)
        {
            DebugOutput.Log($"GetTableElement {tableName}");

            //  my table has a name that COULD be not unique - it should have table at the end - so i can tell the diference.
            var newTableName = GetTableName(tableName);
            DebugOutput.Log($"I is here with a table name {newTableName} using page object {CurrentPage.Name}");
            if (CurrentPage.Elements.ContainsKey(newTableName))
            {
                By? newTableLocator;
                newTableLocator = CurrentPage.Elements[newTableName];
                var newTableElement = SeleniumUtil.GetElement(newTableLocator);
                if (newTableElement == null) return newTableElement;
                DebugOutput.Log($"Table Element {newTableName} = {newTableElement}");
                return newTableElement;
            }
            DebugOutput.Log($"Nothing found ending in ' table'");
            tableName = tableName.ToLower();
            tableName = tableName.Replace(" ", "");
            if (!CurrentPage.Elements.ContainsKey(tableName))
            {
                DebugOutput.Log($"STILL No key found of {tableName} in page {CurrentPage.Name}");
                return null;
            }
            By? tableLocator;
            tableLocator = CurrentPage.Elements[tableName];
            var tableElement = SeleniumUtil.GetElement(tableLocator);
            if (tableElement == null) return tableElement;
            DebugOutput.Log($"Table Element {tableName} = {tableElement}");
            return tableElement;
        }

        private string GetTableName(string tableName)
        {
            DebugOutput.Log($"GetTableName {tableName}");
            tableName = tableName.ToLower();
            tableName = tableName.Replace(" ", "");
            DebugOutput.Log($"GetTableName lower {tableName}");
            if (tableName.Contains(" table")) return tableName;
            DebugOutput.Log($"Adding <space>table to element Name");
            return tableName + " table";
        }



    }
}
