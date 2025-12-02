

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IChipStepHelper : IStepHelper
    {
        bool ArraryContainsChip(string chipArraryName, string chipName);
        bool CloseChip(string chipArrayName, string chipName);
        bool ClickChip(string chipArrayName, string chipName);
        bool IsDisplayed(string chipArraryName);
    }
}
