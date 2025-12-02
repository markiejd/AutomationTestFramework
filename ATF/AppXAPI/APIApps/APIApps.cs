using AppXAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace AppXAPI.APIApps
{




    public class APIApps
    {
        public static HttpResponseMessage? response; 

        public static string Hello()
        {
            return "hello";
        }

        public static string GetURLFiltered(string endPointURL, List<string> attributes, List<string> attributeValue)
        {
            var url = endPointURL;
            int counter = 0;
            foreach (var attribute in attributes)
            {
                if (counter == 0) url = url + @"?";
                if (counter != 0) url = url + @"&";
                var value = attributeValue[counter];
                url = @url + @attribute + @"=" + @value;
                counter++;
            }
            url = System.Text.RegularExpressions.Regex.Replace(url, " ", "%20");
            return url;
        }





    }
}
