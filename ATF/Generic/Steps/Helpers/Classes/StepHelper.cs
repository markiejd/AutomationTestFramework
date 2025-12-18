using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Base helper class for step definitions in automated tests.
    /// Provides common functionality for managing the current page/form context
    /// and accessing feature-level data through Reqnroll's FeatureContext.
    /// </summary>
    public class StepHelper : IStepHelper
	{
		/// <summary>
		/// Initializes a new instance of the StepHelper class.
		/// </summary>
		/// <param name="featureContext">The Reqnroll feature context containing shared test data</param>
		protected StepHelper(FeatureContext featureContext)
		{
			// Store the feature context for accessing shared test data across step definitions
			CurrentFeatureContext = featureContext;
		}

		/// <summary>
		/// Gets the current feature context for the test scenario.
		/// This context stores shared data that persists throughout the feature execution.
		/// </summary>
		private FeatureContext CurrentFeatureContext { get; }

		/// <summary>
		/// Gets or sets the current page/form being tested.
		/// This property manages the active form instance in the feature context,
		/// allowing step definitions to interact with the current UI element.
		/// </summary>
		public FormBase CurrentPage
		{
			// Retrieve the current form from the feature context
			get => CurrentFeatureContext.Get<FormBase>();
			
			// Store the form in the feature context for use across step definitions
			set => CurrentFeatureContext.Set(value);
		}
	}
}
