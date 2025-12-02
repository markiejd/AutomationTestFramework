using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IDropdownStepHelper : IStepHelper
    {
        bool Click(string dropdownName);
        bool ContainsValue(string dropdownName, string value, int timeout = 0);
        bool EnterThenSelectFrom(string selecting, string dropDownName, int timeOut = 0);
        List<string> GetAllValues(string dropdownName, int timeout = 0);
        string? GetCurrentValue(string dropdownName, int timeout = 0);
        bool IsDisplayed(string dropdownName, int timeout = 0);
        bool SelectingFrom(string selecting, string dropdownName, int timeout = 0, bool topOptionAlreadySelected = false, bool textEntry = true);
        bool SelectingFromWithoutText(string selecting, string dropdownName, int timeout = 0, bool topOptionAlreadySelected = false);

    }
}
