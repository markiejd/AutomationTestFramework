using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Core;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Generic.Steps.Helpers.Classes
{
    /// <summary>
    /// Static helper for managing WebDriver instances and browser-related settings.
    /// Reads browser configuration from JSON, prepares driver options and creates drivers.
    /// </summary>
    public static class Drivers
    {
        private const string EnvironmentVariable = "ATFENVIRONMENT";

        /// <summary>
        /// Holds parsed Chrome configuration values read from JSON.
        /// </summary>
        public static ChromeVarData ChromeInstance { get; private set; } = new ChromeVarData();

        /// <summary>
        /// Strongly-typed structure for Chrome options coming from browser.chrome.{env}.json
        /// </summary>
        public class ChromeVarData
        {
            public string Languages { get; set; } = string.Empty;
            public bool SafeBrowsingEnabled { get; set; } = false;
            public bool ProfileDefaultContentSettingsPopups { get; set; } = false;
            public bool DisablePopupBlocking { get; set; } = false;
            public bool PromptForDownload { get; set; } = false;
            public string DownloadDefaultDirectory { get; set; } = string.Empty;
            public bool StartMaximized { get; set; } = false;
            public string[] StartArguments { get; set; } = { string.Empty };
        }

        /// <summary>
        /// Read all browser-related JSON files (currently only Chrome) and populate instances.
        /// </summary>
        public static void ReadJson()
        {
            ReadJsonChromeInstance();
        }

        /// <summary>
        /// Read the chrome configuration file for the current environment and deserialize it.
        /// Uses the repository directory to locate AppTargets/Resources/Browsers.
        /// </summary>
        /// <returns>The populated ChromeVarData or null if reading/parsing failed.</returns>
        public static ChromeVarData? ReadJsonChromeInstance()
        {
            DebugOutput.Log("Proc - ReadJson for BROWSER");

            try
            {
                var repoDirectory = FileUtils.GetRepoDirectory();
                var directory = Path.Combine(repoDirectory, "AppTargets", "Resources", "Browsers");
                var fileName = $"browser.chrome.{Environment}.json";
                var fullFileName = Path.Combine(directory, fileName);

                if (!FileUtils.FileCheck(fullFileName))
                {
                    DebugOutput.Log($"Unable to find the file {fullFileName}");
                    return null;
                }

                var jsonText = File.ReadAllText(fullFileName);
                DebugOutput.Log($"Json - {jsonText}");

                var obj = JsonConvert.DeserializeObject<ChromeVarData>(jsonText);
                if (obj == null)
                {
                    DebugOutput.Log("Json deserialized to null for ChromeVarData");
                    return null;
                }

                ChromeInstance = obj;
                return ChromeInstance;
            }
            catch (Exception ex)
            {
                // Log the exception message for easier troubleshooting
                DebugOutput.Log($"Failed reading Chrome JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Initializes a ChromeDriver with sensible defaults for downloads, headless based on OS/env,
        /// and additional startup arguments from the ChromeInstance configuration.
        /// </summary>
        public static void ChromeDriver()
        {
            DebugOutput.Log("proc - ChromeDriver");

            // Ensure we have configuration loaded; attempt to read if missing
            if (ChromeInstance == null || string.IsNullOrEmpty(ChromeInstance.Languages) &&
                (ChromeInstance.StartArguments == null || ChromeInstance.StartArguments.Length == 1 && ChromeInstance.StartArguments[0] == string.Empty))
            {
                ReadJsonChromeInstance();
            }

            DebugOutput.Log($"READING CHROME LANG: {ChromeInstance?.Languages}");

            // Automatically download and manage Chrome driver binary
            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions();

            // Use repository directory for a consistent download folder
            var repoDir = FileUtils.GetRepoDirectory();
            var downloadDirectory = Path.Combine(repoDir, "AppSpecFlow", "TestResults", "Downloads");

            if (!FileUtils.OSDirectoryCheck(downloadDirectory))
            {
                FileUtils.OSDirectoryCreation(downloadDirectory);
            }

            DebugOutput.Log($"Download Directory {downloadDirectory}");

            // Ensure Chrome options for downloads are set (override with configured value if present)
            var configuredDownloadDir = !string.IsNullOrEmpty(ChromeInstance?.DownloadDefaultDirectory)
                ? ChromeInstance.DownloadDefaultDirectory
                : downloadDirectory;

            options.AddUserProfilePreference("download.default_directory", configuredDownloadDir);
            options.AddUserProfilePreference("download.prompt_for_download", ChromeInstance?.PromptForDownload ?? false);

            // Common command-line arguments
            options.AddArgument("--no-first-run");
            options.AddArgument("--no-default-browser-check");
            options.AddArgument("--no-sandbox");

            // If not running on Windows, default to headless to support CI environments
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                options.AddArgument("--headless");
            }

            // For non-development environments, add stability flags useful in containers/CI
            if (!Environment.Equals("development", StringComparison.OrdinalIgnoreCase))
            {
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--remote-debugging-port=9222");
            }

            if (ChromeInstance != null && ChromeInstance.StartMaximized)
            {
                options.AddArgument("start-maximized");
            }

            if (ChromeInstance?.StartArguments != null && ChromeInstance.StartArguments.Length > 0 && ChromeInstance.StartArguments[0] != string.Empty)
            {
                foreach (var arg in ChromeInstance.StartArguments)
                {
                    DebugOutput.Log($"Adding starting option: {arg}");
                    options.AddArgument(arg);
                }
            }

            SeleniumUtil.webDriver = new ChromeDriver(options);
        }

        /// <summary>
        /// Initializes an EdgeDriver and configures the download directory.
        /// </summary>
        public static void EdgeDriver()
        {
            DebugOutput.Log("proc - EdgeDriver");

            // Automatically download and manage Edge driver binary
            new DriverManager().SetUpDriver(new EdgeConfig());

            var options = new EdgeOptions();
            var repoDir = FileUtils.GetRepoDirectory();
            var downloadDirectory = Path.Combine(repoDir, "AppSpecFlow", "TestResults", "Downloads");

            if (!FileUtils.OSDirectoryCheck(downloadDirectory))
            {
                FileUtils.OSDirectoryCreation(downloadDirectory);
            }

            DebugOutput.Log($"Download Directory {downloadDirectory}");
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false);

            SeleniumUtil.webDriver = new EdgeDriver(options);
        }

        /// <summary>
        /// Initializes a FirefoxDriver. The driver binary is auto-managed.
        /// </summary>
        public static void FireFoxDriver()
        {
            DebugOutput.Log("proc - FireFoxDriver");

            // Automatically download and manage Firefox driver binary
            new DriverManager().SetUpDriver(new FirefoxConfig());

            SeleniumUtil.webDriver = new FirefoxDriver();
        }

        /// <summary>
        /// Initializes an InternetExplorerDriver. Waits briefly after startup to allow the browser to stabilize.
        /// </summary>
        public static void InternetExplorerDriver()
        {
            DebugOutput.Log("proc - InternetExplorerDriver");

            // Automatically download and manage IE driver binary
            new DriverManager().SetUpDriver(new InternetExplorerConfig());

            SeleniumUtil.webDriver = new InternetExplorerDriver();
            Thread.Sleep(TargetConfiguration.Configuration.PositiveTimeout * 1000 * TargetConfiguration.Configuration.TimeoutMultiplie);
        }

        /// <summary>
        /// Close the currently active browser. Delegates to ElementInteraction helper.
        /// </summary>
        public static void CloseWebBrowser()
        {
            DebugOutput.Log("proc - CloseWebBrowser");
            ElementInteraction.CloseWebBrowser();
        }

        /// <summary>
        /// Get the configured environment name from the ATFENVIRONMENT environment variable.
        /// Falls back to "development" when not set.
        /// </summary>
        private static string Environment =>
            System.Environment.GetEnvironmentVariable(EnvironmentVariable) ?? "development";
    }
}