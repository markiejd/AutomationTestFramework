cd ..
cd ..
cd AppSpecFlow

dotnet test --filter:"TestCategory=EID" --logger "trx;logfilename=EID.trx"

c:\drivers\TrxerConsole.exe D:\Repos\adsp-automation-framework\TestAutomationSpecFlow\AppSpecFlow\TestResults\EID.trx

ren  "D:\Repos\adsp-automation-framework\TestAutomationSpecFlow\AppSpecFlow\TestResults\EID.trx.html" "EID_%time:~0,2%%time:~3,2%-%DATE:/=%.html"