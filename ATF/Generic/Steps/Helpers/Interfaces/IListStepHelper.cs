using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IListStepHelper : IStepHelper
    {
        bool IsDisplayed(string list);
        bool ListContainsValue(string list, string value);
    }
}
