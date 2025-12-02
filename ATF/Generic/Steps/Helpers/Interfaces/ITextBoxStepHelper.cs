using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ITextBoxStepHelper : IStepHelper
    {
        bool Clear(string textBoxName);
        bool ClearThenEnterText(string textBoxName, string text);
        bool Click(string textBoxName);
        string GetText(string textBoxName);
        bool EnterText(string textBoxName, string text, string key = "");
        bool EnterTextAndKey(string textBoxName, string text, string key);
        string? GetLabelOfTextBox(string textBoxName, By labelLocator);
        int GetWidthOfTextBox(string textBoxName);
        bool IsDisplayed(string textBoxName, int timeOut = 0);
        bool IsReadOnly(string textBoxName);
        string GetPlaceholderText(string textBoxName);
    }
}
