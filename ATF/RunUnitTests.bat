@echo off
REM Unit Tests Runner - CoreUnitTests using xUnit
REM This is separate from SpecFlow feature tests

setlocal enabledelayedexpansion

REM Build the unit test project
echo Building CoreUnitTests project...
dotnet build CoreUnitTests\CoreUnitTests.csproj -c Debug

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b 1
)

REM Run xUnit tests with detailed output
echo.
echo ============================================================
echo Running Unit Tests (with detailed output)
echo ============================================================
echo.
echo Test Format: [ClassName.TestMethodName] PASSED/FAILED
echo.
dotnet test CoreUnitTests\CoreUnitTests.csproj --no-build -v detailed --logger "console;verbosity=detailed"

echo.
echo ============================================================
if %ERRORLEVEL% EQU 0 (
    echo Unit Tests PASSED - All tests executed successfully!
    echo ============================================================
) else (
    echo Unit Tests FAILED - See error details above
    echo ============================================================
    echo.
    echo TIP: Look for lines starting with "CoreUnitTests." to identify which test failed
    echo.
)

endlocal
