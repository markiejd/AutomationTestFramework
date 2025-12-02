
using Core.FileIO;
using Core.Logging;

namespace Core.Transformations
{
    public static class CommaDelimited
    {

        /// <summary>
        /// Read CSV File
        /// </summary>
        /// <returns>List of data rows, still delimited</returns>
        public static List<string>? GetCommaDelimitedData(string pathAndFileName, bool headerRow = true)
        {
            DebugOutput.Log($"ReadCommaDelimitedFile {pathAndFileName} {headerRow}");
            if (!FileUtils.OSFileCheck(pathAndFileName)) return null;
            var rowsDelimited = ReadOSCommaDelimitedFile(pathAndFileName);
            if (rowsDelimited == null) return null;
            if (headerRow)
            {
                rowsDelimited.RemoveAt(0);
                if (headerRow) DebugOutput.Log($"Removed the header, left with {rowsDelimited.Count} data rows");
                if (headerRow) return rowsDelimited;
            } 
            if (headerRow) DebugOutput.Log($"No header, left with {rowsDelimited.Count} data rows");
            return rowsDelimited;
        }

        public static string? GetCommaDelimitedHeader(string pathAndFileName, bool headerRow = true)
        {
            DebugOutput.Log($"GetCommaDelimitedHeader {pathAndFileName} {headerRow}");
            if (!headerRow)
            {
                DebugOutput.Log($"I need it to have a header to send you the header, or we lose 1 line of data!");
                return null;
            }
            if (!FileUtils.OSFileCheck(pathAndFileName)) return null;
            var rowsDelimited = ReadOSCommaDelimitedFile(pathAndFileName);
            if (rowsDelimited == null) return null;
            var header = rowsDelimited[0];
            DebugOutput.Log($"Our Header is {header}");
            return header;
        }

        /// <summary>
        /// Read the File Name return each line
        ///
        /// </summary>
        /// <returns>string of the json file, null if it failed</returns>
        private static List<string>? ReadOSCommaDelimitedFile(string fullFileName)
        {
            DebugOutput.Log($"Proc - ReadOSCommaDelimitedFile {fullFileName}");
            using var r = FileUtils.GetStream(fullFileName, true);
            var rows = new List<string>();
            try
            {
                string? row;
                while ((row = r.ReadLine()) != null)
                {
                    rows.Add(row);
                }
                return rows;
            }
            catch
            {
                DebugOutput.Log($"issue with reading");
                return null;
            }
        }

        public static int GetRowNumberWhereRow1AndRow2AreEqualTo(string row1Value, string row2Value, List<string> data, bool header = false)
        {
            DebugOutput.Log($"Proc - GetRowNumberWhereRow1AndRow2AreEqualTo {row1Value} {row2Value} {data.Count}");
            int rowNumberCounter = 0;
            foreach (var row in data)
            {
                var cells = StringValues.BreakUpByDelimitedToList(row, ",");
                if (cells[0] == row1Value) 
                {
                    if (cells[1] == row2Value)
                    {
                        DebugOutput.Log($"Found it in DATA row {rowNumberCounter} {cells[0]} {cells[1] }");
                        if (header) return rowNumberCounter - 1;
                        return rowNumberCounter;
                    }
                }
                rowNumberCounter++;
            }
            DebugOutput.Log($"Failed to find! sending back -1");
            return -1;
        }

        /// <summary>
        /// Give us the header name, the header, the data List and what row you want
        /// We will give you the value!
        /// </summary>
        /// <returns>string of the data in row</returns>
        public static string? ReadDataGetValueFromHeader(string headerName, string headers, List<string> data, int rowNumber)
        {
            DebugOutput.Log($"Proc - ReadDataGetValueFromHeader {headerName} {headers} {rowNumber}");
            var listOfHeaders = StringValues.BreakUpByDelimitedToList(headers, ",");
            DebugOutput.Log($"We have {listOfHeaders.Count} headers!");
            if (listOfHeaders.Count == 0) return null;
            int counter = 0;
            int headerNumber = 0;
            foreach (var columnHeader in listOfHeaders)
            {
                if (columnHeader == headerName) headerNumber = counter;
                if (headerNumber != 0) break;
                counter++;
            }
            DebugOutput.Log($"Found header in column {headerNumber}");
            int rowNumberCounter = 1;
            foreach (var row in data)
            {
                if (rowNumberCounter == rowNumber)
                {
                    var rowUnlimited = StringValues.BreakUpByDelimitedToList(row, ",");
                    try
                    {
                        return rowUnlimited[headerNumber];
                    }
                    catch
                    {
                        DebugOutput.Log($"We at the place - just something went wrong!");
                        return null;
                    }                    
                }
                rowNumberCounter++;
            }
            DebugOutput.Log($"Failed something bad!");
            return null;
        }



    }
}