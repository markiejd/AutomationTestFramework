using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
     
namespace AppSpecFlow.AppSteps
{
   [Binding]
   public class ATFAPITesterSteps : StepsBase
   {
       public ATFAPITesterSteps(IStepHelpers helpers,
           GivenSteps givenSteps,
           WhenSteps whenSteps,
           ThenSteps thenSteps) : base(helpers)
       {
           GivenSteps = givenSteps;
           WhenSteps = whenSteps;
           ThenSteps = thenSteps;
       }
       private GivenSteps GivenSteps { get; }
       private WhenSteps WhenSteps { get; }
       private ThenSteps ThenSteps { get; }
   }
}
