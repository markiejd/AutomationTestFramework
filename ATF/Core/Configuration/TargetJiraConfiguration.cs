using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Configuration
{
    public class TargetJiraConfiguration
    {
        private const string EnvironmentVariable = "ATFENVIRONMENT";
        public static Root Configuration { get; private set; } = new Root();

        /// Variables for configuration of Jira (and multiple Jiras)
        public class JiraConfiguration
        {
            public string JiraName { get; set; } = string.Empty;  // a unique id for the Jira
            public string Url { get; set; } = string.Empty;  // The head URL of the Jira
            public string Authorization { get; set; } = string.Empty;  // token or user or 2fa - what do I use to authorise the API Calls
            public string? UserName { get; set; } // not required, should be the email used in Jira
            public string? UserPassword { get; set; }  // not requied unless using user and you have supplied UserName,  text password *note non encrypted - not recommended
            public string? Token { get; set; } // not required hard coded token used for authorisation
            public string? Email { get; set; } // not required, should be the email used in Jira 
            public string? Domain { get; set; }  //  not required, should be the domain used in Jira  *note NOT project (used in multi hosted Jira)
            public bool Sync { get; set; } = false;  // set to false unless changed in the json - Send your test BDD to Jira, if successly executed, overwrite Xray with the steps if different from test BDD executed
            public bool XrayTestExecutions { get; set; } = false;  // set to false unless changed in the json - Create a test execution, add all executed tests to the test execution, and update with status of test from ATF *Note requires Sync to be true (can only execute tests after confirming the tests are exactly as the same)
            public string? ProjectKey { get; set; } = string.Empty; // Project key is normally the first chars in the jira ticket
            public string? TestEnvironment { get; set; } = string.Empty; // Test Environment is the environment the test was run in  -  e.g. UAT, DEV, PROD  (our case HUX_Dev)
            public string? DotNet { get; set; } = "run"; // When calling any dotnet command to do with jira - what is the command - run (compile and run code) exe (run an executable)
        }

        public class Root
        {
            public List<JiraConfiguration>? JiraConfiguration { get; set; }
            public string? TokenLocation { get; set; }
            public string? UserDetailsLocation { get; set; } 
        }

        public static Root GetConfiguration()
        {
            return Configuration;
        }


        public static JiraConfiguration? GetJiraConfigurationByName(string name)
        {
            if (Configuration.JiraConfiguration == null) return null;
            foreach (var conf in Configuration.JiraConfiguration)
            {
                if (conf.JiraName.ToLower() == name.ToLower()) return conf;
            }
            return null;
        }

        private static Root? GetToken(Root config)
        {
            if (config.JiraConfiguration == null) return null;
            var tokenLocation = config.TokenLocation;
            foreach(var jiraDetails in config.JiraConfiguration)
            {
                if (jiraDetails.Authorization.ToLower() == "token")
                {
                    // get the name
                    var name = TargetConfiguration.Configuration.JiraName;
                    if (name != null)
                    {
                        // look for a file
                        var fileName = "jiratoken-" + name.ToUpper() + ".txt";
                        var fullFileName = tokenLocation + fileName;
                        string? token;
                        if (!FileUtils.OSFileCheck(fullFileName))
                        {
                            DebugOutput.Log($"Unable to find the file {fullFileName} maybe I'm a pipeline agent?");
                            // get the token from an environment variable
                            token = System.Environment.GetEnvironmentVariable("JIRA_CLOUD_API_TOKEN");
                        }
                        else
                        {
                            var text = FileUtils.OSGetFileContentsAsString(fullFileName);
                            token = text;
                        }
                        jiraDetails.Token = token;
                    }             
                }
            }
            return config;
        }

        public static Root? ReadJson()
        {
            var fileName = $"jiraSettings.{Environment}.json";
            var directory = "/AppTargets/Resources/Variables/Jira/";
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
                var tokenObj = GetToken(obj);
                if (tokenObj == null)
                {
                    DebugOutput.Log($"Something went wrong dealing with tokens!");
                    Configuration = obj;
                    return Configuration;
                }
                // get the token
                Configuration = tokenObj;
                DebugOutput.Log($"We have an object to return!");
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
