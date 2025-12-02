
using Newtonsoft.Json;


namespace Generic.Steps.JIRA
{
    public class ProjectUsage
    {
        public static List<Project>? MakeProjectModel(string? json)
        {
            if (json == null) return null;
            List<Project>? items = new();
            items = JsonConvert.DeserializeObject<List<Project>>(json);
            return items;
        }

        public static Project? GetProjectByProjectName(List<Project> listOfProjectModels, string projectName)
        {
            foreach (var project in listOfProjectModels)
            {
                if (project.Name == projectName) return project;
            }
            return null;
        }
        

        public static Project? GetProjectById(List<Project> listOfProjectModels, string id)
        {
            foreach (var project in listOfProjectModels)
            {
                if (project.Id == id) return project;
            }
            return null;
        }

    }


}
