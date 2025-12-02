

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IButtonStepHelper : IStepHelper
    {
        bool ClickButton(string buttonName);
        public bool ClickNthButton(string buttonName, string nTh);
        bool DoubleClick(string buttonName);
        bool DragAToB(string buttonAName, string buttonBName);
        string GetText(string buttonName);
        bool IsEnabled(string buttonName);
        bool IsSelected(string buttonName, int timeout = 0);
        bool IsNotDisplayed(string buttonName, int timeout = 30);
        bool IsDisplayed(string buttonName, int timeout = 1);
        bool MouseOver(string buttonName);
        bool RightClick(string buttonName);

    }
}
