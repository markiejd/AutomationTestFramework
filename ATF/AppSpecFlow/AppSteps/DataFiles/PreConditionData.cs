using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSpecFlow.AppSteps.DataFiles
{
    public static class PreConditionData
    {

        public static string PreConditionDataDirectory = @"\AppSpecFlow\TestPreConditionData";
        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string dataDir = "ExhibitsSearchSceneData";
        public static string fulldatDir = appData + @"\" + dataDir;

        private static string OperationalData = fulldatDir + @"\" + "OperationData";
        private static string[] ExhibitsSearchSceneDataSubDirectories = { "AppLogs", "AuditLogs", "Documents", "OperationData", "ReferenceData" };


        public static bool PreConditionDataExists(string preConName)
        {
            DebugOutput.OutputMethod($"Proc - PreConditionDataExists {preConName}");
            // DebugOutput.Log($"Proc - PreConditionDataExists {preConName}");
            var directory = PreConditionDataDirectory + @"\" + preConName;
            return FileUtils.DirectoryCheck(directory);
        }

        public static bool CreatePreConditionData(string preConName, int preConNumber)
        {
            DebugOutput.OutputMethod($"Proc - CreatePreConditionData {preConName}");
            // DebugOutput.Log($"Proc - CreatePreConditionData {preConName}");
            //Create the target directory in project
            //make the changes to operation in \ExhibitsSearchSceneData\OperationData
            List<string> files = FileUtils.OSGetListOfJsonFilesInDirectory(OperationalData);
            if (files.Count > 0)
            {
                DebugOutput.Log($"We have {files.Count} files in {OperationalData}");
                if (!CreatePreConditionDirectories(preConName)) return false;
                foreach (var fileName in files)
                {
                    var fullFileName = OperationalData + @"\" + fileName;
                    DebugOutput.Log($"Replacement service on {fullFileName}");
                    var allText = FileUtils.OSGetAllTextInFile(fullFileName);
                    allText = ChangeSSADataToPreConditionData(allText, preConNumber);
                    var projectDir = PreConditionDataDirectory + @"\" + preConName + @"\" + "OperationData" + @"\" + fileName;
                    DebugOutput.Log($"WILL BE WRITING ALLTEXT TO {projectDir}");
                    if (!FileUtils.FilePopulate(projectDir, allText)) return false;
                }
            }
            return true;
            //make a copy of \ExhibitsSearchSceneData to \preConName

            //copy \preconname to \Project\AppSpecFlow\TestPreConditionData
        }

        private static bool CreatePreConditionDirectories(string preConName)
        {
            DebugOutput.OutputMethod($"Proc - CreatePreConditionDirectories {preConName}");
            FileUtils.DirectoryCreation(PreConditionDataDirectory + @"\" + preConName);
            try
            {
                foreach (var directoryName in ExhibitsSearchSceneDataSubDirectories)
                {
                    FileUtils.DirectoryCreation(PreConditionDataDirectory + @"\" + preConName + @"\" + directoryName);
                }
                DebugOutput.Log($"ALL SUB DIRECTORIES CREATED");
                return true;
            }
            catch
            {
                DebugOutput.Log($"FAILURE CREATING DIRECTORIES!");
                return false;
            }
        }

        private static string ChangeText(string allText, string index, int from, int indexHowMany, int howMany, string replaceWith)
        {
            DebugOutput.OutputMethod($"Proc - ChangeText {allText} {from} {indexHowMany} {howMany} {replaceWith} ");
            // DebugOutput.Log($"Proc - ChangeText {index} {from} {indexHowMany} {howMany} {replaceWith} ");
            var indexPoint = allText.IndexOf(index);
            if (indexPoint > -1)
            {
                var internalId = allText.Substring(indexPoint + from, indexHowMany);
                var subStringInternalId = internalId.Substring(internalId.Length - howMany);
                DebugOutput.Log($"INTERNAL ID TEXT = {internalId}");
                var replacementInternalID = subStringInternalId + replaceWith;
                DebugOutput.Log($"NEW INTERNAL ID TEXT = {replacementInternalID}");
                allText = allText.Replace(internalId, replacementInternalID);
            }
            return allText;
        }

        private static string ChangeText(string allText, string from, string to)
        {
            DebugOutput.OutputMethod($"Proc - ChangeText {allText} {from} {to} ");
            // DebugOutput.Log($"Proc - ChangeText {from} {to}");
            try
            {
                allText = allText.Replace(from, to);
                return allText; 
            }
            catch
            {
                DebugOutput.Log($"Issue with change Text");
                return "";
            }
        }

        private static string ChangeText(string allText, string index, int start, int howMany, string replaceWith)
        {
            DebugOutput.OutputMethod($"Proc - ChangeText {index} {start} {howMany} {replaceWith}");
            // DebugOutput.Log($"Proc - ChangeText {index} {start} {howMany} {replaceWith}");
            var indexOfSeizedDate = allText.IndexOf(index);
            if (indexOfSeizedDate > -1)
            {
                var seizedDate = allText.Substring(indexOfSeizedDate + start, howMany);
                allText = allText.Replace(seizedDate, replaceWith);
            }
            return allText;
        }

        private static string ChangeSSADataToPreConditionData(string allText, int preConNumber)
        {
            DebugOutput.OutputMethod($"Proc - ChangeSSADataToPreConditionData ");
            // DebugOutput.Log($"Proc - ChangeSSADataToPreConditionData ");
            if (allText.Length > 0)
            {
                var preConNumberAsString = preConNumber.ToString();

                allText = ChangeText(allText, "internalId", 13, 19, 10, "EPOCH");
                allText = ChangeText(allText, preConNumberAsString, "<EPOCH>");
                allText = ChangeText(allText, "seizedDate", 13, 10, "<DATEREVERSE>");
                allText = ChangeText(allText, "actionDate", 13, 10, "<DATEACTION>");
                allText = ChangeText(allText, "warrantDate", 14, 10, "<DATEWARRANT>");
                return allText;
            }
            return "";
        }

        public static bool UsePreConditionData(string preConName)
        {
            DebugOutput.OutputMethod($"Proc - UsePreConditionData {preConName} ");
            // DebugOutput.Log($"Proc - UsePreConditionData {preConName}");
            if (!PreConditionDataExists(preConName)) return false;
            var fullpreConName = PreConditionDataDirectory + @"\" + preConName;
            var fullappData = appData + @"\" + preConName;
            //Copy to appData
            if (!FileUtils.CopyDirectoryFromProjectToOS(fullpreConName, fullappData)) return false;
            //Confirm right place
            if (!FileUtils.OSDirectoryCheck(fullappData)) return false;
            //Rename to ExhibitsSearchSceneData
            if (!FileUtils.OSRenameDirectory(fullappData, appData + @"\" + "ExhibitsSearchSceneData")) return false;
            //THEN DO MORE INSIDE
            //need to rename file... 
            //Operations Changes
            if (!EPOCHAllFileNamesInDir(OperationalData)) return false;
            //NEED TO CHANGE INSIDE OF FILE
            if (!TextReplacementAllFilesInDir(OperationalData)) return false;
            return true;
        }

        private static bool TextReplacementAllFilesInDir(string directory)
        {
            DebugOutput.OutputMethod($"Proc - TextReplacementAllFilesInDir {directory} ");
            // DebugOutput.Log($"TextReplacementAllFilesInDir {directory}");
            List<string> files = FileUtils.OSGetListOfJsonFilesInDirectory(OperationalData);
            if (files.Count > 0)
            {
                DebugOutput.Log($"We have {files.Count} files in {OperationalData}");
                foreach (var fileName in files)
                {
                    var fullFileName = directory + @"\" + fileName;
                    DebugOutput.Log($"Replacement service on {fullFileName}");
                    if (!FileUtils.OSReplaceTextInFile(fullFileName)) return false;
                }
            }
            return true;
        }

        private static bool EPOCHAllFileNamesInDir(string directory)
        {
            DebugOutput.OutputMethod($"Proc - EPOCHAllFileNamesInDir {directory} ");
            DebugOutput.Log($"EPOCHAllFileNamesInDir {directory}");
            List<string> files = FileUtils.OSGetListOfJsonFilesInDirectory(OperationalData);
            if (files.Count > 0)
            {
                DebugOutput.Log($"We have {files.Count} files in {OperationalData}");
                foreach (var fileName in files)
                {
                    var newFileName = StringValues.TextReplacementService(fileName);
                    if (newFileName != fileName)
                    {
                        var fullNewFileName = OperationalData + @"\" + newFileName;
                        var fullOldFileName = OperationalData + @"\" + fileName;
                        if (!FileUtils.OSRenameFile(fullOldFileName, fullNewFileName)) return false;
                    }
                }
            }
            return true;
        }

    }
}
