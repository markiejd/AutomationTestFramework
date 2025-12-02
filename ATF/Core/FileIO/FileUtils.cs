using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using System.IO.Compression;
using System.Text;

namespace Core.FileIO
{
    public class FileUtils
    {
        public static string currentDirectory = Directory.GetCurrentDirectory().Replace("\\", "/");
        private static string imageTextFile = "/AppSpecFlow/TestResults/EPOCHImageWarnings.txt";

        
        /// <summary>
        /// Sets the current working directory to the top level of the build
        /// When running in debug the working directory can be different, this makes sure that it is always working in correct top directory
        /// </summary>
        /// <returns>returns true if able to navigate to top directory</returns>
        public static bool SetCurrentDirectoryToTop()
        {
            DebugOutput.OutputMethod($"FileUtls - SetCurrentDirectoryToTop");
            if (currentDirectory.Contains("AppSpecFlow") || 
                currentDirectory.Contains("AppTargets") || 
                currentDirectory.Contains("Core") ||
                    currentDirectory.Contains("Generic"))
            {
                string? tempCurrentDirectory = Path.GetDirectoryName(currentDirectory);
                DebugOutput.Log($"temp current directory = {tempCurrentDirectory}");
                string? oneUpDirectory = Path.GetDirectoryName(tempCurrentDirectory);
                DebugOutput.Log($"one up directory = {tempCurrentDirectory}");
                if (oneUpDirectory != null)
                {
                    currentDirectory = oneUpDirectory;
                }
            }
            else
            {
                DebugOutput.Log($"We at the top! {currentDirectory}");
                Directory.SetCurrentDirectory(currentDirectory);
                return true;
            }
            DebugOutput.Log($"We need to recheck");
            SetCurrentDirectoryToTop(); 
            return false;
        }

        public static bool UpdateTextFile(string fullPathAndFileName, string message)
        {
            DebugOutput.OutputMethod($"UpdateImageWarningFile", message);
            var fileName = StringValues.TextReplacementService(fullPathAndFileName);
            StreamWriter? sw = null;
            FileStream? fs = null;
            try
            {
                fs = File.Open(fullPathAndFileName, FileMode.Append, FileAccess.Write);
                if (fs.Length == 0)
                {
                    // File is empty, add header line
                    sw = new StreamWriter(fs);
                    sw.WriteLine(" *** WARNINGS *** ");
                    sw.WriteLine("  ");
                }
                var now = DateTime.Now;
                sw = new StreamWriter(fs);
                sw.WriteLine(now.ToString() + " - " + message);
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Failed to update the warning text file {fullPathAndFileName}! {e}");
                return false;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
            return true;
        }

        public static bool UpdateImageWarningFile(string message)
        {
            DebugOutput.Log($"UpdateImageWarningFile {message}");
            SetCurrentDirectoryToTop();
            var currentDirectory = Directory.GetCurrentDirectory();
            var fileName = StringValues.TextReplacementService(imageTextFile);
            var fullFileName = currentDirectory + fileName;
            return UpdateTextFile(fullFileName, message);
        }


        public static string GetRepoDirectory()
        {
            DebugOutput.Log($"proc - GetRepoDirectory ");
            var currentDirectory = Directory.GetCurrentDirectory();
            DebugOutput.Log($"Current Directory is {currentDirectory} This is your REPO, me hope!");
            return currentDirectory;
        }        
        

       /// <summary>
       /// Just return the directory where images
       /// For this project should be stored
       /// </summary>
       /// <param name="elementName"></param>
       /// <returns>returns true if successful in finding file of elementName</returns>
        public static string GetImagesProjectDirectory()
        {
            DebugOutput.Log($"Sel - GetImagesProjectDirectory");
            var project = TargetConfiguration.Configuration.AreaPath;
            var directory = GetImagesDirectory() + "/" + project;
            DebugOutput.Log($"GetImagesProjectDirectory directory = {directory}");
            if (!FileUtils.OSDirectoryCheck(directory)) FileUtils.OSDirectoryCreation(directory);
            return directory;
        }


        public static string GetErrorImagesDirectory()
        {
            DebugOutput.Log($"Sel - GetErrorImagesDirectory");
            var project = TargetConfiguration.Configuration.AreaPath;
            SetCurrentDirectoryToTop();
            var directory = currentDirectory + "/AppSpecFlow/TestResults/";
            DebugOutput.Log($"GetErrorImagesDirectory directory = {directory}");
            return directory;
        }


        public static string GetImagesDirectory()
        {
            DebugOutput.Log($"Sel - GetImagesDirectory");
            var project = TargetConfiguration.Configuration.AreaPath;
            SetCurrentDirectoryToTop();
            var directory = currentDirectory + "/AppSpecFlow/TestOutput/PageImages";
            DebugOutput.Log($"GetImagesDirectory directory = {directory}");
            return directory;
        }

        public static string GetImagesErrorDirectory()
        {
            DebugOutput.Log($"Sel - GetImagesDirectory");
            var project = TargetConfiguration.Configuration.AreaPath;
            SetCurrentDirectoryToTop();
            var directory = currentDirectory + "/AppSpecFlow/TestResults";
            DebugOutput.Log($"GetImagesErrorDirectory directory = {directory}");
            return directory;
        }
        

        public static string GetImagePageDirectory(string pageName)
        {
            DebugOutput.Log($"Sel - GetPageImagesDirectory");
            var directory = GetImagesProjectDirectory() + "/" + pageName;
            DebugOutput.Log($"GetPageImagesDirectory directory = {directory}");
            if (!FileUtils.OSDirectoryCheck(directory)) FileUtils.OSDirectoryCreation(directory);
            return directory;
        }


        public static StreamReader GetStream(string pathAndFileName, bool OS = false)
        {
            DebugOutput.Log($"proc GetStream {pathAndFileName}");
            string fullFileName = pathAndFileName;
            if (OS == false)
            {
                fullFileName = GetCorrectDirectory(pathAndFileName);
            }
            StreamReader r = new StreamReader(fullFileName);
            return r;
        }

        public static bool OSDeleteDirectoryIfExists(string directory)
        {
            DebugOutput.Log($"Proc - DeleteDirectoryIfExists {directory}");
            if (!DoesDirectoryExist(directory)) return true;
            DebugOutput.Log($"Need to delete it!");
            return OSDirectoryDeletion(directory);
        }

        public static bool DirectoryCheck(string directory)
        {
            var fullFileName = GetCorrectDirectory(directory);
            DebugOutput.Log($"proc DirectoryCheck {directory}");
            try
            {
                return Directory.Exists(fullFileName);
            }
            catch
            {
                DebugOutput.Log($"Failed to check directory {fullFileName}");
                return false;
            }
        }
        public static bool DoesDirectoryExist(string directory)
        {
            DebugOutput.Log($"Proc - DoesDirectoryExist {directory}");
            return OSDirectoryCheck(directory);
        }

        public static bool FileCheck(string pathAndFileName)
        {
            DebugOutput.Log($"proc FileCheck {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            DebugOutput.Log($"WE IS LOOKING FOR {fullFileName}");
            return File.Exists(fullFileName);
        }

        public static string? GetFileContents(string pathAndFileName)
        {
            DebugOutput.Log($"proc GetFileContents {pathAndFileName} ");
            string readContents = "";
            using (StreamReader streamReader = new StreamReader(pathAndFileName))
            {
                readContents = streamReader.ReadToEnd();                
            }
            if (readContents == "") return null;
            return readContents;
        }

        public static bool FileContains(string pathAndFileName, string findString)
        {
            DebugOutput.Log($"proc FileContains {pathAndFileName} {findString}");
            if (pathAndFileName == null) return false;
            if (findString == null) return false;
            if (!File.Exists(pathAndFileName)) return false;
            DebugOutput.Log($"File exists!");
            var readContents = GetFileContents(pathAndFileName);
            if (readContents == null) return false;
            if (readContents.Contains(findString)) return true;
            DebugOutput.Log($"Have string contents of file - but failed to find!");
            return false;
        }

        public static bool FileDeletionAndConfirm(string pathAndFileName)
        {
            DebugOutput.Log($"proc FileDeletionAndConfirm {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            if (!FileCheck(pathAndFileName))
            {
                DebugOutput.Log($"File {fullFileName} does not exist - so can not delete, so failing this!");
                return false;
            }
            if (!FileDeletion(pathAndFileName))
            {
                DebugOutput.Log($"Failed to delete file {fullFileName}");
                return false;
            }
            if (FileCheck(pathAndFileName))
            {
                DebugOutput.Log($"File {fullFileName} still exists after deletion attempt!");
                return false;
            }
            DebugOutput.Log($"File {fullFileName} has been deleted and confirmed!");
            return true;
        }

        public static bool FileDeletion(string pathAndFileName)
        {
            bool deleted = false;
            DebugOutput.Log($"proc FileDeletion {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            if (FileCheck(pathAndFileName))
            {
                try
                {
                    DebugOutput.Log($"STARTING DELETE! {fullFileName}");
                    File.Delete(fullFileName);
                    deleted = true;
                    DebugOutput.Log($"DELETING! {fullFileName}");
                    return deleted;
                }
                catch (Exception ex)
                {
                    DebugOutput.Log($"Failed to delete file {fullFileName} reason = {ex}");
                    return deleted;
                }
            }
            DebugOutput.Log($"File {fullFileName} does not exist - so can not delete, but still passed!");
            deleted = true;
            return deleted;
        }

        public static string? GetFileContentsAsString(string pathAndFileName)
        {
            DebugOutput.Log($"proc GetFileContentsAsString {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            return OSGetFileContentsAsString(fullFileName);
        }

        public static List<string>? OSGetFileContentsAsListOfStringByLine(string fullPathAndFileName)
        {
            DebugOutput.Log($"proc OSGetFileContentsAsListOfStringByLine {fullPathAndFileName}");
            List<string> lines = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(fullPathAndFileName))
                {
                    string line;
                    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                    #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
            }
            catch (Exception e)
            {
                DebugOutput.Log($"The file could not be read: {fullPathAndFileName} {e.Message}");
                return null;
            }
            return lines;
        }

        public static string OSGetFileContentsAsListOfStringByLineAfterAGivenLine(string fullPathAndFileName, string afterThisLine)
        {
            DebugOutput.Log($"proc OSGetFileContentsAsListOfStringByLineAfterAGivenLine {fullPathAndFileName} after {afterThisLine}");
            StringBuilder lines = new StringBuilder();
            bool found = false;
            try
            {
                using (StreamReader sr = new StreamReader(fullPathAndFileName))
                {
                    string line;
                    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (found)
                        {
                            lines.AppendLine(line);
                        }
                        if (line.Contains(afterThisLine)) found = true;
                    }
                    #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
            }
            catch (Exception e)
            {
                DebugOutput.Log($"The file could not be read: {fullPathAndFileName} {e.Message}");
                return "";
            }
            return lines.ToString();
        }

        public static string? OSGetFileContentsAsString(string fullPathAndFileName)
        {
            DebugOutput.Log($"proc OSGetFileContentsAsString {fullPathAndFileName}");
            try
            {
                return File.ReadAllText(fullPathAndFileName);
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to read all text in {fullPathAndFileName} {ex}.");
                return null;
            }
        }

        public static bool OSDirectoryCreation(string path)
        {
            DebugOutput.Log($"proc OSDirectoryCreation {path}");
            try
            {
                Directory.CreateDirectory(path);
                DebugOutput.Log($"Directory created {path}");
                return true;
            }
            catch
            {
                DebugOutput.Log($"an issue with creating directory {path}");
                return false;
            }
        }

        public static bool DirectoryCreation(string pathAndFileName)
        {
            DebugOutput.Log($"proc DirectoryCreation {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            try
            {
                Directory.CreateDirectory(fullFileName);
                DebugOutput.Log($"Directory created {fullFileName}");
                return true;
            }
            catch
            {
                DebugOutput.Log($"an issue with creating directory {fullFileName}");
                return false;
            }
        }

        public static bool FileLinePopulate(string pathAndFileName, string lineText)
        {
            DebugOutput.Log($"proc FileLinePopulate {pathAndFileName} {lineText}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            DebugOutput.Log($"FileLinePopulate OUTPUTTING TO {fullFileName}");
            try
            {
                using (StreamWriter sw = File.AppendText(fullFileName))
                {
                    sw.WriteLine(lineText);
                    return true;
                }	
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Failed to write line {lineText} into {fullFileName}  {e}");
                return false;
            }

        }

        public static bool FilePopulate(string pathAndFileName, string text)
        {
            DebugOutput.Log($"proc FilePopulate {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            DebugOutput.Log($"OUTPUTTING TO {fullFileName}");
            try
            {
                File.WriteAllText(fullFileName, text);
                DebugOutput.Log($"CREATED FILE {fullFileName}");
                return true;
            }
            catch
            {
                DebugOutput.Log($"FAILED TO CREATE FILE {fullFileName} Insert Line");
                return false;
            }
        }

        public static bool FileCreationAndPopulate(string pathAndFileName, string text)
        {
            DebugOutput.Log($"proc FileCreationAndPopulate {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            if (!OSFileCreation(fullFileName))
            {
                DebugOutput.Log($"Failed to create file! {fullFileName}");
                return false;
            } 
            DebugOutput.Log($"Populate this thing now!");
            try
            {
                // StringBuilder content = new StringBuilder();
                // // Append each line to the StringBuilder
                // content.Append(text);
                // // Write the content to the file
                if (!OSAppendLineToFile(fullFileName, text))
                {
                    DebugOutput.Log($"Failed to OSAppendLineToFile file! {fullFileName}");
                    return false;
                }
                DebugOutput.Log($"File {fullFileName} populated!");
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to create file {pathAndFileName}");
                return false;
            }
        }

        public static bool FileCreation(string pathAndFileName)
        {
            DebugOutput.Log($"proc FileCreation {pathAndFileName}");
            var fullFileName = GetCorrectDirectory(pathAndFileName);
            try
            {
                File.Create(fullFileName);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to create file {pathAndFileName}");
                return false;
            }
        }

        public static bool OSAppendLineToFile(string fullPathAndFileName, string text)
        {
            DebugOutput.OutputMethod("OSAppendLineToFile", $"{fullPathAndFileName}");
            if (!OSFileCheck(fullPathAndFileName))
            {
                DebugOutput.Log($"File Must already exist! Check its OS!");
                fullPathAndFileName = GetCorrectDirectory(fullPathAndFileName);
            }
            DebugOutput.Log($"To WRITE! {fullPathAndFileName}");
            try
            {
                using (StreamWriter writer = File.AppendText(fullPathAndFileName))
                {
                    writer.WriteLine(text);
                }
                return true;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Failed to append to file {e}");
                return false;
            }
        }

        public static bool OSFileCreationAndPopulation(string fullPath, string fileName, StringBuilder text)
        {
            string singleString = text.ToString();
            return OSFileCreationAndPopulation(fullPath, fileName, singleString);
        }

        public static bool OSFileCreationAndPopulation(string fullPath, string fileName, string text)
        {
            DebugOutput.Log($"proc OSFileCreationAndPopulation {fullPath} {fileName} {text} string text");
            try
            {
                // Create a StringBuilder to build the content
                StringBuilder content = new StringBuilder();

                // Append each line to the StringBuilder
                content.Append(text);
                // Write the content to the file
                var fullPathAndFileName = fullPath + fileName;
                File.WriteAllText(fullPathAndFileName, content.ToString());
                DebugOutput.Log($"File {fullPathAndFileName} populated!");
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Error creating file: {ex.Message}");
                return false;
            }

        }

        public static bool OSFileCreationAndPopulation(string fullPath, string fileName, string[] lines)
        {
            DebugOutput.Log($"proc OSFileCreationAndPopulation {fullPath} {fileName}");
            try
            {
                // Create a StringBuilder to build the content
                StringBuilder content = new StringBuilder();

                // Append each line to the StringBuilder
                foreach (string line in lines)
                {
                    content.AppendLine(line);
                }

                // Write the content to the file
                var fullPathAndFileName = fullPath + fileName;
                File.WriteAllText(fullPathAndFileName, content.ToString());
                DebugOutput.Log($"File {fullPathAndFileName} populated!");
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Error creating file: {ex.Message}");
                return false;
            }
        }

        public static bool OSFileCreation(string pathAndFileName)
        {
            DebugOutput.Log($"proc OSFileCreation {pathAndFileName}");
            try
            {
                DebugOutput.Log($"Creating file: {pathAndFileName}");
                // Create the file and close the FileStream
                using (FileStream fs = File.Create(pathAndFileName))
                {
                    // You can write to the file here if needed
                }
                return true;
            }
            catch
            {
                DebugOutput.Log($"OSFileCreation Failed to create file {pathAndFileName}");
                return false;
            }
        }

        public static bool CopyDirectoryFromProjectToOS(string projectDirectory, string osDirectory)
        {
            DebugOutput.Log($"proc MoveDirectoryFromProjectToOS {projectDirectory} to {osDirectory}");
            projectDirectory = GetCorrectDirectory(projectDirectory);
            return CopyDirectory(projectDirectory, osDirectory);
        }

        public static bool OSClearContentsOfAFile(string osFile)
        {
            DebugOutput.OutputMethod($"OSClearContentsOfAFile", $"{osFile}");
            try
            {
                File.WriteAllText(osFile, string.Empty);
                return true;
            }
            catch(Exception e)
            {
                DebugOutput.Log($"Failed to clear contents! {e}");
                return false;
            }
        }

        public static bool OSCopyFile(string sourceFile, string destinationFile, bool overwrite = false)
        {
            DebugOutput.OutputMethod($"OSCopyFile", $"{sourceFile} {destinationFile} {overwrite}");
            try
            {
                File.Copy(sourceFile, destinationFile, overwrite);
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"DID NOT COPY {sourceFile} TO {destinationFile} and overwite {overwrite}.  {ex}");
                return false;
            }
        }

        public static bool OSCopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
        {
            return CopyDirectory(sourceDir, destinationDir, recursive);
        }

        public static bool CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
        {
            DebugOutput.Log($"proc CopyDirectory {sourceDir} to {destinationDir} recursive {recursive}");
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);
            // Check if the source directory exists
            if (!dir.Exists) return false;

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();
            // Create the destination directory
            Directory.CreateDirectory(destinationDir);
            DebugOutput.Log($"Attempt copy!");
            try
            {
                // Get the files in the source directory and copy to the destination directory
                foreach (FileInfo file in dir.GetFiles())
                {
                    string targetFilePath = Path.Combine(destinationDir, file.Name);
                    file.CopyTo(targetFilePath);
                }
            }
            catch
            {
                DebugOutput.Log($"Problem with the Copy of Files!");
                return false;
            }
            try
            {
                DebugOutput.Log($"Attempt recusive");
                // If recursive and copying subdirectories, recursively call this method
                if (recursive)
                {
                    foreach (DirectoryInfo subDir in dirs)
                    {
                        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                        CopyDirectory(subDir.FullName, newDestinationDir, true);
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"FAiled on recursive");
                return false;
            }
            DebugOutput.Log($"COPY COMPELTE!");
            return true;
        }

        public static bool MoveRenameFile(string pathAndFileName, string newPathAndFileName)
        {
            DebugOutput.Log($"proc MoveRenameFile {pathAndFileName} to {newPathAndFileName}");
            bool moved = false;
            if (FileCheck(newPathAndFileName))
            {
                DebugOutput.Log($"Moving a file where a file already exists!");
                if (!FileDeletion(newPathAndFileName))
                {
                    DebugOutput.Log($"FAILED TO DELETE!");
                    return moved;
                }
            }
            var oldFile = GetCorrectDirectory(pathAndFileName);
            var newFile = GetCorrectDirectory(newPathAndFileName);
            try
            {
                File.Move(oldFile, newFile);
                moved = true;
                DebugOutput.Log($"Move completed {oldFile} has moved to {oldFile}");
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Has been an issue moving the files! {ex}");
            }
            return moved;
        }

        public static bool OSDirectoryClean(string fullPath)
        {
            DebugOutput.Log($"proc OSDirectoryClean {fullPath}");
            if (!OSDirectoryCheck(fullPath)) 
            {
                DebugOutput.Log($"The directory does not exist!  I can not clean something that does not exist!");
                return false;
            }
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(fullPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete(); 
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true); 
                }
                return true;
            }
            catch
            {
                DebugOutput.Log($"Something went wrong when trying to clear out the directory {fullPath}");
                return false;
            }
        }

        public static bool OSDirectoryDeletion(string fullPath)
        {
            bool deleted = false;
            DebugOutput.Log($"proc OSDirectoryDeletion {fullPath}");
            if (OSDirectoryCheck(fullPath))
            {
                DebugOutput.Log($"Directory Exists!  TO DELETE");
                try
                {
                    Directory.Delete(fullPath, true);
                    deleted = true;
                    DebugOutput.Log($"DELETED! {fullPath}");
                    return deleted;
                }
                catch (Exception ex)
                {
                    DebugOutput.Log($"problem deleting directory!  {fullPath} = {ex}");
                    return deleted;
                }
            }
            DebugOutput.Log($"Directory {fullPath} no longer exists - so maybe ALREADY deleted!");
            deleted = true;
            return deleted;
        }

        public static bool OSDirectoryCheck(string fullPath)
        {
            bool exists = false;
            DebugOutput.Log($"proc - OSDirectoryCheck {fullPath}");
            try
            {
                exists = Directory.Exists(fullPath);
                return exists;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to find directory {fullPath} {ex}");
                return exists;
            }
        }

        public static bool OSFileCheck(string fullFileName)
        {
            DebugOutput.Log($"proc OSFileCheck {fullFileName}");
            bool exists = false;
            try
            {
                exists = File.Exists(fullFileName);
                return exists;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to file {ex}");
                return exists;
            }
        }

        public static bool OSFileDeletion(string fullName)
        {
            bool deleted = false;
            DebugOutput.Log($"proc OSFileDeletion {fullName}");
            if (OSFileCheck(fullName))
            {
                DebugOutput.Log($"File Exists!  TO DELETE");
                try
                {
                    File.Delete(fullName);
                    deleted = true;
                    DebugOutput.Log($"DELETED! {fullName}");
                    return deleted;
                }
                catch (Exception ex)
                {
                    DebugOutput.Log($"problem deleting!  {fullName} = {ex}");
                    return deleted;
                }
            }
            DebugOutput.Log($"File does not exist! So can not delete something not there - but then! ITS GONE ANYWAY!");
            deleted = true;
            return deleted;
        }

        public static string? GetConvertFileToBase64(string fileName)
        {
            DebugOutput.OutputMethod("GetConvertFileToBase64", $"{fileName}");
            var fullFileName = GetCorrectDirectory(fileName);
            try
            {
                byte[] bytes = File.ReadAllBytes(fullFileName);
                string base64String = Convert.ToBase64String(bytes);
                return base64String;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to convert file to base64 {ex}");
                return null;
            }
        }

        public static string? OSGetConvertFileToBase64(string fileNameAndLocation)
        {
            DebugOutput.OutputMethod("OSConvertFileToBase64", $"{fileNameAndLocation}");            
            try
            {
                byte[] imageBytes = File.ReadAllBytes(fileNameAndLocation);
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting image to Base64: {ex.Message}");
                return null;
            }
        }

        public static bool OSDeleteFilesOfTypeInADirectory(string directory, string fileExtension = "log")
        {
            DebugOutput.Log($"OSDeleteFilesOfTypeInADirectory {directory} {fileExtension}");
            var listOfFilesInDirectory = OSGetListOfFilesInDirectory(directory, fileExtension);
            foreach (var file in listOfFilesInDirectory)
            {
                if (file.ToLower().Contains(fileExtension.ToLower()))
                {
                    var fullFile = directory + file;
                    DebugOutput.Log($"Deleting {fullFile}");
                    try
                    {
                        OSFileDeletion(fullFile);
                    }
                    catch
                    {
                        DebugOutput.Log($"Failed to delete {fullFile}");
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool OSDeleteFilesOfADirectory(string directory)
        {
            DebugOutput.Log($"OSDeletFilesOfADirectory {directory}");
            var listOfFilesInDirectory = OSGetListOfFilesInDirectory(directory);
            foreach(var file in listOfFilesInDirectory)
            {
                var fullFile = directory + file;
                DebugOutput.Log($"Deleting {fullFile}");
                try
                {
                    OSFileDeletion(fullFile);
                }
                catch
                {
                    DebugOutput.Log($"Failed to delete {fullFile}");
                    return false;
                }
            }
            return true;
        }

        public static bool OSDeleteContentsOfADirectory(string directory)
        {
            DebugOutput.Log($"OSDeleteContentsOfADirectory {directory}");
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(directory);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    DebugOutput.Log($"File = {file.FullName}");
                    file.Delete(); 
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true); 
                }
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed cause of some reason;");
                return false;
            }
        }
        
        
        /// <summary>
        /// Get all files in a directory where the file name contains a specific string and has a specific extension.
        /// Returns a list of files that can be ordered by created date, modified date, etc.
        /// </summary>
        /// <param name="directory">The directory to search in.</param>
        /// <param name="containsString">The string that the file name should contain.</param>
        /// <param name="extension">The file extension to filter by (e.g., "txt", "json").</param>
        /// <param name="orderBy">The property to order the files by ("created", "modified").</param>
        /// <returns>A list of file paths matching the criteria, ordered as specified.</returns>
        public static List<string> OSGetFilesByCriteria(string directory, string containsString, string extension, string orderBy = "created")
        {
            DebugOutput.Log($"GetFilesByCriteria: Searching in {directory} for files containing '{containsString}' with extension '{extension}' ordered by {orderBy}");
            if (extension.StartsWith(".")) extension = extension.Substring(1); // Remove leading dot if present
            List<FileInfo> files = new List<FileInfo>();

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);

                // Get all files with the specified extension
                string searchPattern = $"*.{extension}";
                FileInfo[] allFiles = dirInfo.GetFiles(searchPattern);

                // Filter files by the string they contain
                foreach (var file in allFiles)
                {
                    if (file.Name.Contains(containsString, StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(file);
                    }
                }

                // Order files based on the specified property
                if (orderBy.Equals("created", StringComparison.OrdinalIgnoreCase))
                {
                    files = files.OrderByDescending(f => f.CreationTime).ToList();
                }
                else if (orderBy.Equals("modified", StringComparison.OrdinalIgnoreCase))
                {
                    files = files.OrderByDescending(f => f.LastWriteTime).ToList();
                }
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Error in GetFilesByCriteria: {ex.Message}");
            }

            // Return the full paths of the files
            return files.Select(f => f.FullName).ToList();
        }


        /// <summary>
        /// This will find every file with extention with the string findString in and under this directory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="extension"></param>
        /// <param name="findString"></param>
        /// <returns></returns>
        public static List<string> OSGetListOfFilesInAllDirectoriesOfTypeContainingWord(string directory, string extension = "", string findString = "")
        {
            DebugOutput.Log($"OSGetListOfFilesInDirectoryOfTypeContainingWord {directory}  {extension} {findString}");
            List<string> fileNames = new List<string>();
            string fileExtension = "*";
            if (fileExtension != "")
            {
                fileExtension = "*." + extension;
            }  // *.json
            var jsonFilesArray = Directory.GetFiles(directory, fileExtension, SearchOption.AllDirectories);
            var jsonFiles = StringValues.ConvertArrayToList(jsonFilesArray);
            foreach (var jsonFile in jsonFiles)
            {
                if (jsonFile.ToLower().Contains(findString.ToLower())) fileNames.Add(jsonFile);
            }
            return fileNames;
        }


        public static List<string> OSGetListOfFilesInDirectoryOfTypeContainingWord(string directory, string extension = "", string findString = "")
        {
            DebugOutput.Log($"OSGetListOfFilesInDirectoryOfTypeContainingWord {directory}  {extension} {findString}");
            List<string> fileNames = new List<string>();
            DirectoryInfo d = new DirectoryInfo(directory); 
            string fileExtension = "*";
            if (fileExtension != "")
            {
                fileExtension = "*." + extension;
            }
            FileInfo[] Files = d.GetFiles(fileExtension); //Getting files
            foreach(FileInfo file in Files)
            {
                if (findString == "")
                {
                    fileNames.Add(file.Name);
                }
                else
                {
                    if (file.Name.ToLower().Contains(findString.ToLower()))
                    {
                        fileNames.Add(file.Name);
                    }
                    else
                    {
                        DebugOutput.Log($"Not added as the file name {file.Name} does not inclued {findString}");
                    }
                }
            }
            DebugOutput.Log($"Returing {fileNames.Count} File Names");
            return fileNames;


        }

        public static List<string> OSGetListOfFilesInDirectoryOfTypeContainStringInName(string directory, string extension = "", string containString = "")
        {
            DebugOutput.Log($"OSGetListOfFilesInDirectoryOfTypeContainStringInName {directory} {extension} {containString}");   
            var foundFileNames = new List<string>();     
            var listOfLogFiles = OSGetListOfFilesInDirectoryOfType(directory, extension);
            foreach (var fileName in listOfLogFiles)
            {
                DebugOutput.Log($"File Name = {fileName} does it contain {containString}");
                if (fileName.Contains(containString))
                {
                    foundFileNames.Add(fileName);
                }
            }
            return foundFileNames;
        }

        public static List<string> OSGetListOfFilesInDirectoryOfType(string directory, string extension = "")
        {
            DebugOutput.Log($"GetFilesByModifiedDate {directory} {extension}");
            List<FileInfo> sortedFiles = new List<FileInfo>();

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                string fileExtension = extension.StartsWith(".") ? $"*{extension}" : $"*.{extension}";

                // Get files and sort them by LastWriteTime
                sortedFiles = dirInfo.GetFiles(fileExtension)
                                    .OrderByDescending(file => file.LastWriteTime)
                                    .ToList();

                DebugOutput.Log($"Found {sortedFiles.Count} files sorted by modified date.");
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Error retrieving files by modified date: {ex.Message}");
            }
            List<string> fileNames = new List<string>();
            foreach (var file in sortedFiles)
            {
                fileNames.Add(file.Name);
            }
            return fileNames;
        }

        public static List<string> OSGetListOfJsonFilesInDirectory(string directory)
        {
            DebugOutput.Log($"OSGetListOfFilesInDirectory {directory}");
            List<string> fileNames = new List<string>();
            fileNames = OSGetListOfFilesInDirectoryOfType(directory, "json");
            return fileNames;
        }

        public static List<string> OSGetListOfDirectoriesInDirectory(string directory)
        {
            DebugOutput.Log($"OSGetListOfFilesInDirectory {directory}");
            var listOfDirectories = new List<string>();
            if (!OSDirectoryCheck(directory)) return listOfDirectories;
            var directoryInformation = new DirectoryInfo(directory);
            foreach (var dir in directoryInformation.GetDirectories())
            {
                listOfDirectories.Add(dir.FullName);
            }
            return listOfDirectories;
        }

        public static string? OSGetFullFileNameOfNewestFileInDirectory(string directory)
        {
            DebugOutput.Log($"proc OSGetFullFileNameOfNewestFileInDirectory {directory}  ");  
            FileInfo[] files = new DirectoryInfo(directory).GetFiles();
            FileInfo newestFile = files[0];
            foreach (FileInfo file in files)
            {
                if (file.CreationTime > newestFile.CreationTime)
                {
                    newestFile = file;
                }
            }
            return newestFile.FullName;
        }

        public static DateTime OSGetFileCreationTime(string filePath)
        {
            try
            {
                return File.GetCreationTime(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file creation time: {ex.Message}");
                return DateTime.MinValue;
            }
        }

        
        public static string? OSGetFileContentsAsStringFromAZipFile(string fileName, string zipFileLocation, string zipFileName = "")
        {            
            DebugOutput.Log($"proc OSGetFileContentsAsStringFromAZipFile {fileName} {zipFileLocation} {zipFileName} ");
            string? newZipFileName = null;
            if (zipFileName == "") newZipFileName = OSGetFullFileNameOfNewestFileInDirectory(zipFileLocation);
            if (newZipFileName == null) return null;
            zipFileName = newZipFileName;
            DebugOutput.Log($"proc OSGetFileContentsAsStringFromAZipFile {fileName} {zipFileLocation} {zipFileName} ");
            var fullPathAndFileName = "";
            if (zipFileLocation.EndsWith(@"\") || zipFileLocation.EndsWith("/")) fullPathAndFileName = zipFileLocation + zipFileLocation;
            else fullPathAndFileName = zipFileLocation + "/" + zipFileLocation;

            using (ZipArchive archive = ZipFile.OpenRead(fullPathAndFileName))
            {
                ZipArchiveEntry? entry = archive.GetEntry(fileName);
                if (entry != null)
                {
                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        return reader.ReadToEnd();  
                    }
                }
            }
            
            DebugOutput.Log($"Failed some where reading zip!");
            return null;
        }


        public static List<string> OSGetListOfFilesInDirectory(string directory, string extension = "json")
        {
            DebugOutput.Log($"OSGetListOfFilesInDirectory {directory} {extension}");
            List<string> fileNames = new List<string>();
            fileNames = OSGetListOfFilesInDirectoryOfType(directory, extension);
            return fileNames;
        }

        public static List<string> OSGetListOfAllFilesWithExtension(string rootDirectory, string extension)
        {
            DebugOutput.Log($"OSGetListOfAllFilesWithExtension {rootDirectory} {extension}");
            List<string> files = new List<string>();

            if (!Directory.Exists(rootDirectory))
                return files;

            Stack<string> directories = new Stack<string>();
            directories.Push(rootDirectory);

            while (directories.Count > 0)
            {
                string currentDirectory = directories.Pop();

                try
                {
                    foreach (string file in Directory.GetFiles(currentDirectory, "*." + extension))
                    {
                        files.Add(file);
                    }

                    foreach (string subDirectory in Directory.GetDirectories(currentDirectory))
                    {
                        directories.Push(subDirectory);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip directories without access
                    continue;
                }
            }

            return files;
        }


		/// <summary>
		/// count how many lines in a string
		/// </summary>
		/// <param name=""></param>
		/// <returns>the count of how many!</returns>
        public static int GetNumberOfLinesInFile(string fullFileName, bool excludeTop = true, bool excludeBottom = true)
        {
            if (!OSFileCheck(fullFileName)) return -1;
            var allLines = File.ReadLines(fullFileName).Count();
            if (excludeTop) allLines--;
            if (excludeBottom) allLines--;
            return allLines;
        }

        public static bool OSMoveFileFromTo(string fullFileName, string moveToDirectory)
        {
            DebugOutput.Log($"Proc - OSMoveFileFromTo {fullFileName} to {moveToDirectory}");
            var fileName = Path.GetFileName(fullFileName);
            DebugOutput.Log($"File name = {fileName}");
            var newFileName = moveToDirectory + fileName;
            DebugOutput.Log($"New name = {fileName}");
            try
            {
                File.Move(fullFileName, newFileName, false);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failed to move - CAN NOT OVERWRITE!");
                return false;
            }
        }

        public static bool OSRenameDirectory(string oldpathAndFileName, string newpathAndFileName)
        {
            DebugOutput.Log($"Proc - RenameDirectory {oldpathAndFileName} to {newpathAndFileName}");
            try
            {
                Directory.Move(oldpathAndFileName, newpathAndFileName);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failure in renaming Directory");
            }
            return false;   
        }

        public static bool OSRenameFile(string oldpathAndFileName, string newpathAndFileName)
        {
            DebugOutput.Log($"Proc - OSRenameFile {oldpathAndFileName} to {newpathAndFileName}");
            try
            {
                File.Move(oldpathAndFileName, newpathAndFileName);
                return true;
            }
            catch
            {
                DebugOutput.Log($"Failure in renaming File");
            }
            return false;

        }

        public static string OSGetAllTextInFile(string pathAndFileName)
        {
            string allText = "";
            try
            {
                allText = File.ReadAllText(pathAndFileName);
            }
            catch
            {
                DebugOutput.Log($"Failed to read ll text of {pathAndFileName}");
                return "";
            }
            return allText;
        }

        public static bool OSReplaceFullTextInFile(string pathAndFileName, string allText)
        {
            DebugOutput.Log($"Proc - OSReplaceFullTextInFile {pathAndFileName} ");
            try
            {
                File.WriteAllText(pathAndFileName, allText);
                DebugOutput.Log($"{pathAndFileName} is updated!");
                return true;
            }
            catch
            {
                DebugOutput.Log($"Problem writing the allText with updates to file {pathAndFileName}");
                return false;
            }
        }

        public static bool OSReplaceTextInFile(string pathAndFileName, string toBeReplaced = "", string withWhat = "")
        {
            DebugOutput.Log($"Proc - OSReplaceTextInFile {pathAndFileName} to {toBeReplaced} with {withWhat}");
            string allText = OSGetAllTextInFile(pathAndFileName);
            if (allText == "")
            {
                DebugOutput.Log($"File is EMPTY!");
                return false;
            }
            allText = StringValues.TextReplacementService(allText);
            if (toBeReplaced != "")
            {
                if (withWhat != "")
                {
                    try
                    {
                        allText = allText.Replace(toBeReplaced, withWhat);
                    }
                    catch
                    {
                        DebugOutput.Log($"Failed on the single text replacement in file!");
                    }
                }
                else
                {
                    DebugOutput.Log($"FAILED TO REPLACE with NOTHING!  can not do this in this method!");
                    return false;
                }
            }
            return OSReplaceFullTextInFile(pathAndFileName, allText);
        }

        public static string GetUserTempFolder()
        {
            return Path.GetTempPath();
        }

        public static bool OSCreateZipFile(string sourceDir, string zipFileLocation)
        {
            DebugOutput.OutputMethod("OSCreateZipFile", $"{sourceDir} {zipFileLocation}");
            try
            {
                // Create a ZIP archive from the specified directory
                ZipFile.CreateFromDirectory(sourceDir, zipFileLocation);
                DebugOutput.Log($"Successfully created ZIP file at: {zipFileLocation}");
                return true;
            }
            catch (Exception ex)
            { 
                DebugOutput.Log($"Failed to create zip {zipFileLocation} from source {sourceDir} {ex}");
                return false;
            }
        }

        public static string GetCorrectDirectory(string pathAndFileName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            DebugOutput.Log($"Current Directory is {currentDirectory}");
            if (currentDirectory.Contains("bin") && currentDirectory.Contains("Debug"))
            {
                DebugOutput.Log($"Need to move up directory");
                MoveUpToProjectDirectory();
            }
            currentDirectory = Directory.GetCurrentDirectory();
            var fullFileName = currentDirectory + pathAndFileName;
            DebugOutput.Log($"RETURNING FOR FULL NAME {fullFileName}");
            return fullFileName;
        }

        public static bool OSUnZipFile(string OSZipFileAndPath, string? extractPath = "")
        {
            DebugOutput.OutputMethod("UnZipFile", $"{OSZipFileAndPath} {extractPath}");
            // Get the extractPath - the zipFileAndPath without the file name
            var zipPath = Path.GetDirectoryName(OSZipFileAndPath);
            if (zipPath == null)
            {
                DebugOutput.Log($"Failed to get the directory from the zipFileAndPath {OSZipFileAndPath}");
                return false;
            }
            var zipFileName = Path.GetFileName(OSZipFileAndPath);
            if (zipFileName == null)
            {
                DebugOutput.Log($"Failed to get the file name from the zipFileAndPath {OSZipFileAndPath}");
                return false;
            }
            if (zipFileName.ToLower().EndsWith(".zip") == false)
            {
                DebugOutput.Log($"The file name {zipFileName} does not end with .zip so can not extract!");
                return false;
            }
            if (extractPath == "" || extractPath == null)
            {
                extractPath = zipPath + "/" + zipFileName.Replace(".zip", "");
            }
            DebugOutput.Log($"Extracting to {extractPath}");
            try
            {
                ZipFile.ExtractToDirectory(OSZipFileAndPath, extractPath);
                DebugOutput.Log($"Successfully extracted ZIP file to: {extractPath}");
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to extract zip {OSZipFileAndPath} to {extractPath} {ex}");
                return false;
            }
        }

        ///PRIVATE

        private static void MoveUpToProjectDirectory()
        {
            Environment.CurrentDirectory = "../../../../";
            DebugOutput.Log($"New Directory is { Directory.GetCurrentDirectory()}");
        }

    }
}
