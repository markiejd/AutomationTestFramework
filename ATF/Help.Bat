echo off
:menu
cls
echo This is the SBU Test Framework.  Please select an option
echo 1 - Create Feature File
echo 2 - Clear Test History
echo 3 - Build Application
echo 4 - Create Page File
echo 5 - Run Test
echo 6 - Settings
echo 7 - Jira
echo 8 - Application In Test
echo 9 - exit
set /P INPUT="Choose an option (1-9): "

IF "%INPUT%"=="1" GOTO CREATEFEATUREFILE
IF "%INPUT%"=="2" GOTO CLEARTESTHISTORY
IF "%INPUT%"=="3" GOTO BUILDAPPLICATION
IF "%INPUT%"=="4" GOTO CREATEPAGEFILE
IF "%INPUT%"=="5" GOTO RUNTEST
IF "%INPUT%"=="6" GOTO SETTINGS
IF "%INPUT%"=="7" GOTO JIRA
IF "%INPUT%"=="8" GOTO APPLICATION
IF "%INPUT%"=="9" GOTO EXIT
goto menu

:APPLICATION
cls
echo What Application In Test?
echo 1 - New Application In Test
echo 2 - EFR AI Chatbot
echo 3 - ATF360 API
echo 4 - WebSite
echo 5 - RETIRE Application In Test
echo 6 - RESTORE Application In Test
echo 7 - Mock API
echo 8 - UNKNOWNN
echo 9 - return to menu
set /P INPUT="Choose an option (1-9): "

IF "%INPUT%"=="1" GOTO NEWAPPINTEST
IF "%INPUT%"=="2" GOTO EFRAI
IF "%INPUT%"=="3" GOTO API
IF "%INPUT%"=="4" GOTO WEBSITE
IF "%INPUT%"=="5" GOTO RETIREAPP
IF "%INPUT%"=="6" GOTO RETRIEVEAPP
IF "%INPUT%"=="7" GOTO MockAPI
IF "%INPUT%"=="8" GOTO FireFly
IF "%INPUT%"=="9" GOTO menu
goto APPLICATION

:FireFly
cls
cd AppSpecFlow
cd Batch
CALL FireFlySmoke.bat
cd..
cd..
goto APPLICATION

:MockAPI
cls
START .\CommunicationMockAPIServer\MockAPI\runme.bat
goto APPLICATION


:WEBSITE
cls
cd APIServer
cd WebSite
start npm start
cd..
cd..
goto APPLICATION

:API
cls
cd APIServer
echo API Server will start in new command window
start dotnet run
cd..
goto APPLICATION

:NEWAPPINTEST
cls
set INPUT=
set /P INPUT=Application In Test being Created as NEW!: %=% 
echo %INPUT%
CALL .\Core\Batch\CreateNewAppInTest.Bat %INPUT%
goto menu

:EFRAI
cls
echo What Application In Test?
echo 1 - New Base Line CSV File Receieved
echo 2 - Batch Run Base Line
echo 9 - return to Application in Test
set /P INPUT="Choose an option (1-2 or 9): "

IF "%INPUT%"=="1" GOTO EFRAIBASELINE
IF "%INPUT%"=="2" GOTO EFRBATCH
IF "%INPUT%"=="9" GOTO APPLICATION
goto EFRAI

:EFRAIBASELINE
call .\Core\Batch\EFRAI\EFRBaseLineCSV.Bat
goto menu

:EFRBATCH
call .\Core\Batch\EFRAI\EFRBatchRun.Bat
goto menu

:CREATEFEATUREFILE
call .\Core\Batch\CreateFeatureFile.Bat
pause
goto menu

:CLEARTESTHISTORY
call .\Core\Batch\CleanTestHistory.Bat
pause
goto menu

:BUILDAPPLICATION
call .\Core\Batch\CreateBuild.Bat
pause
goto menu

:CREATEPAGEFILE
call .\Core\Batch\PageFile\CreatePageFile.Bat
pause
goto menu

:SETTINGS
cls
echo Configure the VS Code Settings Files
echo 1 - Change to Level 1
echo 2 - Change to Level 2
echo 3 - Change to Level 3
echo 4 - Modify VS Code Settings Files
echo 5 - Change to Test Tool selenium
echo 6 - Change to Test Tool playwright
echo 9 - Return to Menu
set /P INPUT="Choose an option (1-6 or 9): "

IF "%INPUT%"=="1" GOTO CHANGELEVEL1
IF "%INPUT%"=="2" GOTO CHANGELEVEL2
IF "%INPUT%"=="3" GOTO CHANGELEVEL3
IF "%INPUT%"=="4" GOTO SETTINGSFILECHANGE
IF "%INPUT%"=="5" GOTO SELENIUM
IF "%INPUT%"=="6" GOTO PLAYWRIGHT
IF "%INPUT%"=="9" GOTO menu
goto SETTINGS

:CHANGELEVEL1
call .\Core\Batch\ChangeSettings.Bat 1
goto SETTINGS

:CHANGELEVEL2
call .\Core\Batch\ChangeSettings.Bat 2
goto SETTINGS

:CHANGELEVEL3
call .\Core\Batch\ChangeSettings.Bat 3
goto SETTINGS

:SELENIUM
call .\Core\Batch\TestTools\SeleniumTestTool.Bat
goto SETTINGS

:PLAYWRIGHT
call .\Core\Batch\TestTools\PlaywrightTestTool.Bat
goto SETTINGS

:RETIREAPP
echo Retiring Application
set INPUT=
set /P INPUT=Application In Test being Retired: %=% 
echo %INPUT%
CALL .\Core\Batch\Retire\Retire.bat %INPUT%
goto APPLICATION

:RETRIEVEAPP
echo retrieve application
set INPUT=
set /P INPUT=Application In Test being Retrieved: %=% 
echo %INPUT%
CALL .\Core\Batch\Retrieve\Retrieve.bat %INPUT%
goto APPLICATION

:SETTINGSFILECHANGE
cls
echo What are we changing?
echo 1 - Add Steps File Location!
echo 2 - Remove Steps File Location
echo 3 - Reset Settings
echo 9 - Return to Menu
set /P INPUT="Choose an option (1-3 or 9): "

IF "%INPUT%"=="1" GOTO ADDSTEPS
IF "%INPUT%"=="2" GOTO REMOVESTEPS
IF "%INPUT%"=="3" GOTO RESETSETTINGS
IF "%INPUT%"=="9" GOTO SETTINGS
goto SETTINGSFILECHANGE

:REMOVESTEPS
echo Removing Step File
CALL .\Core\Batch\RemoveStepFile.Bat
goto SETTINGSFILECHANGE

:ADDSTEPS
echo Adding Step File
CALL .\Core\Batch\AddStepFile.Bat
goto SETTINGSFILECHANGE

:RESETSETTINGS
echo nothing yet!
pause
goto SETTINGSFILECHANGE

:JIRA
cls
echo Jira Commands
echo 1 - Run Jira Exection
echo 2 - Upload Evidence
echo 3 - nothing yet
echo 4 - nothing yet
echo 9 - Return to Menu
set /P INPUT="Choose an option (1-4 or 9): "

IF "%INPUT%"=="1" GOTO JIRAEXE
IF "%INPUT%"=="2" GOTO JIRAEVIDENCE
IF "%INPUT%"=="3" GOTO JIRAEXE
IF "%INPUT%"=="4" GOTO JIRAEXE
IF "%INPUT%"=="9" GOTO menu
goto JIRA

:JIRAEXE
echo Enter the Key for the Exection
set INPUT=
set /P INPUT=Type input: %=% 
echo Creating Batch Batch File!
CALL .\Core\Batch\JiraExecution.Bat %INPUT%
goto JIRA

:JIRAEVIDENCE
echo Enter Uploading Evidence!
CALL .\Core\Batch\JiraUpload.Bat
goto JIRA

:RUNTEST
call .\Core\Batch\CreateTest.Bat
pause
goto menu

:FAILEXIT
echo We don't have anything for that option yet!
goto EXIT

:EXIT
echo DONE