using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Reqnroll.Bindings.Discovery;

namespace Generic.Steps.JIRA
{
    public class IssueUsage
    {
        public static Issue? MakeIssueModel(string? json)
        {
            if (json == null) return null;
            Issue? items = new();
            items = JsonConvert.DeserializeObject<Issue>(json);
            return items;
        }

        public static bool IssueExists(Issue issueModel, string id)
        {
            if (issueModel.Id == id) return true;
            return false;
        }

    }



}