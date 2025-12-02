:: setup-and-run.bat
@echo off

cd..
dotnet test --no-build --filter:"TestCategory=Smoke-UI" --logger "trx;logfilename=Smoke-UI.trx"

