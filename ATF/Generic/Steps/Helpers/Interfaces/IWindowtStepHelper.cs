using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IWindowStepHelper : IStepHelper
    {
        bool CloseTopElement();
        bool CloseWindow(string windowsName);
        bool IsDisplayed(string windowsName);
        bool SizeOfWindow(int width, int height);
        bool SizeOfWindowString(string compositeSize);
        bool WriteInWindow(string documentName, string text);
    }
}
