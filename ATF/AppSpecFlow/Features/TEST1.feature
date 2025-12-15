@all  @TEST1 @TEST6 @SKIP @TEST3 @TEST7 @TEST3TEST1 
Feature: TEST1 TEST2 
     TEST4 
     TEST5 
          dotnet test --filter:"TestCategory=TEST1" --logger "trx;logfilename=TEST1.trx" 
  
Scenario Outline: TEST1-TEST2 
   Given Browser Is Open
