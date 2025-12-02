

dotnet add package GraphQL.Client
dotnet add package GraphQL.Client.Serializer.Newtonsoft


# CommunicationXrayCloud

This project provides a command-line utility for interacting with the Xray Cloud API. It allows users to perform various operations such as creating test executions, adding tests to executions, updating test runs, and syncing BDD (Behavior-Driven Development) data with Xray Cloud. The utility is implemented in C# and uses GraphQL and REST APIs to communicate with Xray Cloud.

## Features

- **Create Test Executions**: Create new test executions in Xray Cloud.
- **Add Tests to Executions**: Add one or more tests to an existing test execution.
- **Update Test Runs**: Update the status of test runs.
- **Sync BDD Data**: Sync Gherkin/BDD definitions with Xray Cloud.
- **Retrieve Test Data**: Fetch test execution details, test run details, and schema information.
- **Upload Test Results**: Upload test execution results, including evidence for failed tests.

## Prerequisites

- .NET 6.0 SDK installed on your machine.
- An active Xray Cloud account with API access.
- A valid JWT token for authentication with the Xray Cloud API.

## Project Structure

- **`Program.cs`**: The main entry point of the application. Contains the logic for handling command-line arguments and executing the corresponding operations.
- **GraphQL Queries and Mutations**: Defined inline in the methods for interacting with the Xray Cloud API.
- **Helper Methods**: Includes utility methods for encoding files to Base64, sending HTTP requests, and constructing JSON payloads.

## How to Use

### 1. Build the Project

Ensure you have the .NET 6.0 SDK installed. Open a terminal in the project directory and run:

```bash
dotnet build
```

### 2. Execute 
dotnet run --project ./CommunicationXrayCloud.csproj -- <action> <parameters>

### 3. Available Actions

Below is a list of supported actions and their required parameters:

#### `syncbdd`
- **Description**: Sync BDD/Gherkin definitions with Xray Cloud.
- **Parameters**: `<token> <issueId> <bddText>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- syncbdd <token> HUX-1234 "Feature: Example Feature"
  ```

#### `getgherkin`
- **Description**: Retrieve the Gherkin/BDD definition for a test.
- **Parameters**: `<token> <issueId>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- getgherkin <token> HUX-1234
  ```

#### `createtestexecution`
- **Description**: Create a new test execution.
- **Parameters**: `<token> <summary> <description> <projectKey> <testEnvironment>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- createtestexecution <token> "Test Execution" "Description" "PROJ" "Dev" "SubEnv" "10218"
  dotnet run -- createtestexecution "TOKEN" "Test Execution" "Description" "HX25" "HX25-QA"
  ```

#### `addtesttotestexecution`
- **Description**: Add tests to an existing test execution.
- **Parameters**: `<token> <testExecutionId> <testIds>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- addtesttotestexecution <token> HUX-5678 "HUX-1234,HUX-5679"
  ```

#### `gettestrunid`
- **Description**: Get the test run ID for a specific test in a test execution.
- **Parameters**: `<token> <testExecutionId> <testId>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- gettestrunid <token> HUX-5678 HUX-1234
  ```

#### `gettestrunbyid`
- **Description**: Retrieve details of a test run by its ID.
- **Parameters**: `<token> <testRunId>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- gettestrunbyid <token> 12345
  ```

#### `updatetestrun`
- **Description**: Update the status of a test run.
- **Parameters**: `<token> <testRunId> <status>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- updatetestrun <token> 12345 PASSED
  ```

#### `uploadtestexecutionresults`
- **Description**: Upload test execution results, including evidence for failed tests.
- **Parameters**: `<token> <testExecutionKey> <testResults>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- uploadtestexecutionresults <token> HUX-5678 "HUX-1234,PASSED,Comment|HUX-5679,FAILED,FailureComment£Base64Data£FileName£ContentType"
  ```

#### `getschema`
- **Description**: Retrieve the GraphQL schema.
- **Parameters**: `<token>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- getschema <token>
  ```

#### `getschematype`
- **Description**: Retrieve details of a specific GraphQL type.
- **Parameters**: `<token> <type>`
- **Example**:
  ```bash
  dotnet run --project ./CommunicationXrayCloud.csproj -- getschematype <token> Mutation
  ```