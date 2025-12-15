using System.Reflection;
using AppXAPI;
using BoDi;
using Core;
using Core.Configuration;
using Core.Logging;
using Core.Jira;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Core.FileIO;
using Reqnroll;

namespace AppSpecFlow.Libs
{
    [Binding]
    public class Hooks
    {
        public static string TestExecutionKey = "";
        public static string TestBeingRun = "";
        public static bool TestInJira = false;
        public static bool TestEditableInJira = false;        
        public static List<string> TestsThatDoNotUpdateJiraGherkin = new List<string>{"SDIACSC-559", "BatchSystem-001"};

        [BeforeTestRun]
        public static void TestSetup()
        {
            var assembly = Assembly.Load("AppTargets");
            TargetForms.Instance.PopulateList(assembly);
            // Reads the TargetSettings.<ENV> found in AppTargets\Resources  - Application Configuration
            TargetConfiguration.ReadJson();

            // Reads the Locators found in Core\Configuration\Resources - Locators Lists
            TargetLocator.ReadJson();

            // Reads the browser.<driver>.<ENV> found in AppTargets\Resources\Browsers - Browser Settings
            Drivers.ReadJson();

            // Read the Var-<ENV> found in AppTargets\Resources\Variables - Variables Settings
            var x = JsonValues.ReadAPIListJsonFile();
            if (x != null) VariableConfiguration.GetVariableModel(x);    

            // Set the debug level        
            HooksCode.SetDebug(TargetConfiguration.Configuration.Debug);

            // IF TestReport Setup new Test Report
            if (TargetConfiguration.Configuration.TestReport) TargetTestReport.NewTestReport();

            // IF Jira Config in the TargetSettings Read AppTargets\Resources\Variables\Jira\JiraSettings<ENV>
            if (TargetConfiguration.Configuration.Jira) TargetJiraConfiguration.ReadJson();

            // IF AIChatBot Config in the TargetSettings Read AppTargets\Resources\Variables\AIChatBot\AIChatBot.<ENV>
            if (TargetConfiguration.Configuration.AIChatBot) TargetAIChatBotConfiguration.ReadJson();

            // IF AnswerIsAnalysed Config set up New Analysis Report
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed && TargetConfiguration.Configuration.AIChatBot) TargetAnalysisReport.NewAnalysisReport();
        }


        [AfterTestRun]
        public static void TestCleanUp()
        {
            HooksCode.CheckShutDown();
            if (TargetConfiguration.Configuration.TestReport) TargetTestReport.CloseTestPlan();
        }

        [BeforeFeature]
        public static void FeatureSetup(FeatureContext featureContext)
        {
            var feature = featureContext.FeatureInfo.Title;
            HooksCode.SetEpoch();
            if (TargetConfiguration.Configuration.TestReport) TargetTestReport.NewTestPlan(feature);
        }

        [AfterFeature]
        public static void FeatureCloseDown(FeatureContext featureContext)
        {
            var feature = featureContext.FeatureInfo.Title;            
        }

        [BeforeScenario]
        public void ScenarioSetup(ScenarioContext scenarioContext)
        {
            RegisterTypes(scenarioContext);
            var scenario = scenarioContext.ScenarioInfo.Title;

            if (TargetConfiguration.Configuration.TestReport) TargetTestReport.NewScenarioPlan(scenario);
            DebugOutput.Log($"Scenario - {scenario}");    
            var newScenarioTitle = GetScenarioTitle(scenarioContext);
            if (TargetConfiguration.Configuration.Jira && TargetConfiguration.Configuration.JiraName != null) HooksCode.JiraBeforeScenario(newScenarioTitle);
            
            DebugOutput.Log($"ScenarioSetup  ENTRANCE ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        }

        private static string GetScenarioTitle(ScenarioContext scenarioContext)
        {
            var scenarioTitle = scenarioContext.ScenarioInfo.Title;
            var scenarioList = StringValues.BreakUpByDelimitedToList(scenarioTitle,"-");
            var newScenarioTitle = scenarioList[0] + "-" + scenarioList[1];
            return newScenarioTitle;
        }


        [AfterScenario]
        public void ScenarioCleanUp(ScenarioContext scenarioContext)
        {
            DebugOutput.Log($"ScenarioCleanUp  ENTRANCE ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            // Generic.Hooks.Binding.AfterScenario(scenarioContext); 
            var newScenarioTitle = GetScenarioTitle(scenarioContext);     
            if (TargetConfiguration.Configuration.TestReport)
            {                
                DebugOutput.Log($"TEST REPORTING  ENTRANCE ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                TargetTestReport.CloseScenarioPlan();
            } 
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed) TargetAnalysisReport.CloseJsonForPython();   // produces JsonFromAutomatedAI.json in AppXAPI/APIOutFiles
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsAnalysed) TargetAnalysisReport.CloseAnalysisReport(); // READ JsonFromAutomatedAI.json in AppXAPI/APIOutFiles CREATE ANALYSIS CLASS
            if (TargetAIChatBotConfiguration.Configuration.AnswerIsCompared) TargetAnalysisReport.CloseAnswerComparisonReport(newScenarioTitle); // READ ANALYSIS CLASS
            if (TargetConfiguration.Configuration.Jira && TargetConfiguration.Configuration.JiraName != null) HooksCode.JiraAfterScenario(newScenarioTitle, scenarioContext); 
        }


        [BeforeStep]
        public void StepSetup(ScenarioContext scenarioContext)
        {
            var stepContext = scenarioContext.StepContext;
            DebugOutput.Log("Step - " + stepContext.StepInfo.StepDefinitionType + " " + stepContext.StepInfo.Text);
            if (TargetConfiguration.Configuration.TestReport) TargetTestReport.NewStepPlan(stepContext.StepInfo.StepDefinitionType + " " + stepContext.StepInfo.Text);
            if (TargetConfiguration.Configuration.Jira && TargetConfiguration.Configuration.JiraName != null) HooksCode.JiraBeforeStep(scenarioContext);
        }

        [AfterStep]
        public void StepCleanUp(ScenarioContext scenarioContext)
        {
            // Generic.Hooks.Binding.AfterStep(scenarioContext);
            var stepStatus = scenarioContext.ScenarioExecutionStatus.ToString();
            bool pass = false;
            if (stepStatus.Contains("OK")) pass = true;
            if (stepStatus.Contains("StepDefinitionPending")) pass = false;
            if (stepStatus.Contains("UndefinedStep")) pass = false;
            if (stepStatus.Contains("BindingError")) pass = false;
            if (stepStatus.Contains("TestError")) pass = false;
            if (stepStatus.Contains("MissingStepDefinition")) pass = false;
            if (TargetConfiguration.Configuration.TestReport) TargetTestReport.CloseStepPlan(pass);
        }
        



        //   PRIVATE 


        /// <summary>
        /// NEVER REMOVE!  This creates the step helper link to the forms
        /// </summary>
        /// <param name="scenarioContext"></param>
        private static void RegisterTypes(ScenarioContext scenarioContext)
        {
            var container = (IObjectContainer)scenarioContext.GetBindingInstance(typeof(IObjectContainer));
            container.RegisterTypeAs<StepHelpers, IStepHelpers>();
            container.RegisterInstanceAs<ITargetForms>(TargetForms.Instance);
        }

    }
}