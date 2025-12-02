
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Jira.XRAY
{
    public class Customfield10200
    {
        public string value { get; set; } = string.Empty;
    }

    public class Customfield10201
    {
        public string value { get; set; } = string.Empty;
    }

    public class Fields
    {
        public Project project { get; set; } = new Project();
        public string summary { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public Issuetype issuetype { get; set; } = new Issuetype();
        public Customfield10200 customfield_10200 { get; set; } = new Customfield10200();
        public Customfield10201 customfield_10201 { get; set; } = new Customfield10201();
        public string customfield_10202 { get; set; } = string.Empty;
    }

    public class Issuetype
    {
        public string name { get; set; } = string.Empty;
    }

    public class Project
    {
        public string key { get; set; } = string.Empty;
    }

    public class Root
    {
        public Fields fields { get; set; } = new Fields();
    }

    public static class UpdateXRayTests
    {
        public static void Hello()
        {
            DebugOutput.Log($"hhloo");
        }

        public static string? GetRootJson(Root model)
        {
            string jsonString = JsonConvert.SerializeObject(model);
            return jsonString;
        }
        

        public static Root GetRoot()
        {
            var newRoot = new Root();
            if (TargetConfiguration.Configuration.JiraName == null) return newRoot;
            newRoot.fields.project.key = TargetConfiguration.Configuration.JiraName;
            newRoot.fields.summary = "Ach its a new summary";
            newRoot.fields.description = "New Description!";
            var newIssueType = new Issuetype();
            newIssueType.name = "Test";
            newRoot.fields.issuetype = newIssueType;
            newRoot.fields.customfield_10200.value = "Cucumber";
            newRoot.fields.customfield_10201.value = "Scenario";
            newRoot.fields.customfield_10202 = "Given I have a calculator\nWhen I press 1\nAnd I press +\nAnd I press 2\nAnd I press =\nThen I should see 3";
            return newRoot;
        }


    }
}