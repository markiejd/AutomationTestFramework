set arg1=%1
@echo off
setlocal enabledelayedexpansion

set "oldString=.retired"
set "destination=!cd!\AppTargets\RetiredApps\!arg1!\"

for /r "%destination%" %%f in (*%oldString%) do (
    set "originalString=%%f"
    :: Remove the last 8 chars
    set "newString=!originalString:~0,-8!"

    echo Original String: !originalString!
    echo New String: !newString!
    move "!originalString!" "!newString!"
)

endlocal