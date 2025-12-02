# Communication ADF

## Overview
this is a standalone dotnet application that allows interaction with ADF Data Lake Storage


## Before Build
dotnet add package Azure.Storage.Files.DataLake
dotnet add package Newtonsoft.Json

## Before Run
You must set your environment variables to use this application
set AZURE_STORAGE_ACCOUNT_NAME=<this is the name of the storage account>
set AZURE_STORAGE_ACCOUNT_KEY=<get the key of the account>
set AZURE_STORAGE_FILESYSTEM=<get the top level of the file system>
^^^^ ATF Sets this from the environment variable file

## Execution 

### List all files
dotnet run -- list <filePath>

### download a file
dotnet run -- download <filePath> <destinationPath>

### upload a file
dotnet run -- upload <filePath> <destinationPath>
dotnet run -- upload c:\tmp\hello.txt "Input/TEIP Invoice data_BL002/hello.txt" 

### delete a file
dotnet run -- delete <filePath>
