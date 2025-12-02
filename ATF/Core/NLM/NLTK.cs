
using System.Text.Json;


namespace Core.NLTK
{
    public class ComparisonResults
    {
        public float cosineSimilarity { get; set; }
        public float expectedAnswerSentiment { get; set; }
        public float expectedMagnitude { get; set; }
        public float aiAnswerSentiment { get; set; }
        public float aiMagnitude { get; set; }
        public float sentimentDifference { get; set; }
        public int levenshteinDistance { get; set; }
        public float jaccardSimilarity { get; set; }
    }

    public class NLTKAnalysis
    {
        public int questionNumber { get; set; }
        public string question { get; set; } = string.Empty;
        public string? originalQuestion { get; set; }
        public DateTime? TimeOfTest { get; set; }
        public List<string>? QuestionEntities { get; set; }
        public List<string>? QuestionEntitiesType { get; set; }
        public string aiAnswer { get; set; } = string.Empty;
        public List<string>? AIEntities { get; set; }
        public List<string>? AIEntitiesType { get; set; }
        public string AICategory { get; set; } = string.Empty;
        public float AICategoryScore { get; set; }
        public string? expectedAnswer { get; set; }
        public List<string>? ExpectedEntities { get; set; }
        public List<string>? ExpectedEntitiesType { get; set; }
        public string ExpectedCategory { get; set; } = string.Empty;
        public float ExpectedCategoryScore { get; set; }
        public ComparisonResults? comparisonResults { get; set; }
        public SentenceResults? sentenceResults {get; set; }
    }

    public class SentenceResults
    {
        public List<string>? AISentences { get; set; }
        public List<float>? AISentencesSentiment { get; set; }
        public List<string>? AISentencesCategory { get; set; }
        public List<string>? expectedSentances { get; set; }
        public List<float>? expectedSentencesSentiment { get; set; }
        public List<string>? expectedSentencesCategory { get; set; }
    }

    public static class NLTKAnalysisWorking
    {
        public static List<NLTKAnalysis>? ConvertJsonToModel(string jsonString)
        {
            if (jsonString == null) return null;
            List<NLTKAnalysis>? nLTKAnalyses = JsonSerializer.Deserialize<List<NLTKAnalysis>>(jsonString);
            return nLTKAnalyses;
        }
    }

    


}