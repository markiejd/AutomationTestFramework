using System.Configuration;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Text;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;

namespace AppSpecFlow.AppSteps
{
    public class FeatureFile
    {
        public int QuestionNumber { get; set ;} 
        public string Question { get; set ; } = "UNKNOWN";
        public string Answer { get; set ; } = "UNKNOWN";
        public string? MoreInformation { get; set ; }

    }

    public class FeatureFileListOfCSV
    {
        public string Hello()
        {
            return "hello";
        }

        public bool Failure(string message, bool pass = false)
        {
            DebugOutput.Log($"{message}");
            return pass;
        }

        
        public List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            string field = string.Empty;
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    fields.Add(field);
                    field = string.Empty;
                }
                else
                {
                    field += line[i];
                }
            }
            if (!string.IsNullOrEmpty(field))
            {
                fields.Add(field);
            }
            // Remove surrounding quotes from fields
            fields = fields.ConvertAll(f => f.Trim('"'));
            return fields;
        }


        public List<string> GetListOfLines(List<string> linesOfCSV, int startNumber, int howMany)
        {
            DebugOutput.Log($"GetListOfLines {linesOfCSV.Count} {startNumber} {howMany}");
            int counter = 0;
            List<string> FinalListOfCSV = new();
            foreach (string line in linesOfCSV)
            {
                DebugOutput.Log($"XXXX {counter} {startNumber} {howMany} ");
                if (counter >= startNumber && counter <= startNumber + howMany)
                {
                    DebugOutput.Log($"Adding 1 to {counter}");
                    FinalListOfCSV.Add(line);
                }
                counter ++;
            }
            return FinalListOfCSV;
        }

        /// Take list of CSV Lines and convert into Feature File.
        /// 
        public bool CSVListToFeatureFile(List<string> linesOfCSV, string featureFileDirectory, string fileName, string tag = "", int count = 0)
        {
            DebugOutput.Log($"CSVListToFeatureFile {linesOfCSV.Count} {featureFileDirectory} {fileName}");
            var fullPath = FileUtils.GetCorrectDirectory(featureFileDirectory);
            DebugOutput.Log($"CSVListToFeatureFile {linesOfCSV.Count} {fullPath}");
            var numberOfLinesExpected = linesOfCSV.Count;
            var listOfFeatureFile = new List<FeatureFile> ();
            foreach (var line in linesOfCSV)
            {
                var FeatureFileModel = new FeatureFile();
                var returnFromLine = ParseCsvLine(line);
                DebugOutput.Log($"That gives us {returnFromLine.Count} fields!");
                int counter = 0;
                foreach (var field in returnFromLine)
                {
                    DebugOutput.Log($"{field}");
                    if (counter == 0)
                    {
                        int questionNumber = StringValues.GetInt32FromString(field) ?? -1;
                        if (questionNumber == -1) return Failure("Failed to convert the first column to a number!");
                        FeatureFileModel.QuestionNumber = questionNumber;
                    }
                    if (counter == 1)
                    {
                        FeatureFileModel.Question = field.Replace("\"", "");
                    }
                    if (counter == 2)
                    {
                        FeatureFileModel.Answer = field.Replace("\"", "");
                    }
                    if (counter == 3)
                    {
                        FeatureFileModel.MoreInformation = field.Replace("\"", "");
                    }
                    counter ++;
                }
                listOfFeatureFile.Add(FeatureFileModel);
                DebugOutput.Log($"empty space!");
            }
            var brokenUpDirAndFile = StringValues.BreakUpByDelimitedToList(fileName, "/");
            var finalFileName = brokenUpDirAndFile[brokenUpDirAndFile.Count - 1];
            var numberOfModels = listOfFeatureFile.Count();
            if (numberOfLinesExpected != numberOfModels) return Failure("We do not match the number of lines to the number of models!");
            var lines = CreateTheFeatureFile(listOfFeatureFile, finalFileName, tag);
            if (lines == null) return Failure("Failed to populate the text for file!");
            return FileUtils.OSFileCreationAndPopulation(fullPath, finalFileName + ".feature", lines);
        }

        //  Take the Models
        // First create the feature File...
        private StringBuilder CreateTheFeatureFile(List<FeatureFile> ListOfFeatureFiles, string fileName = "", string tag = "", int count = 0)
        {
            DebugOutput.Log($"CreateTheFeatureFile {ListOfFeatureFiles.Count}");
            List<string> AllHeaderLines = new();
            // var counterText = "";
            // WANT TO ADD UNIQUE
            var headerOne = "";
            if (tag == "")
            {
                headerOne = $"@all  @{fileName} @York";
            } 
            else 
            {
                headerOne = $"@all  @{fileName} @York @{tag}";
            }
            AllHeaderLines.Add(headerOne);
            var headerTwo = $"Feature: {fileName} ReadFromCSV";
            AllHeaderLines.Add(headerTwo);
            var headerThree = $"    These tests are created by reading CSV file";
            AllHeaderLines.Add(headerThree);
            var headerFour = $"     .";
            AllHeaderLines.Add(headerFour);
            var headerFive = $"          dotnet test --filter:\"TestCategory={fileName}\" --logger \"trx;logfilename={fileName}.trx\"  ";
            AllHeaderLines.Add(headerFive);
            var headerSix = " ";
            AllHeaderLines.Add(headerSix);
            var headerSeven = $"Scenario Outline: {fileName}-ReadingFromCSV";
            AllHeaderLines.Add(headerSeven);
            var headerEight = " ";
            AllHeaderLines.Add(headerEight);
            var headerNineTwo = "  Given ChatBot Is Open";
            AllHeaderLines.Add(headerNineTwo);
            var headerTen = " ";
            AllHeaderLines.Add(headerTen);
            StringBuilder headerLines = new StringBuilder();
            foreach (string s in AllHeaderLines)
            {
                headerLines.AppendLine(s);
            }

            var numberToRunInSingleBrowser = 9;
            int counter = 1;

            // we loop through the ListOfFeatureFiles
            foreach (var scenario in ListOfFeatureFiles)
            {
                string? scenarioNumberAsString = StringValues.GetStringOfLengthFromInt(scenario.QuestionNumber, 5);
                // if (scenarioNumberAsString == null) return null;
                // var newLine = "Scenario: York-002-ReadingFromCSV-" + scenarioNumberAsString;
                var newLine = "";
                headerLines = headerLines.AppendLine(newLine);
                if (scenario.MoreInformation != null)
                {
                    newLine = "     # # # # #" + scenario.MoreInformation;
                    headerLines = headerLines.AppendLine(newLine);
                }
                newLine = $"# # # #  Question: {counter}";
                headerLines = headerLines.AppendLine(newLine);
                /*
                    When I Enter "What is WIP?" Then Press "Enter" In TextBox "question"
                    Then Question "What is WIP?" Is Newest Display
                    Then Wait For Button "Stop" To Not Be Displayed
                */
                newLine = "     When I Enter \"" + scenario.Question + "\" Then Press \"Enter\" In TextBox \"question\"";
                headerLines = headerLines.AppendLine(newLine);
                newLine = "     Then Question \"" + scenario.Question + "\" Is Newest Display";
                headerLines = headerLines.AppendLine(newLine);
                newLine = "     Then Wait For Answer To Stop";
                headerLines = headerLines.AppendLine(newLine);
                /*                
                    Then Last Answer Contains The Text "Work in Progress" 
                */
                newLine = "     Then Compare Answer With Expected Answer \"" + scenario.Answer + "\"";
                headerLines = headerLines.AppendLine(newLine);
                newLine = " ";
                headerLines = headerLines.AppendLine(newLine);

                // numberToRunInSingleBrowser set to 10, means every 10 questions, close and reopen!
                if (counter % numberToRunInSingleBrowser == 0)
                {
                    int waitTime = counter;
                    int pauseTimer = 30; // seconds
                    if (waitTime > pauseTimer) waitTime = pauseTimer;
                    newLine = "  Then Wait " + waitTime  + " Seconds";
                    headerLines = headerLines.AppendLine(newLine);
                    newLine = " ";
                    headerLines = headerLines.AppendLine(newLine);
                    newLine = "  Given ChatBot Is Closed";
                    headerLines = headerLines.AppendLine(newLine);
                    newLine = "  Then Wait " + waitTime  + " Seconds";
                    headerLines = headerLines.AppendLine(newLine);
                    newLine = " ";
                    headerLines = headerLines.AppendLine(newLine);
                    newLine = "  Given ChatBot Is Open";
                    headerLines = headerLines.AppendLine(newLine);
                }
                counter++;
            }          
            return headerLines;
        }

        /// Take list of CSV Lines and convert into Feature File.
        /// 
        public bool CSVListToVariantFeatureFile(List<string> linesOfCSV, string featureFileDirectory, string fileName)
        {
            DebugOutput.Log($"CSVListToFeatureFile {linesOfCSV.Count} {featureFileDirectory}");
            var fullPath = FileUtils.GetCorrectDirectory(featureFileDirectory);
            DebugOutput.Log($"CSVListToFeatureFile {linesOfCSV.Count} {fullPath}");
            var numberOfLinesExpected = linesOfCSV.Count;
            var listOfFeatureFile = new List<FeatureFile> ();
            foreach (var line in linesOfCSV)
            {
                var FeatureFileModel = new FeatureFile();
                var returnFromLine = ParseCsvLine(line);
                DebugOutput.Log($"That gives us {returnFromLine.Count} fields!");
                int counter = 0;
                foreach (var field in returnFromLine)
                {
                    DebugOutput.Log($"{field}");
                    if (counter == 0)
                    {
                        int questionNumber = StringValues.GetInt32FromString(field) ?? -1;
                        if (questionNumber == -1) return Failure("Failed to convert the first column to a number!");
                        FeatureFileModel.QuestionNumber = questionNumber;
                    }
                    if (counter == 1)
                    {
                        FeatureFileModel.Question = field.Replace("\"", "");
                    }
                    if (counter == 2)
                    {
                        FeatureFileModel.Answer = field.Replace("\"", "");
                    }
                    if (counter == 3)
                    {
                        FeatureFileModel.MoreInformation = field.Replace("\"", "");
                    }
                    counter ++;
                }
                listOfFeatureFile.Add(FeatureFileModel);
                DebugOutput.Log($"empty space!");
            }
            var numberOfModels = listOfFeatureFile.Count();
            if (numberOfLinesExpected != numberOfModels) return Failure("We do not match the number of lines to the number of models!");
            var lines = CreateTheVariantFeatureFile(listOfFeatureFile);
            if (lines == null) return Failure("Failed to populate the text for file!");
            foreach (var line in lines)
            {
                DebugOutput.Log(line);

            }
            var brokenUpDirAndFile = StringValues.BreakUpByDelimitedToList(fileName, "/");
            var finalFileName = brokenUpDirAndFile[brokenUpDirAndFile.Count - 1];
            return FileUtils.OSFileCreationAndPopulation(fullPath, finalFileName + ".feature", lines);
        }

        //  Take the Models
        // First create the feature File...
        private string[]? CreateTheVariantFeatureFile(List<FeatureFile> ListOfFeatureFiles)
        {
            DebugOutput.Log($"CreateTheFeatureFile {ListOfFeatureFiles.Count}");
            string[] headerLines = new string[]
            {
                "@all  @BidGenVariants @BidGenVariants @CSVCheck @BidGen @BidGen",
                "Feature: BidGenVariants ReadFromCSV",
                "   These tests are created by reading CSV file",
                "   .",
                "      dotnet test --filter:\"TestCategory=BidGenVariants\" --logger \"trx;logfilename=BidGenVariants.trx\"  ",
                " ",
                "Scenario Outline: BidGenVariants-ReadingFromCSV",
                " ",
                "       Given Application Set As \"Bidgen\"",
                "       Given Browser Is Open",
                "       When I Navigate To \"https://app-backend-2.azurewebsites.net/\"",
                "       Then Wait For Page \"Chat\" To Be Displayed",
                "       # Then Wait For Button \"Chat History\" To Be Displayed",
                "       Then Wait 15 Seconds",
                " "
            };

            var preabmble = "You are set to rephrase a question. Every Rephrased question should start with the word Rephased. Don't answer the question. Here is the question to Rephrase: ";

            // we loop through the ListOfFeatureFiles
            //foreach (var scenario in ListOfFeatureFiles)
            var subsetOfFeatureFiles = ListOfFeatureFiles.Take(25);
            foreach (var scenario in subsetOfFeatureFiles)
            {
                string? scenarioNumberAsString = StringValues.GetStringOfLengthFromInt(scenario.QuestionNumber, 5);
                if (scenarioNumberAsString == null) return null;
                // var newLine = "Scenario: York-002-ReadingFromCSV-" + scenarioNumberAsString;
                var newLine = "";
                headerLines = headerLines.Append(newLine).ToArray();
                if (scenario.MoreInformation != null)
                {
                    newLine = "     # # # # #" + scenario.MoreInformation;
                    headerLines = headerLines.Append(newLine).ToArray();
                }
                /*
                    #5 variants of Question: "What is WIP?"
                    #Variant 1
                    When I Enter "please rephrase the following: What is WIP?" Then Press "Enter" in Testbox "question"
                    Then Question "please rephrase the following: What is WIP?" Then Press "Enter" Is Newest Display
                    Then Wait For Button "Stop" To Not Be Displayed
                    Then Answer Different to Question "What is WIP?"
                    
                */

                newLine = "     #5 variants of question: "+scenario.Question;
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     #Variant 1";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     When I Enter \"" + preabmble + scenario.Question + "\" Then Press \"Enter\" In TextBox \"question\"";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     Then Question \"" + preabmble + scenario.Question+"\" Is Newest Display";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     Then Wait For Answer To Stop";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     Then Answer Different to Question \""+scenario.Question+"\"";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = " ";
                headerLines = headerLines.Append(newLine).ToArray();

                /*
                    When I Enter Recent Answer Then Press "Enter" In TextBox "question"
                    Then Rephrased Question Is Newest Display
                    Then Wait For Button "Stop" To Not Be Displayed
                    Then Compare Answer With Expected Answer Text "Work in Progress"
                */
                newLine = "     When I Enter Recent Answer Then Press \"Enter\" In TextBox \"question\"";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     Then Rephrased Question Is Newest Display";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = "     Then Wait For Answer To Stop";
                headerLines = headerLines.Append(newLine).ToArray();
                
                newLine = "     Then Compare Answer With Expected Answer \"" + scenario.Answer + "\"";
                headerLines = headerLines.Append(newLine).ToArray();

                newLine = " ";
                headerLines = headerLines.Append(newLine).ToArray();

                //Statements to rephrase the question a further Four Times
                for (int i = 0; i < 4; i++)
                {
                    /*                
                        #Variant X
                        When I Enter "Please Rephrase the following: " And Rephrased Question Then Press "Enter" In TextBox "question"
                        Then Question "Please Rephrase the following: " And Rephrased Question Is Newest Display
                        Then Wait For Button "Stop" To Not Be Displayed
                        Then Last Answer Different to Previous Question
                    */
                    
                    newLine = "     #Variant "+(i+2);
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = "     When I Enter \"" + preabmble + "\" And Rephrased Question Then Press \"Enter\" In TextBox \"question\"";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = "     Then Question \"" + preabmble + "\" And Rephrased Question Is Newest Display";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = "     Then Wait For Answer To Stop";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = "     Then Last Answer Different to Previous Question";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = " ";
                    headerLines = headerLines.Append(newLine).ToArray();

                    /*                
                        When I Enter Recent Answer Then Press "Enter" In TextBox "question"
                        Then Rephrased Question Is Newest Display
                        Then Wait For Button "Stop" To Not Be Displayed
                        Then Compare Answer With Expected Answer "Go to MyWorkSpace and login using your CGI credentialsClick on the drop down under 'Requests' and select 'PCB/AR'.Under the 'Request topic' dropdown, select 'PCB- Project' and under 'Request Type', select 'PCB Add/Update BIL rates' and press 'Next'.Here you can find the required template to be attached. Download the template (PC40_Add and Update Bill Rates or Markup v1) and fill out the details required and save the document. *Best practice is to make sure that the rate is updated ahead of the member submitting their timesheet and by the Friday of revenue recognition week. (If this is not completed by the revenue recognition deadline, please inform your finance team to make them aware).Next, under the 'Options' drop down, you must state whether you are submitting a BIL rate for resource(s) or updating an existing BIL rate.Then click on 'Attachments' and attach the file you just saved.Save and then submit. "
                    */

                    newLine = "     When I Enter Recent Answer Then Press \"Enter\" In TextBox \"question\"";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = "     Then Rephrased Question Is Newest Display";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = "     Then Wait For Answer To Stop";
                    headerLines = headerLines.Append(newLine).ToArray();
                    
                    newLine = "     Then Compare Answer With Expected Answer \"" + scenario.Answer + "\"";
                    headerLines = headerLines.Append(newLine).ToArray();

                    newLine = " ";
                    headerLines = headerLines.Append(newLine).ToArray();
                }

                // newLine = "     Given Browser Is Closed ";
                // headerLines = headerLines.Append(newLine).ToArray();

                // newLine = "     Then Wait 15 Seconds";
                // headerLines = headerLines.Append(newLine).ToArray(); 

                // newLine = "     Given Browser Is Open";
                // headerLines = headerLines.Append(newLine).ToArray();

                // newLine =  "    When I Navigate To \"https://app-backend-2.azurewebsites.net/\"";
                // headerLines = headerLines.Append(newLine).ToArray();
                
                // newLine = "     Then Wait For Page \"Chat\" To Be Displayed";
                // headerLines = headerLines.Append(newLine).ToArray();

                // newLine = "     # Then Wait For Button \"Chat History\" To Be Displayed";
                // headerLines = headerLines.Append(newLine).ToArray();           

                // newLine = "     Then Wait 15 Seconds";
                // headerLines = headerLines.Append(newLine).ToArray(); 
            }          
            return headerLines;
        }

        public bool CSVListToAPIFeatureFile(List<string> linesOfCSV, string featureFileDirectory, string fileName)
        {
            DebugOutput.Log($"CSVListToAPIFeatureFile {linesOfCSV.Count} {featureFileDirectory} {fileName}");
            var fullPath = FileUtils.GetCorrectDirectory(featureFileDirectory);
            DebugOutput.Log($"CSVListToAPIFeatureFile {linesOfCSV.Count} {fullPath}");
            var numberOfLinesExpected = linesOfCSV.Count;
            var listOfFeatureFile = new List<FeatureFile> ();
            foreach (var line in linesOfCSV)
            {
                var FeatureFileModel = new FeatureFile();
                var returnFromLine = ParseCsvLine(line);
                DebugOutput.Log($"That gives us {returnFromLine.Count} fields!");
                int counter = 0;
                foreach (var field in returnFromLine)
                {
                    DebugOutput.Log($"{field}");
                    if (counter == 0)
                    {
                        int questionNumber = StringValues.GetInt32FromString(field) ?? -1;
                        if (questionNumber == -1) return Failure("Failed to convert the first column to a number!");
                        FeatureFileModel.QuestionNumber = questionNumber;
                    }
                    if (counter == 1)
                    {
                        FeatureFileModel.Question = field.Replace("\"", "");
                    }
                    if (counter == 2)
                    {
                        FeatureFileModel.Answer = field.Replace("\"", "");
                    }
                    if (counter == 3)
                    {
                        FeatureFileModel.MoreInformation = field.Replace("\"", "");
                    }
                    counter ++;
                }
                listOfFeatureFile.Add(FeatureFileModel);
                DebugOutput.Log($"empty space!");
            }
            var brokenUpDirAndFile = StringValues.BreakUpByDelimitedToList(fileName, "/");
            var finalFileName = brokenUpDirAndFile[brokenUpDirAndFile.Count - 1];
            var numberOfModels = listOfFeatureFile.Count();
            if (numberOfLinesExpected != numberOfModels) return Failure("We do not match the number of lines to the number of models!");
            var lines = CreateTheAPIFeatureFile(listOfFeatureFile, finalFileName);
            if (lines == null) return Failure("Failed to populate the text for file!");
            return FileUtils.OSFileCreationAndPopulation(fullPath, finalFileName + ".feature", lines);
        }

        
        private StringBuilder CreateTheAPIFeatureFile(List<FeatureFile> ListOfFeatureFiles, string fileName = "")
        {
            DebugOutput.Log($"CreateTheAPIFeatureFile {ListOfFeatureFiles.Count}");
            List<string> AllHeaderLines = new();
            var headerOne = $"@all  @{fileName} @BidGen432K";
            AllHeaderLines.Add(headerOne);
            var headerTwo = $"Feature: {fileName} ReadFromCSV";
            AllHeaderLines.Add(headerTwo);
            var headerThree = $"    These tests are created by reading CSV file";
            AllHeaderLines.Add(headerThree);
            var headerFour = $"     .";
            AllHeaderLines.Add(headerFour);
            var headerFive = $"          dotnet test --filter:\"TestCategory={fileName}\" --logger \"trx;logfilename={fileName}.trx\"  ";
            AllHeaderLines.Add(headerFive);
            var headerSix = " ";
            AllHeaderLines.Add(headerSix);
            var headerSeven = $"Scenario Outline: {fileName}-ReadingFromCSV";
            AllHeaderLines.Add(headerSeven);
            var headerEight = " ";
            AllHeaderLines.Add(headerEight);
            var headerTen = " ";
            AllHeaderLines.Add(headerTen);

            StringBuilder headerLines = new StringBuilder();
            foreach (string s in AllHeaderLines)
            {
                headerLines.AppendLine(s);
            }

            // var preamble = "Every Rephrased question should start with the word Rephrased:. Here is the question to Rephrase: ";

            // We loop through the ListOfFeatureFiles
            foreach (var scenario in ListOfFeatureFiles)
            {
                string? scenarioNumberAsString = StringValues.GetStringOfLengthFromInt(scenario.QuestionNumber, 5);
                var newLine = "";
                headerLines.AppendLine(newLine);

                if (scenario.MoreInformation != null)
                {
                    newLine = "     # # # # #" + scenario.MoreInformation;
                    headerLines.AppendLine(newLine);
                }

                newLine = "     When I Ask Azure Open API \"" + scenario.Question + "\"";
                headerLines.AppendLine(newLine);

                newLine = "     Then Compare API Answer With Expected Answer \"" + scenario.Answer + "\"";
                headerLines.AppendLine(newLine);

                newLine = " ";
                headerLines.AppendLine(newLine);

            //     newLine = "     #5 variants of question: " + scenario.Question;
            //     headerLines.AppendLine(newLine);

            //     newLine = " ";
            //     headerLines.AppendLine(newLine);

            //     newLine = "     #Variant 1";
            //     headerLines.AppendLine(newLine);

            //     newLine = "     When I Ask Azure Open API \"" + preamble + scenario.Question + "\"";
            //     headerLines.AppendLine(newLine);

            //     newLine = "     Then API Answer Different To API Question \"" + scenario.Question + "\"";
            //     headerLines.AppendLine(newLine);

            //     newLine = " ";
            //     headerLines.AppendLine(newLine);

            //     newLine = "     When I Ask Azure Open API Recent Answer";
            //     headerLines.AppendLine(newLine);

            //     newLine = "     Then Compare API Answer With Expected Answer \"" + scenario.Answer + "\"";
            //     headerLines.AppendLine(newLine);

            //     newLine = " ";
            //     headerLines.AppendLine(newLine);

            //     for (int i = 0; i < 4; i++)
            //     {
            //         newLine = "     #Variant " + (i + 2);
            //         headerLines.AppendLine(newLine);

            //         newLine = "     When I Ask Azure Open API \"" + preamble + "\" For Recent Answer";
            //         headerLines.AppendLine(newLine);

            //         newLine = "     Then API Answer Different To API Question \"" + scenario.Question + "\"";
            //         headerLines.AppendLine(newLine);

            //         newLine = " ";
            //         headerLines.AppendLine(newLine);

            //         newLine = "     When I Ask Azure Open API Recent Answer";
            //         headerLines.AppendLine(newLine);

            //         newLine = "     Then Compare API Answer With Expected Answer \"" + scenario.Answer + "\"";
            //         headerLines.AppendLine(newLine);

            //         newLine = " ";
            //         headerLines.AppendLine(newLine);
            //     }
            }

            return headerLines;
        }

        public bool CSVListToAPIVariantFeatureFile(List<string> linesOfCSV, string featureFileDirectory, string fileName)
        {
            DebugOutput.Log($"CSVListToAPIFeatureFile {linesOfCSV.Count} {featureFileDirectory} {fileName}");
            var fullPath = FileUtils.GetCorrectDirectory(featureFileDirectory);
            DebugOutput.Log($"CSVListToAPIFeatureFile {linesOfCSV.Count} {fullPath}");
            var numberOfLinesExpected = linesOfCSV.Count;
            var listOfFeatureFile = new List<FeatureFile> ();
            foreach (var line in linesOfCSV)
            {
                var FeatureFileModel = new FeatureFile();
                var returnFromLine = ParseCsvLine(line);
                DebugOutput.Log($"That gives us {returnFromLine.Count} fields!");
                int counter = 0;
                foreach (var field in returnFromLine)
                {
                    DebugOutput.Log($"{field}");
                    if (counter == 0)
                    {
                        int questionNumber = StringValues.GetInt32FromString(field) ?? -1;
                        if (questionNumber == -1) return Failure("Failed to convert the first column to a number!");
                        FeatureFileModel.QuestionNumber = questionNumber;
                    }
                    if (counter == 1)
                    {
                        FeatureFileModel.Question = field.Replace("\"", "");
                    }
                    if (counter == 2)
                    {
                        FeatureFileModel.Answer = field.Replace("\"", "");
                    }
                    if (counter == 3)
                    {
                        FeatureFileModel.MoreInformation = field.Replace("\"", "");
                    }
                    counter ++;
                }
                listOfFeatureFile.Add(FeatureFileModel);
                DebugOutput.Log($"empty space!");
            }
            var brokenUpDirAndFile = StringValues.BreakUpByDelimitedToList(fileName, "/");
            var finalFileName = brokenUpDirAndFile[brokenUpDirAndFile.Count - 1];
            var numberOfModels = listOfFeatureFile.Count();
            if (numberOfLinesExpected != numberOfModels) return Failure("We do not match the number of lines to the number of models!");
            var lines = CreateTheAPIVariantFeatureFile(listOfFeatureFile, finalFileName);
            if (lines == null) return Failure("Failed to populate the text for file!");
            return FileUtils.OSFileCreationAndPopulation(fullPath, finalFileName + ".feature", lines);
        }

        
        private StringBuilder CreateTheAPIVariantFeatureFile(List<FeatureFile> ListOfFeatureFiles, string fileName = "")
        {
            DebugOutput.Log($"CreateTheAPIFeatureFile {ListOfFeatureFiles.Count}");
            List<string> AllHeaderLines = new();
            var headerOne = $"@all  @{fileName} @BidGen432K";
            AllHeaderLines.Add(headerOne);
            var headerTwo = $"Feature: {fileName} ReadFromCSV";
            AllHeaderLines.Add(headerTwo);
            var headerThree = $"    These tests are created by reading CSV file";
            AllHeaderLines.Add(headerThree);
            var headerFour = $"     .";
            AllHeaderLines.Add(headerFour);
            var headerFive = $"          dotnet test --filter:\"TestCategory={fileName}\" --logger \"trx;logfilename={fileName}.trx\"  ";
            AllHeaderLines.Add(headerFive);
            var headerSix = " ";
            AllHeaderLines.Add(headerSix);
            var headerSeven = $"Scenario Outline: {fileName}-ReadingFromCSV";
            AllHeaderLines.Add(headerSeven);
            var headerEight = " ";
            AllHeaderLines.Add(headerEight);
            var headerTen = " ";
            AllHeaderLines.Add(headerTen);

            StringBuilder headerLines = new StringBuilder();
            foreach (string s in AllHeaderLines)
            {
                headerLines.AppendLine(s);
            }

            var preamble = "Rephrase the question. Every Rephrased question should start with the word Rephrased:. Ensure that the rephrased question is distinctly different from the original and previous versions. Here is the question to rephrase: ";

            // We loop through the ListOfFeatureFiles
            foreach (var scenario in ListOfFeatureFiles)
            {
                string? scenarioNumberAsString = StringValues.GetStringOfLengthFromInt(scenario.QuestionNumber, 5);
                var newLine = "";
                headerLines.AppendLine(newLine);

                if (scenario.MoreInformation != null)
                {
                    newLine = "     # # # # #" + scenario.MoreInformation;
                    headerLines.AppendLine(newLine);
                }

                newLine = "     When I Ask Azure Open API \"" + scenario.Question + "\"";
                headerLines.AppendLine(newLine);

                newLine = "     #Then Compare API Answer With Expected Answer \"" + scenario.Answer + "\"";
                headerLines.AppendLine(newLine);

                newLine = " ";
                headerLines.AppendLine(newLine);

                newLine = "     #5 variants of question: " + scenario.Question;
                headerLines.AppendLine(newLine);

                newLine = " ";
                headerLines.AppendLine(newLine);

                newLine = "     #Variant 1";
                headerLines.AppendLine(newLine);

                newLine = "     When I Ask Azure Open API \"" + preamble + scenario.Question + "\"";
                headerLines.AppendLine(newLine);

                newLine = "     Then API Answer Different To API Question \"" + scenario.Question + "\"";
                headerLines.AppendLine(newLine);

                newLine = " ";
                headerLines.AppendLine(newLine);

                newLine = "     When I Ask Azure Open API Recent Answer";
                headerLines.AppendLine(newLine);

                newLine = "     Then Compare API Answer With Expected Answer \"" + scenario.Answer + "\"";
                headerLines.AppendLine(newLine);

                newLine = " ";
                headerLines.AppendLine(newLine);

                for (int i = 0; i < 4; i++)
                {
                    newLine = "     #Variant " + (i + 2);
                    headerLines.AppendLine(newLine);

                    newLine = "     When I Ask Azure Open API \"" + preamble + "\" For Recent Answer";
                    headerLines.AppendLine(newLine);

                    newLine = "     Then API Answer Different To API Question \"" + scenario.Question + "\"";
                    headerLines.AppendLine(newLine);

                    newLine = " ";
                    headerLines.AppendLine(newLine);

                    newLine = "     When I Ask Azure Open API Recent Answer";
                    headerLines.AppendLine(newLine);

                    newLine = "     Then Compare API Answer With Expected Answer \"" + scenario.Answer + "\"";
                    headerLines.AppendLine(newLine);

                    newLine = " ";
                    headerLines.AppendLine(newLine);
                }
            }

            return headerLines;
        }

    }
}