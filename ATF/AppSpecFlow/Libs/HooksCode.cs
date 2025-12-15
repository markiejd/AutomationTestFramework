
using Core.Configuration;
using Core.FileIO;
using Core.Jira;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Classes;
using Reqnroll;

namespace AppSpecFlow.Libs
{
    public class HooksCode
    {
        public static string TestBeingRun = "";
        public static bool TestEditableInJira = false;    
        public static string TestExecutionKey = "";
        public static bool TestInJira = false;

        public static string Hello()
        {
            return "hello";
        }

        public static void CheckShutDown()
        {        
            ElementInteraction.WebShutdown();
        }

        public static void JiraAfterScenario(string newScenarioTitle, ScenarioContext scenarioContext)
        {            
            //  First check is this test even in JIRA?                          
            var model = Jira.GetJiraModelFromIssue(newScenarioTitle);
            if (model == null) return;


            var TestEditableInJira = Jira.IsIssueEditable(model);
            DebugOutput.Log($"THIS TEST IS EDITABLE? {TestEditableInJira}");
            if (TestEditableInJira)
            {
                DebugOutput.Log($"EDITABLE tests");
                // No execution - just editable!
                if (scenarioContext.TestError != null )
                {
                    // We do not update if a test FAILS
                    DebugOutput.JiraOutput($"There has been an error in the test - we do not update EDITABLE tests that are erroring!");
                    return;
                }

                // if (!TestsThatDoNotUpdateJiraGherkin.Contains(newScenarioTitle))
                // {
                    // If Jira and ATF are not the same
                    if (!Jira.IsJiraAndATFTheSame(newScenarioTitle, model))
                    {
                        // Update JIRA from ATF
                        if (!Jira.UpdateBDDInJira(newScenarioTitle)) return;
                    }
                // }
                
                return;
            }
            
            // EXECUTABLE TESTS ONLY HERE!
            // Pass or Fail!
            if (scenarioContext.TestError == null)
            {
                DebugOutput.Log($"Executed SCENARIO and PASSED");
                // It may have passed, but if it is not the same I'm not going to mark it as pass!                    
                // if (!TestsThatDoNotUpdateJiraGherkin.Contains(newScenarioTitle))
                // {
                    if (!Jira.IsJiraAndATFTheSame(newScenarioTitle, model))
                    {
                        // Can not execute tests where ATF and JIRA are not the same!
                        DebugOutput.JiraOutput($"We are attempting to execute {newScenarioTitle} but Jira's Cucumber and ATF's differ - I can not update an execution where these do not match!");
                        return;
                    }
                // }
                if (!Jira.UpdateTestStatusInTestExecution(TestExecutionKey, TestBeingRun, "PASS", "Rand by ATF")) return;
            }
            else
            {
                if (!Jira.UpdateTestStatusInTestExecution(TestExecutionKey, TestBeingRun, "FAIL", "Rand by ATF")) return;
            }
            if (scenarioContext.TestError != null)
            {
                DebugOutput.Log($"Executed SCENARIO and FAILED");    
                var fileName = "Jira-" + TestExecutionKey;     
            }
            

            // Set Upload Flag - Has to be execution done
            Thread.Sleep(1000);
            var epoch = EPOCHControl.Epoch;
            DebugOutput.Log($"We are lookign in folder {epoch}");
            var testOutputDir =  FileUtils.GetCorrectDirectory("/AppSpecFlow/TestResults/");
            var fullFileName = testOutputDir + epoch;
            DebugOutput.Log($"FULL place looking is {fullFileName}");
            FileUtils.OSFileCreation(fullFileName + @"/" + TestExecutionKey + ".jira");
            FileUtils.OSFileCreation(fullFileName + @"/" + newScenarioTitle + ".test");
        }

        public static void JiraBeforeScenario(string newScenarioTitle)
        {            
            TestBeingRun = newScenarioTitle;
            Jira.AddScenario(newScenarioTitle);
            var projectKey = TargetConfiguration.Configuration.JiraName;

            //  First check is this test editable -> Can not be executed, but can be updated!                
            var model = Jira.GetJiraModelFromIssue(newScenarioTitle);
            if (model == null)
            {
                TestInJira = false;
                return;
            }

            TestEditableInJira = Jira.IsIssueEditable(model);
            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return;
            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return;
            if (jiraDetails.Authorization.ToLower() == "cloud")
            {
                DebugOutput.Log($"We don't create the test execution for each scenario!  Its for each Test Run.. so nothing here for cloud!");
                return;
            }

            /// Non cloud below
            if (!TestEditableInJira)
            {
                // The test is in a situation where it can be executed - can not be updated!
                // Create the Execution    
                TestExecutionKey = Jira.CreateTestExecution() ?? "";
                if (TestExecutionKey == "" || TestExecutionKey == null) return;

                if (!Jira.CreateTestsInTestExecution(TestExecutionKey, newScenarioTitle)) return;

                if (!Jira.CreateIssueStepThroughWorkflow(TestExecutionKey, "11")) return;
                Thread.Sleep(100);
                if (!Jira.CreateIssueStepThroughWorkflow(TestExecutionKey, "201")) return;

                var reporterEmailAddress = Jira.GetIssuesReportersEmailAddress(TestExecutionKey);
                if (reporterEmailAddress == null) return;

                if (!Jira.CreateAssigneeToIssue(TestExecutionKey, reporterEmailAddress)) return ;
            }
        }

        /// <summary>
        /// Set the debug value
        /// </summary>
        /// <param name="debugLevelFromFile"></param>
        public static void SetDebug(int debugLevelFromFile = 7)
        {
            DebugOutput.debugLevel = debugLevelFromFile;
            DebugOutput.Log($"Debug Level set to {debugLevelFromFile}");    
        }

        public static void JiraBeforeStep(ScenarioContext scenarioContext)
        {
            var newScenarioTitle = GetScenarioTitle(scenarioContext);
            Jira.AddStepToScenario(newScenarioTitle, GetStepName(scenarioContext));
        }
        
        private static string GetScenarioTitle(ScenarioContext scenarioContext)
        {
            var scenarioTitle = scenarioContext.ScenarioInfo.Title;
            var scenarioList = StringValues.BreakUpByDelimitedToList(scenarioTitle,"-");
            var newScenarioTitle = scenarioList[0] + "-" + scenarioList[1];
            return newScenarioTitle;
        }

        
        private static string GetStepName(ScenarioContext scenarioContext)
        {
            var stepContext = scenarioContext.StepContext;
            return stepContext.StepInfo.StepDefinitionType + " " + stepContext.StepInfo.Text;
        }

        /// <summary>
        /// Set the epoch value for each feature file so unique text is available
        /// </summary>
        public static void SetEpoch()
        {
            var epochNumber = DateTime.UtcNow.Ticks / 10000000 - 63082281600;
            var epoch = epochNumber.ToString();
            EPOCHControl.Epoch = epoch;
            DebugOutput.Log($"This feature test unique id is equal to {epoch}");
        }
    }
}