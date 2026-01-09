# Automation Test Framework (ATF)

A comprehensive .NET-based test automation framework for testing web applications, APIs, databases, and cloud services. ATF is a production-ready test automation solution featuring BDD (Behavior-Driven Development) with SpecFlow/Reqnroll, multiple testing capabilities, and integrated reporting.

---

## 🎯 Overview

The Automation Test Framework provides an enterprise-grade solution for automated testing with:

- **Multi-Layer Testing**: Web UI, API, Database, and Azure cloud service testing
- **BDD Framework**: Reqnroll (successor to SpecFlow) for behavior-driven test scenarios
- **Browser Automation**: Selenium WebDriver with automated driver management
- **API Testing**: GraphQL and REST API support with comprehensive request/response handling
- **Database Testing**: SQL Server integration for data validation and manipulation
- **Azure Integration**: Azure Data Factory (ADF) and Azure Storage Blob support
- **Reporting**: Automated HTML report generation from TRX test results
- **Load Testing**: Locust-based performance testing capabilities

---

## 📋 Table of Contents

- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Project Structure](#project-structure)
- [Key Features](#key-features)
- [Quick Start](#quick-start)
- [Running Tests](#running-tests)
- [Configuration](#configuration)
- [Test Categories](#test-categories)
- [Reporting](#reporting)
- [Supporting Components](#supporting-components)
- [Development](#development)
- [Troubleshooting](#troubleshooting)
- [License](#license)

---

## 🔧 System Requirements

### Prerequisites

- **Operating System**: Windows (primarily) or compatible OS with .NET support
- **.NET SDK**: 9.x or higher ([Download here](https://dotnet.microsoft.com/download))
- **SQL Server**: For database testing (connection details configured via environment variables)
- **Visual Studio Code** or **Visual Studio 2022**: Recommended IDE
- **Chrome, Firefox and Edge Browsers**: For web UI testing (with WebDriver support)  

### Optional Components

- **Azure CLI**: For Azure service testing
- **Python 3.x**: For load testing with Locust
- **Git**: For version control

---

## 📦 Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd AutomationTestFramework\ATF
```

### 2. Install .NET 9 SDK

Download and install from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/9.0)

Verify installation:

```bash
dotnet --version
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build Solution

```bash
dotnet build
```

### 5. Set Environment Variables

**Windows Command Prompt:**

```cmd
SET SQLSERVER="<sql-server-name>"
SET SQLDATABASE="<database-name>"
SET SQLUSER="<sql-username>"
SET SQLSERVER_PASSWORD="<sql-password>"
SET ATFENVIRONMENT="<environment-name>"
SET AZURE_STORAGE_ACCOUNT_NAME="<storage-account>"
SET AZURE_STORAGE_ACCOUNT_KEY="<storage-key>"
SET AZURE_STORAGE_FILESYSTEM="<filesystem-name>"
SET ENCRYPTION_KEY="<encryption-key>"
SET ENCRYPTION_IV="<encryption-iv>"
```

**Note**: These variables are cleared from memory when your terminal session closes.

---

## 📂 Project Structure

```
ATF/
├── Core/                              # Core utilities and frameworks
│   ├── APIUtil.cs                     # REST/GraphQL API utilities
│   ├── ADFUtils.cs                    # Azure Data Factory utilities
│   ├── SeleniumUtil.cs                # Selenium WebDriver wrapper
│   ├── PlaywrightUtils.cs             # Playwright browser automation
│   ├── GraphQLUtil.cs                 # GraphQL query utilities
│   ├── Accessibility/                 # Accessibility testing utilities
│   ├── Configuration/                 # Configuration management
│   ├── Encryption/                    # Encryption/decryption utilities
│   ├── FileIO/                        # File operation utilities
│   ├── Logging/                       # Logging framework
│   ├── Jira/                          # Jira integration
│   └── Transformations/               # Data transformation utilities
│
├── AppSpecFlow/                       # Main test project (Reqnroll/SpecFlow)
│   ├── Features/                      # Gherkin feature files
│   ├── AppSteps/                      # Step definitions
│   └── TestResults/                   # Test execution results (TRX files)
│
├── AppTargets/                        # Target application definitions
│   ├── Forms/                         # UI element definitions
│   └── Resources/                     # Application resources
│
├── AppXAPI/                           # API application layer
│   ├── APIApps/                       # API endpoint definitions
│   ├── Models/                        # Data models
│   └── Variables.cs                   # Shared variables
│
├── Core/                              # Core test framework
│
├── CoreUnitTests/                     # Unit tests for Core project
│   ├── APIUtilTests.cs
│   ├── ADFUtilsTests.cs
│   ├── GraphQLUtilTests.cs
│   ├── EncryptionHelperTests.cs
│   └── ...
│
├── Generic/                           # Generic/shared functionality
│
├── CommunicationADF/                  # Azure Data Factory communication
│
├── CommunicationExcel/                # Excel file communication
│
├── CommunicationLoad/                 # Load testing (Locust)
│   ├── app.py
│   ├── locustfile.py
│   └── requirements.txt
│
├── CommunicationReporting/            # TRX to HTML report conversion
│   ├── HtmlReportGenerator.cs
│   ├── TrxParser.cs
│   └── Program.cs
│
├── CommunicationSqlServer/            # SQL Server utilities
│
├── CommunicationXrayCloud/            # Xray Cloud integration for test reporting
│
├── Chrome/                            # Chrome WebDriver versions
│   ├── 143.0.7499.169/
│   └── 143.0.7499.192/
│
├── TestAutomationSpecFlow.sln         # Solution file
├── Prep.bat                           # Preparation batch script
├── RunUnitTests.bat                   # Unit test execution script
├── Help.bat                           # Help documentation
└── README.md                          # Original project documentation
```

---

## ✨ Key Features

### 1. **Multi-Protocol Testing**
- **Web UI Testing**: Selenium WebDriver with element identification and interaction
- **API Testing**: REST and GraphQL with request/response validation
- **Database Testing**: SQL Server with query execution and data verification
- **Cloud Services**: Azure Storage, Azure Data Factory integration

### 2. **BDD Framework**
- **Reqnroll/SpecFlow**: Write tests in Gherkin (human-readable) syntax
- **Feature Files**: Organized test scenarios in `.feature` files
- **Step Definitions**: Reusable test steps with C# implementation

### 3. **Browser Automation**
- **Selenium WebDriver**: Industry-standard browser automation
- **WebDriver Manager**: Automatic driver version management
- **Multiple Browsers**: Chrome, Firefox, Edge support
- **Element Locators**: XPath, CSS selectors, ID-based element location

### 4. **API Integration**
- **REST Endpoints**: GET, POST, PUT, DELETE operations
- **GraphQL Queries**: Complex query support with token management
- **Request/Response**: JSON and XML handling
- **Error Handling**: Comprehensive HTTP error scenarios

### 5. **Advanced Testing Capabilities**
- **Accessibility Testing**: WCAG compliance checks
- **Data Encryption**: Built-in string encryption/decryption
- **File Operations**: CSV, Excel, JSON file handling
- **Image Comparison**: Screenshot and visual regression testing
- **Async Operations**: Support for asynchronous test scenarios

### 6. **Test Categorization**
- **Smoke Tests**: Quick sanity checks for database and UI
- **Regression Tests**: Complete feature validation
- **Database Tests**: Data integrity checks
- **API Tests**: Endpoint validation

### 7. **Comprehensive Reporting**
- **TRX to HTML**: Convert test results to professional HTML reports
- **Test Metrics**: Pass/fail statistics and execution times
- **Detailed Logs**: Comprehensive test output and error details
- **Xray Integration**: Test reporting to Atlassian Xray Cloud

---

## 🚀 Quick Start

### 1. Set Up Your Environment

```bash
# Navigate to ATF directory
cd ATF

# Restore NuGet packages
dotnet restore

# Build solution
dotnet build
```

### 2. Configure Database Connection

```cmd
REM Set SQL Server environment variables
SET SQLSERVER="your-server-name"
SET SQLDATABASE="your-database-name"
SET SQLUSER="sa"
SET SQLSERVER_PASSWORD="your-password"
SET ATFENVIRONMENT="Dev"
```

### 3. Run Smoke Tests

```bash
# Database smoke tests
dotnet test --filter:"TestCategory=Smoke-DB" --logger "trx;logfilename=Smoke-DB.trx"

# UI smoke tests
dotnet test --filter:"TestCategory=Smoke-UI" --logger "trx;logfilename=Smoke-UI.trx"
```

### 4. Run Full Regression Suite

```bash
dotnet test --filter:"TestCategory=Green" --logger "trx;logfilename=Green.trx"
```

---

## 🧪 Running Tests

### Build Solution

```bash
dotnet build
```

### Run All Tests

```bash
dotnet test
```

### Run Tests by Category

```bash
# Smoke DB tests
dotnet test --filter:"TestCategory=Smoke-DB"

# Smoke UI tests
dotnet test --filter:"TestCategory=Smoke-UI"

# Full regression (all passing tests)
dotnet test --filter:"TestCategory=Green"

# Database population
dotnet test --filter:"TestCategory=PopulateDefaultDB"

# Clear test data
dotnet test --filter:"TestCategory=ClearADF"
```

### Run Tests with Results Output

```bash
dotnet test --no-build --filter:"TestCategory=Smoke-UI" --logger "trx;logfilename=Smoke-UI.trx"
```

### Run Specific Test Class

```bash
dotnet test --filter:"ClassName=AppSpecFlow.AppSteps.Steps"
```

### Run Unit Tests Only

```bash
dotnet test CoreUnitTests/CoreUnitTests.csproj
```

### Run Tests in Watch Mode

```bash
dotnet watch test
```

---

## ⚙️ Configuration

### Environment Variables

| Variable | Purpose | Example |
|----------|---------|---------|
| `SQLSERVER` | SQL Server hostname | `localhost` |
| `SQLDATABASE` | Database name | `TestDB` |
| `SQLUSER` | SQL login user | `sa` |
| `SQLSERVER_PASSWORD` | SQL password | `YourPassword123` |
| `ATFENVIRONMENT` | Test environment | `Dev`, `Staging`, `Prod` |
| `AZURE_STORAGE_ACCOUNT_NAME` | Azure Storage account | `mystorageaccount` |
| `AZURE_STORAGE_ACCOUNT_KEY` | Storage access key | `<base64-encoded-key>` |
| `AZURE_STORAGE_FILESYSTEM` | Storage container | `test-data` |
| `ENCRYPTION_KEY` | Encryption key (base64) | `your-base64-key` |
| `ENCRYPTION_IV` | Initialization vector | `your-base64-iv` |

### Configuration Files

- **reqnroll.json**: Reqnroll/SpecFlow configuration
- **appsettings.json**: Application settings (where applicable)
- **settings.json**: Reporting settings

---

## 📊 Test Categories

### Smoke Database Tests (`Smoke-DB`)
Quick sanity checks to verify database structure and data integrity.

```bash
dotnet test --filter:"TestCategory=Smoke-DB" --logger "trx;logfilename=Smoke-DB.trx"
```

### Smoke UI Tests (`Smoke-UI`)
Basic UI functionality verification without data modifications.

```bash
dotnet test --filter:"TestCategory=Smoke-UI" --logger "trx;logfilename=Smoke-UI.trx"
```

### Full Regression Tests (`Green`)
Comprehensive test suite covering all expected functionality.

```bash
dotnet test --filter:"TestCategory=Green" --logger "trx;logfilename=Green.trx"
```

### Database Population (`PopulateDefaultDB`)
Populate database with test data.

```bash
dotnet test --filter:"TestCategory=PopulateDefaultDB" --logger "trx;logfilename=PopulateDefaultDB.trx"
```

### Clear Test Data (`ClearADF`)
Clean up test data from Azure Data Factory or database.

```bash
dotnet test --filter:"TestCategory=ClearADF" --logger "trx;logfilename=ClearADF.trx"
```

---

## 📈 Reporting

### Generate HTML Reports from TRX Files

The `CommunicationReporting` project converts TRX (Visual Studio Test Results) files into professional HTML reports.

#### Using CommunicationReporting

```bash
# Basic usage (outputs to same directory as TRX)
dotnet run --project CommunicationReporting/CommunicationReporting.csproj -- "AppSpecFlow/TestResults/Smoke-UI.trx"

# With custom output path
dotnet run --project CommunicationReporting/CommunicationReporting.csproj -- "AppSpecFlow/TestResults/Smoke-UI.trx" "Reports/Smoke-UI-Report.html"
```

#### Report Contents

Generated HTML reports include:

- **Summary Section**
  - Test run timestamp
  - Total tests executed
  - Pass/fail/skip counts
  - Pass percentage

- **Summary Cards**
  - Visual metrics (color-coded)
  - Key performance indicators

- **Detailed Results Table**
  - Test names
  - Execution status (color-coded)
  - Duration
  - Computer name

- **Individual Test Details**
  - Standard output logs
  - Error messages
  - Execution time breakdown

#### Color Coding

- 🟢 **Green**: Passed tests
- 🔴 **Red**: Failed tests
- 🟠 **Orange**: Skipped tests
- ⚫ **Gray**: Unknown status

---

## 🔧 Supporting Components

### CommunicationADF (Azure Data Factory)
Utilities for interacting with Azure Data Factory pipelines and data operations.

### CommunicationExcel
Excel file handling for test data management and reporting.

### CommunicationLoad (Performance Testing)
Load testing using Locust framework.

**Setup:**
```bash
cd CommunicationLoad
pip install -r requirements.txt
locust -f locustfile.py
```

### CommunicationSqlServer
SQL Server utilities for database operations and queries.

### CommunicationXrayCloud
Integration with Atlassian Xray Cloud for test case management and result reporting.

---

## 👨‍💻 Development

### Project Structure for Developers

#### Adding New Tests

1. **Create Feature File** (`.feature`)
   ```bash
   AppSpecFlow/Features/YourFeature.feature
   ```

2. **Implement Steps** (`.cs`)
   ```bash
   AppSpecFlow/AppSteps/Steps.cs
   ```

3. **Run Tests**
   ```bash
   dotnet test --filter:"TestCategory=YourCategory"
   ```

#### Core Utilities

**APIUtil.cs**: Use for REST/GraphQL API calls
```csharp
var response = APIUtil.GetRequest("https://api.example.com/endpoint");
```

**SeleniumUtil.cs**: Use for web UI automation
```csharp
var element = SeleniumUtil.FindElement(By.XPath("//button[@id='submit']"));
```

**FileUtils.cs**: Use for file operations
```csharp
var files = FileUtils.GetFilesInDirectory("/path/to/dir");
```

**EncryptionHelper.cs**: Use for encrypting sensitive data
```csharp
var encrypted = EncryptionHelper.EncryptString("sensitive-data");
```

### Running Unit Tests

```bash
# Run all unit tests
dotnet test CoreUnitTests/CoreUnitTests.csproj

# Run specific test class
dotnet test CoreUnitTests/CoreUnitTests.csproj --filter:"ClassName=CoreUnitTests.APIUtilTests"

# With code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

---

## 🚨 Troubleshooting

### Issue: Cannot Connect to SQL Server

**Solution:**
1. Verify SQL Server is running
2. Check connection string in environment variables
3. Ensure VPN connection is active for CW Network
4. Verify firewall rules allow connection

```cmd
SET SQLSERVER="your-actual-server"
SET SQLUSER="your-actual-user"
SET SQLSERVER_PASSWORD="your-actual-password"
```

### Issue: WebDriver Not Found

**Solution:**
The framework uses WebDriverManager for automatic driver management. Ensure:

1. Chrome browser is installed
2. Framework can access the internet for driver download
3. Check `Chrome/` folder contains compatible driver versions

```bash
dotnet build --configuration Release
```

### Issue: Tests Timeout

**Solution:**
1. Check application server is running
2. Verify network connectivity
3. Increase timeout in configuration if needed
4. Check application logs for errors

### Issue: Azure Storage Connection Fails

**Solution:**
Verify environment variables:

```cmd
SET AZURE_STORAGE_ACCOUNT_NAME="correct-account-name"
SET AZURE_STORAGE_ACCOUNT_KEY="valid-base64-encoded-key"
SET AZURE_STORAGE_FILESYSTEM="valid-container-name"
```

### Issue: Test Results Not Generated

**Solution:**
Ensure you specify the logger parameter:

```bash
dotnet test --logger "trx;logfilename=TestResults.trx"
```

Results will be in: `AppSpecFlow/TestResults/TestResults.trx`

---

## 📋 Nightly Regression Process

The framework supports scheduled nightly regression testing:

```bash
# Run all tests expected to be GREEN
dotnet test --filter:"TestCategory=Green" --logger "trx;logfilename=Green.trx"
```

**Process:**
1. Run Green test category
2. Generate HTML report
3. Any failures treated as priority the next morning
4. Classify failures as: "By Design" (expected) or "Regression" (unexpected)
5. Allocate resources to fix regressions immediately

---

## 📚 Additional Resources

- [Reqnroll Documentation](https://docs.reqnroll.net/)
- [Selenium WebDriver Documentation](https://www.selenium.dev/documentation/)
- [GraphQL Best Practices](https://graphql.org/learn/)
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Azure Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/)

---

## 📄 License

This project is licensed under the terms specified in the LICENSE file.

---

## ✉️ Support

For issues, questions, or contributions:
1. Review existing test examples in `AppSpecFlow/Features/`
2. Check the detailed analysis in [UNIT_TESTS_ANALYSIS.md](ATF/UNIT_TESTS_ANALYSIS.md)
3. Contact the development team via GIT

---

**Last Updated**: January 2026  
**Version**: 1.0  
**Framework**: .NET 9.0, Reqnroll, Selenium 4.24+
