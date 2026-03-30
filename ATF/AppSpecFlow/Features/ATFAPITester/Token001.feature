@all  @Token001 @NA @SKIP @ATFAPI @NA @ATFAPIToken001 
Feature: Token001 GetToken 
     Let us confirm we get a token! 
     . 
          dotnet test --filter:"TestCategory=Token001" --logger "trx;logfilename=Token001.trx" 
  
Scenario Outline: Token001-GetToken 
   Given JWT Token Created Using Payload "{'username': 'string',  'roles': [    'string'  ]}" At URL "<URL>/examples/auth/login"
