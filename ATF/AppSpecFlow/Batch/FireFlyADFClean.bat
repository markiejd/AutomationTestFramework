:: setup-and-run.bat
@echo off

cd..
dotnet test --no-build --filter:"TestCategory=ClearADF" --logger "trx;logfilename=ClearADF.trx" 

