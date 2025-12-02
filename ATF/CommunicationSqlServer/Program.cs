// Step 5: Write Code to Connect to SQL Server
using System;
using System.Data.SqlClient;
using System.IO; // Include for StreamWriter

namespace SqlServerCommunication
{
    class Program
    {
        // Arguments:
        // args[0] - SQL Statement to execute
        // args[1] - licence (not used in this example, but can be used for logging or other purposes)
        // args[2] - Output file path (CSV)
        // Example usage:
        // dotnet run --project ./CommunicationSqlServer/SqlServerCommunication.csproj -- "SELECT * FROM [ACCOUNT]" "output.csv"
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("ERROR - Please provide a SQL statement and licence to execute.");
                return;
            }

            if (args.Length > 3)
            {
                Console.WriteLine("ERROR - Too many arguments provided. Please provide only the SQL statement, licence, and optionally the output file path.");
                return;
            }

            string sqlStatement = args[0]; // SQL Statement to execute
            // Check if the SQL statement is empty
            if (string.IsNullOrWhiteSpace(sqlStatement))
            {
                Console.WriteLine("ERROR - SQL statement is empty. Please provide a valid SQL statement.");
                return;
            }

            string licence = args[1]; // Licence
            // if (string.IsNullOrWhiteSpace(licence))
            // {
            //     Console.WriteLine("Licence is empty. Please provide a valid licence.");
            //     return;
            // }
            // if (licence.ToLower() != "mark")
            // {
            //     Console.WriteLine("Licence must be 'true' or 'false'.");
            //     return;
            // }

            // Check if the output file path is provided    
            string outputFilePath = ""; // Default output file path
            if (args.Length < 3)
            {
                Console.WriteLine("ERROR - Need minimum 3 arguments: SQL Statement, licence, and optional output file path.");
            }
            else
            {
                outputFilePath = args[2]; // Use the provided output file path
            }
            if (outputFilePath.ToLower() == "default")
            {
                outputFilePath = "./CommunicationSqlServer/Export/output.json"; // Default output file path
            }


            string? password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD");
            string? userName = Environment.GetEnvironmentVariable("SQLUSER");
            string? server = Environment.GetEnvironmentVariable("SQLSERVER");
            string? database = Environment.GetEnvironmentVariable("SQLDATABASE");
            if (password != null) password = password.Trim('"');
            if (userName != null) userName = userName.Trim('"');
            if (server != null) server = server.Trim('"');
            if (database != null) database = database.Trim('"');

            // Console.WriteLine($"Using SQL Server credentials");
            if (password == null || userName == null || server == null || database == null)
            {
                Console.WriteLine("ERROR - SQL Server credentials are not set. Please set the environment variables SQLSERVER_PASSWORD, SQLSERVER_USERNAME, SQLSERVER_SERVER, and SQLSERVER_DATABASE.");
                return;
            }
            // Console.WriteLine("SQL Server credentials are set. Proceeding with connection...");
            string connectionString = $"Server={server};Database={database};User Id={userName};Password={password};";
            // Console.WriteLine($"Connecting to SQL Server at {server}...");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Console.WriteLine("Connected to SQL Server successfully.");
                try
                {
                    using (SqlCommand command = new SqlCommand(args[0], connection))
                    {
                        // Execute the command and read the results
                        command.CommandTimeout = 60; // Set command timeout to 60 seconds
                        // Console.WriteLine($"Executing command: {args[0]}");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Console.WriteLine("Command executed successfully. Writing results to CSV...");
                            while (reader.Read())
                            {
                                // Assuming you want to write all columns in the reader to the CSV
                                var rowData = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var value = reader[i];
                                    if (value == null) value = ""; // Handle null values
                                    rowData.Add(reader[i]?.ToString() ?? string.Empty);
                                }
                                var output = string.Join(",", rowData) ?? string.Empty;
                                // Write to the output file
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(outputFilePath))
                                    {
                                        using (StreamWriter writer = new StreamWriter(outputFilePath, true))
                                        {
                                            writer.WriteLine(output);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"ERROR - Failed to write to output file: {ex.Message}");
                                    return;
                                }
                                // print to console for direct feedback
                                Console.WriteLine(string.Join(",", rowData));
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"ERROR - SQL Exception occurred: {ex.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - An unexpected error occurred: {ex.Message}");
                    return;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}