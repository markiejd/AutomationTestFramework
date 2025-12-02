using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ITreeStepHelper : IStepHelper
    {
        bool ConfirmNode(string fullNode, string treeName);
        bool ExpandNodeInTree(string treeName, string nodeName);
        bool IsDisplayed(string treeName);
        bool IsNodeExist(string treeName, string nodeName);
        List<string> NodesList(string treeName);
        bool SelectNodeFromTree(string nodeName, string treeName);
    }
}
