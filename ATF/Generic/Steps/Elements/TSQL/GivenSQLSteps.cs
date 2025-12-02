using Core;
using Core.FileIO;
using Core.Logging;
using Generic.Elements.Steps.TSQL.Code;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.TSQL
{
    [Binding]
    public class GivenTSQLSteps : StepsBase
    {
        public GivenTSQLSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        [Given(@"Data Dictionary xlsx file ""(.*)""")]
        public void GivenDataDictionaryxlsxfile(string fileLocation)
        {
            string proc = $"Given Data Dictionary xlsx file {fileLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                var excelJson = TSQLDataCode.ReadDataDictionaryFromExcel(fileLocation);
                if (string.IsNullOrEmpty(excelJson) || excelJson.Length < 5)
                {
                    CombinedSteps.Failure($"The Excel file {fileLocation} could not be read or is empty.");
                    return;
                }
                var dataDictionary = DataDictionaryRow.DataDictionaryFromJson(excelJson);
                if (dataDictionary == null || dataDictionary.Count == 0)
                {
                    CombinedSteps.Failure($"The Excel file {fileLocation} could not be converted to a Data Dictionary.");
                    return;
                }
                if (!DataDictionaryStorage.SetDataDictionaryStore(dataDictionary))
                {
                    CombinedSteps.Failure($"The Data Dictionary could not be stored from the Excel file {fileLocation}.");
                    return;
                }
                DebugOutput.Log($"The DataDictionaryStorage is set with {dataDictionary.Count} items.");
                return;
            }
        }


        [Given(@"SQL Table ""(.*)"" Is Defined")]
        public void GivenSQLTableIsDefined(string tableName)
        {
            string proc = $"Given SQL Table {tableName} Is Defined";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!TSQLDataCode.SetColumnAndKeys(tableName))
                {
                    CombinedSteps.Failure($"The SQL Table {tableName} is not defined for columns and keys.");
                    return;
                }
                return;
            }
        }




    }
}
