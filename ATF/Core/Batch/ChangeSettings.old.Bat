@echo off
:START
    SET filePath=%cd%\.vscode\hello.json
    SET show=0
    SET lineNumber=0
    echo hello>>%filePath%
    del %filePath% /q
    FOR /F "tokens=* delims=" %%x in (.\.vscode\settings.json) DO (CALL :OUTPUT "%%x")
    GOTO :REPLACEOTHERJSONS
    EXIT /B

:REPLACEOTHERJSONS
echo REPLACEOTHERJSONS
pause
    SET jsonPath=%cd%\.vscode\settings.json
    SET json1Path=%cd%\CORE\.vscode\Levels\settings1.json
    SET json2Path=%cd%\CORE\.vscode\Levels\settings2.json
    SET json3Path=%cd%\CORE\.vscode\Levels\settings3.json
    SET jsonTempPath=%cd%\CORE\.vscode\Levels\temp.json
    SET jsonCLEANPath=%cd%\CORE\.vscode\Levels\settingsClean1.json
    xcopy %filePath% %jsonPath% /v/q/y > nul
    del %filePath% /q
    GOTO :GETFILEEXCLUDES
    EXIT /B

:UPDATEFILE
echo UPDATEFILE
pause

    EXIT /B

:GETFILEEXCLUDES
echo GETFILEEXCLUDES
pause
    SET lineNumber=0
    SET show=0
    SET line1=NULL
    SET line2=NULL
    SET line3=NULL
    SET line4=NULL
    SET line5=NULL
    SET line6=NULL
    SET line7=NULL
    SET line8=NULL
    SET line9=NULL
    SET line10=NULL
    FOR /F "tokens=* delims=" %%x in (%jsonPath%) DO (CALL :GETLINES "%%x")
    if not %line1%==NULL echo %line1%
    if not %line2%==NULL echo %line2%
    if not %line3%==NULL echo %line3%
    if not %line4%==NULL echo %line4%
    if not %line5%==NULL echo %line5%
    if not %line6%==NULL echo %line6%
    if not %line7%==NULL echo %line7%
    if not %line8%==NULL echo %line8%
    if not %line9%==NULL echo %line9%
    if not %line10%==NULL echo %line10%
    GOTO :UPDATEOTHERSETTINGFILES
    
    EXIT /B

:UPDATEOTHERSETTINGFILES
    echo Updating other Json Files in REPO 
    pause
    SET show=0
    del %jsonTempPath% /q
    echo in here
    pause
    FOR /F "tokens=* delims=" %%x in (%json1Path%) DO (CALL :UPDATEOTHERLINES1 "%%x")
    echo out here
    pause
    REM xcopy %jsonTempPath% %json1Path% /v/q/y > nul
    
    EXIT /B

:UPDATEOTHERLINES1
    SET str1=%1
    echo %show% %~1
    if %show%==0 echo %~1>>%jsonTempPath%
    if %show%==2 (
        if not x%str1:],=%==x%str1% SET show=3
    )
    if %show%==3 echo %~1>>%jsonTempPath%
    if %show%==3 EXIT /B
    if %show%==1 (
        echo insert!
        echo %line1%
        echo %line2%
        echo %line3%
        echo %line4%
        echo %line5%
        echo %line6%
        echo %line7%
        echo %line8%
        echo %line9%
        echo %line10%
        if not %line1%==NULL echo %line1%>>%jsonTempPath%
        if not %line2%==NULL echo %line2%>>%jsonTempPath%
        if not %line3%==NULL echo %line3%>>%jsonTempPath%
        if not %line4%==NULL echo %line4%>>%jsonTempPath%
        if not %line5%==NULL echo %line5%>>%jsonTempPath%
        if not %line6%==NULL echo %line6%>>%jsonTempPath%
        if not %line7%==NULL echo %line7%>>%jsonTempPath%
        if not %line8%==NULL echo %line8%>>%jsonTempPath%
        if not %line9%==NULL echo %line9%>>%jsonTempPath%
        if not %line10%==NULL echo %line10%>>%jsonTempPath%
        SET show=2
    )
    if %show%==0 (
        if not x%str1:[=%==x%str1% SET show=1
    )
    EXIT /B

:UPDATEOTHERLINES2
    if %show%==0 echo %~1>>%jsonTempPath%
    if %show%==1 (
        if not x%str1:],=%==x%str1% SET show=2
    )
    if %show%==2 echo %~1>>%jsonTempPath%
    if %show%==1 (
        if not %line1%== NULL echo %line1%>>%jsonTempPath%
        if not %line2%== NULL echo %line2%>>%jsonTempPath%
        if not %line3%== NULL echo %line3%>>%jsonTempPath%
        if not %line4%== NULL echo %line4%>>%jsonTempPath%
        if not %line5%== NULL echo %line5%>>%jsonTempPath%
        if not %line6%== NULL echo %line6%>>%jsonTempPath%
        if not %line7%== NULL echo %line7%>>%jsonTempPath%
        if not %line8%== NULL echo %line8%>>%jsonTempPath%
        if not %line9%== NULL echo %line9%>>%jsonTempPath%
        if not %line10%== NULL echo %line10%>>%jsonTempPath%
    )
    if %show%==0 (
        if not x%str1:[=%==x%str1% SET show=1
    )
    EXIT /B


:GETLINES
    if %show%==9 EXIT /B
    SET str1=%1    
    SET /a lineNumber+=1
    if not x%str1:],=%==x%str1% SET show=9
    if not x%str1:],=%==x%str1% SET lineNumber=100
    if %lineNumber%==5 SET line1=%~1
    if %lineNumber%==6 SET line2=%~1
    if %lineNumber%==7 SET line3=%~1
    if %lineNumber%==8 SET line4=%~1
    if %lineNumber%==9 SET line5=%~1
    if %lineNumber%==10 SET line6=%~1
    if %lineNumber%==11 SET line7=%~1
    if %lineNumber%==12 SET line8=%~1
    if %lineNumber%==13 SET line9=%~1
    if %lineNumber%==14 SET line10=%~1
    if not x%str1:[=%==x%str1% SET show=1
    if %show%==9 EXIT /B

    EXIT /B

:KEEPORDELETE
    echo %~1
    SET deleteLine=0
    echo 1 to delete; 2 to keep
    choice /n /c:12 /M "Choose and option (1 or 2)"
    IF %ERRORLEVEL%==1 SET deleteLine=%lineNumber%
    IF %ERRORLEVEL%==2 SET deleteLine=0
    if %deleteLine%==0 echo %~1>>%filePath%
    EXIT /B


:OUTPUT 
    SET str1=%1
    SET /a lineNumber+=1    
    if not x%str1:./AppSpecFlow/AppSteps/Steps.cs=%==x%str1% SET show=0
    if %show%==1 GOTO KEEPORDELETE %1
    if %show%==0 echo %~1>>%filePath%
    if not x%str1:./Generic/Steps/Elements/*/*.cs=%==x%str1% SET show=1
    EXIT /B