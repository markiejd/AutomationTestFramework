

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IStepHelpers
    {
        /// <summary>
        /// Add new ElementStepHelper.cs? needs to populate BELOW
        /// </summary>
        /// 
        IAccordionStepHelper Accordion { get; }
        IAlertStepHelper Alert { get; }
        IButtonStepHelper Button { get; }
        IChipStepHelper Chip { get; }
        ICheckboxStepHelper Checkbox { get; }
        IDatePickerStepHelper DatePicker { get; }
        IDropdownStepHelper Dropdown { get; }
        IImageStepHelper Image { get; }
        ILinkStepHelper Link { get; }
        IListStepHelper List { get; }
        IOldStepHelper Old { get; }
        IOSStepHelper OS { get; }
        IPageStepHelper Page { get; }
        IRadioButtonStepHelper RadioButton { get; }
        ISliderStepHelper Slider { get; }
        ISpanStepHelper Span { get; }
        ISpinnerStepHelper Spinner { get; }
        IStepperStepHelper Stepper { get; }
        ISwitchStepHelper Switch { get; }
        ITabStepHelper Tab { get; }
        ITableStepHelper Table { get; }
        ITextBoxStepHelper TextBox { get; }
        ITimePickerStepHelper TimePicker { get; }
        ITreeStepHelper Tree { get; }
        ITSQLStepHelper TSQL { get; }
        IVisualiserStepHelper Visualiser { get; }
        IWindowStepHelper Window { get; }
    }
}
