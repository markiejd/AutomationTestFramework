using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IVisualiserStepHelper : IStepHelper
    {
        bool ClickVisualiserButton(string buttonName);
        bool CreateLinkBetweenNodes(string node1Text, string node2Text);
        bool FindEntity();
        int GetHowManyLinksInVisualiser();
        int GetHowManyNodesInVisualiser();
        bool KeyClickOnNode(string keyDown, string nodeText);
        bool IsDisplayed();
        // bool RightClickOnNode(string nodeText);
    }
}
