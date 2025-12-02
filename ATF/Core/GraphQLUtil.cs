using AppXAPI;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Core.Configuration;
using Newtonsoft.Json;

namespace Core
{
    public class GraphQL
    {
        public string? query { get; set; }
        public Data? data { get; set; }

    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public GetTest? getTest { get; set; }
    }

    public class GetTest
    {
        public string? gherkin { get; set; }
    }




    public class GraphQLUtil
    {
        private static string xrayCloudTokenFileName = "xraycloudtoken.txt";
        private static string xrayCloudClientFileName = "xrayclouduser.json";

        public static string GetGraphQLQuery(string query)
        {
            // $"{{ getTest(issueId: \"{issueId}\") {{ gherkin }} }}"
            return $"{{\"query\":\"{query}\"}}";
        }

        public static string GetGraphQLObject(GraphQL model)
        {
            // I want to take the GraphQL model and convert to json
            return JsonConvert.SerializeObject(model);
        }

        public static GraphQL? GetGraphQLModel(string json)
        {
            return JsonConvert.DeserializeObject<GraphQL>(json);
        }

        public static string? GetCurrentJWTTokenForXRAY()
        {
            DebugOutput.OutputMethod("GetCurrentJWTTokenForXRAY");
            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;
            DebugOutput.Log($"Looking for {jiraName}");
            if (jiraDetails.Authorization.ToLower() != "cloud") return null;
            if (TargetJiraConfiguration.Configuration.UserDetailsLocation == null) return null;
            var directory = TargetJiraConfiguration.Configuration.UserDetailsLocation;
            var fullFileNameAndPath = directory + xrayCloudTokenFileName;
            if (!FileUtils.OSFileCheck(fullFileNameAndPath))
            {
                DebugOutput.Log($"File does not exist! Maybe I am a pipeline!");
                // get the system env variable for ATF_XRAY_CLOUD_TOKEN
                var envToken = Environment.GetEnvironmentVariable("ATF_XRAY_CLOUD_TOKEN");
                if (envToken != null)
                {
                    DebugOutput.Log($"Found the env variable for the token!");
                    return envToken;
                }
                DebugOutput.Log($"No env variable for the token!");
                return null;
            }
            DebugOutput.Log($"File exists!");
            var token = FileUtils.OSGetFileContentsAsString(fullFileNameAndPath);
            return token;
        }

        public static string? GetNewJWTTokenForXRAY()
        {
            DebugOutput.OutputMethod("GetNewJWTTokenForXRAY");
            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;
            DebugOutput.Log($"Looking for {jiraName}");
            if (jiraDetails.Authorization.ToLower() != "cloud") return null;
            if (TargetJiraConfiguration.Configuration.UserDetailsLocation == null) return null;
            var directory = TargetJiraConfiguration.Configuration.UserDetailsLocation;
            var fullFileNameAndPath = directory + xrayCloudClientFileName;
            string? jsonContent;
            if (FileUtils.OSFileCheck(fullFileNameAndPath))
            {
                DebugOutput.Log($"File exists!");
                jsonContent = FileUtils.OSGetFileContentsAsString(fullFileNameAndPath);
                if (jsonContent == null) return null;
            }
            else
            {                
                DebugOutput.Log($"File does not exist!");
                jsonContent = Environment.GetEnvironmentVariable("ATF_XRAY_CLOUD_CLIENT");
                if (jsonContent == null)
                {
                    DebugOutput.Log($"No env variable for the client details!");
                    return null;
                }
            }                        
            string AUTH_URL = "https://xray.cloud.getxray.app/api/v2/authenticate";
            var result = APIUtil.Post(AUTH_URL, jsonContent, "XRAY", false).Result;
            if (result == null) return null;
            DebugOutput.Log("result not null");
            // get the returning string from the response result
            var token = result.Content.ReadAsStringAsync().Result;
            DebugOutput.Log($"TOKEN is {token}");

            // write token to file
            var xrayFullFileNameAndPath = directory + xrayCloudTokenFileName;
            if (FileUtils.OSFileCheck(xrayFullFileNameAndPath))
            {
                if (!FileUtils.OSFileDeletion(xrayFullFileNameAndPath)) return null;
            }
            DebugOutput.Log($"population of the token file {token}");
            token = token.Replace("\"","");
            if (!FileUtils.OSFileCreationAndPopulation(directory, xrayCloudTokenFileName, token)) return null;
            return token;
        }

    }
}