
set arg1=%1
echo in retire code with %arg1%

set direcotryToMove="%cd%\AppSpecFlow\AppSteps\Code\%arg1%"
set destination="%cd%\AppTargets\RetiredApps\%arg1%\AppSteps\Code\"

if exist "%direcotryToMove%" (    
    echo MOVING %direcotryToMove% to %destination%
    if not exist "%directoryToMove%" echo nothing there
    if not exist "%destination%" mkdir %destination%
    move %direcotryToMove% %destination% 
    set newExtension=".retired"
    for /r "%destination%" %%f in (*) do (
        ren "%%f" "%%~nf%%~xf%newExtension%"
    )
) else (echo NO App Code To Move)
