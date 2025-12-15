using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Timepicker
{
    [Binding]
    public class ThenTimePickerSteps : StepsBase
    {
        public ThenTimePickerSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"TimePicker ""([^""]*)"" Is Equal To ""([^""]*)""")]
        public void ThenTimePickerIsEqualTo(string timePickerName, string time)
        {
            throw new PendingStepException();
        }




    }
}
