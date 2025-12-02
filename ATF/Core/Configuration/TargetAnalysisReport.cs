
using Core.FileIO;
using Core.Logging;
using Core.NLM;
using Core.Transformations;
using Newtonsoft.Json;

namespace Core.Configuration
{
    public class TargetAnalysisReport
    {
        public static List<NLM.AnalysedAnswer>? AnalysedReports { get; private set;}

        public static readonly string WriteComparisonExtension = "-demo301test";

        public static void NewAnalysisReport()
        {
            DebugOutput.Log($"NewAnalysisReport");
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed)
            {
                var newAnalysisReport = new List<NLM.AnalysedAnswer>();
                AnalysedReports = newAnalysisReport;
            }
        }
        
        public static string? GetAnalysedReportsJson()
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(AnalysedReports);
            return jsonString;
        }

        public static void CloseJsonForPython()
        {
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsOut)
            {
                DebugOutput.Log($"CloseJsonForPython");
                DebugOutput.Log($"We creating JSON FILE");
                var chatbotModelsToBeAnalyised = Core.NLM.ListOfChatbotModels.ChatbotModels;
                if (chatbotModelsToBeAnalyised == null)
                {
                    DebugOutput.Log($"NOTHING TO REPORT UPON!");
                    return;
                }
                DebugOutput.Log($"CLOSING DOWN CloseJsonForPython WITH {chatbotModelsToBeAnalyised.Count} Model to be analysied!");
                DebugOutput.Log($"THIS IS THE CloseJsonForPython");
                var jsonString = Core.NLM.ChatbotModelModelWorking.GetJsonFromModels(chatbotModelsToBeAnalyised);
                if (jsonString == null)
                {
                    DebugOutput.Log($"FAIL TO CREATE THE JSON! we have a model, but no json!");
                    return;
                }
                DebugOutput.Log($"THIS IS IT!");
                DebugOutput.Log(jsonString);
                var homedir = FileUtils.GetCorrectDirectory("/AppXAPI/APIOutFiles/");
                DebugOutput.Log($"HOME DIR = {homedir}");
                if (!FileUtils.OSDirectoryCheck(homedir)) FileUtils.OSDirectoryCreation(homedir);
                if (FileUtils.OSFileCreationAndPopulation(homedir,"JsonFromAutomatedAI.json",jsonString)) return ;
            }
        }

        public static void CloseAnswerComparisonReport(string key = "")
        {           
            if (!TargetAIChatBotConfiguration.Configuration.AnswerIsOut)
            {
                return;
            }
            if (!TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed) return;
            if (!TargetAIChatBotConfiguration.Configuration.AnswerIsCompared) return;
            DebugOutput.Log($"CloseAnswerComparisonReport "); 
            if (AnalysedReports == null) return;
            WriteAnswerComparisonJsonToFile(AnalysedReports, "", key);
            if (TargetAIChatBotConfiguration.Configuration.AnswerHistoricComparison) ReadAllJsonFilesAndAppend();
        }

        public static void CloseAnalysisReport()
        {
            if (!TargetAIChatBotConfiguration.Configuration.AnswerIsOut)
            {
                return;
            }
            // if (TargetConfiguration.Configuration.AnswerNLP.ToLower() != "google" || TargetConfiguration.Configuration.AnswerNLP.ToLower() != "nltk")
            // {
            //     DebugOutput.Log($"We have no NLP assigned - so no way to do any Analysis!");
            //     return;
            // }
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed)
            {                
                if (Core.NLM.ListOfChatbotModels.ChatbotModels != null) DebugOutput.Log($"CloseAnalysisReport {TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower()} with {Core.NLM.ListOfChatbotModels.ChatbotModels.Count()} models!");
                else DebugOutput.Log($"CloseAnalysisReport {TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower()} NULL MODEL");
                var AnalysedReports = Core.NLM.ListOfChatbotModels.ChatbotModels;
                if (AnalysedReports == null) return;
                DebugOutput.Log($"CLOSING DOWN WITH {AnalysedReports.Count} Model to be analysied!");
                if (AnalysedReports != null)
                {                    
                    WriteAnalysisJsonToFile();
                }
            }
        }

        public static bool WriteAnswerComparisonJsonToFile(List<AnalysedAnswer>? analysedAnswers, string fileNameAndLocation = "", string key = "")
        {
            if (fileNameAndLocation == "") fileNameAndLocation = @"\AppSpecFlow\TestResults\";
            DebugOutput.Log($"WriteAnswerComparisonJsonToFile {fileNameAndLocation} {key}");

            if (!TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed) return true;
            if (analysedAnswers == null) return false;

            var jsonString = GetAnalysedReportsJson();
            if (jsonString == null) return false;

            DebugOutput.Log("json = " + jsonString);

            // Get the current epoch
            var directory = fileNameAndLocation + EPOCHControl.Epoch;
            if (!FileUtils.DirectoryCheck(directory)) FileUtils.DirectoryCreation(directory);

            string JsonName = "";
            if (key == "") JsonName = EPOCHControl.Epoch + WriteComparisonExtension;
            else JsonName = EPOCHControl.Epoch + "-" + key;

            // Create the API JSON file
            JsonValues.CreateAPIJsonFile(jsonString, directory, JsonName);

            // Define the status report file location
            var statusReportFileName = "Comparison.html";
            var statusReportFilePath = Path.Combine(directory, statusReportFileName);

            // Generate the HTML comparison report
            var status = HTML.UseHTML.CreateHTMLComparisonReport(analysedAnswers);

            // Populate the status report file
            if (!FileUtils.FilePopulate(statusReportFilePath, status)) return false;

            //Create CSV Report
            var FullFileCreationDir = FileUtils.GetCorrectDirectory(statusReportFilePath);
            DateTime creationTime = FileUtils.OSGetFileCreationTime(FullFileCreationDir);
            DebugOutput.Log($"File Pat CSVFile: " + statusReportFilePath);
            string dateTimeString = creationTime.ToString("yyyy-MM-ddTHH-mm-ss");
            var fileNameCSV = dateTimeString + "-HistoricData" + ".csv";
            //var fileNameCSV = EPOCHControl.Epoch + "-HistoricData" + ".csv";
            var fileNameAndLocationCSV = Path.Combine(directory, fileNameCSV);
            DebugOutput.Log($"Output File CSV: {fileNameAndLocationCSV}");
            ConvertJsonToCsv(analysedAnswers, fileNameAndLocationCSV);

            return true;
        }

        public static bool ReadAllJsonFilesAndAppend()
        {
            string folderPath = FileUtils.GetCorrectDirectory("/AppSpecFlow/TestResults");
            //string folderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestResults"));
            DebugOutput.Log($"Folder Path: {folderPath}");
            var jsonFiles = FileUtils.OSGetListOfFilesInAllDirectoriesOfTypeContainingWord(folderPath, "json", WriteComparisonExtension);
            DebugOutput.Log($"Found Files: {jsonFiles.Count}");
            string outputFile = FileUtils.GetCorrectDirectory("/AppXAPI/APIOutFiles/HistoricComparison.json");
            //Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AppXAPI", "APIOutFiles", "WriteComparison.json"));
            DebugOutput.Log($"Output File Path: {outputFile}");
            //FileUtils.OSFileCreation(outputFile);
            FileUtils.OSReplaceFullTextInFile(outputFile, string.Empty);

            //File.WriteAllText(outputFile, string.Empty);
            
            var existingData = new List<List<AnalysedAnswer>>();
            foreach (var jsonFile in jsonFiles)
            {
                var json = FileUtils.OSGetAllTextInFile(jsonFile);
                var result = JsonConvert.DeserializeObject<List<AnalysedAnswer>>(json);
                if (result != null)
                {
                    // Get the file creation time
                    var creationTime = FileUtils.OSGetFileCreationTime(jsonFile);

                    // Add the creation time to each item in the result
                    foreach (var item in result)
                    {
                        item.TimeOfTest = creationTime;
                    }

                    existingData.Add(result);
                }
            }
            var outputJson = JsonConvert.SerializeObject(existingData, Formatting.Indented);
            File.WriteAllText(outputFile, outputJson);


            // string outputFileLocation = "/AppXAPI/APIOutFiles/";
            // DebugOutput.Log($"Output File Path: {outputFileLocation}");
            // var fileNameAndLocationStatusReport = outputFileLocation + "HistoricComparisonReport" + ".html";
            // DebugOutput.Log($"Output File Path: {fileNameAndLocationStatusReport}");
            // var report = HTML.UseHTML.CreateHTMLHistoricComparison(existingData);
            // if (!FileUtils.FilePopulate(fileNameAndLocationStatusReport, report))return false;

            //CSV Histoic Creation 
            string outputCSVFileLocation = "/AppXAPI/APIOutFiles/";
            var fileNameAndLocationCSV = outputCSVFileLocation + "HistoricData" + ".csv";
            DebugOutput.Log($"Output File CSV: {fileNameAndLocationCSV}");
            ConverHistoricJsonToHistoricCsv(existingData, fileNameAndLocationCSV);

            return true;
        }
        public static bool ConverHistoricJsonToHistoricCsv(List<List<AnalysedAnswer>> data, string outputPath)
        {
            try
            {
                string directoryOutputPath = FileUtils.GetCorrectDirectory(outputPath);
                FileUtils.OSReplaceFullTextInFile(directoryOutputPath, string.Empty);
                FileUtils.FileLinePopulate(outputPath, "QuestionNumber,Question,ExpectedAnswer,AIAnswer,Measurements_CosineSimilarity,Measurements_LevenshteinDistance,Measurements_JaccardSimilarity,AISentiment_Score,AISentiment_Magnitude,ExpectedSentiment_Score,ExpectedSentiment_Magnitude");

                foreach (var dataList in data)
                {
                    foreach (var answer in dataList)
                    {
                        if (answer.Question != null && answer.ExpectedAnswer != null && answer.AIAnswer != null) FileUtils.FileLinePopulate(outputPath, $"{answer.QuestionNumber},{EscapeCsvField(answer.Question)},{EscapeCsvField(answer.ExpectedAnswer)},{EscapeCsvField(answer.AIAnswer)},{answer.Measurements?.CosineSimilarity},{answer.Measurements?.LevenshteinDistance},{answer.Measurements?.JaccardSimilarity},{answer.AISentiment?.Score},{answer.AISentiment?.Magnitude},{answer.ExpectedSentiment?.Score},{answer.ExpectedSentiment?.Magnitude}");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV: {ex.Message}");
                return false;
            }
        }

        public static bool ConvertJsonToCsv(List<AnalysedAnswer> data, string outputPath)
            {
                try
                {
                    string directoryOutputPath = FileUtils.GetCorrectDirectory(outputPath);
                    FileUtils.OSReplaceFullTextInFile(outputPath, string.Empty);
                    FileUtils.FileLinePopulate(outputPath, "QuestionNumber,Question,ExpectedAnswer,AIAnswer,Measurements_CosineSimilarity,Measurements_LevenshteinDistance,Measurements_JaccardSimilarity,AISentiment_Score,AISentiment_Magnitude,ExpectedSentiment_Score,ExpectedSentiment_Magnitude");

                    foreach (var answer in data)
                    {
                        if (answer.Question != null && answer.ExpectedAnswer != null && answer.AIAnswer != null)
                        {
                            FileUtils.FileLinePopulate(outputPath, $"{answer.QuestionNumber},{EscapeCsvField(answer.Question)},{EscapeCsvField(answer.ExpectedAnswer)},{EscapeCsvField(answer.AIAnswer)},{answer.Measurements?.CosineSimilarity},{answer.Measurements?.LevenshteinDistance},{answer.Measurements?.JaccardSimilarity},{answer.AISentiment?.Score},{answer.AISentiment?.Magnitude},{answer.ExpectedSentiment?.Score},{answer.ExpectedSentiment?.Magnitude}");
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to CSV: {ex.Message}");
                    return false;
                }
            }
        public static string EscapeCsvField(string field)
        {
        // If the field contains commas, double quotes, or newlines, surround it with double quotes and escape any existing double quotes
        if (field.Contains(",") || field.Contains("\"") || field.Contains(Environment.NewLine))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
        }


        public static List<AnalysedAnswer>? GetAnalysedAnswersFromGoogle()
        {
            if (Core.NLM.ListOfChatbotModels.ChatbotModels == null) return null;
            List<AnalysedAnswer> listOfAnalysedAsnwers = new();
            int questionNumber = 1;            

            foreach (var model in Core.NLM.ListOfChatbotModels.ChatbotModels)
            {
                if (model.question == null || model.aiAnswer == null) return null;

                var newAnalysedAnswer = new AnalysedAnswer();

                newAnalysedAnswer.Question = model.question;
                newAnalysedAnswer.QuestionNumber = EPOCHControl.GetCurrentDateTimeInEPOCH();
                newAnalysedAnswer.QuestionEntities = Core.NLM.Analyse.GetEntities(questionNumber, model.question);

                newAnalysedAnswer.AIAnswer = model.aiAnswer;
                newAnalysedAnswer.AIEntities = Core.NLM.Analyse.GetEntities(questionNumber, model.aiAnswer);
                newAnalysedAnswer.AISentiment = Core.NLM.Analyse.GetSentiment(questionNumber, model.aiAnswer);
                newAnalysedAnswer.AICategories = Core.NLM.Analyse.GetCategories(questionNumber, model.aiAnswer);
                newAnalysedAnswer.AISentances = Core.NLM.Analyse.GetSentances(questionNumber, model.aiAnswer);
                if (newAnalysedAnswer.AISentances != null)
                {
                    var listOfSentanceSentimate = new List<Core.NLM.Sentiment>();
                    foreach (var sentance in newAnalysedAnswer.AISentances)
                    {
                        var sentanceSentiment = Core.NLM.Analyse.GetSentiment(questionNumber, sentance);
                        if (sentanceSentiment != null) listOfSentanceSentimate.Add(sentanceSentiment);
                    }
                    newAnalysedAnswer.AISentanceSentiment = listOfSentanceSentimate;
                }
                if (newAnalysedAnswer.AISentances != null)
                {
                    var listOfSentanceCategories = new List<List<Core.NLM.Category>>();
                    foreach (var sentance in newAnalysedAnswer.AISentances)
                    {
                        var sentanceSentiment = Core.NLM.Analyse.GetCategories(questionNumber, sentance);
                        if (sentanceSentiment != null) listOfSentanceCategories.Add(sentanceSentiment);
                    }
                    newAnalysedAnswer.AISentanceCategories = listOfSentanceCategories;
                }

                newAnalysedAnswer.ExpectedAnswer = model.expectedAnswer;
                if (model.expectedAnswer != null) newAnalysedAnswer.ExpectedSentances = Core.NLM.Analyse.GetSentances(questionNumber, model.expectedAnswer);
                if (model.expectedAnswer != null) newAnalysedAnswer.ExpectedEntities = Core.NLM.Analyse.GetEntities(questionNumber, model.expectedAnswer);
                if (model.expectedAnswer != null) newAnalysedAnswer.ExpectedSentiment = Core.NLM.Analyse.GetSentiment(questionNumber, model.expectedAnswer);
                if (model.expectedAnswer != null) newAnalysedAnswer.ExpectedCategories = Core.NLM.Analyse.GetCategories(questionNumber, model.expectedAnswer);
                if (newAnalysedAnswer.ExpectedSentances != null)
                {
                    var listOfSentanceSentimate = new List<Core.NLM.Sentiment>();
                    foreach (var sentance in newAnalysedAnswer.ExpectedSentances)
                    {
                        var sentanceSentiment = Core.NLM.Analyse.GetSentiment(questionNumber, sentance);
                        if (sentanceSentiment != null) listOfSentanceSentimate.Add(sentanceSentiment);
                    }
                    newAnalysedAnswer.ExpectedSentanceSentiment = listOfSentanceSentimate;
                }
                if (newAnalysedAnswer.ExpectedSentances != null)
                {
                    var listOfSentanceCategories = new List<List<Core.NLM.Category>>();
                    foreach (var sentance in newAnalysedAnswer.ExpectedSentances)
                    {
                        var sentanceSentiment = Core.NLM.Analyse.GetCategories(questionNumber, sentance);
                        if (sentanceSentiment != null) listOfSentanceCategories.Add(sentanceSentiment);
                    }
                    newAnalysedAnswer.ExpectedSentanceCategories = listOfSentanceCategories;
                }
                var newMeasurement = new CompareMeasurements();
                if (newAnalysedAnswer.ExpectedAnswer != null)
                {
                    newMeasurement.CosineSimilarity = Analyse.GetCosineSimilarity(newAnalysedAnswer.AIAnswer, newAnalysedAnswer.ExpectedAnswer);
                    newMeasurement.LevenshteinDistance = Analyse.GetLevenshteinDistance(newAnalysedAnswer.AIAnswer, newAnalysedAnswer.ExpectedAnswer);
                    newMeasurement.JaccardSimilarity = Analyse.GetJaccardSimilarity(newAnalysedAnswer.AIAnswer, newAnalysedAnswer.ExpectedAnswer);
                    newAnalysedAnswer.Measurements = newMeasurement;
                } 

                listOfAnalysedAsnwers.Add(newAnalysedAnswer);
            }
            return listOfAnalysedAsnwers;
        }


        public static List<AnalysedAnswer>? GetAnalysedAnswers()
        {
            DebugOutput.Log($"GetAnalysedAnswers");            
            List<AnalysedAnswer>? listOfAnalysedAsnwers = new();
            var tool = TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower();
            if (Core.NLM.ListOfChatbotModels.ChatbotModels == null) return null;
            DebugOutput.Log($"THE TOOL WE WILL BE USING IS {tool}");
            switch(tool)
            {
                case "nltk":
                {
                    DebugOutput.Log($"THIS IS NLTK");
                    CmdUtil.ExecuteCommand("python ./ExtensionPython/NLTK/createJSON.py");
                    var currentFile = FileUtils.GetCorrectDirectory("/AppxAPI/APIOutfiles/") + "comparison_results.json";
                    if (!FileUtils.OSFileCheck(currentFile))
                    {
                        DebugOutput.Log($"Can not find Json @ {currentFile}");
                        return null;
                    }
                    DebugOutput.Log($"FOUND json file from NLTP @ {currentFile}");
                    var jsonString = FileUtils.OSGetFileContentsAsString(currentFile);
                    if (jsonString == null) return null;
                    DebugOutput.Log($"JSON FROM NLTK - {jsonString}");
                    //listOfAnalysedAsnwers = GetAnalysedAnswersFromNLTK(json);
                    var analysisModel = Core.NLTK.NLTKAnalysisWorking.ConvertJsonToModel(jsonString);
                    if (analysisModel == null) return null;
                    listOfAnalysedAsnwers = Core.NLM.Analyse.ConvertNLTKtoAnalysedAnswer(analysisModel);
                    break;
                }
                case "google":
                {
                    DebugOutput.Log($"THIS IS GOOGLE NLP");
                    listOfAnalysedAsnwers = GetAnalysedAnswersFromGoogle();
                    break;
                }
                default:
                {
                    DebugOutput.Log($"WHAT IS THIS!?");
                    return null;
                }
            }

            return listOfAnalysedAsnwers;
        }
        
        public static bool WriteAnalysisJsonToFile(string fileNameAndLocation = @"\AppSpecFlow\TestResults\")
        {
            if(!TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed) return true;

            if (Core.NLM.ListOfChatbotModels.ChatbotModels == null) return false;
            DebugOutput.Log($"We are using {TargetAIChatBotConfiguration.Configuration.AnswerNLP} to analise our questions!");

            var listOfAnalysedAsnwers = GetAnalysedAnswers();
            if (listOfAnalysedAsnwers == null)
            {
                DebugOutput.Log($"Nothing or No way to analise!");
                return false;
            }

            DebugOutput.Log($"This gives us {listOfAnalysedAsnwers.Count()} questions and answers analysed!");

            TargetAnalysisReport.UpdateAnalysisReport(listOfAnalysedAsnwers);
            DebugOutput.Log($"---------------------------------------------------------------------------------------------");

            if (AnalysedReports == null) return false;
            var jsonString = GetAnalysedReportsJson();
            if (jsonString == null) return false;
            DebugOutput.Log(jsonString);
            JsonValues.CreateAPIJsonFile(jsonString, "WriteAnalysis");
            var directory = fileNameAndLocation + EPOCHControl.Epoch;
            if (!FileUtils.DirectoryCheck(directory)) FileUtils.DirectoryCreation(directory);
            var fileNameAndLocationStatusReport = directory + "\\" + "Analysis" + ".html";
            var report = HTML.UseHTML.CreateHTMLAnalysisReport(AnalysedReports);
            if (!FileUtils.FilePopulate(fileNameAndLocationStatusReport, report)) return false;
            return true;

        }

        public static void UpdateAnalysisReport(List<NLM.AnalysedAnswer> analysedAnswer)
        {
            DebugOutput.Log($"UpdateAnalysisReport");
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed)
            {
                AnalysedReports = analysedAnswer;
            }
        }


        
    }
}
