@ECHO EFR Create BATCH job

cd
dotnet build
   START dotnet test --no-build --filter:"TestCategory=BaseLine-001"  --logger "trx;logfilename=BaseLine-001.trx"
   START dotnet test --no-build --filter:"TestCategory=BaseLine-002"  --logger "trx;logfilename=BaseLine-002.trx"
   START dotnet test --no-build --filter:"TestCategory=BaseLine-003"  --logger "trx;logfilename=BaseLine-003.trx"
   START dotnet test --no-build --filter:"TestCategory=BaseLine-004"  --logger "trx;logfilename=BaseLine-004.trx"
   START dotnet test --no-build --filter:"TestCategory=BaseLine-005"  --logger "trx;logfilename=BaseLine-005.trx"
   START dotnet test --no-build --filter:"TestCategory=BaseLine-006"  --logger "trx;logfilename=BaseLine-006.trx"

pause
