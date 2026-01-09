using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CommunicationReporting
{
    public class TrxParser
    {
        public TestRunData ParseTrxFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"TRX file not found: {filePath}");

            try
            {
                XDocument doc = XDocument.Load(filePath);
                return ExtractTestRunData(doc);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing TRX file: {ex.Message}", ex);
            }
        }

        private TestRunData ExtractTestRunData(XDocument doc)
        {
            XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
            var rootElement = doc.Root;

            if (rootElement == null)
                throw new Exception("Invalid TRX file: root element not found");

            var testRun = new TestRunData
            {
                Id = rootElement.Attribute("id")?.Value ?? string.Empty,
                Name = rootElement.Attribute("name")?.Value ?? string.Empty,
                RunUser = rootElement.Attribute("runUser")?.Value ?? string.Empty
            };

            // Extract times
            var timesElement = rootElement.Element(ns + "Times");
            if (timesElement != null)
            {
                testRun.CreationTime = DateTime.Parse(timesElement.Attribute("creation")?.Value ?? DateTime.Now.ToString());
                testRun.StartTime = DateTime.Parse(timesElement.Attribute("start")?.Value ?? DateTime.Now.ToString());
                testRun.FinishTime = DateTime.Parse(timesElement.Attribute("finish")?.Value ?? DateTime.Now.ToString());
            }

            // Extract test results
            var resultsElement = rootElement.Element(ns + "Results");
            if (resultsElement != null)
            {
                foreach (var result in resultsElement.Elements(ns + "UnitTestResult"))
                {
                    var testResult = new UnitTestResult
                    {
                        ExecutionId = result.Attribute("executionId")?.Value ?? string.Empty,
                        TestId = result.Attribute("testId")?.Value ?? string.Empty,
                        TestName = result.Attribute("testName")?.Value ?? string.Empty,
                        ComputerName = result.Attribute("computerName")?.Value ?? string.Empty,
                        Duration = result.Attribute("duration")?.Value ?? "00:00:00",
                        StartTime = DateTime.Parse(result.Attribute("startTime")?.Value ?? DateTime.Now.ToString()),
                        EndTime = DateTime.Parse(result.Attribute("endTime")?.Value ?? DateTime.Now.ToString()),
                        Outcome = result.Attribute("outcome")?.Value ?? "Unknown"
                    };

                    // Extract standard output
                    var outputElement = result.Element(ns + "Output");
                    if (outputElement != null)
                    {
                        var stdOutElement = outputElement.Element(ns + "StdOut");
                        testResult.StdOut = stdOutElement?.Value ?? string.Empty;
                    }

                    testRun.Results.Add(testResult);
                }
            }

            // Extract test definitions
            var testDefinitionsElement = rootElement.Element(ns + "TestDefinitions");
            if (testDefinitionsElement != null)
            {
                foreach (var testDef in testDefinitionsElement.Elements(ns + "UnitTest"))
                {
                    var testDefinition = new TestDefinition
                    {
                        Id = testDef.Attribute("id")?.Value ?? string.Empty,
                        Name = testDef.Attribute("name")?.Value ?? string.Empty
                    };

                    testRun.TestDefinitions.Add(testDefinition);
                }
            }

            return testRun;
        }
    }

    public class TestRunData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RunUser { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public List<UnitTestResult> Results { get; set; } = new();
        public List<TestDefinition> TestDefinitions { get; set; } = new();

        public int TotalTests => Results.Count;
        public int PassedTests => Results.Count(r => r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase));
        public int FailedTests => Results.Count(r => r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase));
        public int SkippedTests => Results.Count(r => r.Outcome.Equals("NotExecuted", StringComparison.OrdinalIgnoreCase));
        public double PassPercentage => TotalTests > 0 ? (PassedTests / (double)TotalTests) * 100 : 0;
    }

    public class UnitTestResult
    {
        public string ExecutionId { get; set; } = string.Empty;
        public string TestId { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
        public string ComputerName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Outcome { get; set; } = string.Empty;
        public string StdOut { get; set; } = string.Empty;

        public string OutcomeClass
        {
            get
            {
                return Outcome.ToLower() switch
                {
                    "passed" => "outcome-passed",
                    "failed" => "outcome-failed",
                    "notexecuted" => "outcome-skipped",
                    _ => "outcome-unknown"
                };
            }
        }
    }

    public class TestDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
