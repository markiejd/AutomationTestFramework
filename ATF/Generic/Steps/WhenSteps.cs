using Core;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps
{
    [Binding]
    public class WhenSteps : StepsBase
    {
        public WhenSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        private bool Failed(string proc, string message = "")
        {
            if (message != "") DebugOutput.Log(message);            
            CombinedSteps.Failure(proc);
            return false;
        }


        [When(@"I Execute Command ""(.*)""")]
        public void WhenIExecuteCommand(string command)
        {
            CmdUtil.ExecuteCommand(command);
        }


        [When(@"I Unzip File At ""(.*)""")]
        public bool WhenIUnzipFile(string zipFileRepoLocation)
        {
            string proc = $"When I Unzip File {zipFileRepoLocation}";
            if (CombinedSteps.OutputProc(proc))
            {
                // the directory and path give must be made into OS format
                var osZipFileRepoLocation = FileUtils.GetCorrectDirectory(zipFileRepoLocation);
                DebugOutput.Log($"The OS format of the zip file location is {osZipFileRepoLocation}");
                if (!FileUtils.OSFileCheck(osZipFileRepoLocation))
                {
                    Failed(proc, $"Failed to find file {osZipFileRepoLocation}");
                    return false;
                }
                DebugOutput.Log($"We have found the file {osZipFileRepoLocation}");
                var success = FileUtils.OSUnZipFile(osZipFileRepoLocation);
                if (!success)
                {
                    Failed(proc, $"Failed to unzip file {osZipFileRepoLocation}");
                    return false;
                }
                DebugOutput.Log($"We have unzipped the file {osZipFileRepoLocation}");
                return true;
            }
            return false;
        }

        
        [When(@"I SQL Command As CSV ""(.*)""")]
        public void WhenISQLCommandAsCSV(string sqlCommand)
        {
            sqlCommand.Replace(";", "");
            sqlCommand.Replace("\"", "");
            sqlCommand = "\"" + sqlCommand + " \" c:/tmp/hello1235.csv";
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            DebugOutput.Log($"THE RETURN STRING IS :-");
            DebugOutput.Log($"{returnedString}");
            List<string> lines = new List<string>(returnedString.Split(new[] { "\r\n" }, StringSplitOptions.None));
            DebugOutput.Log($"We have returned {lines.Count()} lines from the command {sqlCommand}");
        }
        
        [When(@"I SQL Command As JSON ""(.*)""")]
        public void WhenISQLCommandAsJSON(string sqlCommand)
        {
            sqlCommand.Replace(";", "");
            sqlCommand.Replace("\"", "");
            sqlCommand = "\"" + sqlCommand + " FOR JSON AUTO; \" c:/tmp/hello1235.csv";
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            DebugOutput.Log($"THE RETURN STRING IS :-");
            DebugOutput.Log($"{returnedString}");
            List<string> lines = new List<string>(returnedString.Split(new[] { "\r\n" }, StringSplitOptions.None));
            DebugOutput.Log($"We have returned {lines.Count()} lines from the command {sqlCommand}");
        }



        [When(@"I Click On Back Button In Browser")]
        public bool WhenIClickOnBackButtonInBrowser()
        {
            string proc = $"When I Click On Back Button In Browser";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!ElementInteraction.ClickBackButtonInBrowser()) return Failed(proc, "Failed to click on browsers back button!");
                return true;
            }
            return false;
        }
        
        
        [When(@"I Clear Repo Directory ""(.*)""")]
        public void WhenIClearRepoDirectory(string repoDirectory)
        {
            // if reportDirectory does not start with a / make it so
            if (!repoDirectory.StartsWith("/"))
            {
                repoDirectory = "/" + repoDirectory;
            }
            // if repoDirectory does not end with a / make it so
            if (!repoDirectory.EndsWith("/"))
            {
                repoDirectory = repoDirectory + "/";
            }
            string proc = $"When I Clear Repo Directory {repoDirectory}";
            if (CombinedSteps.OutputProc(proc))
            {
                var osDirectory = FileUtils.GetCorrectDirectory(repoDirectory);
                DebugOutput.Log($"The OS format of the directory is {osDirectory}");
                if (!FileUtils.OSDirectoryCheck(osDirectory))
                {
                    DebugOutput.Log($"The directory {osDirectory} does not exist - nothing to clear!");
                    return;
                }
                DebugOutput.Log($"We have found the directory {osDirectory} - clearing it!");
                if (!FileUtils.OSDirectoryClean(osDirectory))
                {
                    CombinedSteps.Failure("Failed to clear the directory {osDirectory}!");
                    return;
                }
                DebugOutput.Log($"We have cleared the directory {osDirectory}!");
            }
        }


        
        [When(@"I Get From URL ""(.*)""")]
        public async Task<bool> WhenIGetFromURL(string url)
        {
            string proc = $"When I Get From URL {url}";
            if (CombinedSteps.OutputProc(proc))
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
            if (CombinedSteps.OutputProc(proc))
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
            if (CombinedSteps.OutputProc(proc))
            {
                var response = await APIUtil.Post(url, jSonString, "post", false);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"Successfull GET'ed - Json file has been created with output - this maybe empty btw!");
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
            if (CombinedSteps.OutputProc(proc))
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
            if (CombinedSteps.OutputProc(proc))
            {
                var response = await APIUtil.Post(url, jSonString, "post", false);
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"Successfull GET'ed - Json file has been created with output - this maybe empty btw!");
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
            if (CombinedSteps.OutputProc(proc))
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

        
        [When(@"I Switch To Tab (.*)")]
        public bool WhenISwitchToTab(int tabNumber)
        {
            string proc = $"When I Switch To Tab {tabNumber}";
            if (CombinedSteps.OutputProc(proc))
            {
                return false;
                // if (!ElementInteraction.SwapTabByNumber(tabNumber))
                // {
                //     return Failed(proc, "Failed to switch tab!");
                // }
                // return true;
            }
            return false;
        }

        
        [When(@"I Close Tab (.*)")]
        public bool WhenICloseTab(int tabNumber)
        {
            string proc = $"When I Close Tab {tabNumber}";
            if (CombinedSteps.OutputProc(proc))
            {
                return false;
                // if (!ElementInteraction.CloseTabByNumber(tabNumber))
                // {
                //     return Failed(proc, "Failed to switch tab!");
                // }
                // return true;
            }
            return false;
        }



    }
}
