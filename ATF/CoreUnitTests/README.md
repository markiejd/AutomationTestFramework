# CoreUnitTests - Unit Test Project

## Overview

This is a dedicated **xUnit-based unit test project** for testing utilities and helper methods in the **Core** project. It operates independently from the SpecFlow feature tests and uses a separate execution path.

### Key Features
- ✅ **xUnit 2.7.0** testing framework
- ✅ **41 unit tests** across 9 test classes
- ✅ **Easy to run** via batch script or dotnet CLI
- ✅ **Parameterized tests** using `[Theory]` and `[InlineData]`
- ✅ **Isolated from SpecFlow** tests (no interference with feature file commands)

---

## Project Structure

```
CoreUnitTests/
├── CoreUnitTests.csproj                 # Project configuration with xUnit dependencies
├── README.md                             # This file
├── APIUtilTests.cs                      # HTTP/API method tests
├── ADFUtilsTests.cs                     # Azure Data Factory integration tests
├── SeleniumUtilTests.cs                 # Selenium WebDriver utility tests
├── GraphQLUtilTests.cs                  # GraphQL query/response handling tests
├── StringValuesTests.cs                 # Text replacement & transformation tests
├── FileUtilsTests.cs                    # File I/O operation tests
├── EncryptionHelperTests.cs             # AES encryption/decryption tests
├── DateValuesTests.cs                   # Date utility tests
└── TimeValuesTests.cs                   # Time utility tests
```

---

## Prerequisites

### System Requirements
- **Windows 10+** or **Linux/macOS**
- **.NET 9.0 SDK** installed
- **PowerShell** or **Command Prompt**

### Verify Installation
```bash
dotnet --version
```

Should output version 9.0.0 or higher.

---

## Running Tests

### Option 1: Batch Script (Recommended for Windows)

Run all tests with a single command:

```batch
RunUnitTests.bat
```

**What it does:**
1. Builds the CoreUnitTests project
2. Runs all xUnit tests
3. Displays results in console
4. Shows success/failure summary

**Location:** Root of ATF project (`c:\Repos\GITHUB\AutomationTestFramework\ATF\`)

---

### Option 2: Command Line - Run All Tests

**Using dotnet CLI:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --no-build -v normal
```

**From PowerShell:**
```powershell
dotnet test CoreUnitTests\CoreUnitTests.csproj --no-build -v normal
```

**Parameters:**
- `--no-build` - Skip rebuilding (assumes already built)
- `-v normal` - Set verbosity level

**Verbosity Options:**
- `quiet` - Minimal output
- `normal` - Standard output
- `detailed` - More information
- `diagnostic` - Maximum detail

---

### Option 3: Run Specific Test Class

Run tests from a single test class:

```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=FileUtilsTests"
```

**Examples:**

**Run FileUtilsTests only:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=FileUtilsTests"
```

**Run StringValuesTests only:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=StringValuesTests"
```

**Run EncryptionHelperTests only:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=EncryptionHelperTests"
```

---

### Option 4: Run Specific Test Method

Run a single test method:

```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "FullyQualifiedName~FileUtilsTests.FileUtils_GetRepoDirectory_ReturnsValidPath"
```

**Simplified approach:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "Name=FileUtils_GetRepoDirectory_ReturnsValidPath"
```

**Examples:**

**Run one test from FileUtilsTests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "Name=FileUtils_GetRepoDirectory_ReturnsValidPath"
```

**Run one test from StringValuesTests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "Name=StringValues_TextReplacementService_WithMYREPO_ReturnsReplacedPath"
```

---

## Test Categories

### 1. **APIUtilTests.cs** (2 tests)
Tests HTTP request/response handling in APIUtil class.

**Run only API tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=APIUtilTests"
```

### 2. **ADFUtilsTests.cs** (4 tests)
Tests Azure Data Factory integration utilities.

**Run only ADF tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=ADFUtilsTests"
```

### 3. **SeleniumUtilTests.cs** (2 tests)
Tests Selenium WebDriver browser support.

**Run only Selenium tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=SeleniumUtilTests"
```

### 4. **GraphQLUtilTests.cs** (7 tests)
Tests GraphQL query building and response handling.

**Run only GraphQL tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=GraphQLUtilTests"
```

### 5. **StringValuesTests.cs** (5 tests)
Tests text replacement and transformation functions (ATFVARIABLE, MYREPO, etc.).

**Run only String Transformation tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=StringValuesTests"
```

### 6. **FileUtilsTests.cs** (7 tests)
Tests file I/O, path management, and file operations.

**Run only File Utility tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=FileUtilsTests"
```

### 7. **EncryptionHelperTests.cs** (4 tests)
Tests AES encryption and decryption (requires ENCRYPTION_KEY and ENCRYPTION_IV environment variables).

**Run only Encryption tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=EncryptionHelperTests"
```

### 8. **DateValuesTests.cs** (5 tests)
Tests date utility functions and date validation.

**Run only Date Utility tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=DateValuesTests"
```

### 9. **TimeValuesTests.cs** (5 tests)
Tests time utility functions and time validation.

**Run only Time Utility tests:**
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --filter "ClassName=TimeValuesTests"
```

---

## Test Execution Examples

### Build First (Optional)
```bash
dotnet build CoreUnitTests\CoreUnitTests.csproj -c Debug
```

### Run All Tests with Detailed Output
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj -v detailed
```

### Run Tests and Generate Report
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --logger "console;verbosity=normal"
```

### Run Tests Multiple Times (for stability check)
```bash
for /L %i in (1,1,3) do dotnet test CoreUnitTests\CoreUnitTests.csproj
```

---

## Understanding Test Results

### Success Output
```
✓ FileUtils_GetRepoDirectory_ReturnsValidPath (123ms)
✓ FileUtils_CurrentDirectory_IsNotEmpty (45ms)

2 passed, 0 failed
```

### Failure Output
```
✗ FileUtils_GetRepoDirectory_ReturnsValidPath
  Expected: path should not be empty
  Actual: path was empty

1 passed, 1 failed
```

### Common Issues

| Issue | Solution |
|-------|----------|
| `dotnet: command not found` | Install .NET 9.0 SDK |
| `Test project not found` | Run from ATF root directory |
| `Build failed` | Check Core project references |
| `Encryption tests fail` | Set ENCRYPTION_KEY and ENCRYPTION_IV environment variables |

---

## Environment Variables

Some tests require environment variables to be set:

### For EncryptionHelperTests:
```bash
# Windows Command Prompt
set ENCRYPTION_KEY=YourSecretKeyHere32BytesLong
set ENCRYPTION_IV=YourSecretIV16Bytes
```

```powershell
# PowerShell
$env:ENCRYPTION_KEY="YourSecretKeyHere32BytesLong"
$env:ENCRYPTION_IV="YourSecretIV16Bytes"
```

If not set, encryption tests will return `UNKNOWN` instead of encrypted values.

---

## Test Anatomy

All tests follow the **Arrange-Act-Assert (AAA)** pattern:

```csharp
[Fact]
public void FileUtils_GetRepoDirectory_ReturnsValidPath()
{
    // ARRANGE - Set up test data
    
    // ACT - Execute the method being tested
    var repoDir = FileUtils.GetRepoDirectory();

    // ASSERT - Verify the results
    Assert.NotNull(repoDir);
    Assert.NotEmpty(repoDir);
}
```

### Test Types

**[Fact]** - Single test case
```csharp
[Fact]
public void TestName() { /* single scenario */ }
```

**[Theory]** - Parameterized test with multiple data sets
```csharp
[Theory]
[InlineData("value1")]
[InlineData("value2")]
public void TestName(string input) { /* runs twice */ }
```

---

## Adding New Tests

### 1. Create New Test File
Create a file named `*Tests.cs`:
```csharp
using Xunit;
using Core.YourNamespace;

namespace CoreUnitTests
{
    public class YourComponentTests
    {
        [Fact]
        public void YourComponent_Method_ExpectedBehavior()
        {
            // Arrange
            
            // Act
            
            // Assert
        }
    }
}
```

### 2. Run Tests
```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj
```

### 3. View Results
All new tests will appear in the test results.

---

## Continuous Integration (CI)

Use this command in CI/CD pipelines:

```bash
dotnet test CoreUnitTests\CoreUnitTests.csproj --no-build --logger "console;verbosity=minimal" --results-directory ./TestResults
```

---

## Troubleshooting

### Build Issues
```bash
# Clean and rebuild
dotnet clean CoreUnitTests\CoreUnitTests.csproj
dotnet build CoreUnitTests\CoreUnitTests.csproj -c Debug
```

### Test Discovery Issues
```bash
# List all discoverable tests
dotnet test CoreUnitTests\CoreUnitTests.csproj --collect:"XPlat Code Coverage" --no-build -v detailed
```

### Dependency Issues
```bash
# Restore NuGet packages
dotnet restore CoreUnitTests\CoreUnitTests.csproj
```

---

## Comparison: Unit Tests vs Feature Tests

| Aspect | Unit Tests | Feature Tests |
|--------|-----------|--------------|
| **Framework** | xUnit | SpecFlow |
| **Location** | `CoreUnitTests/` | `AppSpecFlow/Features/` |
| **Run Command** | `dotnet test CoreUnitTests/...` | `dotnet test --filter:"TestCategory=TEST1"` |
| **Focus** | Core utility methods | End-to-end BDD scenarios |
| **Execution Path** | Completely separate | Completely separate |
| **Script** | `RunUnitTests.bat` | Feature files command |

---

## Summary

| Task | Command |
|------|---------|
| Run all tests | `RunUnitTests.bat` |
| Run specific class | `dotnet test ... --filter "ClassName=FileUtilsTests"` |
| Run single test | `dotnet test ... --filter "Name=TestMethodName"` |
| Build project | `dotnet build CoreUnitTests\CoreUnitTests.csproj` |
| Show test details | Add `-v detailed` to any command |

---

## Resources

- [xUnit Documentation](https://xunit.net/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)
- [Test Filtering in dotnet test](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test)

---

**Questions or Issues?** Check the [UNIT_TESTS_ANALYSIS.md](../UNIT_TESTS_ANALYSIS.md) document for detailed component analysis.
