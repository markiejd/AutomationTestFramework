using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class WindowStepHelper : StepHelper, IWindowStepHelper
    {
        private readonly ITargetForms targetForms;
        public WindowStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        public bool SizeOfWindow(int width, int height)
        {
            DebugOutput.Log($"SizeOfWindow {width} {height} ");
            return ElementInteraction.SetWindowSize(width, height);
        }

        public bool SizeOfWindowString(string compositeSize)
        {
            DebugOutput.Log($"SizeOfWindow {compositeSize}");
            int width = 800;
            int height = 800;
            if (compositeSize.ToLower() == "default")
            {
                compositeSize =  TargetConfiguration.Configuration.ScreenSize;
            }
            var sizes = StringValues.BreakUpByDelimited(compositeSize,"x");
            if (sizes.Count() != 2)
            {
                DebugOutput.Log($"You need an x and y! Delimited by X You gave us {compositeSize}");
                return false;
            }
            try
            {
                width = Int32.Parse(sizes[0]);
                height = Int32.Parse(sizes[1]);
            }
            catch
            {
                DebugOutput.Log($"Failed to convert {compositeSize} to ints");
                return false;
            }
            return ElementInteraction.SetWindowSize(width, height);      
        }

        public bool CloseWindow(string windowsName)
        {
            DebugOutput.Log($"CloseWindow ");
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, windowsName, "window", "", "close");
        }

        public bool CloseTopElement()
        {
            DebugOutput.Log($"CloseTopElement ");
            ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, "EditableTextBox", "wundow", "", "close");
            return ElementInteraction.ClickOnElement(CurrentPage, "Yes", "Button");
        }

        public bool IsDisplayed(string windowsName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, windowsName, "Windows");
        }

        public bool WriteInWindow(string documentName, string text)
        {
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, documentName, "window", text);
        }

    }
}
