
using System.Diagnostics;
using System.Net.Http.Headers;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Newtonsoft.Json;
using OpenQA.Selenium.DevTools;


namespace Core.Configuration
{
    public class TargetTestReport
    {
        public static TestReportTiming TestReport { get; private set;} = new TestReportTiming();

        // Full Test - multiple Feature Files
        public class TestReportTiming
        {
            public string? EPOCHNumber { get; set; }
            public List<TestReportTestPlan> TestPlans { get; set; } = new List<TestReportTestPlan>();
        }

        // Feature File
        public class TestReportTestPlan
        {
            public string? TestPlanName { get; set; }
            public string? FeatureName { get; set; }
            public DateTime? TestPlanStart { get; set; }
            public DateTime? TestPlanEnd { get; set; }
            public long? TestPlanExecution { get; set; }
            public string? EPOCHNumber { get; set; }
            public List<TestReportScenarioPlan> ScenarioPlans { get; set; } = new List<TestReportScenarioPlan>();
        }

        // Scenario
        public class TestReportScenarioPlan
        {
            public string? TestScenarioName { get; set; }
            public DateTime? TestScenarioStart { get; set; }
            public DateTime? TestScenarioEnd { get; set; }
            public long? TestScenarioExecution { get; set; }
            public bool? TestScenarioStatus { get; set; }
            public List<TestReportStepPlan> StepPlans { get; set; } = new List<TestReportStepPlan>();
        }

        // BDD Steps
        public class TestReportStepPlan
        {
            public string? TestStepName { get; set; }
            public DateTime? TestStepStart { get; set; }
            public DateTime? TestStepEnd { get; set; }
            public long? TestStepExecution { get; set; }
            public bool? TestStepStatus { get; set; }
            public List<TestReportProcPlan> ProcPlans { get; set; } = new List<TestReportProcPlan>();
        }
        
        // Procs
        public class TestReportProcPlan
        {
            public string? TestProcName { get; set; }
            public string? Arguments { get; set; }
            public DateTime? TestProcStart { get; set; }
            public DateTime? TestProcEnd { get; set; }
            public long? TestProcExecution { get; set; }
        }

        public static string? GetJson()
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(TestReport);
            return jsonString;
        }

        public static bool WriteJsonToFile(string fileNameAndLocation = @"/AppSpecFlow/TestResults/")
        {            
            // if (!TargetConfiguration.Configuration.PerformanceTesting) return true;
            DebugOutput.Log($"WriteJsonToFile {fileNameAndLocation}");
            var jsonString = GetJson();
            if (jsonString == null) return false;
            var directory = fileNameAndLocation + EPOCHControl.Epoch;
            if (!FileUtils.DirectoryCheck(directory)) FileUtils.DirectoryCreation(directory);

            var fileNameAndLocationStatusReport = directory + "/" + "STATUS" + ".html";
            var status = HTML.UseHTML.CreateHTMLAssertionReport(TestReport);
            if (!FileUtils.FilePopulate(fileNameAndLocationStatusReport, status)) return false;

            var fileNameAndLocationReport = directory + "/" + EPOCHControl.Epoch + ".html";
            var html = HTML.UseHTML.DisplayJsonAsHtml(jsonString);
            if (!FileUtils.FilePopulate(fileNameAndLocationReport, html)) return false;

            var fileNameAndLocationHTML = directory + "/" + EPOCHControl.Epoch + "-Report.html";
            var htmlReport = HTML.UseHTML.CreateHTMLPerformanceReport(TestReport);
            if (!FileUtils.FilePopulate(fileNameAndLocationHTML, htmlReport)) return false;

            var fileNameAndLocationJson = directory + "/" + EPOCHControl.Epoch + ".json";
            if (!FileUtils.FilePopulate(fileNameAndLocationJson, jsonString)) return false;

            // create the test execution in XRAY (if switched on)
            if (TargetConfiguration.Configuration.Jira && TargetConfiguration.Configuration.JiraName != null)
            {
                var jiraDetails = Core.Configuration.TargetJiraConfiguration.GetJiraConfigurationByName(TargetConfiguration.Configuration.JiraName);
                if (jiraDetails != null)
                {
                    if (jiraDetails.XrayTestExecutions)
                    {                        
                        // Create a Test Execution  
                        var testExecutionKey = Core.Jira.Jira.CreateTestExecution();
                        if (testExecutionKey == null) return false;
                        // DebugOutput.Log($"Empty Test Execution Created with Key: {testExecutionKey}");


                        // Jira Execution Import File Creation
                        var xrayModel = new Core.Jira.XRAY.Execution.Model.Root();
                        var currentModel = TestReport;

                        // where do we store the test execution we created?
                        var xrayModelInfo  = new Core.Jira.XRAY.Execution.Model.Info();
                        xrayModelInfo.summary = "Test Execution Summary - Automated";
                        xrayModelInfo.description = "These test have been automated.";
                        xrayModelInfo.startDate =  currentModel.TestPlans[0].TestPlanStart;
                        xrayModelInfo.finishDate = currentModel.TestPlans[0].TestPlanEnd;   
                        xrayModel.info = new Core.Jira.XRAY.Execution.Model.Info();
                        xrayModel.info.testExecutionKey = testExecutionKey;   

                        var xrayModelTests = new List<Core.Jira.XRAY.Execution.Model.Test>();

                        foreach (var testPlanA in currentModel.TestPlans)
                        {
                            foreach (var scenarioPlan in testPlanA.ScenarioPlans)
                            {
                                var xrayModelTest = new Core.Jira.XRAY.Execution.Model.Test();
                                var testStatus = "PASSED";
                                var testStartTime = scenarioPlan.StepPlans[0].TestStepStart;
                                DateTime? testEndTime = null;
                                var testName = scenarioPlan.TestScenarioName;
                                foreach (var stepPlan in scenarioPlan.StepPlans)
                                {
                                    if (stepPlan.TestStepStatus == false) testStatus = "FAILED";
                                    testEndTime = stepPlan.TestStepEnd;
                                }
                                if (testName != null)
                                {
                                    if (testName.Contains("HUX"))
                                    {
                                        // only want the HUX-1480
                                        xrayModelTest.testKey = testName.Length >= 8 ? testName.Substring(0, 8) : testName;
                                    }
                                    else if (testName.Contains("HX25"))
                                    {
                                        // HX25-1480  only the 9 chars!
                                        xrayModelTest.testKey = testName.Length >= 9 ? testName.Substring(0, 9) : testName;
                                    }
                                    else
                                    {
                                        xrayModelTest.testKey = testName;
                                    }
                                }
                                xrayModelTest.start = testStartTime;
                                xrayModelTest.finish = testEndTime;
                                xrayModelTest.status = testStatus;
                                if (testStatus == "FAILED") xrayModelTest.comment = "Requires Human Intervention";
                                xrayModelTests.Add(xrayModelTest);
                            }
                        }
                        xrayModel.tests = xrayModelTests;
                        DebugOutput.Log($"We should have an xray model now!");
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        var xrayJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(xrayModel, settings);
                        var jiraExecutionFileAndLocation = directory + "/" + EPOCHControl.Epoch + "-JiraExecution.json";
                        if (!FileUtils.FilePopulate(jiraExecutionFileAndLocation, xrayJsonString)) return false;

                        // We need to create a json string of tests executed and their results
                        string testResults = "";
                        var report = TestReport;
                        var testPlan = report.TestPlans;
                        foreach (var test in testPlan)
                        {
                            var testStatus = "";
                            var testName = "";
                            var scenarioPlan = test.ScenarioPlans;
                            var failureComment = "";
                            foreach (var scenario in scenarioPlan)
                            {
                                var tempTestName = scenario.TestScenarioName;
                                if (tempTestName != null)
                                {
                                    var tempTestNameBrokenUp = tempTestName.Split("-");
                                    testName = tempTestNameBrokenUp[0] + "-" + tempTestNameBrokenUp[1];
                                    bool foundFail = false;
                                    foreach (var step in scenario.StepPlans)
                                    {
                                        if (!foundFail)
                                        {
                                            var thisStepStatus = step.TestStepStatus??false;
                                            if (thisStepStatus)
                                            {
                                                testStatus = "PASSED";
                                            }
                                            else
                                            {
                                                testStatus = "FAILED";
                                                if (step.TestStepName != null)
                                                {
                                                    var stepText = step.TestStepName.Replace("\"", "'");
                                                    failureComment = "FAILED STEP => " + stepText;
                                                    var stepTextForEvidence = stepText.Replace("'", "");
                                                    // does the image file exist
                                                    var imageDirectory = FileUtils.GetErrorImagesDirectory();
                                                    var listOfFiles = FileUtils.OSGetListOfFilesInDirectoryOfTypeContainingWord(imageDirectory, "png", stepTextForEvidence);
                                                    if (listOfFiles == null || listOfFiles.Count < 1)
                                                    {
                                                        DebugOutput.Log($"No image file found matching {stepTextForEvidence}*.png");    
                                                    }
                                                    else
                                                    {
                                                        var wantedFile = listOfFiles[0];
                                                        var fileName = wantedFile;
                                                        var contentType = "image/png";
                                                        failureComment = failureComment + "£NO IMAGE DATA" + "£" + imageDirectory + fileName + "£" + contentType;
                                                    }
                                                }
                                                else
                                                {
                                                    failureComment = "Unable to find failure step!  check for weird chars in the step!";
                                                }
                                                foundFail = true;
                                            }
                                        }
                                    }
                                    if (!foundFail) failureComment = "No Comment";
                                }
                            }
                            testResults = testResults + testName + "," + testStatus + "," + failureComment + "|";
                        }
                        // remove the | from the testResults, if the last char is |
                        if (testResults.Length > 0)
                        {
                            if (testResults[testResults.Length - 1] == '|')
                            {
                                testResults = testResults.Substring(0, testResults.Length - 1);
                            }
                        }
                        DebugOutput.Log(testResults);
                        // I need token {token} test execution key {testExecutionKey} and the results {testResults}
                        if (!Core.Jira.Jira.UpdateTestStatusInTestExecution(testExecutionKey, testResults))
                        {
                            DebugOutput.Log($"Failed to update the test execution with the results!");
                            return false;                            
                        }
                    }
                }
            }
            
            var test1 = directory + "/" + "goodbye1.txt";
            FileUtils.FilePopulate(@"test1", "hello");
            // FileUtils.FilePopulate(@"/AppSpecFlow/TestResults/goodbye.txt", "hello");

            if (TargetConfiguration.Configuration.PerformanceComparison)
            {
                if (!RunPerformanceComparisonReport(fileNameAndLocation)) return false;
            } 
            return true;
        }

        private static bool RunPerformanceComparisonReport(string fileNameAndLocation)
        {
            // Create a List of TestPlans
            var listOfTestPlans = new List<TestReportTiming>();

            // Needs a pause as the build completes. 
            Thread.Sleep(1000);

            // Get a list of ALL the Json Files (and location)
            var repoDirectory = FileUtils.GetRepoDirectory();
            var testResultsDirectory = repoDirectory + fileNameAndLocation;

                // List of all Directories in here
            var directories = FileUtils.OSGetListOfDirectoriesInDirectory(testResultsDirectory);
            var numberOfDirectories = directories.Count;         

            // ready every Json File and populate List of TestPlans
            foreach (var directory in directories)
            {
                if (!directory.ToLower().Contains("deploy"))
                {
                    // need the last directory to get the EPOCH
                    var brokenUpDirectory = StringValues.BreakUpByDelimitedToList(directory, @"\");
                    if (brokenUpDirectory != null)
                    {
                        var epoch = brokenUpDirectory[brokenUpDirectory.Count - 1];
                        var jsonFile = directory + @"\" + epoch + ".json";
                        var jsonString = FileUtils.OSGetAllTextInFile(jsonFile);
                        var newModel = GetReportFromJson(jsonString);
                        if (newModel != null) listOfTestPlans.Add(newModel);
                    }
                }
            }

            // Get all the Steps together! - and we'll see how we get on at this point
            var listOfSteps = new List<TestReportStepPlan>();
            DateTime? end = null;
            DateTime? start = null;
            var listOfMethods = new List<TestReportProcPlan>();

            foreach(var tests in listOfTestPlans)
            {
                foreach (var testPlan in tests.TestPlans)
                {
                    foreach (var testScens in testPlan.ScenarioPlans)
                    {
                        if (end == null)
                        {
                            end = testScens.TestScenarioEnd;
                            start = testScens.TestScenarioStart;
                        } 
                        if (testScens.TestScenarioEnd > end)
                        {
                            end = testScens.TestScenarioEnd;
                            start = testScens.TestScenarioStart;
                        } 
                        foreach (var testSteps in testScens.StepPlans)
                        {
                            var status = testSteps.TestStepStatus??false;
                            if (status) listOfSteps.Add(testSteps);       
                            foreach (var procPlan in testSteps.ProcPlans)
                            {
                                listOfMethods.Add(procPlan);
                            }                     
                        }
                    }
                }
            }

            var lastRunStart = start??DateTime.Now;
            var lastRunEnd = end??DateTime.Now;

            // Create the BDD Step Performance Compare
            var reportString = HTML.UseHTML.CreateHTMLPerformanceComparisonReport(listOfSteps, lastRunStart, lastRunEnd);
            FileUtils.FilePopulate(@"\AppSpecFlow\TestResults\StepPerformanceComparison.html", reportString); 

            // Create the Method Performance Compare
            var reportMethodString = HTML.UseHTML.CreateHTMLMethodPerformanceComparisonReport(listOfMethods, lastRunStart, lastRunEnd);
            FileUtils.FilePopulate(@"\AppSpecFlow\TestResults\MethodPerformanceComparison.html", reportMethodString); 


            // show me we got here!
            
            return false;
        }


        private static TestReportTiming? GetReportFromJson(string jsonString)
        {
            TestReportTiming? myDeserializedClass = JsonConvert.DeserializeObject<TestReportTiming>(jsonString);
            if (myDeserializedClass == null) return null;
            return myDeserializedClass;
        }

        public static int GetHowManyFeatures()
        {
            return TestReport.TestPlans.Count;
        }

        public static int GetHowManyScenarios()
        {
            int howManyScenarios = 0;
            foreach (var testPlan in TestReport.TestPlans)
            {
                howManyScenarios += testPlan.ScenarioPlans.Count;
            }
            return howManyScenarios;
        }

        public static int GetHowManySteps()
        {
            int howManySteps = 0;
            foreach (var testPlan in TestReport.TestPlans)
            {
                foreach (var scenarioPlan in testPlan.ScenarioPlans)
                {
                    howManySteps += scenarioPlan.StepPlans.Count;
                }
            }
            return howManySteps;
        }

        public static int GetHowManyStepFailures()
        {
            int howManyStepsFailed = 0;
            foreach (var testPlan in TestReport.TestPlans)
            {
                foreach (var scenarioPlan in testPlan.ScenarioPlans)
                {
                    foreach (var stepPlan in scenarioPlan.StepPlans)
                    {
                        bool failed = stepPlan.TestStepStatus??false;
                        if (!failed) howManyStepsFailed++;
                    }
                }
            }
            return howManyStepsFailed;
        }


        // Test Report -> Test Plan -> Scenario Plan -> Step Plan -> Proc Plan 
        public static void NewTestReport()
        {
            var newTestReport = new TestReportTiming
            {
                EPOCHNumber = EPOCHControl.Epoch
            };
            TestReport = newTestReport;
        }

        public static void NewTestPlan(string featureName = "")
        {
            var newTestPlan = new TestReportTestPlan
            {
                TestPlanStart = DateTime.Now,
                EPOCHNumber = EPOCHControl.Epoch
            };
            if (featureName != "") newTestPlan.FeatureName = featureName;
            TestReport.TestPlans.Add(newTestPlan);
        }

        public static void NewScenarioPlan(string? scenarioName = null)
        {
            CloseScenarioPlan();
            var newScenarioPlan = new TestReportScenarioPlan
            {
                TestScenarioStart = DateTime.Now,
                TestScenarioName = scenarioName
            };
            int howManyTestPlans = TestReport.TestPlans.Count;
            TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans.Add(newScenarioPlan);
        }

        public static void NewStepPlan(string? stepName = null)
        {
            CloseStepPlan();
            var newStepPlan = new TestReportStepPlan
            {
                TestStepStart = DateTime.Now,
                TestStepName = stepName
            };
            int howManyTestPlans = TestReport.TestPlans.Count;
            int howManyScenarioPlans = TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans.Count;
            TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans[howManyScenarioPlans - 1].StepPlans.Add(newStepPlan);
        }

        public static void NewProcPlan(string? procName = null, string arguments = "")
        {
            CloseProcPlan();
            var newProcPlan = new TestReportProcPlan
            {
                TestProcStart = DateTime.Now,
                TestProcName = procName
            };
            if (arguments != "") newProcPlan.Arguments = arguments;
            
            // Safety check for unit tests or external callers that don't use TestReport structure
            int howManyTestPlans = TestReport.TestPlans.Count;
            if (howManyTestPlans == 0) return;
            
            int howManyScenarioPlans = TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans.Count;
            if (howManyScenarioPlans == 0) return;
            
            int howManyStepPlans = TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans[howManyScenarioPlans - 1].StepPlans.Count;
            if (howManyStepPlans == 0) return;
            
            TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans[howManyScenarioPlans - 1].StepPlans[howManyStepPlans -1].ProcPlans.Add(newProcPlan);
        }

        public static void CloseProcPlan()
        {
            // Safety check for unit tests or external callers that don't use TestReport structure
            int howManyTestPlans = TestReport.TestPlans.Count;
            if (howManyTestPlans == 0) return;
            
            int howManyScenarioPlans = TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans.Count;
            if (howManyScenarioPlans == 0) return;
            
            int howManyStepPlans = TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans[howManyScenarioPlans - 1].StepPlans.Count;
            if (howManyStepPlans == 0) return;
            
            foreach (var proc in TestReport.TestPlans[howManyTestPlans -1].ScenarioPlans[howManyScenarioPlans -1].StepPlans[howManyStepPlans -1].ProcPlans)
            {
                if (proc.TestProcEnd == null) proc.TestProcEnd = DateTime.Now;
            }
            // WriteJsonToFile();
        }

        public static void CloseStepPlan(bool pass = false)
        {
            int howManyTestPlans = TestReport.TestPlans.Count;
            int howManyScenarioPlans = TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans.Count;
            foreach (var step in TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans[howManyScenarioPlans - 1].StepPlans)
            {
                if (step.TestStepEnd == null) step.TestStepEnd = DateTime.Now;
                if (step.TestStepStatus == null) step.TestStepStatus = pass;
            }
            // WriteJsonToFile();
        }

        public static void CloseScenarioPlan()
        {
            int howManyTestPlans = TestReport.TestPlans.Count();
            foreach (var scenario in TestReport.TestPlans[howManyTestPlans - 1].ScenarioPlans)
            {
                if (scenario.TestScenarioEnd == null) scenario.TestScenarioEnd = DateTime.Now;
            }
            // WriteJsonToFile();
        }

        private static long GetDifferenceInMilliSeconds(DateTime? start, DateTime? end)
        {
            var newStart = start??DateTime.Now;
            var newEnd = end??DateTime.Now;
            long diff = newEnd.Ticks - newStart.Ticks;
            if (diff < 0) diff = diff * -1;
            return diff;
        }

        public static void CloseTestPlan()
        {
            foreach(var testPlan in TestReport.TestPlans)
            {
                DateTime? date = null;
                if (testPlan.TestPlanEnd == null)
                {
                    testPlan.TestPlanEnd = DateTime.Now;
                }
                date = testPlan.TestPlanEnd;
                testPlan.TestPlanExecution = GetDifferenceInMilliSeconds(testPlan.TestPlanStart, testPlan.TestPlanEnd);
                foreach (var scenarioPlan in testPlan.ScenarioPlans)
                {
                    if (scenarioPlan.TestScenarioEnd == null)
                    {
                        scenarioPlan.TestScenarioEnd = date;
                    }
                    date = scenarioPlan.TestScenarioEnd;
                    scenarioPlan.TestScenarioExecution = GetDifferenceInMilliSeconds(scenarioPlan.TestScenarioStart, scenarioPlan.TestScenarioEnd);
                    foreach (var stepPlan in scenarioPlan.StepPlans)
                    {
                        if (stepPlan.TestStepEnd == null)
                        {
                            stepPlan.TestStepEnd = date;
                        }
                        date = stepPlan.TestStepEnd;
                        stepPlan.TestStepExecution = GetDifferenceInMilliSeconds(stepPlan.TestStepStart, stepPlan.TestStepEnd);
                        foreach(var procPlan in stepPlan.ProcPlans)
                        {
                            if (procPlan.TestProcEnd == null)
                            {
                                procPlan.TestProcEnd = date;
                            }
                            date = procPlan.TestProcEnd;
                            procPlan.TestProcExecution = GetDifferenceInMilliSeconds(procPlan.TestProcStart, procPlan.TestProcEnd);
                        }
                    }
                }
            }
            if (!WriteJsonToFile()) DebugOutput.Log($"FAILURE TO WRITE ALL FILES!");
        }


    }


    
}
