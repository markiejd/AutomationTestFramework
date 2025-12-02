using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ISliderStepHelper : IStepHelper
    {
        bool EnterSliderValue(string sliderName, string value);
        bool IsDisplayed(string sliderName);
        bool SetSliderValue(string sliderName, string value);
    }
}
