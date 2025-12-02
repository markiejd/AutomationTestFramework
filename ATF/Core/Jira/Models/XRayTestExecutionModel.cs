using Newtonsoft.Json;

namespace Core.Jira.XRAY.Execution.Model
{    
    public class Evidence
    {
        public string? data { get; set; }
        public string? filename { get; set; }
        public string? contentType { get; set; }
    }

    public class Info
{
        public string? summary { get; set; }
        public string? description { get; set; }
        public string? version { get; set; }
        public string? user { get; set; }
        public string? revision { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? finishDate { get; set; }        
        public string? testExecutionKey { get; set; }
        public string? testPlanKey { get; set; }
        public List<string>? testEnvironments { get; set; }
    }

    public class Root
    {
        public Info? info { get; set; }
        public List<Test>? tests { get; set; }
    }

    public class Test
    {
        public string? testKey { get; set; }
        public DateTime? start { get; set; }
        public DateTime? finish { get; set; }
        public string? comment { get; set; }
        public string? status { get; set; }
        public List<Evidence>? evidences { get; set; }  
        public string? testVersion { get; set; }
    }

}