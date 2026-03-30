using Core.Configuration;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;
using Core;
using Core.FileIO;
using Core.Transformations;
using AppXAPI.Models;
using AppXAPI;
using System.Threading.Tasks;

namespace Generic.Steps
{
    // Class contains step definitions used by the test framework for API interactions.
    // Methods are intentionally simple wrappers around APIUtil with logging and basic failure handling.
    [Binding]
    public class APISteps : StepsBase
    {
        public APISteps(IStepHelpers helpers) : base(helpers)
        {
        }

        /// <summary>
        /// Central failure helper that logs and reports a failure for the current procedure.
        /// Returns the provided flag to allow concise returns from step methods.
        /// </summary>
        private bool Failed(string proc, string message = "***** FAILURE ******", bool flag = true)
        {
            DebugOutput.Log(message);
            CombinedSteps.Failure(proc);
            return flag;
        }

        [When(@"I ""([^""]*)"" Payload ""([^""]*)"" At The ""([^""]*)"" Endpoint")]
        public bool WhenIPayloadAtTheEndpoint(string action, string payload, string endPointURL)
        {
            string proc = $"When I \"{action}\" Payload \"{payload}\" At The \"{endPointURL}\" Endpoint";
            endPointURL = StringValues.TextReplacementService(endPointURL);
            proc = $"When I \"{action}\" Payload \"{payload}\" At The \"{endPointURL}\" Endpoint";

            if (CombinedSteps.OutputProc(proc))
            {            
                // single quotes are used in the FF to make it easy to read and avoid escaping issues, but the APIUtil expects double quotes for JSON - so replace them here before sending to the helper method.
                payload = payload.Replace("'", "\"");
                DebugOutput.Log($"Payload after replacement: {payload}");
                switch(action.ToLower())
                {
                    case "post":
                        var postResponse = APIUtil.Post(endPointURL, payload, action.ToLower(), false).Result;
                        break;
                    case "patch":
                        var patchResponse = APIUtil.Post(endPointURL, payload, action.ToLower(), false).Result; // Note: using Post helper for PATCH - consider a dedicated Patch helper if needed.
                        break;
                    case "get":
                        return Failed(proc, "GET method does not support a body - use the step without body parameter for GET requests.");
                    default:
                        return Failed(proc, $"Unsupported *YET* HTTP method '{action}'");
                }
                return true;
            }
            CombinedSteps.Failure(proc);
            return false;
        }

        [When(@"I ""([^""]*)"" The ""([^""]*)"" Endpoint")]
        public async Task<bool> WhenITheEndpoint(string action, string endPointURL)
        {
            string proc = $"When I \"{action}\" The \"{endPointURL}\" Endpoint";         
            endPointURL = StringValues.TextReplacementService(endPointURL);
            proc = $"When I \"{action}\" The \"{endPointURL}\" Endpoint";         
            if (CombinedSteps.OutputProc(proc))
            {
                try
                {
                    switch(action.ToLower())
                    {
                        case "get":
                            await APIUtil.Get(endPointURL, action.ToLower());
                            break;
                        case "post":
                            return Failed (proc, "POST method requires a body - use the step that includes the body parameter for POST requests.");
                        case "patch":
                            return Failed(proc, "PATCH method requires a body - use the step that includes the body parameter for PATCH requests.");
                        default:
                            return Failed(proc, $"Unsupported *YET* HTTP method '{action}'");
                    }
                }
                catch (Exception ex)
                {
                    return Failed(proc, $"Exception during API call: {ex.Message}");
                }
                return true;
            }
            CombinedSteps.Failure(proc);
            return false;
        }

        // [When(@"I ""([^""]*)"" The ""([^""]*)"" Endpoint With The Body ""([^""]*)""")]
        // [When(@"I ""([^""]*)"" The ""([^""]*)"" Endpoint With The Payload ""([^""]*)""")]
        // public bool WhenITheEndpointWithTheBody(string action, string endPointURL, string body)
        // {
        //     string proc = $"When I \"{action}\" The \"{endPointURL}\" Endpoint With The Payload \"{body}\"";       
        //     endPointURL = StringValues.TextReplacementService(endPointURL);
        //     proc = $"When I \"{action}\" The \"{endPointURL}\" Endpoint";         
                
        //     if (CombinedSteps.OutputProc(proc))
        //     {
        //         switch(action.ToLower())
        //         {
        //             case "post":
        //                 var postResponse = APIUtil.Post(endPointURL, body, action.ToLower(), false).Result;
        //                 break;
        //             case "patch":
        //                 var patchResponse = APIUtil.Post(endPointURL, body, action.ToLower(), false).Result; // Note: using Post helper for PATCH - consider a dedicated Patch helper if needed.
        //                 break;
        //             case "get":
        //                 return Failed(proc, "GET method does not support a body - use the step without body parameter for GET requests.");
        //             default:
        //                 return Failed(proc, $"Unsupported *YET* HTTP method '{action}'");
        //         }
        //         return true;
        //     }
        //     CombinedSteps.Failure(proc);
        //     return false;
        // }






        [Then(@"I Receive A ""([^""]*)"" Status Code")]
        public bool ThenIReceiveAStatusCode(string statusCode)
        {
            string proc = $"Then I Receive A \"{statusCode}\" Status Code";

            // convert the string statusCode to a status code and compare to the cached APIResponse status code - if can not convert fail here.
            if (!int.TryParse(statusCode, out int expectedStatusCode))
            {
                return Failed(proc, $"Failed to parse expected status code '{statusCode}' as an integer.    Make sure the status code in the feature file is a valid integer.");
            }
                            
            if (CombinedSteps.OutputProc(proc))
            {
                if (APIResponse.fullResponse == null) return Failed(proc, "Failed to read response! Make sure you have a previous communication step that populates APIResponse.fullResponse before this validation step.");
                var statusCodeReceived = (int)APIResponse.fullResponse.StatusCode;
                if (statusCodeReceived == expectedStatusCode) return true;
                DebugOutput.Log($"Did not match! Received {statusCodeReceived}");
                return Failed(proc, "Did not match!");
            }
            CombinedSteps.Failure(proc);
            return false;
        }

        [Then(@"The Response Body Is Equal To ""([^""]*)""")]
        public bool ThenTheResponseBodyIsEqualTo(string responseBodyExpected)
        {
            string proc = $"Then The Response Body Is Equal To \"{responseBodyExpected}\"";
                
            if (CombinedSteps.OutputProc(proc))
            {
                if (APIResponse.fullResponse == null) return Failed(proc, "Failed to read response! Make sure you have a previous communication step that populates APIResponse.fullResponse before this validation step.");
                var responseBody = APIResponse.fullResponse.Content.ReadAsStringAsync().Result;
                if (responseBody == responseBodyExpected) return true;
                DebugOutput.Log($"Did not match! Received {responseBody} - but those double quotes are a nightmare to work with in the feature file, so consider using single quotes in the feature and replacing them here for a more readable feature file.");
                // replace the response body double quotes with single quotes for easier comparison if the feature file uses single quotes to avoid escaping issues - this is a bit of a hack but makes the feature files more readable.
                responseBody = responseBody.Replace("\"", "'");
                DebugOutput.Log($"After replacement, comparing {responseBody} to {responseBodyExpected}");
                if (responseBody == responseBodyExpected) return true;
                return Failed(proc, "Did not match!");
            }
            CombinedSteps.Failure(proc);
            return false;
        }











        /// <summary>
        /// Verifies the cached APIResponse status code equals the supplied expected value.
        /// </summary>
        [Then(@"Response Status Code Is Equal To ""(.*)""")]
        public bool ThenResponseStatusCodeIsEqualTo(string statusCode)
        {
            var proc = $"Then Response Status Code Is Equal To '{statusCode}'";
            if (CombinedSteps.OutputProc(proc))
            {
                if (APIResponse.fullResponse == null) return Failed(proc, "Failed to read response!");
                var statusCodeReceived = (int)APIResponse.fullResponse.StatusCode;
                if (statusCodeReceived.ToString() == statusCode) return true;
                DebugOutput.Log($"Did not match! Received {statusCodeReceived}");
                return Failed(proc, "Did not match!");
            }
            return false;            
        }

        /// <summary>
        /// Performs a GET using an active JSESSIONID and handles redirection JSESSIONID retrieval.
        /// </summary>
        [When(@"I With Active JSession Get From URL ""(.*)""")]
        public async Task<bool> WhenIWithActiveJSessionGetFromURL(string url)
        {
            string proc = $"When I With Active JSession Get From URL {url}";
            if (CombinedSteps.OutputProc(proc))
            {
                // Retrieve session id
                var JSESSIONID = await APIUtil.GetJsessionId();
                if (JSESSIONID == null) return Failed(proc, "Failed to get JSESSIONID");

                // Retrieve any redirection session id if required
                var redirectionJESSSIONID = await APIUtil.GetRedirectionURL();
                if (redirectionJESSSIONID == null) return Failed(proc, "Failed to get JSESSIONID from redirection");

                // Note: actual GET call is performed elsewhere; this step ensures session acquisition.
                return true;
            }
            return false;
        }

        [Given(@"Session Token")]
        public void GivenSessionToken()
        {
            string proc = $"Given Session Token";
            if (CombinedSteps.OutputProc(proc))
            {
                // Placeholder: session token will be acquired/validated by helper methods if needed.
            }
        }

        /// <summary>
        /// Ensures a redirection URL based on JSESSION is available.
        /// </summary>
        [Given(@"I Have Redirection URL Using JSession")]
        public async Task<bool> GivenIHaveRedirectionURLUsingJSession()
        {
            string proc = $"Given I Have Redirection URL Using JSession";
            if (CombinedSteps.OutputProc(proc))
            {
                var x = await APIUtil.GetRedirectionURL();
                if (x == null) return Failed(proc, "Failed in the redirection !");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Performs a simple GET to the provided URL and logs success/failure.
        /// </summary>
        [When(@"I Get From URL ""(.*)""")]
        public async Task<bool> WhenIGetFromURL(string endPointURL)
        {
            string proc = $"When I Get From URL {endPointURL}";
            endPointURL = StringValues.TextReplacementService(endPointURL);
            proc = $"When I Get From URL {endPointURL}";
            if (CombinedSteps.OutputProc(proc))
            {
                var response = await APIUtil.Get(endPointURL, "get");
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log("Successful GET - JSON file has been created with output (may be empty).");
                    return true;
                }

                DebugOutput.Log($"Unsuccessful response code '{response.StatusCode}' - content: {response.Content}");
            }    
            return Failed(proc);
        }

        /// <summary>
        /// Performs a GET using Windows authentication with supplied credentials.
        /// </summary>
        [When(@"Using Windows Authentication User ""(.*)"" Password ""(.*)"" I Get From URL ""(.*)""")]
        public async Task<bool> WhenUsingWindowsAuthenticationUserPasswordIGetFromURL(string winUser,string winPassword,string endPointURL)
        {
            string proc = $"When I Get From URL {endPointURL} using Windows Authentication with user {winUser}";
            endPointURL = StringValues.TextReplacementService(endPointURL);
            proc = $"When I Get From URL {endPointURL} using Windows Authentication with user {winUser}";
            if (CombinedSteps.OutputProc(proc))
            {
                var response = await APIUtil.GetWithWindowsAuth(endPointURL,"getWithAuth", winUser, winPassword);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log("Successful GET with Windows Auth - JSON file created (may be empty).");
                    return true;
                }
                DebugOutput.Log($"Unsuccessful response code '{response.StatusCode}' - content: {response.Content}");
            }
            return Failed(proc);
        }

        /// <summary>
        /// POSTs a JSON string payload to the specified URL.
        /// </summary>
        [When(@"I Post Json ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPostJsonToURL(string jSonString,string endPointURL)
        {
            endPointURL = StringValues.TextReplacementService(endPointURL);
            string proc = $"When I Post Json {jSonString} To URL {endPointURL}";
            if (CombinedSteps.OutputProc(proc))
            {
                var response = await APIUtil.Post(endPointURL, jSonString, "post", false);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log("Successful POST - JSON file created with output (may be empty).");
                    return true;
                }
                DebugOutput.Log($"Unsuccessful response code '{response.StatusCode}' - content: {response.Content}");
            }
            return Failed(proc);
        }

        [When(@"I Post Json File ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPostJsonFileToURL(string jsonFileLocation,string endPointURL)
        {
            endPointURL = StringValues.TextReplacementService(endPointURL);
            string proc = $"When I Post Json File {jsonFileLocation} To URL {endPointURL}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!FileUtils.OSFileCheck(jsonFileLocation))
                {
                    return Failed(proc, $"Failed to find file {jsonFileLocation}");
                }
                var jSonText = JsonValues.ReadOSJsonFile(jsonFileLocation);
                if (jSonText == null) return Failed(proc, "Read the file, Json part failed!");
                var success =  await WhenIPostJsonToURL(jSonText, endPointURL);
                if (success) return true;
                return Failed(proc, "Failed to post!");
            }
            return false;
        }

        [When(@"I Patch Json ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPatchJsonToURL(string jSonString,string endPointURL)
        {
            endPointURL = StringValues.TextReplacementService(endPointURL);
            string proc = $"When I Patch Json {jSonString} To URL {endPointURL}";

            if (CombinedSteps.OutputProc(proc))
            {
                // Note: APIUtil.Post is used for patching in this codebase; consider a dedicated Patch helper if needed.
                var response = await APIUtil.Post(endPointURL, jSonString, "post", false);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log("Successful PATCH - JSON file created with output (may be empty).");
                    return true;
                }
                DebugOutput.Log($"Unsuccessful response code '{response.StatusCode}' - content: {response.Content}");
            }
            return Failed(proc);
        }

        [When(@"I Patch Json File ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPatchJsonFileToURL(string jsonFileLocation,string endPointURL)
        {
            endPointURL = StringValues.TextReplacementService(endPointURL);
            string proc = $"When I Patch Json File {jsonFileLocation} To URL {endPointURL}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!FileUtils.OSFileCheck(jsonFileLocation))
                {
                    return Failed(proc, $"Failed to find file {jsonFileLocation}");
                }
                var jSonText = JsonValues.ReadOSJsonFile(jsonFileLocation);
                if (jSonText == null) return Failed(proc, "Read the file, Json part failed!");
                var success =  await WhenIPostJsonToURL(jSonText, endPointURL);
                if (success) return true;
                return Failed(proc, "Failed to post!");
            }
            return Failed(proc);
        }

        [Given(@"(.*) Users Ramping Up At (.*) User Per (.*) Second Gap Of (.*) Second Query ""(.*)""")]
        public void GivenUsersRampingUpAtUserPerSecondGapOfSecondQuery(int totalNumberOfUsers,int userSteps,int betweenRamps,int betweenUsers,string query)
        {
            var startTime = DateTime.Now;
            int numberOfUsers = 0;
            DebugOutput.Log($"START TIME {startTime}");
            while (numberOfUsers < totalNumberOfUsers)
            {
                long firstTick = DateTime.Now.Ticks;
                for (int i = 0; i <= numberOfUsers; i++) // this is thread for each user
                {
                    if (betweenUsers > 0 ) Thread.Sleep(betweenUsers * 1000);
                    else Thread.Sleep(1);
                    long secondTick = DateTime.Now.Ticks;
                    _ = APIUtil.MyAsyncMethod(firstTick, secondTick);
                }
                var before = DateTime.Now;
                var ramps = betweenRamps * 1000;
                Thread.Sleep(ramps);
                var after = DateTime.Now;
                DebugOutput.Log($"{numberOfUsers} BEFORE AND AFTER {before} {after}");
                numberOfUsers = numberOfUsers + userSteps;

                // Last ramp - need to wait for this one to complete!
                // Or the test thread completes and output stops!
                if (numberOfUsers >= totalNumberOfUsers)
                {
                    firstTick = DateTime.Now.Ticks;
                    long secondTick = DateTime.Now.Ticks;
                    DebugOutput.Log("AWAITING HERE!");
                    var task = APIUtil.MyAsyncMethod(firstTick, secondTick, query);
                    task.Wait();
                    var result = task.Result;
                    DebugOutput.Log($"AWAITING HERE???? {result}");
                }
            }
            var endTime = DateTime.Now;
            DebugOutput.Log($"END TIME {endTime}");
        }

        [Given(@"(.*) Users Ramping Up at (.*) User Per (.*) Second Query ""(.*)""")]
        public void GivenUsersRampingUpatUserPerSecondQuery(int totalNumberOfUsers, int userSteps, int betweenRamps, string query)
        {
            GivenUsersRampingUpAtUserPerSecondGapOfSecondQuery(totalNumberOfUsers, userSteps, betweenRamps , 0, query);
        }
    }
}