using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Textbox
{
    [Binding]
    public class GivenTextBoxSteps : StepsBase
    {
        public GivenTextBoxSteps(IStepHelpers helpers) : base(helpers)
        {
        }

    }
}
