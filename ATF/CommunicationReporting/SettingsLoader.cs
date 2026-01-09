using System;
using System.IO;
using Newtonsoft.Json;

namespace CommunicationReporting
{
    public class AppSettings
    {
        [JsonProperty("logoUrl")]
        public string LogoUrl { get; set; } = string.Empty;
    }

    public class SettingsLoader
    {
        private static readonly string DefaultSettingsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "settings.json"
        );

        public static AppSettings LoadSettings(string? settingsPath = null)
        {
            string pathToUse = settingsPath ?? DefaultSettingsPath;

            if (!File.Exists(pathToUse))
            {
                Console.WriteLine($"Warning: settings.json not found at {pathToUse}");
                Console.WriteLine("Using default settings with empty logo URL.");
                return new AppSettings();
            }

            try
            {
                string json = File.ReadAllText(pathToUse);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                return settings ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load settings from {pathToUse}: {ex.Message}");
                return new AppSettings();
            }
        }
    }
}
