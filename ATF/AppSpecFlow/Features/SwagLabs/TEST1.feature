@all  @TEST1 @TEST6 @SKIP @TEST3 @TEST7 @TEST3TEST1 
Feature: TEST1 TEST2 
     TEST4 
     TEST5 
          
  
Scenario Outline: TEST1-TEST2 
#    Given Browser Is Open
#    When I Navigate To "https://www.saucedemo.com/"
#    Then Page "Login" Is Displayed
#    Then Wait 3 Seconds

   When I Query Audio File "C:\Temp\Recording.m4a"
   Then Audio File Is Valid
   Then Audio File Metadata "fileName" Is Equal To "Recording.m4a"

   When I Compare Audio Files "C:\Temp\Recording.m4a" And "C:\Temp\Recording - Copy.m4a" They Are Identical
   When I Compare Audio Files "C:\Temp\Recording.m4a" And "C:\Temp\Recording - Copy.m4a" They Are Not The Same
   
   When I Compare Audio Files "C:\Temp\Recording.m4a" And "C:\Temp\Recording (2).m4a" They Are Identical
   When I Compare Audio Files "C:\Temp\Recording.m4a" And "C:\Temp\Recording (2).m4a" They Are Not The Same

