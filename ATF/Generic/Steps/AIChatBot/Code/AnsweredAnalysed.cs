
using Core.Logging;

namespace Generic.Steps.AIChatBots
{
    public class AnswersAnalysed
    {
        public string Id { get; set; } = string.Empty;
        public string QuestionAsked { get; set; } = string.Empty;
        public string AnswerRecieved { get; set; } = string.Empty;
        public int NumberOfCitations { get; set; } = 0;
        public string? CitationTitles { get; set; } = null;
        public TimeSpan TimeQuestionTaken { get; set; } = TimeSpan.Zero;
        public TimeSpan TimeAnswerProduced { get; set; } = TimeSpan.Zero;
        public TimeSpan TimeToReadAnswer { get; set; } = TimeSpan.Zero;

        ///  from NLTK
        ///  
        public bool? ExactQuestionFound { get; set; } = null;
        public string? QuestionComparedTo { get; set; } = null;
        public string? AnswerComparedTo { get; set; } = null;      


    }

    public class AnswersAnalysedUse
    {
        public static AnswersAnalysed GetDemoModel()
        {
            var newModel = new AnswersAnalysed();
            newModel.Id = EPOCHControl.Epoch ?? "1";
            newModel.QuestionAsked = "Provide boiler plate information about CGI";
            newModel.AnswerRecieved = "CGI is a global information technology consulting and outsourcing company . They offer a wide range of services including project management, technology solutions selection, vendor management, software installation and configuration, training, risk management, and more 1 . CGI has experience in various sectors such as publishing automation, digital photography workflow, and technology testing assessment 1 . They have expertise in energy efficiency programs, renewable energy technologies, and conservation potential review 2 3 .";
            newModel.NumberOfCitations = 3;
            newModel.CitationTitles = "hello^testing delimited^goodbye^";
            newModel.TimeQuestionTaken = TimeSpan.Parse("00:00:00.3201175");
            newModel.TimeAnswerProduced = TimeSpan.Parse("00:00:05.2543461");
            newModel.TimeToReadAnswer = TimeSpan.Parse("00:00:00.0529270");
            newModel.ExactQuestionFound = true;
            newModel.QuestionComparedTo = null;
            newModel.AnswerComparedTo = "CGI is a global information technology consulting and outsourcing company . They offer a wide range of services including project management, technology solutions selection, vendor management, software installation and configuration, training, risk management, and more 1 . CGI has experience in various sectors such as publishing automation, digital photography workflow, and technology testing assessment 1 . They have expertise in energy efficiency programs, renewable energy technologies, and conservation potential review 2 3 .";

            /*

        public bool? ExactQuestionFound { get; set; } = null;
        public string? QuestionComparedTo { get; set; } = null;
        public string? AnswerComparedTo { get; set; } = null;      
            */
            return newModel;

        }
    }
}