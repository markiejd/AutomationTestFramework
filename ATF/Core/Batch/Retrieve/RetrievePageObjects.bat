set arg1=%1
echo in retrieve Page Objects with %arg1%

set destination="%cd%\AppTargets\Forms\"
set direcotryToMove="%cd%\AppTargets\RetiredApps\%arg1%\Forms\%arg1%"

if exist "%direcotryToMove%" (    
    echo MOVING %direcotryToMove% to %destination%
    if not exist "%directoryToMove%" echo nothing there
    if not exist "%destination%" mkdir %destination%
    move %direcotryToMove% %destination%  
) else (echo NO PageObjects To Move)