using Newtonsoft.Json;

namespace AppXAPI
{
    /// This will allow you to create environmental variables!
    /// We read a Json file based upon the system environment variable of 
    ///         private const string EnvironmentVariable = "ENVIRONMENT";
    /// This is read in the Hooks and is called by APIUtil.GetAPILinkJson();
    public class VariableConfiguration
    {
        ///     Environment variable that is read to get the target environment name
        /// </summary>
        private const string EnvironmentVariable = "ATFENVIRONMENT";

        public static VariableConfiguration.VariableConfigurationData Configuration {get; private set;} = new VariableConfiguration.VariableConfigurationData();
    
        public class VariableConfigurationData
        {          
            public string URL { get; set; } = "UNKNOWN";
            public string DefaultUserName { get; set; } = "UNKNOWN";
            public string DefaultPassword { get; set; } = "UNKNOWN";
            public string FileSystemName { get; set; } = "UNKNOWN";




            // below may not be used by your application, but should be kept!
            public string APIServer { get; set; } = "UNKNOWN";
            public string AppUserName { get; set; } = "UNKNOWN";
            public string AppUserPassword { get; set; } = "UNKNOWN";    

            public string JiraServer { get; set; } = "UNKNOWN";
            public string JiraAPI { get; set; } = "UNKNOWN";    
            public string JiraUserName { get; set; } = "UNKNOWN";
            public string JiraUserPassword { get; set; } = "UNKNOWN";
            public string XRayAPI { get; set; } = "UNKNOWN";
        }
        
        public static VariableConfigurationData GetVariableModel(string jsonString)
        {
            var APIListModel = new VariableConfiguration.VariableConfigurationData();
            var defaultAPIListModel = new VariableConfiguration.VariableConfigurationData();
            if (jsonString == null) return defaultAPIListModel;
            APIListModel = JsonConvert.DeserializeObject<VariableConfiguration.VariableConfigurationData>(jsonString);
            if (APIListModel == null) return defaultAPIListModel;
            Configuration = APIListModel;
            return APIListModel;
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
