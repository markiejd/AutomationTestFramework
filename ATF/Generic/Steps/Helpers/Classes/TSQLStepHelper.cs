using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using Core;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Reqnroll;

namespace Generic.Steps.Helpers.Classes
{
    public class TSQLStepHelper : StepHelper, ITSQLStepHelper
    {
        private readonly ITargetForms targetForms;
        public TSQLStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
        {
            this.targetForms = targetForms;
        }

        private string Stored = "";

        public bool Hello()
        {
            DebugOutput.Log("Hello from TSQLStepHelper");
            DebugOutput.Log($"THE STORED STRING IS :-");
            DebugOutput.Log($"{Stored}");
            return true;
        }


        public string SendSQLCommand(string sqlCommand, string licence = "", string outputFile = "", bool json = false)
        {
            licence = "\"" + licence + "\""; // Ensure licence is always a string, even if empty
            outputFile = "\"" + outputFile + "\""; // Ensure outputFile is always a string
            if (json)
                {
                    sqlCommand = GetSQLCommandAsJson(sqlCommand);
                }
            sqlCommand = GetSQLCommandAsCSV(sqlCommand);
            string arguments = GetCommandSqlServerArguments(sqlCommand, licence, outputFile);

            // // dotnet run --project ./CommunicationSqlServer/SqlServerCommunication.csproj -- "select * from [ACCOUNT]" "<licence>" ""

            var returnedString = CmdUtil.ExecuteDotnet("./CommunicationSqlServer/SqlServerCommunication.csproj", arguments);
            // remove any double returns
            returnedString = returnedString.Replace("\r\n\r\n", "\r\n").Replace("\n\n", "\n").Trim();            
            returnedString = Regex.Replace(returnedString, @"&#xD;", string.Empty);
            returnedString = Regex.Replace(returnedString, @"\r\n", string.Empty);
            returnedString = Regex.Replace(returnedString, @"\n", string.Empty);
            returnedString = returnedString.Trim();
            try
            {
                var parsedJson = JsonSerializer.Deserialize<object>(returnedString);
                Console.WriteLine("JSON parsed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing JSON: " + ex.Message);
            }

            DebugOutput.Log($"THE RETURN STRING IS !!!!! :-");
            DebugOutput.Log($"{returnedString}");
            if (outputFile == "\"\"")
            {
                Stored = returnedString;
            }
            return returnedString;
        }

        /// <summary>
        /// Constructs the command line arguments for executing a SQL command in SQL Server.
        /// This method combines the SQL command, licence, and output file into a single string.
        /// The SQL command is expected to be in a format that can be executed by the SQL Server communication project.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="licence"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        private string GetCommandSqlServerArguments(string sqlCommand, string licence, string outputFile)
        {
            return sqlCommand + " " + licence + " " + outputFile;
        }

        /// <summary>
        /// Converts the SQL command to a JSON format suitable for execution.
        /// This method removes semicolons and quotes, then appends "FOR JSON AUTO"
        /// and wraps the command in quotes.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        private string GetSQLCommandAsJson(string sqlCommand)
        {
            return GetSQLCommandAsCSV(sqlCommand) + " FOR JSON AUTO; ";
        }   

        /// <summary>
        /// Converts the SQL command to a CSV format suitable for execution.
        /// This method removes semicolons and quotes, then wraps the command in quotes.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        private string GetSQLCommandAsCSV(string sqlCommand)
        {
            sqlCommand = sqlCommand.Replace(";", "").Replace("\"", "");
            sqlCommand = "\"" + sqlCommand + " \"";
            return sqlCommand;
        }

    }
}
