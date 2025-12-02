using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Configuration
{
    public class TargetAIChatBotConfiguration
    {
        public static Root Configuration { get; private set; } = new Root();
        private const string EnvironmentVariable = "ATFENVIRONMENT";


        public class Root
        {
            public string ChatBotApplication { get; set; } = String.Empty;
            public string ChatBotURL { get; set; } = String.Empty;
            public bool AnswerIsAnalysed { get; set; } = false;
            public bool AnswerIsCompared { get; set; } = false;
            public bool AnswerIsOut { get; set; } = false;
            public bool AnswerHistoricComparison { get; set; } = false;
            public string AnswerNLP { get; set; } = String.Empty;
            public string ProjectLocationOfToBeAnalysedQuestions { get; set; } = String.Empty;
            public string AnalysedAnswerLocation { get; set; } = String.Empty;
        }
        

        public static Root GetConfiguration()
        {
            return Configuration;
        }
        
        public static Root? ReadJson()
        {
            DebugOutput.Log($"ReadJson for TargetAIChatBotConfiguration");
            var fileName = $"AIChatBot.{Environment}.json";
            var directory = "/AppTargets/Resources/Variables/AIChatBot/";
            var fullFileName = FileUtils.GetCorrectDirectory(directory + fileName);            
            if (!FileUtils.OSFileCheck(fullFileName))
            {
                DebugOutput.Log($"Unable to find the file {fullFileName}");
                return null;
            }
            var jsonText = File.ReadAllText(fullFileName);
            DebugOutput.Log($"Json - {jsonText}");
            try
            {
                var obj = JsonConvert.DeserializeObject<Root>(jsonText);
                if (obj == null) return null;
                Configuration = obj;
                DebugOutput.Log($"We have a TargetAIChatBotConfiguration object to return!");
                return Configuration;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"We out ere {e}");
                return null;
            }
        }

        
        /// <summary>
        ///     Set up an environment variable called "ENVIRONMENT", and set it to type of environment 
        ///     development, beta, live etc.
        ///     Then you need a targetSettings.ENVIRONMENT.json file in the Resources folder of the AppTargets project
        ///     Different environments need different configuration.  this is how that is controled.
        ///     If there is no ENVIRONMENT environment variable it will use development as default.
        /// </summary>
        private static string Environment =>
            System.Environment.GetEnvironmentVariable(EnvironmentVariable) ?? "development";
        

    }

}