using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppXAPI.Models
{
    public class CatFacts
    {
        public static string? fact { get; set; }
        public static int? length { get; set; }
    }
    public class CatFactsAPP
    {
        public static List<CatFacts>? MakeModel(string json)
        {
            if (json == null) return null;
            List<CatFacts>? items = new List<CatFacts>();
            json = "["+json+"]";
            items = JsonConvert.DeserializeObject<List<CatFacts>>(json);
            return items;
        }
    }
}
