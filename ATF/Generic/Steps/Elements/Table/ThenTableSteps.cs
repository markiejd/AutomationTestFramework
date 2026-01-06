using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Table
{
    [Binding]
    public class ThenTableSteps : StepsBase
    {
        public ThenTableSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Table ""([^""]*)"" Is Displayed")]
        public void ThenTableIsDisplayed(string tableName)
        {
            string proc = $"Then Table {tableName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.IsDisplayed(tableName, 5))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

                
        [Then(@"Table ""(.*)"" Has (.*) Rows")]
        public void ThenTableHasRows(string tableName,int totalNumberOfRows)
        {
            string proc = $"Then Table {tableName} Has {totalNumberOfRows} Rows";
            if (CombinedSteps.OutputProc(proc))
            {
                var numberOfRows = Helpers.Table.GetNumberOfRowsTotal(tableName);
                DebugOutput.Log($"Total Number of Rows Found: {numberOfRows}");
                if (numberOfRows == (int)totalNumberOfRows)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


                
        [Then(@"Table ""(.*)"" Is Not Displayed")]
        public void ThenTableIsNotDisplayed(string tableName)
        {
            string proc = $"Then Table {tableName} Is Not Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Table.IsDisplayed(tableName, 1))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Table ""(.*)"" Has Column ""(.*)""")]
        [Then(@"Table ""([^""]*)"" Has Column ""([^""]*)""")]
        public void ThenTableHasColumn(string tableName,string columnName)
        {
            string proc = $"Then Table {tableName} Has Column {columnName}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.IsColumnExist(tableName, columnName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

         [Then(@"Table ""(.*)"" Has Column (.*) Title ""(.*)""")]       
        [Then(@"Table ""([^""]*)"" Has Column (.*) Title ""([^""]*)""")]
        public bool ThenTableHasColumnTitle(string tableName,int columnNumber,string columnTitle)
        {
            string proc = $"Then Table {tableName} Has Column {columnNumber} Title {columnTitle}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.IsColumnExistInLocation(tableName, columnTitle, columnNumber))
                {
                    DebugOutput.Log($"Correct! {columnNumber}");
                    return true;
                }
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }


        [Then(@"Table ""([^""]*)"" Displayed (.*) Rows")]
        [Then(@"Table ""([^""]*)"" Has (.*) Rows Displayed")]
        public void ThenTableHasRowsDisplayed(string tableName, int numberOfRows)
        {
            string proc = $"Then Table {tableName} Has Rows {numberOfRows} Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.GetNumberOfRowsDisplayed(tableName) == numberOfRows)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Then(@"Table ""([^""]*)"" Is Empty")]
        public void ThenTableIsEmpty(string tableName)
        {
            string proc = $"Then Table {tableName} Is Empty";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.GetNumberOfRowsDisplayed(tableName) == 0)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Table ""([^""]*)"" Is Not Empty")]
        public void ThenTableIsNotEmpty(string tableName)
        {
            string proc = $"Then Table {tableName} Is Not Empty";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.GetNumberOfRowsDisplayed(tableName) > 0)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        
        [Then(@"Table ""(.*)"" Row (.*) Column (.*) Contains Hyperlink")]
        public void ThenTableRowColumnContainsHyperlink(string tableName,int rowNumber,int columnNumber)
        {
            string proc = $"Then Table {tableName} Row {rowNumber} Column {columnNumber} Contains Hyperlink";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.DoesRowColumnNumberContainsLink(tableName, rowNumber, columnNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }
        
        [Then(@"Table ""(.*)"" Row (.*) Column (.*) Contains No Hyperlink")]
        public void ThenTableRowColumnContainsNoHyperlink(string tableName,int rowNumber,int columnNumber)
        {
            string proc = $"Then Table {tableName} Row {rowNumber} Column {columnNumber} Contains No Hyperlink";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Table.DoesRowColumnNumberContainsLink(tableName, rowNumber, columnNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Table ""([^""]*)"" has (.*) Populated Rows Displayed")]
        public void ThenTableHasPopulatedRowsDisplayed(string tableName, int populatedRows)
        {
            string proc = $"Then Table {tableName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.GetNumberOfRowsDisplayed(tableName) == populatedRows)
                //if (Helpers.Table.GetNumberOfPopulatedRowsDisplayed(tableName) == populatedRows)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Table ""([^""]*)"" Column Title ""([^""]*)"" Row (.*) Is Equal To ""([^""]*)""")]
        public void ThenTableColumnTitleRowIsEqualTo(string tableName, string columnTitle, int rowNumber, string value)
        {
            string proc = $"Then Table {tableName} Column Title {columnTitle} Row {rowNumber} Is Equal To {value}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.GetValueOfGridBoxUsingColumnTitle(tableName, columnTitle, rowNumber) == value)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Table ""([^""]*)"" Row (.*) Is Highlighted")]
        public void ThenTableRowIsHighlighted(string tableName, int rowNumber)
        {
            string proc = $"Then Table {tableName} Row {rowNumber} Is Highlighted";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.IsRowHighlighted(tableName, rowNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Table ""([^""]*)"" Row (.*) Is Not Highlighted")]
        public void ThenTableRowIsNotHighlighted(string tableName, int rowNumber)
        {
            string proc = $"Then Table {tableName} Row {rowNumber} Is Not Highlighted";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.Table.IsRowHighlighted(tableName, rowNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




        [Then(@"Table ""([^""]*)"" Row (.*) Column (.*) Is Equal To ""([^""]*)""")]
        public void ThenTableRowColumnIsEqualTo(string tableName, int rowNumber, int columnNumber, string value)
        {
            string proc = $"Then Table {tableName} Row {rowNumber} Column {columnNumber} Is Equal To {value}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.GetValueOfGridBox(tableName, rowNumber, columnNumber, false) == value)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Table ""([^""]*)"" Column Title ""([^""]*)"" Contains Value ""([^""]*)""")]
        public void ThenTableColumnTitleContainsValue(string tableName, string columnName, string value)
        {
            string proc = $"Then Table {tableName} Column Title {columnName} Contains Value {value}";
            value = StringValues.TextReplacementService(value);
            proc = $"Then Table {tableName} Column Title {columnName} Contains Value {value}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Table.IsColumnContainValue(tableName, columnName, value))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
