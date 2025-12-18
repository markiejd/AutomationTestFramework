using Core;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// StepHelpers is a factory class that provides centralized access to all step helper implementations.
    /// It initializes and exposes helper instances for different UI elements and operations used in BDD test scenarios.
    /// Each helper is responsible for performing specific actions on its corresponding UI element type.
    /// </summary>
    public class StepHelpers : IStepHelpers
    {
        // Context objects required for test execution and element interaction
        private readonly FeatureContext featureContext;
        private readonly ITargetForms targetForms;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Initializes a new instance of the StepHelpers class.
        /// Sets up the feature context and target forms, then initializes all child helper instances.
        /// </summary>
        /// <param name="featureContext">The current feature context from Reqnroll/SpecFlow containing scenario data.</param>
        /// <param name="targetForms">The target forms provider for locating and interacting with UI elements.</param>
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
        
        // UI Element Helpers - Each property exposes a specific helper for interacting with its element type
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

        /// <summary>
        /// Initializes all step helper instances by creating new instances of each helper class.
        /// Each helper is configured with the same feature context and target forms.
        /// This method is called during constructor initialization to ensure all helpers are ready for use.
        /// </summary>
        private void InitializeHelpers()
        {
            // Initialize UI element helpers
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
            
            // Initialize legacy and system helpers
            Old = new OldStepHelper(featureContext, targetForms);
            OS = new OSStepHelper(featureContext, targetForms);
            
            // Initialize page and navigation helpers
            Page = new PageStepHelper(featureContext, targetForms);
            
            // Initialize input and selection helpers
            RadioButton = new RadioButtonStepHelper(featureContext, targetForms);
            Slider = new SliderStepHelper(featureContext, targetForms);
            Span = new SpanStepHelper(featureContext, targetForms);
            
            // Initialize state and progress indicator helpers
            Spinner = new SpinnerStepHelper(featureContext, targetForms);
            Stepper = new StepperStepHelper(featureContext, targetForms);
            Switch = new SwitchStepHelper(featureContext, targetForms);
            
            // Initialize navigation and data display helpers
            Tab = new TabStepHelper(featureContext, targetForms);
            Table = new TableStepHelper(featureContext, targetForms);
            
            // Initialize input and data helpers
            TextBox = new TextBoxStepHelper(featureContext, targetForms);
            TimePicker = new TimePickerStepHelper(featureContext, targetForms);
            
            // Initialize complex component helpers
            Tree = new TreeStepHelper(featureContext, targetForms);
            
            // Initialize data and visualization helpers
            TSQL = new TSQLStepHelper(featureContext, targetForms);
            Visualiser = new VisualiserStepHelper(featureContext, targetForms);
            
            // Initialize window and browser helpers
            Window = new WindowStepHelper(featureContext, targetForms);
        }

    }
}
