using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Radiobutton
{
    [Binding]
    public class ThenRadioButtonSteps : StepsBase
    {
        public ThenRadioButtonSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"RadioButton ""([^""]*)"" Is Displayed")]
        public void ThenRadioButtonIsDisplayed(string radioButtonName)
        {
            string proc = $"Then Radiobutton {radioButtonName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.RadioButton.IsDisplayed(radioButtonName))   
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"RadioButton ""([^""]*)"" Is Read Only")]
        public void ThenRadioButtonIsReadOnly(string radioButtonName)
        {
            string proc = $"Then Radiobutton {radioButtonName} Is Read Only";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.RadioButton.IsEnabled(radioButtonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"RadioButton ""([^""]*)"" Is Enabled")]
        public void ThenRadioButtonIsEnabled(string radioButtonName)
        {
            string proc = $"Then Radiobutton {radioButtonName} Is Read Only";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.RadioButton.IsEnabled(radioButtonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"RadioButton ""([^""]*)"" Is Selected")]
        public void ThenRadioButtonIsSelected(string radioButtonName)
        {
            string proc = $"Then Radiobutton {radioButtonName} Is Selected";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.RadioButton.IsSelected(radioButtonName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"RadioButton ""([^""]*)"" Is Not Selected")]
        public void ThenRadioButtonIsNotSelected(string radioButtonName)
        {
            string proc = $"Then Radiobutton {radioButtonName} Is Not Selected";
            if (CombinedSteps.OutputProc(proc))
            {
                if (!Helpers.RadioButton.IsSelected(radioButtonName))
                {
                    DebugOutput.Log($"NO SELECTED");
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }






    }
}
