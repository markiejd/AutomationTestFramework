using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ITimePickerStepHelper : IStepHelper
    {
        bool EnterValueInTimePicker(string timePickerName, string time, int timeOut);
        string? GetCurrentValue(string timePickerName, int timeOut);
        bool IsDisplayed(string timePickerName, int timeout = 0);
        bool SetTimeValue(string timePickerName, string time, int timeOut = 0);

    }
}