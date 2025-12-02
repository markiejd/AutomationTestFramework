using AppXAPI.Models;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Transformations
{
    public static class XMLValues
    {
        private static string XMLOutFiles = "\\AppXAPI\\APIOutFiles\\";  
        

        /// <summary>
        /// Take a class and get it as a string in XML Format
        /// </summary>
        /// <returns>string in XML format, null, if it failed</returns>
        public static string? DeSerializeObject<T>(T dataObject)
        {
            if (dataObject == null)
            {
                return null;
            }
            try
            {
                using (StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);
                    return stringWriter.ToString();
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to Searlize XML ");
                return null;
            }
        }

        /// <summary>
        /// Pass in string in XML format, and write it to a XML file
        /// </summary>
        /// <returns>true if successfully created file, false, if it failed</returns>
        public static bool CreateXMLFile(string XMLText, string apiName = "default")
        {
            DebugOutput.Log($"Proc - ReadAPIJsonFile {apiName}");
            var fileName = $"{apiName}.xml";
            var directory = XMLOutFiles;
            var fullFileName = directory + fileName;
            if (FileUtils.FileCheck(fullFileName))
            {
                FileUtils.FileDeletion(fullFileName);
            }
            if (FileUtils.FilePopulate(fullFileName, XMLText)) return true;
            DebugOutput.Log($"Failed to write to file {fullFileName}");
            return false;
        }

        /// <summary>
        /// Read an xml file with the name passed in, return as a string.
        /// </summary>
        /// <returns>XML in string, null, if it failed</returns>
        public static string? ReadAPIXMLFile(string apiName)
        {
            DebugOutput.Log($"Proc - ReadAPIJsonFile {apiName}");
            var fileName = $"{apiName}.xml";
            var directory = XMLOutFiles;
            var fullFileName = directory + fileName;
            if (!FileUtils.FileCheck(fullFileName)) return "";
            DebugOutput.Log($"File {fullFileName} Exists");
            return ReadXMLFile(fullFileName);
        }

        /// <summary>
        /// Read an xml file with the full file location passed in, return as a string.
        /// </summary>
        /// <returns>XML in string, null, if it failed</returns>
        public static string? ReadXMLFile(string fullFileName, bool OS = false)
        {
            DebugOutput.Log($"Proc - ReadJsonFile {fullFileName} {OS}");
            using var r = FileUtils.GetStream(fullFileName, OS);
            try
            {
                string xml = r.ReadToEnd();
                return xml;
            }
            catch
            {
                DebugOutput.Log($"issue with reading");
                return "";
            }
        }




    }
}