using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class StepHelpers : IStepHelpers
    {
        private readonly FeatureContext featureContext;
        private readonly ITargetForms targetForms;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public StepHelpers(FeatureContext featureContext, ITargetForms targetForms)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.featureContext = featureContext;
            this.targetForms = targetForms;
            InitializeHelpers();
        }

        /// <summary>
        /// Add new ElementStepHelper.cs needs to populate BELOW
        /// </summary>
        public IAccordionStepHelper Accordion { get; private set; }
        public IAlertStepHelper Alert { get; private set; }
        public IButtonStepHelper Button { get; private set; }
        public ICheckboxStepHelper Checkbox { get; private set; }
        public IChipStepHelper Chip { get; private set; }
        public IDatePickerStepHelper DatePicker { get; private set; }
        public IDropdownStepHelper Dropdown { get; private set; }
        public IImageStepHelper Image { get; private set; }
        public ILinkStepHelper Link { get; private set; }
        public IListStepHelper List { get; private set; }
        public IOldStepHelper Old { get; private set; }
        public IOSStepHelper OS { get; private set; }
        public IPageStepHelper Page { get; private set; }
        public IRadioButtonStepHelper RadioButton { get; private set; }
        public ISliderStepHelper Slider { get; private set; }
        public ISpanStepHelper Span { get; private set; }
        public ISpinnerStepHelper Spinner { get; private set; }
        public IStepperStepHelper Stepper { get; private set; }
        public ISwitchStepHelper Switch { get; private set; }
        public ITabStepHelper Tab { get; private set; }
        public ITableStepHelper Table { get; private set; }
        public ITextBoxStepHelper TextBox { get; private set; }
        public ITimePickerStepHelper TimePicker { get; private set; }
        public ITreeStepHelper Tree { get; private set; }
        public ITSQLStepHelper TSQL { get; private set; }
        public IVisualiserStepHelper Visualiser { get; private set; }

        public IWindowStepHelper Window { get; private set; }

        private void InitializeHelpers()
        {
            Accordion = new AccordionStepHelper(featureContext, targetForms);
            Alert = new AlertStepHelper(featureContext, targetForms);
            Button = new ButtonStepHelper(featureContext, targetForms);
            Checkbox = new CheckboxStepHelper(featureContext, targetForms);
            Chip = new ChipStepHelper(featureContext, targetForms);
            DatePicker = new DatePickerStepHelper(featureContext, targetForms);
            Dropdown = new DropdownStepHelper(featureContext, targetForms);
            Image = new ImageStepHelper(featureContext, targetForms);
            Link = new LinkStepHelper(featureContext, targetForms);
            List = new ListStepHelper(featureContext, targetForms);
            Old = new OldStepHelper(featureContext, targetForms);
            OS = new OSStepHelper(featureContext, targetForms);
            Page = new PageStepHelper(featureContext, targetForms);
            RadioButton = new RadioButtonStepHelper(featureContext, targetForms);
            Slider = new SliderStepHelper(featureContext, targetForms);
            Span = new SpanStepHelper(featureContext, targetForms);
            Spinner = new SpinnerStepHelper(featureContext, targetForms);
            Stepper = new StepperStepHelper(featureContext, targetForms);
            Switch = new SwitchStepHelper(featureContext, targetForms);
            Tab = new TabStepHelper(featureContext, targetForms);
            Table = new TableStepHelper(featureContext, targetForms);
            TextBox = new TextBoxStepHelper(featureContext, targetForms);
            TimePicker = new TimePickerStepHelper(featureContext, targetForms);
            Tree = new TreeStepHelper(featureContext, targetForms);
            TSQL = new TSQLStepHelper(featureContext, targetForms);
            Visualiser = new VisualiserStepHelper(featureContext, targetForms);
            Window = new WindowStepHelper(featureContext, targetForms);
        }

    }
}
