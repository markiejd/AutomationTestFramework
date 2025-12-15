using Core.Configuration;
using Core.Logging;
using Core.UnitTests;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;
using Core;
using Core.FileIO;
using Core.Transformations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Generic.Steps.Helpers.Classes;

namespace Generic.Steps
{
    [Binding]
    public class GivenSteps : StepsBase
    {

        public GivenSteps(IStepHelpers helpers) : base(helpers)
        {
        }
    
        [Given(@"I Have Failed")]
        public static void GivenIHaveFailed()
        {
            string proc = "Given I Have Failed";
            CombinedSteps.Failure(proc);
            return;
        }


        [Given(@"Test A Works")]
        public void GivenTestAWorks()
        {
            var x = APIUtil.GetResponseWithAccessToken().Result;
            DebugOutput.Log($"THis response is {x.StatusCode}");
            DebugOutput.Log($"THis CONTENT is {x.Content}");
            var response = APIUtil.GetStringWithAccessToken().Result;
            DebugOutput.Log($"WE GOT A RESPONSE {response}");

        }


        [Given(@"Unit Test Is Executed In Core")]
        public void GivenUnitTestIsExecutedInCore()
        {
            var x = CoreUnitTests.Hello();
            DebugOutput.Log($"WE have x from Core {x}");
            var allPass = true;
            // if (!CoreUnitTests.PassAddJsonStringToListOfAnalusedAnswersSupplied_NotNull_Count()) allPass = false;
            // if (!CoreUnitTests.Pass_ReturnNowDateAsString_Default()) allPass = false;
            // if (!CoreUnitTests.Pass_ReturnFirstOfThisMonth_Default()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateOnlyFromDateTime()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromDateString()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromDateString_Format_Default())  allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromStringX_Now()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromStringX_Today()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromStringX_Today_Minus_Day()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromStringX_Today_Minus_Hour()) allPass = false;
            // if (!CoreUnitTests.Pass_GetDateTimeFromStringX_Today_Minus_Minute()) allPass = false;
            // if (!CoreUnitTests.Pass_GetSecondsBetweenDateTimes()) allPass = false;
            // if (!CoreUnitTests.Pass_IsTwoDateTimesWithinSeconds()) allPass = false;
            // if (!CoreUnitTests.Pass_BreakUpDateAndTime()) allPass = false;
            // if (!CoreUnitTests.Pass_Format_Default()) allPass = false;
            // if (!CoreUnitTests.Pass_Format_1()) allPass = false;
            // if (!CoreUnitTests.Pass_Format_2()) allPass = false;
            // if (!CoreUnitTests.Pass_Format_23()) allPass = false;
            // if (!CoreUnitTests.Pass_MathsToDate()) allPass = false;
            // if (!CoreUnitTests.Pass_TurnStringDateAround()) allPass = false;
            // if (!CoreUnitTests.Pass_GetTimeInUnix()) allPass = false;
            // if (!CoreUnitTests.Pass_GetStaticUnix()) allPass = false;
 
            if (!allPass) Assert.Fail();
        }


        [Given(@"Unit Test Is Executed In Generic")]
        public void GivenUnitTestIsExecutedInGeneric()
        {
            DebugOutput.Log($"We are in Generic Given!");
        }


        
        [Given(@"Application Set As ""(.*)""")]
        public void GivenApplicationSetAs(string applicationName)
        {
            TargetConfiguration.Configuration.AreaPath = applicationName;
            DebugOutput.Log($"Application = TargetConfiguration.Configuration.AreaPath");
        }


        [Given(@"App ""([^""]*)"" Is Open")]
        public void GivenAppIsOpen(string applicationExePath)
        {
            string proc = $"Given App {applicationExePath} Is Open";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Application.OpenApplication(applicationExePath)) return;
                CombinedSteps.Failure("There has been a failure when trying to get the windows driver to be active");
            }            
        }

        [Given(@"App Is Closed")]
        public void GivenAppIsClosed()
        {
            string proc = $"Given App Is Close";
            if (!CombinedSteps.OuputProc(proc))
            {
                DebugOutput.Log($"We still want to try and close the app - even after failure");
            }
            Application.CloseWinDriver();
        }

        [Given(@"Browser Is Open")]
        public void GivenBrowserIsOpen()
        {
            string proc = $"Given Browser Is Open";
            DebugOutput.Log(proc);            
            if (TargetConfiguration.Configuration == null) return;
            var expectedBrowser = TargetConfiguration.Configuration.Browser;
            GivenBrowserIsOpen(expectedBrowser);
        }


        [Given(@"Browser ""([^""]*)"" Is Open")]
        public void GivenBrowserIsOpen(string browser)
        {
            string proc = $"Given Browser {browser} Is Open";
            if (ElementInteraction.IsDriverActive())
            {
                DebugOutput.Log($"There is already a web driver running!");
                Drivers.CloseWebBrowser();
            }

            var compositeBrowserSize = StringValues.BreakUpByDelimited(TargetConfiguration.Configuration.ScreenSize,"x");
            int length = 1200;
            int height = 800;
            if (compositeBrowserSize.Count() != 2)
            {
                DebugOutput.Log($"We need 2 sizes, height and length!  But we won't crash out at this point! We just set it to 800x800");
            }
            else
            {
                try
                {
                    length = Int32.Parse(compositeBrowserSize[0]);
                    height = Int32.Parse(compositeBrowserSize[1]);
                }
                catch
                {
                    DebugOutput.Log($"FAiled to convert {compositeBrowserSize[0]} or {compositeBrowserSize[1]} to an int!  Need an Int");
                }
            }

            if (CombinedSteps.OuputProc(proc))
            {
                switch (browser.ToLower())
                {
                    default:
                    case "chrome":
                        {
                            Drivers.ChromeDriver();
                            break;
                        }
                    case "edge":
                        {
                            Drivers.EdgeDriver();
                            break;
                        }
                    case "firefox":
                        {
                            Drivers.FireFoxDriver();
                            break;
                        }
                    case "ie":
                    case "internet explorer":
                        {
                            Drivers.InternetExplorerDriver();
                            break;
                        }
                }
                if (TargetConfiguration.Configuration == null) return ;
                TargetConfiguration.Configuration.ApplicationType = "web";
                ElementInteraction.SetWindowSize(length, height);
            }
        }

        [Given(@"Browser Is Closed")]
        public void GivenBrowserIsClosed()
        {
            if (ElementInteraction.IsDriverActive())
            {
                DebugOutput.Log($"There is currently no web driver recorded!");
                return;
            }
            Drivers.CloseWebBrowser();
            TargetConfiguration.Configuration.AreaPath = "";
            return;
        }

        
        [Given(@"File ""(.*)"" Exists")]
        public bool GivenFileExists(string fullFileLocation)
        {
            string proc = $"Given File {fullFileLocation} Exists";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.OSFileCheck(fullFileLocation))
                {
                    DebugOutput.Log($"This is a GIVEN - make it happen if not there!");
                    if (!FileUtils.OSFileCreation(fullFileLocation))
                    {
                        CombinedSteps.Failure($"There has been a failure when trying to create the file {fullFileLocation}");
                        return false;
                    }
                }
                DebugOutput.Log($"File {fullFileLocation} exists!");
                return true;
            }
            return false;
        }

        
        [Given(@"File ""(.*)"" Does Not Exists")]
        public bool GivenFileDoesNotExists(string fullFileLocation)
        {
            string proc = $"Given File {fullFileLocation} Does Not Exists";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!FileUtils.OSFileCheck(fullFileLocation))
                {
                    DebugOutput.Log($"It already does not exist!");
                    return true;
                }
                DebugOutput.Log($"It does exists - this is a given - so get rid of it!");
                if (!FileUtils.OSFileDeletion(fullFileLocation))
                {
                        CombinedSteps.Failure($"Failed to delete {fullFileLocation}");
                        return false;                    
                }
                DebugOutput.Log("Its gone!");
                return true;
            }
            return false;
        }

    }
}
