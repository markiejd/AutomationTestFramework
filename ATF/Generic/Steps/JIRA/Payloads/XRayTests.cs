
using Newtonsoft.Json;

namespace Generic.Steps.XRay
{
    public class XRayTestsUsage
    {
        public static List<Test>? MakeTestsModel(string? json)
        {
            if (json == null) return null; 
            List<Test>? items = new();
            items = JsonConvert.DeserializeObject<List<Test>>(json);
            return items;
        }
    }
}