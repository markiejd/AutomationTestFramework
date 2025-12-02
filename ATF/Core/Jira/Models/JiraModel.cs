
using Newtonsoft.Json;

namespace Core.Jira.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Aggregateprogress
    {
        public int? progress { get; set; }
        public int? total { get; set; }
    }

    public class Assignee
    {
        public string? self { get; set; }
        public string? accountId { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? displayName { get; set; }
        public bool? active { get; set; }
        public string? timeZone { get; set; }
        public string? accountType { get; set; }
    }

    public class Author
    {
        public string? self { get; set; }
        public string? accountId { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? displayName { get; set; }
        public bool? active { get; set; }
        public string? timeZone { get; set; }
        public string? accountType { get; set; }
    }

    public class AvatarUrls
    {
        [JsonProperty("48x48")]
        public string? _48x48 { get; set; }

        [JsonProperty("24x24")]
        public string? _24x24 { get; set; }

        [JsonProperty("16x16")]
        public string? _16x16 { get; set; }

        [JsonProperty("32x32")]
        public string? _32x32 { get; set; }
    }

    public class Comment
    {
        public List<Comment>? comments { get; set; }
        public string? self { get; set; }
        public int? maxResults { get; set; }
        public int? total { get; set; }
        public int? startAt { get; set; }
    }

    public class Comment2
    {
        public string? self { get; set; }
        public string? id { get; set; }
        public Author? author { get; set; }
        public string? body { get; set; }
        public UpdateAuthor? updateAuthor { get; set; }
        public DateTime? created { get; set; }
        public DateTime? updated { get; set; }
        public bool? jsdPublic { get; set; }
    }

    public class Creator
    {
        public string? self { get; set; }
        public string? accountId { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? displayName { get; set; }
        public bool? active { get; set; }
        public string? timeZone { get; set; }
        public string? accountType { get; set; }
    }

    public class Customfield10007
    {
        public bool? hasEpicLinkFieldDependency { get; set; }
        public bool? showField { get; set; }
        public NonEditableReason? nonEditableReason { get; set; }
    }

    public class Customfield10241
    {
        public string? self { get; set; }
        public string? value { get; set; }
        public string? id { get; set; }
    }

    public class Customfield10247
    {
        public string? self { get; set; }
        public string? value { get; set; }
        public string? id { get; set; }
    }

    public class Fields
    {
        public DateTime? statuscategorychangedate { get; set; }
        public Parent? parent { get; set; }
        public string? customfield_10230 { get; set; }
        public object?  customfield_10231 { get; set; }
        public List<object? >?  fixVersions { get; set; }
        public string? customfield_10232 { get; set; }
        public object?  resolution { get; set; }
        public object?  customfield_10233 { get; set; }
        public object?  customfield_10113 { get; set; }
        public object?  customfield_10234 { get; set; }
        public string? customfield_10114 { get; set; }
        public object?  customfield_10235 { get; set; }
        public object?  customfield_10225 { get; set; }
        public object?  customfield_10226 { get; set; }
        public object?  customfield_10227 { get; set; }
        public object?  customfield_10228 { get; set; }
        public object?  customfield_10229 { get; set; }
        public DateTime? lastViewed { get; set; }
        public object?  customfield_10220 { get; set; }
        public Priority? priority { get; set; }
        public object?  customfield_10221 { get; set; }
        public DateTime? customfield_10100 { get; set; }
        public object?  customfield_10222 { get; set; }
        public object?  customfield_10101 { get; set; }
        public object?  customfield_10223 { get; set; }
        public object?  customfield_10102 { get; set; }
        public List<string?>? labels { get; set; }
        public object?  customfield_10224 { get; set; }
        public string? customfield_10216 { get; set; }
        public object?  timeestimate { get; set; }
        public string? customfield_10218 { get; set; }
        public object?  aggregatetimeoriginalestimate { get; set; }
        public List<object? >?  versions { get; set; }
        public double? customfield_10219 { get; set; }
        public List<Issuelink>? issuelinks { get; set; }
        public Assignee? assignee { get; set; }
        public Status? status { get; set; }
        public List<object? >?  components { get; set; }
        public object?  customfield_10290 { get; set; }
        public object?  customfield_10291 { get; set; }
        public object?  customfield_10292 { get; set; }
        public object?  customfield_10295 { get; set; }
        public object?  customfield_10296 { get; set; }
        public object?  customfield_10210 { get; set; }
        public object?  customfield_10299 { get; set; }
        public List<object? >?  customfield_10203 { get; set; }
        public object?  customfield_10204 { get; set; }
        public object?  customfield_10205 { get; set; }
        public object?  customfield_10207 { get; set; }
        public object?  aggregatetimeestimate { get; set; }
        public Creator? creator { get; set; }
        public List<object? >?  subtasks { get; set; }
        public double? customfield_10280 { get; set; }
        public object?  customfield_10281 { get; set; }
        public object?  customfield_10282 { get; set; }
        public object?  customfield_10283 { get; set; }
        public object?  customfield_10284 { get; set; }
        public Reporter? reporter { get; set; }
        public object?  customfield_10285 { get; set; }
        public object?  customfield_10286 { get; set; }
        public Aggregateprogress? aggregateprogress { get; set; }
        public object?  customfield_10287 { get; set; }
        public object?  customfield_10200 { get; set; }
        public object?  customfield_10288 { get; set; }
        public object?  customfield_10201 { get; set; }
        public object?  customfield_10289 { get; set; }
        public object?  customfield_10202 { get; set; }
        public Progress? progress { get; set; }
        public Votes? votes { get; set; }
        public Worklog? worklog { get; set; }
        public Issuetype? issuetype { get; set; }
        public object?  timespent { get; set; }
        public object?  customfield_10270 { get; set; }
        public object?  customfield_10271 { get; set; }
        public object?  customfield_10272 { get; set; }
        public Project? project { get; set; }
        public object?  customfield_10274 { get; set; }
        public double? customfield_10276 { get; set; }
        public object?  aggregatetimespent { get; set; }
        public double? customfield_10277 { get; set; }
        public double? customfield_10278 { get; set; }
        public double? customfield_10279 { get; set; }
        public object?  resolutiondate { get; set; }
        public int? workratio { get; set; }
        public Issuerestriction? issuerestriction { get; set; }
        public Watches? watches { get; set; }
        public DateTime? created { get; set; }
        public object?  customfield_10260 { get; set; }
        public object?  customfield_10265 { get; set; }
        public object?  customfield_10266 { get; set; }
        public object?  customfield_10267 { get; set; }
        public object?  customfield_10300 { get; set; }
        public object?  customfield_10268 { get; set; }
        public object?  customfield_10301 { get; set; }
        public object?  customfield_10258 { get; set; }
        public object?  customfield_10259 { get; set; }
        public DateTime? updated { get; set; }
        public object?  timeoriginalestimate { get; set; }
        public string? customfield_10250 { get; set; }
        public string? description { get; set; }
        public object?  customfield_10252 { get; set; }
        public object?  customfield_10253 { get; set; }
        public object?  customfield_10254 { get; set; }
        public object?  customfield_10255 { get; set; }
        public object?  customfield_10256 { get; set; }
        public Timetracking? timetracking { get; set; }
        public object?  customfield_10257 { get; set; }
        public Customfield10247? customfield_10247 { get; set; }
        public string? customfield_10006 { get; set; }
        public object?  customfield_10248 { get; set; }
        public object?  security { get; set; }
        public object?  customfield_10249 { get; set; }
        public Customfield10007? customfield_10007 { get; set; }
        public List<object? >?  attachment { get; set; }
        public string? summary { get; set; }
        public Customfield10241? customfield_10241 { get; set; }
        public string? customfield_10000 { get; set; }
        public object?  customfield_10001 { get; set; }
        public List<object? >?  customfield_10002 { get; set; }
        public object?  customfield_10244 { get; set; }
        public object?  customfield_10246 { get; set; }
        public object?  customfield_10236 { get; set; }
        public object?  environment { get; set; }
        public object?  customfield_10117 { get; set; }
        public object?  duedate { get; set; }
        public Comment? comment { get; set; }
        public string? customfield_10214 { get; set; }
    }

    public class Issuelink
    {
        public string? id { get; set; }
        public string? self { get; set; }
        public Type? type { get; set; }
        public OutwardIssue? outwardIssue { get; set; }
    }

    public class Issuerestriction
    {
        public Issuerestrictions? issuerestrictions { get; set; }
        public bool? shouldDisplay { get; set; }
    }

    public class Issuerestrictions
    {
    }

    public class Issuetype
    {
        public string? self { get; set; }
        public string? id { get; set; }
        public string? description { get; set; }
        public string? iconUrl { get; set; }
        public string? name { get; set; }
        public bool? subtask { get; set; }
        public int? hierarchyLevel { get; set; }
        public int? avatarId { get; set; }
    }

    public class NonEditableReason
    {
        public string? reason { get; set; }
        public string? message { get; set; }
    }

    public class OutwardIssue
    {
        public string? id { get; set; }
        public string? key { get; set; }
        public string? self { get; set; }
        public Fields? fields { get; set; }
    }

    public class Parent
    {
        public string? id { get; set; }
        public string? key { get; set; }
        public string? self { get; set; }
        public Fields? fields { get; set; }
    }

    public class Priority
    {
        public string? self { get; set; }
        public string? iconUrl { get; set; }
        public string? name { get; set; }
        public string? id { get; set; }
    }

    public class Progress
    {
        public int? progress { get; set; }
        public int? total { get; set; }
    }

    public class Project
    {
        public string? self { get; set; }
        public string? id { get; set; }
        public string? key { get; set; }
        public string? name { get; set; }
        public string? projectTypeKey { get; set; }
        public bool? simplified { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
    }

    public class Reporter
    {
        public string? self { get; set; }
        public string? accountId { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? displayName { get; set; }
        public bool? active { get; set; }
        public string? timeZone { get; set; }
        public string? accountType { get; set; }
    }

    public class Root
    {
        public string? expand { get; set; }
        public string? id { get; set; }
        public string? self { get; set; }
        public string? key { get; set; }
        public Fields? fields { get; set; }
    }

    public class Status
    {
        public string? self { get; set; }
        public string? description { get; set; }
        public string? iconUrl { get; set; }
        public string? name { get; set; }
        public string? id { get; set; }
        public StatusCategory? statusCategory { get; set; }
    }

    public class StatusCategory
    {
        public string? self { get; set; }
        public int? id { get; set; }
        public string? key { get; set; }
        public string? colorName { get; set; }
        public string? name { get; set; }
    }

    public class Timetracking
    {
    }

    public class Type
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? inward { get; set; }
        public string? outward { get; set; }
        public string? self { get; set; }
    }

    public class UpdateAuthor
    {
        public string? self { get; set; }
        public string? accountId { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? displayName { get; set; }
        public bool? active { get; set; }
        public string? timeZone { get; set; }
        public string? accountType { get; set; }
    }

    public class Votes
    {
        public string? self { get; set; }
        public int? votes { get; set; }
        public bool? hasVoted { get; set; }
    }

    public class Watches
    {
        public string? self { get; set; }
        public int? watchCount { get; set; }
        public bool? isWatching { get; set; }
    }

    public class Worklog
    {
        public int? startAt { get; set; }
        public int? maxResults { get; set; }
        public int? total { get; set; }
        public List<object? >?  worklogs { get; set; }
    }




    
}