using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transformations.Elements
{
    public static class ElementClasses
    {
        /// <summary>
        /// What tags are we going to store.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetBarredTags()
        {
            var expectedTags = new List<string>
            {
                // Add all tags in lower case (as they are in html)
                //"a",
                //"base",
                "body", //Top Line of Body not needed
                //"button",
                "div",
                //"foot",
                //"footer",
                //"form",
                //"i",
                //"img",
                //"input",
                //"li",
                //"link",
                //"h1",
                //"h2",
                //"h3",
                //"h4",
                //"h5",
                //"h6",
                //"head",
                //"header",
                "html", //Top Line not needed
                "meta",  //used only for meta data within a website, so no need to find this element!
                //"nav",
                //"p",
                "script", //used only for running html scripts, no interaction for this element!
                //"span",
                //"title",
                //"ul",
                ""
            };
            return expectedTags;
        }

    }
}
