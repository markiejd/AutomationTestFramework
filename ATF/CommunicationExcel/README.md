# CommunicationExcel

## Overview
`CommunicationExcel` is a standalone .NET application designed to process Excel files. It can be run as an executable and accepts a file path as a command-line argument.

## Usage
To run the application, use the following command:
```bash
CommunicationExcel.exe <filePath> true/false <worksheetname>
```

filePath - Required - The filePath should include the name of the file.  Will work with relative path (assume path is ./CommunicationExcel/)
true/false - Required - Does the excel sheet have a header or use just column numbers
worksheetname - NOT required - What sheet in Excel to use, if not supplied will use first sheet.

## To Execute from Other DotNet Project
dotnet run --project <location of this file /SqlServerCommunication.csproj> --<action> <filePath> true/false <worksheetname>

#### `readall`
- **Description**: Takes every row in the default first sheet of an excel document and converts to Json
- **Parameters**: `<path and file name> <header row?>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readall c:\tmp\TEST1.xlsx true
  dotnet run readall "C:/REPO/Firefly/FireFly_Testing/ATF/AppSpecFlow/TestPreConditionData/FireFly/DataModel/Data_Dictionary.xlsx" true
  ```

#### `readall` specific worksheet
- **Description**: Takes every row in a specific worksheet of an excel document and converts to Json
- **Parameters**: `<path and file name> <header row?> <worksheet name>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readall c:\tmp\TEST1.xlsx true Mark1
  ```
  
#### `readrow`
- **Description**: Takes a single given row in the default first sheet of an excel document and converts to Json
- **Parameters**: `<path and file name> <header row?> <worksheet name>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readrow c:\tmp\TEST1.xlsx true 2
  ```

#### `readrow` specific worksheet
- **Description**: Takes a single given row in a specific worksheet of an excel document and converts to Json
- **Parameters**: `<path and file name> <header row?> <worksheet name>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readrow c:\tmp\TEST1.xlsx true 2
  ```

#### `readrows`
- **Description**: Takes a group of rows (inclusive) the default first sheet of an excel document and converts to Json
- **Parameters**: `<path and file name> <header row?> <worksheet name>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readrows c:\tmp\TEST1.xlsx true 2 3
  ```

#### `readrows` specific worksheet
- **Description**: Takes a group of rows (inclusive) in a specific worksheet of an excel document and converts to Json
- **Parameters**: `<path and file name> <header row?> <worksheet name>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readrows c:\tmp\TEST1.xlsx true 2 3 Mark1
  ```

#### 'readcell' specific cell in a specific worksheet
- **Description**: returns the value of a cell in a worksheet
- **Parameters**: `<path and file name> <sheet> <cell>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj readcell "c:\tmp\TEST1.xlsx" "meter" "A2"
  ```
GetSheetNames

#### 'getsheets' returns all the worksheets in excel
- **Description**: returns a json of all the worksheet names
- **Parameters**: `<path and file name>
- **Example**:
  ```bash
  dotnet run --project ./CommunicationExcel.csproj getsheets "c:\tmp\TEST1.xlsx" 
  ```



