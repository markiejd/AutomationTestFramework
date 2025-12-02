cls
@ECHO MUST ANSWER ALL QUESTIONS!!!!!

set seleniumFile=%cd%\Generic\Steps\Helpers\Elements\Selenium\LocatorValues.cs
set playwrightFile=%cd%\Generic\Steps\Helpers\Elements\Playwright\LocatorValues.cs
@ECHO seleniumFile = %seleniumFile%
@ECHO playwrightFile = %playwrightFile%

if exist "%seleniumFile%" (
    @ECHO SELENIUM
    call .\Core\Batch\PageFile\CreatePageFileSelenium.Bat
    GOTO :EOF
)

if exist "%playwrightFile%" (
    @ECHO PLAYWRIGHT
    call .\Core\Batch\PageFile\CreatePageFilePlaywright.Bat
    GOTO :EOF
)


echo PAGE FILE HAS BEEN CREATED make sure it is in the right location!
echo REMEMBER TO ADD THE DEFAULT PAGE LOCATOR FOR THE ID!