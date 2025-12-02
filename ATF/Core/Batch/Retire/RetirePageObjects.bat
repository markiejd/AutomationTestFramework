
set arg1=%1
echo in retire page objects with %arg1%

set direcotryToMove="%cd%\AppTargets\Forms\%arg1%"
set destination="%cd%\AppTargets\RetiredApps\%arg1%\Forms\"

if exist "%direcotryToMove%" (    
    echo MOVING %direcotryToMove% to %destination%
    if not exist "%directoryToMove%" echo nothing there
    if not exist "%destination%" mkdir %destination%
    move %direcotryToMove% %destination% 
    set newExtension=".retired"
    for /r "%destination%" %%f in (*) do (
        ren "%%f" "%%~nf%%~xf%newExtension%"
    )
) else (echo NO App page objects To Move)
