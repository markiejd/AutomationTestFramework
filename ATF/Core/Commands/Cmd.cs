using Core.Logging;
using System.Diagnostics;
using System.Text;

namespace Core
{
    public static class CmdUtil
    {
        
        public static int RunPowerShell(string ps1File, string psArguments)
        {
            // powershell.exe -NoProfile -ExecutionPolicy ByPass -File "D:\EIDPNDExtract\LIVE\PNDExtract_and_Transform.ps1" PNDEXTRACT "2022-01-25T01:01:01Z" "2024-01-25T01:01:01Z" "Master-PND" "Person,Vehicle,Organisation,Cyber,Communication,Account,Firearm,Location" "Atlas CM-PND,Atlas KB-PND"
            var command = "powershell.exe -NoProfile -ExecutionPolicy ByPass -File" + " \"" + ps1File + "\"" + psArguments;
            DebugOutput.Log($"Command is {command} ");
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;            

            var process = Process.Start(processInfo);
            if (process != null)
            {
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                    DebugOutput.Log("output>>" + e.Data);
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                    DebugOutput.Log("error>>" + e.Data);
                process.BeginErrorReadLine();

                process.WaitForExit();

                var exitCode = process.ExitCode;

                DebugOutput.Log($"ExitCode: {0} {exitCode}");
                process.Close();
                return exitCode;
            }
            return -1;            
        }

        public static string ExecuteDotnet(string projectPath, string argument)
        {
            var fileName = "dotnet";
            var arguments = "run --project " + projectPath + " -- " + argument;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            DebugOutput.Log($"THIS IS THE DOTNET COMMAND = {fileName} {arguments}");

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

            using (Process process = Process.Start(startInfo))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.

                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    process.WaitForExit();
                    return result;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        }

        ///
        /// // dotnet test --filter:"TestCategory=Jira-0000" --logger "trx;logfilename=Jira-0000.trx" 
        ///
        public static string ExecuteDotnet(string dotnetType, string filter = "", string logger = "")
        {            
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "test --no-build --filter:\"TestCategory=Jira-0000\" --logger \"trx;logfilename=Jira-0000.trx\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }


        public static int ExecuteCommand (string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);
            if (process != null)
            {
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                    DebugOutput.Log("output>>" + e.Data);
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                    DebugOutput.Log("error>>" + e.Data);
                process.BeginErrorReadLine();

                process.WaitForExit();

                var exitCode = process.ExitCode;

                DebugOutput.Log($"ExitCode: {0} {exitCode}");
                process.Close();
                return exitCode;
            }
            return -1;
        }

        public static string? ExecuteCommandGetOutput (string command)
       {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var output = new StringBuilder();

            using (var process = Process.Start(processInfo))
            {
                if (process != null)
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.BeginOutputReadLine();

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    DebugOutput.Log($"Error ExitCode: {process.ExitCode}");
                }
            }
            return output.ToString();
        }



    }
}