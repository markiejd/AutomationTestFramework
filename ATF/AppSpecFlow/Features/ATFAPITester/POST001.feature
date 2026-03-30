@all  @POST001 @ATFAPI @SKIP @ATFAPI @NA @ATFAPIPOST001 
Feature: POST001 SimplePOST 
     Simple test POSTing information 
     . 
          dotnet test --filter:"TestCategory=POST001" --logger "trx;logfilename=POST001.trx" 
  
Scenario Outline: POST001-SimplePOST 
     When I "POST" The "<URL>/login" Endpoint With The Payload "{'name':'Mocky McMockface'}"
     Then I Receive A "201" Status Code
