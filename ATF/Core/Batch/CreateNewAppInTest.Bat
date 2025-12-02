set arg1=%1
echo Setting New App in Test As = %arg1%

@REM Apps should have AppSteps.
set appSteps=%arg1%Steps
set appStepsDirectory=%cd%\AppSpecFlow\AppSteps\App\
echo we going to do %appStepsDirectory%%appSteps%.cs
set fullDirectory=%appStepsDirectory%%arg1%
mkdir %fullDirectory%
set fileName=%fullDirectory%\%appSteps%.cs

(
echo using Generic.Steps;
echo using Generic.Steps.Helpers.Interfaces;
echo.     
echo namespace AppSpecFlow.AppSteps
echo {
echo    [Binding]
echo    public class %appSteps% : StepsBase
echo    {
echo        public %appSteps%^(IStepHelpers helpers,
echo            GivenSteps givenSteps,
echo            WhenSteps whenSteps,
echo            ThenSteps thenSteps^) : base^(helpers^)
echo        {
echo            GivenSteps = givenSteps;
echo            WhenSteps = whenSteps;
echo            ThenSteps = thenSteps;
echo        }
echo        private GivenSteps GivenSteps { get; }
echo        private WhenSteps WhenSteps { get; }
echo        private ThenSteps ThenSteps { get; }
echo    }
echo }
) > "%fileName%"


@REM Apps should have Code.
set appCodeDirectory=%cd%\AppSpecFlow\AppSteps\Code\%arg1%\
mkdir %appCodeDirectory%
set fileCodeName=%appCodeDirectory%\%arg1%Code.cs
(
echo namespace AppSpecFlow.AppSteps
echo {
echo    public class %arg1%
echo    {
echo        public bool Failure^(string message, bool pass = false^)
echo        {
echo            return pass;
echo        }
echo    }
echo }
) > "%fileCodeName%"


@REM Apps should have Feature Files.
set appFeaureDirectory=%cd%\AppSpecFlow\Features\%arg1%\
mkdir %appFeaureDirectory%
set fileFeatureName=%appFeaureDirectory%\%arg1%-Starter.feature
(
echo @all @%arg1%
echo.
echo Feature: %arg1%-Dummy
echo    A simple non executable dummy feature Files
echo    Just creates the Directory
echo        dotnet test --filter:"TestCategory=%arg1%" --logger "trx;logfilename=%arg1%.trx" 
echo.
echo Scenario Outline: %arg1%-Starter
echo    Given Test Passes
) > "%fileFeatureName%"


@REM Apps should have Page Object Models.
set appPOMDirectory=%cd%\AppTargets\Forms\%arg1%\
mkdir %appPOMDirectory%
set pageName=%arg1%StarterPage
set filePOMName=%appPOMDirectory%\%pageName%.cs
(
echo using Core; 
echo using OpenQA.Selenium;
echo.
echo    namespace AppTargets.Forms 
echo    {
echo        public class %pageName% : FormBase  
echo        {
echo            public %pageName%^(^) : base^(By.Id^("%pageName%"^), "%pageName% page"^) 
echo            {
echo                /// Add Elements - Element Idenifiers are LOWER case ^(apart from ID^) 
echo                ////
echo                Elements.Add^("ID", By.Id^("ENTERIDHERE"^)^);
echo            }
echo        }
echo    }
) > "%filePOMName%"


@REM Apps should have Test Pre Condition Data.
set appTPCDDirectory=%cd%\AppSpecFlow\TestPreConditionData\%arg1%\
mkdir %appTPCDDirectory%
set pageName=%arg1%EMPTY
set fileRPCDName=%appTPCDDirectory%\%pageName%.txt
(
echo This is here JUST to make the dir
) > "%fileRPCDName%"
