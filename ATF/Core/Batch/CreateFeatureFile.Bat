cls
@ECHO MUST ANSWER ALL QUESTIONS!

@echo off
set "testname="
set /p testname=What Is The Test Number (Project included)?
set "title="
set /p title=Title of Test (required - NO SPACES)?
set "app="
set /p app=What Application Is Being Tested (required - name of Application)?
set "testdesc1="
set /p testdesc1=Line 1 sentenace description (required - do not use keywords as first word)?
set "testdesc2="
set /p testdesc2=Line 2 sentenace description (required - but DOT can work)?
set "testset="
set /p testset=What Test Set is this test in? (required - if not known enter NA)?
set "team="
set /p team=What Team are you in? (required - Team Name)?


set file = .\AppSpecFlow\Features\%testname%.feature

echo @all  @%testname% @%testset% @SKIP @%app% @%team% @%app%%testname% >.\AppSpecFlow\Features\%testname%.feature
echo Feature: %testname% %title% >>.\AppSpecFlow\Features\%testname%.feature
echo      %testdesc1% >>.\AppSpecFlow\Features\%testname%.feature
echo      %testdesc2% >>.\AppSpecFlow\Features\%testname%.feature
echo           dotnet test --filter:"TestCategory=%testname%" --logger "trx;logfilename=%testname%.trx" >>.\AppSpecFlow\Features\%testname%.feature
echo.  >>.\AppSpecFlow\Features\%testname%.feature
echo Scenario Outline: %testname%-%title% >>.\AppSpecFlow\Features\%testname%.feature
echo    Given Test Passes >>.\AppSpecFlow\Features\%testname%.feature

if exist .\AppSpecFlow\Features\%app%\ (xcopy .\AppSpecFlow\Features\%testname%.feature .\AppSpecFlow\Features\%app%\%testname%.feature*)
if exist .\AppSpecFlow\Features\%app%\ (del .\AppSpecFlow\Features\%testname%.feature)

echo FEATURE FILE SHOULD BE GREEN and EASY to find!
