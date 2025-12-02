
set arg1=%1
echo in retire %arg1%
CALL .\Core\Batch\Retire\RetireAppSteps.bat %arg1%
CALL .\Core\Batch\Retire\RetireCode.bat %arg1%
CALL .\Core\Batch\Retire\RetireFeatures.bat %arg1%
CALL .\Core\Batch\Retire\RetirepageObjects.bat %arg1%
CALL .\Core\Batch\Retire\RetirePreConditionData.bat %arg1%
echo %arg1% has been retired