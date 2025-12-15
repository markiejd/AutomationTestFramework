using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class StepHelper : IStepHelper
	{
		protected StepHelper(FeatureContext featureContext)
		{
			CurrentFeatureContext = featureContext;
		}

		private FeatureContext CurrentFeatureContext { get; }

		public FormBase CurrentPage
		{
			get => CurrentFeatureContext.Get<FormBase>();
			set => CurrentFeatureContext.Set(value);
		}
	}
}
