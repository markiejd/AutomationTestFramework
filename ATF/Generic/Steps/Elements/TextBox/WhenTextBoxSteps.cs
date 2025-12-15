using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Textbox
{
    [Binding]
    public class WhenTextBoxSteps : StepsBase
    {
        public WhenTextBoxSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        [When(@"I Enter ""(.*)"" In Textbox ""(.*)""")]
        [When(@"I Enter ""([^""]*)"" In TextBox ""([^""]*)""")]
        public void WhenIEnterInTextBox(string text, string textBoxName)
        {
            string proc = $"When I Enter {text} In TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.EnterText(textBoxName, text))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        
        [When(@"Press Key ""(.*)"" In TextBox ""(.*)""")]
        [When(@"Press Send Key ""(.*)"" In TextBox ""(.*)""")]
        public void WhenPressKeyInTextBox(string keyStroke,string textBoxName)
        {
            string proc = $"When I Press Key {keyStroke} In TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.EnterTextAndKey(textBoxName, "", keyStroke)) return;
                CombinedSteps.Failure(proc);
                return;
            }
        }        

                
        [When(@"I Clear The TextBox ""(.*)""")]
        public void WhenIClearTextBox(string textBoxName)
        {
            string proc = $"When I Clear TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.Clear(textBoxName))
                {
                    DebugOutput.Log($"That did clear it!");
                    return;
                }
                DebugOutput.Log($"Failed to clear");
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [When(@"I Clear Then Enter ""([^""]*)"" In TextBox ""([^""]*)""")]
        public void WhenIClearThenEnterInTextBox(string text, string textBoxName)
        {
            string proc = $"When I Clear Then Enter {text} In TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.ClearThenEnterText(textBoxName, text))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [When(@"I Enter ""([^""]*)"" Then Press ""([^""]*)"" In TextBox ""([^""]*)""")]
        public void WhenIEnterThenPressInTextBox(string text, string key, string textBoxName)
        {
            string proc = $"When I Enter {text} Then Press {key} In TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.EnterTextAndKey(textBoxName, text, key))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [When(@"I Click On TextBox ""([^""]*)""")]
        public void WhenIClickOnTextBox(string textBoxName)
        {
            string proc = $"When I Click On TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.Click(textBoxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [When(@"I Output TextBox ""([^""]*)""")]
        public void WhenIOutputTextBox(string textBoxName)
        {
            string proc = $"When I Output TextBox {textBoxName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.IsDisplayed(textBoxName))
                {
                    var text = Helpers.TextBox.GetText(textBoxName);
                    DebugOutput.Log($"Output {textBoxName} ={text}");
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
