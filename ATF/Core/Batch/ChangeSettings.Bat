@echo off
:START
    SET level=%1
    SET settingLocation=%cd%\.vscode\settings.json
    SET tempFile=%cd%\CORE\.vscode\temp.json
    SET level1=%cd%\CORE\.vscode\Levels\settings1.txt
    SET level2=%cd%\CORE\.vscode\Levels\settings2.txt
    SET level3=%cd%\CORE\.vscode\Levels\settings3.txt
    del %tempFile% /q
    SET show=1
    FOR /F "tokens=* delims=" %%x in (%settingLocation%) DO (CALL :READSETTINGS "%%x")
    xcopy %tempFile% %settingLocation% /v/q/y > nul
    del %tempFile% /q
    GOTO :END

:READSETTINGS
    SET str1=%1
    if %show%==1 echo %~1>>%tempFile%
    if not x%str1:files.exclude=%==x%str1% SET show=2
    if %show%==4 echo     "terminal.integrated.defaultProfile.windows": "Command Prompt",>>%tempFile%
    if %show%==4 SET show=5
    if %show%==5 echo %~1>>%tempFile%
    if %show%==3 GOTO :CLOSEEXCLUDES
    if %show%==2 GOTO :INSERTEXCLUDES

    EXIT /B

:READJSON
    if %show%==2 SET show=3
    if %level%==1 FOR /F "tokens=* delims=" %%x in (%level1%) DO (CALL :READSETTINGSJSON "%%x")
    if %level%==2 FOR /F "tokens=* delims=" %%x in (%level2%) DO (CALL :READSETTINGSJSON "%%x")
    if %level%==3 FOR /F "tokens=* delims=" %%x in (%level3%) DO (CALL :READSETTINGSJSON "%%x")
    EXIT /B

:READSETTINGSJSON
    SET str1=%1
    if %show%==3 echo %~1>>%tempFile%
    EXIT /B


:CLOSEEXCLUDES
    if not x%str1:},=%==x%str1% SET show=4
    if %show%==4 echo     },>>%tempFile%
    EXIT /B

:INSERTEXCLUDES
    if %show%==2 GOTO :READJSON
    EXIT /B

:END