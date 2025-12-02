
using Core;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Steps
{
    [Binding]
    public class ThenSteps : StepsBase
    {
        public ThenSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        private bool Failure(string proc, string message, bool pass = false)
        {
            DebugOutput.Log($"Failure in ThenSteps {message}");
            CombinedSteps.Failure(proc);
            return pass;
        }

        [Then(@"Wait (.*) Seconds")]
        public void ThenWaitSeconds(int seconds)
        {
            seconds *= 1000;
            Thread.Sleep(seconds);
        }


        [Then(@"Wait ""([^""]*)"" Seconds")]
        public void ThenWaitSeconds(string secondsText)
        {
            var seconds = int.Parse(secondsText);
            seconds *= 1000;
            Thread.Sleep(seconds);
        }

        [Then(@"More To Do Here")]
        public void ThenMoreToDoHere()
        {
            Assert.Inconclusive();
        }
                
        [Then(@"CSV File ""(.*)"" Has (.*) Headers")]
        public void ThenCSVFileHasHeaders(string fullFilePath,int expectedNumberOfHeaders)
        {
            string proc = $"Then CSV File {fullFilePath} Has {expectedNumberOfHeaders} Headers";
            if (CombinedSteps.OuputProc(proc))
            {
                var actualColumns = GetNumberOfHeadersInCSVFile(fullFilePath, "¤");
                if (actualColumns != expectedNumberOfHeaders)
                {
                    DebugOutput.Log($"Failed to match column count {actualColumns}  with {expectedNumberOfHeaders}");
                    CombinedSteps.Failure(proc);
                }
            }
        }

        private int GetNumberOfHeadersInCSVFile(string fullFilePath, string delimiter = "|")
        {
            DebugOutput.Log($"Proc -GetNumberOfHeadersInCSVFile");
            using (var reader = new StreamReader(fullFilePath))
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    DebugOutput.Log($"Failed to find ANY lines!");
                    return -1;
                }
                var values = StringValues.BreakUpByDelimitedToList(line, delimiter);
                return values.Count();
            }
        }

                
        [Then(@"Excel File ""(.*)"" Cell ""(.*)"" Sheet ""(.*)"" Equal To ""(.*)""")]
        public void ThenExcelFileCellSheetEqualTo(string osExcelFile,string cell,string sheetName,string value)
        {
            string proc = $"Then Excel File {osExcelFile} Cell {cell} Sheet {sheetName} Equal To {value}";
            if (CombinedSteps.OuputProc(proc))
            {
                DebugOutput.Log($"We have the file {osExcelFile} cell {cell} sheet {sheetName} equal to {value}");
                if (!ThenExcelFileContainsSheet(osExcelFile, sheetName))
                {
                    DebugOutput.Log($"Failed to find sheet {sheetName}");
                    CombinedSteps.Failure(proc);
                    return;
                }
                DebugOutput.Log($"We have the sheet {sheetName}");
                var response = CmdUtil.ExecuteDotnet("./CommunicationExcel/CommunicationExcel.csproj", $"\"readcell\" \"{osExcelFile}\" \"{sheetName}\" \"{cell}\"").Trim();
                DebugOutput.Log($"Response was {response}");
                if (response.ToLower().StartsWith("error"))
                {
                    DebugOutput.Log($"Failed to get cell {response}");
                    CombinedSteps.Failure(proc);
                    return;
                }
                DebugOutput.Log($"We have the cell value {response}");
                if (response != value)
                {
                    DebugOutput.Log($"Did not match actual {response} and wanted {value}");
                    CombinedSteps.Failure(proc);
                    return;
                }
                DebugOutput.Log($"We matched actual {response} and wanted {value}");
            }
        }


        
        [Then(@"Excel File ""(.*)"" Contains Sheet ""(.*)""")]
        public bool ThenExcelFileContainsSheet(string osExcelFile,string sheetName)
        {
            string proc = $"Then Excel File {osExcelFile} Contains Sheet {sheetName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.OSFileCheck(osExcelFile))
                {
                    DebugOutput.Log($"Failed to find file {osExcelFile}");
                    CombinedSteps.Failure(proc);
                    return false;
                }
                DebugOutput.Log($"We have the file");
                var response = CmdUtil.ExecuteDotnet("./CommunicationExcel/CommunicationExcel.csproj", $"\"getsheets\" \"{osExcelFile}\"").Trim();
                DebugOutput.Log($"Response was {response}");
                if (response.ToLower().StartsWith("error"))
                {
                    DebugOutput.Log($"Failed to get sheets {response}");
                    CombinedSteps.Failure(proc);
                    return false;
                }
                var sheets = response.Split(',').Select(s => s.Trim()).ToList();
                DebugOutput.Log($"We have {sheets.Count} sheets");
                foreach (var sheet in sheets)
                {
                    // remove any doubel quotes
                    var thisSheet = sheet.Replace("\"", "");
                    DebugOutput.Log($"THIS SHEET IS '{thisSheet}' comparing to '{sheetName}'");
                    if (thisSheet == sheetName)
                    {
                        DebugOutput.Log($"We found the sheet {thisSheet}");
                        return true;
                    }
                }
                DebugOutput.Log($"We Did NOT find the sheet {sheetName}");
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }


        
        // [Then(@"Output Text To Text From Word Document ""(.*)""")]
        // public bool ThenOutputTextToTextFromWordDocument(string fullFilePathAndName)
        // {
        //     string proc = $"Then Output Text To Text From Word Document {fullFilePathAndName}";
        //     if (CombinedSteps.OuputProc(proc))
        //     {
        //         if (ThenWordDocumentExists(fullFilePathAndName))
        //         {
        //             var text = MicrosoftWord.GetTextFromWordDocument(fullFilePathAndName);
        //             if (text == null) return Failure(proc, "Failed to get text from {fullFilePathAndName}");
        //             DebugOutput.Log($"The TEXT IS {text}");
        //             var directory = "/AppXAPI/APIOutFiles/";
        //             var fileName = "WordText.txt";
        //             if (!FileUtils.OSAppendLineToFile(directory+fileName, text))  return Failure(proc, $"Failed to create and or populate file {fileName} in directory {directory}");
        //             DebugOutput.Log($"File created!");
        //             return true;
        //         }
        //     }
        //     return false;
        // }


        
        [Then(@"Word Document ""(.*)"" Exists")]
        public bool ThenWordDocumentExists(string fullFilePathAndName)
        {
            string proc = $"Then Word Document {fullFilePathAndName} Exists";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.OSFileCheck(fullFilePathAndName))
                {
                    DebugOutput.Log($"FILE NOT FOUND {fullFilePathAndName}");
                    CombinedSteps.Failure(proc);
                    return false;
                }
                return true;
            }
            return false;
        }


        
        [Then(@"CSV File ""(.*)"" Contains (.*) Rows Not Including Header")]
        public void ThenCSVFileContainsRowsNotIncludingHeader(string fullFileName,int expectedNumberOfRows)
        {
            string proc = $"Then CSV File {fullFileName} Contains {expectedNumberOfRows} Not Including Header";
            if (CombinedSteps.OuputProc(proc))
            {
                var NumberOfLines = FileUtils.GetNumberOfLinesInFile(fullFileName, true, false);
                DebugOutput.Log($"We have {NumberOfLines} LINES");
                if (NumberOfLines != expectedNumberOfRows)
                {
                    DebugOutput.Log($"WE wanted {expectedNumberOfRows} but got {NumberOfLines}");
                    CombinedSteps.Failure(proc);
                }
            }
        }

        
        [Then(@"Number Of Tabs In Browser Is Equal To (.*)")]
        public bool ThenNumberOfTabsInBrowserIsEqualTo(int numberOfTabs)
        {
            string proc = $"Then Number Of Tabs In Browser Is Equal To {numberOfTabs}";
            if (CombinedSteps.OuputProc(proc))
            {
                var actualNumberOfTabs = ElementInteraction.GetTheNumberOfTabsOpenInBrowser();
                if (numberOfTabs != actualNumberOfTabs)
                {
                    DebugOutput.Log($"Did not match actual {actualNumberOfTabs} and wanted {numberOfTabs}");
                    CombinedSteps.Failure(proc);
                    return false;
                }
                return true;
            }
            return false;
        }





        

        




        



    }
}
