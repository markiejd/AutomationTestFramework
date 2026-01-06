using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Timepicker
{
    [Binding]
    public class GivenTimePickerSteps : StepsBase
    {
        public GivenTimePickerSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"TimePicker ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void GivenTimePickerIsEqualTo(string timePickerName, string time)
        {
            string proc = $"Given TimePicker {timePickerName} Is Equal To {time}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.TimePicker.SetTimeValue(timePickerName, time))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
