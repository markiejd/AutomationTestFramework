set arg1=%1
echo in retrieve App with %arg1%

set destination="%cd%\AppSpecFlow\AppSteps\Code"
set direcotryToMove="%cd%\AppTargets\RetiredApps\%arg1%\AppSteps\Code\%arg1%"

if exist "%direcotryToMove%" (    
    echo MOVING %direcotryToMove% to %destination%
    if not exist "%directoryToMove%" echo nothing there
    if not exist "%destination%" mkdir %destination%
    move %direcotryToMove% %destination%  
) else (echo NO AppSteps To Move)