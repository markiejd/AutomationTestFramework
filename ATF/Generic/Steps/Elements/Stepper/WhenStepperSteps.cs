using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Elements.Stepper
{
    [Binding]
    public class WhenStepperSteps : StepsBase
    {
        public WhenStepperSteps(IStepHelpers helpers) : base(helpers)
        {
        }
    }
}
