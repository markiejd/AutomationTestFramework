using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using TechTalk.SpecFlow;

namespace Generic.Steps.Helpers.Classes
{
    public class TreeStepHelper : StepHelper, ITreeStepHelper
    {
        private readonly ITargetForms targetForms;
        public TreeStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        // //1st is web, 2nd is Windows
        // private readonly By[] TreeNodeLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeLocator);
        // private readonly By[] TreeNodeToggleLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeToggleLocator);
        // private readonly By[] TreeNodeSelector = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeSelector);
        // private readonly String[] TreeAddNodeText = TargetLocator.Configuration.TreeAddNodeText;
        // private readonly String[] TreeAddNodeButton = TargetLocator.Configuration.TableNextPageButton;

        /// <summary>
        /// Is the tree displayed
        /// </summary>
        /// <param name="treeName"></param>
        /// <returns></returns>
        public bool IsDisplayed(string treeName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, treeName, "Tree");
        }

        /// <summary>
        /// Checks the list of Node Names to see if the nodeName is in there (case sensative)
        /// </summary>
        /// <param name="treeName"></param>
        /// <param name="nodeName"></param>
        /// <returns>true if found!</returns>
        public bool IsNodeExist(string treeName, string nodeName)
        {
            DebugOutput.Log($"Proc - IsNodeDisplayed {treeName} {nodeName}");
            return ElementInteraction.IsSubElementDisplayed(CurrentPage, treeName, "tree", nodeName);
        }

        /// <summary>
        /// Get all the node names
        /// </summary>
        /// <param name="treeName"></param>
        /// <param name="nodeNameValue"></param>
        /// <returns></returns>
        public List<string> NodesList(string treeName)
        {
            DebugOutput.Log($"Proc - NodesDisplayed {treeName}");
            List<string> nodeNames = new List<string>();
            return ElementInteraction.GetSubElementsTextOfElement(CurrentPage, treeName, "tree") ?? nodeNames;
        }


        public bool ConfirmNode(string fullNode, string treeName)
        {
            DebugOutput.Log($"Proc - ConfirmNode {fullNode} {treeName}");
            return false;
        }

        public bool EnterNodeDetails(string nodeName)
        {
            DebugOutput.Log($"Proc - EnterNodeDetails {nodeName}");
            return false;
        }

        // public bool NodeExistUnderNode(IWebElement parentNode, string nodeName)
        // {
        //     DebugOutput.Log($"Proc - NodeExistUnderNode {parentNode} {nodeName}");
        //     return false;
        // }

        public bool SelectNodeFromTree(string nodeName, string treeName)
        {
            DebugOutput.Log($"Proc - SelectNodeFromTree {nodeName} {treeName}");
            return false;
        }

        public bool ExpandNodeInTree(string treeName, string nodeName)
        {
            DebugOutput.Log($"Proc - SelectNodeFromTree {nodeName} {treeName}");
            return false;
        }

        // public bool ExpandNode(IWebElement nodeElement)
        // {
        //     DebugOutput.Log($"Proc - ExpandNode {nodeElement}");
        //     return false;
        // }

    }
}
