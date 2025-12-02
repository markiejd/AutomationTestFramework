using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;

namespace Core.Configuration
{
    public class TargetLocator
    {
        public static TargetLocatorData Configuration { get; private set; } = new TargetLocatorData();
        public class TargetLocatorData
        {
            public string[] DatePickerButtonOpen { get; set; } = { "By.Name(\"PART_DropDownButton\")" };
            public string[] DatePickerText { get; set; } = { "By.ClassName(\"RadWatermarkTextBox\")", "By.Name(\"PART_DropDownButton\")" };
            public string[] DropDownItemLocators { get; set; } = 
            { 
                "By.ClassName(\"RadComboBoxItem\")",
                "By.TagName(\"option\")",
                "By.TagName(\"li\")" 
            };
            public string[] ListItemLocator { get; set; } = 
            { 
                "By.ClassName(\"MuiButtonBase-root\")",
                "By.ClassName(\"RadListBoxItem\")"
            };
            public string[] SpinnerLocator { get; set; } = 
            {                 
                "By.Id(\"loading\")",
                "By.Id(\"spinner\")"
            };
            public string[] StepperStepLocator { get; set; } = { "By.TagName(\"a\")" };
            public string[] TabLocator { get; set; } = 
            { 
                "By.XPath(\"//div[@role='tab']\")",
                "By.ClassName(\"nav-item\")",
                "By.ClassName(\"RadTabItem\")"
            };
            public string[] TabTextLocator { get; set; } = { "By.XPath(\"//span\")" };
            public int TablePrimaryColumnNumber { get; set; } = 1;
            public string[] TableActions { get; set; } = 
            { 
                "accept & open",
                "view",
                "view exhibits",
                "view receipt",
                "upload",
                "...",
                "import exhibits",
                "select",
                "view / update" 
            };
            public string[] TableActionsLocator { get; set; } = 
            { 
                "By.XPath(\"//span[2]/button[1]\")",
                "By.XPath(\"//span[1]/button[1]\")",
                "By.XPath(\"//*[@Name='View Exhibits']\")",
                "By.Name(\"View Receipt\")",
                "By.Id(\"Upload\")",
                "By.Id(\"trigger\")",
                "By.Id(\"importExhibits\")",
                "By.ClassName(\"tdcenter\")",
                "By.Id(\"viewPersonActionButton\")"
            }; 
            public string[] TableHeadLocator { get; set; } = 
            { 
                "By.XPath(\"//thead\")",
                "By.ClassName(\"GridViewHeaderRow\")"
            };
            public string[] TableHeadCellsLocator { get; set; } = 
            { 
                "By.TagName(\"th\")",
                "By.ClassName(\"header\")",
                "By.ClassName(\"GridViewHeaderCell\")",
                "By.ClassName(\"datatable-header-cell\")" 
            };
            public string[] TableBodyLocator { get; set; } = 
            { 
                "By.TagName(\"tbody\")",
                "By.ClassName(\"PART_GridViewVirtualizingPanel\")" 
            };
            public string[] TableBodyRowLocator { get; set; } = 
            { 
                "By.ClassName(\"table-row\")",
                "By.TagName(\"tr\")",
                "By.XPath(\"//tbody/tr\")",
                "By.ClassName(\"GridViewRow\")",
                "By.ClassName(\"datatable-body-row\")"
            };
            public string[] TableBodyRowSubLocator { get; set; } = { "By.ClassName(\"ExhibitsSearchScene.JsonDAL.Models.Search\")" };
            public string[] TableBodyCellsLocator { get; set; } = 
            { 
                "By.TagName(\"td\")",
                "By.ClassName(\"datatable-body-cell\")",
                "By.ClassName(\"GridViewCell\")",
                "By.XPath(\"./td\")"
            };
            public string[] TableNextPageButton { get; set; } = 
            { 
                "By.CSS_SELECTOR, '[title=\"Go to next page\"][aria-label=\"Go to next page\"]'",
                "By.XPath(\"//button[contains(text(),'Next')]\")"
            };
            public string[] TablePreviousPageButton { get; set; } = { "By.XPath(\"//button[contains(text(),'Previous')]\")" };
            public string[] TableFilterLocator { get; set; } = { "By.Id(\"searchBox\")" };
            public string[] TimePickerButtonOpen { get; set; } = { "By.Name(\"PART_DropDownButton\")" };
            public string[] TimePickerText { get; set; } = { "By.ClassName(\"RadWatermarkTextBox\")" };
            public string[] TreeNodeLocator { get; set; } = 
            { 
                "By.ClassName(\"RadTreeViewItem\")",
                "By.ClassName(\"leaf\")"
            };
            public string[] TreeNodeSelector { get; set;} = { "By.ClassName(\"org\")" };
            public string[] TreeNodeToggleLocator { get; set; } = 
            {
                "By.Id(\"Expander\")",
                "By.ClassName(\"hitarea\")"
            };
            public string[] TreeAddNodeText { get; set; } = { "new location" };
            public string[] TreeAddNodeButton { get; set; } = { "add new location" };
        }
        

        //public static void TargetConfiguration()
        public static TargetLocatorData? ReadJson()
        {
            string currentDirectory = Environment.CurrentDirectory;
            if (!currentDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                currentDirectory += Path.DirectorySeparatorChar;
            }
            var fileName = $"Locators.json";
            var directory = "/Core/Configuration/Resources/";
            var fullFileName = currentDirectory + directory + fileName;
            if (!FileUtils.OSFileCheck(fullFileName))
            {
                DebugOutput.Log($"Unable to find the file {fullFileName}");
                return null;
            }
            var jsonText = File.ReadAllText(fullFileName);
            DebugOutput.Log($"Json - {jsonText}");
            try
            {
                var obj = JsonConvert.DeserializeObject<TargetLocatorData>(jsonText);
                if (obj == null) return null;
                Configuration = obj;
                // DebugOutput.Log($">>>> {obj.AreaPath}  ... {Configuration.AreaPath}");
                // DebugOutput.Log($"THIS DEBUG LEVEL SET TO {System.Environment.GetEnvironmentVariable("ENVIRONMENT")}");
                return Configuration;
            }
            catch
            {
                DebugOutput.Log($"We out ere");
                return null;
            }
        }
    }

}