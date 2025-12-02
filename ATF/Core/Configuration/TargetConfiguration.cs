using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Configuration
{
    public class TargetConfiguration
    {
        /// <summary>
        ///     Environment variable that is read to get the target environment name
        /// </summary>
        private const string EnvironmentVariable = "ATFENVIRONMENT";
        public static TargetConfigurationData Configuration { get; private set; } = new TargetConfigurationData();

        public class TargetConfigurationData
        {
            public bool AIChatBot { get; set; } = false; // Are we testing an AI Chat Bot - if true we need to read TargetAIChatBotConfiguration
            public string ApplicationType { get; set; } = String.Empty; // Windows, Browser ??
            public string AreaPath { get; set; } = String.Empty;  // What is our application area path
            public bool AzureOpenAI { get; set; } = false;  //  Are we using Azure Open AI
            public string AzureOpenAIDeployment { get; set; } = String.Empty;   //  What deployment to use
            public string Browser { get; set; } = String.Empty;  //  What Browser to use
            public string DateFormat { get; set; } = String.Empty;  // What is our date format
            public int Debug { get; set; } = 0;
            public bool Jira { get; set; } = false;
            public string? JiraName { get; set; }
            public bool HandleBugs { get; set; } = true; // Handle bugs, true = use bug work around, false = do not use bug work around
            public bool HandleImages { get; set; } = true;  // Use Images to navigate, compare etc.
            public int NegativeTimeout { get; set; } = 2;  // How long to wait for something to leave
            public bool OutputOnly { get; set; } = false;  // true = do not run the tests just output the BDD
            public bool PerformanceTesting { get; set; } = true; // false does not output performance test reports,  true will produce the reports
            public bool PerformanceComparison { get; set; } = true; // false does not run performance comparison reports,  true will produce the reports
            public int PositiveTimeout { get; set; } = 15;  // How long to wait for something to appear
            public string ProjectName { get; set; } = String.Empty;
            public string ScreenSize { get; set; } = "2400x2400";
            public bool SelfHeal { get; set; } = false;
            public bool SkipOnFailure { get; set; } = false;
            public int TimeoutMultiplie { get; set; } = 1;
            public bool TestReport { get; set; } = true;
            public string TestTool { get; set; } = "S";  // S=Selenium, P=Playwright
            public string Version { get; set; } = String.Empty;
            public bool WarningFileOutput { get; set; } = false;

            public string[] ATFVariableArray = Enumerable.Repeat(string.Empty, 10).ToArray(); // Array to hold the variable values
            
        }

        //public static void TargetConfiguration()
        public static TargetConfigurationData? ReadJson()
        {
            var fileName = $"targetSettings.{Environment}.json";
            var directory = "./AppTargets/Resources/";
            var fullFileName = directory + fileName;
            if (!FileUtils.FileCheck(fullFileName))
            {
                DebugOutput.Log($"Unable to find the file {fullFileName}");
                return null;
            }
            var jsonText = File.ReadAllText(fullFileName);
            DebugOutput.Log($"Json - {jsonText}");
            try
            {
                var obj = JsonConvert.DeserializeObject<TargetConfigurationData>(jsonText);
                if (obj == null) return null;
                Configuration = obj;
                // DebugOutput.Log($">>>> {obj.AreaPath}  ... {Configuration.AreaPath}");
                // DebugOutput.Log($"THIS DEBUG LEVEL SET TO {System.Environment.GetEnvironmentVariable("ENVIRONMENT")}");
                return Configuration;
            }
            catch
            {
                DebugOutput.Log($"We out ere");
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