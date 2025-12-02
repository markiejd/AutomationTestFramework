using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Radiobutton
{
    [Binding]
    public class GivenRadioButtonSteps : StepsBase
    {
        public GivenRadioButtonSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        [Given(@"RadioButton ""([^""]*)"" Is Selected")]
        public void GivenRadioButtonIsSelected(string radioButtonName)
        {
            string proc = $"Given Radiobutton {radioButtonName} Is Selected";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.RadioButton.Select(radioButtonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }



    }
}
