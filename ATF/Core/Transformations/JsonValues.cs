using AppXAPI.Models;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transformations
{
    public static class JsonValues
    {
        private static string jSonOutFiles = "/AppXAPI/APIOutFiles/";
        private static string jSonAPILinksFileLocation = "/AppTargets/Resources/Variables/";
        private static string payloadLocation = @"./AppXAPI/Models/Payloads/";

        /// <summary>
        /// Pass in Json as a String, and output into a file.  If you supply apiName will create the file with that name, if not its called default.
        /// </summary>
        /// <returns>true if the file is created, false, if it failed</returns>
        public static bool CreateAPIJsonFile(string jSonText, string fileNameAndLocation, string apiName = "default")
        {
            DebugOutput.Log($"Proc - ReadAPIJsonFile {fileNameAndLocation} {apiName}");
            DebugOutput.Log($"Json - {jSonText}");

            var fileName = $"{apiName}.json";
            var fullFileName = Path.Combine(fileNameAndLocation, fileName);

            if (FileUtils.FileCheck(fullFileName))
            {
                FileUtils.FileDeletion(fullFileName);
            }

            if (FileUtils.FilePopulate(fullFileName, jSonText)) return true;

            DebugOutput.Log($"Failed to write to file {fullFileName}");
            return false;
        }

        /// <summary>
        /// Read a file, which we hope is in Json format, and return as one big string.
        /// </summary>
        /// <returns>String of text, or null if failed to read</returns>
        public static string? ReadOSJsonFile(string fullFileName)
        {
            DebugOutput.Log($"Proc - ReadOSJsonFile {fullFileName}");
            return ReadOSJsonFile(fullFileName, true);
        }
        
        public static string? ReadPayloadFile(string fileName, string extenstion = "")
        {
            DebugOutput.Log($"Proc - ReadPayloadFile {fileName} {extenstion}");
            if (extenstion != "")
            {
                extenstion = "." + extenstion;
            }
            var file = payloadLocation + fileName + extenstion;
            DebugOutput.Log($"Proc - ReadOSJsonFile {file}");
            return ReadOSJsonFile(file, false);
        }

        public static string? ReadAPIListJsonFile()
        {
            DebugOutput.Log($"Proc - ReadAPIListJsonFile");
            var environment = Environment.GetEnvironmentVariable("ATFENVIRONMENT");
            DebugOutput.Log($"environment is equal to {environment}");
            if (environment == null) environment = "development";
            string fileName = "var-" + environment + ".json";
            var file = jSonAPILinksFileLocation + fileName;
            DebugOutput.Log($"Proc - ReadAPIListJsonFile {file}");
            return ReadOSJsonFile(file, false);
        }

        
        /// <summary>
        /// Read the File Name, and return the contents as a string.
        /// </summary>
        /// <returns>string of the json file, null if it failed</returns>
        public static string? ReadDatabaseResourceFile(string fileName)
        {
            DebugOutput.Log($"Proc - ReadDatabaseResourceFile {fileName}");
            var file = @"/AppTargets/Resources/Databases/" + fileName;
            return ReadOSJsonFile(file, false);
        }

        /// <summary>
        /// Read a json file called apiName
        /// </summary>
        /// <returns>A big string or null if fail</returns>
        public static string? ReadAPIJsonFile(string apiName)
        {
            DebugOutput.Log($"Proc - ReadAPIJsonFile {apiName}");
            var fileName = $"{apiName}.json";
            var directory = jSonOutFiles;
            var fullFileName = "." + directory + fileName;
            if (!FileUtils.FileCheck(fullFileName)) return "";
            DebugOutput.Log($"API Json File {fullFileName} Exists");
            return ReadOSJsonFile(fullFileName, false);
        }

        public static string ReturnIndentedJson(string json)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
        }


        /// <summary>
        /// Read the File Name (will be different on every pc), and return the contents as a string.
        /// If OS is true needs full directory structure, if false (default) assumes it part of this repo.
        /// </summary>
        /// <returns>string of the json file, null if it failed</returns>
        private static string? ReadOSJsonFile(string fullFileName, bool OS = false)
        {
            DebugOutput.Log($"Proc - ReadJsonFile {fullFileName} {OS}");
            using var r = FileUtils.GetStream(fullFileName, OS);
            try
            {
                string json = r.ReadToEnd();
                return json;
            }
            catch
            {
                DebugOutput.Log($"issue with reading");
                return "";
            }
        }

    }


}
