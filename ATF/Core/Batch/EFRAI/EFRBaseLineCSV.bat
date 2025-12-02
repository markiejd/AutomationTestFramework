cls
@ECHO EFR AI Chatbot Base Test Creation
setlocal enabledelayedexpansion

@REM Move the current BaseLine if exists
:: Extract year, month, and day from %date%
set "year=!DATE:~6,4!"
set "month=!DATE:~3,2!"
set "day=!DATE:~0,2!"
set "hour=!TIME:~0,2!"
set "min=!TIME:~3,2!"
set "sec=!TIME:~7,2!"

IF NOT EXIST ".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\New\Baseline.csv" goto NONEW

set newFile=".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\Retired\Baseline%year%%month%%day%%hour%%min%%sec%.csv"

IF EXIST ".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\Current\Baseline.csv" MOVE ".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\Current\Baseline.csv" %newFile%
IF EXIST ".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\New\Baseline.csv" MOVE ".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\New\Baseline.csv" ".\AppSpecFlow\TestPreConditionData\YorkCSV\BaseLineCSV\Current\Baseline.csv"


set "output="
set /p output=How many questions per batch?

@ECHO %output%
@ECHO Manually Create the Test In Jira - Record the Jira Key for the TEST
set "jiraKey="
set /p jiraKey=What is the Jira Key?
@ECHO %jiraKey%
@REM CREATE THE BATCH TO CREATE THE TEST FEATURE FILE


set file = .\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo @BatchSystem-001 >.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo Feature: Batch Create >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo      THIS IS A SYSTEM TEST >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo      TO CREATE BATCHES OF TESTS >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo           dotnet test --filter:"TestCategory=BatchSystem" --logger "trx;logfilename=BatchSystem.trx" >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo.  >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo Scenario Outline: BatchSystem-001 >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature
echo    Given I Create Batches Of %output% Questions FOR EFR AI With Jira Test Key "%jiraKey%" >>.\AppSpecFlow\Features\Yorkshire\Base\CreateBatches.feature

CALL dotnet test --filter:"TestCategory=BatchSystem-001" --logger "trx;logfilename=BatchSystem.trx" 

@ECHO Run ALL Questions
cd
@REM cd AppSpecFlow
@REM cd Batch
cd
CALL .\AppSpecFlow\Batch\BatchRUN.bat


@ECHO Now to Collate the Results

set file = .\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo @all @%jiraKey% >.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo Feature: RRRRRRR%jiraKey% >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo      hello >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo      goofbye >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo           dotnet test --filter:"TestCategory=%jiraKey%" --logger "trx;logfilename=%jiraKey%.trx" >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo.  >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo Scenario Outline: %jiraKey%-justWORK >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature
echo    Given All EFR BaseLine Tests Are Executed Upload To Jira >>.\AppSpecFlow\Features\Yorkshire\Base\Batch%jiraKey%.feature

@ECHO Now to Collate the Results
@ECHO CALL dotnet test --filter:"TestCategory=%jiraKey%" --logger "trx;logfilename=%jiraKey%.trx" 
CALL dotnet test --filter:"TestCategory=%jiraKey%" --logger "trx;logfilename=%jiraKey%.trx" 

@ECHO Now to Upload
echo Enter Uploading Evidence!
CALL .\Core\Batch\JiraUpload.Bat


goto EXIT

:NONEW
@ECHO There is no new file - so I'm not changing ANYTHING:
pause

:EXIT
echo DONE