using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.Chip
{
    [Binding]
    public class ThenChipSteps : StepsBase
    {
        public ThenChipSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Chip Array ""([^""]*)"" Is Displayed")]
        public void ThenChipArrayIsDisplayed(string chipArrayName)
        {
            string proc = $"Then Chip Arrary {chipArrayName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Chip.IsDisplayed(chipArrayName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Chip Array ""([^""]*)"" Contains Chip ""([^""]*)""")]
        public void ThenChipArrayContainsChip(string chipArrayName, string chipName)
        {
            string proc = $"Then Chip Arrary {chipArrayName} Contains {chipName}";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Chip.ArraryContainsChip(chipArrayName, chipName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
