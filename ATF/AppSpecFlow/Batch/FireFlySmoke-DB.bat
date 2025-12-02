:: setup-and-run.bat
@echo off

cd..
dotnet test --no-build --filter:"TestCategory=Smoke-DB" --logger "trx;logfilename=Smoke-BD.trx"

