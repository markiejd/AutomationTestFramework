cls
@ECHO This will run a test and output


call .\Batch\CleanTestHistory.bat

@echo off
set "tagname="
set /p tagname=What Tag to be Run?      

set "output="
set /p output=What output required (HTML, trx)?    

set testType=UNKNOWN

if %output%==html set testType=html
if %output%==HTML set testType=html
if %output%==trx set testType=trx
if %output%==TRX set testType=trx

echo Test type is %testType%


if %testType%==html dotnet test --filter:"TestCategory=%tagname%" --logger "html;logfilename=%tagname%.html"
if %testType%==trx dotnet test --filter:"TestCategory=%tagname%" --logger "trx;logfilename=%tagname%.trx"


echo Test Should Have Ran!