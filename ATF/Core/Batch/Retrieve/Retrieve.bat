
set arg1=%1
echo Retrieve %arg1%
CALL .\Core\Batch\Retrieve\UnRetireFiles.bat %arg1%
CALL .\Core\Batch\Retrieve\RetrieveAppSteps.bat %arg1%
CALL .\Core\Batch\Retrieve\RetrieveCode.bat %arg1%
CALL .\Core\Batch\Retrieve\RetrieveFeatures.bat %arg1%
CALL .\Core\Batch\Retrieve\RetrievePageObjects.bat %arg1%
CALL .\Core\Batch\Retrieve\RetrievePreConditionData.bat %arg1%
echo %arg1% has been RETRIEVED!  it is alive!

set destination="%cd%\AppTargets\RetiredApps\%arg1%"
echo %destination%
rmdir /s %destination%