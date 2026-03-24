@all  @GET001 @ATFAPI @SKIP @ATFAPITester @NA @ATFAPITesterGET001 
Feature: GET001 SimpleGET 
     Simple test to GET information 
     . 
          dotnet test --filter:"TestCategory=GET001" --logger "trx;logfilename=GET001.trx" 
  
Scenario Outline: GET001-SimpleGET 
   When I "GET" The "http://localhost:4000/users/123" Endpoint 
   Then I Receive A "200" Status Code
   Then The Response Body Is Equal To "{'id':123,'name':'Mocky McMockface'}"
   
   When I "GET" The "http://localhost:4000/users/222" Endpoint 
   Then I Receive A "200" Status Code
   Then The Response Body Is Equal To "{'id':222,'name':'Mocky McMockface'}"
