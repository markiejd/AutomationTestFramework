using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.Chip
{
    [Binding]
    public class WhenChipSteps : StepsBase
    {
        public WhenChipSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        
        [When(@"I Click On Chip ""(.*)"" In Chip Array ""(.*)""")]
        public void WhenIClickOnChipInChipArray(string chipName,string chipArrayName)
        {
            string proc = $"When I Click On Chip {chipName} In Chip Array {chipArrayName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Chip.ClickChip(chipArrayName, chipName)) return;
            }
            CombinedSteps.Failure(proc);
            return;
        }


        [When(@"I Close Chip ""([^""]*)"" In Chip Array ""([^""]*)""")]
        public void WhenICloseChipInChipArray(string chipName, string chipArrayName)
        {
            string proc = $"When I Close Chip {chipName} In Chip Array {chipArrayName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Chip.CloseChip(chipArrayName, chipName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
