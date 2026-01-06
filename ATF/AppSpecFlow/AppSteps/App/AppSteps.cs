using System.Diagnostics;
using AppXAPI;
using Core;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Generic.Elements.Steps.Button;
using Generic.Elements.Steps.Page;
using Generic.Elements.Steps.Table;
using Generic.Elements.Steps.Textbox;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using OpenQA.Selenium.DevTools;

namespace AppSpecFlow.AppSteps
{

    /// <summary>
    /// AppSteps contains SpecFlow step implementations that interact with ADF, files and generic UI steps.
    /// Comments were added to clarify responsibilities of the most important methods. No behavior changes made.
    /// </summary>
    [Binding]
    public class AppSteps : StepsBase
    {
        /// <summary>
        /// Constructor - wires up helper step classes used by the step definitions.
        /// </summary>
        public AppSteps(IStepHelpers helpers,
            GivenSteps givenSteps,
            WhenSteps whenSteps,
            ThenSteps thenSteps,

            WhenButtonSteps whenButtonSteps,
            GivenPageSteps givenPageSteps,
            WhenPageSteps whenPageSteps,
            ThenPageSteps thenPageSteps,
            ThenTableSteps thenTableSteps, 
            WhenTextBoxSteps whenTextBoxSteps

            ) : base(helpers)
        {
            GivenSteps = givenSteps;
            WhenSteps = whenSteps;
            ThenSteps = thenSteps;

            WhenButtonSteps = whenButtonSteps;
            GivenPageSteps = givenPageSteps;
            WhenPageSteps = whenPageSteps;
            ThenPageSteps = thenPageSteps;
            ThenTableSteps = thenTableSteps;
            WhenTextBoxSteps = whenTextBoxSteps;

        }

        private GivenSteps GivenSteps { get; }
        private WhenSteps WhenSteps { get; }
        private ThenSteps ThenSteps { get; }

        private WhenButtonSteps WhenButtonSteps { get; }
        private GivenPageSteps GivenPageSteps { get; }
        private WhenPageSteps WhenPageSteps { get; }
        private ThenPageSteps ThenPageSteps { get; }
        private ThenTableSteps ThenTableSteps { get; }
        private WhenTextBoxSteps WhenTextBoxSteps { get; }

        // ADF ingestion directory names - marked readonly since these are constants for the class
        private readonly string ADFSI003Directory = "Upload of Customer and Contract detail_SI003";
        private readonly string ADFMI003Directory = "Account Billing Preferences & Account Adjustments Preferences Upload_MI003";
        private readonly string ADFBL002Directory = "TEIP Invoice data_BL002";
        private readonly string ADFBL001Directory = "SPID Core Corrections Upload_BL001";
        private readonly string ADFMI007Directory = "Tariff Data Upload & Tariff standing data_MI007";


        /// <summary>
        /// Step to deliberately fail the test with a message (used for debugging/flags).
        /// </summary>
        [Then(@"FAIL here because ""(.*)""")]
        public void ThenFAILherebecause(string args1)
        {
            DebugOutput.Log($"*****   FAILURE FLAG RAISED CAUSE {args1}   *****");
            CombinedSteps.Failure(args1);
        }

                private string GetADFDateDirectoryStructure()
        {
            // we need to get todays date broken into yyyy mm dd
            var today = DateTime.Today;
            var yyyy = today.Year.ToString();
            var mm = today.Month.ToString("D2");
            var dd = today.Day.ToString("D2");
            // Directory Structure will be
            var directoryDateStructure = $"{yyyy}/{mm}/{dd}/";
            return directoryDateStructure;
        }

        /// <summary>
        /// Insert today's date into a filename before the extension. Handles rejected/incomplete naming rules.
        /// </summary>
        private string GetFileNameWithDateStamp(string fileName, string processType = "Upload", string ingestionType = "BL001")
        {
            DebugOutput.OutputMethod("GetFileNameWithDateStamp", $"{fileName}, {processType} {ingestionType}");
            // fileName is like MI003_HX25_7983_Update_Existing_Account
            // we need to get todays date broken into yyyy mm dd
            var today = DateTime.Today;
            var yyyy = today.Year.ToString();
            var mm = today.Month.ToString("D2");
            var dd = today.Day.ToString("D2");
            // we need to insert _yyyyMMdd before the file extension
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fileExtension = Path.GetExtension(fileName);
            string? newFileName = null;
            if (processType.ToLower() == "rejected")
            {
                if ((ingestionType.ToLower() != "bl001" && ingestionType.ToLower() != "bl002") || ingestionType.ToLower() == "mi003" || ingestionType.ToLower() == "mi007")
                {
                    newFileName = $"{fileNameWithoutExtension}_Error_{yyyy}{mm}{dd}{fileExtension}";
                }
                else
                {
                    newFileName = $"{fileNameWithoutExtension}_{yyyy}{mm}{dd}{fileExtension}";
                }
            }
            else if (processType.ToLower() == "incomplete")
            {
                newFileName = $"{fileNameWithoutExtension}_Error_{yyyy}{mm}{dd}{fileExtension}";
            }
            else
            {
                newFileName = $"{fileNameWithoutExtension}_{yyyy}{mm}{dd}{fileExtension}";
            } 
            return newFileName;
        }
      

        /// <summary>
        /// Map a short ingestion type to the ADF directory name used in this project.
        /// </summary>
        private string GetIngestionTypeDirectory(string ingestionType)
        {
            DebugOutput.OutputMethod("GetIngestionTypeDirectory", $"{ingestionType}");
            var ingestionTypeDirectory = "";
            switch (ingestionType.ToLower())
            {
                case "bl001":
                    {
                        ingestionTypeDirectory = ADFBL001Directory;
                        return ingestionTypeDirectory;
                    }
                case "bl002":
                    {
                        ingestionTypeDirectory = ADFBL002Directory;
                        return ingestionTypeDirectory;
                    }
                case "si003":
                    {
                        ingestionTypeDirectory = ADFSI003Directory;
                        return ingestionTypeDirectory;
                    }
                case "mi003":
                    {
                        ingestionTypeDirectory = ADFMI003Directory;
                        return ingestionTypeDirectory;
                    }
                case "mi007":
                    {
                        ingestionTypeDirectory = ADFMI007Directory;
                        return ingestionTypeDirectory;
                    }
                default:
                    {
                        return "";      
                    }
            }   
        }

                private string? FindFileInADF(string newFileName, string fullDirectoryStructure)
        {
            DebugOutput.OutputMethod("FindFileInADF", $"{newFileName} {fullDirectoryStructure}");
            // get all files in the directory
            var allFiles = ADFUtils.GetAllFileNamesInADFDirectory(fullDirectoryStructure);
            if (allFiles.Count == 0)
            {
                DebugOutput.Log($"No files found in ADF directory {fullDirectoryStructure} OR Error!");
                return null;
            }
            DebugOutput.Log($"We have Found {allFiles.Count} files in ADF directory {fullDirectoryStructure}");
            var newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);
            DebugOutput.Log($"Looking for {newFileNameWithoutExtension} in the file list");
            foreach (var file in allFiles)
            {
                DebugOutput.Log($"Checking file {file} starting with {fullDirectoryStructure + newFileNameWithoutExtension}");
                if (file.StartsWith(fullDirectoryStructure + newFileNameWithoutExtension))
                {
                    DebugOutput.Log($"File {file} starts with {newFileNameWithoutExtension}");
                    return string.Join(Environment.NewLine, allFiles);
                }
            }
            DebugOutput.Log($"File {newFileName} was not found in ADF directory! {fullDirectoryStructure}");
            return null;
        }

        /// <summary>
        /// Extract the full file name (including timestamp) from a joined string of files returned by ADF listing.
        /// </summary>
        private string IsFileFoundInStringOfFiles(string newFileNameWithoutExtension, string? allFiles, string extensionType = ".csv")
        {
            DebugOutput.OutputMethod("IsFileFoundInArrayOfFiles", $"'{newFileNameWithoutExtension}' '{allFiles}'");
            string newFileName = "";
            if (allFiles == null)
            {
                DebugOutput.Log("All Files are NULL");
                return "";
            }
            if (allFiles.Contains("error")
                || allFiles.Contains("Exception"))
            {
                DebugOutput.Log("cONTAINS eRROR");
                return "";
            }   
            if (allFiles.Count() == 0)
            {
                DebugOutput.Log("count 0");
                return "";
            }
            DebugOutput.Log($"Hello");
            var searchString = newFileNameWithoutExtension + "_";
            var startIndex = allFiles.IndexOf(searchString);
            DebugOutput.Log($"rgfsk;liuitg;");
            if (startIndex == -1)
            {
                CombinedSteps.Failure($"File {newFileName} was found in ADF directory but could not find the full file name with time stamp");
                return "";
            }
            DebugOutput.Log($"kjhoihjp");
            var endIndex = allFiles.IndexOf(extensionType, startIndex);
            DebugOutput.Log($"qpqpqpqpq");
            if (endIndex == -1)
            {
                DebugOutput.Log($"wmememrmemr");
                CombinedSteps.Failure($"File {newFileName} was found in ADF directory but could not find the full file name with time stamp");
                return "";
            }
            DebugOutput.Log($"ljvghfkljuycvfkcgf");
            var fullFileNameWithTimeStamp = allFiles.Substring(startIndex, endIndex - startIndex + 4);
            DebugOutput.Log($"Full file name with time stamp is {fullFileNameWithTimeStamp}");
            return fullFileNameWithTimeStamp;
        }

        /// <summary>
        /// Download a file from ADF to a local test results folder, then delete it from ADF when successful.
        /// </summary>
        private bool DownloadAFileFromADF(string processType, string ingestionType, string fullDirectoryStructure, string fullFileNameWithTimeStamp)
        {
            DebugOutput.OutputMethod("DownloadAFileFromADF", $"");
            // now we need to download the file from ADF to a local file
            var localDirectory = FileUtils.GetRepoDirectory() + $"/AppSpecFlow/TestResults/{processType}/";
            if (!FileUtils.OSDirectoryCheck(localDirectory))
            {
                FileUtils.OSDirectoryCreation(localDirectory);
            }
            var localFileNameAndPath = localDirectory + fullFileNameWithTimeStamp;
            localFileNameAndPath = localFileNameAndPath.Replace("\\", "/");
            DebugOutput.Log($"Local file name and path is {localFileNameAndPath} and ADF file is {fullDirectoryStructure + fullFileNameWithTimeStamp}");

            if (!ADFUtils.DownloadAFileFromADF(fullDirectoryStructure + fullFileNameWithTimeStamp, localFileNameAndPath))
            {
                CombinedSteps.Failure($"File {fullFileNameWithTimeStamp} could not be downloaded from ADF directory {fullDirectoryStructure} to local file {localFileNameAndPath}");
                return false;
            }

            if (!FileUtils.OSFileCheck(localFileNameAndPath))
            {
                CombinedSteps.Failure($"File {fullFileNameWithTimeStamp} was downloaded from ADF directory {fullDirectoryStructure} to local file {localFileNameAndPath} but the local file does not exist");
                return false;
            }

            DebugOutput.Log($"File {fullFileNameWithTimeStamp} was downloaded from ADF directory {fullDirectoryStructure} to local file {localFileNameAndPath}");

            // now we need to delete that file from ADF
            if (!ADFUtils.DeleteAFileInADF(fullDirectoryStructure + fullFileNameWithTimeStamp))
            {
                CombinedSteps.Failure($"File {fullFileNameWithTimeStamp} could not be deleted from ADF directory {fullDirectoryStructure} after being downloaded to local file {localFileNameAndPath}");
                return false;
            }
            DebugOutput.Log($"File {fullFileNameWithTimeStamp} was deleted from ADF directory {fullDirectoryStructure} after being downloaded to local file {localFileNameAndPath}");
            return true;
        }


        /// <summary>
        /// Wait for a file to appear in ADF, then download it. Respects PositiveTimeout * 6.
        /// </summary>
        private bool ThenWaitForFileInADF(string fileName, string processType = "Upload", string ingestionType = "SI003")
        {
            DebugOutput.OutputMethod("ThenWaitForFileInADF", $"{fileName}, {processType} {ingestionType}");
            var ingestionTypeDirectory = GetIngestionTypeDirectory(ingestionType);
            if (ingestionTypeDirectory == "")
            {
                CombinedSteps.Failure($"Ingestion type {ingestionType} is not recognised. Supported types are SI003, MI003, BL002");
                return false;
            }
            var fullDirectoryStructure = $"{processType}/{ingestionTypeDirectory}/" + GetADFDateDirectoryStructure();
            DebugOutput.Log($"Full directory structure is {fullDirectoryStructure}");
            var newFileName = GetFileNameWithDateStamp(fileName, processType, ingestionType);
            DebugOutput.Log($"New file name with date stamp is {newFileName}");

            bool found = false;
            int timeout = TargetConfiguration.Configuration.PositiveTimeout * 6; // default is 30 seconds * 6 = 3 minutes
            // get the time now - loop round until the time = time now + timeout in seconds
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(timeout);

            DebugOutput.Log($"Start time is {startTime} end time is {endTime}");
            string? allFiles = null;
            var newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);

            // give me extension of newFileName
            var fileExtension = Path.GetExtension(newFileName);
            DebugOutput.Log($"File extension is {fileExtension}");

            while (DateTime.Now < endTime)
            {
                if (FindFileInADF(newFileName, fullDirectoryStructure) == "")
                {
                    DebugOutput.Log($"Did not find file {newFileName} in ADF directory {fullDirectoryStructure} waiting 10 seconds before checking again");
                    System.Threading.Thread.Sleep(10000);
                }
                else
                {
                    found = true;
                    allFiles = FindFileInADF(newFileName, fullDirectoryStructure);
                    break;
                }
            }
            if (!found)
            {
                CombinedSteps.Failure($"File {newFileName} was not found in ADF directory {fullDirectoryStructure} after waiting {timeout} seconds");
                return false;
            }
            DebugOutput.Log($"File {newFileName} was found in ADF directory {fullDirectoryStructure}");

            var fullFileNameWithTimeStamp = IsFileFoundInStringOfFiles(newFileNameWithoutExtension, allFiles, fileExtension);
            if (fullFileNameWithTimeStamp == "")
            {
                CombinedSteps.Failure($"File {newFileName} was found in ADF directory {fullDirectoryStructure} but could not get the full file name with time stamp");
                return false;
            }
            DebugOutput.Log($"Full file name with time stamp is {fullFileNameWithTimeStamp}");

            if (!DownloadAFileFromADF(processType, ingestionType, fullDirectoryStructure, fullFileNameWithTimeStamp))
            {
                CombinedSteps.Failure($"File {newFileName} could not be downloaded from ADF directory {fullDirectoryStructure}");
                return false;
            }
            DebugOutput.Log($"File {newFileName} was processed in ADF directory {fullDirectoryStructure}");
            return true;
        }

        [Then(@"Wait For File ""(.*)"" To Be ""(.*)"" By ""(.*)""")]
        public void ThenWaitForFileToBeBy(string fileName, string process, string ingestionType)
        {
            string proc = $"Then Wait For File {fileName} To Be {process} By {ingestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!ThenWaitForFileInADF(fileName, process, ingestionType))
                {
                    CombinedSteps.Failure($"File {fileName} was not found in ADF as {process} for ingestion type {ingestionType}");
                    return;
                }
                DebugOutput.Log($"File {fileName} was {process} by {ingestionType} and downloaded from ADF to local file");
            }
        }
        
                
        [Given(@"Table ""(.*)"" Row (.*) Is Ready For View Data Correction")]
        public void GivenTableRowIsReadyForViewDataCorrection(string tableName,int rowNumber)
        {
            string proc = $"Given Table {tableName} Row {rowNumber} Is Ready For View Data Correction";
            if (CombinedSteps.OutputProc(proc))
            {
                // find the table
                ThenTableSteps.ThenTableIsDisplayed(tableName);


                // get the now from the table  rowNumber

                // then I need to find out is the view data correction displayed in that row

                // if it is - good all done

                // if it is not - I need to approve it
            }
        }




        /// <summary>
        /// Upload a local repo file into ADF input directory. Ensures file existence and sets AZURE_STORAGE_FILESYSTEM.
        /// </summary>
        public bool InputFileForingestion(string fileToBeUploaded, string ADFInputDirectory)
        {
            DebugOutput.OutputMethod("InputFileForingestion", $"{fileToBeUploaded}, {ADFInputDirectory}");
            // we need to get the full linux directory for the fileToBeUploaded
            var fullFileNameAndPath = FileUtils.GetRepoDirectory() + fileToBeUploaded;
            fullFileNameAndPath = fullFileNameAndPath.Replace("\\", "/");
            DebugOutput.Log($"Full file name and path to be uploaded is {fullFileNameAndPath}");
            if (!FileUtils.OSFileCheck(fullFileNameAndPath))
            {
                CombinedSteps.Failure($"File to be uploaded {fullFileNameAndPath} does not exist");
                return false;
            }
            // we need the directory to place it in ADF
            var fileSystemName = VariableConfiguration.Configuration.FileSystemName;
            if (fileSystemName == "UNKNOWN")
            {
                CombinedSteps.Failure($"FileSystemName is not set in the variable configuration for this environment can not use CommunicationADF without this variable");
                return false;
            }
            // set the environment variable AZURE_STORAGE_FILESYSTEM to equal fileSystemName
            Environment.SetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM", fileSystemName);
            var fileName = Path.GetFileName(fullFileNameAndPath);
            var finalLocation = ADFInputDirectory + fileName;
            DebugOutput.Log($"File Location in ADF is {finalLocation}");
            // now we need to upload the file to ADF                
            if (!ADFUtils.UploadAFileToADF(fullFileNameAndPath, finalLocation))
            {
                CombinedSteps.Failure($"File to be uploaded {fullFileNameAndPath} could not be uploaded to ADF");
                return false;
            }
            DebugOutput.Log($"File to be uploaded {fullFileNameAndPath} was uploaded to ADF {finalLocation}");
            return true;
        }


        [When(@"I Unzip File ""(.*)"" By ""(.+)""")]
        public void WhenIUnzipFileBy(string repoZipFile, string ingestionType)
        {
            string proc = $"When I Unzip File {repoZipFile} By {ingestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                // repoZipFile is something like /AppSpecFlow/TestResults/Rejected/HX25-8744_BL001_corrupted_Unreadable.zip
                // we need just the path not the file name
                var tmpFileName = Path.GetFileName(repoZipFile);
                if (tmpFileName == null)
                {
                    CombinedSteps.Failure($"Could not get the file name from the file name {repoZipFile}");
                    return;
                }
                var repoZipFileDir = repoZipFile.Replace(tmpFileName, "");
                DebugOutput.Log($"Directory where zip file is {repoZipFileDir}");
                // the repoZipFile is something like /AppSpecFlow/TestResults/Rejected/HX25-8744_BL001_corrupted_Unreadable.zip
                // we need the repo directory
                repoZipFile = FileUtils.GetRepoDirectory() + "/" + repoZipFile;
                repoZipFile = repoZipFile.Replace("\\", "/");
                repoZipFile = repoZipFile.Replace("//", "/");
                DebugOutput.Log($"Full repo zip file after replace is {repoZipFile}");
                var fileName = Path.GetFileNameWithoutExtension(repoZipFile);
                var fileExtension = Path.GetExtension(repoZipFile);
                DebugOutput.Log($"File name without extension is {fileName} and file extension is {fileExtension} and found in the repo at {repoZipFileDir}");
                // confirm the file in the directory exists within the repo as an OS File
                var osDirectory = FileUtils.GetRepoDirectory() + repoZipFileDir;
                osDirectory = osDirectory.Replace("\\", "/");
                DebugOutput.Log($"OS Directory is {osDirectory}");
                if (!FileUtils.OSDirectoryCheck(osDirectory))
                {
                    CombinedSteps.Failure($"Directory where zip file is {osDirectory} does not exist");
                    return;
                }
                // get a list of all files found in the osDirectory
                var allFiles = FileUtils.OSGetListOfFilesInDirectoryOfType(osDirectory, fileExtension);
                DebugOutput.Log($"All files with {fileExtension} extension in directory {osDirectory} are {allFiles.Count}");
                var allNamedFiles = new List<string>();
                foreach (var file in allFiles)
                {
                    DebugOutput.Log($"Checking file {file}");
                    if (file.StartsWith(fileName) && file.EndsWith(fileExtension))
                    {
                        DebugOutput.Log($"File {file} starts with {fileName} and ends with {fileExtension}");
                        allNamedFiles.Add(file);
                    }
                }
                if (allNamedFiles.Count == 0)
                {
                    CombinedSteps.Failure($"Could not find any files in directory {osDirectory} that start with {fileName} and have extension {fileExtension}");
                    return;
                }
                if (allNamedFiles.Count > 1)
                {
                    CombinedSteps.Failure($"Found {allNamedFiles.Count} files in directory {osDirectory} that start with {fileName} and have extension {fileExtension} when we expected to find 1");
                    return;
                }
                DebugOutput.Log($"File {allNamedFiles[0]} was found in directory {osDirectory} that starts with {fileName} and has extension {fileExtension}");
                // now we need to unzip the file
                var OSFileNameAndPath = osDirectory + allNamedFiles[0];
                DebugOutput.Log($"Full OS file name and path is {OSFileNameAndPath}");
                if (!FileUtils.OSFileCheck(OSFileNameAndPath))
                {
                    CombinedSteps.Failure($"Zip File to be unzipped {OSFileNameAndPath} does not exist");
                    return;
                }
                DebugOutput.Log($"Zip File to be unzipped {OSFileNameAndPath} exists");
                if (!FileUtils.OSUnZipFile(OSFileNameAndPath, osDirectory + fileName))
                {
                    CombinedSteps.Failure($"Zip File to be unzipped {OSFileNameAndPath} could not be unzipped");
                    return;
                }
                DebugOutput.Log($"Zip File to be unzipped {OSFileNameAndPath} was unzipped");

            }
        }
        
        
        [When(@"I Delete Zipped File ""(.*)"" By ""(.*)""")]
        public void WhenIDeleteZippedFileBy(string repoFile,string ingestionType)
        {
            string proc = $"When I Delete Zipped File {repoFile} By {ingestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                // repoFile is something like /AppSpecFlow/TestResults/Rejected/HX25-8744_BL001_corrupted.zip
                // we need just the path not the file name
                var tmpFileName = Path.GetFileName(repoFile);
                if (tmpFileName == null)
                {
                    CombinedSteps.Failure($"Could not get the file name from the file name {repoFile}");
                    return;
                }
                var repoFileDir = repoFile.Replace(tmpFileName, "");
                DebugOutput.Log($"Directory where zip file is {repoFileDir}");
                var osDirectory = FileUtils.GetRepoDirectory() + repoFileDir;
                osDirectory = osDirectory.Replace("\\", "/");
                DebugOutput.Log($"OS Directory is {osDirectory}");
                if (!FileUtils.OSDirectoryCheck(osDirectory))
                {
                    CombinedSteps.Failure($"Directory where zip file is {osDirectory} does not exist");
                    return;
                }
                // the file will have a datestamp (YYYYMMDD) an Underscore and a time stamp (HHMMSSSSS) after the file name
                var fileName = Path.GetFileNameWithoutExtension(repoFile);
                var fileExtension = Path.GetExtension(repoFile);
                DebugOutput.Log($"File name without extension is {fileName} and file extension is {fileExtension} and found in the repo at {repoFileDir}");
                // confirm the file in the directory exists within the repo as an OS File
                var allFiles = FileUtils.OSGetListOfFilesInDirectoryOfType(osDirectory, fileExtension);
                DebugOutput.Log($"All files with {fileExtension} extension in directory {osDirectory} are {allFiles.Count}");
                var allNamedFiles = new List<string>();
                foreach (var file in allFiles)
                {
                    DebugOutput.Log($"Checking file {file}");
                    if (file.StartsWith(fileName) && file.EndsWith(fileExtension))
                    {
                        DebugOutput.Log($"File {file} starts with {fileName} and ends with {fileExtension}");
                        allNamedFiles.Add(osDirectory + file);
                    }
                }
                if (allNamedFiles.Count == 0)
                {
                    CombinedSteps.Failure($"Could not find any files in directory {osDirectory} that start with {fileName} and have extension {fileExtension}");
                    return;
                }
                if (allNamedFiles.Count > 1)
                {
                    CombinedSteps.Failure($"Found {allNamedFiles.Count} files in directory {osDirectory} that start with {fileName} and have extension {fileExtension} when we expected to find 1");
                    return;
                }
                DebugOutput.Log($"File {allNamedFiles[0]} was found in directory {osDirectory} that starts with {fileName} and has extension {fileExtension}");
                if (!FileUtils.OSFileCheck(allNamedFiles[0]))
                {
                    CombinedSteps.Failure($"File to be deleted {allNamedFiles[0]} does not exist");
                    return;
                }
                DebugOutput.Log($"File to be deleted {allNamedFiles[0]} exists");
                if (!FileUtils.OSFileDeletion(allNamedFiles[0]))
                {
                    CombinedSteps.Failure($"File to be deleted {allNamedFiles[0]} could not be deleted");
                    return;
                }
                DebugOutput.Log($"File to be deleted {allNamedFiles[0]} was deleted");
            }
        }




        [Then(@"File ""(.*)"" Is Found In ""(.*)"" By ""(.*)"" As Unreadable")]
        public string? ThenFileIsFoundInByAsUnreadable(string originalZipFile, string repoPathWhereZipFileWas, string ingestionType)
        {
            string proc = $"Then File {originalZipFile} Is Found In {repoPathWhereZipFileWas} By {ingestionType} As Unreadable";
            if (CombinedSteps.OutputProc(proc))
            {
                // does the repoPathWhereZipFileWas exist
                var fullRepoPathWhereZipFileWas = FileUtils.GetRepoDirectory() + "\\" + repoPathWhereZipFileWas;
                fullRepoPathWhereZipFileWas = fullRepoPathWhereZipFileWas.Replace("\\", "/");
                DebugOutput.Log($"Full repo path where zip file was is {fullRepoPathWhereZipFileWas}");
                if (!FileUtils.OSDirectoryCheck(fullRepoPathWhereZipFileWas))
                {
                    CombinedSteps.Failure($"Directory where zip file was {fullRepoPathWhereZipFileWas} does not exist");
                    return null;
                }
                DebugOutput.Log($"Directory where zip file was {fullRepoPathWhereZipFileWas} exists!!!!!");
                // original file name was HX25-8744_BL001_corrupted.zip
                // it is changed to have a date and a time stamp and END with _Unreadable and then the extension
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalZipFile);
                var fileExtension = Path.GetExtension(originalZipFile);
                DebugOutput.Log($"File name without extension is {fileNameWithoutExtension} and file extension is {fileExtension}");
                var allFiles = FileUtils.OSGetListOfFilesInDirectoryOfType(fullRepoPathWhereZipFileWas, fileExtension);
                DebugOutput.Log($"All files with {fileExtension} extension in directory {fullRepoPathWhereZipFileWas} are {allFiles.Count}");
                foreach (var file in allFiles)
                {
                    DebugOutput.Log($"Checking file {file}");
                    if (file.StartsWith(fileNameWithoutExtension) && file.EndsWith("_Unreadable" + fileExtension))
                    {
                        DebugOutput.Log($"File {file} starts with {fileNameWithoutExtension} and ends with _Unreadable{fileExtension}");
                        DebugOutput.Log($"File {file} was found in directory {fullRepoPathWhereZipFileWas} that starts with {fileNameWithoutExtension} and ends with _Unreadable{fileExtension}");
                        return fullRepoPathWhereZipFileWas + "/" + file;
                    }
                }
                CombinedSteps.Failure($"Could not find the unzipped unreadable file in directory {fullRepoPathWhereZipFileWas} that starts with {fileNameWithoutExtension} and ends with _Unreadable{fileExtension}");

            }
            return "";
        }
        
                
        [Then(@"File ""(.*)"" Is Found In ""(.*)"" By ""(.*)"" As Error")]
        public void ThenFileIsFoundInByAsError(string originalZipFile, string repoPathWhereZipFileWas, string ingestionType)
        {
            ThenFileIsFoundInBy(originalZipFile, repoPathWhereZipFileWas, ingestionType);
        }



        [Then(@"File ""(.*)"" Is Found In ""(.*)"" By ""(.*)""")]
        public string? ThenFileIsFoundInBy(string originalZipFile, string repoPathWhereZipFileWas, string ingestionType)
        {
            string proc = $"Then File {originalZipFile} Is Found In {repoPathWhereZipFileWas} By {ingestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                // does the repoPathWhereZipFileWas exist
                var fullRepoPathWhereZipFileWas = FileUtils.GetRepoDirectory() + "\\" + repoPathWhereZipFileWas;
                fullRepoPathWhereZipFileWas = fullRepoPathWhereZipFileWas.Replace("\\", "/");
                DebugOutput.Log($"Full repo path where zip file was is {fullRepoPathWhereZipFileWas}");
                if (!FileUtils.OSDirectoryCheck(fullRepoPathWhereZipFileWas))
                {
                    CombinedSteps.Failure($"Directory where zip file was {fullRepoPathWhereZipFileWas} does not exist");
                    return null;
                }
                DebugOutput.Log($"Directory where zip file was {fullRepoPathWhereZipFileWas} exists");
                // we are supplying orginalZipFile name which was AH003_BL001_Unreadable.xlsx
                // But it now has a datestamp (YYYYMMDD) an Underscore and a time stamp (HHMMSSSSS) between AH003_BL001_ and _Unreadable.xlsx
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalZipFile);
                var fileExtension = Path.GetExtension(originalZipFile);
                DebugOutput.Log($"File name without extension is {fileNameWithoutExtension} and file extension is {fileExtension}");
                // get a list of all files in that direcotry that start with the file name with the file name without the extension
                var allFiles = FileUtils.OSGetListOfFilesInDirectoryOfType(fullRepoPathWhereZipFileWas, fileExtension);
                DebugOutput.Log($"All files with {fileExtension} extension in directory {fullRepoPathWhereZipFileWas} are {allFiles.Count}");
                // need a list of known reserved words
                var reservedWords = new List<string> { "Unreadable" };
                foreach (var reservedWord in reservedWords)
                {
                    DebugOutput.Log($"Checking for reserved word {reservedWord} in file name {fileNameWithoutExtension}");
                    if (fileNameWithoutExtension.ToLower().Contains(reservedWord.ToLower()))
                    {
                        // we need to remove the reserved word from the file name
                        fileNameWithoutExtension = fileNameWithoutExtension.Replace("_" + reservedWord, "");
                        DebugOutput.Log($"File name without extension after removing reserved word {reservedWord} is {fileNameWithoutExtension}");
                    }
                }
                DebugOutput.Log($"Looking for {fileNameWithoutExtension} in the file list");
                // now loop through all files that start with fileNameWithoutExtension
                var listOfFileStartingRight = new List<string>();
                foreach (var file in allFiles)
                {
                    DebugOutput.Log($"Checking file {file}");
                    if (file.StartsWith(fileNameWithoutExtension))
                    {
                        listOfFileStartingRight.Add(file);
                        DebugOutput.Log($"File {file} starts with {fileNameWithoutExtension}");
                    }
                }
                if (listOfFileStartingRight.Count == 0)
                {
                    CombinedSteps.Failure($"Could not find any files in directory {fullRepoPathWhereZipFileWas} that start with {fileNameWithoutExtension} and have extension {fileExtension}");
                    return null;
                }
                DebugOutput.Log($"Matched files are {listOfFileStartingRight.Count}");

                DebugOutput.Log($"File {listOfFileStartingRight[0]} was found in directory {fullRepoPathWhereZipFileWas} that starts with {fileNameWithoutExtension} and has extension {fileExtension}");
                // get the all the files in the directory that start with the fileNameWithoutExtension allFiles
                var unZippedFileFullPath = "";
                foreach (var file in allFiles)
                {
                    DebugOutput.Log($"All files in directory {fullRepoPathWhereZipFileWas} are {file}");
                    // file contains the path - just want the file name
                    var justFileName = Path.GetFileName(file);
                    DebugOutput.Log($"Just file name is {justFileName}");
                    // if the file starts with fileNameWithoutExtension and ends with fileExtension then we have found the file
                    if (justFileName.StartsWith(fileNameWithoutExtension) && justFileName.EndsWith(fileExtension))
                    {
                        DebugOutput.Log($"File {justFileName} starts with {fileNameWithoutExtension} and ends with {fileExtension}");
                        DebugOutput.Log($"File {justFileName} was found in directory {fullRepoPathWhereZipFileWas}");
                        unZippedFileFullPath = fullRepoPathWhereZipFileWas + "/" + file;
                        break;
                    }
                }
                if (unZippedFileFullPath == "")
                {
                    CombinedSteps.Failure($"Could not find the unzipped file in directory {fullRepoPathWhereZipFileWas} that starts with {fileNameWithoutExtension} and has extension {fileExtension}");
                    return null;
                }
                DebugOutput.Log($"Unzipped file full path is {unZippedFileFullPath}");
                return unZippedFileFullPath;
            }
            return null;
        }


        [Then(@"Excel File ""(.*)"" Found In ""(.*)"" By ""(.*)"" Has Sheet ""(.*)""")]
        public void ThenExcelFileFoundInByHasSheet(string excelFileName, string zippedFolderPath, string ingestionType, string sheetName)
        {
            string proc = $"Then Excel File {excelFileName} Found In {zippedFolderPath} By {ingestionType} Has Sheet {sheetName}";
            if (CombinedSteps.OutputProc(proc))
            {
                var OSFileName = ThenFileIsFoundInBy(excelFileName, zippedFolderPath, ingestionType);
                if (OSFileName == null)
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} could not be found in {zippedFolderPath} by {ingestionType}");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} was found in {zippedFolderPath} by {ingestionType} at location {OSFileName}");
                if (!FileUtils.OSFileCheck(OSFileName))
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} was found in {zippedFolderPath} by {ingestionType} at location {OSFileName} but the file does not exist");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} was found in {zippedFolderPath} by {ingestionType} at location {OSFileName} and the file exists");

                // where is the sheet check
                var response = CmdUtil.ExecuteDotnet("./CommunicationExcel/CommunicationExcel.csproj", $"\"getsheets\" \"{OSFileName}\"").Trim();
                DebugOutput.Log($"Response was {response}");
                // our response is multi line - I'd like that as a list please
                var listOfSheets = response.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                DebugOutput.Log($"List of sheets are {string.Join(", ", listOfSheets)}");
                foreach (var sheet in listOfSheets)
                {
                    DebugOutput.Log($"Checking sheet {sheetName.Trim().ToLower()} against {sheet.Trim().ToLower()}");
                    if (sheet.Trim().ToLower().Contains(sheetName.Trim().ToLower()))
                    {
                        DebugOutput.Log($"Excel File {excelFileName} found in {zippedFolderPath} by {ingestionType} contains the sheet {sheetName}");
                        return;
                    }
                }
                CombinedSteps.Failure($"Excel File {excelFileName} found in {zippedFolderPath} by {ingestionType} DONES NOT CONTAIN the sheet {sheetName}");

            }
        }

                
        [Then(@"Excel File ""(.*)"" Sheet ""(.*)"" Cell ""(.*)"" Equal To ""(.*)"" Found In ""(.*)"" By ""(.*)""")]
        public void ThenExcelFileSheetCellEqualToFoundInBy(string excelFileName,string sheetName,string cellLocation,string expectedValue,string folderPath,string ingestionType)
        {
            if (expectedValue == "")
            {
                expectedValue = string.Empty;
            }
            string proc = $"Then Excel File {excelFileName} Sheet {sheetName} Cell {cellLocation} Equal To {expectedValue} Found In {folderPath} By {ingestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                var OSFileName = ThenFileIsFoundInBy(excelFileName, folderPath, ingestionType);
                if (OSFileName == null)
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} could not be found in {folderPath} by {ingestionType}");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} was found in {folderPath} by {ingestionType} at location {OSFileName}");
                if (!FileUtils.OSFileCheck(OSFileName))
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} was found in {folderPath} by {ingestionType} at location {OSFileName} but the file does not exist");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} was found in {folderPath} by {ingestionType} at location {OSFileName} and the file exists");
                // where is the sheet check
                var response = CmdUtil.ExecuteDotnet("./CommunicationExcel/CommunicationExcel.csproj", $"\"readcell\" \"{OSFileName}\" \"{sheetName}\" \"{cellLocation}\" ").Trim();
                DebugOutput.Log($"Response was {response}");
                // if the response starts with error then fail
                if (response.ToLower().StartsWith("error"))
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} found in {folderPath} by {ingestionType} could not read cell {cellLocation} in sheet {sheetName} because {response}");
                    return;
                }
                // check the respons is exactly like the expected value
                if (response != expectedValue)
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} found in {folderPath} by {ingestionType} has cell {cellLocation} in sheet {sheetName} with value {response} when we expected it to be {expectedValue}");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} found in {folderPath} by {ingestionType} has cell {cellLocation} in sheet {sheetName} with the expected value {expectedValue}");
                
            }
        }


        
                
        [Then(@"Excel File ""(.*)"" Found In ""(.*)"" By ""(.*)"" Has (.*) Sheets")]
        public void ThenExcelFileFoundINByHasSheets(string excelFileName, string zippedFolderPath, string ingestionType,int expectedSheetCount)
        {
            string proc = $"Then Excel File {excelFileName} Found IN {zippedFolderPath} By {ingestionType} Has {expectedSheetCount} Sheets";
            if (CombinedSteps.OutputProc(proc))
            {
                var OSFileName = ThenFileIsFoundInBy(excelFileName, zippedFolderPath, ingestionType);
                if (OSFileName == null)
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} could not be found in {zippedFolderPath} by {ingestionType}");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} was found in {zippedFolderPath} by {ingestionType} at location {OSFileName}");
                if (!FileUtils.OSFileCheck(OSFileName))
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} was found in {zippedFolderPath} by {ingestionType} at location {OSFileName} but the file does not exist");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} was found in {zippedFolderPath} by {ingestionType} at location {OSFileName} and the file exists");
                // where is the sheet check
                var response = CmdUtil.ExecuteDotnet("./CommunicationExcel/CommunicationExcel.csproj", $"\"getsheets\" \"{OSFileName}\"").Trim();
                DebugOutput.Log($"Response was {response}");
                // our response is multi line - I'd like that as a list please
                var listOfSheets = response.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                DebugOutput.Log($"List of sheets are {string.Join(", ", listOfSheets)} AND there is {listOfSheets.Count} number of sheets");
                var realSheetCount = listOfSheets.Count - 2; // we have a json return with square brakets so we need to minus 2
                if (realSheetCount != expectedSheetCount)
                {
                    CombinedSteps.Failure($"Excel File {excelFileName} found in {zippedFolderPath} by {ingestionType} has {realSheetCount} sheets when we expected it to have {expectedSheetCount} sheets");
                    return;
                }
                DebugOutput.Log($"Excel File {excelFileName} found in {zippedFolderPath} by {ingestionType} contains the expected number of sheets {expectedSheetCount}");
            }
        }







        [Then(@"Input File Directory Structure Is Created For ""(.*)""")]
        public void ThenInputFileDirectoryStructureIsCreatedFor(string IngestionType)
        {
            string proc = $"Then Input File Directory Structure Is Created For {IngestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                string ADFDirectory = "";
                switch (IngestionType.ToLower())
                {
                    case "bl001":
                        ADFDirectory = $"{ADFBL001Directory}/";
                        break;
                    case "bl002":
                        ADFDirectory = $"{ADFBL002Directory}/";
                        break;
                    case "si003":
                        ADFDirectory = $"{ADFSI003Directory}/";
                        break;
                    case "mi003":
                        ADFDirectory = $"{ADFMI003Directory}/";
                        break;
                    default:
                        ADFDirectory = "";
                        break;
                }

                if (ADFDirectory == "")
                {
                    CombinedSteps.Failure($"Ingestion type {IngestionType} is not recognised. Supported types are SI003, MI003, BL002");
                    return;
                }

                var fullDirectoryStructure = $"Input/{ADFDirectory}/";
                DebugOutput.Log($"Full directory structure is {fullDirectoryStructure}");
                if (!ADFUtils.DoesADFDirectoryExist(fullDirectoryStructure))
                {
                    CombinedSteps.Failure($"Could not create ADF directory structure {fullDirectoryStructure}");
                    return;
                }
                DebugOutput.Log($"ADF directory structure {fullDirectoryStructure} was created");
            }
        }


        [When(@"I API Trigger ""(.*)""")]
        public void WhenIAPITrigger(string IngestionType)
        {
            string proc = $"When I API Trigger {IngestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                var interfaceId = GetIngestionTypeDirectory(IngestionType);
                if (interfaceId == "")
                {
                    CombinedSteps.Failure($"Ingestion type {IngestionType} is not recognised. Supported types are SI003, MI003, BL002");
                    return;
                }
                var result = APIUtil.PostTrigger(IngestionType).GetAwaiter().GetResult();
                if (!result)
                {
                    CombinedSteps.Failure($"API Trigger for ingestion type {IngestionType} failed");
                    return;
                }                
            }
        }


        [Then(@"File ""(.*)"" Contains The Error ""(.*)""")]
        public void ThenFileContainsTheError(string fileInRepoPath, string errorText)
        {
            string proc = $"Then File {fileInRepoPath} Contains The Error {errorText}";
            var errorWarningText = "---- ERROR REPORT ----";
            if (CombinedSteps.OutputProc(proc))
            {
                var fullFileNameAndPath = FileUtils.GetRepoDirectory() + fileInRepoPath;
                fullFileNameAndPath = fullFileNameAndPath.Replace("\\", "/");
                DebugOutput.Log($"Full file name and path to be checked is {fullFileNameAndPath}");

                // Get a list of all files in that direcotry that start with the file name with the file name without the extension
                var directory = Path.GetDirectoryName(fullFileNameAndPath);
                if (directory == null)
                {
                    CombinedSteps.Failure($"Could not get the directory from the file name {fullFileNameAndPath}");
                    return;
                }
                var fileName = Path.GetFileNameWithoutExtension(fullFileNameAndPath);
                var fileExtension = Path.GetExtension(fullFileNameAndPath);
                var allFiles = FileUtils.OSGetListOfFilesInDirectoryOfType(directory, fileExtension);
                DebugOutput.Log($"All files with {fileExtension} extension in directory {directory} are {allFiles.Count}");
                var matchedFiles = allFiles.Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(fileName)).ToList();
                if (matchedFiles.Count == 0)
                {
                    CombinedSteps.Failure($"Could not find any files in directory {directory} that start with {fileName} and have extension {fileExtension}");
                    return;
                }
                DebugOutput.Log($"Matched files are {matchedFiles.Count}");
                var stringAfterError = FileUtils.OSGetFileContentsAsListOfStringByLineAfterAGivenLine(directory + "/" + matchedFiles[0], errorWarningText);
                if (stringAfterError == null)
                {
                    CombinedSteps.Failure($"Could not read the file {matchedFiles[0]} OR Empty after line {errorWarningText}");
                    return;
                }
                DebugOutput.Log($"File contents after line {errorWarningText} are '{stringAfterError}' ");
                DebugOutput.Log($"Checking for error text {errorText} in file {matchedFiles[0]} after line {errorWarningText}");
                if (!stringAfterError.Contains(errorText))
                {
                    CombinedSteps.Failure($"File {matchedFiles[0]} does not contain the error text {errorText}");
                    return;
                }
                DebugOutput.Log($"File {matchedFiles[0]} contains the error text {errorText}");
            }
        }

                
        [Given(@"ADF ""(.*)"" Has Been Cleared")]
        public void GivenADFHasBeenCleared(string ingestionType)
        {
            string proc = $"Given ADF {ingestionType} Has Been Cleared";
            if (CombinedSteps.OutputProc(proc))
            {
                string ADFDirectory = "";
                switch (ingestionType.ToLower())
                {
                    case "bl001":
                        ADFDirectory = $"{ADFBL001Directory}/";
                        break;
                    case "bl002":
                        ADFDirectory = $"{ADFBL002Directory}/";
                        break;
                    case "si003":
                        ADFDirectory = $"{ADFSI003Directory}/";
                        break;
                    case "mi003":
                        ADFDirectory = $"{ADFMI003Directory}/";
                        break;
                    default:
                        ADFDirectory = "";
                        break;
                }

                if (ADFDirectory == "")
                {
                    CombinedSteps.Failure($"Ingestion type {ingestionType} is not recognised. Supported types are SI003, MI003, BL002");
                    return;
                }
                DebugOutput.Log($"Clearing ADF for ingestion type {ingestionType} in directory {ADFDirectory}");
                // get the dates year
                var year = DateTime.Now.Year.ToString();
                var incompleteDirectory =   $"Incomplete/{ADFDirectory}{year}";
                var inputDirectory =        $"Input/{ADFDirectory}{year}";
                var processedDirectory =    $"Processed/{ADFDirectory}{year}";
                var rejectedDirectory =     $"Rejected/{ADFDirectory}{year}";
                var uploadedDirectory = $"Uploaded/{ADFDirectory}{year}";
                var directories = new List<string> { incompleteDirectory, inputDirectory, processedDirectory, rejectedDirectory, uploadedDirectory };
                foreach (var directory in directories)
                {
                    DebugOutput.Log($"Clearing ADF directory {directory}");
                    if (!ADFUtils.DeleteDirectoryAndContentsInADF(directory))
                    {
                        CombinedSteps.Failure($"Could not clear ADF directory {directory}");
                        return;
                    }
                    DebugOutput.Log($" {directory} ADF directory was cleared");
                }


            }
        }






        [Given(@"Input File ""(.*)"" For ""(.*)""")]
        public void GivenInputFileFor(string fileToBeUploaded, string IngestionType)
        {
            string proc = $"Given Input File {fileToBeUploaded} For {IngestionType}";
            if (CombinedSteps.OutputProc(proc))
            {
                string ADFDirectory = "";
                switch(IngestionType.ToLower())
                {
                    case "bl001":
                        ADFDirectory = $"Input/{ADFBL001Directory}/";
                        break;
                    case "bl002":
                        ADFDirectory = $"Input/{ADFBL002Directory}/";
                        break;
                    case "si003":
                        ADFDirectory = $"Input/{ADFSI003Directory}/";
                        break;
                    case "mi003":
                        ADFDirectory = $"Input/{ADFMI003Directory}/";
                        break;
                    case "mi007":
                        ADFDirectory = $"Input/{ADFMI007Directory}/";
                        break;
                    default:
                        ADFDirectory = "";
                        break;
                }   

                if (ADFDirectory == "")
                {
                    CombinedSteps.Failure($"Ingestion type {IngestionType} is not recognised. Supported types are SI003, MI003, BL002");
                    return;
                }

                if (!InputFileForingestion(fileToBeUploaded, ADFDirectory))
                {
                    CombinedSteps.Failure($"File to be uploaded {fileToBeUploaded} could not be uploaded to ADF");
                    return;
                }
            }
        }



    }
}