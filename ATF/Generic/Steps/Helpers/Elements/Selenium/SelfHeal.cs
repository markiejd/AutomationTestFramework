using System;
using System.Collections.Generic;
using System.Linq;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Represents a persisted element for the self-heal mechanism.
    /// Only a subset of element properties are stored.
    /// </summary>
    public class SelfHealModel
    {
        /// <summary>Page identifier where the element lives.</summary>
        public string PageName { get; set; } = string.Empty;

        /// <summary>Logical name of the element.</summary>
        public string ElementName { get; set; } = string.Empty;

        /// <summary>Type/category of the element (e.g. Button, Input).</summary>
        public string ElementType { get; set; } = string.Empty;

        /// <summary>Known locator (POM) when available. May be null and is ignored during serialization when null.</summary>
        public By? ElementKnownLocator { get; set; }

        /// <summary>Computed XPath string for the element when available.</summary>
        public string? ElementXPathString { get; set; }

        /// <summary>Visible text of the element, if any.</summary>
        public string? ElementText { get; set; }

        /// <summary>Tag name of the element, if known.</summary>
        public string? ElementTag { get; set; }

        /// <summary>Whether the element was enabled when recorded.</summary>
        public bool ElementEnabled { get; set; } = true;
    }

    /// <summary>
    /// Utilities for reading and writing SelfHealModel entries to disk.
    /// The persistent format is JSON, one model per line in Elements.txt.
    /// </summary>
    public static class SelfHeal
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Serializes a SelfHealModel into a compact JSON string (omits nulls).
        /// </summary>
        public static string CreateJsonString(SelfHealModel model)
        {
            DebugOutput.Log($"CreateJsonString {model.PageName} {model.ElementName} {model.ElementType}");
            return JsonConvert.SerializeObject(model, Formatting.None, SerializerSettings);
        }

        /// <summary>
        /// Deserializes a JSON line into a SelfHealModel. Returns null on failure.
        /// </summary>
        public static SelfHealModel? CreateModelFromJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString)) return null;

            try
            {
                var model = JsonConvert.DeserializeObject<SelfHealModel>(jsonString);
                if (model == null)
                {
                    DebugOutput.Log($"CreateModelFromJson: Deserialization returned null for input: {jsonString}");
                }
                return model;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"CreateModelFromJson: Failed to deserialize json. Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Writes or updates a SelfHealModel entry for the given page.
        /// </summary>
        public static bool WriteToFile(SelfHealModel model)
        {
            var osFile = GetElementFileDetails(model.PageName);
            return AppendFile(osFile, model);
        }

        /// <summary>
        /// Reads the first matching model for the given page / element name / type. Returns null if not found.
        /// </summary>
        public static SelfHealModel? GetSelfHealModel(string pageName, string elementName, string elementType)
        {
            var osFile = GetElementFileDetails(pageName);
            if (!FileUtils.OSFileCheck(osFile)) return null;

            var textFileAsLines = FileUtils.OSGetFileContentsAsListOfStringByLine(osFile);
            if (textFileAsLines == null) return null;

            DebugOutput.Log($"WE have {textFileAsLines.Count} lines in the file");

            foreach (var line in textFileAsLines)
            {
                var checkModel = CreateModelFromJson(line);
                if (checkModel == null) continue; // skip malformed lines

                if (string.Equals(checkModel.PageName, pageName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(checkModel.ElementName, elementName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(checkModel.ElementType, elementType, StringComparison.OrdinalIgnoreCase))
                {
                    DebugOutput.Log($"Found matching model for page:{pageName} element:{elementName} type:{elementType}");
                    return checkModel;
                }
            }

            DebugOutput.Log($"No matching model found for page:{pageName} element:{elementName} type:{elementType}");
            return null;
        }

        // Build the expected file path for a page's Elements.txt
        private static string GetElementFileDetails(string pageName)
        {
            var outputLocation = "/AppTargets/Forms/" + TargetConfiguration.Configuration.AreaPath + "/SelfHeal/" + pageName + "/";
            var OSSelfHelpDirectory = FileUtils.GetCorrectDirectory(outputLocation);
            DebugOutput.Log($"SelfHeal directory: {OSSelfHelpDirectory}");
            if (!FileUtils.OSDirectoryCheck(OSSelfHelpDirectory)) FileUtils.OSDirectoryCreation(OSSelfHelpDirectory);
            var OSSelfHelpFile = OSSelfHelpDirectory + "Elements.txt";
            DebugOutput.Log($"SelfHeal file: {OSSelfHelpFile}");
            return OSSelfHelpFile;
        }

        /// <summary>
        /// Appends or replaces a model line in the file. If the exact model already exists nothing is changed.
        /// If the page/name/type exists but other properties differ the old line is replaced.
        /// </summary>
        private static bool AppendFile(string osFile, SelfHealModel model)
        {
            DebugOutput.Log($"AppendFile {osFile} {model.PageName} {model.ElementName} {model.ElementType}");

            // If file does not exist, create and add the first line.
            if (!FileUtils.OSFileCheck(osFile))
            {
                DebugOutput.Log("Creating new self-heal file.");
                if (!FileUtils.OSFileCreation(osFile))
                {
                    DebugOutput.Log($"Failed to create file {osFile}");
                    return false;
                }

                var inputLine = CreateJsonString(model);
                // Return the result of the append operation (true on success).
                return FileUtils.OSAppendLineToFile(osFile, inputLine);
            }

            var linesFromFile = FileUtils.OSGetFileContentsAsListOfStringByLine(osFile);
            if (linesFromFile == null)
            {
                DebugOutput.Log("Existing file could not be read.");
                return false;
            }

            DebugOutput.Log($"The file has {linesFromFile.Count} lines.");

            var newLinesForFile = new List<string>();

            foreach (var line in linesFromFile)
            {
                var lineModel = CreateModelFromJson(line);
                if (lineModel == null)
                {
                    // Keep malformed lines to avoid data loss, but log.
                    DebugOutput.Log($"Skipping malformed line: {line}");
                    newLinesForFile.Add(line);
                    continue;
                }

                // If page/name/type match, decide whether to replace or keep original.
                if (string.Equals(lineModel.PageName, model.PageName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(lineModel.ElementName, model.ElementName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(lineModel.ElementType, model.ElementType, StringComparison.OrdinalIgnoreCase))
                {
                    // If the models are fully equal, keep the original.
                    if (AreModelsEqual(lineModel, model))
                    {
                        DebugOutput.Log("Found identical model already stored; nothing to change.");
                        newLinesForFile.Add(line);
                    }
                    else
                    {
                        // Different data for same page/name/type -> replace (i.e. skip old line)
                        DebugOutput.Log("Found same page/name/type but different data; will replace old entry.");
                        // do not add the old line (it will be replaced)
                    }
                }
                else
                {
                    // Different element -> keep the line
                    newLinesForFile.Add(line);
                }
            }

            // Add the new/updated model line.
            var modelJson = CreateJsonString(model);
            newLinesForFile.Add(modelJson);

            // Clear and rewrite file
            if (!FileUtils.OSClearContentsOfAFile(osFile))
            {
                DebugOutput.Log("Failed to clear file before rewrite.");
                return false;
            }

            foreach (var nl in newLinesForFile)
            {
                if (!FileUtils.OSAppendLineToFile(osFile, nl))
                {
                    DebugOutput.Log($"Failed to write line during rewrite: {nl}");
                    return false;
                }
            }

            DebugOutput.Log("Self-heal file updated successfully.");
            return true;
        }

        // Helper: compare important fields for equality (case-insensitive for strings)
        private static bool AreModelsEqual(SelfHealModel a, SelfHealModel b)
        {
            if (a == null || b == null) return false;
            return string.Equals(a.PageName, b.PageName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.ElementName, b.ElementName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.ElementType, b.ElementType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.ElementXPathString ?? string.Empty, b.ElementXPathString ?? string.Empty, StringComparison.Ordinal)
                && string.Equals(a.ElementText ?? string.Empty, b.ElementText ?? string.Empty, StringComparison.Ordinal)
                && string.Equals(a.ElementTag ?? string.Empty, b.ElementTag ?? string.Empty, StringComparison.Ordinal)
                && a.ElementEnabled == b.ElementEnabled;
        }

        // Note: legacy/experimental methods and large commented blocks were removed to improve readability.
    }
}