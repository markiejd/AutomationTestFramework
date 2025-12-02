
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Jira.Using
{
    public static class UsingXRayModel
    {
        public static string? ConvertJiraModelToString(Core.Jira.XRAY.Model.Root model)
        {
            string jsonString = JsonConvert.SerializeObject(model);
            return jsonString;
        }

        public static Core.Jira.XRAY.Model.Root? ConvertJsonToModel(string jsonString)
        {
            Core.Jira.XRAY.Model.Root? myDeserializedClass = new();
            try
            {                
                if (jsonString == null) return null;
                myDeserializedClass = JsonConvert.DeserializeObject<Core.Jira.XRAY.Model.Root>(jsonString);
                return myDeserializedClass;
            }
            catch (Exception e)
            {
                DebugOutput.JiraOutput($"WE THROWING EXCEPTION {e}");
                return null;
            }
        }
    }


}