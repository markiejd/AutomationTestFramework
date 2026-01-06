using AppXAPI;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;
using Core;
using Core.FileIO;
using Core.Transformations;
using Generic.Steps.JIRA;
using Generic.Steps.Trx;
using Generic.Steps.XRay;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Core.Configuration;
using Core.Jira;

namespace Generic.Steps
{
    [Binding]
    public class JIRASteps : StepsBase
    {

        public JIRASteps(IStepHelpers helpers) : base(helpers)
        {
        }

        private bool Failed(string proc, string message = "***** FAILURE ******", bool flag = true)
        {
            DebugOutput.Log(message);
            CombinedSteps.Failure(proc);
            return flag;
        }

        private bool IsJiraEnabled()
        {
            if (!TargetConfiguration.Configuration.Jira)
            {
                DebugOutput.Log($"Jira is not enabled!");
                return false;
            }
            var jiraDetails = TargetJiraConfiguration.Configuration;
            if (jiraDetails == null)
            {
                DebugOutput.Log($"We have no jira details to work with");
                return false;
            }
            var name = TargetConfiguration.Configuration.JiraName;
            if (name == null)
            {
                DebugOutput.Log($"We need to know what we call this jira so we can access the information,  there are many jira!");
                return false;
            }
            DebugOutput.Log($"See no reason you cannot use Jira");
            return true;
        }
        
                
        [Given(@"Jira Server Is Available")]
        public async Task<HttpResponseMessage?> GivenJiraServerIsAvailable()
        {
            string proc = $"Given Jira Server Is Available";
            if (CombinedSteps.OutputProc(proc))
            {
                TargetJiraConfiguration.JiraConfiguration? jiraDetails;
                if (!IsJiraEnabled() || TargetConfiguration.Configuration.JiraName == null )
                {
                    CombinedSteps.Failure(proc);
                    return null;
                } 
                jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails != null)
                {
                    DebugOutput.Log($" We have JIRA {TargetConfiguration.Configuration.JiraName} which uses {jiraDetails.Authorization} on the url {jiraDetails.Url} authorising as {jiraDetails.Authorization} maybe a username {jiraDetails.UserName} and password {jiraDetails.UserPassword} MAY EVEN HAVE A TOKEN! {jiraDetails.Token}");
                    switch(jiraDetails.Authorization.ToLower())
                    {
                        default:
                        {
                            DebugOutput.Log($"Sorry I do not know that autoroization type in the jira {jiraDetails.JiraName}");
                            CombinedSteps.Failure(proc);
                            return null;
                        }
                        case "token":
                        {
                            if (jiraDetails.Token != null)
                            {
                                var response = await APIUtil.GetResponseWithAccessToken(jiraDetails.Token, jiraDetails.Url);
                                if (response.IsSuccessStatusCode) DebugOutput.Log($"GivenJiraServerIsAvailable WINNER1");
                                if (response.IsSuccessStatusCode) return response;
                            }
                            break;
                        }
                        case "basic":
                        {
                            if (jiraDetails.UserName != null && jiraDetails.UserPassword != null)
                            {
                                var response = await APIUtil.GetResponseWithApplicationAuth(jiraDetails.Url, "testy", jiraDetails.UserName, jiraDetails.UserPassword);
                                if (response.IsSuccessStatusCode) DebugOutput.Log($"GivenJiraServerIsAvailable WINNER2");
                                if (response.IsSuccessStatusCode) return response;
                            }
                            break;
                        }
                    }
                }
            }
            DebugOutput.Log($"All gone pete tong!");
            CombinedSteps.Failure(proc);
            return null;
        }

        
        [Given(@"Jira Evidence Is Uploaded From This ATF")]
        public async Task<bool> GivenJiraEvidenceIsUploadedFromThisATF()
        {
            string proc = $"Given Jira Evidence Is Uploaded From This ATF";
            if (CombinedSteps.OutputProc(proc))
            {
                // get a list of directories in TestResults.
                var testResultsDirectory = FileUtils.GetCorrectDirectory("/AppSpecFlow/TestResults/");
                var listOfDirectoriesInTestResults = FileUtils.OSGetListOfDirectoriesInDirectory(testResultsDirectory);
                DebugOutput.Log($"We have {listOfDirectoriesInTestResults.Count()} directories!");   
                var tempDirectory = FileUtils.GetUserTempFolder() + "jiraTransfers";
                DebugOutput.Log($"Temp directory = {tempDirectory}");
                var cleanedUp = FileUtils.OSDeleteDirectoryIfExists(tempDirectory);
                if (!cleanedUp) return Failed(proc, "Failed to clean up directory");
                var createdTemp = FileUtils.OSDirectoryCreation(tempDirectory + @"\");
                if (!createdTemp) return Failed(proc, "Failed to create temp location!");   
                bool any = false;       

                // go through list
                foreach (var directory in listOfDirectoriesInTestResults)
                {
                    DebugOutput.Log($"Looking in directory {directory} for *.jira!");
                    var listOfFilesInDirectory = FileUtils.OSGetListOfFilesInDirectory(directory, "jira");
                    string? jiraTestEecutionKey;
                    foreach (var file in listOfFilesInDirectory)
                    {
                        // If list containst .Jira
                        jiraTestEecutionKey = file.Replace(".jira","");
                        DebugOutput.Log($"WE have a Jira issue number of {jiraTestEecutionKey}");

                        // copy TRX into epoch folder
                        DebugOutput.Log($"DOING TRX!");
                        var listOfTestFilesInEpoch = FileUtils.OSGetListOfFilesInDirectory(directory, "test");
                        var testName = "";
                        foreach (var testResultsfile in listOfTestFilesInEpoch)
                        {
                            testName = testResultsfile.Replace(".test","");
                        }
                        if (testName != "")
                        {
                            DebugOutput.Log($"WE HAVE TEST {testName}");
                            var listOfFilesInTestResult = FileUtils.OSGetListOfFilesInDirectory(directory, "test");
                            foreach (var testResultFile in listOfFilesInTestResult)
                            {
                                if (testResultFile.ToLower().Contains(testName.ToLower()))
                                {
                                    var newTestResultFile = testResultFile.Replace(".test",".trx");
                                    var fullTestResultFile = testResultsDirectory + newTestResultFile;
                                    var newFullTestResultFile = directory + @"/" + newTestResultFile;
                                    if (FileUtils.OSFileCheck(fullTestResultFile))
                                    {
                                        if (!FileUtils.OSCopyFile(fullTestResultFile, newFullTestResultFile, false)) return Failed(proc, $"FAILED to copy files {fullTestResultFile} to {newFullTestResultFile}");
                                    }
                                }
                            }
                        }

                        // Upload ANY FAIL jira images for this 
                        var repoFailImageLocation = "/AppSpecFlow/TestOutput/";
                        var testOutputImageFolder = FileUtils.GetCorrectDirectory(repoFailImageLocation);
                        var listOfPNGFilesInDirectory = FileUtils.OSGetListOfFilesInDirectory(testOutputImageFolder, "png");
                        foreach (var pngFile in listOfPNGFilesInDirectory)
                        {
                            DebugOutput.Log($"Checking image file {pngFile}");
                            if (pngFile.Contains(jiraTestEecutionKey))
                            {
                                DebugOutput.Log($"This one has this execution {jiraTestEecutionKey}");
                                var fullPNGFilePath = FileUtils.GetCorrectDirectory(repoFailImageLocation + pngFile);
                                var epochDirectory = directory + "/" + pngFile;
                                if (!FileUtils.OSCopyFile(fullPNGFilePath, epochDirectory, false)) return Failed(proc, $"FAILED to copy PNG file {fullPNGFilePath} to {directory + pngFile}");
                                DebugOutput.Log($"Copied {fullPNGFilePath} to EPOCH {epochDirectory}, renamed.  Job done!");
                                if (!FileUtils.OSFileDeletion(fullPNGFilePath)) return Failed(proc, $"Failed to delete the png {fullPNGFilePath} after its copy!");
                            }
                        }

                        any = true;
                        
                        // the delete the .jira
                        var deleteFile = FileUtils.OSFileDeletion(directory + @"\" +file);
                        if (!deleteFile) return Failed(proc, $"Failed to delete file {file}");


                        // zip the folder
                        var zippedFileName = tempDirectory + @"\" + jiraTestEecutionKey + ".zip";
                        var zipped = FileUtils.OSCreateZipFile(directory, zippedFileName);
                        if (!zipped) return Failed(proc, $"Failed to zip the directory {directory}");
                        // send to Jira based on that number
                        var uploaded = await Jira.UploadAttachment(jiraTestEecutionKey, zippedFileName);
                        if (!uploaded) DebugOutput.Log($"For the love of the wee man!");
                        if (uploaded) any = true;
                    }
                }
                if (any) return true;


            }
            return false;
        }


        
        [Then(@"Jira Issue ""(.*)"" Exists")]
        public async Task<bool> ThenJiraIssueExists(string issueNumber)
        {
            string proc = $"Then Jira Issue {issueNumber} Exists";
            if (CombinedSteps.OutputProc(proc))
            {
                TargetJiraConfiguration.JiraConfiguration? jiraDetails;
                if (!IsJiraEnabled() || TargetConfiguration.Configuration.JiraName == null )
                {
                    CombinedSteps.Failure(proc);
                    return false;
                } 
                jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails != null)
                {
                    DebugOutput.Log($" We have JIRA {TargetConfiguration.Configuration.JiraName} which uses {jiraDetails.Authorization} on the url {jiraDetails.Url} authorising as {jiraDetails.Authorization} maybe a username {jiraDetails.UserName} and password {jiraDetails.UserPassword} MAY EVEN HAVE A TOKEN! {jiraDetails.Token}");
                    var extraUrl = "rest/api/latest/issue/";
                    var newUrl = jiraDetails.Url + extraUrl + issueNumber;
                    switch(jiraDetails.Authorization.ToLower())
                    {
                        default:
                        {
                            DebugOutput.Log($"Sorry I do not know that autoroization type in the jira {jiraDetails.JiraName}");
                            CombinedSteps.Failure(proc);
                            return false;
                        }
                        case "token":
                        {
                            if (jiraDetails.Token != null)
                            {
                                // https://proactionuk.ent.cgi.com/jira/
                                var response = await APIUtil.GetResponseWithAccessToken(jiraDetails.Token, newUrl);
                                if (response.IsSuccessStatusCode) DebugOutput.Log($"ThenJiraIssueExists WINNER1");
                                if (response.IsSuccessStatusCode) return true;
                            }
                            break;
                        }
                        case "basic":
                        {
                            if (jiraDetails.UserName != null && jiraDetails.UserPassword != null)
                            {
                                var response = await APIUtil.GetResponseWithApplicationAuth(newUrl, "testy", jiraDetails.UserName, jiraDetails.UserPassword);
                                if (response.IsSuccessStatusCode) DebugOutput.Log($"ThenJiraIssueExists WINNER2");
                                if (response.IsSuccessStatusCode) return true;
                            }
                            break;
                        }
                    }
                }
            }
            DebugOutput.Log($"FAiled here ThenJiraIssueExists");
            return Failed(proc);
        }

        
        [When(@"I Upload ""(.*)"" To Jira Test Execution ""(.*)""")]
        public async Task<bool> WhenIUploadToJiraTestExecution(string file,string executionKey)
        {
            string proc = $"When I Upload {file} To Jira Test Execution Key {executionKey}";
            if (CombinedSteps.OutputProc(proc))
            {
                var uploaded = await Jira.UploadAttachment(executionKey, file);
                if (uploaded)  return true;
                // var jira = TargetConfiguration.Configuration.JiraName;
                // if (jira != null)
                // {
                //     var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jira);
                //     if (jiraDetails == null) return false;
                //     // https://proactionuk.ent.cgi.com/jira/
                //     var url = jiraDetails.Url + "rest/api/2/issue/" + executionKey + "/attachments";
                //     if (jiraDetails.Token != null)
                //     {
                //         var response = await APIUtil.PostZipFileWithToken(url, file);
                //         if (response.IsSuccessStatusCode) return true;
                //     }
                //     else
                //     {
                //         if (jiraDetails.UserName != null && jiraDetails.UserPassword != null)
                //         {
                //             var response = await APIUtil.PostZipFileWithWindowsAuth(url, file, executionKey, "default", jiraDetails.UserName, jiraDetails.UserPassword);
                //             if (response.IsSuccessStatusCode) return true;
                //         }
                //     }
                //     DebugOutput.Log($"FAILED TO UPLOAD!");
                // }
                return Failed(proc);
            }
            return false;
        }

        
        [Then(@"Test Execution Key ""(.*)"" Exists In Jira")]
        public async Task<bool> ThenTestExecutionKeyExistsInJira(string executionKey)
        {
            string proc = $"Then Test Execution Key {executionKey} Exists In Jira";
            if (CombinedSteps.OutputProc(proc))
            {
                TargetJiraConfiguration.JiraConfiguration? jiraDetails;
                if (!IsJiraEnabled() || TargetConfiguration.Configuration.JiraName == null )
                {
                    CombinedSteps.Failure(proc);
                    return false;
                } 
                jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails != null)
                {
                    DebugOutput.Log($" We have JIRA {TargetConfiguration.Configuration.JiraName} which uses {jiraDetails.Authorization} on the url {jiraDetails.Url} authorising as {jiraDetails.Authorization} maybe a username {jiraDetails.UserName} and password {jiraDetails.UserPassword} MAY EVEN HAVE A TOKEN! {jiraDetails.Token}");
                    var extraUrl = "rest/raven/1.0/api/testexec/";
                    // https://proactionuk.ent.cgi.com/jira/rest/raven/1.0/api/testexec/SDIACSC-178/test
                    // https://triggersbroom.org/jira/rest/raven/1.0/api/testexec/ADSP-11046/test
                    var newUrl = jiraDetails.Url + extraUrl + executionKey + "/test";
                    switch(jiraDetails.Authorization.ToLower())
                    {
                        default:
                        {
                            DebugOutput.Log($"Sorry I do not know that autoroization type in the jira {jiraDetails.JiraName}");
                            CombinedSteps.Failure(proc);
                            return false;
                        }
                        case "token":
                        {
                            if (jiraDetails.Token != null)
                            {
                                var jsonString = await APIUtil.GetStringWithAccessToken(jiraDetails.Token, newUrl);
                                if (jsonString != null) return true;
                            }
                            break;
                        }
                        case "basic":
                        {
                            if (jiraDetails.UserName != null && jiraDetails.UserPassword != null)
                            {
                                var jsonString = await APIUtil.GetStringWithAppAuth(newUrl, executionKey, jiraDetails.UserName, jiraDetails.UserPassword);
                                if (jsonString != null) return true;
                            }
                            break;
                        }
                    }
                }
            }
            return Failed(proc);
        }

        
        [Then(@"Text Execution Key ""(.*)"" Contains Test ""(.*)""")]
        public async Task<bool> ThenTextExecutionKeyContainsTest(string executionKey,string testKey)
        {
            string proc = $"Then Test Execution Key {executionKey} Contains Test {testKey}";
            if (CombinedSteps.OutputProc(proc))
            {
                TargetJiraConfiguration.JiraConfiguration? jiraDetails;
                if (!IsJiraEnabled() || TargetConfiguration.Configuration.JiraName == null )
                {
                    CombinedSteps.Failure(proc);
                    return false;
                } 
                jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails != null)
                {
                    DebugOutput.Log($" We have JIRA {TargetConfiguration.Configuration.JiraName} which uses {jiraDetails.Authorization} on the url {jiraDetails.Url} authorising as {jiraDetails.Authorization} maybe a username {jiraDetails.UserName} and password {jiraDetails.UserPassword} MAY EVEN HAVE A TOKEN! {jiraDetails.Token}");
                    var extraUrl = "rest/raven/1.0/api/testexec/";
                    // https://proactionuk.ent.cgi.com/jira/rest/raven/1.0/api/testexec/SDIACSC-178/test
                    // https://triggersbroom.org/jira/rest/raven/1.0/api/testexec/ADSP-11046/test
                    var newUrl = jiraDetails.Url + extraUrl + executionKey + "/test";
                    List<SimpleXRayModel> listOfModels = new();
                    SimpleXRayModel[]? array = null;
                    switch(jiraDetails.Authorization.ToLower())
                    {
                        default:
                        {
                            DebugOutput.Log($"Sorry I do not know that autoroization type in the jira {jiraDetails.JiraName}");
                            CombinedSteps.Failure(proc);
                            return false;
                        }
                        case "token":
                        {
                            if (jiraDetails.Token != null)
                            {
                                var jsonString = await APIUtil.GetStringWithAccessToken(jiraDetails.Token, newUrl);
                                if (jsonString != null)
                                {
                                    array = Generic.Steps.XRay.SimpleXRayModelUsage.CreateModelFromExecutionJson(jsonString);
                                    if (array != null)
                                    {
                                        foreach (var a in array)
                                        {
                                            var singleModel = new Generic.Steps.XRay.SimpleXRayModel();
                                            singleModel = a;
                                            listOfModels.Add(singleModel);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        case "basic":
                        {
                            if (jiraDetails.UserName != null && jiraDetails.UserPassword != null)
                            {
                                var jsonString = await APIUtil.GetStringWithAppAuth(newUrl, executionKey, jiraDetails.UserName, jiraDetails.UserPassword);
                                if (jsonString != null)
                                {
                                    array = Generic.Steps.XRay.SimpleXRayModelUsage.CreateModelFromExecutionJson(jsonString);
                                }
                            }
                            break;
                        }
                        
                    }
                    if (array != null)
                    {
                        foreach (var a in array)
                        {
                            var singleModel = new Generic.Steps.XRay.SimpleXRayModel();
                            singleModel = a;
                            listOfModels.Add(singleModel);
                        }
                    }
                    foreach (var model in listOfModels)
                    {
                        if (model.key != null)
                        {
                            if (model.key.ToLower() == testKey.ToLower()) return true;
                        }
                    }
                    DebugOutput.Log($"Been through the {listOfModels.Count()} and failed to find {testKey}");
                }
            }
            return Failed(proc);
        }

        
        [When(@"I Move Test Execution Key ""(.*)"" Test ""(.*)"" To Status ""(.*)""")]
        public void WhenIMoveTestExecutionKeyTestToStatus(string executionKey,string testKey,string status)
        {
            string proc = $"Then Test Execution Key {executionKey} Contains Test {testKey}";
            if (CombinedSteps.OutputProc(proc))
            {
                
            }
        }   

        
        // [When(@"I Execute Jira Test Execution ""(.*)""")]
        // public void WhenIExecuteJiraTestExecution(string testexec)
        // {
        //     var model = Jira.GetJiraModelFromIssue(testexec);
        //     int numberOfTestsInExection = 0;
        //     // create a batch file in .Core/Batch/Temp/ called testexec.bat
        //     var file = "/Core/Batch/Temp/testexec.bat";
        //     if (FileUtils.FileCheck(file)) FileUtils.FileDeletion(file);
        //     FileUtils.FileCreation(file);
        //     if (model != null)
        //     {
        //         if (model.fields != null)
        //         {
        //             if (model.fields.customfield_10225 != null)
        //             {
        //                 numberOfTestsInExection = model.fields.customfield_10225.Count();
        //                 DebugOutput.Log($"NUMBER OF TESTS IN EXECUTION = {numberOfTestsInExection}");
        //                 foreach (var test in model.fields.customfield_10225)
        //                 {
        //                     Thread.Sleep(100);
        //                     var key = test.testKey;
        //                     // populate the batch file in .Core/Batch/Temp/ called testexec.bat
        //                     // with Start dotnet test --no-build --filter "QA000010"
        //                     var line = "    Start dotnet test --no-build --filter \"" + key + "\"";
        //                     DebugOutput.Log($"Willbe adding : {line}");
        //                     FileUtils.FileLinePopulate(file, line);
        //                 }
        //             }
        //         }
        //     }
        //     return;
        // }




        
        [When(@"I Update Jira Execution Key ""(.*)"" In Jira")]
        public  async Task<bool> WhenIUpdateJiraExecutionKeyInJira(string executionKey)
        {
            string proc = $"When I Update Jira Execution Key {executionKey} In Jira";
            if (CombinedSteps.OutputProc(proc))
            {
                //Make sure there is an execution executionKey
                if (TargetConfiguration.Configuration.JiraName == null) return false;
                var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails == null) return false;
                var url = Jira.GetURLFor("testexec", executionKey);
                if (url == null) return false;
                HttpResponseMessage? response = null;
                if (jiraDetails.Token != null) response = await APIUtil.GetResponseWithAccessToken("", url);
                if (jiraDetails.UserName != null && jiraDetails.UserPassword != null) response = await APIUtil.GetResponseWithApplicationAuth(url, "JiraResponse", jiraDetails.UserName, jiraDetails.UserPassword);
                if (response == null) return false;
                if (!response.IsSuccessStatusCode) return Failed(proc, $"Failed to read {url}");
                var json = await response.Content.ReadAsStringAsync();
                DebugOutput.Log($"json - have {json}");
                //make a model of all the tests in the execution
                var executionModel = XRayExecutionUsage.MakeExecutionModel(json);
                if (executionModel == null) return Failed(proc, "Failed to create Execution Model");
                var listOfTestsInExecution = XRayExecutionUsage.GetListOfKeysToDoFromExecution(executionModel);
                var listOfTestIDsInExecution = XRayExecutionUsage.GetListOfIDsToDoFromExecution(executionModel);
                int counter = 0;

                // go through each TRX file for each test in the execution
                foreach (var test in listOfTestsInExecution)
                {
                    var fileName = test + ".trx";
                    var directory = @"\AppSpecFlow\TestResults\";
                    var fullFileName = directory + fileName;
                    if (!FileUtils.FileCheck(fullFileName))
                    {
                        return Failed(proc, $"Failed to find {fullFileName}");
                    }
                    DebugOutput.Log($"Found {fullFileName}");
                    var xmlString = XMLValues.ReadXMLFile(fullFileName);
                    if (xmlString == null) return Failed(proc, "Failed to populate XML string!");

                    //Create a TRX model based upon the trx file for each test
                    var trxModel = TrxXMLProcedures.MakeTrxMode<TestRun>(xmlString);
                    if (trxModel == null) return Failed(proc, "Failed to populate TRX model!");
                    if (trxModel.Results == null) return Failed(proc, "Failed to populate results!");
                    if (trxModel.Results.UnitTestResult == null) return Failed(proc, "Failed to populate unit test results!");
                    var a = trxModel.Results.UnitTestResult;
                    var jsonPayload = "";
                    foreach (var b in a)
                    {
                        var outcome = b.Outcome;
                        DebugOutput.Log($"This test has the result {outcome}");
                        if (outcome == "Passed") outcome ="PASS";
                        if (outcome == "Failed") outcome ="FAIL";
                        DebugOutput.Log($"This test has the JIRA result {outcome}");
                        // jsonPayload = "{ \"status\": \"" + outcome + "\"}";
                        jsonPayload = "{ \"status\": \"" + outcome + "\"}";
                        var testID = listOfTestIDsInExecution[counter];
                        url = Jira.GetURLFor("testrun", testID);
                        
                        //Update the test using the ID (not the key) with the result from the trx file
                        if (jiraDetails.Token != null) await APIUtil.PutResponseWithToken(url, "PutJira", "", jsonPayload);
                        if (jiraDetails.UserName != null && jiraDetails.UserPassword != null) await APIUtil.PutResponseWithApplicationAuth(url, "PutJira", jiraDetails.UserName, jiraDetails.UserPassword, jsonPayload);
                        break;
                    }
                    counter++;
                }
                return true;
            }
            return false;
        }


        /// TO DO
        [When(@"I Create Execute Test Execution Key ""(.*)"" In Jira")]
        public async Task<bool> WhenICreateExecuteTestExecutionKeyInJira(string executionKey)
        {
            string proc = $"When I Create Execute Test Execution Key {executionKey} In Jira";
            if (CombinedSteps.OutputProc(proc))
            {
                if (TargetConfiguration.Configuration.JiraName == null) return false;
                var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails == null) return false;
                var url = Jira.GetURLFor("testexec", executionKey);
                HttpResponseMessage? response = null;
                if (jiraDetails.Token != null) response = await APIUtil.GetResponseWithAccessToken("", url);
                if (jiraDetails.UserName != null && jiraDetails.UserPassword != null) response = await APIUtil.GetResponseWithApplicationAuth(url, "JiraResponse", jiraDetails.UserName, jiraDetails.UserPassword);
                if (response == null) return false;
                if (!response.IsSuccessStatusCode) return Failed(proc, $"Failed to read {url}");
                var json = await response.Content.ReadAsStringAsync();
                DebugOutput.Log($"json - have {json}");
                var executionModel = XRayExecutionUsage.MakeExecutionModel(json);
                if (executionModel == null) return Failed(proc, "Failed to create Execution Model");
                var listOfTestsInExecution = XRayExecutionUsage.GetListOfKeysToDoFromExecution(executionModel);
                //make the batch file
                var fileName = @"\AppSpecFlow\Batch\" + executionKey + "Run.bat";
                FileUtils.FileDeletion(fileName);
                Thread.Sleep(1000);
                FileUtils.FileLinePopulate(fileName, "cd..");
                // FileUtils.FileCreation(fileName);
                foreach (var test in listOfTestsInExecution)
                {
                    //Start dotnet test --no-build --filter "QA000050"
                    //dotnet test --filter:"TestCategory=QA000010" --logger "trx;logfilename=QA000010.trx"
                    var line = "Call dotnet test --no-build --filter:\"TestCategory=" + test + "\" --logger \"trx;logfilename=" + test + ".trx";
                    DebugOutput.Log($"{line}");
                    FileUtils.FileLinePopulate(fileName, line);
                }
                return FileUtils.FileLinePopulate(fileName, "cd batch");
            }
            return false;
        }


        


        
        [Then(@"Test Key ""(.*)"" Exists In Jira")]
        public async Task<bool> ThenTestKeyExistsInJira(string testKey)
        {
            string proc = $"Then Test Key {testKey} Exists In Jira";
            if (CombinedSteps.OutputProc(proc))
            {
                var x = VariableConfiguration.Configuration;
                var userName = x.JiraUserName;
                var userPassword = x.JiraUserPassword;
                var url = x.JiraServer + x.XRayAPI + @"test?keys=" + testKey;
                var response = await APIUtil.GetResponseWithApplicationAuth(url, "JiraResponse", userName, userPassword);
                if (!response.IsSuccessStatusCode) return Failed(proc, "Failed to read {url}");
                var json = await response.Content.ReadAsStringAsync();
                DebugOutput.Log($"json - have {json}");
                var testsModel = XRayTestsUsage.MakeTestsModel(json);
                if (testsModel == null) return Failed(proc, "Failed to make tests model!");
                foreach (var test in testsModel)
                {
                    if (test.key == testKey) return true;
                }
                return Failed(proc, $"Failed to find {testKey}");
            }
            return false;
        }


        [Then(@"Project ""(.*)"" Exists In Jira")]
        public async Task<bool> ThenProjectExistsInJira(string projectName)
        {
            string proc = $"Then Project {projectName} Exists In Jira";
            if (CombinedSteps.OutputProc(proc))
            {
                var x = VariableConfiguration.Configuration;
                var userName = x.JiraUserName;
                var userPassword = x.JiraUserPassword;
                var url = x.JiraServer + x.JiraAPI + @"2/project";
                var response = await APIUtil.GetResponseWithApplicationAuth(url, "JiraResponse", userName, userPassword);
                if (!response.IsSuccessStatusCode) return Failed(proc, "Failed to read {url}");
                DebugOutput.Log($"We have a response");
                var json = await response.Content.ReadAsStringAsync();
                DebugOutput.Log($"json - have {json}");
                var projectModels = ProjectUsage.MakeProjectModel(json);
                if (projectModels == null) return Failed(proc, "Failed to create the project models!");
                var projectModel = ProjectUsage.GetProjectByProjectName(projectModels, projectName);
                if (projectModel == null) return Failed(proc, "Failed to find the project in the models");
                if (projectModel.Name == projectName) return true;
                return Failed(proc, $"Failed to find {projectName}");
            }
            return false;
        }
        

        [When(@"I Get Jira Data From URL ""(.*)""")]
        public async Task<bool> WhenIGetJiraDataFromURL(string url)
        {
            string proc = $"When I Get Jira Data From URL {url}";
            if (CombinedSteps.OutputProc(proc))
            {
                var x = VariableConfiguration.Configuration;
                var userName = x.JiraUserName;
                var userPassword = x.JiraUserPassword;
                var response = await APIUtil.GetResponseWithApplicationAuth(url, "JiraResponse", userName, userPassword);
                if (!response.IsSuccessStatusCode) return Failed(proc, "Failed to read {url}");
                DebugOutput.Log($"We have a response");
                var text = await response.Content.ReadAsStringAsync();
                DebugOutput.Log($"WE have {text}");
                return true;
            }
            return false;
        }

    }
}
