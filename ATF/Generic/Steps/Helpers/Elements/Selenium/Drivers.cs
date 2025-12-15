using System.Runtime.InteropServices;
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
    public static class Drivers
    {
        
        private const string EnvironmentVariable = "ATFENVIRONMENT";

        public static ChromeVarData ChromeInstance { get; private set; } = new ChromeVarData();

        public class ChromeVarData
        {
            public string Languages { get; set; } = string.Empty;
            public bool SafeBrowsingEnabled { get; set; } = false;
            public bool ProfileDefaultContentSettingsPopups { get; set;} = false;
            public bool DisablePopupBlocking { get; set; } = false;
            public bool PromptForDownload { get; set; } = false;
            public string DownloadDefaultDirectory { get; set;} = string.Empty;
            public bool StartMaximized { get; set; } = false;
            public string[] StartArguments { get; set; } = { "" };
        }


        public static void ReadJson()
        {
            ReadJsonChromeInstance();
        }

        
        public static ChromeVarData? ReadJsonChromeInstance()
        {
            DebugOutput.Log($"Proc - ReadJson for BROWSER");
            var fileName = $"browser.chrome.{Environment}.json";
            var directory = ".\\AppTargets\\Resources\\Browsers\\";
            var fullFileName = directory + fileName;
            if (!FileUtils.FileCheck(fullFileName))
            {
                DebugOutput.Log($"Unable to find the file {fullFileName}");
                return null;
            }
            var jsonText = File.ReadAllText(fullFileName);
            DebugOutput.Log($"Json - {jsonText}");
            try
            {
                var obj = JsonConvert.DeserializeObject<ChromeVarData>(jsonText);
                if (obj == null) return null;
                ChromeInstance = obj;
                return ChromeInstance;
            }
            catch
            {
                DebugOutput.Log($"We out ere");
                return null;
            }
        }

        public static void ChromeDriver()
        {
            DebugOutput.Log($"proc - ChromeDriver");
            DebugOutput.Log($"READING CHOMRE {ChromeInstance.Languages}");
            
            // Automatically download and manage Chrome driver
            new DriverManager().SetUpDriver(new ChromeConfig());
            
            ChromeOptions options = new ChromeOptions();
            
            // Determine the download directory based on the operating system
            var repoDirectory = FileUtils.GetRepoDirectory();
            var downloadDirectory = Path.Combine(repoDirectory, "AppSpecFlow", "TestResults", "Downloads");
            if (!FileUtils.OSDirectoryCheck(downloadDirectory))
            {
                FileUtils.OSDirectoryCreation(downloadDirectory);
            }
            DebugOutput.Log($"Download Directory {downloadDirectory}");
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false); // Disable download prompts
            options.AddArgument("--no-first-run");
            options.AddArgument("--no-default-browser-check");
            options.AddArgument("--no-sandbox");
            // check the OS we running
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                options.AddArgument("--headless");                
            }
            if (Environment.ToLower() != "development")
            {
                // options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--remote-debugging-port=9222");
            }
            if (ChromeInstance.StartMaximized) options.AddArgument("start-maximized");
            // if (ChromeInstance.DownloadDefaultDirectory != String.Empty) options.AddUserProfilePreference("download.default_directory", ChromeInstance.DownloadDefaultDirectory);
            if (ChromeInstance.StartArguments.Length > 0 && ChromeInstance.StartArguments[0] != "")
            {
                foreach (var arg in ChromeInstance.StartArguments)
                {
                    DebugOutput.Log($"Adding starting options of {arg}");
                    options.AddArgument(arg);
                }
            }
            SeleniumUtil.webDriver = new ChromeDriver(options);
        }
        
        public static void EdgeDriver()
        {
            DebugOutput.Log($"proc - EdgeDriver");
            
            // Automatically download and manage Edge driver
            new DriverManager().SetUpDriver(new EdgeConfig());
            
            EdgeOptions options = new EdgeOptions();
            var repoDirectory = FileUtils.GetRepoDirectory();
            var downloadDirectory = Path.Combine(repoDirectory, "AppSpecFlow", "TestResults", "Downloads");
            if (!FileUtils.OSDirectoryCheck(downloadDirectory))
            {
                FileUtils.OSDirectoryCreation(downloadDirectory);
            }
            DebugOutput.Log($"Download Directory {downloadDirectory}");
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false); // Disable download prompts

            SeleniumUtil.webDriver = new EdgeDriver(options);
        }

        public static void FireFoxDriver()
        {
            DebugOutput.Log($"proc - FireFoxDriver");
            
            // Automatically download and manage Firefox driver
            new DriverManager().SetUpDriver(new FirefoxConfig());
            
            SeleniumUtil.webDriver = new FirefoxDriver();
        }

        public static void InternetExplorerDriver()
        {
            DebugOutput.Log($"proc - InternetExplorerDriver");
            
            // Automatically download and manage IE driver
            new DriverManager().SetUpDriver(new InternetExplorerConfig());
            
            SeleniumUtil.webDriver = new InternetExplorerDriver();
            Thread.Sleep(TargetConfiguration.Configuration.PositiveTimeout * 1000 * TargetConfiguration.Configuration.TimeoutMultiplie);
        }



        public static void CloseWebBrowser()
        {
            DebugOutput.Log($"proc - CloseWebBrowser");
            ElementInteraction.CloseWebBrowser();
        }

        
        /// <summary>
        ///     Set up an environment variable called "ENVIRONMENT", and set it to type of environment 
        ///     development, beta, live etc.
        ///     Then you need a targetSettings.ENVIRONMENT.json file in the Resources folder of the AppTargets project
        ///     Different environments need different configuration.  this is how that is controled.
        ///     If there is no ENVIRONMENT environment variable it will use development as default.
        /// </summary>
        private static string Environment =>
            System.Environment.GetEnvironmentVariable(EnvironmentVariable) ?? "development";
    }
}