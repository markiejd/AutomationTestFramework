using Core.Configuration;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;
using Core;
using Core.FileIO;
using Core.Transformations;
using AppXAPI.Models;
using AppXAPI;

namespace Generic.Steps
{
    [Binding]
    public class APISteps : StepsBase
    {

        public APISteps(IStepHelpers helpers) : base(helpers)
        {
        }

        private bool Failed(string proc, string message = "***** FAILURE ******", bool flag = true)
        {
            DebugOutput.Log(message);
            CombinedSteps.Failure(proc);
            return flag;
        }

        
        [Then(@"Response Status Code Is Equal To ""(.*)""")]
        public bool ThenResponseStatusCodeIsEqualTo(string statusCode)
        {
            var proc = $"Then Response Status Code Is Equal To '{statusCode}'";
            if (CombinedSteps.OuputProc(proc))
            {
                if (APIResponse.fullResponse == null) return Failed(proc, "Failed to read response!");
                var statusCodeReceived = (int)APIResponse.fullResponse.StatusCode;
                if (statusCodeReceived.ToString() == statusCode) return true;
                DebugOutput.Log($"Did not match! {statusCodeReceived.ToString()} ");
                return Failed(proc, "Did not mathc!");
            }
            return false;            
        }

        
        
        [When(@"I With Active JSession Get From URL ""(.*)""")]
        public  async Task<bool> WhenIWithActiveJSessionGetFromURL(string url)
        {
            string proc = $"When I With Active JSession Get From URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                var JSESSIONID = await APIUtil.GetJsessionId();
                if (JSESSIONID == null) return Failed(proc, "Failed to get JSESSIONID");
                var redirectionJESSSIONID = await APIUtil.GetRedirectionURL();
                if (redirectionJESSSIONID == null) return Failed(proc, "Failed to get JSESSIONID from redirection");


                return true;
            }
            return false;
        }

        
        [Given(@"Session Token")]
        public void GivenSessionToken()
        {
            string proc = $"Given Session Token";
            if (CombinedSteps.OuputProc(proc))
            {
                
            }
        }


        
        [Given(@"I Have Redirection URL Using JSession")]
        public async Task<bool> GivenIHaveRedirectionURLUsingJSession()
        {
            string proc = $"Given I Have Redirection URL Using JSession";
            if (CombinedSteps.OuputProc(proc))
            {
                var x = await APIUtil.GetRedirectionURL();
                if (x == null) return Failed("Failed in the redirection !");
                return true;
            }
            return false;
        }


        
        [When(@"I Get From URL ""(.*)""")]
        public async Task<bool> WhenIGetFromURL(string url)
        {
            string proc = $"When I Get From URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                var response = await APIUtil.Get(url, "get");
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"Successfull GET'ed - Json file has been created with output - this maybe empty btw!");
                    return true;
                }
                DebugOutput.Log($"We have an unsuccessful response code '{response.StatusCode}' exception {response.Content}");
            }    
            return Failed(proc);        
        }

        
        [When(@"Using Windows Authentication User ""(.*)"" Password ""(.*)"" I Get From URL ""(.*)""")]
        public async Task<bool> WhenUsingWindowsAuthenticationUserPasswordIGetFromURL(string winUser,string winPassword,string url)
        {
            string proc = $"When I Get From URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                var response = await APIUtil.GetWithWindowsAuth(url,"getWithAuth", winUser, winPassword);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"Successfull GET'ed - Json file has been created with output - this maybe empty btw!");
                    return true;
                }
                DebugOutput.Log($"We have an unsuccessful response code '{response.StatusCode}' exception {response.Content}");
            }
            return Failed(proc);
        }

        
        [When(@"I Post Json ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPostJsonToURL(string jSonString,string url)
        {
            string proc = $"When I Post Json {jSonString} To URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                var response = await APIUtil.Post(url, jSonString, "post", false);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"Successfull POST'ed - Json file has been created with output - this maybe empty btw!");
                    return true;
                }
                DebugOutput.Log($"We have an unsuccessful response code '{response.StatusCode}' exception {response.Content}");
            }
            return Failed(proc);
        }

        
        [When(@"I Post Json File ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPostJsonFileToURL(string jsonFileLocation,string url)
        {
            string proc = $"When I Post Json File {jsonFileLocation} To URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.OSFileCheck(jsonFileLocation))
                {
                    return Failed(proc, "Failed to find file {jsonFileLocation}");
                }
                var jSonText = JsonValues.ReadOSJsonFile(jsonFileLocation);
                if (jSonText == null) return Failed(proc, "Read the file, Json part failed!");
                var success =  await WhenIPostJsonToURL(jSonText, url);
                if (success) return true;
                return Failed(proc, "Failed to post!");
            }
            return false;
        }


        [When(@"I Patch Json ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPatchJsonToURL(string jSonString,string url)
        {
            string proc = $"When I Patch Json {jSonString} To URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                var response = await APIUtil.Post(url, jSonString, "post", false);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"Successfull PATCH'ed - Json file has been created with output - this maybe empty btw!");
                    return true;
                }
                DebugOutput.Log($"We have an unsuccessful response code '{response.StatusCode}' exception {response.Content}");
            }
            return Failed(proc);
        }

        
        [When(@"I Patch Json File ""(.*)"" To URL ""(.*)""")]
        public async Task<bool> WhenIPatchJsonFileToURL(string jsonFileLocation,string url)
        {
            string proc = $"When I Patch Json {jsonFileLocation} To URL {url}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.OSFileCheck(jsonFileLocation))
                {
                    return Failed(proc, "Failed to find file {jsonFileLocation}");
                }
                var jSonText = JsonValues.ReadOSJsonFile(jsonFileLocation);
                if (jSonText == null) return Failed(proc, "Read the file, Json part failed!");
                var success =  await WhenIPostJsonToURL(jSonText, url);
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
            DebugOutput.Log($"START TIME {startTime.ToString()}");
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
                DebugOutput.Log($"{numberOfUsers} BEFORE AND AFTER {before.ToString()} {after.ToString()}");
                numberOfUsers = numberOfUsers + userSteps;

                // Last ramp - need to wait for this one to complete!
                // Or the test threat completes and output stops!
                if (numberOfUsers >= totalNumberOfUsers)
                {
                    firstTick = DateTime.Now.Ticks;
                    long secondTick = DateTime.Now.Ticks;
                    DebugOutput.Log($"AWAITING HERE!");
                    var task = APIUtil.MyAsyncMethod(firstTick, secondTick, query);
                    task.Wait();
                    var result = task.Result;
                    DebugOutput.Log($"AWAITING HERE???? {result}");
                }
            }
            var endTime = DateTime.Now;
            DebugOutput.Log($"END TIME {endTime.ToString()}");
        }


        
        [Given(@"(.*) Users Ramping Up at (.*) User Per (.*) Second Query ""(.*)""")]
        public void GivenUsersRampingUpatUserPerSecondQuery(int totalNumberOfUsers, int userSteps, int betweenRamps, string query)
        {
            GivenUsersRampingUpAtUserPerSecondGapOfSecondQuery(totalNumberOfUsers, userSteps, betweenRamps , 0, query);
        }


                
    }
}