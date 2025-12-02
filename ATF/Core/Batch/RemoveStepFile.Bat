@echo off
:START
    SET settingLocation=%cd%\.vscode\settings.json
    SET tempFile=%cd%\CORE\.vscode\temp.json
    del %tempFile% /q
    SET show=1
    cls
    echo There are 3 Default Step File Locations
    echo ./Generic/Steps/*.cs
    echo ./Generic/Steps/Elements/*/*.cs
    echo and
    echo ./AppSpecFlow/AppSteps/Steps.cs
    echo These CAN NOT BE removed!
    REM SHOW WHAT WE HAVE ALREADY
    FOR /F "tokens=* delims=" %%x in (%settingLocation%) DO (CALL :READSETTINGS "%%x")
    xcopy %tempFile% %settingLocation% /v/q/y > nul
    pause
    GOTO :END

:KEEP
    SET strKEEP=%1   
    echo %~1>>%tempFile%
    EXIT /B

:DELETE
    echo removed!
    EXIT /B

:SHOW2
    SET str2=%1   
    echo %~1
    if not x%str2:./AppSpecFlow/AppSteps/Steps.cs=%==x%str2% SET show=3
    if %show%==3 echo %~1>>%tempFile%
    if %show%==3 EXIT /B
    echo 1 to Keep, 2 to Delete
    choice /n /c:12 /M "Choose and option (1 or 2) "
    IF %ERRORLEVEL%==1 GOTO KEEP %1
    IF %ERRORLEVEL%==2 GOTO DELETE %1
    EXIT /B

:SETTERMINAL
    SET strSETTERMINAL=%1 
    echo     "terminal.integrated.defaultProfile.windows": "Command Prompt",>>%tempFile%
    SET show=6
    EXIT /B


   
:READSETTINGS
    SET str1=%1    
    if not x%str1:terminal.in=%==x%str1% SET show=5
    if %show%==1 echo %~1>>%tempFile%
    if %show%==3 echo %~1>>%tempFile%
    if %show%==6 echo %~1>>%tempFile%
    if %show%==2 GOTO SHOW2 %1
    if not x%str1:./Generic/Steps/Elements/*/*.cs=%==x%str1% SET show=2
    if %show%==5 GOTO SETTERMINAL %1
    EXIT /B

:END
