using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Datepicker
{
    [Binding]
    public class ThenDatePickerSteps : StepsBase
    {
        public ThenDatePickerSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"DatePicker ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void ThenDatePickerIsEqualTo(string datePickerName, string date)
        {
            string proc = $"Then DatePicker {datePickerName} Is Equal To {date}";
            if (CombinedSteps.OutputProc(proc))
            {
                date = StringValues.TextReplacementService(date);
                if (Helpers.DatePicker.GetCurrentValue(datePickerName, 1) == date)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"DatePicker Is Displayed")]
        public void ThenDatePickerIsDisplayed()
        {
            string proc = $"Then DatePicker Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                //  all our date pickers are unique to a page - i.e. only 1  #mat-datepicker-1
                if (Helpers.DatePicker.IsDisplayed(""))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
