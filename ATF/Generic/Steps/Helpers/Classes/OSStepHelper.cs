using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class OSStepHelper : StepHelper, IOSStepHelper
    {
        private readonly ITargetForms targetForms;
        public OSStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }


    }
}
