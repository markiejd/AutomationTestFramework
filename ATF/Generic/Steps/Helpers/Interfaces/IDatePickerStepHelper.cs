using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IDatePickerStepHelper : IStepHelper
    {
        string GetCurrentValue(string datePickerName, int timeOut);
        bool IsDisplayed(string datePickerName, int timeout = 0);
        bool SetDateValue(string datePickerName, string date, string dateFormat = "", int timeOut = 0);

    }
}
