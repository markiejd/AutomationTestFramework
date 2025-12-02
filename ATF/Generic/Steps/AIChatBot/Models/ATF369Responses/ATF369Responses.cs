

using System.Diagnostics;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Generic.Steps.AIChatBots.QandAResponse
{
    // QandAResponse myDeserializedClass = JsonConvert.DeserializeObject<QandAResponse>(myJsonResponse);
    public class QandAResponse
    {
        public int? MessageCode { get; set; }
        public string? Message { get; set; }
        public string? FilePath { get; set; }
        public int? TotalEntries { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? ClosestQuestion { get; set; }
        public string? ClosestAnswer { get; set; }
        public double? SimilarityScore { get; set; }
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("NLTK Measurement")]
        public NLTKMeasurement? NLTKMeasurement { get; set; }
    }

    public class ComparisonResults
    {
        public double? CosineSimilarity { get; set; }
        public double? ClosestAnswerSentiment { get; set; }
        public double? ClosestMagnitude { get; set; }
        public double? CurrentAnswerSentiment { get; set; }
        public double? CurrentMagnitude { get; set; }
        public double? SentimentDifference { get; set; }
        public int? LevenshteinDistance { get; set; }
        public double? JaccardSimilarity { get; set; }
    }

    public class NLTKMeasurement
    {
        public List<string>? CurrentQuestionEntities { get; set; }
        public List<string>? CurrentQuestionEntitiesType { get; set; }
        public List<string>? CurrentAnswerEntities { get; set; }
        public List<string>? CurrentAnswerEntitiesType { get; set; }
        public List<string>? CurrentAnswerCategory { get; set; }
        public double? CurrentAnswerCategoryScore { get; set; }
        public List<string>? ClosestQuestionEntities { get; set; }
        public List<string>? ClosestQuestionEntitiesType { get; set; }
        public List<string>? ClosestAnswerEntities { get; set; }
        public List<string>? ClosestAnswerEntitiesType { get; set; }
        public List<string>? ClosestAnswerCategory { get; set; }
        public double? ClosestAnswerCategoryScore { get; set; }
        public ComparisonResults? ComparisonResults { get; set; }
        public SentenceResults? SentenceResults { get; set; }
    }

    public class SentenceResults
    {
        public List<string>? CurrentAnswerSentences { get; set; }
        public List<double>? CurrentAnswerSentimentsSentiment { get; set; }
        public List<string>? CurrentAnswerSentencesCategory { get; set; }
        public List<string>? ClosestAnswerSentences { get; set; }
        public List<double>? ClosestAnswerSentencesSentiment { get; set; }
        public List<string>? ClosestAnswerSentencesCategory { get; set; }
    }


    public class QandAResponseUsing
    {
        public static QandAResponse? CreateModel(string message, string filePath, int totalEntries, string question, string answer, string closestQuestion, string closestAnswer, double similarityScore, NLTKMeasurement nltkMeasurement)
        {
            var newQandAResponse = new QandAResponse();
            newQandAResponse.Message = message;
            newQandAResponse.FilePath = filePath;
            newQandAResponse.TotalEntries = totalEntries;
            newQandAResponse.Question = question;
            newQandAResponse.Answer = answer;
            newQandAResponse.ClosestQuestion = closestQuestion;
            newQandAResponse.ClosestAnswer = closestAnswer;
            newQandAResponse.SimilarityScore = similarityScore;
            newQandAResponse.NLTKMeasurement = nltkMeasurement;
            return newQandAResponse;
        }

        public static List<QandAResponse>? GetModelsFromJson(string? json)
        {
            var models = new List<QandAResponse>();
            if (json == null) return models;
            try
            {
                models = JsonConvert.DeserializeObject<List<QandAResponse>>(json);
                return models;
            }
            catch
            {
                return null;
            }
        }

        public static List<QandAResponse>? OrderModels(List<QandAResponse> models)
        {
            if (models == null) return null;
            return models.OrderBy(x => x.Question).ThenBy(x => x.ClosestAnswer).ToList();
        }

        public static List<QandAResponse>? GetMultipleModelsFromModelsByQuestion(List<QandAResponse> models, string question, int howMany = 0, bool ignoreFirstQuestion = false)
        {
            if (models == null) return null;
            var modelsToReturn = new List<QandAResponse>();
            int counter = 0;
            foreach (var model in models)
            {
                if (model.Question != null)
                {
                    if (model.Question.ToLower().Trim() == question.ToLower().Trim())
                    {
                        if (ignoreFirstQuestion)
                        {
                            if (counter == 0)
                            {
                                counter++;
                                continue;
                            }
                        }
                        modelsToReturn.Add(model);
                    }
                }
            }
            if (howMany > 0)
            {
                if (modelsToReturn.Count > howMany)
                {
                    // Calculate the number of elements to skip to get the last 10
                    int skip = Math.Max(0, modelsToReturn.Count - howMany);
                    // Use Skip to ignore the first 'skip' elements, then take the remaining ones
                    var lastX = modelsToReturn.Skip(skip).ToList();
                    return lastX;
                }
            }
            return modelsToReturn;
        }

        public static QandAResponse? GetSingleModelFromModelsByQuestion(List<QandAResponse> models, string question)
        {
            if (models == null) return null;
            foreach (var model in models)
            {
                if (model.Question != null)
                {
                    if (model.Question.ToLower().Trim() == question.ToLower().Trim())
                    {
                        return model;
                    }
                }
            }
            return null;
        }

        public static string? GetJsonFromModels(List<QandAResponse> models)
        {
            if (models == null) return null;
            var json = JsonConvert.SerializeObject(models, Formatting.Indented);
            return json;    
        }

        public static string? GetJsonFromModelsNoFormatting(List<QandAResponse>? models)
        {
            if (models == null) return null;
            var json = JsonConvert.SerializeObject(models, Formatting.None);
            return json;    
        }



        public static QandAResponse? GetModelFromJson(string json)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<QandAResponse>(json);
                if (model == null) return null;
                if (model.CreatedDate == null)
                {
                    model.CreatedDate = DateTime.UtcNow;
                }
                return model;
            }
            catch
            {
                return null;
            }
        }   

        public static string GetJsonFromModelInOneLine(QandAResponse model)
        {
            return JsonConvert.SerializeObject(model, Formatting.None);
        }

        public static string GetJsonFromModel(QandAResponse model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }

        public static bool AddAnalysisToFileStore(QandAResponse model)
        {
            if (model.Question == null) return false;
            DebugOutput.OutputMethod("AddAnalysisToFileStore", model.Question);
            var location = "/APIServer/BlobStore/AnswersAnalysed/";
            var file = "AnswerAnalysed.json";
            var projectPathAndFail = location + file;
            // If there is already data in AnswerAnalysed.json
            // We need to store that in a model - so we can append it!
            List<QandAResponse>? fullModel = new();
            if (FileUtils.FileCheck(projectPathAndFail))
            {
                var fileString = FileUtils.GetFileContentsAsString(projectPathAndFail);
                if (fileString != null)
                {
                    if (fileString.Length > 5)
                    {
                        var currentJson = FileUtils.GetFileContentsAsString(projectPathAndFail);
                        if (currentJson == null) return false;
                        DebugOutput.Log($"CurrentJson gotten! {currentJson}");
                        var currentModel = GetModelsFromJson(currentJson);
                        if (currentModel == null) return false;
                        fullModel = currentModel;
                        DebugOutput.Log($"currentModel gotten now full model!");
                    }             
                }
            }
            // We adding this new model to the full model
            // even if full model is empty
            fullModel?.Add(model);
            DebugOutput.Log($"We now have the full model + this model added");
            
            var newModelJson = GetJsonFromModelsNoFormatting(fullModel);
            if (newModelJson == null) return false;
            DebugOutput.Log($"POPOULATE");
            if (!FileUtils.FileDeletion(projectPathAndFail)) return false;
            DebugOutput.Log($"DELETE DONE gotten!");
            return FileUtils.FileLinePopulate(projectPathAndFail, newModelJson);
        }

        public static QandAResponse? GetSingleModelByQuestion(List<QandAResponse> listOfModels, string question)
        {
            foreach (var model in listOfModels)
            {
                if (model.Question != null)
                {
                    if (model.Question.ToLower().Trim() == question.ToLower().Trim())
                    {
                        return model;
                    }
                }
            }
            return null;
        }

    }   

    



}