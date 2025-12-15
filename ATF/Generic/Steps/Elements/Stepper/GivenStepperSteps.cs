using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Elements.Stepper
{
    [Binding]
    public class GivenStepperSteps : StepsBase
    {
        public GivenStepperSteps(IStepHelpers helpers) : base(helpers)
        {
        }
    }
}
