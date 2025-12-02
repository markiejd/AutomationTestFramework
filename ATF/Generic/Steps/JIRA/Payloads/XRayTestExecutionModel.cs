
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Generic.Steps.XRay
{
    public class XRayExecutionUsage
    {
        public static string TODO = "TODO";

        public static List<Test>? MakeExecutionModel(string? json)
        {
            if (json == null) return null; 
            List<Test>? items = new();
            items = JsonConvert.DeserializeObject<List<Test>>(json);
            return items;
        }

        public static int HowManyTestsToDoInExecutionMode(List<Test> executionModel)
        {
            int counter = 0;
            foreach (var test in executionModel)
            {
                if (test.status == TODO) counter++;
            }
            return counter;
        }

        public static int HowManyTestsInExecutionModel(List<Test> executionModel)
        {
            return executionModel.Count;            
        }

        public static List<string> GetListOfIDsToDoFromExecution(List<Test> executionModel)
        {
            List<string> testIDsInExecution = new List<string>();
            foreach (var test in executionModel)
            {
                if (test.id != null)
                {
                    var intToString = test.id.ToString();
                    if (intToString != null) testIDsInExecution.Add(intToString);
                }
            }
            return testIDsInExecution;
        }

        public static List<string> GetListOfKeysToDoFromExecution(List<Test> executionModel)
        {
            List<string> testInExecution = new List<string>();
            foreach (var test in executionModel)
            {
                if (test.key != null) testInExecution.Add(test.key);
            }
            return testInExecution;
        }


    }
}