using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class for interacting with tree components across different target forms.
    /// Provides common actions such as display checks, node existence checks,
    /// listing nodes, selecting and expanding nodes.
    /// </summary>
    public class TreeStepHelper : StepHelper, ITreeStepHelper
    {
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of the TreeStepHelper.
        /// </summary>
        /// <param name="featureContext">Current Reqnroll feature context.</param>
        /// <param name="targetForms">Target forms provider to resolve platform-specific behavior.</param>
        public TreeStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            // Store the target forms reference for later platform-specific logic.
            this.targetForms = targetForms;
        }

        // //1st is web, 2nd is Windows
        // private readonly By[] TreeNodeLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeLocator);
        // private readonly By[] TreeNodeToggleLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeToggleLocator);
        // private readonly By[] TreeNodeSelector = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeSelector);
        // private readonly String[] TreeAddNodeText = TargetLocator.Configuration.TreeAddNodeText;
        // private readonly String[] TreeAddNodeButton = TargetLocator.Configuration.TableNextPageButton;

        /// <summary>
        /// Determines whether the tree identified by name is displayed on the current page.
        /// </summary>
        /// <param name="treeName">The logical name of the tree element.</param>
        /// <returns>True if the tree is displayed; otherwise, false.</returns>
        public bool IsDisplayed(string treeName)
        {
            // Delegate to a generic element interaction helper for display checks.
            return ElementInteraction.IsElementDisplayed(CurrentPage, treeName, "Tree");
        }

        /// <summary>
        /// Checks if a node exists within the specified tree (case sensitive match).
        /// </summary>
        /// <param name="treeName">The logical name of the tree element.</param>
        /// <param name="nodeName">The exact node text to search for.</param>
        /// <returns>True if the node exists; otherwise, false.</returns>
        public bool IsNodeExist(string treeName, string nodeName)
        {
            DebugOutput.Log($"Proc - IsNodeDisplayed {treeName} {nodeName}");
            // Search for a sub-element with text equal to nodeName within the tree.
            return ElementInteraction.IsSubElementDisplayed(CurrentPage, treeName, "tree", nodeName);
        }

        /// <summary>
        /// Retrieves a list of all node names under the specified tree.
        /// </summary>
        /// <param name="treeName">The logical name of the tree element.</param>
        /// <returns>List of node names; empty list if none are found.</returns>
        public List<string> NodesList(string treeName)
        {
            DebugOutput.Log($"Proc - NodesDisplayed {treeName}");
            List<string> nodeNames = new List<string>();
            // Returns all sub-elements text under the tree; falls back to empty list.
            return ElementInteraction.GetSubElementsTextOfElement(CurrentPage, treeName, "tree") ?? nodeNames;
        }

        /// <summary>
        /// Confirms the existence and possibly the structure of a node path within the tree.
        /// </summary>
        /// <param name="fullNode">Full path or identifier of the node (e.g., "Root/Child/Subchild").</param>
        /// <param name="treeName">The logical name of the tree element.</param>
        /// <returns>True if the node path is confirmed; otherwise, false.</returns>
        public bool ConfirmNode(string fullNode, string treeName)
        {
            DebugOutput.Log($"Proc - ConfirmNode {fullNode} {treeName}");
            // Intended flow:
            // 1. Parse fullNode into segments.
            // 2. Iteratively expand/verify each segment exists.
            // 3. Return true only if the final segment is present.
            // TODO: Implement node path parsing and verification logic.
            return false;
        }

        /// <summary>
        /// Enters details for creating or updating a node (e.g., typing a name into an input field).
        /// </summary>
        /// <param name="nodeName">The node name to enter.</param>
        /// <returns>True if entry succeeds; otherwise, false.</returns>
        public bool EnterNodeDetails(string nodeName)
        {
            DebugOutput.Log($"Proc - EnterNodeDetails {nodeName}");
            // Intended flow:
            // 1. Locate the input field associated with node creation.
            // 2. Clear existing text and type the provided nodeName.
            // 3. Optionally click a confirm/save button.
            // 4. Verify the node now exists.
            // TODO: Implement interaction with input and confirmation elements.
            return false;
        }

        // public bool NodeExistUnderNode(IWebElement parentNode, string nodeName)
        // {
        //     DebugOutput.Log($"Proc - NodeExistUnderNode {parentNode} {nodeName}");
        //     // Intended flow: search within the children of parentNode for a matching nodeName.
        //     return false;
        // }

        /// <summary>
        /// Selects a node from the tree by its visible text.
        /// </summary>
        /// <param name="nodeName">The exact text of the node to select.</param>
        /// <param name="treeName">The logical name of the tree element.</param>
        /// <returns>True if the node is successfully selected; otherwise, false.</returns>
        public bool SelectNodeFromTree(string nodeName, string treeName)
        {
            DebugOutput.Log($"Proc - SelectNodeFromTree {nodeName} {treeName}");
            // Intended flow:
            // 1. Ensure the tree is displayed.
            // 2. Expand parents as needed so the node becomes visible.
            // 3. Click/tap the node element.
            // 4. Optionally verify selection state (e.g., active/selected class).
            // TODO: Implement selection logic using ElementInteraction APIs.
            return false;
        }

        /// <summary>
        /// Expands a specific node in the tree to reveal its children.
        /// </summary>
        /// <param name="treeName">The logical name of the tree element.</param>
        /// <param name="nodeName">The exact text of the node to expand.</param>
        /// <returns>True if expansion succeeds; otherwise, false.</returns>
        public bool ExpandNodeInTree(string treeName, string nodeName)
        {
            DebugOutput.Log($"Proc - SelectNodeFromTree {nodeName} {treeName}");
            // Intended flow:
            // 1. Locate the node by text.
            // 2. Find the associated expand/collapse toggle.
            // 3. Click the toggle to expand if currently collapsed.
            // 4. Verify children are now visible.
            // TODO: Implement expand logic using toggle locators.
            return false;
        }

        // public bool ExpandNode(IWebElement nodeElement)
        // {
        //     DebugOutput.Log($"Proc - ExpandNode {nodeElement}");
        //     // Intended flow: locate toggle within nodeElement and click to expand.
        //     return false;
        // }

    }
}
