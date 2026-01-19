@all  @TEST1 @TEST6 @SKIP @TEST3 @TEST7 @TEST3TEST1 
Feature: TEST1 TEST2 
     TEST4 
     TEST5 
          
  
Scenario Outline: TEST1-TEST2 
   Given Browser Is Open
   When I Navigate To "https://www.saucedemo.com/"
   Then Page "Login" Is Displayed
   Then Wait 3 Seconds

