using System;
using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper class containing common checkbox interactions used by step definitions.
    /// </summary>
    public class CheckboxStepHelper : StepHelper, ICheckboxStepHelper
    {
        // Reference to form target helpers (kept for potential future use)
        private readonly ITargetForms _targetForms;

        // Element type used when interacting with checkbox-like controls.
        // Kept as a constant for clarity and to avoid accidental mutation.
        private const string ElementType = "Button";

        /// <summary>
        /// Constructor wiring FeatureContext and ITargetForms.
        /// Throws if targetForms is null to fail fast on incorrect DI wiring.
        /// </summary>
        public CheckboxStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            _targetForms = targetForms ?? throw new ArgumentNullException(nameof(targetForms));
        }

        /// <summary>
        /// Determines whether the checkbox with the given name is displayed on the current page.
        /// </summary>
        /// <param name="checkboxName">Logical name of the checkbox element.</param>
        /// <param name="timeout">Optional timeout (currently unused).</param>
        /// <returns>True if displayed; otherwise false.</returns>
        public bool IsDisplayed(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"IsDisplayed {checkboxName}");

            // Guard against invalid input early
            if (string.IsNullOrWhiteSpace(checkboxName))
            {
                DebugOutput.Log("IsDisplayed called with empty checkboxName");
                return false;
            }

            // Try finding the checkbox element directly first
            if (ElementInteraction.IsElementDisplayed(CurrentPage, checkboxName, "CheckBox"))
                return true;

            // Fallback: sometimes the visible representation is the parent container
            return ElementInteraction.IsElementsParentElementDisplayed(CurrentPage, checkboxName, "CheckBox");
        }

        /// <summary>
        /// Checks if the checkbox is currently selected/checked.
        /// </summary>
        /// <param name="checkboxName">Logical name of the checkbox element.</param>
        /// <param name="timeout">Optional timeout (currently unused).</param>
        /// <returns>True if selected; otherwise false.</returns>
        public bool IsSelected(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"IsSelected {checkboxName}");

            if (string.IsNullOrWhiteSpace(checkboxName))
            {
                DebugOutput.Log("IsSelected called with empty checkboxName");
                return false;
            }

            // Delegate selection check to shared ElementInteraction helper
            return ElementInteraction.IsElementSelected(CurrentPage, checkboxName, ElementType);
        }

        /// <summary>
        /// Attempts to click/select the checkbox. Uses a normal click first and falls back to
        /// a specialized click for checkboxes whose input may be hidden under another element.
        /// </summary>
        /// <param name="checkboxName">Logical name of the checkbox element.</param>
        /// <param name="timeout">Optional timeout (currently unused).</param>
        /// <returns>True if click was successful; otherwise false.</returns>
        public bool Select(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"Select {checkboxName}");

            if (string.IsNullOrWhiteSpace(checkboxName))
            {
                DebugOutput.Log("Select called with empty checkboxName");
                return false;
            }

            // Try the standard click flow first
            if (ElementInteraction.ClickOnElement(CurrentPage, checkboxName, ElementType))
                return true;

            // If normal click fails, try the specialized checkbox fallback (e.g. hidden input)
            DebugOutput.Log("Standard click failed; attempting checkbox-specific sub-element click.");
            return ElementInteraction.ClickOnElement_CheckBoxSub(CurrentPage, checkboxName, ElementType);
        }

        /// <summary>
        /// Ensures the checkbox is selected. If already selected returns true, otherwise attempts to select it.
        /// </summary>
        /// <param name="checkboxName">Logical name of the checkbox element.</param>
        /// <param name="timeout">Optional timeout (currently unused).</param>
        /// <returns>True if the end state is selected; otherwise false.</returns>
        public bool Selected(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"Selected {checkboxName}");

            if (string.IsNullOrWhiteSpace(checkboxName))
            {
                DebugOutput.Log("Selected called with empty checkboxName");
                return false;
            }

            // No action needed if already selected
            if (IsSelected(checkboxName))
                return true;

            // Attempt to select and return the result
            return Select(checkboxName, timeout);
        }

        /// <summary>
        /// Ensures the checkbox is NOT selected. If already not selected returns true, otherwise attempts to toggle it.
        /// </summary>
        /// <param name="checkboxName">Logical name of the checkbox element.</param>
        /// <param name="timeout">Optional timeout (currently unused).</param>
        /// <returns>True if the end state is not selected; otherwise false.</returns>
        public bool SelectedNot(string checkboxName, int timeout = 0)
        {
            DebugOutput.Log($"SelectedNot {checkboxName}");

            if (string.IsNullOrWhiteSpace(checkboxName))
            {
                DebugOutput.Log("SelectedNot called with empty checkboxName");
                return false;
            }

            // If it's already not selected, nothing to do
            if (!IsSelected(checkboxName))
                return true;

            // It's selected: try clicking to unselect
            return Select(checkboxName, timeout);
        }
    }
}
