using Core;
using Core.Configuration;
using Core.Logging;
using Generic.Steps.AIChatBots.QandAResponse;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.AIChatBots
{
    [Binding]
    public class AIChatBotSteps : StepsBase
    {

        public AIChatBotSteps(IStepHelpers helpers) : base(helpers)
        {
        }
        
        private static bool Failure(string proc, string message = "***** FAILURE ******", bool flag = false)
        {
            DebugOutput.Log("***** FAILURE ****** " + message);
            CombinedSteps.Failure(proc);
            return flag;
        }

        
        [When(@"I Ask And Analyse ChatBot The Question ""(.*)""")]
        public async Task<bool> WhenIAskAndAnalyseChatBotTheQuestion(string question)
        {
            string proc = $"When I Ask And Analyse ChatBot The Question {question}";
            if (CombinedSteps.OuputProc(proc))
            {
                var startTime = DateTime.Now;
                if (!WhenIAskChatBotTheQuestion(question)) return Failure(proc, $"Failed to send question {question}");
                var questionCompleted = DateTime.Now;
                if (!ThenWaitForAnswerToGenerate()) return Failure(proc, $"Failed to wait for answer!");
                var answerProduced = DateTime.Now;
                if (IsThereAnyErrorMessages()) return Failure(proc, "Application in test has thrown a wobbly!");
                var answer = ElementInteraction.GetTextFromLastElement(Helpers.Page.CurrentPage, "answers", "textbox");
                var readAnswer = DateTime.Now;;
                if (answer == null) return Failure(proc, $"Failed to get an answer back!");
                var timeQuestionTaken = questionCompleted - startTime;
                var timeAnswerProduced = answerProduced - questionCompleted;
                var timeToReadAnswer = readAnswer - answerProduced;
                var numberOfCitations = GetTheNumberOfCitationsInLastAnswer() ?? 0;
                string? citationTitles = null;
                if (numberOfCitations == 0)
                {                    
                    citationTitles = GetTitlesOfAllCitationsInLastAnswer(numberOfCitations);
                }
                // create the model
                var questionAndAnswerModel = QuestionsAndAnswersUsing.CreateModel(question, answer, numberOfCitations, citationTitles, timeQuestionTaken, timeAnswerProduced, timeToReadAnswer);
                if (questionAndAnswerModel == null) return Failure($"Failed to create the Q&A model!");
                var success = await QuestionsAndAnswersUsing.SendQuestionAndAnswerModel(questionAndAnswerModel);
                if (!success) return Failure($"Failed to send the Q&A model for analysis!");  
                return true;
            }
            return false;
        }

        
        [When(@"I Ask ChatBot The Question ""(.*)""")]
        public bool WhenIAskChatBotTheQuestion(string question)
        {
            string proc = $"When I Ask ChatBot The Question {question}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.TextBox.EnterTextAndKey("question", question, "enter")) return Failure($"Failed to enter text and key!");
                DebugOutput.Log($"Sent {question} to question element");
                return true;
            }
            return false;
        }


        [Then(@"Wait For Answer To Generate")]
        public bool ThenWaitForAnswerToGenerate()
        {
            string proc = $"Then Wait For Answer To Generate";
            if (CombinedSteps.OuputProc(proc))
            {
                var answerBeingGeneratedText = "Generating answer...";                
                var waitTime = TargetConfiguration.Configuration.PositiveTimeout;
                if (ElementInteraction.WaitForTextNotToChangeInLastElement(Helpers.Page.CurrentPage, "answer", "Textbox", answerBeingGeneratedText, waitTime)) return true;
                return Failure(proc, $"Failed waiting for last answer to finish generation!");
            }
            return false;
        }
        

        
        [Then(@"There Are (.*) Citation Shown")]
        public bool ThenThereAreCitationShown(int expectedNumberOfCitations)
        {
            string proc = $"Then There Are Citation Shown {expectedNumberOfCitations}";
            if (CombinedSteps.OuputProc(proc))
            {
                var numberOfCitations = GetTheNumberOfCitationsInLastAnswer();
                if (numberOfCitations != expectedNumberOfCitations) return Failure(proc, $"Failed as they do not match - wanted {expectedNumberOfCitations} gotten {numberOfCitations}");
                return true;
            }
            return false;
        }


        [Then(@"Citation Panel Is Displayed")]
        public bool ThenCitationPanelIsDisplayed()
        {
            string proc = $"Then Citation Panel Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                var elementName = "citation panel";
                if (!ElementInteraction.IsElementDisplayed(Helpers.Page.CurrentPage, elementName, "textbox")) return Failure(proc, $"Failed - it is displayed");
                var text = ElementInteraction.GetTextFromElement(Helpers.Page.CurrentPage, elementName, "textbox");
                DebugOutput.Log($"CITATION PANEL READS {text}");          
                return true;      
            }
            return false;
        }

        [Then(@"Citation Panel Is Not Displayed")]
        public bool ThenCitationPanelIsNotDisplayed()
        {
            string proc = $"Then Citation Panel Is Not Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                var elementName = "citation panel";
                if (ElementInteraction.IsElementNotDisplayed(Helpers.Page.CurrentPage, elementName, "textbox")) return true;
                return Failure(proc, "I can still see the Citation panel!");
            }
            return false;
        }

        [Then(@"Last Answer Contains The Text ""(.*)""")]
        public bool ThenLastAnswerContainsTheText(string answerContains)
        {
            string proc = $"Then Last Answer Contains The Text {answerContains}";
            if (CombinedSteps.OuputProc(proc))
            {
                var answer = ElementInteraction.GetTextFromLastElement(Helpers.Page.CurrentPage, "answers", "textbox");
                if (answer == null) return Failure(proc, "Failed to get the last answer, or the text from last answer!");
                if (answer.ToLower().Contains(answerContains.ToLower())) return true;
                return Failure(proc, $"Failed to find {answerContains} in the last answer {answer}");
            }
            return false;
        }


        [Then(@"Wait For Chat History To Be Displayed")]
        public bool ThenWaitForChatHistoryToBeDisplayed()
        {
            string proc = "Then Wait For Chat History To Be Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (ElementInteraction.IsElementDisplayed(Helpers.Page.CurrentPage, "chat history panel", "textbox", 30)) return true;
                DebugOutput.Log($"We have waited - no chat history!");
                return false;
            }
            return false;
        }
        


        ///////   CODE BELOW!



        private int? GetTheNumberOfCitationsInLastAnswer()
        {
            DebugOutput.OutputMethod("GetTheNumberOfCitationsInLastAnswer");
            var numberOfCitations = ElementInteraction.GetTheNumberOfSubElementsOfNthElement(Helpers.Page.CurrentPage, "answers", "textbox", "citation counter", "textbox", 0);
            return numberOfCitations;
        }

        private string?  GetTitlesOfAllCitationsInLastAnswer(int? numberOfCitationsExpected)
        {
            var listOfCitationTitles = ElementInteraction.GetTextListFromSubSubElementsOfNthElement(Helpers.Page.CurrentPage, "answers", "textbox", "answers citation wrapper", "textbox", "answers citation", "textbox", 0, "title");
            if (listOfCitationTitles == null) return null;
            if (numberOfCitationsExpected != null)
            {                
                if (listOfCitationTitles.Count != numberOfCitationsExpected) return null;
            }
            string? allCitationTitles = null;
            foreach (var title in listOfCitationTitles)
            {
                allCitationTitles = allCitationTitles + title + "^";
            }
            if (allCitationTitles == null) return null;
            allCitationTitles = allCitationTitles.Substring(0, allCitationTitles.Length - 1);
            return allCitationTitles;
        }        

        public bool IsThereAnyErrorMessages()
        {
            DebugOutput.OutputMethod($"IsThereAnyErrorMessages");
            var errorMessageElementName = "error answer";
            if (ElementInteraction.WaitForElementToNotBeDisplayed(Helpers.Page.CurrentPage, errorMessageElementName, "textbox", TargetConfiguration.Configuration.NegativeTimeout)) return false ;
            DebugOutput.Log($"IsThereAnyErrorMessages  THERE ARE!");
            return true;
        }       

    }
}
