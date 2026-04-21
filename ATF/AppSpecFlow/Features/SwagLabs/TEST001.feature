@all  @TEST001 @NA @SKIP @SwagLabs @ATEAM @SwagLabsTEST001 
Feature: TEST001 BrowserNavigation 
     Just open and navigate test 
     . 
          dotnet test --filter:"TestCategory=TEST001" --logger "trx;logfilename=TEST001.trx" 
  
Scenario Outline: TEST001-BrowserNavigation 
   Given Browser Is Open
   When I Navigate To "https://www.saucedemo.com/"
   Then Page "Login" Is Displayed
