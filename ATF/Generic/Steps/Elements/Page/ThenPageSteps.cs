using System.Configuration;
using System.Diagnostics;
using AppXAPI;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.CommonModels;

namespace Generic.Elements.Steps.Page
{
    [Binding]
    public class ThenPageSteps : StepsBase
    {
        public ThenPageSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [Then(@"Page ""(.*)"" Is Not Displayed")]
        public void ThenPageIsNotDisplayed(string pageName)
        {
            string proc = $"Then Page Is Not Displayed {pageName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Page.IsDisplayed(pageName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }

        }


        [Then(@"Frame ""(.*)"" Is Displayed")]
        public void ThenFrameIsDisplayed(string frame)
        {
            string proc = $"Then Frame {frame} Is Displayed";
            var currentPage = Helpers.Page.CurrentPage.Name;
            DebugOutput.Log($"So the current page is {currentPage} and I have frame {frame}");
            var pageName = currentPage.Replace(" page","");
            pageName = pageName + frame + " page";
            pageName = pageName.Replace(" ","");
            DebugOutput.Log($"Frame is {pageName}");
            ThenPageIsDisplayed(pageName);
        }


        [Then(@"Wait For Page ""(.*)"" To Be Displayed")]
        public bool ThenWaitForPageToBeDisplayed(string pageName)
        {
            string proc = $"Then Wait For Page {pageName} To Be Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Page.IsExists(pageName))
                {
                    int counter = 0;
                    int timeOut = TargetConfiguration.Configuration.PositiveTimeout;
                    while(counter <= timeOut)
                    {
                        if (Helpers.Page.IsDisplayed(pageName))
                        {
                            Helpers.Page.SetCurrentPage(pageName);    
                            DebugOutput.Log($"The page {pageName} ID was found, AND displayed?  Nice!");
                            return true;
                        }
                        else
                        {
                            DebugOutput.Log($"The page {pageName} ID was found, but not displayed?  Try to use another locator!");
                            if (Helpers.Spinner.SpinnerIsGone())
                            {
                                DebugOutput.Log($"There was a spinner! its gone now!");
                                if (Helpers.Page.IsDisplayed(pageName, 1))
                                {
                                    DebugOutput.Log($"The page {pageName} ID was found, AND displayed? we had to wait for a spinner but its done now!  Nice!");
                                    Helpers.Page.SetCurrentPage(pageName);    
                                    return true;  
                                }
                                DebugOutput.Log($"Still the ID element for page is NOT displayed!  check it is a DISPLAYED element - it has to be displayed, at the moment, your ID is not visible, so failing to use page {pageName}!");
                            }
                        }
                        counter ++;
                        Thread.Sleep(1000);
                    }
                }
                DebugOutput.Log($"Well that page has gone wrong - {pageName}");
                CombinedSteps.Failure(proc);
            }
            return false;
        }


        [Then(@"Dialog ""([^""]*)"" Is Displayed")]
        [Then(@"Page ""([^""]*)"" Is Displayed")]
        public bool ThenPageIsDisplayed(string pageName)
        {
            pageName = pageName.Replace("-"," ");
            string proc = $"Then Page {pageName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Page.IsExists(pageName))
                {
                    pageName = StringValues.CapitalizeWords(pageName);
                }
                if (Helpers.Page.IsExists(pageName))
                {
                    if (Helpers.Page.IsDisplayed(pageName))
                    {
                        DebugOutput.Log($"The page {pageName} ID was found, AND displayed?  Nice! returning true and setting {pageName} to current");
                        Helpers.Page.SetCurrentPage(pageName);    
                        return true;
                    }
                    else
                    {
                        DebugOutput.Log($"The page {pageName} ID was found, but not displayed?  Try to use another locator!");
                        if (Helpers.Spinner.SpinnerIsGone())
                        {
                            DebugOutput.Log($"There was a spinner! its gone now!");
                            if (Helpers.Page.IsDisplayed(pageName, 1))
                            {
                                DebugOutput.Log($"The page {pageName} ID was found, AND displayed? we had to wait for a spinner but its done now!  Nice!");
                                Helpers.Page.SetCurrentPage(pageName);    
                                return true;  
                            }
                            DebugOutput.Log($"Still the ID element for page is NOT displayed!  check it is a DISPLAYED element - it has to be displayed, at the moment, your ID is not visible, so failing to use page {pageName}!");
                        }
                        DebugOutput.Log($"The spinner is still spinning?!");
                        CombinedSteps.Failure(proc);
                        return false;
                    }
                }
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }

        [Then(@"Message ""([^""]*)"" Is Displayed")]
        [Then(@"Page Displays Message ""([^""]*)""")]
        public void ThenPageDisplaysMessage(string message)
        {
            string proc = $"Then Page Displays Message {message}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Page.IsMessageDisplayed(message))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Message ""([^""]*)"" Is Not Displayed")]
        public void ThenMessageIsNotDisplayed(string message)
        {
            string proc = $"Then Page Displays Message {message}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.Page.IsMessageDisplayed(message))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
