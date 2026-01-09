# CommunicationReporting

A .NET Console application that converts TRX (Test Results) files into beautifully, if I say so myself, formatted HTML reports.

## Overview

CommunicationReporting is designed to parse Visual Studio TRX test result files and generate comprehensive, easy-to-read HTML reports. The reports include:

- **Test Summary**: Overview of test execution with statistics
- **Visual Metrics**: Quick cards showing Pass/Fail/Skip counts and pass percentage
- **Detailed Results Table**: Summary of all test results with status and duration
- **Detailed Output**: Full test information including standard output for each test
- **Professional Styling**: Responsive, modern HTML design with color-coded test outcomes

## Project Structure

```
CommunicationReporting/
├── Program.cs                   # Entry point and command-line interface
├── TrxParser.cs                 # Parses TRX files and extracts test data
├── HtmlReportGenerator.cs       # Generates styled HTML reports
└── CommunicationReporting.csproj # Project configuration
```

## Usage

### Basic Usage

```bash
dotnet run --project CommunicationReporting/CommunicationReporting.csproj -- "<path-to-trx-file>"
```

The HTML report will be saved in the same directory as the TRX file with the suffix `_report.html`.

### With Custom Output Path

```bash
dotnet run --project CommunicationReporting/CommunicationReporting.csproj -- "<path-to-trx-file>" "<output-html-path>"
```

### Example

```bash
dotnet run --project CommunicationReporting/CommunicationReporting.csproj -- "AppSpecFlow/TestResults/TEST1.trx" "AppSpecFlow/TestResults/TEST1_report.html"
```

## Command-Line Arguments

| Argument | Required | Description |
|----------|----------|-------------|
| `<trxFilePath>` | Yes | Path to the input TRX test results file |
| `[outputHtmlPath]` | No | Path where the HTML report will be saved. If not provided, defaults to `{trx-filename}_report.html` in the same directory as the TRX file |

## Building

```bash
dotnet build CommunicationReporting/CommunicationReporting.csproj
```

## Output

The generated HTML report includes:

1. **Header Section**
   - Test run name
   - Run user information
   - Test execution timestamps

2. **Summary Cards**
   - Total number of tests
   - Number of passed tests
   - Number of failed tests
   - Number of skipped tests
   - Pass percentage

3. **Results Table**
   - Test name
   - Outcome status (color-coded)
   - Execution duration
   - Computer name

4. **Detailed Results**
   - Individual test information
   - Standard output/logs for each test
   - Execution times

## Test Outcomes

Test results are color-coded for easy identification:

- **Green**: Passed tests
- **Red**: Failed tests
- **Orange**: Skipped tests
- **Gray**: Unknown status

## Dependencies

- .NET 9.0 or higher
- Newtonsoft.Json (NuGet package)

## Example TRX File

The application expects standard Visual Studio TRX files, which are XML files containing test execution results. Example test result files can be found in `AppSpecFlow/TestResults/`.

## Features

✅ Parse standard TRX test result files
✅ Extract comprehensive test statistics
✅ Generate professional HTML reports
✅ Color-coded test outcome visualization
✅ Full test output/logging capture
✅ Responsive design for all screen sizes
✅ Clear command-line feedback and error messages

## Notes

- The TRX file path must exist and be a valid TRX XML file
- The output directory must be writable
- Large test files with extensive output may produce larger HTML files
