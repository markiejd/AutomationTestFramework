@all  @POST001 @ATFAPI @SKIP @ATFAPI @NA @ATFAPIPOST001 
Feature: POST001 SimplePOST 
     Simple test POSTing information 
     . 
          dotnet test --filter:"TestCategory=POST001" --logger "trx;logfilename=POST001.trx" 
  
Scenario Outline: POST001-SimplePOST 
     When I "POST" Payload "{'additionalProp1':'Prop1','additionalProp2':'Prop2','additionalProp3':'Prop3'}" At The "<URL>/login" Endpoint
     Then I Receive A "200" Status Code
