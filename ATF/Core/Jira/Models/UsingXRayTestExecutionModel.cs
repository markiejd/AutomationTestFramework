using Newtonsoft.Json;

namespace Core.Jira.Using
{
    public static class UsingXRayTestExecutionModel
    {
        public static string? ConvertJiraModelToString(Core.Jira.XRAY.Execution.Model.Root model)
        {
            string jsonString = JsonConvert.SerializeObject(model);
            return jsonString;
        }

        public static Core.Jira.XRAY.Execution.Model.Root? ConvertJsonToModel(string jsonString)
        {
                Core.Jira.XRAY.Execution.Model.Root? myDeserializedClass = new();
                if (jsonString == null) return null;
                myDeserializedClass = JsonConvert.DeserializeObject<Core.Jira.XRAY.Execution.Model.Root>(jsonString);
                return myDeserializedClass;
        }

        public static string? ConvertParametersInToFullModelJson(string textExecutionKey, string testKey, string status, string comment)
        {
            var x = new Core.Jira.XRAY.Execution.Model.Root();
            var info = new Core.Jira.XRAY.Execution.Model.Info();
            info.testExecutionKey = textExecutionKey;
            var listOfTests = new List<Core.Jira.XRAY.Execution.Model.Test>();
            var tests = new Core.Jira.XRAY.Execution.Model.Test();
            tests.testKey = testKey;
            tests.status = status;
            listOfTests.Add(tests);
            x.tests = listOfTests;
            return ConvertJiraModelToString(x);
        }
    }


}