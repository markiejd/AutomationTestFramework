cls
@echo off
del .\AppSpecFlow\TestOutput\*.png
del .\AppSpecFlow\TestOutput\PageImages\*.png
del .\AppXAPI\APIOutFiles\*.json
del /S .\AppSpecFlow\Features\*.cs
del .\AppSpecFlow\TestResults\*.trx
del .\AppSpecFlow\TestResults\*.html
del .\AppSpecFlow\TestResults\*.json
del .\AppSpecFlow\TestResults\*ImageWarnings.txt
RD /S /Q .\AppSpecFlow\TestResults\

echo ALL Details Cleaned!
echo Don't worry if you get "Could Not Find" can't tidy what does not exist!