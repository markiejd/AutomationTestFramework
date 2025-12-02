using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Datepicker
{
    [Binding]
    public class WhenDatePickerSteps : StepsBase
    {
        public WhenDatePickerSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        [When(@"I Select Date ""(.*)"" From DatePicker")]
        public void WhenISelectDateFromDatePicker(string date)
        {
            string proc = $"When I Select Date {date} From DatePicker";
            if (CombinedSteps.OuputProc(proc))
            {
                Thread.Sleep(500); // wait for date picker to be ready
                if (Helpers.DatePicker.SetDateValue("", date)) return;
                CombinedSteps.Failure(proc);

            }            
        }





    }
}
