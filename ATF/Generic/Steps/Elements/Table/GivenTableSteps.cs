using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.Table
{
    [Binding]
    public class GivenTableSteps : StepsBase
    {
        public GivenTableSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Table ""([^""]*)"" Is Displayed")]
        public void GivenTableIsDisplayed(string tableName)
        {
            string proc = $"Given Table {tableName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.IsDisplayed(tableName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [Given(@"Table ""(.*)"" Row (.*) Is Ready ""(.*)""")]
        public void GivenTableRowIsReady(string tableName,int rowNumber,string doing)
        {
            string proc = $"Given Table {tableName} Row {rowNumber} Is Ready {doing}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.IsRowForAction(tableName, rowNumber, doing))
                {
                    return;
                }
                // it is A Given - it is not set - we need to set it!

                if (!Helpers.Table.ClickOnCheckBoxInTableColumnRow(tableName, rowNumber, 1))
                {
                    DebugOutput.Log($"Could not click the checkbox to set the row!");
                    CombinedSteps.Failure(proc);
                    return;
                }

                // MARK TO DO  !!!!
                // click button "Approve Selected"
                if (!Helpers.Button.ClickButton("Approve Selected"))
                {
                    DebugOutput.Log($"Could not click the Approve Selected button to set the row!");
                    CombinedSteps.Failure(proc);
                    return;
                }

                if (!Helpers.Button.ClickButton("Refresh"))
                {
                    DebugOutput.Log($"Could not click the Refresh button to set the row!");
                    CombinedSteps.Failure(proc);
                    return;
                }

                Thread.Sleep(1000); // wait for the table to refresh


                // it should be there now
                if (Helpers.Table.IsRowForAction(tableName, rowNumber, doing))
                {
                    return;
                }
                DebugOutput.Log($"FOR THE LOVE OF THE WEE MAN HOW IS THIS NOT WORKING!");
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Given(@"Table ""([^""]*)"" Is Ordered By Column ""([^""]*)"" Descending")]
        public void GivenTableIsOrderedByColumnDescending(string tableName, string columnName)
        {
            string proc = $"Given Table {tableName} Is Ordered by Column {columnName} Descending";
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
        
        [Given(@"Table ""(.*)"" Rows To Page Is Equal To ""(.*)""")]
        public bool GivenTableRowsToPageIsEqualTo(string tableName,int expectedRowsPerPage)
        {
            string proc = $"Given Table {tableName} Rows To Page Is Equal To {expectedRowsPerPage}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Table.GetNumberOfRowsPerPage(tableName) == expectedRowsPerPage) return true;
                DebugOutput.Log($"Need to set it!");
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }





    }
}
