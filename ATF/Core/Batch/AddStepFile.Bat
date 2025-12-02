@echo off
:START
    SET settingLocation=%cd%\.vscode\settings.json
    SET tempFile=%cd%\CORE\.vscode\temp.json
    del %tempFile% /q
    SET show=1
    SET line=0
    SET line1=""
    SET line2=""
    SET line3=""
    SET line4=""
    SET line5=""
    SET line6=""
    SET line7=""
    SET line8=""
    SET line9=""
    cls
    echo There are 3 Default Step File Locations
    echo ./Generic/Steps/*.cs
    echo ./Generic/Steps/Elements/*/*.cs
    echo and
    echo ./AppSpecFlow/AppSteps/Steps.cs
    echo Please enter you Step File using the same format as above, direction below, double quotes not needed
    echo "./<project>/<dir>/<filename>.cs "
    set /p "stepLocation=Enter location : "
    echo %stepLocation%
    REM SHOW WHAT WE HAVE ALREADY
    FOR /F "tokens=* delims=" %%x in (%settingLocation%) DO (CALL :READSETTINGS "%%x")
    xcopy %tempFile% %settingLocation% /v/q/y > nul
    echo ADDED!
    GOTO :END


:STEPS 
    SET strSTEPS=%1  
    if not x%strSTEPS:./AppSpecFlow/AppSteps/Steps.cs=%==x%strSTEPS% SET show=3
    if %show%==2 echo %~1>>%tempFile%
    if %show%==3 echo         "%stepLocation%",>>%tempFile%
    if %show%==3 echo %~1>>%tempFile%
    if %show%==3 SET show=4
    EXIT /B

:SHOW4
    SET strSHOW4=%1    
    if %show%==4 echo %~1>>%tempFile% 
    if not x%str1:},=%==x%str1% SET show=5
    EXIT /B


:SHOW5
    SET strSHOW5=%1   
    if not x%strSHOW5:terminal.in=%==x%strSHOW5% echo     "terminal.integrated.defaultProfile.windows": "Command Prompt",>>%tempFile%
    SET show=6
    EXIT /B

    
:READSETTINGS
    SET str1=%1    
    if not x%str1:cucumberautocomplete.steps=%==x%str1% SET show=2
    if %show%==6 echo %~1>>%tempFile%   
    if %show%==1 echo %~1>>%tempFile%
    if %show%==2 GOTO STEPS %1
    if %show%==4 GOTO SHOW4 %1
    if %show%==5 GOTO SHOW5 %1
    EXIT /B

:END
