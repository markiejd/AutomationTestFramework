#  SQL Client Interface
The ability to send SQL Commands to a MS SQL Server and return json or csvs of the data model returned.

## Requirements to build
dotnet add package System.Data.SqlClient

## Must have the following Environment Variables set (or change to secrets)
    password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD");
    userName = Environment.GetEnvironmentVariable("SQLSERVER_USERNAME");
    server = Environment.GetEnvironmentVariable("SQLSERVER_SERVER");
    database = Environment.GetEnvironmentVariable("SQLSERVER_DATABASE");

## To Execute
dotnet run "<SQL STATEMENT>" "<Json Output>" "<licence>"

## To Execute from Other DotNet Project
dotnet run --project <location of this file /SqlServerCommunication.csproj> -- "<SQL STATEMENT>" "<licence>" "<outputlocation>"

### Examples
#### No Output file given,  only display output
dotnet run --project ./CommunicationSqlServer/SqlServerCommunication.csproj -- "select * from [ACCOUNT]" "<licence>" ""
#### Specific Outfile file given, and display
dotnet run --project ./CommunicationSqlServer/SqlServerCommunication.csproj -- "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ACCOUNT';" "<licence>" "c:\tmp\output.csv"
#### Output to default directory
dotnet run --project ./CommunicationSqlServer/SqlServerCommunication.csproj -- "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ACCOUNT';" "<licence>" "default"



### Note
You need quotes around the SQL Statement, as it will contain spaces... 
You need quotes around the licence, as it too could contain spaces...
You need quotes around the directory (or empty quotes) as the / can cause issues