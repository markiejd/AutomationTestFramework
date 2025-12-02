
namespace Generic.Steps.JIRA
{
    public class Aggregateprogress
    {
        public int? Progress { get; set; }
        public int? Total { get; set; }
    }

    public class Attachment
    {
        public string? Self { get; set; }
        public string? Id { get; set; }
        public string? Filename { get; set; }
        public Author? Author { get; set; }
        public DateTime? Created { get; set; }
        public int? Size { get; set; }
        public string? MimeType { get; set; }
        public string? Content { get; set; }
        public string? Thumbnail { get; set; }
    }

    public class Author
    {
        public string? Self { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? EmailAddress { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? DisplayName { get; set; }
        public bool? Active { get; set; }
        public string? TimeZone { get; set; }
    }

    public class AvatarUrls
    {
        public string? _48x48 { get; set; }
        public string? _24x24 { get; set; }
        public string? _16x16 { get; set; }
        public string? _32x32 { get; set; }
    }

    public class Comment
    {
        public List<Comment>? Comments { get; set; }
        public int? MaxResults { get; set; }
        public int? Total { get; set; }
        public int? StartAt { get; set; }
    }

    public class Comment2
    {
        public string? Self { get; set; }
        public string? Id { get; set; }
        public Author? Author { get; set; }
        public string? Body { get; set; }
        public UpdateAuthor? UpdateAuthor { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class Component
    {
        public string? Self { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Creator
    {
        public string? Self { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? EmailAddress { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? DisplayName { get; set; }
        public bool? Active { get; set; }
        public string? TimeZone { get; set; }
    }

    public class Customfield10400
    {
        public string? Self { get; set; }
        public string? Value { get; set; }
        public string? Id { get; set; }
    }

    public class Customfield11001
    {
        public string? Self { get; set; }
        public string? Value { get; set; }
        public string? Id { get; set; }
    }

    public class Customfield11123
    {
        public string? IssueKey { get; set; }
        public string? Status { get; set; }
        public string? StatusStyle { get; set; }
        public int? Ok { get; set; }
        public double? OkPercent { get; set; }
        public string? OkJql { get; set; }
        public int? Nok { get; set; }
        public double? NokPercent { get; set; }
        public string? NokJql { get; set; }
        public int? Notrun { get; set; }
        public double? NotrunPercent { get; set; }
        public string? NotRunJql { get; set; }
        public int? Unknown { get; set; }
        public double? UnknownPercent { get; set; }
        public string? UnknownJql { get; set; }
    }

    public class Customfield11200
    {
        public string? Self { get; set; }
        public string? Value { get; set; }
        public string? Id { get; set; }
    }

    public class Customfield11301
    {
        public string? Self { get; set; }
        public string? Value { get; set; }
        public string? Id { get; set; }
    }

    public class Customfield11900
    {
        public string? Self { get; set; }
        public string? Value { get; set; }
        public string? Id { get; set; }
    }

    public class Fields
    {
        public List<object?>? FixVersions { get; set; }
        public Customfield11200? Customfield11200 { get; set; }
        public object? Resolution { get; set; }
        public List<string?>? Customfield10104 { get; set; }
        public string? Customfield10105 { get; set; }
        public double? Customfield10106 { get; set; }
        public object? Customfield10902 { get; set; }
        public string? Customfield10903 { get; set; }
        public DateTime? LastViewed { get; set; }
        public object? Customfield12000 { get; set; }
        public double? Customfield12001 { get; set; }
        public Priority? Priority { get; set; }
        public object? Customfield10100 { get; set; }
        public object? Customfield12400 { get; set; }
        public List<object?>? Labels { get; set; }
        public object? Customfield10610 { get; set; }
        public object? Customfield11303 { get; set; }
        public object? Customfield11700 { get; set; }
        public object? Customfield10611 { get; set; }
        public object? Customfield11704 { get; set; }
        public object? Timeestimate { get; set; }
        public object? Aggregatetimeoriginalestimate { get; set; }
        public List<object>? Versions { get; set; }
        public object? Customfield11706 { get; set; }
        public object? Customfield11705 { get; set; }
        public List<object>? Issuelinks { get; set; }
        public object? Assignee { get; set; }
        public Status? Status { get; set; }
        public List<Component>? Components { get; set; }
        public object? Customfield11300 { get; set; }
        public Customfield11301? Customfield11301 { get; set; }
        public object? Customfield11302 { get; set; }
        public object? Customfield11810 { get; set; }
        public object? Customfield12107 { get; set; }
        public string? Customfield10600 { get; set; }
        public object? Customfield12106 { get; set; }
        public object? Customfield10601 { get; set; }
        public object? Customfield12109 { get; set; }
        public object? Customfield10602 { get; set; }
        public object? Customfield12108 { get; set; }
        public object? Customfield12504 { get; set; }
        public object? Customfield10603 { get; set; }
        public object? Customfield12507 { get; set; }
        public object? Aggregatetimeestimate { get; set; }
        public object? Customfield10604 { get; set; }
        public object? Customfield12506 { get; set; }
        public object? Customfield10605 { get; set; }
        public object? Customfield10606 { get; set; }
        public object? Customfield10607 { get; set; }
        public object? Customfield10608 { get; set; }
        public object? Customfield10609 { get; set; }
        public Creator? Creator { get; set; }
        public List<object>? Subtasks { get; set; }
        public Reporter? Reporter { get; set; }
        public object? Customfield12101 { get; set; }
        public object? Customfield12100 { get; set; }
        public Aggregateprogress? Aggregateprogress { get; set; }
        public object? Customfield12103 { get; set; }
        public object? Customfield10200 { get; set; }
        public object? Customfield12102 { get; set; }
        public object? Customfield10201 { get; set; }
        public object? Customfield12105 { get; set; }
        public object? Customfield12501 { get; set; }
        public object? Customfield12500 { get; set; }
        public object? Customfield11800 { get; set; }
        public object? Customfield11803 { get; set; }
        public object? Customfield11805 { get; set; }
        public object? Customfield11804 { get; set; }
        public object? Customfield11807 { get; set; }
        public object? Customfield11806 { get; set; }
        public Progress? Progress { get; set; }
        public object? Customfield11809 { get; set; }
        public Votes? Votes { get; set; }
        public Worklog? Worklog { get; set; }
        public Issuetype? Issuetype { get; set; }
        public object? Timespent { get; set; }
        public Project? Project { get; set; }
        public Customfield11001? Customfield11001 { get; set; }
        public List<Customfield11123>? Customfield11123 { get; set; }
        public object? Aggregatetimespent { get; set; }
        public object? Customfield11400 { get; set; }
        public object? Customfield10700 { get; set; }
        public object? Resolutiondate { get; set; }
        public int? Workratio { get; set; }
        public Watches? Watches { get; set; }
        public DateTime? Created { get; set; }
        public object? Customfield12200 { get; set; }
        public object? Customfield12201 { get; set; }
        public object? Customfield11502 { get; set; }
        public object? Customfield11501 { get; set; }
        public Customfield11900? Customfield11900 { get; set; }
        public DateTime? Updated { get; set; }
        public object? Timeoriginalestimate { get; set; }
        public string? Description { get; set; }
        public Timetracking? Timetracking { get; set; }
        public object? Customfield10402 { get; set; }
        public List<Attachment>? Attachment { get; set; }
        public string? Summary { get; set; }
        public string? Customfield10000 { get; set; }
        public object? Customfield12300 { get; set; }
        public Customfield10400? Customfield10400 { get; set; }
        public object? Customfield11601 { get; set; }
        public object? Customfield11600 { get; set; }
        public object? Environment { get; set; }
        public object? Customfield11603 { get; set; }
        public object? Customfield11602 { get; set; }
        public object? Customfield11605 { get; set; }
        public object? Customfield11604 { get; set; }
        public object? Duedate { get; set; }
        public Comment? Comment { get; set; }
    }

    public class Issuetype
    {
        public string? Self { get; set; }
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? Name { get; set; }
        public bool? Subtask { get; set; }
        public int? AvatarId { get; set; }
    }

    public class Priority
    {
        public string? Self { get; set; }
        public string? IconUrl { get; set; }
        public string? Name { get; set; }
        public string? Id { get; set; }
    }

    public class Progress
    {
        public int? progress { get; set; }
        public int? Total { get; set; }
    }

    public class Project
    {
        public string? Self { get; set; }
        public string? Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? ProjectTypeKey { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public ProjectCategory? ProjectCategory { get; set; }
    }

    public class ProjectCategory
    {
        public string? Self { get; set; }
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
    }

    public class Reporter
    {
        public string? Self { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? EmailAddress { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? DisplayName { get; set; }
        public bool? Active { get; set; }
        public string? TimeZone { get; set; }
    }

    public class Issue
    {
        public string? Expand { get; set; }
        public string? Id { get; set; }
        public string? Self { get; set; }
        public string? Key { get; set; }
        public Fields? Fields { get; set; }
    }

    public class Status
    {
        public string? Self { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? Name { get; set; }
        public string? Id { get; set; }
        public StatusCategory? StatusCategory { get; set; }
    }

    public class StatusCategory
    {
        public string? Self { get; set; }
        public int? Id { get; set; }
        public string? Key { get; set; }
        public string? ColorName { get; set; }
        public string? Name { get; set; }
    }

    public class Timetracking
    {
    }

    public class UpdateAuthor
    {
        public string? Self { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? EmailAddress { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public string? DisplayName { get; set; }
        public bool? Active { get; set; }
        public string? TimeZone { get; set; }
    }

    public class Votes
    {
        public string? Self { get; set; }
        public int? votes { get; set; }
        public bool? HasVoted { get; set; }
    }

    public class Watches
    {
        public string? Self { get; set; }
        public int? WatchCount { get; set; }
        public bool? IsWatching { get; set; }
    }

    public class Worklog
    {
        public int? StartAt { get; set; }
        public int? MaxResults { get; set; }
        public int? Total { get; set; }
        public List<object?>? Worklogs { get; set; }
    }
    public class Projects
    {
        public string? Expand { get; set; }
        public string? Self { get; set; }
        public string? Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
        public ProjectCategory? ProjectCategory { get; set; }
        public string? ProjectTypeKey { get; set; }
    }

}