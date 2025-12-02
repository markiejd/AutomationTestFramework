
using Core.Configuration;
using Core.Transformations;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    public static class GetSubLocators
    {
        /// <summary>
        /// ACCORDION
        /// </summary>      
        /// 
        public static string[] AccordionSubElementXPathStart = { $"//div[contains(text(),'", "//p[contains(text(),'" };
        public static string[] AccordionSubElementXPathEnd = { $"')]/../../../span[1]/div[1]", "')]/../../../../../../.." };  // mat-expansion-panel
        public static string[] AccordionSubElementExpanded = { $"./../../*[@class='element-list collapse show']", "mat-expansion-panel-header" }; 
        public static int AccordionVersionNumber = 1;

        /// <summary>
        /// CHECKBOX
        /// </summary>      
        /// 
        public static By CheckBoxLabelElementLocator = By.ClassName("form-check-label");

        /// <summary>
        /// CHIPS
        /// </summary>      
        /// 
        public static By ChipLocator = By.TagName("alert");
        public static By ChipCloseLocator = By.TagName("button");
        public static By ChipTextLocator { get; set; } = By.ClassName("alert-info");

        /// <summary>
        /// DATE PICKER
        /// </summary>      
        /// 
        public static By[] DatePickerText = LocatorValues.locatorParser(TargetLocator.Configuration.DatePickerText);

        /// <summary>
        /// DROPDOWNS
        /// </summary>      
        /// 
        // public static By[] DropDownItemLocators = LocatorValues.locatorParser(TargetLocator.Configuration.DropDownItemLocators);
        public static By[] DropDownItemLocators = new[]
        {
            By.TagName("mat-option"),
            By.ClassName("RadComboBoxItem"),
            By.TagName("option"),
            By.TagName("li")
        };


        /// <summary>
        /// List
        /// </summary>
        public static By[] ListItemLocator = LocatorValues.locatorParser(TargetLocator.Configuration.ListItemLocator);

        /// <summary>
        /// SPINNERS
        /// </summary>
        public static By[] SpinnerLocators = LocatorValues.locatorParser(TargetLocator.Configuration.SpinnerLocator);

        /// <summary>
        /// STEPPERS
        /// </summary>
        public static By[] StepperStepLocator = LocatorValues.locatorParser(TargetLocator.Configuration.StepperStepLocator);
        public static By[] StepperStepLabelLocator = { By.ClassName("item-label") };

        /// <summary>
        /// TAB
        /// </summary>
        public static By[] TabLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TabLocator);

        /// <summary>
        /// TABLE
        /// </summary>
        public static By[] TableHeadLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableHeadLocator);
        public static By[] TableHeadCellsLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableHeadCellsLocator);
        public static By[] TableBodyLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyLocator);
        public static By[] TableBodyRowLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyRowLocator);
        public static By[] TableBodyRowSubLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyRowSubLocator);
        public static By[] TableBodyCellsLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TableBodyCellsLocator);
        public static By[] TableNextPageButton = LocatorValues.locatorParser(TargetLocator.Configuration.TableNextPageButton);

        /// <summary>
        /// TREE
        /// </summary>
        public static By[] TreeNodeLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeLocator);
        public static By[] TreeNodeToggleLocator = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeToggleLocator);
        public static By[] TreeNodeSelector = LocatorValues.locatorParser(TargetLocator.Configuration.TreeNodeSelector);




    }
}