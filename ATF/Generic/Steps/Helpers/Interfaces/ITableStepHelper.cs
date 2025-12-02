
namespace Generic.Steps.Helpers.Interfaces
{
    public interface ITableStepHelper : IStepHelper
    {
        bool ActionRowByValueInColumnName(string tableName, string action, string columnName, string value);
        bool ActionRowByNumber(string tableName, string action, int rowNumber);
        bool ClickOnButtonInTableColumnRow(string tableName, int columnNumber, int rowNumber);
        bool ClickOnCheckBoxInTableColumnRow(string tableName, int rowNumber, int columnNumber);
        bool ClikcOnLinkInTableColumnRow(string tableName, int columnNumber, int rowNumber);
        bool ClickOnLinkInTableColumnNameRow(string tableName, string columnName, int rowNumber);
        bool ClickOnNextPageInPagination(string tableName, string button);
        bool ClickOnRow(int rowNumber, string tableName);
        bool DoesRowColumnNumberContainsLink(string tableName, int rowNumber, int columnNumber);
        bool DoesRowContainAction(string tableName, string action, string columnName, string value);
        int GetNumberOfRowsPerPage(string tableName);
        int GetNumberOfPopulatedRowsDisplayed(string tableName);
        int GetNumberOfRowsDisplayed(string tableName);
        int GetNumberOfRowsTotal(string tableName);
        // int GetNumberOfRowsWithLocatorFound(string tableName, By locator);
        string GetValueOfGridBox(string tableName, int rowNumber, int columnNumber, bool header);
        string GetValueOfGridBoxUsingColumnTitle(string tableName, string columnTitle, int rowNumber);
        bool Filter(string tableName, string value);
        bool IsColumnContainValue(string tableName, string columnName, string value);
        bool IsColumnExist(string tableName, string columnName);
        bool IsColumnExistInLocation(string tableName, string columnName, int count = 0);
        bool IsDisplayed(string tableName, int timeout = 0);
        bool IsRowForAction(string tableName, int rowNumber, string doing);
        bool IsRowHighlighted(string tableName, int rowNumber);
        bool OrderTableByColumn(string tableName, string columnName);
        bool OrderTableByColumnDesc(string tableName, string columnName);
    }
}
