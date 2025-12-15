@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

REM Check if a parameter is passed
IF "%~1"=="" (
    echo No parameter provided.
    GOTO :EOF 
)

REM Convert the parameter to lowercase
SET "input=%~1"
CALL :toLowerCase input

REM Echo the lowercase result
ECHO %input%

REM Check the result and echo messages
IF "%input%"=="targets" ( 
    ECHO Targets Building
    cd targets
    dotnet build
    cd..
    ECHO Targets Built
) ELSE IF "%input%"=="api" (
    ECHO API Building
    cd AppXAPI
    dotnet build
    cd..
    ECHO API Built
) ELSE IF "%input%"=="core" (
    ECHO Core Building
    cd core
    echo %cd%
    dotnet build
    cd..
    ECHO Core Built
) ELSE IF "%input%"=="generic" (
    ECHO generic Building
    cd generic
    dotnet build
    cd..
    ECHO generic Built
)

ENDLOCAL
GOTO :EOF

:toLowerCase
SET "%~1=!%~1:A=a!"
SET "%~1=!%~1:B=b!"
SET "%~1=!%~1:C=c!"
SET "%~1=!%~1:D=d!"
SET "%~1=!%~1:E=e!"
SET "%~1=!%~1:F=f!"
SET "%~1=!%~1:G=g!"
SET "%~1=!%~1:H=h!"
SET "%~1=!%~1:I=i!"
SET "%~1=!%~1:J=j!"
SET "%~1=!%~1:K=k!"
SET "%~1=!%~1:L=l!"
SET "%~1=!%~1:M=m!"
SET "%~1=!%~1:N=n!"
SET "%~1=!%~1:O=o!"
SET "%~1=!%~1:P=p!"
SET "%~1=!%~1:Q=q!"
SET "%~1=!%~1:R=r!"
SET "%~1=!%~1:S=s!"
SET "%~1=!%~1:T=t!"
SET "%~1=!%~1:U=u!"
SET "%~1=!%~1:V=v!"
SET "%~1=!%~1:W=w!"
SET "%~1=!%~1:X=x!"
SET "%~1=!%~1:Y=y!"
SET "%~1=!%~1:Z=z!"
GOTO :EOF
