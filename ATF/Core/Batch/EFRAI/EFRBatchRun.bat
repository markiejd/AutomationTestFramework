cls
@ECHO EFR Batch Run
setlocal enabledelayedexpansion

set "jiraKey="
set /p jiraKey=What is the Jira Key?
@ECHO %jiraKey%

cd
CALL .\AppSpecFlow\Batch\BatchRUN.bat
cd 

@ECHO BatchRUN Executed
pause

@REM set file = .\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo @all @%jiraKey% >.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo Feature: RRRRRRR%jiraKey% >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo      hello >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo      goofbye >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo           dotnet test --filter:"TestCategory=%jiraKey%" --logger "trx;logfilename=%jiraKey%.trx" >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo.  >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo Scenario Outline: %jiraKey%-justWORK >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
@REM echo    Given All EFR BaseLine Tests Are Executed Upload To Jira >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature


@ECHO Now to Collate the Results
cd
CALL dotnet test --filter:"TestCategory=%jiraKey%" --logger "trx;logfilename=%jiraKey%.trx" 

@ECHO Now to Upload
echo Enter Uploading Evidence!
CALL .\Core\Batch\JiraUpload.Bat


@ECHO Now to CLEAN UP
pause 
call .\Core\Batch\CleanTestHistory.Bat

