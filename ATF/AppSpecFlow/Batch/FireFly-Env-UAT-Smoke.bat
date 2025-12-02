REM filepath: c:\REPO\Firefly\FireFly_Testing\ATF\AppSpecFlow\Batch\test1.bat
@echo off
echo ATFENVIRONMENT is set to %ATFENVIRONMENT%
echo SQLSERVER is set to %SQLSERVER%
echo SQLDATABASE is set to %SQLDATABASE%
echo AZURE_STORAGE_FILESYSTEM is set to %AZURE_STORAGE_FILESYSTEM%
echo CONFIRM (press any key) OR CLOSE DOWN THIS WINDOW
pause
cd..
dotnet build
cd Batch
START "Environment DB Smoke Tests" cmd /k "CALL FireFlySmoke-DB.bat"
START "Environment UI Smoke Tests" cmd /k "CALL FireFlySmoke-UI.bat"
