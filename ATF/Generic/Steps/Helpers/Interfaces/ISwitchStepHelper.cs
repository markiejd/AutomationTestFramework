
namespace Generic.Steps.Helpers.Interfaces
{
    public interface ISwitchStepHelper : IStepHelper
    {
        bool Click(string switchName);
        bool IsDisplayed(string switchName);
        bool Status(string switchName);
        
    }
}
