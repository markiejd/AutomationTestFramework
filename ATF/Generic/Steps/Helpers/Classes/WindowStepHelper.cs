using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Helper for interacting with application windows in step definitions.
    /// Provides utilities for sizing, closing, visibility checks, and text entry.
    /// </summary>
    public class WindowStepHelper : StepHelper, IWindowStepHelper
    {
        // Provides lookup and configuration for target forms used in the current test context.
        private readonly ITargetForms targetForms;

        /// <summary>
        /// Initializes a new instance of <see cref="WindowStepHelper"/>.
        /// </summary>
        /// <param name="featureContext">The current feature context.</param>
        /// <param name="targetForms">Form target provider used by steps.</param>
        public WindowStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            // Store dependency for later use by step helpers.
            this.targetForms = targetForms;
        }

        /// <summary>
        /// Sets the window size using explicit width and height values.
        /// </summary>
        /// <param name="width">Desired window width in pixels.</param>
        /// <param name="height">Desired window height in pixels.</param>
        /// <returns>True if the operation succeeded; otherwise false.</returns>
        public bool SizeOfWindow(int width, int height)
        {
            // Log inputs for traceability.
            DebugOutput.Log($"SizeOfWindow {width} {height} ");

            // Guard against non-sensical sizes (negative or zero).
            if (width <= 0 || height <= 0)
            {
                DebugOutput.Log("Width and height must be positive integers.");
                return false;
            }

            // Delegate to ElementInteraction which performs the actual resize.
            return ElementInteraction.SetWindowSize(width, height);
        }

        /// <summary>
        /// Sets the window size from a composite string (e.g., "1024x768" or "default").
        /// </summary>
        /// <param name="compositeSize">Size string formatted as "widthxheight" or "default".</param>
        /// <returns>True if the operation succeeded; otherwise false.</returns>
        public bool SizeOfWindowString(string compositeSize)
        {
            // Log the raw input for diagnostics.
            DebugOutput.Log($"SizeOfWindow {compositeSize}");

            // Fallback defaults in case parsing fails.
            int width = 800;   // Default width
            int height = 800;  // Default height

            // Validate input: must not be null or whitespace.
            if (string.IsNullOrWhiteSpace(compositeSize))
            {
                DebugOutput.Log("Composite size must not be null or empty.");
                return false;
            }

            // Normalize input for consistent parsing (trim and lower for comparison).
            compositeSize = compositeSize.Trim();

            // Support a "default" token that pulls from target configuration.
            if (compositeSize.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                compositeSize = TargetConfiguration.Configuration.ScreenSize;
            }

            // Expect an "x" delimiter between width and height.
            var sizes = StringValues.BreakUpByDelimited(compositeSize, "x");
            if (sizes.Count() != 2)
            {
                DebugOutput.Log($"You need an x and y! Delimited by X You gave us {compositeSize}");
                return false;
            }

            try
            {
                // Parse width and height from tokens safely.
                if (!int.TryParse(sizes[0], out width) || !int.TryParse(sizes[1], out height))
                {
                    DebugOutput.Log($"Failed to convert {compositeSize} to ints");
                    return false;
                }

                // Guard against non-sensical sizes (negative or zero).
                if (width <= 0 || height <= 0)
                {
                    DebugOutput.Log("Width and height must be positive integers.");
                    return false;
                }
            }
            catch
            {
                // Defensive: in case token access throws unexpectedly.
                DebugOutput.Log($"Failed to convert {compositeSize} to ints");
                return false;
            }

            // Apply the parsed size to the window.
            return ElementInteraction.SetWindowSize(width, height);
        }

        /// <summary>
        /// Sends a close command to a window by name.
        /// </summary>
        /// <param name="windowsName">The target window name.</param>
        /// <returns>True if the close command was issued successfully; otherwise false.</returns>
        public bool CloseWindow(string windowsName)
        {
            // Log the action for traceability.
            DebugOutput.Log($"CloseWindow ");

            // Basic validation to avoid empty targets.
            if (string.IsNullOrWhiteSpace(windowsName))
            {
                DebugOutput.Log("Window name must not be null or empty.");
                return false;
            }

            // Enter the window name and issue a close key/action.
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, windowsName, "window", "", "close");
        }

        /// <summary>
        /// Attempts to close the currently focused top element (e.g., modal/dialog) and confirm.
        /// </summary>
        /// <returns>True if the top element was closed and confirmed; otherwise false.</returns>
        public bool CloseTopElement()
        {
            // Log the intent to close the top element.
            DebugOutput.Log($"CloseTopElement ");

            // Issue a close action to the top editable element.
            // Fix typo: "wundow" -> "window" to ensure correct target type.
            ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, "EditableTextBox", "window", "", "close");

            // Confirm the close by clicking the affirmative button.
            return ElementInteraction.ClickOnElement(CurrentPage, "Yes", "Button");
        }

        /// <summary>
        /// Checks if a given window is displayed.
        /// </summary>
        /// <param name="windowsName">The window name to check.</param>
        /// <returns>True if the window is visible; otherwise false.</returns>
        public bool IsDisplayed(string windowsName)
        {
            // Validate input to prevent unnecessary UI queries.
            if (string.IsNullOrWhiteSpace(windowsName))
            {
                DebugOutput.Log("Window name must not be null or empty.");
                return false;
            }

            // Query the UI layer for visibility of the target window.
            return ElementInteraction.IsElementDisplayed(CurrentPage, windowsName, "Windows");
        }

        /// <summary>
        /// Writes text into a document or window by name.
        /// </summary>
        /// <param name="documentName">Target document/window name.</param>
        /// <param name="text">Text content to write.</param>
        /// <returns>True if text entry succeeded; otherwise false.</returns>
        public bool WriteInWindow(string documentName, string text)
        {
            // Validate inputs to ensure meaningful operation.
            if (string.IsNullOrWhiteSpace(documentName))
            {
                DebugOutput.Log("Document name must not be null or empty.");
                return false;
            }

            // Treat null text as empty to avoid null reference issues.
            text ??= string.Empty;

            // Delegate text entry to the interaction layer.
            return ElementInteraction.EnterTextAndKeyIntoElement(CurrentPage, documentName, "window", text);
        }
    }
}
