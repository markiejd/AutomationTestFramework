
using Core;
using Core.Logging;
using Generic.Steps.AIChatBots.QandAResponse;
using Newtonsoft.Json;

namespace Generic.Steps.AIChatBots
{
    public class QuestionsAndAnswers
    {
        public string? QuestionAsked { get; set; }
        public string? AnswerReceived { get; set; }
        public int? NumberOfCitations { get; set; }
        public string? CitationTitles { get; set; }
        public TimeSpan? TimeQuestionTaken { get; set; }
        public TimeSpan? TimeAnswerProduced { get; set; }
        public TimeSpan? TimeToReadAnswer { get; set; }
    }

    public class QuestionsAndAnswersUsing
    {
        
        private static bool Failure(string proc, string message = "***** FAILURE ******", bool flag = false)
        {
            DebugOutput.Log("***** FAILURE ****** " + message);
            CombinedSteps.Failure(proc);
            return flag;
        }

        
        public static QuestionsAndAnswers? CreateModel(string questionAsked, string answerReceived, int? numberOfCitations, string? citationTitles, TimeSpan? timeQuestionTaken, TimeSpan? timeAnswerProduced, TimeSpan? timeToReadAnswer)
        {
            var newQuestionsAndAnswers = new QuestionsAndAnswers();
            newQuestionsAndAnswers.QuestionAsked = questionAsked;
            newQuestionsAndAnswers.AnswerReceived = answerReceived;
            newQuestionsAndAnswers.NumberOfCitations = numberOfCitations;
            newQuestionsAndAnswers.CitationTitles = citationTitles;
            newQuestionsAndAnswers.TimeQuestionTaken = timeQuestionTaken;
            newQuestionsAndAnswers.TimeAnswerProduced = timeAnswerProduced;
            newQuestionsAndAnswers.TimeToReadAnswer = timeToReadAnswer;
            return newQuestionsAndAnswers;
        }

        public static string GetJsonFromModel(QuestionsAndAnswers model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }

        private static QandAResponse.QandAResponse? HandleMessageCode(QandAResponse.QandAResponse? qAndAResponseModel)
        {
            if (qAndAResponseModel == null) return null;
            if (qAndAResponseModel.Message == null) return null;
            var first3CharsOfQandAResponse = qAndAResponseModel.Message.Substring(0, 3);
            try 
            {
                var intMessageCode = Int32.Parse(first3CharsOfQandAResponse);
                qAndAResponseModel.MessageCode = intMessageCode;
                switch(qAndAResponseModel.MessageCode)
                {
                    default: return qAndAResponseModel;

                    case 200:
                    {
                        DebugOutput.Log($"This is a NEW question - no variant found - nothing to measure against!");
                        qAndAResponseModel.SimilarityScore = 1;
                        qAndAResponseModel.ClosestAnswer = "New Question";
                        qAndAResponseModel.ClosestQuestion = null;
                        if (qAndAResponseModel.NLTKMeasurement != null)
                        {
                            qAndAResponseModel.NLTKMeasurement.ClosestAnswerCategory = new List<string> { "New Question - no comparison" };
                            qAndAResponseModel.NLTKMeasurement.ClosestAnswerCategoryScore = 1;  
                            qAndAResponseModel.NLTKMeasurement.ClosestAnswerEntities = new List<string> { "" };
                            qAndAResponseModel.NLTKMeasurement.ClosestAnswerEntitiesType = new List<string> { "" };
                            qAndAResponseModel.NLTKMeasurement.ClosestQuestionEntities = new List<string> { "" };
                            qAndAResponseModel.NLTKMeasurement.ClosestQuestionEntitiesType = new List<string> { "" };
                            if (qAndAResponseModel.NLTKMeasurement.ComparisonResults != null)
                            {
                                qAndAResponseModel.NLTKMeasurement.ComparisonResults.ClosestAnswerSentiment = 0;
                                qAndAResponseModel.NLTKMeasurement.ComparisonResults.ClosestMagnitude = 0;
                                qAndAResponseModel.NLTKMeasurement.ComparisonResults.CosineSimilarity = 1;
                                qAndAResponseModel.NLTKMeasurement.ComparisonResults.JaccardSimilarity = 1;
                                qAndAResponseModel.NLTKMeasurement.ComparisonResults.LevenshteinDistance = 0;
                                qAndAResponseModel.NLTKMeasurement.ComparisonResults.SentimentDifference = 0;
                            }
                        } 
                        return qAndAResponseModel;
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to parse the first 3 chars of the message '{first3CharsOfQandAResponse}' from 369 model!");
                return null;
            }
        }

        
        public async static Task<bool> SendQuestionAndAnswerModel(QuestionsAndAnswers questionAndAnswerModel)
        {
            var questionAndAnswerJson = QuestionsAndAnswersUsing.GetJsonFromModel(questionAndAnswerModel);
            DebugOutput.Log($"JSON OUTPUT for Q&A IS {questionAndAnswerJson}");
            if (questionAndAnswerJson == null) return Failure($"Failed to conver model back into JSON!");

            // want to send this for analysis!
            var url = "https://atf369-cvewbtehe3f9geh4.eastus-01.azurewebsites.net/api/POST/UploadQandA";
            var response = await APIUtil.Post(url, questionAndAnswerJson, "postQandA", false);
            DebugOutput.Log($"Response from API {url} '{response.StatusCode}'");
            string? responseText = null;
            if(response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
                DebugOutput.Log($"That is REAL NICE {responseText}");
                if (responseText.Length > 10)
                {
                    var QandAResponseModel = QandAResponse.QandAResponseUsing.GetModelFromJson(responseText);
                    QandAResponseModel = HandleMessageCode(QandAResponseModel);
                    if (QandAResponseModel == null) return Failure($"Failed to get the Q&A model back from the Json convert!");
                    if (QandAResponseModel.Message == null) return Failure($"Failed to get the message text from 369 model!");
                    if (!QandAResponseUsing.AddAnalysisToFileStore(QandAResponseModel)) return Failure($"Failed to add the analysis to the file store!");      
                }              
                return true;
            }
            else
            {
                DebugOutput.Log($"FAIL FAIL FAIL {response.StatusCode}");
                return Failure($"Failed around the response from API {url}");
            }
        }


        
    }

}