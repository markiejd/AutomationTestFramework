using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Datepicker
{
    [Binding]
    public class GivenDatePickerSteps : StepsBase
    {
        public GivenDatePickerSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"DatePicker ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void GivenDatePickerIsEqualTo(string datePickerName, string date)
        {
            string proc = $"Given DatePicker {datePickerName} Is Equal To {date}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.DatePicker.SetDateValue(datePickerName, date))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
