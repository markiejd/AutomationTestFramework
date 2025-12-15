using Core;
using Core.Configuration;
using Core.Logging;
using Core.Transformations;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.DevTools;
using Reqnroll;

namespace Generic.Elements.Steps.TSQL
{
    [Binding]
    public class WhenTSQLSteps : StepsBase
    {
        public WhenTSQLSteps(IStepHelpers helpers) : base(helpers)
        {
        }

                
        [When(@"I Store SQL Command ""(.*)""")]
        public void WhenIStoreSQLCommand(string sqlCommand)
        {
            string proc = $"When I Store SQL Command {sqlCommand}";
            if (CombinedSteps.OuputProc(proc))
            {
                var returnedString = Helpers.TSQL.SendSQLCommand(sqlCommand, "", "");
                if (returnedString == null)
                {
                    return;
                }
                DebugOutput.Log($"We have returned '{returnedString}' from the SQL command");
                // if the first 5 chars returned are "Error" then we have an error
                if (returnedString.ToLower().StartsWith("error"))
                {
                    CombinedSteps.Failure($"We have an error returned from the SQL command: {returnedString}");
                    return;
                }
                // I get a comma delimited string back, I need to break that up into an array
                var array = StringValues.ConvertCSVStringToArray(returnedString);
                if (array.Length > 10)
                {
                    CombinedSteps.Failure($"We do not support more than 10 values returned from the SQL command only 1 to 10");
                    return;
                }
                TargetConfiguration.Configuration.ATFVariableArray = array;
                DebugOutput.Log($"We have stored the returned string in TargetConfiguration.Configuration.ATFVariableArray '{TargetConfiguration.Configuration.ATFVariableArray.Length}'");
            }   
        }


        
        [When(@"I Store As Json SQL Command ""(.*)""")]
        public void WhenIStoreAsJsonSQLCommand(string sqlCommand)
        {
            string proc = $"When I Store SQL Command {sqlCommand}";
            if (CombinedSteps.OuputProc(proc))
            {
                var returnedString = Helpers.TSQL.SendSQLCommand(sqlCommand, "", "", true);
                if (returnedString == null)
                {
                    return;
                }
                // if the first 5 chars returned are "Error" then we have an error
                if (returnedString.StartsWith("Error"))
                {
                    CombinedSteps.Failure($"We have an error returned from the SQL command: {returnedString}");
                    return;
                }
                DebugOutput.Log($"{returnedString}");
                return;
            }
        }


        [When(@"I SQL Command (.*)")]
        public void WhenISQLCommand(string sqlCommand)
        {
            string proc = $"When I SQL Command {sqlCommand}";
            if (CombinedSteps.OuputProc(proc))
            {
                var returnedString = Helpers.TSQL.SendSQLCommand(sqlCommand);
                if (returnedString == null)
                {
                    return;
                }
                // if the first 5 chars returned are "Error" then we have an error
                if (returnedString.StartsWith("Error"))
                {
                    CombinedSteps.Failure($"We have an error returned from the SQL command: {returnedString}");
                    return;
                }
                DebugOutput.Log($"{returnedString}");
                return;
            }
        }

        
        [When(@"I SQL Command As CSV .*")]
        public void WhenISQLCommandAsCSV(string sqlCommand)
        {
            sqlCommand.Replace(";", "");
            sqlCommand.Replace("\"", "");
            sqlCommand = "\"" + sqlCommand + " \" c:/tmp/hello1235.csv";
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            DebugOutput.Log($"THE RETURN STRING IS :-");
            DebugOutput.Log($"{returnedString}");
            List<string> lines = new List<string>(returnedString.Split(new[] { "\r\n" }, StringSplitOptions.None));
            DebugOutput.Log($"We have returned {lines.Count()} lines from the command {sqlCommand}");
        }
        
        [When(@"I SQL Command As JSON .*")]
        public void WhenISQLCommandAsJSON(string sqlCommand)
        {
            sqlCommand.Replace(";", "");
            sqlCommand.Replace("\"", "");
            sqlCommand = "\"" + sqlCommand + " FOR JSON AUTO; \" c:/tmp/hello1235.csv";
            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", sqlCommand);
            DebugOutput.Log($"THE RETURN STRING IS :-");
            DebugOutput.Log($"{returnedString}");
            List<string> lines = new List<string>(returnedString.Split(new[] { "\r\n" }, StringSplitOptions.None));
            DebugOutput.Log($"We have returned {lines.Count()} lines from the command {sqlCommand}");
        }

        




    }
}
