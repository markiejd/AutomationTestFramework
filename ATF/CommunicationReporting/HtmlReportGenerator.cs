using System;
using System.Text;

namespace CommunicationReporting
{
    public class HtmlReportGenerator
    {
        public string LogoUrl { get; set; } = string.Empty;

        public string GenerateHtmlReport(TestRunData testRunData)
        {
            if (testRunData == null)
                throw new ArgumentNullException(nameof(testRunData));

            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>Test Report - " + HtmlEncode(testRunData.Name) + "</title>");
            html.AppendLine(GetStyleSheet());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            html.AppendLine(GetReportHeader(testRunData));
            html.AppendLine(GetSummarySection(testRunData));
            html.AppendLine(GetTestResultsTable(testRunData));
            html.AppendLine(GetDetailedResults(testRunData));
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        public void SaveHtmlReport(TestRunData testRunData, string outputPath, string logoUrl = "")
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be empty", nameof(outputPath));

            try
            {
                LogoUrl = logoUrl;
                string html = GenerateHtmlReport(testRunData);
                File.WriteAllText(outputPath, html);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving HTML report: {ex.Message}", ex);
            }
        }

        private string GetStyleSheet()
        {
            return """
                <style>
                    * {
                        margin: 0;
                        padding: 0;
                        box-sizing: border-box;
                    }
                    
                    body {
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        background-color: #f5f5f5;
                        color: #333;
                        line-height: 1.6;
                    }
                    
                    .container {
                        max-width: 1200px;
                        margin: 0 auto;
                        padding: 20px;
                    }
                    
                    .header {
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        padding: 30px;
                        border-radius: 8px;
                        margin-bottom: 30px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                        position: relative;
                    }
                    
                    .header-logo {
                        position: absolute;
                        top: 20px;
                        right: 20px;
                        max-width: 120px;
                        height: auto;
                        border-radius: 4px;
                    }
                    
                    .header h1 {
                        font-size: 2em;
                        margin-bottom: 10px;
                    }
                    
                    .header p {
                        opacity: 0.9;
                        font-size: 0.95em;
                    }
                    
                    .summary {
                        display: grid;
                        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                        gap: 20px;
                        margin-bottom: 30px;
                    }
                    
                    .summary-card {
                        background: white;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        text-align: center;
                    }
                    
                    .summary-card h3 {
                        color: #667eea;
                        font-size: 0.85em;
                        text-transform: uppercase;
                        margin-bottom: 10px;
                        letter-spacing: 1px;
                    }
                    
                    .summary-card .value {
                        font-size: 2.5em;
                        font-weight: bold;
                        color: #333;
                    }
                    
                    .summary-card.passed .value {
                        color: #27ae60;
                    }
                    
                    .summary-card.failed .value {
                        color: #e74c3c;
                    }
                    
                    .summary-card.skipped .value {
                        color: #f39c12;
                    }
                    
                    .summary-card.percentage .value {
                        color: #667eea;
                    }
                    
                    .section {
                        background: white;
                        padding: 25px;
                        border-radius: 8px;
                        margin-bottom: 30px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                    }
                    
                    .section h2 {
                        color: #667eea;
                        font-size: 1.5em;
                        margin-bottom: 20px;
                        padding-bottom: 10px;
                        border-bottom: 2px solid #667eea;
                    }
                    
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 10px;
                    }
                    
                    th {
                        background-color: #667eea;
                        color: white;
                        padding: 15px;
                        text-align: left;
                        font-weight: 600;
                        font-size: 0.95em;
                    }
                    
                    td {
                        padding: 12px 15px;
                        border-bottom: 1px solid #e0e0e0;
                    }
                    
                    tr:hover {
                        background-color: #f9f9f9;
                    }
                    
                    .outcome-passed {
                        background-color: #d4edda;
                        color: #155724;
                        padding: 6px 12px;
                        border-radius: 4px;
                        font-weight: 600;
                        font-size: 0.85em;
                        display: inline-block;
                    }
                    
                    .outcome-failed {
                        background-color: #f8d7da;
                        color: #721c24;
                        padding: 6px 12px;
                        border-radius: 4px;
                        font-weight: 600;
                        font-size: 0.85em;
                        display: inline-block;
                    }
                    
                    .outcome-skipped {
                        background-color: #fff3cd;
                        color: #856404;
                        padding: 6px 12px;
                        border-radius: 4px;
                        font-weight: 600;
                        font-size: 0.85em;
                        display: inline-block;
                    }
                    
                    .outcome-unknown {
                        background-color: #e2e3e5;
                        color: #383d41;
                        padding: 6px 12px;
                        border-radius: 4px;
                        font-weight: 600;
                        font-size: 0.85em;
                        display: inline-block;
                    }
                    
                    .test-detail {
                        background: #f9f9f9;
                        padding: 15px;
                        margin: 15px 0;
                        border-left: 4px solid #667eea;
                        border-radius: 4px;
                    }
                    
                    .test-detail h4 {
                        color: #333;
                        margin-bottom: 10px;
                    }
                    
                    .test-detail p {
                        margin: 5px 0;
                        font-size: 0.9em;
                    }
                    
                    .test-detail pre {
                        background: white;
                        padding: 10px;
                        border-radius: 4px;
                        overflow-x: auto;
                        font-size: 0.85em;
                        margin-top: 10px;
                        max-height: 300px;
                        overflow-y: auto;
                    }
                    
                    .footer {
                        text-align: center;
                        color: #999;
                        font-size: 0.9em;
                        margin-top: 30px;
                        padding-top: 20px;
                        border-top: 1px solid #e0e0e0;
                    }
                    
                    .duration {
                        color: #667eea;
                        font-weight: 600;
                    }
                    
                    @media (max-width: 768px) {
                        .header h1 {
                            font-size: 1.5em;
                        }
                        
                        table {
                            font-size: 0.9em;
                        }
                        
                        th, td {
                            padding: 10px;
                        }
                        
                        .summary {
                            grid-template-columns: 1fr;
                        }
                    }
                </style>
                """;
        }

        private string GetReportHeader(TestRunData testRunData)
        {
            string logoHtml = string.IsNullOrWhiteSpace(LogoUrl) 
                ? string.Empty 
                : $"                        <img src=\"{HtmlEncode(LogoUrl)}\" alt=\"Logo\" class=\"header-logo\" />";
            
            return $"""
                <div class="container">
                    <div class="header">
                        {logoHtml}
                        <h1>Test Execution Report</h1>
                        <p><strong>Run Name:</strong> {HtmlEncode(testRunData.Name)}</p>
                        <p><strong>Run User:</strong> {HtmlEncode(testRunData.RunUser)}</p>
                        <p><strong>Run ID:</strong> {HtmlEncode(testRunData.Id)}</p>
                        <p><strong>Start Time:</strong> {testRunData.StartTime:yyyy-MM-dd HH:mm:ss}</p>
                        <p><strong>Finish Time:</strong> {testRunData.FinishTime:yyyy-MM-dd HH:mm:ss}</p>
                    </div>
                """;
        }

        private string GetSummarySection(TestRunData testRunData)
        {
            var passed = testRunData.PassedTests;
            var failed = testRunData.FailedTests;
            var skipped = testRunData.SkippedTests;
            var total = testRunData.TotalTests;
            var percentage = testRunData.PassPercentage;

            return $"""
                    <div class="summary">
                        <div class="summary-card passed">
                            <h3>Passed</h3>
                            <div class="value">{passed}</div>
                        </div>
                        <div class="summary-card failed">
                            <h3>Failed</h3>
                            <div class="value">{failed}</div>
                        </div>
                        <div class="summary-card skipped">
                            <h3>Skipped</h3>
                            <div class="value">{skipped}</div>
                        </div>
                        <div class="summary-card percentage">
                            <h3>Pass Rate</h3>
                            <div class="value">{percentage:F1}%</div>
                        </div>
                        <div class="summary-card">
                            <h3>Total Tests</h3>
                            <div class="value">{total}</div>
                        </div>
                    </div>
                """;
        }

        private string GetTestResultsTable(TestRunData testRunData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("    <div class=\"section\">");
            sb.AppendLine("        <h2>Test Results Summary</h2>");
            sb.AppendLine("        <table>");
            sb.AppendLine("            <thead>");
            sb.AppendLine("                <tr>");
            sb.AppendLine("                    <th>Test Name</th>");
            sb.AppendLine("                    <th>Status</th>");
            sb.AppendLine("                    <th>Duration</th>");
            sb.AppendLine("                    <th>Computer</th>");
            sb.AppendLine("                </tr>");
            sb.AppendLine("            </thead>");
            sb.AppendLine("            <tbody>");

            foreach (var result in testRunData.Results)
            {
                sb.AppendLine("                <tr>");
                sb.AppendLine($"                    <td>{HtmlEncode(result.TestName)}</td>");
                sb.AppendLine($"                    <td><span class=\"{result.OutcomeClass}\">{HtmlEncode(result.Outcome)}</span></td>");
                sb.AppendLine($"                    <td class=\"duration\">{HtmlEncode(result.Duration)}</td>");
                sb.AppendLine($"                    <td>{HtmlEncode(result.ComputerName)}</td>");
                sb.AppendLine("                </tr>");
            }

            sb.AppendLine("            </tbody>");
            sb.AppendLine("        </table>");
            sb.AppendLine("    </div>");

            return sb.ToString();
        }

        private string GetDetailedResults(TestRunData testRunData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("    <div class=\"section\">");
            sb.AppendLine("        <h2>Detailed Test Results</h2>");

            foreach (var result in testRunData.Results)
            {
                sb.AppendLine("        <div class=\"test-detail\">");
                sb.AppendLine($"            <h4>{HtmlEncode(result.TestName)}</h4>");
                sb.AppendLine($"            <p><strong>Status:</strong> <span class=\"{result.OutcomeClass}\">{HtmlEncode(result.Outcome)}</span></p>");
                sb.AppendLine($"            <p><strong>Duration:</strong> <span class=\"duration\">{HtmlEncode(result.Duration)}</span></p>");
                sb.AppendLine($"            <p><strong>Start Time:</strong> {result.StartTime:yyyy-MM-dd HH:mm:ss}</p>");
                sb.AppendLine($"            <p><strong>End Time:</strong> {result.EndTime:yyyy-MM-dd HH:mm:ss}</p>");
                sb.AppendLine($"            <p><strong>Computer:</strong> {HtmlEncode(result.ComputerName)}</p>");

                if (!string.IsNullOrWhiteSpace(result.StdOut))
                {
                    sb.AppendLine("            <p><strong>Output:</strong></p>");
                    sb.AppendLine($"            <pre>{HtmlEncode(result.StdOut)}</pre>");
                }

                sb.AppendLine("        </div>");
            }

            sb.AppendLine("    </div>");

            return sb.ToString();
        }

        private string HtmlEncode(string text)
        {
            return System.Net.WebUtility.HtmlEncode(text);
        }
    }
}
