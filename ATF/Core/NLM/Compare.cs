

namespace Core.NLM
{
    public class CompareAnswer
    {
        public int? QuestionNumber { get; set; }
        public string? Question { get; set; }
        public List<string>? ListOfQuestionEntitiesType { get; set; }
        public List<string>? ListOfQuestionEntitiesName { get; set; }
        public List<string>? ListOfQuestionEntitiesSalience {get; set; }
        // public AnalyzeSyntaxResponse? QuestionPOS { get; set; }

        public string? AIAnswer { get; set; }
        public float? AIAnswerSentiment { get; set; }
        public float? AIAnswerSentimentMagnatude { get; set; }
        public List<string>? ListOfAIAnswerEntitiesType { get; set; }
        public List<string>? ListOfAIAnswerEntitiesName { get; set; }
        public List<string>? ListOfAIAnswerEntitiesSalience {get; set; }
        // public AnalyzeSyntaxResponse? AIAnswerPOS { get; set; }

        public string? ExpectedAnswer { get; set; }
        public float? ExpectedAnswerSentiment { get; set; }
        public float? ExpectedAnswerSentimentMagnatude { get; set; }
        public List<string>? ListOfExpectedAnswersEntitiesName { get; set; }
        public List<string>? ListOfExpectedAnswersEntitiesType { get; set; }
        public List<string>? ListOfExpectedAnswersEntitiesSalience { get; set; }
        // public AnalyzeSyntaxResponse? ExpectedAnswerPOS { get; set; }

        public double? CosineSimilarity { get; set; }
        public int? LevenshteinDistance { get; set; }
        public double? JaccardSimilarity { get; set; }




    }
}
