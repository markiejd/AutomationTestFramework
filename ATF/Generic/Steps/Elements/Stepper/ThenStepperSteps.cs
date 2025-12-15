using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Elements.Stepper
{
    [Binding]
    public class ThenStepperSteps : StepsBase
    {
        public ThenStepperSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Stepper ""([^""]*)"" Is Displayed")]
        public void ThenStepperIsDisplayed(string stepperName)
        {
            string proc = $"Then Stepper {stepperName} Is Displayed";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Stepper.IsDisplayed(stepperName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Stepper ""([^""]*)"" Contains (.*) Steps")]
        public void ThenStepperContainsSteps(string stepperName, int numberOfSteps)
        {
            string proc = $"Then Stepper {stepperName} Contains {numberOfSteps} Steps";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Stepper.GetNumberOfSteps(stepperName) == numberOfSteps)
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Stepper ""([^""]*)"" Contains Step ""([^""]*)""")]
        public void ThenStepperContainsStep(string stepperName, string stepName)
        {
            string proc = $"Then Stepper {stepperName} Contains Step {stepName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Stepper.IsStepDisplayed(stepperName, stepName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Step ""([^""]*)"" In Stepper ""([^""]*)"" Status Is ""([^""]*)""")]
        public void ThenStepInStepperStatusIs(string stepName, string stepperName, string status)
        {
            string proc = $"Then Step {stepName} In Stepper {stepperName} Stust Is {status}";
            if (CombinedSteps.OuputProc(proc))
            {
                var stepValue = Helpers.Stepper.GetStatusOfStep(stepperName, stepName);
                if (stepValue != null)
                {
                    if (stepValue.ToLower() == status.ToLower()) return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


    }
}
