


using Core.Logging;

namespace Core
{
    public class ADFUtils
    {
        public static string hello()
        {
            return "Hello from ADFUtils";
        }
        
        public static bool DoesADFDirectoryExist(string directoryInADF)
        {
            DebugOutput.OutputMethod($"DoesADFDirectoryExist", $"{directoryInADF}");
            // var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationADF/CommunicationADF.csproj", $"exist \"{directoryInADF}\"");
            DebugOutput.Log($"{returnedString}");
            if (returnedString.ToLower().Contains("error")
                || returnedString.ToLower().Contains("Exception"))
            {
                DebugOutput.WarningMessage($"DoesADFDirectoryExist failed with {returnedString}");
                return false;
            }
            if (returnedString.ToLower().Contains("true"))
            {
                return true;
            }
            return false;            
        }

        public static List<string> GetAllFileNamesInADFDirectory(string directoryInADF)
        {
            DebugOutput.OutputMethod($"GetAllFileNamesInADFDirectory", $"{directoryInADF}");
            // var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationADF/CommunicationADF.csproj", $"list \"{directoryInADF}\"");
            var listOfFiles = new List<string>();
            DebugOutput.Log($"list of files: {returnedString}");
            if (returnedString.ToLower().StartsWith("error")
                || returnedString.Contains("Exception"))
            {
                DebugOutput.WarningMessage($"GetAllFileNamesInADFDirectory failed with {returnedString}");
                return listOfFiles;
            }
            // I need to break up returnedString into lines and return as a list of strings
            DebugOutput.Log($"Splitting returned string into lines");
            var x = returnedString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            DebugOutput.Log($"Found {x.Length} files in ADF directory {directoryInADF}");
            // convert array to list
            foreach (var file in x)
            {
                listOfFiles.Add(file);
            }
            return listOfFiles;
        }
        
        public static bool DeleteDirectoryAndContentsInADF(string directoryInADF)
        {
            DebugOutput.OutputMethod($"DeleteDirectoryAndContentsInADF", $"{directoryInADF}");
            // var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationADF/CommunicationADF.csproj", $"deletedirectory \"{directoryInADF}\"");
            DebugOutput.Log($"DeleteDirectoryAndContentsInADF returned {returnedString}");
            if (returnedString.ToLower().Contains("error")
                || returnedString.ToLower().Contains("exception"))
            {
                DebugOutput.WarningMessage($"DeleteDirectoryAndContentsInADF failed with {returnedString}");
                return false;
            }
            return true;
        }

        public static bool DownloadAFileFromADF(string ADFfullFileNameAndPathToBeDownloaded, string LocalfullFileNameAndPathToBeDownloadedTo)
        {
            DebugOutput.OutputMethod($"DownloadAFileFromADF", $"{ADFfullFileNameAndPathToBeDownloaded} {LocalfullFileNameAndPathToBeDownloadedTo}");
            // var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationADF/CommunicationADF.csproj", $"download \"{ADFfullFileNameAndPathToBeDownloaded}\" \"{LocalfullFileNameAndPathToBeDownloadedTo}\"");
            DebugOutput.Log($"DownloadAFileFromADF returned {returnedString}");
            if (returnedString.Contains("error")
                || returnedString.Contains("Exception"))
            {
                DebugOutput.WarningMessage($"DownloadAFileFromADF failed with {returnedString}");
                return false;
            }
            return true;
        }

        public static bool DeleteAFileInADF(string fullFileNameAndPathInADF)
        {
            DebugOutput.OutputMethod($"DeleteAFileInADF", $"{fullFileNameAndPathInADF}");
            // var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationADF/CommunicationADF.csproj", $"delete \"{fullFileNameAndPathInADF}\"");
            DebugOutput.Log($"DeleteAFileInADF returned {returnedString}");
            if (returnedString.Contains("error")
                || returnedString.Contains("Exception"))
            {
                DebugOutput.WarningMessage($"DeleteAFileInADF failed with {returnedString}");
                return false;
            }
            return true;
        }   

        public static bool UploadAFileToADF(string fullFileNameAndPathToBeUploaded, string fullFileNameAndPathInADF)
        {
            DebugOutput.OutputMethod($"UploadAFileToADF", "${fullFileNameAndPathToBeUploaded} {fullFileNameAndPathInADF}");
            // var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationADF/CommunicationADF.csproj", $"upload \"{fullFileNameAndPathToBeUploaded}\" \"{fullFileNameAndPathInADF}\"");
            DebugOutput.Log($"UploadAFileToADF returned {returnedString}");
            if (returnedString.Contains("error")
                || returnedString.Contains("Exception"))
            {
                DebugOutput.WarningMessage($"UploadAFileToADF failed with {returnedString}");
                return false;
            }
            return true;
        }
    }
}