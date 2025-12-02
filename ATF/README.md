# Project Huxley Read Me
You need .Net 9.x SDK Installed

## Post Deployment
Devs have pushed a deployment onto a new environment

<u>Make sure you are connected to CW Network via VPN!</u>

1. Inform the team you are checking the deloyment (state what environment you are checking, allow others to check other environments at the same time - speed up this process)
2. Confirm you can connect to the web front end for that environment
3. Confirm you can connect to the SQL Server for that environment
4. Configure ATF for that environment
4.1. Setting the local temp variables in TERMINAL
4.1.01. SET SQLSERVER=""
4.1.02. SET SQLDATABASE=""
4.1.03. SET SQLUSER=""
4.1.04. SET SQLSERVER_PASSWORD=""
4.1.05. SET ATFENVIRONMENT=""
4.1.06. SET AZURE_STORAGE_ACCOUNT_NAME=""
4.1.07. SET AZURE_STORAGE_ACCOUNT_KEY=""
4.1.08. SET AZURE_STORAGE_FILESYSTEM=""

These will be cleared from memory when you close down ATF VSC

5. In the TERMINAL Execute
dotnet build
dotnet test --no-build --filter:"TestCategory=Smoke-DB" --logger "trx;logfilename=Smoke-BD.trx" 
dotnet test --no-build --filter:"TestCategory=Smoke-UI" --logger "trx;logfilename=Smoke-UI.trx"

dotnet test --no-build --filter:"TestCategory=ClearADF" --logger "trx;logfilename=ClearADF.trx" 

This will run all the Database Smoke tests - including database checks.  This is your simple sanity checks the database has (or has not) changed
<u>If failure</u>
5.1. Inform team (if more than one environment)
5.2. Find out if changes by design
<u>If By Design</u>
5.2.1. Inform team
5.2.2. Create Branch
5.2.3. Update these DB Tests
5.2.4. Merge and Inform Team to pull Main and start again
<u>If NOT By Design</u>
5.2.5. Inform DBs!


6. In the TERMINAL Execute
dotnet test --filter:"TestCategory=PopulateDefaultDB" --logger "trx;logfilename=PopulateDefaultDB.trx"

This will populate the database with our test data set *NOTE this may fail due to defects in the placement of files POST import (you may need to comment out the checks for now)

7. Update the table in the Teams channcel. 
8. In the TERMINAL Execute
dotnet test --filter:"TestCategory=SmokeUI" --logger "trx;logfilename=SmokeUI.trx"

This will run all the Smoke UI Tests - just look and feel - functionality WITHOUT any data changes (i.e. the database exists and in the state we expecte)

<u>If failure</u>
8.1. Inform team (if more than one environment)
8.2. Rerun failed tests, kicking off manually
<u>If STILL failure</u>
8.3. Is the failures by design (i.e. something has changed in this release)
<u>If By Design</u>
8.4. Allocate resources to fix these as priority! (regression > new feature testing)
<u>Changes merged in</u>
8.4.1. Rerun SmokeUI - until Green.
<u>Green</u>
9.0. Regression is done - inform others!


## Nightly Regression
dotnet test --filter:"TestCategory=Green" --logger "trx;logfilename=Green.trx"
Will run every test we expect to be GREEN!
Any failures here are treated as priority the next morning (by design / not by design)

