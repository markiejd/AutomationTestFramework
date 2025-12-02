

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ICheckboxStepHelper : IStepHelper
    {
        bool IsSelected(string checkboxName, int timeout = 0);
        bool IsDisplayed(string checkboxName, int timeout = 0);
        bool Select(string checkboxName, int timeout = 0);
        bool Selected(string checkboxName, int timeout = 0);
        bool SelectedNot(string checkboxName, int timeout = 0);
    }
}
