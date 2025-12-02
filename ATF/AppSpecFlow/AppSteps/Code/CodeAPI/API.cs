using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;

namespace AppSpecFlow.AppSteps
{
    public class API : StepsBase
    {
        public API(IStepHelpers helpers,
            GivenSteps givenSteps,
            WhenSteps whenSteps,
            ThenSteps thenSteps
            ) : base(helpers)
        {
            GivenSteps = givenSteps;
            WhenSteps = whenSteps;
            ThenSteps = thenSteps;
        }

        private GivenSteps GivenSteps { get; }
        private WhenSteps WhenSteps { get; }    
        private ThenSteps ThenSteps { get; }



        public string Hello()
        {
            DebugOutput.OutputMethod($"Hello");
            return "HELLO"; //Do This

        }

        /////  AtlasFrontSearchResults - The section where the operation search is



    }
}
