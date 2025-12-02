set arg1=%1
echo in retrieve Features with %arg1%

set destination="%cd%\AppSpecFlow\Features\"
set direcotryToMove="%cd%\AppTargets\RetiredApps\%arg1%\Features\%arg1%"

if exist "%direcotryToMove%" (    
    echo MOVING %direcotryToMove% to %destination%
    if not exist "%directoryToMove%" echo nothing there
    if not exist "%destination%" mkdir %destination%
    move %direcotryToMove% %destination%  
) else (echo NO Features To Move)