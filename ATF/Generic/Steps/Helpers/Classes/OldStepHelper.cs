using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class OldStepHelper : StepHelper, IOldStepHelper
    {
        private readonly ITargetForms targetForms;
        public OldStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }


    }
}
