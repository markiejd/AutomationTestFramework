using System;
using System.IO;
using Newtonsoft.Json; // Install Newtonsoft.Json via NuGet
using ClosedXML.Excel; // Install ClosedXML via NuGet
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {

        if (args.Length < 2)
        {
            Console.WriteLine("Usage: CommunicationExcel.exe <filePath> <hasHeaders> [worksheetName]");
            return;
        }

        string action = args[0];
        // want to send the rest of the parameters and use 0 to find what we doing
        string[] parameters = args.Length > 1 ? args[1..] : Array.Empty<string>();

        //repsonse from methods
        string response = "";

        switch (action.ToLower())
        {
            default:
                response = "Invalid action specified. Please use 'readall' to read an Excel file.";
                break;
            case "readall":
                response = ReadAllFromSheet(parameters);
                break;
            case "readrow":
                response = ReadRow(parameters);
                break;
            case "readrows":
                response = ReadRows(parameters);
                break;
            case "getsheets":
                if (parameters.Length < 1)
                {
                    response = "error: Insufficient parameters provided. Please provide the file path.";
                }
                else
                {
                    response = GetSheetNames(parameters[0]);
                }
                break;
            case "readcell":
                if (parameters.Length < 3)
                {
                    response = "error :Insufficient parameters provided. Please provide the file path, sheet name, and cell address.";
                }
                else
                {
                    response = ReadCellValue(parameters[0], parameters[1], parameters[2]);
                }
                break;
        }
        Console.WriteLine(response);
    }

    /// <summary>
    /// Reads the value of a specific cell in the specified Excel sheet.
    /// </summary>
    /// <param name="filePath">The path to the Excel file.</param>
    /// <param name="sheetName">The name of the worksheet.</param>
    /// <param name="cellAddress">The address of the cell (e.g., "A1").</param>
    /// <returns>The value of the cell as a string, or an error message if the cell cannot be read.</returns>
    static string ReadCellValue(string filePath, string sheetName, string cellAddress)
    {
        if (!IsValidFilePath(filePath))
        {
            return $"The specified file '{filePath}' does not exist or is not a valid file path.";
        }
        try
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(sheetName);
                if (worksheet == null)
                {
                    return $"The specified worksheet '{sheetName}' does not exist.";
                }

                var cell = worksheet.Cell(cellAddress);
                if (cell == null || cell.IsEmpty())
                {
                    return string.Empty;
                }

                return cell.GetValue<string>() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            return $"Error reading cell value: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Retrieves a list of all sheet names in the specified Excel file.
    /// </summary>
    /// <param name="filePath">The path to the Excel file.</param>
    /// <returns>A JSON string containing a list of sheet names, or an error message if the file is invalid.</returns>
    static string GetSheetNames(string filePath)
    {
        if (!IsValidFilePath(filePath))
        {
            return $"The specified file '{filePath}' does not exist or is not a valid file path.";
        }
        try
        {
            var sheetNames = new List<string>();
            using (var workbook = new XLWorkbook(filePath))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    sheetNames.Add(worksheet.Name);
                }
            }
            return JsonConvert.SerializeObject(sheetNames, Formatting.Indented);
        }
        catch (Exception ex)
        {
            return $"Error retrieving sheet names: {ex.Message}";
        }
    }

    /// <summary>
    /// Reads specific rows (inclusive) from the specified Excel sheet and returns them as a JSON string.
    /// /// The first parameter is the file path,
    /// the second indicates if the file has headers,
    /// /// the third is the start row number (1-based index),
    /// /// the fourth is the end row number (1-based index),   
    /// /// and the fifth is the optional worksheet name.
    /// /// If the worksheet name is not provided, the first worksheet will be used.
    /// /// This method is designed to read multiple rows from the Excel file,
    /// /// which is useful for scenarios where you need to retrieve a range of rows' data
    /// /// /// such as when processing a batch of records or extracting a subset of data.
    /// /// The rows are returned as a JSON array of objects, where each object represents a row in the specified range.
    /// /// The keys in each object correspond to the column headers, and the values are the cell values for that row.
    /// /// If the specified range exceeds the number of rows in the worksheet, an error message    
    /// /// will be returned indicating the issue.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    static string ReadRows(string[] parameters)
    {
        if (parameters.Length < 4)
        {
            return "Insufficient parameters provided. Please provide the file path, whether the file has headers, start row number, and end row number.";
        }

        string filePath = parameters[0];
        bool hasHeaders = bool.TryParse(parameters[1], out bool result) && result;
        if (!int.TryParse(parameters[2], out int startRow) || startRow < 1)
        {
            return "Invalid start row number provided. Please provide a valid positive integer.";
        }
        if (!int.TryParse(parameters[3], out int endRow) || endRow < startRow)
        {
            return "Invalid end row number provided. Please provide a valid positive integer greater than or equal to the start row.";
        }
        string? worksheetName = parameters.Length > 4 ? parameters[4] : null;

        if (!IsValidFilePath(filePath))
        {
            return $"The specified file '{filePath}' does not exist or is not a valid file path.";
        }

        try
        {
            var data = new List<Dictionary<string, object>>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = string.IsNullOrWhiteSpace(worksheetName)
                    ? workbook.Worksheets.FirstOrDefault()
                    : workbook.Worksheet(worksheetName);

                if (worksheet == null)
                {
                    throw new Exception(string.IsNullOrWhiteSpace(worksheetName)
                        ? "No worksheets found in the Excel file."
                        : $"The specified worksheet '{worksheetName}' does not exist.");
                }

                var rangeUsed = worksheet.RangeUsed();
                if (rangeUsed == null || rangeUsed.IsEmpty())
                {
                    throw new Exception("The worksheet is empty or does not contain any data.");
                }

                var rows = rangeUsed.RowsUsed().ToList(); // Convert to a list to avoid type issues
                if (rows == null || !rows.Any())
                {
                    throw new Exception("The worksheet is empty.");
                }

                if (startRow < 1 || endRow > rows.Count)
                {
                    throw new Exception($"Row range {startRow}-{endRow} is out of range. The worksheet has {rows.Count} rows.");
                }

                var headers = new List<string>();
                if (hasHeaders)
                {
                    var firstRow = rows.FirstOrDefault();
                    if (firstRow == null)
                    {
                        throw new Exception("The worksheet does not contain a valid header row.");
                    }

                    foreach (var cell in firstRow.Cells())
                    {
                        headers.Add(cell.GetValue<string>() ?? string.Empty);
                    }
                }
                else
                {
                    // Generate default headers if no headers are present
                    for (int col = 1; col <= rows.First().Cells().Count(); col++)
                    {
                        headers.Add($"Column{col}");
                    }
                }

                for (int i = startRow - 1; i < endRow; i++)
                {
                    var row = rows[i];
                    var rowData = new Dictionary<string, object>();
                    for (int col = 1; col <= headers.Count; col++)
                    {
                        var cellValue = row.Cell(col).GetValue<string>();
                        rowData[headers[col - 1]] = cellValue ?? string.Empty;
                    }
                    data.Add(rowData);
                }
            }

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            return $"Error reading Excel file: {ex.Message}";
        }
    }

    /// <summary>
    /// Reads a specific row from the specified Excel sheet and returns it as a JSON string.
    /// The first parameter is the file path, 
    /// the second indicates if the file has headers, 
    /// the third is the row number to read (1-based index),
    /// and the fourth is the optional worksheet name.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    static string ReadRow(string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return "Insufficient parameters provided. Please provide the file path, whether the file has headers, and the row number.";
        }

        string filePath = parameters[0];
        bool hasHeaders = bool.TryParse(parameters[1], out bool result) && result;
        if (!int.TryParse(parameters[2], out int rowNumber) || rowNumber < 1)
        {
            return "Invalid row number provided. Please provide a valid positive integer.";
        }
        string? worksheetName = parameters.Length > 3 ? parameters[3] : null;

        if (!IsValidFilePath(filePath))
        {
            return $"The specified file '{filePath}' does not exist or is not a valid file path.";
        }

        try
        {
            return ReadExcelFileOnly1Row(filePath, hasHeaders, rowNumber, worksheetName);
        }
        catch (Exception ex)
        {
            return $"Error reading Excel file: {ex.Message}";
        }
    }

    /// <summary>
    /// Reads all data from the specified Excel sheet and returns it as a JSON string.
    /// The first parameter is the file path, 
    /// the second indicates if the file has headers,
    /// the third is the optional worksheet name.
    /// If the worksheet name is not provided, the first worksheet will be used.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    static string ReadAllFromSheet(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return "Insufficient parameters provided. Please provide the file path";
        }
        if (parameters.Length < 2)
        {
            return "Insufficient parameters provided. Please provide whether the file has headers.";
        }

        string filePath = parameters[0];
        bool hasHeaders = bool.TryParse(parameters[1], out bool result) && result;
        string? worksheetName = parameters.Length > 2 ? parameters[2] : null;

        if (!IsValidFilePath(filePath))
        {
            return $"The specified file '{filePath}' does not exist or is not a valid file path.";
        }

        try
        {
            return ReadExcelFileAsJson(filePath, hasHeaders, worksheetName);
        }
        catch (Exception ex)
        {
            return $"Error reading Excel file: {ex.Message}";
        }
    }



    static bool IsValidFilePath(string filePath)
    {
        return !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
    }

    /// <summary>
    /// Reads a specific row from the specified Excel file and returns it as a JSON string.
    /// The first parameter is the file path,   
    /// the second indicates if the file has headers,
    /// the third is the row number to read (1-based index),    
    /// the fourth is the optional worksheet name.
    /// If the worksheet name is not provided, the first worksheet will be used.
    /// /// This method is designed to read only one row from the Excel file,
    /// which is useful for scenarios where you only need to retrieve a specific row's data.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="hasHeaders"></param>
    /// <param name="rowNumber"></param>
    /// <param name="worksheetName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    static string ReadExcelFileOnly1Row(string filePath, bool hasHeaders, int rowNumber, string? worksheetName = null)
    {
        var data = new Dictionary<string, object>();

        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = string.IsNullOrWhiteSpace(worksheetName)
                ? workbook.Worksheets.FirstOrDefault()
                : workbook.Worksheet(worksheetName);

            if (worksheet == null)
            {
                throw new Exception(string.IsNullOrWhiteSpace(worksheetName)
                    ? "No worksheets found in the Excel file."
                    : $"The specified worksheet '{worksheetName}' does not exist.");
            }

            var rangeUsed = worksheet.RangeUsed();
            if (rangeUsed == null || rangeUsed.IsEmpty())
            {
                throw new Exception("The worksheet is empty or does not contain any data.");
            }

            var rows = rangeUsed.RowsUsed().ToList(); // Convert to a list to avoid type issues
            if (rows == null || !rows.Any())
            {
                throw new Exception("The worksheet is empty.");
            }

            if (rowNumber < 1 || rowNumber > rows.Count)
            {
                throw new Exception($"Row number {rowNumber} is out of range. The worksheet has {rows.Count} rows.");
            }

            var row = rows[rowNumber - 1]; // Convert to 0-based index

            var headers = new List<string>();
            if (hasHeaders)
            {
                var firstRow = rows.FirstOrDefault();
                if (firstRow == null)
                {
                    throw new Exception("The worksheet does not contain a valid header row.");
                }

                foreach (var cell in firstRow.Cells())
                {
                    headers.Add(cell.GetValue<string>() ?? string.Empty);
                }
            }
            else
            {
                // Generate default headers if no headers are present
                for (int col = 1; col <= row.Cells().Count(); col++)
                {
                    headers.Add($"Column{col}");
                }
            }

            for (int col = 1; col <= headers.Count; col++)
            {
                var cellValue = row.Cell(col).GetValue<string>();
                data[headers[col - 1]] = cellValue ?? string.Empty;
            }
        }

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    static string ReadExcelFileAsJson(string filePath, bool hasHeaders, string? worksheetName = null)
    {
        var data = new List<Dictionary<string, object>>();

        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = string.IsNullOrWhiteSpace(worksheetName)
                ? workbook.Worksheets.FirstOrDefault()
                : workbook.Worksheet(worksheetName);

            if (worksheet == null)
            {
                throw new Exception(string.IsNullOrWhiteSpace(worksheetName)
                    ? "No worksheets found in the Excel file."
                    : $"The specified worksheet '{worksheetName}' does not exist.");
            }

            var rangeUsed = worksheet.RangeUsed();
            if (rangeUsed == null || rangeUsed.IsEmpty())
            {
                throw new Exception("The worksheet is empty or does not contain any data.");
            }

            var rows = rangeUsed.RowsUsed().ToList(); // Convert to a list to avoid type issues
            if (rows == null || !rows.Any())
            {
                throw new Exception("The worksheet is empty.");
            }

            var headers = new List<string>();
            var dataRows = rows;

            if (hasHeaders)
            {
                var firstRow = rows.FirstOrDefault();
                if (firstRow == null)
                {
                    throw new Exception("The worksheet does not contain a valid header row.");
                }

                foreach (var cell in firstRow.Cells())
                {
                    headers.Add(cell.GetValue<string>() ?? string.Empty);
                }

                dataRows = rows.Skip(1).ToList(); // Skip the header row
            }
            else
            {
                // Generate default headers if no headers are present
                var firstRow = rows.FirstOrDefault();
                if (firstRow != null)
                {
                    for (int col = 1; col <= firstRow.Cells().Count(); col++)
                    {
                        headers.Add($"Column{col}");
                    }
                }
            }

            foreach (var row in dataRows)
            {
                var rowData = new Dictionary<string, object>();
                for (int col = 1; col <= headers.Count; col++)
                {
                    var cellValue = row.Cell(col).GetValue<string>();
                    rowData[headers[col - 1]] = cellValue ?? string.Empty;
                }
                data.Add(rowData);
            }
        }

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }
}