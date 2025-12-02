
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Jira.Using
{
    public static class UsingJIraModel
    {
        public static string? ConvertJiraModelToString(Core.Jira.Model.Root model)
        {
            string jsonString = JsonConvert.SerializeObject(model);
            return jsonString;
        }

        public static Core.Jira.Model.Root? ConvertJsonToModel(string jsonString)
        {
            Core.Jira.Model.Root? myDeserializedClass = new();
            try
            {
                if (jsonString == null) return null;
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                myDeserializedClass = JsonConvert.DeserializeObject<Core.Jira.Model.Root>(jsonString, settings);   
                return myDeserializedClass; 
            }
            catch (Exception ex)
            {   
                DebugOutput.Log($"I CAN READA THIS? {ex}");
                return null;
            }
        }
    }


}