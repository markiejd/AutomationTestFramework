
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.IO;
using System.Text.Json;
using Core.Logging;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualBasic;
using TechTalk.SpecFlow;

namespace Generic.Steps
{
    [Binding]
    public class LoadSteps : StepsBase
    {
        Generic.Steps.Config.ConfigModel configModel = new Generic.Steps.Config.ConfigModel();
        

        public LoadSteps(IStepHelpers helpers) : base(helpers)
        {
        }


        private bool Failed(string proc, string message = "***** FAILURE ******", bool flag = true)
        {
            DebugOutput.Log(message);
            CombinedSteps.Failure(proc);
            return flag;
        }


        [Given(@"Load Json Config Is Set As API ""(.*)"" Verify SSL ""(.*)"" Request ""(.*)"" API KEY ""(.*)"" User Number (.*) Increment Rate (.*) Run Time ""(.*)""")]
        public bool GivenATFJsonConfigIsSetAsAPIVerifySSLRequestAPIKEYUserNumberIncrementRateRunTime(string API, string SSL, string requestName, string APIKey, int userNumber, decimal incrementRate, string runTime)
        {
            string proc = $"Given ATF368 Json Config Is Set As API \"{API}\" Verify SSL \"{SSL}\" Request \"{requestName}\" API KEY \"{APIKey}\" User Number {userNumber} Increment Rate {incrementRate} Run Time \"{runTime}\"";

            if (CombinedSteps.OuputProc(proc))
            {
                DebugOutput.Log($"Creating Model with the following parameters: API: {API} verify_ssl: {SSL} request: {requestName} api_key: {APIKey} user_number: {userNumber} increment_rate: {incrementRate} run_time: {runTime}");
                if (API == null || requestName == null)
                {
                    DebugOutput.Log($"API or requestName is null!");
                    return Failed(proc);
                }
                else
                {
                    configModel.API = API;
                    configModel.verify_ssl = Convert.ToBoolean(SSL);   
                    configModel.request = requestName;
                    configModel.api_key = Convert.ToBoolean(APIKey);
                    configModel.user_number = userNumber;
                    configModel.increment_rate = (int)incrementRate;
                    configModel.run_time = runTime;
                }

                
                DebugOutput.Log($"Model Created : API: {configModel.API} verify_ssl: {configModel.verify_ssl} request: {configModel.request} api_key: {configModel.api_key} user_number: {configModel.user_number} increment_rate: {configModel.increment_rate} run_time: {configModel.run_time}");
                return true;
            }
            return Failed(proc);
        }


        [When(@"I run the locust load test")]
        public async Task<bool> WhenIRunTheLocustLoadTest()
        {
            string proc = $"When I run the locust load test";
            string _repoRoot = Directory.GetCurrentDirectory();
            
            // Paths
            string communicationLoadDir = Path.Combine(_repoRoot, "CommunicationLoad");
            string _testResultsDir = Path.Combine(_repoRoot, "AppSpecFlow", "TestResults", "ATF368");
            Directory.CreateDirectory(_testResultsDir);

            // Prefer a repo-local venv python if present, otherwise fallback to 'python' on PATH
            string pythonExe = Path.Combine(_repoRoot, ".venv", "Scripts", "python.exe");
            if (!File.Exists(pythonExe)) pythonExe = "python";

            string locustFile = Path.Combine(communicationLoadDir, "locustfile.py");
            if (!File.Exists(locustFile)){
                DebugOutput.Log($"locustfile.py not found at expected location: {locustFile}");
                return Failed(proc);
            }

            string htmlReport = Path.Combine(_testResultsDir, "ATF368_report.html");
            string csvPrefix = Path.Combine(_testResultsDir, "ATF368");

            // Decide how long to run locust. Prefer the value from configModel.run_time if present.
            string runTimeString = string.IsNullOrWhiteSpace(configModel.run_time) ? "5s" : configModel.run_time;

            // Build locust args using the selected runtime
            string args = $"-m locust -f \"{locustFile}\" --headless -u \"{configModel.user_number}\" -r \"{configModel.increment_rate}\" -t {runTimeString} --html \"{htmlReport}\" --csv \"{csvPrefix}\" --csv-full-history";

            // Compute timeout from the runtime string + small buffer to allow report generation
            TimeSpan timeout = ParseLocustDurationToTimeSpan(runTimeString) + TimeSpan.FromSeconds(30);

            // If the locust script reads a target_config.json, create/replace it with our configModel so the script will use these values.
            string targetConfigPath = Path.Combine(communicationLoadDir, "config", "target_config.json");
            string backupConfigPath = targetConfigPath + ".bak";

            // Serialize the configModel into JSON compatible with the script's expectations.
            var tempConfig = new
            {
                API = configModel.API,
                verify_ssl = configModel.verify_ssl,
                request = configModel.request,
                api_key = configModel.api_key,
                user_number = configModel.user_number,
                increment_rate = configModel.increment_rate,
                run_time = configModel.run_time
            };

            string serializedConfig = JsonSerializer.Serialize(tempConfig, new JsonSerializerOptions { WriteIndented = true });

            if (File.Exists(targetConfigPath))
            {
                // backup existing config
                File.Copy(targetConfigPath, backupConfigPath, overwrite: true);
            }
            File.WriteAllText(targetConfigPath, serializedConfig);

            // Use the directory that contains the locust file as the working directory
            ProcessResult result;
            try
            {
                result = await RunProcessAsync(pythonExe, args, communicationLoadDir, timeout);
            }
            finally
            {
                // clean up even if RunProcessAsync failed
                try
                {
                    if (File.Exists(backupConfigPath))
                    {
                        File.Copy(backupConfigPath, targetConfigPath, overwrite: true);
                        File.Delete(backupConfigPath);
                    }
                }
                catch (Exception ex)
                {
                    DebugOutput.Log($"Failed to restore original target_config.json: {ex.Message}");
                }
            }

            if (result.ExitCode != 0)
            {
                // Save logs for diagnostics
                File.WriteAllText(Path.Combine(_testResultsDir, "ATF368_stdout.log"), result.StandardOutput);
                File.WriteAllText(Path.Combine(_testResultsDir, "ATF368_stderr.log"), result.StandardError);
                DebugOutput.Log($"ATF368 returned exit code {result.ExitCode}. There are request failures, check ATF368_stderr.log for details.");
                return Failed(proc);

            }

            // Verify that the HTML report was produced
            if (!File.Exists(htmlReport))
            {
                DebugOutput.Log($"ATF368 HTML report not produced: {htmlReport}");
                return Failed(proc);
            }

            return true;
        }

        private async Task<ProcessResult> RunProcessAsync(string fileName, string arguments, string workingDirectory, TimeSpan timeout)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            var outputTcs = new TaskCompletionSource<bool>();
            var errorTcs = new TaskCompletionSource<bool>();
            var exitTcs = new TaskCompletionSource<bool>();

            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null) outputTcs.TrySetResult(true);
                else outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data == null) errorTcs.TrySetResult(true);
                else errorBuilder.AppendLine(e.Data);
            };
            process.Exited += (s, e) => exitTcs.TrySetResult(true);

            if (!process.Start())
                throw new InvalidOperationException("Failed to start process: " + fileName);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using var cts = new CancellationTokenSource(timeout);
            var cancellationToken = cts.Token;

            // Wait for exit or timeout
            var completedTask = await Task.WhenAny(exitTcs.Task, Task.Delay(Timeout.Infinite, cancellationToken));
            if (completedTask != exitTcs.Task)
            {
                try { process.Kill(true); } catch { /* ignore */ }
                throw new TimeoutException($"Process '{fileName}' timed out after {timeout.TotalSeconds} seconds.");
            }

            // Ensure all output events processed
            await Task.WhenAll(outputTcs.Task, errorTcs.Task).ConfigureAwait(false);

            return new ProcessResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = outputBuilder.ToString(),
                StandardError = errorBuilder.ToString()
            };
        }

        
        private static TimeSpan ParseLocustDurationToTimeSpan(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return TimeSpan.FromMinutes(1);

            s = s.Trim().ToLowerInvariant();
            // supports forms: "30s", "120s", "1m", "2m", "1h", "1500ms"
            try
            {
                if (s.EndsWith("ms") && double.TryParse(s[..^2], out var ms))
                    return TimeSpan.FromMilliseconds(ms);
                if (s.EndsWith("s") && double.TryParse(s[..^1], out var sec))
                    return TimeSpan.FromSeconds(sec);
                if (s.EndsWith("m") && double.TryParse(s[..^1], out var min))
                    return TimeSpan.FromMinutes(min);
                if (s.EndsWith("h") && double.TryParse(s[..^1], out var hr))
                    return TimeSpan.FromHours(hr);

                // fallback: try parse as seconds
                if (double.TryParse(s, out var seconds))
                    return TimeSpan.FromSeconds(seconds);
            }
            catch
            {
                // fall through to default
            }

            // default to 1 minute if unparseable
            return TimeSpan.FromMinutes(1);
        }

        private class ProcessResult
        {
            public int ExitCode { get; set; }
            public string StandardOutput { get; set; } = "";
            public string StandardError { get; set; } = "";
        }
    }
}