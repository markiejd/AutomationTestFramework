using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppXAPI.Models
{

    public class TodDoFromAtlas
    {
        public int userId { get; set; }
        public int id { get; set; } 
        public string? title { get; set; }
        public bool completed { get; set; }
    }

    public class ToDoFromAtlasAPP
    {
        public static List<TodDoFromAtlas>? MakeModel(string json)
        {
            if (json == null) return null;
            List<TodDoFromAtlas>? items = new();
            items = JsonConvert.DeserializeObject<List<TodDoFromAtlas>>(json);
            return items;
        }
    }
}
