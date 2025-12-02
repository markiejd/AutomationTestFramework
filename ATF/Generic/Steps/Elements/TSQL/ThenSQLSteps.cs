using System.Text.RegularExpressions;
using Core;
using Core.Logging;
using Generic.Elements.Steps.TSQL.Code;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.TSQL
{
    [Binding]
    public class ThenTSQLSteps : StepsBase
    {
        public ThenTSQLSteps(IStepHelpers helpers) : base(helpers)
        {
        }

                
        [Then(@"SQL Command ""(.*)"" Is Equal To ""(.*)""")]
        public void ThenSQLCommandIsEqualTo(string sqlCommand,string expectedValue)
        {
            string proc = $"Then SQL Command {sqlCommand} Is Equal To {expectedValue}";
            if (CombinedSteps.OuputProc(proc))
            {
                var returnedString = Helpers.TSQL.SendSQLCommand(sqlCommand, "", "");
                if (returnedString == null)
                {
                    CombinedSteps.Failure($"The SQL command returned null");
                    return;
                }
                DebugOutput.Log($"We have returned '{returnedString}' from the SQL command");
                // if the first 5 chars returned are "Error" then we have an error
                if (returnedString.ToLower().StartsWith("error"))
                {
                    CombinedSteps.Failure($"We have an error returned from the SQL command: {returnedString}");
                    return;
                }

                if (returnedString != expectedValue)
                {
                    CombinedSteps.Failure($"The SQL command returned '{returnedString}' which does not match expected value '{expectedValue}'");
                    return;
                }
            }   
            
        }



        [Then(@"Deployed Data Model matches Data Dictionary")]
        public void ThenDeployedDataModelmatchesDataDictionary()
        {
            string proc = $"Then Deployed Data Model matches Data Dictionary";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!TSQLDataCode.CompareDatabaseToExcel())
                {
                    CombinedSteps.Failure($"The Deployed Data Model does not match the Data Dictionary");
                    return;
                }
                return;
            }
        }


        [Then(@"Defined SQL Table Keys ""(.*)"" Has keyName ""(.*)"" ColumnName ""(.*)"" keyType ""(.*)""")]
        public void ThenDefinedSQLTableKeysHaskeyNameColumnNamekeyType(string tableName, string keyName, string columnName, string keyType)
        {
            string proc = $"Then Defined SQL Table Keys {tableName} Has keyName {keyName} ColumnName {columnName} keyType {keyType}";
            if (CombinedSteps.OuputProc(proc))
            {
                var descTableKeys = DescTableKeyStorage.GetDescTableKeyStore();
                if (descTableKeys == null || descTableKeys.Count == 0)
                {
                    CombinedSteps.Failure($"No DescTableKeys found for table {tableName}");
                    return;
                }
                
                // Step 1: Find by keyName
                var matchingKeyName = descTableKeys.FirstOrDefault(dt => dt.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (matchingKeyName == null)
                {
                    CombinedSteps.Failure($"Key {keyName} not found in table {tableName}");
                    return;
                }

                // Step 2: Find by columnName within the matching keyName
                var matchingColumnName = descTableKeys.FirstOrDefault(dt => 
                    dt.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase) &&
                    dt.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                if (matchingColumnName == null)
                {
                    CombinedSteps.Failure($"Column {columnName} not found for Key {keyName} in table {tableName}");
                    return;
                }

                // Step 3: Find by keyType within the matching keyName and columnName
                var matchingKeyType = descTableKeys.FirstOrDefault(dt => 
                    dt.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase) &&
                    dt.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase) &&
                    dt.KeyType.Equals(keyType, StringComparison.OrdinalIgnoreCase));
                if (matchingKeyType == null)
                {
                    CombinedSteps.Failure($"Key Type {keyType} not found for Key {keyName} and Column {columnName} in table {tableName}");
                    return;
                }
            }
            return;
        }

        
        [Then(@"Defined SQL Table ""(.*)"" Has Column ""(.*)"" With Data Type ""(.*)"" And Max Length ""(.*)"" And Is Nullable ""(.*)""")]
        public void ThenDefinedSQLTableHasColumnWithDataTypeAndMaxLengthAndIsNullable(string tableName,string columnName,string dataType,string maxLength,string nullableString)
        {
            string proc = $"Then Defined SQL Table {tableName} Has Column {columnName} With Data Type {dataType} And Max Length {maxLength} And Is Nullable {nullableString}";
            
            if (CombinedSteps.OuputProc(proc))
            {
                if (maxLength.ToLower() == "null")
                {
                    maxLength = string.Empty; // Handle "null" as an empty string for comparison
                }

                var descTables = DescTableStorage.GetDescTableStore();
                if (descTables == null || descTables.Count == 0)
                {
                    CombinedSteps.Failure($"No DescTables found for table {tableName}");
                    return;
                }

                DescTable? descTable = new DescTable();

                if (maxLength != string.Empty && !int.TryParse(maxLength, out _))
                {
                    descTable = descTables.FirstOrDefault(dt => dt.COLUMN_NAME.Equals(columnName, StringComparison.OrdinalIgnoreCase) &&
                                                            dt.DATA_TYPE.Equals(dataType, StringComparison.OrdinalIgnoreCase) &&
                                                            dt.CHARACTER_MAXIMUM_LENGTH.ToString() == maxLength &&
                                                            dt.IS_NULLABLE.Equals(nullableString, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    descTable = descTables.FirstOrDefault(dt => dt.COLUMN_NAME.Equals(columnName, StringComparison.OrdinalIgnoreCase) &&
                                                            dt.DATA_TYPE.Equals(dataType, StringComparison.OrdinalIgnoreCase) &&
                                                            dt.IS_NULLABLE.Equals(nullableString, StringComparison.OrdinalIgnoreCase));
                }

                

                if (descTable == null)
                {
                    CombinedSteps.Failure($"Column {columnName} with Data Type {dataType}, Max Length {maxLength}, and Is Nullable {nullableString} not found in table {tableName}");
                    return;
                }

            }
        }
        




    }
}
