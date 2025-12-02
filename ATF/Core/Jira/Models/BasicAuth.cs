
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Jira
{
    // Root myDeserializedClass = JsonConvert.Deserializestring?<Root>(myJsonResponse);
    public class JiraBasicCredentials
    {
        public string username { get; set; } = "duffym";
        public string password { get; set; } = "JIRAMarkie101";
    }

    public class UsingJiraBasicCredentials
    {
        public static JiraBasicCredentials? ReadJiraCredentials()
        {
            DebugOutput.OutputMethod("JiraCredentials");
            // get the place
            var jiraDetails = TargetJiraConfiguration.Configuration;
            var location = jiraDetails.UserDetailsLocation;
            var fileName = "JiraBasic.json";
            var fullFileName = location + fileName;
            DebugOutput.Log($"Looking in {fullFileName} for basic credentials!");

            if (!FileUtils.OSFileCheck(fullFileName)) return null;

            // Read the file
            var jsonString = FileUtils.OSGetFileContentsAsString(fullFileName);
            if (jsonString == null) return null;

            // convert Json to Model
            try
            {
                var model = JsonConvert.DeserializeObject<JiraBasicCredentials>(jsonString);
                if (model == null) return null;
                DebugOutput.Log($"WE have our model of Jira details! {model.username} ");
                return model;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"We failed to create the basic jira model due to {ex}");
                return null;
            }
        }
    }
}
