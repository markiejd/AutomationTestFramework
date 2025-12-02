using Core.Configuration;
using Core.FileIO;
using Core.Transformations;
using System.Diagnostics;

namespace Core.Logging
{
    public static class DebugOutput
    {
        public static int debugLevel { get; set; }
        private static string? previousBDD { get; set; }
        private static string previousBDDKeyWord { get; set; } = "given";

        public static void OutputMethod(string method, string arguments = "")
        {
            if (TargetConfiguration.Configuration.PerformanceTesting) TargetTestReport.CloseProcPlan();
            if (TargetConfiguration.Configuration.PerformanceTesting) TargetTestReport.NewProcPlan(method, arguments);
            Log(method + " " + arguments);
        }

        public static void WarningMessage(string message)
        {
            var warningMessage = " *** WARNING *** " + message;
            if (TargetConfiguration.Configuration.WarningFileOutput) WarningMessageToFile(message);
            Log(warningMessage);
        }

        public static void JiraOutput(string message)
        {
            if (!TargetConfiguration.Configuration.Jira) return;
            JiraMessageToFile(message);
            var warningMessage = " *** JIRA WARNING *** " + message;
            Log(warningMessage);
        }

        private static void JiraMessageToFile(string message)
        {
            var fullDirectoryName = FileUtils.GetCorrectDirectory("/AppSpecFlow/TestResults/JiraWarnings/");
            if (!FileUtils.OSDirectoryCheck(fullDirectoryName)) FileUtils.OSDirectoryCreation(fullDirectoryName);
            var nowDate = DateValues.ReturnNowDateAsString();
            var nowTime = TimeValues.ReturnNowTimeAsString();
            var fileName = "jiraWarnings.txt";
            var fullFileName = fullDirectoryName + "/" + fileName;
            if (!FileUtils.OSFileCheck(fullFileName)) FileUtils.OSFileCreationAndPopulation(fullDirectoryName,fileName, "");
            message = nowDate + " " + nowTime + " " + message;
            FileUtils.OSAppendLineToFile(fullFileName, message);
        }

        private static void WarningMessageToFile(string message)
        {
            var repoDirectory = FileIO.FileUtils.GetRepoDirectory();
            var fileName = "warnings.log";
            var warningDirectory = repoDirectory + @"\AppSpecFlow\TestResults\";
            if (!FileIO.FileUtils.DoesDirectoryExist(warningDirectory)) FileIO.FileUtils.DirectoryCreation(warningDirectory);
            var warningFileFull = warningDirectory + fileName;
            if (!FileIO.FileUtils.FileCheck(warningFileFull)) FileIO.FileUtils.FileCreation(warningFileFull);
            if (FileIO.FileUtils.UpdateTextFile(warningFileFull, message)) return;
        }

        /// <summary>
        /// NEW VERSION 29/08/2022
        //- 0. No output at all
        //- 1. Only Top BDD in 1 list
        //- 2. Only Top BDD also with scenarios
        //- 3. All BDD with Scenarios(would be nice to be indented if a Given calls multiple)
        //- 4. All BDD with Scenarios(would be nice to be indented if a Given calls multiple), but also the Procs(again nice to be indented)
        //- 5. All BDD with Scanarios and Proces - with all debug information(nice if that was indented)
        //- 6. Everything BAR the BDD - Procs and Debug Information Only
        //- 7. EVERYTHING
        //- 8. BDD and Json
        /// </summary>

        public static void Log(string message)
        {
            //Uncomment the below line to overwrite the json value
            //debugLevel = 5;
            //Output nothing

            if (debugLevel == 0)
            {
                return;
            }


            ///
            /// Debug 7 - Everything
            /// 
            if (debugLevel == 7)
            {
                Debug.WriteLine(message);
                return;
            }

            var first5Chars = "";
            if (message.Length >= 5)
            {
                first5Chars = message.Substring(0, 5).ToLower();
                first5Chars = first5Chars.Replace(" ", "");
                first5Chars = first5Chars.Replace("-", "");
            }

            ///
            /// Debug 8 BDD and Json - Procs and Debug Information Only
            /// 
            if (debugLevel == 8)
            {
                switch (first5Chars)
                {
                    default:
                        {
                            return;
                        }
                    case "given":
                        {
                            if (previousBDDKeyWord == "when" || previousBDDKeyWord == "then") Debug.WriteLine(" ");
                            previousBDDKeyWord = first5Chars;
                            break;
                        }
                    case "when":
                        {
                            if (previousBDDKeyWord == "then") Debug.WriteLine(" ");
                            previousBDDKeyWord = first5Chars;
                            break;
                        }
                    case "then":
                        {
                            previousBDDKeyWord = first5Chars;
                            break;
                        }
                    case "json":
                        {
                            message = message.Replace("json", "");
                            break;
                        }
                }
                Debug.WriteLine(message);
                return;
            }

            ///
            /// Debug 6 Everything BAR the BDD - Procs and Debug Information Only
            /// 
            if (debugLevel == 6)
            {
                switch (first5Chars)
                {
                    case "given":
                    case "when":
                    case "then":
                    case "scena":
                    case "aScen":
                    case "step":
                    case "json":
                        {
                            return;
                        }
                }
                Debug.WriteLine(message);
                return;
            }


            ///
            /// Debug 5 - All BDD with Scanarios and Proces - with all debug information(nice if that was indented)
            /// 
            if (debugLevel == 5)
            {
                switch (first5Chars)
                {
                    case "json":
                        {   
                            return;
                        }
                    case "scena":
                        {
                            Debug.WriteLine(message);
                            return;
                        }
                    case "step":
                        {
                            message = message.Replace("Step - ", "");
                            message = message.Replace("step - ", "");
                            message = message.Replace("\"", "");
                            previousBDD = message;
                            message = StringValues.Tabs(1, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    case "given":
                    case "when":
                    case "then":
                        {
                            if (previousBDD == null) return;
                            if (message.ToLower().Contains(previousBDD.ToLower()))
                            {
                                return;
                            }
                            message = StringValues.Tabs(2, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    case "procs":
                    case "proc":
                        {
                            message = StringValues.Tabs(3, message);
                            Debug.WriteLine(message);
                                                        return;
                        }
                    default:
                        {
                            message = StringValues.Tabs(4, message);
                            Debug.WriteLine(message);
                            return;
                        }
                }
            }

            ///
            /// Debug 4 - All BDD with Scenarios(would be nice to be indented if a Given calls multiple), but also the Procs(again nice to be indented)
            /// 
            if (debugLevel == 4)
            {
                switch (first5Chars)
                {
                    case "scena":
                        {
                            Debug.WriteLine(message);
                            return;
                        }
                    case "step":
                        {
                            message = message.Replace("Step - ", "");
                            message = message.Replace("step - ", "");
                            message = message.Replace("\"", "");
                            previousBDD = message;
                            message = StringValues.Tabs(1, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    case "given":
                    case "when":
                    case "then":
                        {
                            if (previousBDD == null) return;
                            if (message.ToLower().Contains(previousBDD.ToLower())) return;
                            message = StringValues.Tabs(2, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    case "procs":
                    case "proc":
                        {
                            message = StringValues.Tabs(3, message);
                            Debug.WriteLine(message);
                                                        return;
                        }
                    default:
                    case "json":
                        {
                            return;
                        }
                }
            }

            ///
            /// Debug 3 - All BDD with Scenarios(would be nice to be indented if a Given calls multiple)
            /// 
            if (debugLevel == 3)
            {
                switch (first5Chars)
                {
                    case "scena":
                        {
                            Debug.WriteLine(message);
                            return;
                        }
                    case "step":
                        {
                            message = message.Replace("Step - ", "");
                            message = message.Replace("step - ", "");
                            message = message.Replace("\"", "");
                            previousBDD = message;
                            message = StringValues.Tabs(1, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    case "given":
                    case "when":
                    case "then":
                        {
                            if (previousBDD == null) return;
                            if (message.ToLower().Contains(previousBDD.ToLower()))
                            {
                                return;
                            }
                            message = StringValues.Tabs(2, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    default:
                    case "json":
                        {
                            return;
                        }
                }
            }

            ///
            /// Debug 2 - Only Top BDD also with scenarios
            /// 
            if (debugLevel == 2)
            {
                switch (first5Chars)
                {
                    case "scena":
                        {
                            Debug.WriteLine(message);
                            return;
                        }
                    case "step":
                        {
                            message = message.Replace("Step - ", "");
                            message = message.Replace("step - ", "");
                            message = message.Replace("\"", "");
                            previousBDD = message;
                            message = StringValues.Tabs(1, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    default:
                    case "json":
                        {
                            return;
                        }
                }
            }

            ///
            /// Debug 1 - Only Top BDD in 1 list
            /// 
            if (debugLevel == 1)
            {
                switch (first5Chars)
                {
                    case "step":
                        {
                            message = message.Replace("Step - ", "");
                            message = message.Replace("step - ", "");
                            message = message.Replace("\"", "");
                            previousBDD = message;
                            message = StringValues.Tabs(1, message);
                            Debug.WriteLine(message);
                            return;
                        }
                    default:
                        {
                            return;
                        }
                }
            }

        }

        private static void Output(string message)
        {
            switch (debugLevel)
            {
                case 1:
                case 2:
                case 4:
                default:
                    {
                        return;
                    }
                case 3:
                    {
                        Debug.WriteLine(message);
                        return;
                    }
            }

        }

        private static void ProcOutput(string message)
        {
            switch (debugLevel)
            {
                case 1:
                case 3:
                    {
                        return ;
                    }
                case 2:
                    {
                        Debug.WriteLine(message);
                        return;
                    }
                case 4:
                default:
                    {
                        Debug.WriteLine(message);
                        return;
                    }
            }

        }

        private static void BDDOutput(string message)
        {
            switch (debugLevel)
            {
                case 1:
                    {
                        Debug.WriteLine(message);
                        return;
                    }
                case 2:
                case 3:
                case 4:
                default:
                    {
                        return;
                    }
            }
        }




    }
}