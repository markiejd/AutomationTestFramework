
using System.Dynamic;
using Core.Logging;

namespace Core.NLM
{
    public static class ListOfChatbotModels
    {
        public static List<ChatbotModel>? ChatbotModels { get; set; }
    }

    public class ChatbotModel
    {
        public int? questionNumber { get; set; }
        public string? question { get; set; }
        public string? aiAnswer { get; set; }
        public string? expectedAnswer { get; set; }
        public string? originalQuestion { get; set; }
        public DateTime? TimeOfTest { get; set; }
    }

    public static class ChatbotModelModelWorking
    {
        public static bool CreateSingleAnalysisModel(int questionNumber, string question, string aIAnswer, float? aIAnswerSentiment, float? aIAnswerSentimentMagnatude, string? ExpectedAnswer, float? expectedAnswerSentiment, float? expectedAnswerSentimentMagnatude, float? CosineSimilarity, float? JaccardSimilarity, int? LevenshteinDistance)
        {
            DebugOutput.OutputMethod("CreateSingleAnalysisModel", $"{questionNumber} {question}");

            return false;
        }

        public static string? GetJsonFromModels(List<ChatbotModel>? chatbotModels)
        {
            if (chatbotModels == null) return null;
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(chatbotModels);
            return jsonString;
        }
    }
}
