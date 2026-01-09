using System;
using System.IO;
using CommunicationReporting;

class Program
{
    // Usage: CommunicationReporting.exe <trxFilePath> [outputHtmlPath]
    // Example: CommunicationReporting.exe "C:\path\to\TEST1.trx" "C:\output\report.html"
    // If outputHtmlPath is not provided, it will be generated in the same directory as the TRX file
    static void Main(string[] args)
    {
        try
        {
            // Validate arguments
            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            string trxFilePath = args[0];
            string outputHtmlPath = args.Length > 1 ? args[1] : GenerateOutputPath(trxFilePath);

            Console.WriteLine($"Processing TRX file: {trxFilePath}");
            Console.WriteLine($"Output HTML path: {outputHtmlPath}");
            Console.WriteLine();

            // Parse the TRX file
            Console.WriteLine("Parsing TRX file...");
            var parser = new TrxParser();
            TestRunData testRunData = parser.ParseTrxFile(trxFilePath);

            Console.WriteLine($"✓ Successfully parsed TRX file");
            Console.WriteLine($"  - Total Tests: {testRunData.TotalTests}");
            Console.WriteLine($"  - Passed: {testRunData.PassedTests}");
            Console.WriteLine($"  - Failed: {testRunData.FailedTests}");
            Console.WriteLine($"  - Skipped: {testRunData.SkippedTests}");
            Console.WriteLine($"  - Pass Rate: {testRunData.PassPercentage:F2}%");
            Console.WriteLine();

            // Load settings and get logo URL
            Console.WriteLine("Loading settings...");
            var settings = SettingsLoader.LoadSettings();
            if (!string.IsNullOrWhiteSpace(settings.LogoUrl))
            {
                Console.WriteLine($"  - Logo URL loaded: {settings.LogoUrl}");
            }
            else
            {
                Console.WriteLine("  - No logo URL configured in settings");
            }
            Console.WriteLine();

            // Generate HTML report
            Console.WriteLine("Generating HTML report...");
            var htmlGenerator = new HtmlReportGenerator();
            htmlGenerator.SaveHtmlReport(testRunData, outputHtmlPath, settings.LogoUrl);

            Console.WriteLine($"✓ HTML report successfully generated");
            Console.WriteLine($"  - Output file: {outputHtmlPath}");
            if (!string.IsNullOrWhiteSpace(settings.LogoUrl))
            {
                Console.WriteLine($"  - Logo URL: {settings.LogoUrl}");
            }
            Console.WriteLine();
            Console.WriteLine("Report generation completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Details: {ex.InnerException.Message}");
            }
            Environment.Exit(1);
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("CommunicationReporting - TRX to HTML Report Converter");
        Console.WriteLine("======================================================");
        Console.WriteLine();
        Console.WriteLine("Usage: CommunicationReporting.exe <trxFilePath> [outputHtmlPath]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  <trxFilePath>      - Path to the input TRX (Test Results) file");
        Console.WriteLine("  [outputHtmlPath]   - (Optional) Path where the HTML report will be saved");
        Console.WriteLine("                       If not provided, the report will be saved in the same");
        Console.WriteLine("                       directory as the TRX file with '_report.html' suffix");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  CommunicationReporting.exe \"C:\\TestResults\\TEST1.trx\"");
        Console.WriteLine("  CommunicationReporting.exe \"C:\\TestResults\\TEST1.trx\" \"C:\\Reports\\my_report.html\"");
        Console.WriteLine();
    }

    static string GenerateOutputPath(string trxFilePath)
    {
        string directory = Path.GetDirectoryName(trxFilePath) ?? ".";
        string filename = Path.GetFileNameWithoutExtension(trxFilePath);
        return Path.Combine(directory, $"{filename}_report.html");
    }
}
