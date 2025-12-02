
set formsDirectory="%cd%\Core\Forms"
set helpersDirectory="%cd%\Generic\Steps\Helpers\Elements"

if exist "%formsDirectory%\Form.playwright" (
    echo ALREADY SELENIUM
    pause
    goto :eof
)

move %formsDirectory%\Form.cs %formsDirectory%\Form.playwright
move %formsDirectory%\FormBase.cs %formsDirectory%\FormBase.playwright
move %formsDirectory%\Form.selenium %formsDirectory%\Form.cs
move %formsDirectory%\FormBase.selenium %formsDirectory%\FormBase.cs

if exist "%helpersDirectory%\Playwright\ElementInteraction.playwright" (
    echo ALREADY SELENIUM but we got to helpers dir?
    pause
    goto :eof
)

echo switching off playwright MOVE %helpersDirectory%\Playwright
move %helpersDirectory%\Playwright\ElementInteraction.cs %helpersDirectory%\Playwright\ElementInteraction.playwright
move %helpersDirectory%\Playwright\GetDictionaryLocator.cs %helpersDirectory%\Playwright\GetDictionaryLocator.playwright
move %helpersDirectory%\Playwright\GetSubLocators.cs %helpersDirectory%\Playwright\GetSubLocators.playwright
move %helpersDirectory%\Playwright\GetTheElement.cs %helpersDirectory%\Playwright\GetTheElement.playwright
move %helpersDirectory%\Playwright\GetTheElements.cs %helpersDirectory%\Playwright\GetTheElements.playwright
move %helpersDirectory%\Playwright\LocatorValues.cs %helpersDirectory%\Playwright\LocatorValues.playwright
move %helpersDirectory%\Playwright\SelfHeal.cs %helpersDirectory%\Playwright\SelfHeal.playwright


echo switching on selenium MOVE %helpersDirectory%\Selenium
move %helpersDirectory%\Selenium\ElementInteraction.selenium %helpersDirectory%\Selenium\ElementInteraction.cs
move %helpersDirectory%\Selenium\GetDictionaryLocator.selenium %helpersDirectory%\Selenium\GetDictionaryLocator.cs
move %helpersDirectory%\Selenium\GetSubLocators.selenium %helpersDirectory%\Selenium\GetSubLocators.cs
move %helpersDirectory%\Selenium\GetTheElement.selenium %helpersDirectory%\Selenium\GetTheElement.cs
move %helpersDirectory%\Selenium\GetTheElements.selenium %helpersDirectory%\Selenium\GetTheElements.cs
move %helpersDirectory%\Selenium\LocatorValues.selenium %helpersDirectory%\Selenium\LocatorValues.cs
move %helpersDirectory%\Selenium\SelfHeal.selenium %helpersDirectory%\Selenium\SelfHeal.cs

pause