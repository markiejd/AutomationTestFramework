using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IStepperStepHelper : IStepHelper
    {
        int GetNumberOfSteps(string stepperName);
        string? GetStatusOfStep(string stepperName, string stepName);
        bool IsDisplayed(string stepperName);
        bool IsStepDisplayed(string stepperName, string stepName);
    }
}
