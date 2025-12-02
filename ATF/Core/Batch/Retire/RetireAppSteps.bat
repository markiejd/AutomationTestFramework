set arg1=%1
echo in retire with %arg1%

set direcotryToMove="%cd%\AppSpecFlow\AppSteps\App\%arg1%"
set destination="%cd%\AppTargets\RetiredApps\%arg1%\AppSteps\App\"

if exist "%direcotryToMove%" (    
    echo MOVING %direcotryToMove% to %destination%
    if not exist "%directoryToMove%" echo nothing there
    if not exist "%destination%" mkdir %destination%
    move %direcotryToMove% %destination%    
    set newExtension=".retired"
    timeout /t 1 /nobreak >nul
    for /r "%destination%" %%f in (*) do (
        ren "%%f" "%%~nf%%~xf%newExtension%"
    )
) else (echo NO AppSteps To Move)

