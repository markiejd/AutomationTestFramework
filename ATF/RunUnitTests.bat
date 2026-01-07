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

REM Run xUnit tests using vstest.console (alternative to dotnet test)
echo.
echo Running xUnit tests...
dotnet test CoreUnitTests\CoreUnitTests.csproj --no-build -v normal --logger "console;verbosity=normal"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Unit tests completed successfully!
) else (
    echo.
    echo Unit tests failed!
    exit /b 1
)

endlocal
