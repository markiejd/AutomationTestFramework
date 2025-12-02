using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppXAPI.Models
{
    public class Delay
    {
        public static int Page { get; set; }
        public static int Per_page { get; set; }
        public static int Total { get; set; }
        public static int Total_pages { get; set; }
        public static Data Data { get; set; } = new Data();
        public static Support Support { get; set; } = new Support();

    }

    public class Data
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }

    public class Support
    {
        public string URL { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

    }

    public class DelayAPP
    {
        public static List<Delay>? MakeModel(string json)
        {
            if (json == null) return null;
            List<Delay>? items = new();
            json = "[" + json + "]";
            items = JsonConvert.DeserializeObject<List<Delay>>(json);
            return items;
        }
    }
}
