using System.Diagnostics;
using AppSpecFlow.AppSteps.DataFiles;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Generic.Elements.Steps.Button;
using Generic.Elements.Steps.Page;
using Generic.Elements.Steps.Textbox;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace AppSpecFlow.AppSteps
{
    [Binding]
    public class Steps : StepsBase
    {
        /// <summary>
        /// Step bindings for AppSpecFlow - orchestrates feature file creation and precondition data handling.
        /// </summary>
        public Steps(IStepHelpers helpers,
            GivenSteps givenSteps,
            ThenSteps thenSteps,

            WhenButtonSteps whenButtonSteps,
            GivenPageSteps givenPageSteps,
            WhenPageSteps whenPageSteps,
            ThenPageSteps thenPageSteps,
            WhenTextBoxSteps whenTextBoxSteps,

            UnitTests unitTests,
            FeatureFileListOfCSV featureFileListOfCSV

            ) : base(helpers)
        {
            GivenSteps = givenSteps;
            ThenSteps = thenSteps;

            WhenButtonSteps = whenButtonSteps;
            GivenPageSteps = givenPageSteps;
            WhenPageSteps = whenPageSteps;
            ThenPageSteps = thenPageSteps;
            WhenTextBoxSteps = whenTextBoxSteps;

            UnitTests = unitTests;
            FeatureFileListOfCSV = featureFileListOfCSV;

        }

        private GivenSteps GivenSteps { get; }
        private ThenSteps ThenSteps { get; }

        private WhenButtonSteps WhenButtonSteps { get; }
        private GivenPageSteps GivenPageSteps { get; }
        private WhenPageSteps WhenPageSteps { get; }
        private ThenPageSteps ThenPageSteps { get; }
        private WhenTextBoxSteps WhenTextBoxSteps { get; }

        private UnitTests UnitTests { get; }
        private FeatureFileListOfCSV FeatureFileListOfCSV { get; }


        private bool Failure(string proc, string message, bool pass = false)
        {
            // fixed typo in log string and made message clearer
            DebugOutput.Log($"We are failing in Steps.cs PROC '{proc}' with the message '{message}'");
            CombinedSteps.Failure(proc);
            return pass;
        }

        /// <summary>
        /// Ensure the given directory exists. Attempt to create if it does not.
        /// Returns true when directory exists (after creation attempt if necessary), false otherwise.
        /// </summary>
        private bool EnsureDirectory(string proc, string directory)
        {
            if (FileUtils.DirectoryCheck(directory)) return true;
            DebugOutput.Log($"Directory not found - creating: {directory}");
            if (!FileUtils.DirectoryCreation(directory)) return Failure(proc, $"Failed to create directory {directory}");
            if (!FileUtils.DirectoryCheck(directory)) return Failure(proc, $"Directory does not exist after creation: {directory}");
            return true;
        }

        [Given(@"STEPS HERE")]
        public void GivenSTEPSHERE()
        {
            // placeholder step used by features when no action is required
            DebugOutput.Log("NOTHING HERE");
        }


        [Given(@"Unit Test Is Executed In App")]
        public void GivenUnitTestIsExecutedInApp()
        {
            DebugOutput.Log($"We are running unit tests against app folder (methods)");
            var x = UnitTests.Hello();
            DebugOutput.Log($"UNIT TEST = {x}");

        }


        [Given(@"Feature File Created In Directory ""(.*)"" Named As ""(.*)"" Using PreCondition Data ""(.*)""")]
        public bool GivenFeatureFileCreatedInDirectoryNamedAsUsingPreConditionData(string featureFileDirectory, string testNumber, string preConditionDataLocation)
        {
            // Creates a single feature file from CSV precondition data
            featureFileDirectory = "/AppSpecFlow/Features" + featureFileDirectory;
            var proc = $"Given Feature File Created In Directory {featureFileDirectory} Named As {testNumber} Using PreCondition Data {preConditionDataLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.FileCheck(preConditionDataLocation)) return Failure(proc, $"Failed to find PreConditionDataLocation {preConditionDataLocation}");
                if (!EnsureDirectory(proc, featureFileDirectory)) return false;

                List<string>? csvDataLines = new();
                var csvFileOSLocation = FileUtils.GetCorrectDirectory(preConditionDataLocation);
                csvDataLines = CommaDelimited.GetCommaDelimitedData(csvFileOSLocation, true);
                if (csvDataLines == null) return Failure(proc, $"No data from the CSV File {csvFileOSLocation}");
                var listOfThisFeatureFile = FeatureFileListOfCSV.GetListOfLines(csvDataLines, 1, csvDataLines.Count());
                DebugOutput.Log($"THIS IS {listOfThisFeatureFile.Count} lines to be added to a single Feature File!");
                if (!FeatureFileListOfCSV.CSVListToFeatureFile(listOfThisFeatureFile, featureFileDirectory, testNumber)) return Failure(proc, "Failed to create the CSV FILE for single file!");
                return true;
            }
            return false;
        }



        [Given(@"Feature Files Created In Directory ""(.*)"" Named As ""(.*)"" Using PreCondition Data ""(.*)""")]
        public bool GivenFeatureFilesCreatedInDirectoryNamedAsUsingPreConditionData(string featureFileDirectory, string testNumbers, string preConditionDataLocation)
        {
            // Creates multiple feature files (named) from precondition CSV data
            featureFileDirectory = "/AppSpecFlow/Features" + featureFileDirectory;
            var proc = $"Given Feature Files Created In Directory {featureFileDirectory} Named As {testNumbers} Using PreCondition Data {preConditionDataLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.FileCheck(preConditionDataLocation)) return Failure(proc, $"Failed to find PreConditionDataLocation {preConditionDataLocation}");
                if (!EnsureDirectory(proc, featureFileDirectory)) return false;

                var numberOfTestFiles = StringValues.BreakUpByDelimitedToList(testNumbers, "|");
                var testFilesRequired = numberOfTestFiles.Count;
                var numberOfTestsPerFeature = 1;
                var remainder = 0;
                List<string>? csvDataLines = new();
                if (testFilesRequired > 0)
                {
                    var csvFileOSLocation = FileUtils.GetCorrectDirectory(preConditionDataLocation);
                    csvDataLines = CommaDelimited.GetCommaDelimitedData(csvFileOSLocation, true);
                    if (csvDataLines == null) return Failure(proc, $"No data from the CSV File {csvFileOSLocation}");
                    DebugOutput.Log($"We got CSV of {csvDataLines.Count} LINES");
                    if (csvDataLines.Count > 20)
                    {
                        DebugOutput.Log($"We are going to break you up!");
                        if (testFilesRequired > 1) testFilesRequired--;
                        numberOfTestsPerFeature = csvDataLines.Count / testFilesRequired;
                        remainder = csvDataLines.Count % testFilesRequired;
                    }
                    else
                    {
                        DebugOutput.Log($"We are not breaking you up!");
                        numberOfTestsPerFeature = csvDataLines.Count;
                    }
                }
                int counter = 0;
                foreach (var test in numberOfTestFiles)
                {
                    var listOfThisFeatureFile = FeatureFileListOfCSV.GetListOfLines(csvDataLines, counter, numberOfTestsPerFeature);
                    if (!FeatureFileListOfCSV.CSVListToFeatureFile(listOfThisFeatureFile, featureFileDirectory, test)) return Failure(proc, "Failed to create the CSV FILE!");
                    counter += numberOfTestsPerFeature;
                }
                return true;
            }
            return false;
        }


        [Given(@"Feature Files Created In Directory ""(.*)"" Based On PreCondition Data ""(.*)""")]
        public bool GivenFeatureFilesCreatedInDirectoryBasedOnPreConditionData(string featureFileDirectory, string preConditionDataLocation)
        {
            // Create feature files based on CSV filename
            featureFileDirectory = "/AppSpecFlow/Features" + featureFileDirectory;
            var proc = $"Given Feature Files Created In Directory {featureFileDirectory} Based On PreCondition Data {preConditionDataLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.FileCheck(preConditionDataLocation)) return Failure(proc, $"Failed to find PreConditionDataLocation {preConditionDataLocation}");
                if (!EnsureDirectory(proc, featureFileDirectory)) return false;

                // Read the CSV File
                var csvFileOSLocation = FileUtils.GetCorrectDirectory(preConditionDataLocation);
                var csvDataLines = CommaDelimited.GetCommaDelimitedData(csvFileOSLocation);
                if (csvDataLines == null) return Failure(proc, $"No data from the CSV File {csvFileOSLocation}");
                DebugOutput.Log($"We got CSV of {csvDataLines.Count} LINES");
                var brokenUpDirAndFile = StringValues.BreakUpByDelimitedToList(preConditionDataLocation, "//");
                var fileName = brokenUpDirAndFile[brokenUpDirAndFile.Count - 1];
                if (!FeatureFileListOfCSV.CSVListToFeatureFile(csvDataLines, featureFileDirectory, fileName)) return Failure(proc, "Failed to create the CSV FILE!");
                DebugOutput.Log($"All done and file created - hoping in the right place!");
                return true;
            }
            return false;
        }

        [Given(@"Variant Feature Files Created In Directory ""(.*)"" Based On PreCondition Data ""(.*)""")]
        public bool GivenVariantFeatureFilesCreatedInDirectoryBasedOnPreConditionData(string featureFileDirectory, string preConditionDataLocation)
        {
            // Create variant feature files (variants generated by FeatureFileListOfCSV)
            featureFileDirectory = "/AppSpecFlow/Features" + featureFileDirectory;
            var proc = $"Given Variant Feature Files Created In Directory {featureFileDirectory} Based On PreCondition Data {preConditionDataLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.FileCheck(preConditionDataLocation)) return Failure(proc, $"Failed to find PreConditionDataLocation {preConditionDataLocation}");
                if (!EnsureDirectory(proc, featureFileDirectory)) return false;

                // Read the CSV File
                var csvFileOSLocation = FileUtils.GetCorrectDirectory(preConditionDataLocation);
                var csvDataLines = CommaDelimited.GetCommaDelimitedData(csvFileOSLocation);
                if (csvDataLines == null) return Failure(proc, $"No data from the CSV File {csvFileOSLocation}");
                DebugOutput.Log($"We got CSV of {csvDataLines.Count} LINES");
                var brokenUpDirAndFile = StringValues.BreakUpByDelimitedToList(preConditionDataLocation, "//");
                var fileName = brokenUpDirAndFile[brokenUpDirAndFile.Count - 1];
                //CSVListToVariantFeatureFile is where the lines generated are defined
                if (!FeatureFileListOfCSV.CSVListToVariantFeatureFile(csvDataLines, featureFileDirectory, fileName)) return Failure(proc, "Failed to create the CSV FILE!");
                DebugOutput.Log($"All done and file created - hoping in the right place!");
                return true;
            }
            return false;
        }



        [Given(@"Feature Files Created In Directory ""(.*)"" Named As ""(.*)"" Using PreCondition Data ""(.*)"" Using API")]
        public bool GivenFeatureFilesCreatedInDirectoryNamedAsUsingPreConditionDataUsingAPI(string featureFileDirectory, string testNumbers, string preConditionDataLocation)
        {
            // Create API feature files named as provided
            featureFileDirectory = "/AppSpecFlow/Features" + featureFileDirectory;
            var proc = $"Given Feature Files Created In Directory {featureFileDirectory} Named As {testNumbers} Using PreCondition Data {preConditionDataLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.FileCheck(preConditionDataLocation)) return Failure(proc, $"Failed to find PreConditionDataLocation {preConditionDataLocation}");
                if (!EnsureDirectory(proc, featureFileDirectory)) return false;

                var numberOfTestFiles = StringValues.BreakUpByDelimitedToList(testNumbers, "|");
                var testFilesRequired = numberOfTestFiles.Count;
                var numberOfTestsPerFeature = 1;
                var remainder = 0;
                List<string>? csvDataLines = new();
                if (testFilesRequired > 0)
                {
                    var csvFileOSLocation = FileUtils.GetCorrectDirectory(preConditionDataLocation);
                    csvDataLines = CommaDelimited.GetCommaDelimitedData(csvFileOSLocation, true);
                    if (csvDataLines == null) return Failure(proc, $"No data from the CSV File {csvFileOSLocation}");
                    DebugOutput.Log($"We got CSV of {csvDataLines.Count} LINES");
                    if (csvDataLines.Count > 20)
                    {
                        DebugOutput.Log($"We are going to break you up!");
                        if (testFilesRequired > 1) testFilesRequired--;
                        numberOfTestsPerFeature = csvDataLines.Count / testFilesRequired;
                        remainder = csvDataLines.Count % testFilesRequired;
                    }
                    else
                    {
                        DebugOutput.Log($"We are not breaking you up!");
                        numberOfTestsPerFeature = csvDataLines.Count;
                    }
                }
                int counter = 0;
                foreach (var test in numberOfTestFiles)
                {
                    var listOfThisFeatureFile = FeatureFileListOfCSV.GetListOfLines(csvDataLines, counter, numberOfTestsPerFeature);
                    if (!FeatureFileListOfCSV.CSVListToAPIFeatureFile(listOfThisFeatureFile, featureFileDirectory, test)) return Failure(proc, "Failed to create the CSV FILE!");
                    counter += numberOfTestsPerFeature;
                }
                return true;
            }
            return false;
        }

        [Given(@"Feature Files Created In Directory ""(.*)"" Named As ""(.*)"" Using PreCondition Data ""(.*)"" Using API With Variants")]
        public bool GivenFeatureFilesCreatedInDirectoryNamedAsUsingPreConditionDataUsingAPIWithVariants(string featureFileDirectory, string testNumbers, string preConditionDataLocation)
        {
            // Create API variant feature files
            featureFileDirectory = "/AppSpecFlow/Features" + featureFileDirectory;
            var proc = $"Given Feature Files Created In Directory {featureFileDirectory} Named As {testNumbers} Using PreCondition Data {preConditionDataLocation}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.FileCheck(preConditionDataLocation)) return Failure(proc, $"Failed to find PreConditionDataLocation {preConditionDataLocation}");
                if (!EnsureDirectory(proc, featureFileDirectory)) return false;

                var numberOfTestFiles = StringValues.BreakUpByDelimitedToList(testNumbers, "|");
                var testFilesRequired = numberOfTestFiles.Count;
                var numberOfTestsPerFeature = 1;
                var remainder = 0;
                List<string>? csvDataLines = new();
                if (testFilesRequired > 0)
                {
                    var csvFileOSLocation = FileUtils.GetCorrectDirectory(preConditionDataLocation);
                    csvDataLines = CommaDelimited.GetCommaDelimitedData(csvFileOSLocation, true);
                    if (csvDataLines == null) return Failure(proc, $"No data from the CSV File {csvFileOSLocation}");
                    DebugOutput.Log($"We got CSV of {csvDataLines.Count} LINES");
                    if (csvDataLines.Count > 20)
                    {
                        DebugOutput.Log($"We are going to break you up!");
                        if (testFilesRequired > 1) testFilesRequired--;
                        numberOfTestsPerFeature = csvDataLines.Count / testFilesRequired;
                        remainder = csvDataLines.Count % testFilesRequired;
                    }
                    else
                    {
                        DebugOutput.Log($"We are not breaking you up!");
                        numberOfTestsPerFeature = csvDataLines.Count;
                    }
                }
                int counter = 0;
                foreach (var test in numberOfTestFiles)
                {
                    var listOfThisFeatureFile = FeatureFileListOfCSV.GetListOfLines(csvDataLines, counter, numberOfTestsPerFeature);
                    if (!FeatureFileListOfCSV.CSVListToAPIVariantFeatureFile(listOfThisFeatureFile, featureFileDirectory, test)) return Failure(proc, "Failed to create the CSV FILE!");
                    counter += numberOfTestsPerFeature;
                }
                return true;
            }
            return false;
        }




        [Given(@"PreCondition Data ""([^""]*)"" Exists")]
        public void GivenPreConditionDataExists(string nameOfPreconditionDataDirectory)
        {
            // Validate that a named precondition data directory exists
            var proc = $"Given PreConditionData {nameOfPreconditionDataDirectory} Exists";
            if (CombinedSteps.OuputProc(proc))
            {
                if (PreConditionData.PreConditionDataExists(nameOfPreconditionDataDirectory))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [When(@"I Create PreCondition Data (.*)")]
        public static void WhenICreatePreConditionData(int preConditionNumber)
        {
            // Create a numbered precondition data directory based on template
            var proc = $"When I Create PreConditionData {preConditionNumber}";
            if (CombinedSteps.OuputProc(proc))
            {
                string preConNumberAsString = preConditionNumber.ToString();
                string newNameOfPreconditionDataDirectory = PreConditionData.dataDir + preConNumberAsString;
                DebugOutput.Log($"Creating {newNameOfPreconditionDataDirectory}");
                if (!PreConditionData.PreConditionDataExists(newNameOfPreconditionDataDirectory))
                {
                    if (PreConditionData.CreatePreConditionData(newNameOfPreconditionDataDirectory, preConditionNumber))
                    {
                        return;
                    }
                }
                else
                {
                    DebugOutput.Log($"The directory {newNameOfPreconditionDataDirectory} already exists!  We can not create new precon data on top of already created data!");
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Given(@"PreCondition Data ""([^""]*)"" Is Used")]
        [When(@"I Use PreCondition Data ""([^""]*)""")]
        public static void WhenIUsePreConditionData(string nameOfPreconditionDataDirectory)
        {
            // Replace current local data with named precondition data
            var proc = $"When I Use PreConditionData {nameOfPreconditionDataDirectory}";
            if (CombinedSteps.OuputProc(proc))
            {
                var localDataDir = PreConditionData.fulldatDir;
                if (FileUtils.OSDeleteDirectoryIfExists(localDataDir))
                {
                    if (PreConditionData.PreConditionDataExists(nameOfPreconditionDataDirectory))
                    {
                        if (PreConditionData.UsePreConditionData(nameOfPreconditionDataDirectory))
                        {
                            return;
                        }
                    }
                    else
                    {
                        DebugOutput.Log($"PreCondata does not exists! {localDataDir}");
                        CombinedSteps.Failure(proc);
                        return;

                    }
                }
                else
                {
                    DebugOutput.Log($"Failed deleting {localDataDir}");
                    CombinedSteps.Failure(proc);
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

    }
}