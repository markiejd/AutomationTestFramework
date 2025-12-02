
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace XrayCloudCommunication
{
    class Program
    {

        private static string URL = "https://xray.cloud.getxray.app/api/v2/graphql";
        private static string RestURL = "https://xray.cloud.getxray.app/api/v2";

        static async Task Main(string[] args)
        { 
            if (args.Length < 2)
            {
                return;
            } 

            string action = args[0];
            // want to send the rest of the parameters and use 0 to find what we doing
            string[] parameters = args.Length > 1 ? args[1..] : Array.Empty<string>();

            //repsonse from methods
            string response = "";

            switch (action.ToLower())
            {
                case "syncbdd":
                    response = await SyncIssueIdBDD(parameters);
                    break;
                case "getgherkin":
                    response = await GetGherkinIssueId(parameters);
                    break;
                case "createtestexecution":
                    response = await CreateANewTestExecution(parameters);
                    break;
                case "addtesttotestexecution":
                    response = await AddTestToATestExecution(parameters);
                    break;
                case "gettestrunid":
                    response = await GetTestRunIdForTestInTestExecution(parameters);
                    break;
                case "gettestrunbyid":
                    response = await GetTestRunById(parameters);
                    break;
                case "updatetestrun":
                    response = await UpdateTestRun(parameters);
                    break;
                case "gettestexecutionbyid":
                    response = await GetTestExecutionById(parameters);
                    break;
                case "getschema":
                    response = await QuerySchema(parameters);
                    break;
                case "getschematype":
                    response = await QuerySchemaType(parameters);
                    break;
                case "test1":
                    response = await Test1(parameters);
                    break;  
                case "addevidencetotestrun":
                    response = await AddEvidenceToTestRun(parameters);
                    break;
                case "uploadtestexecutionresults":
                    response = await UploadTestExecutionResults(parameters);
                    break;
                // case "updatetestexecution":
                //     response = await UpdateTestExecution(parameters);
                //     break;

                // case "mutation":
                //     await ExecuteMutation(parameters);
                //     break;
                default:
                    return;
            }
            Console.WriteLine(response);
        }


        /// <summary>
        /// Upload a funny file via REST API!
        /// </summary>
        /// <param name="token"></param>
        /// <param name="testExecutionKey"></param>
        /// <param name="testResults"></param>
        /// <returns></returns>
        public static async Task<string> UploadTestExecutionResults(string[] parameters)
        {
            // if (parameters.Length != 5)
            // {
            //     return "";
            // }            
            
            var client = new HttpClient();
            string token = parameters[0];
            string testExecutionKey = parameters[1];
            string testResults = parameters[2];
            // the test results comprised of multiple tests delimited by | within each test is its testKey, status and comment
            // e.g. HUX-1316,PASSED,This is a comment|HUX-1317,FAILED,This is a comment
            // so we need to split the test results by | and then by , to get the testKey, status and comment
            var tests = testResults.Split('|');
            var listOfTestKeys = new List<string>();
            var listOfTestStatus = new List<string>();
            var listOfComments = new List<string>();
            var listOfEvidenceData = new List<string>();
            var listOfEvidenceFileName = new List<string>();
            var listOfEvidenceContentType = new List<string>();
            foreach (var test in tests)
            {
                var testDetails = test.Split(',');
                var testKey = testDetails[0];
                listOfTestKeys.Add(testKey);
                var status = testDetails[1];
                listOfTestStatus.Add(status);
                if (status.ToLower() != "failed")
                {
                    var comment = testDetails[2];
                    listOfComments.Add(comment);
                    listOfEvidenceData.Add("");
                    listOfEvidenceFileName.Add("");
                    listOfEvidenceContentType.Add("");
                }
                else
                {
                    // failed we need to populate evidence
                    var testResult = testDetails[2];
                    var brokenUpCommentForFailure = testResult.Split('£');
                    if (brokenUpCommentForFailure.Count() == 4)
                    {
                        // should have the comment 0 data 1 filename 2 contenttype 3
                        listOfComments.Add(brokenUpCommentForFailure[0]);
                        listOfEvidenceData.Add(brokenUpCommentForFailure[1]);
                        listOfEvidenceFileName.Add(brokenUpCommentForFailure[2]);
                        listOfEvidenceContentType.Add(brokenUpCommentForFailure[3]);
                    } 
                    else
                    {
                        var comment = testDetails[2];
                        listOfComments.Add(comment);
                        listOfEvidenceData.Add("");
                        listOfEvidenceFileName.Add("");
                        listOfEvidenceContentType.Add("");
                    }
                }
            }

            string jsonPayload = "";
            
            var jsonPayloadstart = @$"
            {{
                ""testExecutionKey"": ""{testExecutionKey}"",
                ""tests"": 
                    [";

            jsonPayload = jsonPayloadstart;
            
            foreach(var testKey in listOfTestKeys)
            {
                var index = listOfTestKeys.IndexOf(testKey);
                var status = listOfTestStatus[index];
                var comment = listOfComments[index];
                var jsonPayloadTests = @$"
                        {{
                            ""testKey"": ""{testKey}"",
                            ""status"": ""{status}"",
                            ""comment"": ""{comment}""";
                jsonPayload = jsonPayload + jsonPayloadTests;
                if (status.ToLower() != "failed") jsonPayload = jsonPayload + @$"
                        }}";
                if (status.ToLower() != "passed") 
                {
                    var base64String = OSGetConvertFileToBase64(listOfEvidenceFileName[index]);
                    var OSfileName = listOfEvidenceFileName[index];
                    // the OSfileName contains path as well as file name, just want the file name
                    var OSfileNameSplit = OSfileName.Split('\\');
                    var OSfileNameSplitLength = OSfileNameSplit.Length;
                    var OSfileNameSplitIndex = OSfileNameSplitLength - 1;
                    var fileName = OSfileNameSplit[OSfileNameSplitIndex];

                    jsonPayload = jsonPayload + @$",
                            ""evidences"": 
                            [
                                {{
                                    ""data"": ""{base64String}"",
                                    ""filename"": ""{fileName}"",
                                    ""contentType"": ""{listOfEvidenceContentType[index]}""
                                }}
                            ]
                        }}";
                }
                if (index < listOfTestKeys.Count - 1) jsonPayload = jsonPayload + ",";
            }

            jsonPayload = jsonPayload + @$"
                    ]
            }}";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var url = RestURL + "/import/execution";
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                return "Test results uploaded successfully.";
            }
            else
            {
                return $"Error to upload test results. Status code: {response.StatusCode}";
            }
        }

        
        private static string? OSGetConvertFileToBase64(string fileNameAndLocation)
        {      
            try
            {
                byte[] imageBytes = File.ReadAllBytes(fileNameAndLocation);
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting image to Base64: {ex.Message}");
                return null;
            }
        }


        public static async Task<string> AddEvidenceToTestRun(string[] parameters)
        {
            // need 3 parameters to work - any more or any less - we out!
            if (parameters.Length != 5)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test run id and the 3rd is the evidence, so any more than 3, we out too!
            string token = parameters[0];
            string testRunId = parameters[1];
            string fileName = parameters[2];
            string mimeType = parameters[3];
            string data = parameters[4];

            var query = $@"
            mutation 
            {{
                addEvidenceToTestRun
                (
                    id: ""{testRunId}"",
                    evidence: 
                    [{{
                        fileName: ""{fileName}"",
                        mimeType: ""{mimeType}"",
                        data: ""{data}""
                    }}]
                ) 
                {{
                    addedEvidence
                    warnings
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        public static async Task<string> Test1(string[] parameters)
        {
            if (parameters.Length != 1)
            {
                return "";
            }
            string token = parameters[0];var query = $@"
            query 
            {{
                __type(name: ""Mutation"") 
                {{
                    fields 
                    {{
                        name
                        args 
                        {{
                            name
                            type 
                            {{
                                name
                                kind
                                ofType 
                                {{
                                    name
                                    kind
                                }}
                            }}
                        }}
                        type 
                        {{
                            name
                            kind
                            ofType
                            {{
                                name
                                kind
                            }}
                        }}
                    }}
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        public static async Task<string> QuerySchemaType(string[] parameters)
        {
            if (parameters.Length != 2)
            {
                return "";
            }
            string token = parameters[0];
            string type = parameters[1];
            var query = $@"
            query 
            {{
                __type(name: ""{type}"") 
                {{
                    name
                    kind
                    description
                    fields 
                    {{
                        name
                    }}
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        public static async Task<string> QuerySchema(string[] parameters)
        {
            if (parameters.Length != 1)
            {
                return "";
            }
            string token = parameters[0];
            var query = $@"
            query 
            {{
                __schema 
                {{
                    types 
                    {{
                        name
                        kind
                        description
                        fields 
                        {{
                            name
                        }}
                    }}
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        public static async Task<string> GetTestExecutionById(string[] parameters)
        {
            // need 2 parameters to work - any more or any less - we out!
            if (parameters.Length != 2)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test execution id, so any more than 2, we out too!
            string token = parameters[0];
            string testExecutionId = parameters[1];
            var query = $@"
            query 
            {{
                getTestExecution(issueId: ""{testExecutionId}"") 
                {{
                    issueId
                    tests(limit: 100)
                    {{
                        total
                        start
                        limit
                        results 
                        {{
                            issueId
                            testType
                            {{
                                name
                            }}
                        }}
                    }}
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        public static async Task<string> GetTestRunById(string[] parameters)
        {
            // need 2 parameters to work - any more or any less - we out!
            if (parameters.Length != 2)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test run id, so any more than 2, we out too!
            string token = parameters[0];
            string testRunId = parameters[1];
            var query = $@"
            query 
            {{
                getTestRunById(id: ""{testRunId}"") 
                {{
                    id
                    status {{
                        name
                        color
                        description
                    }}
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        public static async Task<string> UpdateTestRun(string[] parameters)
        {
            // need 3 parameters to work - any more or any less - we out!
            if (parameters.Length != 3)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test run id and the 3rd is the status, so any more than 3, we out too!
            string token = parameters[0];
            string testRunId = parameters[1];
            string status = parameters[2];
            var query = $@"
            mutation 
            {{
                updateTestRunStatus(id: ""{testRunId}"", status: ""{status}"")
            }}";
            var response = await PostCotent(token, query);
            return response;
        }

        /// <summary>
        /// Get the Test Run ID for a given test Id in a given test execution ID
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<string> GetTestRunIdForTestInTestExecution(string[] parameters)
        {
            // need 2 parameters to work - any more or any less - we out!
            if (parameters.Length != 3)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test execution id, so any more than 2, we out too!
            string token = parameters[0];
            string testExecutionId = parameters[1];
            string testId = parameters[2];
            var query = $@"
            query 
            {{
                getTestRun( testIssueId: ""{testId}"", testExecIssueId: ""{testExecutionId}"") 
                {{
                    id                    
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }



        /// <summary>
        /// Create a brand new test execution
        /// </summary>
        /// <param name="token"></param>
        /// <param name="summary"></param>
        /// <param name="description"></param>
        /// <param name="projectKey"></param>
        /// <param name="testEnvironment"></param>
        /// <returns></returns>
        public static async Task<string> CreateANewTestExecution(string[] parameters)
        {
            // need 5 parameters to work - any more or any less - we out!
            if (parameters.Length != 5)
            {
                return "error - need 5 parameters to work - any more or any less - we out!";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test plan id and the 3rd is the test execution name, so any more than 3, we out too!
            string token = parameters[0];
            string summary = parameters[1];
            string description = parameters[2];
            string projectKey = parameters[3];
            // The following can be "" - nulls only
            string testEnvironment = parameters[4]; // this is the JIRA test Environment "HEX_Dev" 
            // string subEnvironment = parameters[5]; // the string of the custom field created by Harpreet "Dev"
            // string customfield_10243Value = parameters[6];  // this is a custom field created by Harpreet "10218"
            
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(summary) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(projectKey))
            {
                return "error - we have null or empty parameters - we out!";
            }

            var query = $@"
                mutation 
                {{
                    createTestExecution(
                        testEnvironments: [""{testEnvironment}""],
                        jira: {{
                            fields: {{
                                summary: ""{summary}"",
                                description: ""{description}"",
                                project: {{ key: ""{projectKey}"" }}
                            }}
                        }}
                    ) 
                    {{
                        testExecution {{
                            issueId
                            jira(fields: [""key""])
                        }}
                        warnings
                        createdTestEnvironments
                    }}
                }}";
            var response = await PostCotent(token, query);
            return response;

        }


        /// <summary>
        /// Add a test to a test execution
        /// Or a comma delimited list of test ids to a test execution
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<string> AddTestToATestExecution(string[] parameters)
        {
            // need 3 parameters to work - any more or any less - we out!
            if (parameters.Length != 3)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the test execution id and the 3rd is the test id, so any more than 3, we out too!
            string token = parameters[0];
            string testExecutionId = parameters[1];
            // testIds can be a comma delimited string of test ids
            string testIds = parameters[2];
            // https://castlejira.atlassian.net/rest/api/2/issue/HUX-2967
            var query = $@"
            mutation 
            {{
                addTestsToTestExecution(
                    issueId: ""{testExecutionId}"",
                    testIssueIds: [""{testIds}""]
                ) 
                {{
                    addedTests
                    warning
                }}
            }}";
            var response = await PostCotent(token, query);
            return response;
        }


        /// <summary>
        /// Get the BDD\Gherkin from XRAY issue ID
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<string> GetGherkinIssueId(string [] parameters)
        {
            // need 2 parameters to work - any more or any less - we out!
            if (parameters.Length != 2)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the issue id, so any more than 2, we out too!
            string token = parameters[0];
            string issueId = parameters[1];
            var query = $@"
            query 
            {{
                getTest(
                    issueId: ""{issueId}""
                ) 
                {{
                    gherkin
                }}
            }}";

            var response = await PostCotent(token, query);
            return response;
        }


        /// <summary>
        /// Overwrite Cucumber in Xray
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<string> SyncIssueIdBDD(string[] parameters)
        {
            // need 3 parameters to work - any more or any less - we out!
            if (parameters.Length != 3)
            {
                return "";
            }
            // set the parameters as 1st parameter is the token, 2nd is the issue id and the 3rd is the BDD text, so any more than 3, we out too!
            string token = parameters[0];
            string issueId = parameters[1];
            string bddText = parameters[2];
            var query = $@"
            mutation 
            {{
                updateGherkinTestDefinition(
                    issueId: ""{issueId}"",
                    gherkin: ""{bddText.Replace("\"", "\\\"")}""
                ) 
                {{
                    issueId,
                    gherkin
                }}
            }}";

            var response = await PostCotent(token, query);
            return response;
        }


        /// <summary>
        /// This is the most important part - the actual push of the query - they should all follow same process! 
        /// Thats what GraphQL is all about!
        /// </summary>
        /// <param name="token"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private static async Task<string> PostCotent(string token, string query)
        {
            // Console.WriteLine($"Using ATF GraphQL with query: {query}");
            var content = new StringContent(JsonConvert.SerializeObject(new { query }), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.PostAsync(URL, content);
                if (!response.IsSuccessStatusCode)
                {
                    // Handle error
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"error Using ATF GraphQL: {errorContent}");
                }
                // show response code
                // Console.WriteLine($"Response Status Code: {response.StatusCode}");

                // Read the response content
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }




    }
}