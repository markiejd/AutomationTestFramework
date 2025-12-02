

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.RegularExpressions;
using Core;
using Core.FileIO;
using Core.Logging;
using Generic.Steps;

namespace Generic.Elements.Steps.TSQL.Code
{

    public class TSQLDataCode
    {


        private static string Stored = "";

        public bool Hello()
        {
            DebugOutput.Log("Hello from TSQLStepHelper");
            DebugOutput.Log($"THE STORED STRING IS :-");
            DebugOutput.Log($"{Stored}");
            return true;
        }


        public static string SendSQLCommand(string sqlCommand, string licence = "", string outputFile = "", bool json = false)
        {
            licence = "\"" + licence + "\""; // Ensure licence is always a string, even if empty
            outputFile = "\"" + outputFile + "\""; // Ensure outputFile is always a string
            if (json)
            {
                sqlCommand = GetSQLCommandAsJson(sqlCommand);
            }
            sqlCommand = GetSQLCommandAsCSV(sqlCommand);
            string arguments = GetCommandSqlServerArguments(sqlCommand, licence, outputFile);

            // // dotnet run --project ./CommunicationSqlServer/SqlServerCommunication.csproj -- "select * from [ACCOUNT]" "<licence>" ""

            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", arguments);
            // remove any double returns
            returnedString = returnedString.Replace("\r\n\r\n", "\r\n").Replace("\n\n", "\n").Trim();
            returnedString = Regex.Replace(returnedString, @"&#xD;", string.Empty);
            returnedString = Regex.Replace(returnedString, @"\r\n", string.Empty);
            returnedString = Regex.Replace(returnedString, @"\n", string.Empty);
            returnedString = returnedString.Trim();
            try
            {
                var parsedJson = JsonSerializer.Deserialize<object>(returnedString);
                Console.WriteLine("JSON parsed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing JSON: " + ex.Message);
            }

            DebugOutput.Log($"THE RETURN STRING IS !!!!! :-");
            DebugOutput.Log($"{returnedString}");
            if (outputFile == "\"\"")
            {
                Stored = returnedString;
            }
            return returnedString;
        }

        /// <summary>
        /// Constructs the command line arguments for executing a SQL command in SQL Server.
        /// This method combines the SQL command, licence, and output file into a single string.
        /// The SQL command is expected to be in a format that can be executed by the SQL Server communication project.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="licence"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        private static string GetCommandSqlServerArguments(string sqlCommand, string licence, string outputFile)
        {
            return sqlCommand + " " + licence + " " + outputFile;
        }

        /// <summary>
        /// Converts the SQL command to a JSON format suitable for execution.
        /// This method removes semicolons and quotes, then appends "FOR JSON AUTO"
        /// and wraps the command in quotes.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        private static string GetSQLCommandAsJson(string sqlCommand)
        {
            return GetSQLCommandAsCSV(sqlCommand) + " FOR JSON AUTO; ";
        }

        /// <summary>
        /// Converts the SQL command to a CSV format suitable for execution.
        /// This method removes semicolons and quotes, then wraps the command in quotes.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        private static string GetSQLCommandAsCSV(string sqlCommand)
        {
            sqlCommand = sqlCommand.Replace(";", "").Replace("\"", "");
            sqlCommand = "\"" + sqlCommand + " \"";
            return sqlCommand;
        }


        public static bool SetColumns(string tableName)
        {
            // DESC the table in questions
            var sqlCommand = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}';";
            DebugOutput.Log($"The SQL Command is: {sqlCommand}");
            var returnedString = GetReturnedString(sqlCommand);
            if (string.IsNullOrEmpty(returnedString))
            {
                CombinedSteps.Failure($"The SQL command returned an empty string: {returnedString}");
                return false;
            }
            var descTable = DescTable.DescTableFromJson(returnedString);
            if (descTable == null || descTable.Count == 0)
            {
                CombinedSteps.Failure($"The returned string is null or empty: {returnedString}");
                return false;
            }
            DebugOutput.Log($"The DescTableStorage is set with {descTable.Count} items.");
            return DescTableStorage.SetDescTableStore(descTable);
        }


        public static bool SetKeys(string tableName)
        {
            var sqlCommand = $"SELECT kc.name AS KeyName, kc.type_desc AS KeyType, c.name AS ColumnName FROM sys.key_constraints kc JOIN sys.tables t ON kc.parent_object_id = t.object_id JOIN sys.index_columns ic ON kc.unique_index_id = ic.index_id AND kc.parent_object_id = ic.object_id JOIN sys.columns c ON ic.column_id = c.column_id AND ic.object_id = c.object_id WHERE t.name = '{tableName}'  FOR JSON PATH";
            DebugOutput.Log($"This SQL Command is: {sqlCommand}");
            var returnedString = GetReturnedString(sqlCommand, "", "", false);
            if (string.IsNullOrEmpty(returnedString))
            {
                CombinedSteps.Failure($"The SQL command returned an empty string: {returnedString}");
                return false;
            }
            var descTableKeys = DescTableKey.DescTableKeyFromJson(returnedString);
            DebugOutput.Log($"The DescTableKeyStorage is set with {descTableKeys.Count} items.");
            return DescTableKeyStorage.SetDescTableKeyStore(descTableKeys);
        }



        private static string GetReturnedString(string sqlCommand, string licence = "", string saveLocation = "", bool isJson = true)
        {
            DebugOutput.Log($"This SQL Command is: {sqlCommand}");
            var returnedString = TSQLDataCode.SendSQLCommand(sqlCommand, licence, saveLocation, isJson);
            DebugOutput.Log($"This returned string is: {returnedString}");
            // check the returned string, it can be null or empty, but if it starts with "Error" then we have an error
            if (returnedString.ToLower().StartsWith("error"))
            {
                CombinedSteps.Failure($"We have an error returned from the SQL command: {returnedString}");
                return string.Empty;
            }
            return returnedString;
        }


        public static bool SetColumnAndKeys(string tableName)
        {
            if (!SetColumns(tableName))
            {
                return false;
            }
            if (!SetKeys(tableName))
            {
                return false;
            }
            return true;
        }


        public static string ReadDataDictionaryFromExcel(string fileLocation)
        {
            DebugOutput.Log($"ReadDataDictionaryFromExcel {fileLocation}");
            // if the first character is / or \ then don't add it to the file location, if not, add a forward slash
            if (!fileLocation.StartsWith("/") || !fileLocation.StartsWith("\\"))
            {
                fileLocation = "/" + fileLocation;
            }
            // get the full file path, which is the location of this repo + fileLocation
            var fullFilePath = FileUtils.GetCorrectDirectory(fileLocation);
            var linuxFilePath = fullFilePath.Replace("\\", "/");
            DebugOutput.Log($"The full file path is: {fullFilePath}");
            if (!FileUtils.OSFileCheck(fullFilePath))
            {
                CombinedSteps.Failure($"The file {fullFilePath} does not exist.");
                return string.Empty;
            }
            var response = CmdUtil.ExecuteDotnet("./CommunicationExcel/CommunicationExcel.csproj", $"\"readall\" \"{fullFilePath}\" \"true\"").Trim();
            // g
            if (response.Length < 5)
            {
                CombinedSteps.Failure($"The response from CommunicationExcel is too short: {response}");
                return string.Empty;
            }
            DebugOutput.Log($"The response from CommunicationExcel is: {response.Substring(0, Math.Min(5, response.Length))}...");
            string checkText = response.Substring(0, Math.Min(5, response.Length)) ?? "";
            checkText = checkText.Trim().ToLower();
            if (checkText == "error")
            {
                CombinedSteps.Failure($"The response from CommunicationExcel is an error: {response}");
                return string.Empty;
            }
            return response;
        }


        public static bool CompareDatabaseToExcel()
        {
            var excelDataDictionary = DataDictionaryStorage.GetDataDictionaryStore();
            if (excelDataDictionary == null || excelDataDictionary.Count == 0)
            {
                CombinedSteps.Failure($"No Data Dictionary found in storage.");
                return false;
            }
            // we have a data dictionary, now we need to get the table Names from the data 
            // go through each line of the data dictionary and get the physical Table Name
            // The data dictionary is the definition of the EXCEL!
            var currentTableName = "THIS TABLE DOES NOT EXIST";
            foreach (var excelRow in excelDataDictionary)
            {
                string? excelTableName = excelRow.PhysicalTableName?.Trim();
                if (string.IsNullOrEmpty(excelRow.PhysicalTableName) || excelTableName == null)
                {
                    CombinedSteps.Failure($"No Physical Table Name found in Data Dictionary row for Attribute {excelRow.AttributeName}");
                    return false;
                }
                if (currentTableName != excelTableName)
                {
                    // we now query the database directly to get the Database definition of the table
                    if (!TSQLDataCode.SetColumnAndKeys(excelTableName))
                    {
                        CombinedSteps.Failure($"The SQL Table {excelTableName} is not found in the database for columns and keys.");
                        return false;
                    }
                    currentTableName = excelTableName;
                    // now we have the table definition from the database, we can move on!                        
                }
                // now we have the table definition from the database, we can check the column exists
                var databaseTables = DescTableStorage.GetDescTableStore();
                if (databaseTables == null || databaseTables.Count == 0)
                {
                    CombinedSteps.Failure($"No DescTables found for table {excelTableName}");
                    return false;
                }
                // new we have the tables stories (list) we need the row we are looking for
                var databaseRow = databaseTables.FirstOrDefault(dt => dt.COLUMN_NAME.Equals(excelRow.PhysicalColumnName, StringComparison.OrdinalIgnoreCase));
                if (databaseRow == null)
                {
                    CombinedSteps.Failure($"Column {excelRow.PhysicalColumnName} not found in table {excelTableName}");
                    return false;
                }
                // now we have the row from the database, we can compare it to the data dictionary row
                // it is not that easy - the excel stores it as "varchar(40)" but the database stores the data_type it as "varchar" and "40" is stored in CHARACTER_MAXIMUM_LENGTH
                var XLdataType = excelRow.DataType?.Trim();
                if (string.IsNullOrEmpty(XLdataType) || XLdataType == null)
                {
                    CombinedSteps.Failure($"No Data Type found in Data Dictionary row for Attribute {excelRow.AttributeName} EVERYTHING has to have a data type");
                    return false;
                }
                var match = Regex.Match(XLdataType, @"^(?<type>\w+)(\((?<length>\d+)\))?$");
                if (!match.Success)
                {
                    CombinedSteps.Failure($"Data Type {XLdataType} in Data Dictionary row for Attribute {excelRow.AttributeName} is not in the correct format");
                    return false;
                }
                var XLType = match.Groups["type"].Value.ToLower();
                var XLlengthString = match.Groups["length"].Value.ToLower();
                var databaseDataType = databaseRow.DATA_TYPE.ToLower();
                var databaseLengthString = databaseRow.CHARACTER_MAXIMUM_LENGTH?.ToString().ToLower();
                if (XLType != databaseDataType)
                {
                    CombinedSteps.Failure($"Data Type {XLdataType} in Data Dictionary row for Attribute {excelRow.AttributeName} does not match database type {databaseRow.DATA_TYPE} in table {excelTableName} column {excelRow.PhysicalColumnName}");
                    return false;
                }
                if (XLlengthString != databaseLengthString)
                {
                    // if both are empty or null, then they match
                    if (!(string.IsNullOrEmpty(XLlengthString) && string.IsNullOrEmpty(databaseDataType)))
                    {
                        CombinedSteps.Failure($"Data Type Length {XLdataType} in Data Dictionary row for Attribute {excelRow.AttributeName} does not match database length {databaseRow.CHARACTER_MAXIMUM_LENGTH} in table {excelTableName} column {excelRow.PhysicalColumnName}");
                        return false;
                    }
                }
                // that is data type and length done, now we need to check nullable
                // excel stories it as true or false or empty.  Lets convert these to Yes or No, where empty is No
                var databaseNullableValue = databaseRow.IS_NULLABLE.Trim().ToLower();
                if (databaseNullableValue == "false" || databaseNullableValue == "no" || databaseNullableValue == null || databaseNullableValue == string.Empty)
                {
                    databaseNullableValue = "no";
                }
                else
                {
                    databaseNullableValue = "yes";
                }
                bool databaseNullableValueBool = false;
                if (databaseNullableValue == "yes") databaseNullableValueBool = true;

                // now I need to retrieve the excel value
                var excelNullableValueBool = excelRow.Nullable ?? false; // if null, then it is false

                if (databaseNullableValueBool != excelNullableValueBool)
                {
                    CombinedSteps.Failure($"Nullable value {excelRow.Nullable} in Data Dictionary row for Attribute {excelRow.AttributeName} does not match database nullable value {databaseRow.IS_NULLABLE} in table {excelTableName} column {excelRow.PhysicalColumnName}");
                    return false;
                }

                DebugOutput.Log($"Data Dictionary row for Attribute {excelRow.AttributeName} matches database table {excelTableName} column {excelRow.PhysicalColumnName}");     
            }
            return true;
        }







    }


}
