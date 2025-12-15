using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class VisualiserStepHelper : StepHelper, IVisualiserStepHelper
    {
        private readonly ITargetForms targetForms;
        public VisualiserStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        // private By FindEntityLinkLocator = By.XPath("//a[contains(text(),'Find Entities...')]");
        // private By NodeLocator = By.ClassName("node-wrapper");
        // private By LinkLocator = By.ClassName("link");
        // private By CreateLinkLocator = By.XPath("//a[contains(text(),'Create Link...')]");
        // private By ArrangeViewLocator = By.Id("dagre_layout");
        // private By SaveLayoutLocator = By.Id("save_graph");
        // private By ResetLayoutLocator = By.Id("reset_graph");
        // private By ZoomInLocator = By.Id("zoom_in");
        // private By ZoomOutLocator = By.Id("zoom_out");
        // private By ZoomResetLocator = By.Id("zoom_reset");
        // private By ZoomToFitLocator = By.Id("zoom_to_fit");
        // private By SelectAllLocator = By.Id("select_all");
        // private By ToggleSelectionLocator = By.Id("toggle_selection");
        // private By ToggleSearchLocator = By.Id("toggle_search");
        

        public bool ClickVisualiserButton(string buttonName)
        {
            return false;
            // buttonName = buttonName.Replace(" ", "");
            // buttonName = buttonName.ToLower();
            // By? locator = null;
            // switch(buttonName)
            // {
            //     default: return false;
            //     case "zoomtofit":
            //     {
            //         locator = ZoomToFitLocator;
            //         break;
            //     }
            // }
            // if (locator == null) return false;
            // var buttonElement = SeleniumUtil.GetElement(locator);
            // if (buttonElement == null) return false;
            // return SeleniumUtil.Click(buttonElement);
        }

        /// <summary>
        /// Is the tree displayed
        /// </summary>
        /// <param name="treeName"></param>
        /// <returns></returns>
        public bool IsDisplayed()
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, "visualiser", "Visualiser");
        }

        public bool FindEntity()
        {
            DebugOutput.Log($"IsDisplayed visulaiser");
            return false;
            // var visualiserElement = GetVisualiserElement();
            // if (visualiserElement == null) return false;
            // if (SeleniumUtil.RightClick(visualiserElement))
            // {
            //     var findElementButton = SeleniumUtil.GetElement(FindEntityLinkLocator,2);
            //     if (findElementButton == null) return false;
            //     if (!SeleniumUtil.Click(findElementButton)) return false;
            //     return true;
            // }
            // DebugOutput.Log($"Failed to right click - its there just failing here");
            // return false;
        }

        public int GetHowManyNodesInVisualiser()
        {
            DebugOutput.Log($"GetHowManyNodesInVisualiser");
            return -1;
            // var visualiserElement = GetVisualiserElement();
            // if (visualiserElement == null) return 0;
            // var nodes = SeleniumUtil.GetElementsUnder(visualiserElement, NodeLocator, 1);
            // return nodes.Count();
        }

        public int GetHowManyLinksInVisualiser()
        {
            DebugOutput.Log($"GetHowManyLinksInVisualiser visulaiser");
            return -1;
            // var visualiserElement = GetVisualiserElement();
            // if (visualiserElement == null) return 0;
            // var links = SeleniumUtil.GetElementsUnder(visualiserElement, LinkLocator, 1);
            // return links.Count();
        }

        public bool KeyClickOnNode(string keyDown, string nodeText)
        {
            DebugOutput.Log($"KeyClickOnNode {keyDown}  {nodeText}");
            return false;
            // var nodeElement = GetNodeElementByText(nodeText);
            // if (nodeElement == null) return false;
            // if (!SeleniumUtil.DownKeyAndClick(nodeElement, keyDown))
            // {
            //     DebugOutput.Log($"Failed to keydown {keyDown} and click on node {nodeText}");
            //     return false;
            // }
            // DebugOutput.Log($"Should be done!");
            // return true;
        }

        // public bool RightClickOnNode(string nodeText)
        // {
        //     DebugOutput.Log($"RightClickOnNode {nodeText}");
        //     return false;
        //     var nodeElement = GetNodeElementByText(nodeText);
        //     if (nodeElement == null) return false;
        //     if (!SeleniumUtil.RightClick(nodeElement)) return false;
        //     DebugOutput.Log($"Should be right cliked!");
        //     return true;
        // }

        public bool CreateLinkBetweenNodes(string node1Text, string node2Text)
        {
            DebugOutput.Log($"CreateLinkBetweenNodes {node1Text} {node2Text}");
            return false;
            // // if (!KeyClickOnNode("ctrl", node1Text)) return false;
            // // DebugOutput.Log($"Ctrl on node 1 done");
            // // if (!KeyClickOnNode("ctrl", node2Text)) return false;
            // // DebugOutput.Log($"Ctrl on node 2 done");
            // // if (!RightClickOnNode(node2Text)) return false;
            // // DebugOutput.Log($"Right on node 2 done");
            // var createLinkElement = SeleniumUtil.GetElement(CreateLinkLocator);
            // if (createLinkElement == null)
            // {
            //     DebugOutput.Log($"Failed to find create link element!");
            //     return false;
            // }
            // DebugOutput.Log($"Attempting Click");
            // return SeleniumUtil.Click(createLinkElement);
        }



        /////////////////////////////   PRIVATE

        
        /// <summary>
        /// Find the tree element
        /// </summary>
        /// <param name="treeName"></param>
        /// <returns></returns>
        // private IWebElement? GetVisualiserElement()
        // {
        //     DebugOutput.Log($"GetVisualiserElement ALWAYS  'visualiser'");
        //     return null;
        //     // var visualisierLocator = GetDictionaryLocator.GetElementLocator("visualiser", CurrentPage, "visualiser");
        //     // if (visualisierLocator == null) return null;   
        //     // DebugOutput.Log($"We have the LOCATOR for visualiser {visualisierLocator}");
        //     // var element = SeleniumUtil.GetElement(visualisierLocator, 1);
        //     // DebugOutput.Log($"Visualiser Element {visualisierLocator} = {element}");
        //     // return element;
        // }

        // private List<IWebElement> GetAllNodesInVisualiser()
        // {
        //     DebugOutput.Log($"GetAllNodesInVisualiser");
        //     var allNodes = new List<IWebElement>();
        //     return allNodes;
        //     // var visualiserElement = GetVisualiserElement();
        //     // if (visualiserElement == null) return allNodes;
        //     // allNodes = SeleniumUtil.GetElementsUnder(visualiserElement, NodeLocator, 1);
        //     // DebugOutput.Log($"There are {allNodes.Count()} Nodes returned!");
        //     // return allNodes;
        // }

        // private IWebElement? GetNodeElementByText(string text)
        // {
        //     DebugOutput.Log($"GetNodeElementByText");
        //     return null;
        //     // var allNodes = GetAllNodesInVisualiser();
        //     // foreach(var node in allNodes)
        //     // {
        //     //     var nodeText = SeleniumUtil.GetElementtextDirect(node);
        //     //     if (nodeText != null)
        //     //     {
        //     //         if (nodeText.Contains(text)) return node;
        //     //     }
        //     // }
        //     // DebugOutput.Log($"Found none!");
        //     // return null;
        // }




    }
}
