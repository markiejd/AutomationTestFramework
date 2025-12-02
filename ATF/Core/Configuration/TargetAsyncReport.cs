using System.ComponentModel;
using System.Dynamic;
using Core.FileIO;
using Core.Logging;
using Newtonsoft.Json;


namespace Core.Configuration
{
    public class TargetAsyncReport
    {
        public static TargetAsyncReportData Data { get; private set; } = new TargetAsyncReportData();

        public class TargetAsyncReportData
        {
            public string ID { get; set; } = "";
            public List<TargetAsyncReportDataRun> targetAsyncReportDataRun { get; set; } = new List<TargetAsyncReportDataRun>();
        }

        public class TargetAsyncReportDataRun
        {
            public long ThreadID { get; set; } = 0;
            public string Description { get; set; } = "";
            public long StartTick { get; set; } = 0;
            public long EndTick { get; set; } = 0;       
            public long TotalTicks { get; set; } = 0;
        }

        public static TargetAsyncReportData GetAsyncReportData()
        {
            return Data;
        }

        public static void NewAsyncReport()
        {
            Data = new TargetAsyncReportData();
            if (EPOCHControl.Epoch != null) Data.ID = EPOCHControl.Epoch;       
        }

        public static void NewAsyncReportDataRun(long threadID, long startTick, long endTick, string description = "")
        {
            var diff = endTick - startTick;
            var newRun = new TargetAsyncReportDataRun
            {
                ThreadID = threadID,
                StartTick = startTick,
                EndTick = endTick,
                TotalTicks = diff,
                Description = description
            };
            Data.targetAsyncReportDataRun.Add(newRun);
        }

        public static string? GetJson()
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
            return jsonString;
        }


        public static bool CloseAsyncReport(string fileNameAndLocation = @"\AppSpecFlow\TestResults\")
        {
            if (TargetConfiguration.Configuration.PerformanceTesting == false) return true;
            var jsonString = GetJson();
            if (jsonString == null) return false;
            var directory = fileNameAndLocation + EPOCHControl.Epoch;
            if (!FileUtils.DirectoryCheck(directory)) FileUtils.DirectoryCreation(directory);
            var fileNameAndLocationAsyncReport = directory + "\\" + "ASYNC" + ".html";
            var status = HTML.UseHTML.CreateHTMLAsyncReport(Data);
            if (!FileUtils.FilePopulate(fileNameAndLocationAsyncReport, status)) return false;
            DebugOutput.Log($"File Created!");
            return true;
        }

    }
}

