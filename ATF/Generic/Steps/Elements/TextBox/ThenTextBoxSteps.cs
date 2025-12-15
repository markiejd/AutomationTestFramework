using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Textbox
{
    [Binding]
    public class ThenTextBoxSteps : StepsBase
    {
        public ThenTextBoxSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"TextBox ""([^""]*)"" Is Displayed")]
        [Given(@"Textbox ""([^""]*)"" Is Displayed")]
        public void GivenTextBoxIsDisplayed(string textBoxName)
        {
            ThenTextBoxIsDisplayed(textBoxName);
        }

        [Then(@"TextBox ""([^""]*)"" Is Displayed")]
        [Then(@"Textbox ""([^""]*)"" Is Displayed")]
        public void ThenTextBoxIsDisplayed(string textBoxName)
        {
            string proc = $"Then TextBox {textBoxName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.IsDisplayed(textBoxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"TextBox ""([^""]*)"" Is Not Displayed")]
        public void ThenTextBoxIsNotDisplayed(string textBoxName)
        {
            string proc = $"Then TextBox {textBoxName} Is Not Displayed HELLO";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.TextBox.IsDisplayed(textBoxName, 1))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"TextBox ""([^""]*)"" Is Read Only")]
        [Then(@"Textbox ""([^""]*)"" Is Read Only")]
        public void ThenTextBoxIsReadOnly(string textBoxName)
        {
            string proc = $"Then TextBox {textBoxName} Is Read Only";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.IsReadOnly(textBoxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Textbox ""(.*)"" Is Not Read Only")]
        public void ThenTextboxIsNotReadOnly(string textBoxName)
        {
            string proc = $"Then TextBox {textBoxName} Is Not Read Only";
            if (CombinedSteps.OuputProc(proc))
            {
                if (!Helpers.TextBox.IsReadOnly(textBoxName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Label ""([^""]*)"" Is Equal To ""([^""]*)""")]
        [Then(@"TextBox ""([^""]*)"" Is Equal To ""([^""]*)""")]
        [Then(@"Textbox ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void ThenTextBoxIsEqualTo(string textBoxName, string text)
        {
            string proc = $"Then TextBox {textBoxName} Is Equal To {text}";
            text = StringValues.TextReplacementService(text);
            if (CombinedSteps.OuputProc(proc))
            {
                text = StringValues.TextReplacementService(text);
                if (Helpers.TextBox.GetText(textBoxName) == text)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Label ""([^""]*)"" Contains ""([^""]*)""")]
        [Then(@"TextBox ""([^""]*)"" Contains ""([^""]*)""")]
        [Then(@"Textbox ""([^""]*)"" Contains ""([^""]*)""")]
        public void ThenTextBoxContains(string textBoxName, string text)
        {
            string proc = $"Then TextBox {textBoxName} Contains {text}";
            text = StringValues.TextReplacementService(text);
            proc = $"Then TextBox {textBoxName} Contains {text}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.GetText(textBoxName).Contains(text))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"TextBox ""([^""]*)"" Is Not Equal To ""([^""]*)""")]
        [Then(@"Textbox ""([^""]*)"" Is Not Equal To ""([^""]*)""")]
        public void ThenTextBoxIsNotEqualTo(string textBoxName, string text)
        {
            string proc = $"Then TextBox {textBoxName} Is Not Equal To {text}";
            text = StringValues.TextReplacementService(text);
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.GetText(textBoxName) != text)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"TextBox ""(.*)"" Is Wider Then TextBox ""(.*)""")]
        [Then(@"Textbox ""(.*)"" Is Wider Then Textbox ""(.*)""")]
        public void ThenTextboxIsWiderThenTextBox(string textBox1, string textBox2)
        {
            string proc = $"Then TextBox {textBox1} Is Wider Then TextBox {textBox2}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.GetWidthOfTextBox(textBox1) > Helpers.TextBox.GetWidthOfTextBox(textBox2))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Placeholder In TextBox ""(.*)"" Is ""(.*)""")]
        public void ThenPlaceholderInTextBoxIs(string textBoxName, string placeHolderText)
        {
            string proc = $"Then Placeholder In TextBox {textBoxName} Is {placeHolderText}";
            placeHolderText = StringValues.TextReplacementService(placeHolderText);
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.TextBox.GetPlaceholderText(textBoxName) == placeHolderText)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
            
        }




    }
}
