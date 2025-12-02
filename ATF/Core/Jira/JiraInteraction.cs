using System;
using System.CodeDom;
using System.Data;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using AppXAPI;
using Core.Configuration;
using Core.FileIO;
using Core.Images;
using Core.Jira.XRAY.Execution.Model;
using Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Jira
{
    public class JiraTestModel
    {
        public string Scenario { get; set; } = string.Empty;
        public List<string> Steps { get; set; } = new List<string>();
    }
    
    public class Credentials
    {
        public string client_id { get; set; } = string.Empty;
        public string client_secret { get; set; } = string.Empty;
    }

    public class Jira
    {
        public static List<JiraTestModel> AllScenarios { get; private set;} = new List<JiraTestModel>();
        public static List<string> status = new List<string> {"open",   "ready for work",   "in progress",  "internal pending",     "external pending",     "requrest for approval",    "in review",    "published",    "closed",   "ready for testing",    "on hold",  "blocked",  "cancelled"};
        public static List<string> action = new List<string> {"edit",   "edit",             "edit",         "non edit",             "non edit",             "non edit",                 "non edit",     "non edit",     "non edit", "edit",                 "edit",     "edit",     "non edit"};


        private static string? CreateTestExecutionStandard()
        {
            DebugOutput.OutputMethod("CreateTestExecutionStandard", $"");
            var issueData = new
            {
                fields = new
                {
                    project = new { key = "SDIACSC" }, // Replace with your project key
                    summary = "Test Execution Issue",
                    description = "Creating a test execution issue via REST API",
                    issuetype = new { name = "Test Execution" } // Replace with the appropriate issue type
                }
            };
            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(issueData), Encoding.UTF8, "application/json");
            var url = GetURLFor("description", "");
            if (url == null) return null;
            var response = APIUtil.PostContentWithToken(url, jsonContent).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                if (responseContent != null && responseContent.Length > 1)
                {
                    #pragma warning disable CS8604 // Possible null reference argument.
                    #pragma warning disable CS8601 // Possible null reference assignment.
                    var TestExecutionKey = JObject.Parse(responseContent)["key"].Value<string>();
                    #pragma warning restore CS8601 // Possible null reference assignment.
                    #pragma warning restore CS8604 // Possible null reference argument.
                    return TestExecutionKey;
                }
            }
            else
            {
                DebugOutput.JiraOutput("FAILED to create or read from new Execution Code");
                return null;
            }
            DebugOutput.JiraOutput($"FAILED to get success code {(int)response.StatusCode} creating test execution");
            return null;
        }

        private static string? CreateTestExecutionCloud()
        {
            DebugOutput.OutputMethod("CreateTestExecutionCloud", $"");
            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;
            var summary = "Automated Test Execution";
            var description = "This is created when a test execution is run in ATF360";
            var projectKey = jiraDetails.ProjectKey;
            var jiraEnvironment = jiraDetails.TestEnvironment;
            // var harpreetEnvironment = "Dev";
            // var customfield_10243Value = "10218";
            var token = GraphQLUtil.GetCurrentJWTTokenForXRAY();
            if (token == null) token = GraphQLUtil.GetNewJWTTokenForXRAY();
            if (token == null) return null;
            var response = CmdUtil.ExecuteDotnet("./CommunicationXrayCloud/XrayCloudCommunication.csproj", $"\"CreateTestExecution\" \"{token}\" \"{summary}\" \"{description}\" \"{projectKey}\" \"{jiraEnvironment}\"").Trim();
            if (response == "" || response == null || response.ToLower().Contains("error"))
            {
                DebugOutput.Log($"Need to try new token");
                token = GraphQLUtil.GetNewJWTTokenForXRAY();
                if (token == null) return null;
                response = CmdUtil.ExecuteDotnet("./CommunicationXrayCloud/XrayCloudCommunication.csproj", $"\"CreateTestExecution\" \"{token}\" \"{summary}\" \"{description}\" \"{projectKey}\" \"{jiraEnvironment}\"").Trim();
            }
            if (response == "" || response == null || response.ToLower().Contains("error")) return null;
            // I have the response - but I only want to return the test execution key!
            // Parse the JSON string
            try
            {
                string? keyValue = null;
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    // Navigate to the key value
                    JsonElement root = doc.RootElement;
                    JsonElement keyElement = root
                        .GetProperty("data")
                        .GetProperty("createTestExecution")
                        .GetProperty("testExecution")
                        .GetProperty("jira")
                        .GetProperty("key");

                    // Extract and print the key value
                    keyValue = keyElement.GetString();
                }
                return keyValue;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to get the key from the response {ex}");
                return null;
            }
        }


        /// <summary>
        /// Create a Test Execution and return the Key
        /// </summary>
        /// <returns></returns>
        public static string? CreateTestExecution()
        {
            DebugOutput.OutputMethod("CreateTestExecution", $"");
            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;
            switch(jiraDetails.Authorization.ToLower())
            {
                default: return CreateTestExecutionStandard();
                case "cloud": return CreateTestExecutionCloud();
            }
        }

        /// <summary>
        /// Add a single test to a test execution already created
        /// </summary>
        /// <param name="testExecutionKey"></param>
        /// <param name="testKey"></param>
        /// <returns></returns>
        public static bool CreateTestsInTestExecution(string testExecutionKey, string testKey)
        {
            DebugOutput.OutputMethod("CreateTestsInTestExecution", $"{testExecutionKey} {testKey}");
            var url = GetURLFor("testexec", testExecutionKey);
            if (url == null) return false;
            var content = new StringContent("{ \"add\" : [ \"" + testKey + "\" ] }", Encoding.UTF8, "application/json");
            var response = APIUtil.PostContentWithToken(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                DebugOutput.JiraOutput($"Unable to add {testKey} to Execution {testExecutionKey} " + (int)response.StatusCode + " " + url);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Move an issue in Jira to new state
        /// transitionId needs to be known!
        /// </summary>
        /// <param name="issueKey"></param>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        public static bool CreateIssueStepThroughWorkflow(string issueKey, string transitionId)
        {
            DebugOutput.OutputMethod("CreateIssueStepThroughWorkflow", $"{issueKey} {transitionId}");
            var url = GetURLFor("transitions", issueKey);
            if (url == null) return false;
            var requestBody = "{ \"transition\": { \"id\": \"" + transitionId + "\" } }";
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = APIUtil.PostContentWithToken(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                DebugOutput.JiraOutput($"Failed! TRANSITION! {transitionId}! {url} + {(int)response.StatusCode}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Assign an email address to an issue
        /// </summary>
        /// <param name="issueKey"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static bool CreateAssigneeToIssue(string issueKey, string emailAddress)
        {
            DebugOutput.OutputMethod("CreateAssigneeToIssue", $"{issueKey} {emailAddress}");
            var requestBody = "{ \"name\": \"" + emailAddress + "\"}";
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var url = GetURLFor("assignee", issueKey);
            var response = APIUtil.PutContentWithToken(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                DebugOutput.JiraOutput($"FAILED TO ASSIGN {emailAddress} to {issueKey} {(int)response.StatusCode}");
                return false;
            }
            return true;
        }
        

        public static string? GetActionFromStatus(string givenStatus)
        {
            DebugOutput.Log($"GetActionFromStatus {givenStatus} ");
            int counter = 0;
            foreach (var s in status)
            {
                
                if (s.ToLower() == givenStatus.ToLower())
                {
                    DebugOutput.Log($"RETURNING STATUS OF {action[counter]}");
                    return action[counter];
                } 
                counter++;
            }
            DebugOutput.Log($"NO STATUS Found for {givenStatus.ToLower()}!");
            return null;
        }

        public static Core.Jira.Model.Root? GetJiraModelFromIssue(string issueKey)
        {
            DebugOutput.OutputMethod("GetJiraModelFromIssue", $"{issueKey} ");
            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;
            var url = Jira.GetURLFor("description", issueKey);
            if (url == null) return null;
            string? jsonString;
            if (jiraDetails.Authorization == "token")
            {
                jsonString = APIUtil.GetStringWithAccessToken("", url, jiraDetails.Authorization).Result;
                if (jsonString == null) return null;
            }
            else if (jiraDetails.Authorization == "2fa" || jiraDetails.Authorization == "cloud")
            {
                if (jiraDetails.Domain == null) return null;
                if (jiraDetails.Email == null) return null; 
                jsonString = APIUtil.GetStringWithAccessToken("", url, jiraDetails.Authorization, jiraDetails.Email, jiraDetails.Domain).Result;
                if (jsonString == null) return null;
            }
            else
            {
                DebugOutput.Log($"We have no idea what authorisation we are using!");
                return null;
            }
            DebugOutput.Log($"We have the json string from {url}");
            DebugOutput.Log($"Json - {jsonString}");
            try
            {                
                var model = Core.Jira.Using.UsingJIraModel.ConvertJsonToModel(jsonString);
                if (model == null)
                {
                    DebugOutput.JiraOutput($"Failed to convert Jira to ATF Model {issueKey} ");
                    return null;
                }
                DebugOutput.Log($"Returning the model for {issueKey} {model.id}");
                return model;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"WE got an exception - my gues - we got 200 but makes NO senes! {ex}");
                return null;
            }
        }

        public static string? GetIssuesReportersEmailAddress(string issueKey)
        {
            DebugOutput.OutputMethod("GetIssuesReportersEmailAddress", $"{issueKey} ");
            var url = GetURLFor("description", issueKey);
            if (url == null) return null;
            var jsonString = APIUtil.GetStringWithAccessToken("", url).Result;
            if (jsonString == null) return null;
            try
            {                
                var model = Core.Jira.Using.UsingJIraModel.ConvertJsonToModel(jsonString);
                if (model == null) return null;
                if (model.fields != null)
                {
                    if (model.fields.reporter != null)
                    {
                        return "X";
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Something wrong in getting email address - likely model mismatch");
            }
            DebugOutput.JiraOutput($"Unable to find EMAIL Address of reporter of issue {issueKey}");
            return null;
        }


        /// <summary>
        /// Make up the URL based on your Jira Settings
        /// </summary>
        /// <param name="what"></param>
        /// <param name="key"></param>
        /// <returns>url as a string</returns>
        public static string GetURLFor(string what, string key)
        {
            DebugOutput.OutputMethod("GetURLFor", $"{what} {key}");
            if (TargetConfiguration.Configuration.JiraName == null) return "";
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
            if (jiraDetails == null) return "";
            // https://proactionuk.ent.cgi.com/jira/
            var url = jiraDetails.Url;
            var projectKey = TargetConfiguration.Configuration.JiraName;
            switch(what.ToLower())
            {
                case "assignee":
                {
                    url = url + "rest/api/2/issue/" + key + "/assignee";
                    break;
                }
                default:
                {
                    DebugOutput.Log($"I am not sure what url the is for?");
                    return "";
                }
                case "comment":
                case "comments":
                {
                    url = url + "rest/api/2/issue/" + key + "/comment";
                    break;
                }
                case "description":
                {
                    url = url + "rest/api/2/issue/" + key;
                    break;
                }
                case "execution":
                {
                    url = url + "rest/raven/1.0/import/execution?projectKey=" + projectKey;
                    break;
                }
                case "import":
                {
                    url = url + "rest/raven/1.0/import/execution";
                    break;
                }
                case "testexec":
                {
                    url = url + "rest/raven/1.0/api/testexec/" + key + "/test";
                    break;
                }
                case "testrun":
                {
                    url = url + "rest/raven/2.0/api/testrun/" + key;
                    break;
                }
                case "transition":
                case "transitions":
                {
                    url = url + "rest/api/2/issue/" + key + "/transitions";
                    break;
                }
            }
            return url;
        }


        /// <summary>
        /// Quick Boolean check if the ISSUE Exists in Jira
        /// </summary>
        /// <param name="issueKey"></param>
        /// <returns></returns>
        public static bool DoesIssueExistInJira(string issueKey)
        {
            DebugOutput.OutputMethod("DoesIssueExistInJira", $"{issueKey} ");
            var model = GetJiraModelFromIssue(issueKey);
            if (model == null) return false;
            return true;
        }


        /// <summary>
        /// Is the issue in a state in Jira where it can be updated
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsIssueEditable(Model.Root model)
        {
            DebugOutput.OutputMethod("IsIssueEditable", $" ");
            return true;
            // if (model.fields != null)
            // {
            //     if (model.fields.status != null)
            //     {
            //         var status = model.fields.status.name;
            //         DebugOutput.Log($"WE HAVE GOTTEN THE STATUS FROM API of {status}");
            //         if (status != null)
            //         {
            //             var edit = Jira.GetActionFromStatus(status);
            //             if (edit == "edit") return true;
            //         }
            //         DebugOutput.Log($"NO STATUS?");
            //     }
            //     DebugOutput.Log($"NO model.fields.status");
            // }
            // DebugOutput.Log($"To get here something went wrong with the getting if the issue is editable!");
            // return false;
        }


        public static bool IsJiraAndATFTheSame(string jiraTestKey, Model.Root model)
        {
            DebugOutput.OutputMethod("IsJiraAndATFTheSame", $"{jiraTestKey} ");
            TargetJiraConfiguration.JiraConfiguration? jiraDetails = null;
            var stepsFromJira = "";
            if (TargetConfiguration.Configuration.JiraName != null)
            {
                jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails == null) return false;
                DebugOutput.Log($"Do we have to sync?");
                if (jiraDetails.Sync == false)
                {
                    DebugOutput.Log($"Actually i don't care if they synced -its in the jira configuration I don't care !");
                    return false;
                }
                    
                DebugOutput.Log($"Looking at steps!");
                if (model.fields != null)
                {
                    if (jiraDetails.Authorization == "cloud")
                    {
                        // on the cloud Jira does not contain the steps!
                        // authenticate with XRAY

                        // we need the id from Jira!?
                        var issueId = model.id;
                        DebugOutput.Log($"The JIRA ID is {issueId}");
                        var query = $"{{ getTest(issueId: \"{issueId}\") {{ gherkin }} }}";
                        var token = GraphQLUtil.GetCurrentJWTTokenForXRAY();
                        if (token == null) token = GraphQLUtil.GetNewJWTTokenForXRAY();
                        if (token == null)
                        {
                            DebugOutput.Log($"NO TOKEN AVAILABLE JWT");
                            return false;
                        }
                        var back = CmdUtil.ExecuteDotnet("./CommunicationXrayCloud/XrayCloudCommunication.csproj", $"\"GetGherkin\" \"{token}\" \"{issueId}\"").Trim();
                        DebugOutput.Log($"The response is {back}");
                        if (back == "JWT ERROR" || back == null || back == "")
                        {
                            token = GraphQLUtil.GetNewJWTTokenForXRAY();
                            if (token == null)
                            {
                                DebugOutput.Log($"NO token");
                                return false;
                            }
                            back = CmdUtil.ExecuteDotnet("./CommunicationXrayCloud/XrayCloudCommunication.csproj", $"\"GetGherkin\" \"{token}\" \"{issueId}\"").Trim();
                            DebugOutput.Log($"The response is {back}");
                            if (back == null || back == "")
                            {
                                DebugOutput.Log($"Failed to get the response from XRAY - second time of asking!");
                                return false;
                            }
                            if (back.ToLower().Contains("error"))
                            {
                                DebugOutput.Log($"ERROR RETURNED FROM ATF360 XRAY GraphQL Interface");
                                return false;
                            }
                        }
                        // Convert the response into a GraphQL Model
                        var graphModel = GraphQLUtil.GetGraphQLModel(back);
                        if (graphModel == null)
                        {
                            DebugOutput.Log($"Failed to convert the response from XRAY to a model");
                            return false;
                        }
                        if (graphModel.data != null)
                        {
                            if (graphModel.data.getTest != null)
                            {
                                stepsFromJira = graphModel.data.getTest.gherkin;
                            }
                        }
                        if (stepsFromJira == "")
                        {
                            DebugOutput.Log($"Somewthign wrong with steps from jira");
                            return false;
                        }
                    }
                    if (jiraDetails.Authorization == "basic")
                    {
                        stepsFromJira = model.fields.customfield_10214;
                    }
                }
            }
            // Check the text in Jira is same as text in ATF
            var text = GetAlLSteps(jiraTestKey);
            bool sameSteps = false;
            if (stepsFromJira != null)
            {
                if (text.Trim() == stepsFromJira.Trim())
                {
                    sameSteps = true;
                }           
            }
            return sameSteps;

        }


        /// <summary>
        /// Make a json file of all steps in a scenario
        /// </summary>
        /// <param name="scenarioTitle"></param>
        /// <returns></returns>
        public static string GetAlLSteps(string scenarioTitle)
        {
            DebugOutput.OutputMethod("GetAlLSteps", $"{scenarioTitle} ");
            var returnString = "\n";
            string? currentBDD = "GIVEN";
            foreach (var scenario in AllScenarios)
            {
                if (scenario.Scenario == scenarioTitle)
                {
                    foreach (var step in scenario.Steps)
                    {
                        var typeOfBDD = step.Substring(0,5);
                        typeOfBDD = typeOfBDD.Trim().ToUpper();
                        if (typeOfBDD != currentBDD)
                        {
                            if (typeOfBDD == "GIVEN" && currentBDD == "WHEN") returnString = returnString  + "\n";;
                            if (typeOfBDD == "GIVEN" && currentBDD == "THEN") returnString = returnString  + "\n";;
                            if (typeOfBDD == "WHEN" && currentBDD == "THEN") returnString = returnString  + "\n";
                            currentBDD = typeOfBDD;
                        }
                        returnString = returnString  + step + "\n";
                    }                    
                }
            }
            return returnString;
        }


        /// <summary>
        /// Upload attachment to JIRA Issue
        /// </summary>
        /// <param name="issueKey"></param>
        /// <returns>true if uploaded successfully</returns>

        public static async Task<bool> UploadAttachment(string issueKey, string file)
        {
            DebugOutput.OutputMethod("UploadAttachment", $"{issueKey} {file}");
            if (TargetConfiguration.Configuration.JiraName == null)
            return false;
            var jira = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
            if (jira != null)
            {
                var jiraName = TargetConfiguration.Configuration.JiraName;
                if (jira != null)
                {
                    var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
                    if (jiraDetails == null) return false;
                    var url = jiraDetails.Url + "rest/api/2/issue/" + issueKey + "/attachments";
                    switch(jiraDetails.Authorization.ToLower())
                    {
                        case "token":
                        {
                            var response = await APIUtil.PostZipFileWithToken(url, file);
                            if (response.IsSuccessStatusCode)
                            {
                                DebugOutput.Log($"TOKEN SUCCESS CODE RECEIVED {(int)response.StatusCode}");
                                return true;
                            }
                            break;
                        }
                        case "basic":
                        {
                            var basicModel = Core.Jira.UsingJiraBasicCredentials.ReadJiraCredentials();
                            if (basicModel != null)
                            {
                                if (basicModel.username != null && basicModel.password != null)
                                {
                                    var response = await APIUtil.PostZipFileWithWindowsAuth(url, file, issueKey, "default", basicModel.username, basicModel.password);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        DebugOutput.Log($"WINDOWS AUTH SUCCESS CODE RECEIVED {(int)response.StatusCode}");
                                        return true;
                                    }
                                }
                            }
                            DebugOutput.Log($"For basic you need username and password!");
                            break;
                        }
                        default:
                        {
                            DebugOutput.Log($"I do not know of authorisation to jira as {jiraDetails.Authorization.ToLower()}");
                            break;
                        }
                    }
                    DebugOutput.Log($"FAILED TO UPLOAD!");
                }
            }
            return false;
        }


        /// <summary>
        /// Change the BDD in Jira 
        /// </summary>
        /// <param name="testExecutionKey"></param>
        /// <returns></returns>
        public static bool UpdateBDDInJira(string testKey)
        {
            DebugOutput.OutputMethod("UpdateBDDInJira", $"{testKey} ");
            var text = GetAlLSteps(testKey);
            if (TargetConfiguration.Configuration.JiraName == null) return false;   
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
            if (jiraDetails == null) return false;
            if (jiraDetails.Authorization.ToLower() != "cloud")
            {
                var body = new { fields = new { customfield_10214 = text } };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var url = Jira.GetURLFor("description", testKey);
                if (url == null) return false;
                DebugOutput.Log($"URL to test is {url}");
                if (jiraDetails == null) return false;
                if (jiraDetails.Authorization == "token")
                {                
                    var response = APIUtil.PutContentWithToken(url, content).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        DebugOutput.JiraOutput($"{(int)response.StatusCode} FAILED TO CHANGE!!");
                        return false;
                    } 
                    return true;
                }
            }
            if (jiraDetails.Authorization == "cloud")
            {      
                DebugOutput.Log($"CLOUD XRAY!");
                var issueId = GetJiraModelFromIssue(testKey)?.id;
                if (issueId == null) return false;
                DebugOutput.Log($"The issue id is {issueId}");
                DebugOutput.Log($"The GHERKIN is {text}");
                var temp1 = text.Replace("\n", "<NEWLINE>");
                DebugOutput.Log($"The GHERKIN is {temp1}");
                var temp2 = temp1.Replace("\"", "<doublequotes>");
                DebugOutput.Log($"The GHERKIN is {temp2}");
                var temp3 = temp2.Replace("\r", "<CARRIAGERETURN>");
                DebugOutput.Log($"The GHERKIN is {temp3}");
                var temp4 = temp3.Replace("\t", "<TAB>");
                DebugOutput.Log($"The GHERKIN is {temp4}");

                var temp5 = temp4.Replace("<NEWLINE>", "\\n");
                DebugOutput.Log($"The GHERKIN is {temp5}");
                var temp6 = temp5.Replace("<doublequotes>", "\\\"");
                DebugOutput.Log($"The GHERKIN is {temp6}");
                var temp7 = temp6.Replace("<CARRIAGERETURN>", "\\r");
                DebugOutput.Log($"The GHERKIN is {temp7}");
                var temp8 = temp7.Replace("<TAB>", "\\t");
                DebugOutput.Log($"The FINAL GHERKIN is {temp8}");       

                var finalGherkin = temp8;
                // var query = $"mutation {{ updateGherkinTestDefinition(issueId: \"{issueId}\", gherkin: \"{finalGherkin}\") {{ issueId, gherkin }} }}";
                var token = GraphQLUtil.GetCurrentJWTTokenForXRAY();
                if (token == null) token = GraphQLUtil.GetNewJWTTokenForXRAY();
                if (token == null) return false;
                string argument = $"\"SyncBDD\" \"{token}\" \"{issueId}\" \"{finalGherkin}\"";
                string projectLocation = "./CommunicationXrayCloud/XrayCloudCommunication.csproj";
                var back = CmdUtil.ExecuteDotnet(projectLocation, argument);
                if (back == null || back == "")
                {
                    DebugOutput.Log($"Second try!");
                    token = GraphQLUtil.GetNewJWTTokenForXRAY();
                    if (token == null) return false;
                    back = CmdUtil.ExecuteDotnet(projectLocation, argument);
                    if (back == null || back == "")
                    {
                        DebugOutput.Log($"Even second time failure!");
                        return false;
                    }
                }
                DebugOutput.Log($"it is all done!");
                return true;
            }
            DebugOutput.Log($"lost here!");
            return false;
        }

        public static bool UpdateTestStatusInTestExecution(string testExecutionKey, string testResults)
        {
            DebugOutput.OutputMethod("UpdateTestStatusInTestExecution", $"{testExecutionKey}  {testResults}");
            var token = GraphQLUtil.GetCurrentJWTTokenForXRAY();
            if (token == null) token = GraphQLUtil.GetNewJWTTokenForXRAY();
            try
            {
                var back = CmdUtil.ExecuteDotnet("./CommunicationXrayCloud/XrayCloudCommunication.csproj", $"\"UploadTestExecutionResults\" \"{token}\" \"{testExecutionKey}\" \"{testResults}\"").Trim();
                if (back == "" || back == null || back.ToLower().Contains("error"))
                {
                    token = GraphQLUtil.GetNewJWTTokenForXRAY();
                    if (token == null) return false;
                    back = CmdUtil.ExecuteDotnet("./CommunicationXrayCloud/XrayCloudCommunication.csproj", $"\"UploadTestExecutionResults\" \"{token}\" \"{testExecutionKey}\" \"{testResults}\"").Trim();
                    if (back == "" || back == null || back.ToLower().Contains("error")) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to update test execution {ex}");
                return false;
            }
        }

        /// <summary>
        /// Update the status of a test within a test execution
        /// </summary>
        /// <param name="testExecutionKey"></param>
        /// <param name="testKey"></param>
        /// <param name="status"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static bool UpdateTestStatusInTestExecution(string testExecutionKey, string testKey, string status, string comment)
        {
            DebugOutput.OutputMethod("UpdateTestStatusInTestExecution", $"{testExecutionKey}  {testKey} {status} {comment}");
            var testResultsJson = Core.Jira.Using.UsingXRayTestExecutionModel.ConvertParametersInToFullModelJson(testExecutionKey, testKey, status, comment);
            if (testResultsJson == null)
            {
                DebugOutput.JiraOutput("Failed to create the content to update Jira to pass/fail");
                return false;
            }
            var url = GetURLFor("import", "");
            var content = new StringContent(testResultsJson, Encoding.UTF8, "application/json");
            var response = APIUtil.PostContentWithToken(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                DebugOutput.JiraOutput($"Failed to update exection {testExecutionKey} {(int)response.StatusCode} {url} {testResultsJson}");
                return false;
            }
            return true;
        }


        /// <summary>
        /// Add a new scenario to our list
        /// </summary>
        /// <param name="scenarioTitle"></param>
        public static void AddScenario(string scenarioTitle)
        {
            var x = new JiraTestModel();
            x.Scenario = scenarioTitle;
            AllScenarios.Add(x);
        }


        /// <summary>
        /// Add steps to an already created scenario
        /// </summary>
        /// <param name="scenarioTitle"></param>
        /// <param name="step"></param>
        public static void AddStepToScenario(string scenarioTitle, string step)
        {
            foreach (var scenario in AllScenarios)
            {
                if (scenario.Scenario == scenarioTitle)
                {
                    var newSteps = new List<string>();
                    scenario.Steps.Add(step);
                }
            }
        }

    }
}
