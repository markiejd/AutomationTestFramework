using System.Configuration;
using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Table
{
    [Binding]
    public class WhenTableSteps : StepsBase
    {
        public WhenTableSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [When(@"I Filter Table ""([^""]*)"" By ""([^""]*)""")]
        public void WhenIFilterTableBy(string tableName, string value)
        {
            string proc = $"When I Filter Table {tableName} By {value}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.Filter(tableName, value))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [When(@"I Click On CheckBox In Table ""(.*)"" Row (.*) Column (.*)")]
        public void WhenIClickOnCheckBoxInTableRowColumn(string tableName,int rowNumber,int columnNumber)
        {
            string proc = $"When I Click On CheckBox In Table {tableName} Row {rowNumber} Column {columnNumber}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ClickOnCheckBoxInTableColumnRow(tableName, rowNumber, columnNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Action ""([^""]*)"" Table ""([^""]*)"" Where Column ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void WhenIActionTableWhereColumnIsEqualTo(string action, string tableName, string columnName, string value)
        {
            string proc = $"When I Action {action} Table {tableName} Where Column {columnName} Is Equal To {value}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ActionRowByValueInColumnName(tableName, action, columnName, value))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Click On Table ""([^""]*)"" Row (.*)")]
        public void WhenIClickOnTableRow(string tableName, int rowNumber)
        {
            string proc = $"When I Click On Row {rowNumber} Of Table {tableName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ClickOnRow(rowNumber, tableName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [When(@"I Click On Link In Table ""(.*)"" In Row (.*) Column Title ""(.*)""")]
        public void WhenIClickOnLinkInTableInRowColumnTitle(string tableName,int rowNumber,string columnName)
        {
            string proc = $"When I Click On Link In Table {tableName} In Row {rowNumber} Column Title {columnName}";
            if (CombinedSteps.OuputProc(proc))
            {
                // get the column number where columnNmae is the title 
                if (Helpers.Table.ClickOnLinkInTableColumnNameRow(tableName, columnName, rowNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [When(@"I Click On Link In Table ""(.*)"" In Row (.*) Column (.*)")]
        public void WhenIClickOnLinkInTableInRowColumn(string tableName,int rowNumber,int columnNumber)
        {
            string proc = $"When I Click On Link In Table {tableName} Row {rowNumber} Column {tableName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ClikcOnLinkInTableColumnRow(tableName, columnNumber, rowNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }

        }
        
        
        [When(@"I Click In Table ""(.*)"" In Row (.*) Column (.*) Button")]
        public void WhenIClickInTableInRowColumnButton(string tableName,int rowNumber,int columnNumber)
        {
            string proc = $"When I Click In Table {tableName} Row {rowNumber} Column {tableName} Button";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ClickOnButtonInTableColumnRow(tableName, columnNumber, rowNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



        [When(@"I Order Table ""([^""]*)"" By Column ""([^""]*)""")]
        public void WhenIOrderTableByColumn(string tableName, string columnName)
        {
            string proc = $"When I Order Table {tableName} By Column {columnName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.OrderTableByColumn(tableName, columnName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Action ""([^""]*)"" From Table ""([^""]*)"" In Row (.*)")]
        public void WhenIActionFromTableInRow(string action, string tableName, int rowNumber)
        {
            string proc = $"When I Action {action} Table {tableName} In Row {rowNumber}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ActionRowByNumber(tableName, action, rowNumber))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [When(@"I Click On ""(.*)"" Button In Table ""(.*)""")]
        public bool WhenIClickOnButtonInTable(string buttonName,string tableName)
        {
            string proc = $"When I Click On {buttonName} Button In Table {tableName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.ClickOnNextPageInPagination(tableName, buttonName)) return true;
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }




    }
}
