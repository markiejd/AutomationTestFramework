
set formsDirectory="%cd%\Core\Forms\"
set helpersDirectory="%cd%\Generic\Steps\Helpers\Elements"

if exist "%formsDirectory%\Form.selenium" (
    echo ALREADY Playwright
    pause
    goto :eof
)

move %formsDirectory%\Form.cs %formsDirectory%\Form.selenium
move %formsDirectory%\FormBase.cs %formsDirectory%\FormBase.selenium
move %formsDirectory%\Form.playwright %formsDirectory%\Form.cs
move %formsDirectory%\FormBase.playwright %formsDirectory%\FormBase.cs

if exist "%helpersDirectory%\Selenium\ElementInteraction.selenium" (
    echo ALREADY Playwright but we got to helpers dir?
    pause
    goto :eof
)

echo switching off selenium MOVE %helpersDirectory%\Selenium
move %helpersDirectory%\Selenium\ElementInteraction.cs %helpersDirectory%\Selenium\ElementInteraction.selenium
move %helpersDirectory%\Selenium\GetDictionaryLocator.cs %helpersDirectory%\Selenium\GetDictionaryLocator.selenium
move %helpersDirectory%\Selenium\GetSubLocators.cs %helpersDirectory%\Selenium\GetSubLocators.selenium
move %helpersDirectory%\Selenium\GetTheElement.cs %helpersDirectory%\Selenium\GetTheElement.selenium
move %helpersDirectory%\Selenium\GetTheElements.cs %helpersDirectory%\Selenium\GetTheElements.selenium
move %helpersDirectory%\Selenium\LocatorValues.cs %helpersDirectory%\Selenium\LocatorValues.selenium
move %helpersDirectory%\Selenium\SelfHeal.cs %helpersDirectory%\Selenium\SelfHeal.selenium


echo switching on playwright MOVE %helpersDirectory%\Playwright
move %helpersDirectory%\Playwright\ElementInteraction.playwright %helpersDirectory%\Playwright\ElementInteraction.cs
move %helpersDirectory%\Playwright\GetDictionaryLocator.playwright %helpersDirectory%\Playwright\GetDictionaryLocator.cs
move %helpersDirectory%\Playwright\GetSubLocators.playwright %helpersDirectory%\Playwright\GetSubLocators.cs
move %helpersDirectory%\Playwright\GetTheElement.playwright %helpersDirectory%\Playwright\GetTheElement.cs
move %helpersDirectory%\Playwright\GetTheElements.playwright %helpersDirectory%\Playwright\GetTheElements.cs
move %helpersDirectory%\Playwright\LocatorValues.playwright %helpersDirectory%\Playwright\LocatorValues.cs
move %helpersDirectory%\Playwright\SelfHeal.playwright %helpersDirectory%\Playwright\SelfHeal.cs

