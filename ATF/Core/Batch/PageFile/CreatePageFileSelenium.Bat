
@echo off
set "appName="
set /p appName=What Is The Application this page is from? (App Name)?     
set "pageName="
set /p pageName=What Is unique name of this page? (Non Spaced Pascal Case)? Do Not Include Application name or page!     

set file = .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
Echo %file%

md .\AppTargets\Forms\%appName%\


@echo off
echo using Core; > .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo using OpenQA.Selenium; >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo.  >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo    namespace AppTargets.Forms >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo    { >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo        public class %appName%%pageName%Page : FormBase >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo        { >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo            public %appName%%pageName%Page() : base(By.Id("%appName%%pageName%"), "%appName%%pageName% page") >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo            { >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo                /// Add Elements - Element Idenifiers are LOWER case (apart from ID) >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo                /// >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo                Elements.Add("ID", By.Id("UNKNOWN")); >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo.  >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo            } >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo        } >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs
echo    } >> .\AppTargets\Forms\%appName%\%appName%%pageName%Page.cs