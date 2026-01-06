using Core;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Elements.Steps.OS
{
    [Binding]
    public class GivenOSPageSteps : StepsBase
    {
        public GivenOSPageSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Given(@"Directory ""([^""]*)"" Does Not Exist")]
        public void GivenDirectoryDoesNotExist(string fullDirectoryName)
        {
            fullDirectoryName = StringValues.TextReplacementService(fullDirectoryName);
            string proc = $"Given Directory {fullDirectoryName} Does Not Exist";
            if (CombinedSteps.OutputProc(proc))
            {
                if (FileUtils.OSDeleteDirectoryIfExists(fullDirectoryName))
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

    }
}
