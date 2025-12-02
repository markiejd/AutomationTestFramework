@all  @UnitTesting @NA  @Framework @UnitTesting-UnitTesting
Feature: UnitTesting UnitTesting 
     Unit test our framework 
     . -- NEVER DELETE THIS - - It keeps the directory for GIT
          dotnet test --filter:"TestCategory=UnitTesting" --logger "trx;logfilename=UnitTesting.trx" 
  
Scenario Outline: UnitTesting-UnitTesting 
#    Given Unit Test Is Executed In App
   Given Unit Test Is Executed In Core
#    Given Unit Test Is Executed In Generic
   
