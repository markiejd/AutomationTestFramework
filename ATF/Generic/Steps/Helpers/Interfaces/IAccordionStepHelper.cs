using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IAccordionStepHelper : IStepHelper
    {
        bool AccordionClick(string accordionName);
        bool AccordionItemClick(string accordionName, string item);
        bool ButtonClick(string accordionName, string groupName);
        bool GroupClick(string accordionName, string groupName);
        bool GroupIsDisplayed(string accordionName, string groupName);
        bool GroupIsExpanded(string accordionName, string groupName);
        bool GroupIsNotExpanded(string accordionName, string groupName);
        bool IsAccordionExpanded(string accordionName);
        bool IsButtonDisplayed(string accordionName, string buttonName);
        bool IsDisplayed(string accordionName);
    }
}
