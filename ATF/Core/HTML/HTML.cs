using Core.Configuration;
using Core.Images;
using Core.Logging;
using Core.Transformations;
using System.Net;
using System.Text;

namespace Core.HTML
{
    public static class UseHTML
    {
        public static string DisplayJsonAsHtml(string json)
        {
            var html = "<html>";
            html += "<head><title>JSON Content</title></head>";
            html += "<body><pre>";            

            var indentedJson = JsonValues.ReturnIndentedJson(json);
            
            html += WebUtility.HtmlEncode(indentedJson);

            html += "</pre></body>";
            html += "</html>";

            return html; 
        }

        private static StringBuilder CreateHeader(StringBuilder report, string title, string heading1)
        {
            report.AppendLine("<head>"); 
            report.AppendLine($"<title>{title}</title>");
            report.AppendLine($"<h1><u>{heading1}</u></h1>");  

            // Add CSS styles    
            report.AppendLine("<style>");   
            report.AppendLine("body {font-family: Arial;}");   
            report.AppendLine("table {border-collapse: collapse;}");   
            report.AppendLine("th, td {border: 1px solid black; padding: 5px;}");
            report.AppendLine("</style>");

            report.AppendLine("</head>"); 

            return report;
        }


        // public static string CreateAccessibilityHTMLReport(string pageName, string tabName, List<ImageDetails>? images, List<string> headerType, List<string> headerText, List<FailedContrastRatio> badContrast, List<IWebElement> hiddenElementWithTabIndex)
        // {
        //     DebugOutput.Log($"CreateAccessibilityHTMLReport");
        //     var report = new StringBuilder();
        //     string title = $"Automated Page Accessibility Report ";
        //     var app = TargetConfiguration.Configuration.AreaPath;
        //     var outPutPageName = pageName.Replace(app, "");
        //     outPutPageName = outPutPageName.Replace(" page", "");
        //     string heading1 = $"Page '{outPutPageName}' within Application '{app}'";
        //     report = CreateHeader(report, title, heading1);

        //     //  The Body
        //     report.AppendLine("<body>");  
        //     report.AppendLine($"<h4>Date\\Time of Report: {DateTime.UtcNow.ToString()}</h4>");  
        //     report.AppendLine("<hr />");

        //     //  Tab Title
        //     report.AppendLine($"<h2><u>Browser Tab</u></h2>"); 
        //     report.AppendLine($"<h3>Tab Title</h3>");   
        //     report.AppendLine("<table>");
        //     report.AppendLine(@"<table><tr><th>Title</th><th>Details</th></tr>");
        //     var tableDataLine = "";
        //     if (tabName == null) tableDataLine = $"<tr><td style=\"background-color: tomato; color: black; text-align: center;\">NULL</td><td>We have a blank tab title - this fails XYZ</td></tr>";
        //     if (tabName != null) tableDataLine = $"<tr><td style=\"background-color: Lime; color: black; text-align: center;\">{tabName}</td><td style=\"background-color: Lime; color: black; text-align: center;\">Tab Title Supplied</td></tr>";
        //     report.AppendLine(tableDataLine);
        //     report.AppendLine("</table>");
        //     report.AppendLine("<hr />");

        //     //  Image Alt 
        //     report.AppendLine($"<h2><u>Alt Text For Image(s)</u></h2>"); 
        //     if (images != null)
        //     {
        //         if (images.Count > 0)
        //         {
        //             DebugOutput.Log($"IMAGES!");
        //             report.AppendLine($"<h3>Images Should Have ALT Text</h3>");  
        //             report.AppendLine("<table>");
        //             report.AppendLine(@"<table><tr><th>image</th><th>Alt Text</th><th>Link?</th></tr>");
        //             foreach (var image in images)
        //             {
        //                 tableDataLine = "";
        //                 if (image.AltText == null || image.AltText == "") tableDataLine = $"<tr><td>Image</td><td style=\"background-color: tomato; color: black;\">{image.AltText}</td><td>{image.SRC}</td></tr>"; 
        //                 else tableDataLine = $"<tr><td>Image</td><td>{image.AltText}</td><td>{image.SRC}</td></tr>";        
        //                 report.AppendLine(tableDataLine);               
        //             }
        //             report.AppendLine("</table>");
        //         }
        //         else
        //         {
        //                 report.AppendLine($"<h3>No IMG's found!</h3>");  
        //         }
        //     }
        //     else
        //     {
        //             report.AppendLine($"<h3>No IMG's found!</h3>");  
        //     }
        //     report.AppendLine("<hr />");

        //     // Headers
        //     int counter = 0;
        //     report.AppendLine($"<h2><u>Header Elements</u></h2>"); 
        //     if (headerType.Count > 0)
        //     {
        //         report.AppendLine($"<h3><u>Headers and how they look without formatting</u></h3>");  
        //         foreach (var header in headerType)
        //         {
        //             DebugOutput.Log($"Header type = {header}");
        //             DebugOutput.Log($"Header text = '{headerText[counter]}'");
        //             if (headerText[counter] != "" || headerText[counter] != null)
        //             {
        //                 if (header == "h1") report.AppendLine($"<h1>{headerText[counter]}</h1>");
        //                 if (header == "h2") report.AppendLine($"<h2>{headerText[counter]}</h2>");
        //                 if (header == "h3") report.AppendLine($"<h3>{headerText[counter]}</h3>");
        //                 if (header == "h4") report.AppendLine($"<h4>{headerText[counter]}</h4>");
        //                 if (header == "h5") report.AppendLine($"<h5>{headerText[counter]}</h5>");
        //                 if (header == "h6") report.AppendLine($"<h6>{headerText[counter]}</h6>");
        //                 if (header == "h7") report.AppendLine($"<h7>{headerText[counter]}</h7>");
        //                 if (header == "h8") report.AppendLine($"<h8>{headerText[counter]}</h8>");
        //                 if (header == "h9") report.AppendLine($"<h9>{headerText[counter]}</h9>");
        //                 if (header == "h10") report.AppendLine($"<h10>{headerText[counter]}</h10>");
        //             }
        //             else
        //             {
        //                 var text = "NO HEADER TEXT FOUND!";
        //                 if (header == "h1") report.AppendLine($"<h1 style=\"background-color: tomato; color: black;\">{text}</h1>");
        //                 if (header == "h2") report.AppendLine($"<h2 style=\"background-color: tomato; color: black;\">{text}</h2>");
        //                 if (header == "h3") report.AppendLine($"<h3 style=\"background-color: tomato; color: black;\">{text}</h3>");
        //                 if (header == "h4") report.AppendLine($"<h4 style=\"background-color: tomato; color: black;\">{text}</h4>");
        //                 if (header == "h5") report.AppendLine($"<h5 style=\"background-color: tomato; color: black;\">{text}</h5>");
        //                 if (header == "h6") report.AppendLine($"<h6 style=\"background-color: tomato; color: black;\">{text}</h6>");
        //                 if (header == "h7") report.AppendLine($"<h7 style=\"background-color: tomato; color: black;\">{text}</h7>");
        //                 if (header == "h8") report.AppendLine($"<h8 style=\"background-color: tomato; color: black;\">{text}</h8>");
        //                 if (header == "h9") report.AppendLine($"<h9 style=\"background-color: tomato; color: black;\">{text}</h9>");
        //                 if (header == "h10") report.AppendLine($"<h10 style=\"background-color: tomato; color: black;\">{text}</h10>");
        //             }
        //             counter++;
        //         }
        //     }
        //     else
        //     {
        //         report.AppendLine($"<h3>No HEADERS's found!</h3>");
        //     }
        //     report.AppendLine("<hr />");


        //     ///  Tabs
        //     ///  Hidden Tabs with Tab Index
        //     ///  
        //     report.AppendLine($"<h2><u>Tab within Page</u></h2>"); 
        //     report.AppendLine($"<h3><u>Hidden Elements with Active Tab Index</u></h3>"); 
        //     if (hiddenElementWithTabIndex.Count > 0)
        //     {
        //         report.AppendLine($"We have found {hiddenElementWithTabIndex.Count} elements that is not currently displayed but have tab index >= 0 (i.e. can be tabbed to)");  
        //     }
        //     else
        //     {
        //         report.AppendLine($"None Found!");
        //     }




        //     // End of Report            
        //     report.AppendLine("</body></html>");                  
        //     return report.ToString();
        // }


        public static string CreateHTMLAsyncReport(TargetAsyncReport.TargetAsyncReportData asyncReport)
        {
            DebugOutput.Log($"CreateHTMLAsyncReport");
            var report = new StringBuilder();
            string title = $"Automated Async Test Result Report {asyncReport.ID}";
            string heading1 = "All SBU Async Step Test Results";
            report = CreateHeader(report, title, heading1);
            
            //  The body   
            report.AppendLine("<body>");  
            report.AppendLine($"<h4>Date\\Time of Report: {DateTime.UtcNow.ToString()}</h4>");  
            report.AppendLine($"<h3>{asyncReport.ID}</h3>");  

            var orderedRun = asyncReport.targetAsyncReportDataRun.OrderBy(s => s.ThreadID).ThenBy(s => s.StartTick);
            int idGroup = 0;
            report.AppendLine("</table>");
            report.AppendLine(@"<table><tr><th>Method</th><th>ID</th><th>Start DateTime</th><th>End DateTime</th><th>Execution Time (ticks)</th><th>Execution Time (seconds)</th></tr>");
            foreach ( var run in orderedRun)
            {
                long id = run.ThreadID;
                string desc = run.Description;
                long startTick = run.StartTick;
                long endTick = run.EndTick;
                long diff = run.EndTick - run.StartTick;
                DateTime start = new DateTime(startTick);
                DateTime end = new DateTime(endTick);
                double totalSeconds = Math.Round((double)diff / 10000000, 2);
                if (totalSeconds < 0) totalSeconds *= -1;
                if (idGroup != (int)id)
                {
                    var tableDataLine = $"<tr><td style=\"background-color:#000000;\"></td><td style=\"background-color:#000000;\"></td><td style=\"background-color:#000000;\"></td><td style=\"background-color:#000000;\"></td><td style=\"background-color:#000000;\"></td><td style=\"background-color:#000000;\"></td></tr>";
                    report.AppendFormat(tableDataLine);
                    idGroup = (int)id;
                }
                var tableDataData = $"<tr><td>{desc}</td><td>{id}</td><td>{start}</td><td>{end}</td><td style=\"text-align: right;\">{diff}</td><td style=\"text-align: right;\">{totalSeconds}</td></tr>";
                report.AppendFormat(tableDataData);
            }
            // End of Report            
            report.AppendLine("</body></html>");                  
            return report.ToString();
        }

        private static StringBuilder CreateHTMLAnalysisReportTableHeaderRow(StringBuilder report)
        {
            report.AppendLine("<tr><td>Question Number</td><td>Question</td><td>Question Entities</td><td>Full Answer</td><td>Entities</td><td>Full Answer Sentiment</td><td>Categories</td><td>Sentances</td><td>Sentance Categories</td>");
            report.AppendLine("</tr>");
            return report;
        }

        private static StringBuilder CreateHTMLAnalysisReportTableQuestionEntities(StringBuilder report, NLM.AnalysedAnswer detail)
        {
            if (detail.QuestionEntities == null) return report;
            var newQuestionEntityList = detail.QuestionEntities.OrderBy(o=>o.Type == "Other").ThenByDescending(o=>o.Salience).ToList();
            if (newQuestionEntityList.Count == 0) 
            {
                report.AppendLine("<td></td>");
                return report;
            }
            report.AppendLine("<td>");
            report.AppendLine("<table><tbody>");
            report.AppendLine("<tr>");
            report.AppendLine("<td>Entity Type</td>");
            report.AppendLine("<td>Name</td>");
            report.AppendLine("<td>Salience</td>");
            report.AppendLine("</tr>");
            foreach (var entity in newQuestionEntityList)
            {
                report.AppendLine("<tr>");
                if (entity.Name != null)
                {
                    //<td style=\"background-color: {red}; text-align: right;\"> <td>
                    var colour = "white";
                    if (entity.Type != "Other") colour = "lightgreen";
                    report.AppendLine($"<td style=\"background-color: {colour};\">{entity.Type}</td>");
                    report.AppendLine($"<td style=\"background-color: {colour};\">{entity.Name}</td>");
                    report.AppendLine($"<td style=\"background-color: {colour};\">{entity.Salience}</td>");                            
                }   
                report.AppendLine("</tr>");
            }
            report.AppendLine("</tr>");
            report.AppendLine("</tbody></table>");
            return report;
        }

        private static StringBuilder CreateHTMLAnalysisReportTableEntities(StringBuilder report, NLM.AnalysedAnswer detail)
        {
            if (detail.AIEntities == null) return report;
            var newEntityList = detail.AIEntities.OrderBy(o=>o.Type == "Other").ThenByDescending(o=>o.Salience).ToList();
            if (newEntityList.Count == 0)
            {
                report.AppendLine("<td></td>");
                return report;
            } 
            report.AppendLine("<td>");
            report.AppendLine("<table>");
            report.AppendLine("<tbody>");
            report.AppendLine("<tr>");
            report.AppendLine("<td>Entity Type</td>");
            report.AppendLine("<td>Name</td>");
            report.AppendLine("<td>Salience</td>");
            report.AppendLine("</tr>");
            foreach (var entity in newEntityList)
            {
                report.AppendLine("<tr>");
                if (entity.Name != null)
                {
                    //<td style=\"background-color: {red}; text-align: right;\"> <td>
                    var colour = "white";
                    if (entity.Type != "Other") colour = "lightgreen";
                    report.AppendLine($"<td style=\"background-color: {colour};\">{entity.Type}</td>");
                    report.AppendLine($"<td style=\"background-color: {colour};\">{entity.Name}</td>");
                    report.AppendLine($"<td style=\"background-color: {colour};\">{entity.Salience}</td>");                            
                }   
                report.AppendLine("</tr>");
            }
            report.AppendLine("</tbody>");
            report.AppendLine("</table>");
            return report;
        }

        private static StringBuilder CreateHTMLAnalysisReportTableFullSentiment(StringBuilder report, NLM.AnalysedAnswer detail)
        {
            if (detail.AISentiment == null) return report;
            var colour = "white";
            var sentiment = detail.AISentiment.Score;
            if (sentiment > 0.5) colour = "tomato";
            if (sentiment < -0.5) colour = "lightskyblue";
            report.AppendLine("<td>");
            report.AppendLine("<table>");
            report.AppendLine("<tbody>");
            report.AppendLine("<tr>");            
            report.AppendLine("<td>Sentiment Score</td>");
            var score = $"{detail.AISentiment.Score:0.00}";
            report.AppendLine($"<td style=\"background-color: {colour};\">{score}</td>");
            report.AppendLine("</tr>");

            report.AppendLine("<tr>");    
            report.AppendLine("<td>Sentiment Magnitude</td>");
            var magnitude = $"{detail.AISentiment.Magnitude:0.00}";
            report.AppendLine($"<td>{detail.AISentiment.Magnitude}</td>");
            report.AppendLine("</tr>");
            
            
            colour = "white";
            report.AppendLine("<tr>");    
            report.AppendLine("<td>Sentiment Total</td>");
            var total = detail.AISentiment.Score * detail.AISentiment.Magnitude;
            if (total > 0.5) colour = "tomato";
            if (total < -0.5) colour = "lightskyblue";
            var newTotal = $"{total:0.00}";
            report.AppendLine($"<td style=\"background-color: {colour};\">{newTotal}</td>");
            report.AppendLine("</tr>");

            report.AppendLine("</tbody>");
            report.AppendLine("</table>");            
            report.AppendLine("</td>");

            return report;
        }
        

        private static StringBuilder CreateHTMLAnalysisReportTableFullCategories(StringBuilder report, NLM.AnalysedAnswer detail)
        {
            if (detail.AICategories == null || detail.AICategories.Count < 1)
            {
                report.AppendLine("<td></td>");
                return report;
            }
            report.AppendLine("<td>");
            report.AppendLine("<table>");
            report.AppendLine("<tbody>");
            report.AppendLine("<tr>");
            report.AppendLine("<td>Cateogry</td>");
            report.AppendLine("<td>Confidence</td>");
            report.AppendLine("</tr>");
            
            foreach (var category in detail.AICategories)
            {
                report.AppendLine("<tr>");
                report.AppendLine("<td>");
                report.AppendLine(category.Name);                
                report.AppendLine("</td>");
                report.AppendLine("<td>");
                report.AppendLine($"{category.Confidence:0.00}");                
                report.AppendLine("</td>");
                report.AppendLine("</tr>");
            }
            report.AppendLine("</tbody>");
            report.AppendLine("</table>");
            report.AppendLine("</td>");

            return report;
        }


        private static StringBuilder CreateHTMLAnalysisReportTableSentances(StringBuilder report, NLM.AnalysedAnswer detail)
        {
            if (detail.AISentances != null)
            {
                if (detail.AISentances.Count <= 1)
                {
                    report.AppendLine("<td></td>");
                    return report;
                }        
                report.AppendLine("<td>");
                report.AppendLine("<table>");
                report.AppendLine("<tbody>");
                report.AppendLine("<tr>");
                report.AppendLine("<td>Sentance</td>"); 
                report.AppendLine("<td>Sentance Score</td>"); 
                report.AppendLine("<td>Sentance Magnitude</td>");
                report.AppendLine("</tr>");
                int counter = 0;
                foreach (var sentance in detail.AISentances)
                {
                    if (detail.AISentanceSentiment != null)
                    {                
                        var x = detail.AISentanceSentiment[counter];
                        
                        report.AppendLine("<tr>");
                        report.AppendLine("<td>");
                        report.AppendLine(sentance);                        
                        report.AppendLine("</td>");      
                        report.AppendLine("<td>");
                        report.AppendLine($"{x.Score}");                        
                        report.AppendLine("</td>");      
                        report.AppendLine("<td>");
                        report.AppendLine($"{x.Magnitude}");                        
                        report.AppendLine("</td>");   
                        report.AppendLine("</tr>");  
                    }
                    counter++;
                }                
                report.AppendLine("</tbody>");     
                report.AppendLine("</table>");
                report.AppendLine("</td>");
            }
            return report;
        }

        public static StringBuilder CreateHTMLComparisonTableCompareAnswersHeaderRow(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {

            report.AppendLine("<tr>");
            
            report.AppendLine("<td>");
            report.AppendLine("Answer");
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            report.AppendLine("Sentiment <sub>1</sub>");
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            report.AppendLine("S.Magnatude <sub>2</sub>");
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            report.AppendLine("C.Sentiment <sub>3</sub>");
            report.AppendLine("</td>");
            
            report.AppendLine("</tr>");            

            return report;
        }

        private static StringBuilder CreateHTMLComparisonTableCompareAnswersAIRow(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {
            report.AppendLine("<tr>");
            
            report.AppendLine("<td>");
            report.AppendLine("AI Answer");
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            if (model.AISentiment != null ) report.AppendLine(model.AISentiment.Score.ToString());
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            if (model.AISentiment != null ) report.AppendLine(model.AISentiment.Magnitude.ToString());
            report.AppendLine("</td>");
            
            if (model.AISentiment != null)
            {
                var combinedSentiment = model.AISentiment.Score * model.AISentiment.Magnitude;
                report.AppendLine("<td>");
                report.AppendLine(combinedSentiment.ToString());
                report.AppendLine("</td>"); 
            }
            else
            {
                report.AppendLine("<td>");
                report.AppendLine("</td>");
            }

            report.AppendLine("</tr>"); 

            return report;
        }

        private static StringBuilder CreateHTMLComparisonTableCompareAnswersExpectedRow(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {
            report.AppendLine("<tr>");
            
            report.AppendLine("<td>");
            report.AppendLine("Ex. Answer");
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            if (model.ExpectedSentiment != null) report.AppendLine(model.ExpectedSentiment.Score.ToString());
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            if (model.ExpectedSentiment != null)report.AppendLine(model.ExpectedSentiment.Magnitude.ToString());
            report.AppendLine("</td>");
            
            if (model.ExpectedSentiment != null)
            {
                var combinedSentiment = model.ExpectedSentiment.Score * model.ExpectedSentiment.Magnitude;
                report.AppendLine("<td>");
                report.AppendLine(combinedSentiment.ToString());
                report.AppendLine("</td>");
            }
            else
            {
                report.AppendLine("<td>");
                report.AppendLine("</td>");
            }

            report.AppendLine("</tr>");

            return report;
        }

        private static StringBuilder CreateHTMLComparisonTableCompareAnswersDifferencedRow(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {

            report.AppendLine("<tr>");
            
            report.AppendLine("<td>");
            report.AppendLine("Difference");
            report.AppendLine("</td>");    
            
            if (model.AISentiment != null && model.ExpectedSentiment != null)
            {
                var diff = model.AISentiment.Score - model.ExpectedSentiment.Score;
                report.AppendLine("<td>");
                report.AppendLine(diff.ToString());
                report.AppendLine("</td>");
            }            

            if (model.AISentiment != null && model.ExpectedSentiment != null)
            {
                var diff = model.AISentiment.Magnitude - model.ExpectedSentiment.Magnitude;
                report.AppendLine("<td>");
                report.AppendLine(diff.ToString());
                report.AppendLine("</td>");
            }
            
            if (model.AISentiment != null && model.ExpectedSentiment != null)
            {            
                var diff = (model.AISentiment.Score * model.AISentiment.Magnitude) - (model.ExpectedSentiment.Score * model.ExpectedSentiment.Magnitude);
                report.AppendLine("<td>");
                report.AppendLine(diff.ToString());
                report.AppendLine("</td>");   
            }     

            report.AppendLine("</tr>");

            return report;
        }

        public static StringBuilder CreateHTMLComparisonTableCompareAnswers(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {            
            report.AppendLine("<table>");  
            report.AppendLine("<tbody>");

            report = CreateHTMLComparisonTableCompareAnswersHeaderRow(report, model, lineNumber);

            report = CreateHTMLComparisonTableCompareAnswersAIRow(report, model, lineNumber);

            report = CreateHTMLComparisonTableCompareAnswersExpectedRow(report, model, lineNumber);

            report = CreateHTMLComparisonTableCompareAnswersDifferencedRow(report, model, lineNumber);

            report.AppendLine("</tbody>");
            report.AppendLine("</table>");  

            return report;
        }

        private static StringBuilder CreateHTMLComparisonTableCompareAnswersDetailsTableRow(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {
            if (model.Measurements == null) return report;
            report.AppendLine("<tr>");
            
            // Style for Cosine Similarity
            if (model.Measurements.CosineSimilarity != null) 
                report.AppendLine($"<td style=\"background-color: {GetColourForSimilarity((float)model.Measurements.CosineSimilarity)};\">");
            report.AppendLine(model.Measurements.CosineSimilarity.ToString());
            report.AppendLine("</td>");

            // Style for Jaccard Similarity
            if (model.Measurements.JaccardSimilarity != null) 
                report.AppendLine($"<td style=\"background-color: {GetColourForSimilarity((float)model.Measurements.JaccardSimilarity)};\">");
            report.AppendLine(model.Measurements.JaccardSimilarity.ToString());
            report.AppendLine("</td>");

            string percentageText = "0.0"; 

            if (model.AIAnswer != null && model.ExpectedAnswer != null)
            {
                var aiAnswerLength = model.AIAnswer.Length;
                var expectedAnswerLength = model.ExpectedAnswer.Length;

                // Calculate Levenshtein distance
                var levenshteinDistance = model.Measurements.LevenshteinDistance ?? 0;
                var max = Math.Max(aiAnswerLength, expectedAnswerLength);

                // Calculate the percentage
                float percentage = max != 0 ? 1 - (float)Math.Max(0, levenshteinDistance) / max : 0;

                // Ensure the percentage is within the range [0, 1]
                percentage = Math.Max(0, Math.Min(1, percentage));

                // Convert percentage to string
                percentageText = percentage.ToString();

            }

            // Display percentage
            report.AppendLine($"<td style=\"background-color: {GetColourForSimilarity(float.Parse(percentageText))};\">");
            report.AppendLine(percentageText);
            report.AppendLine("</td>");

            report.AppendLine("</tr>");
            return report;
        }

        private static string GetColourForSimilarity(float similarity)
        {
            if (similarity == 1) return "lime";
            else if (similarity > 0.75) return "lightgreen";
            else if (similarity > 0.50) return "white";
            else if (similarity > 0.25) return "yellow";
            else return "tomato";
        }

        private static StringBuilder CreateHTMLComparisonTableCompareAnswersDetailsTableHeader(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {
            report.AppendLine("<tr>");
            
            report.AppendLine("<td>");
            report.AppendLine("Cosine Similarity");
            report.AppendLine("</td>");   
            
            report.AppendLine("<td>");
            report.AppendLine("Jaccard Similarity");
            report.AppendLine("</td>");     
            
            report.AppendLine("<td>");
            report.AppendLine("Levenshtein Distance");
            report.AppendLine("</td>");

            report.AppendLine("</tr>");
            return report;
        }

        private static StringBuilder CreateHTMLComparisonTableCompareAnswersDetails(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {
            report.AppendLine("<table>");  
            report.AppendLine("<tbody>");

            report = CreateHTMLComparisonTableCompareAnswersDetailsTableHeader(report, model, lineNumber);
            report = CreateHTMLComparisonTableCompareAnswersDetailsTableRow(report, model, lineNumber);

            // MORE

            report.AppendLine("</tbody>");
            report.AppendLine("</table>");  
            return report;
        }

        public static StringBuilder CreateHTMLComparisonTableRow(StringBuilder report, NLM.AnalysedAnswer model, int lineNumber)
        {
            report.AppendLine("<tr>");

            report.AppendLine("<td>");
            report.AppendLine(lineNumber.ToString());
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            if (EPOCHControl.Epoch == null) report.AppendLine(model.QuestionNumber.ToString());
            else
            {
                if (model.TimeOfTest != null)
                    {
                        report.AppendLine(model.TimeOfTest.ToString());
                    }
                else
                    {
                        var epochTime = EPOCHControl.GetDateTimeFromEPOCH(EPOCHControl.Epoch);
                        report.AppendLine(epochTime.ToString());
                    }
            }
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            report.AppendLine(model.Question);
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            report.AppendLine(model.ExpectedAnswer);
            report.AppendLine("</td>");
            
            report.AppendLine("<td>");
            report.AppendLine(model.AIAnswer);
            report.AppendLine("</td>");
    
            report.AppendLine("<td>");
            report = CreateHTMLComparisonTableCompareAnswers(report, model, lineNumber);
            report.AppendLine("</td>");
    
            report.AppendLine("<td>");
            report = CreateHTMLComparisonTableCompareAnswersDetails(report, model, lineNumber);
            report.AppendLine("</td>");


            report.AppendLine("</tr>");
            return report;
        }

        public static StringBuilder CreateHTMLComparisonTableHeaderRow(StringBuilder report)
        { 
            report.AppendLine("<tr>");

            
            report.AppendLine("<td width=\"100\">");
            report.AppendLine("Question #");
            report.AppendLine("</td>");
            
            report.AppendLine("<td width=\"200\">");
            if (EPOCHControl.Epoch == null) report.AppendLine("EPOCH");
            else
            {
                var now = EPOCHControl.GetDateTimeFromEPOCH(EPOCHControl.Epoch);
                report.AppendLine("Date and Time");
            }
            report.AppendLine("</td>");
            
            report.AppendLine("<td width=\"300\">");
            report.AppendLine("Full Question");
            report.AppendLine("</td>");
            
            report.AppendLine("<td width=\"300\">");
            report.AppendLine("Expected Answer");
            report.AppendLine("</td>");
            
            report.AppendLine("<td width=\"300\">");
            report.AppendLine("AI Answer");
            report.AppendLine("</td>");
            
            report.AppendLine("<td width=\"300\">");
            report.AppendLine("Compare Answers Sentiment");
            report.AppendLine("</td>");
            
            report.AppendLine("<td width=\"300\">");
            report.AppendLine("Compare Answers");
            report.AppendLine("</td>");


            report.AppendLine("</tr>");
            return report;            
        }

        public static string CreateHTMLComparisonReport(List<NLM.AnalysedAnswer> newComparisonModel)
        {
            DebugOutput.Log($"CreateHTMLComparisonReport");
            var report = new StringBuilder();
            string title = "Automated Test Analysis Report";
            string heading1 = "Comparing AI Answers and Expected Answers";
            report = CreateHeader(report, title, heading1);
            report = CreateKey(report);
            
            //  The body   
            report.AppendLine("<body>");  
            
            report.AppendLine("<table>");  
            report.AppendLine("<tbody>");

            report = CreateHTMLComparisonTableHeaderRow(report);
            int questionNumber = 1;
            foreach (var model in newComparisonModel)
            {
                report = CreateHTMLComparisonTableRow(report, model, questionNumber);
                questionNumber++;
            }


            
  
            return report.ToString();
        }


        public static string CreateHTMLAnalysisReport(List<NLM.AnalysedAnswer> analysedReports)
        {
            DebugOutput.Log($"CreateHTMLAnalysisReport");
            var report = new StringBuilder();
            string title = "Automated Test Analysis Report";
            string heading1 = "Testing AI Questions and Answers";
            report = CreateHeader(report, title, heading1);
            
            //  The body   
            report.AppendLine("<body>");  
            
            report.AppendLine("<table>");  
            report.AppendLine("<tbody>");
            
            report = CreateHTMLAnalysisReportTableHeaderRow(report);

            foreach(var detail in analysedReports)
            {
                report.AppendLine("<tr>");
                report.AppendLine($"<td>{detail.QuestionNumber}</td>");
                report.AppendLine($"<td>{detail.Question}</td>");
                // Question Entities
                if (detail.QuestionEntities != null)
                {
                    report = CreateHTMLAnalysisReportTableQuestionEntities(report, detail);
                }
                else report.AppendLine("<td></td>");
                report.AppendLine($"<td>{detail.AIAnswer}</td>");
                if (detail.AIEntities != null)
                {
                    report = CreateHTMLAnalysisReportTableEntities(report, detail);
                }
                else report.AppendLine("<td></td>");
                report = CreateHTMLAnalysisReportTableFullSentiment(report, detail);
                report = CreateHTMLAnalysisReportTableFullCategories(report, detail);
                report = CreateHTMLAnalysisReportTableSentances(report, detail);

            }

            report.AppendLine("</tbody>");
            report.AppendLine("</table>");
            // End of Report            
            report.AppendLine("</body></html>");        
            return report.ToString();
        }

        // public static string CreateHTMLHistoricComparison(List<List<NLM.AnalysedAnswer>> historicData)
        // {
        //     var report = new StringBuilder();
        //     string title = "Historic Comparison Report";
        //     string heading1 = "Comparison Results";
        //     report = CreateHeader(report, title, heading1);

        //     // Group analysed answers by the starting phrase of each question
        //     var groupedByPhrase = historicData.SelectMany(group => group)
        //                                     .GroupBy(answer => GetStartingPhrase(answer.Question));

        //     foreach (var group in groupedByPhrase)
        //     {
        //         // Check if the group contains analyses with a starting phrase
        //         if (!string.IsNullOrEmpty(group.Key))
        //         {
        //             report.AppendLine($"<h2>{group.Key}</h2>");

        //             report.AppendLine("<table>");
        //             report.AppendLine("<tbody>");

        //             report = CreateHTMLComparisonTableHeaderRow(report);
        //             int questionNumber = 1;
        //             foreach (var model in group)
        //             {
        //                 report = CreateHTMLComparisonTableRow(report, model, questionNumber);
        //                 questionNumber++;
        //             }

        //             report.AppendLine("</tbody>");
        //             report.AppendLine("</table>");
        //         }
        //     }

        //     return report.ToString();
        // }

        // Helper method to extract the starting phrase of a question
        private static string GetStartingPhrase(string question)
        {
            if (string.IsNullOrEmpty(question))
                return string.Empty;

            int index = question.IndexOf('?');
            if (index != -1)
            {
                return question.Substring(0, index + 1);
            }
            return string.Empty;
        }

        public static string CreateHTMLAssertionReport(TargetTestReport.TestReportTiming testReport)
        {
            DebugOutput.Log($"CreateHTMLAssertionReport");
            var report = new StringBuilder();
            string title = "Automated Test Result Report";
            string heading1 = "All SBU Feature Step Test Results";
            report = CreateHeader(report, title, heading1);
            
            //  The body   
            report.AppendLine("<body>");  
            
            //  Over View
            int howManyFeatures = TargetTestReport.GetHowManyFeatures();
            int howManyScenarios = TargetTestReport.GetHowManyScenarios();
            int howManySteps = TargetTestReport.GetHowManySteps();
            int howManyStepsFailed = TargetTestReport.GetHowManyStepFailures();
            report.AppendLine("<table>");  
            report.AppendFormat("<tr><th>Feature Count</th><td>{0}</td></tr>", howManyFeatures);
            report.AppendFormat("<tr><th>Scenarios Count</th><td>{0}</td></tr>", howManyScenarios);
            report.AppendFormat("<tr><th>Step Count</th><td>{0}</td></tr>", howManySteps);
            if (howManyStepsFailed > 0) report.AppendFormat("<tr><th>Step Failures</th><td style=\"background-color: tomato; color: black; text-align: center;\">{0}</td></tr>", howManyStepsFailed); 
            else report.AppendFormat("<tr><th>Step Failures</th><td style=\"text-align: center;\">{0}</td></tr>", howManyStepsFailed);
            
            // Features broken down to Scenario and Steps
            report.AppendLine("<h2>Feature Details</h2>");
            string? previousFeatureName = "";
            string? previousScenarioName = "";
            report.AppendLine(@"<table><tr><th>Feature</th><th>Scenario</th><th>Step</th><th>Status</th></tr>");
            foreach (var testPlan in testReport.TestPlans)
            {
                int totalScenarioCount = testPlan.ScenarioPlans.Count;
                var featureName = testPlan.FeatureName;
                // report.AppendLine($"<h2>Feature: {featureName}</h2>");
                for (int i = 0; i < totalScenarioCount; i++)
                {
                    var scenarioName = testPlan.ScenarioPlans[i].TestScenarioName;
                    var scenarioStatus = testPlan.ScenarioPlans[i].TestScenarioStatus.ToString();
                    // report.AppendLine($"<h3>Scenario: {scenarioName} {scenarioStatus}</h3>");
                    foreach (var step in testPlan.ScenarioPlans[i].StepPlans)
                    {
                        var stepName = step.TestStepName;
                        var testStepStatus = step.TestStepStatus??false;
                        string stepStatus;
                        if (testStepStatus) stepStatus = "Pass";
                        else stepStatus = "Fail";

                        // Truncate long step names (over 100 chars) and append ellipsis
                        var displayStepName = stepName;
                        if (!string.IsNullOrEmpty(displayStepName) && displayStepName.Length > 100)
                        {
                            displayStepName = displayStepName.Substring(0, 97) + "...";
                        }         

                        bool missFeatureName = false;
                        if (previousFeatureName == featureName) missFeatureName = true;
                        previousFeatureName = featureName;

                        bool missScenarioName = false;
                        if (previousScenarioName == scenarioName) missScenarioName = true;
                        previousScenarioName = scenarioName;

                        string tableDataLine = "";
                        var colour = "lightgreen";
                        var textColour ="black";
                        if (!testStepStatus) colour = "tomato";
                        if (missFeatureName) tableDataLine = $"<tr><td></td><td>{scenarioName}</td><td>{displayStepName}</td><td style=\"background-color: {colour}; color: {textColour}; text-align: center;\">{stepStatus}</td></tr>";
                        if (missScenarioName) tableDataLine = $"<tr><td></td><td></td><td>{displayStepName}</td><td style=\"background-color: {colour}; text-align: center;\">{stepStatus}</td></tr>";
                        if (!missFeatureName && !missScenarioName) tableDataLine = $"<tr><td>{featureName}</td><td>{scenarioName}</td><td>{displayStepName}</td><td style=\"background-color: {colour}; text-align: center;\">{stepStatus}</td></tr>";
                        report.AppendFormat(tableDataLine);
                    }                    
                }
            }

            // End of Report            
            report.AppendLine("</body></html>");                  
            return report.ToString();
        }


        private static long GetAverageExecutionOfMethod(IGrouping<string?, TargetTestReport.TestReportProcPlan>? group)
        {
            long totalExecutionTime = 0;
            int totalNumberOfMethods = 0;
            if (group == null) return 0; 
            foreach (var testReportStepPlan in group)
            {
                totalNumberOfMethods++;
                long stepExecution = testReportStepPlan.TestProcExecution??0;
                totalExecutionTime += stepExecution;
            }
            var ave = totalExecutionTime / totalNumberOfMethods;
            return ave;
        }


        private static long GetAverageExecutionOfTestReportStepPlan(IGrouping<string?, TargetTestReport.TestReportStepPlan>? group)
        {
            long totalExecutionTime = 0;
            int totalNumberOfSteps = 0;
            if (group == null) return 0; 
            foreach (var testReportStepPlan in group)
            {
                totalNumberOfSteps++;
                long stepExecution = testReportStepPlan.TestStepExecution??0;
                totalExecutionTime += stepExecution;
            }
            var ave = totalExecutionTime / totalNumberOfSteps;
            return ave;
        }


        public static string CreateHTMLMethodPerformanceComparisonReport(List<TargetTestReport.TestReportProcPlan> listOfMethods, DateTime lastRunStart, DateTime lastRunEnd)
        {
            var methodsGroupedByName = listOfMethods.GroupBy(TestReportProcPlan => TestReportProcPlan.TestProcName);
            var report = new StringBuilder();
            string title = "Performance Comparison Methods";
            string heading1 = "SBU Performance Comparison Methods Test Results";
            report = CreateHeader(report, title, heading1);

            //  The body   
            report.AppendLine("<body>");         
            report.AppendLine($"<h5>Date\\Time of Report: {DateTime.UtcNow.ToString()}</h5>");  
            report.AppendLine($"<h6>Last run: {lastRunStart} till {lastRunEnd}</h6>");  

            foreach (var group in methodsGroupedByName)
            {
                report.AppendLine($"<h2>{group.Key}</h2>");  
                
                var aveExecutionOfMethods = GetAverageExecutionOfMethod(group);
                report.AppendLine($"<h3>Average Execution for this step is {aveExecutionOfMethods.ToString()} ticks</h3>");  
                var fivePercent = aveExecutionOfMethods / 5;
                report.AppendLine($"<h4>Highlighting Step Execution with difference from Average  {fivePercent.ToString()} ticks</h4>");
                report.AppendLine(@"<table><tr><th>Execution (ticks)</th><th>Difference (ticks)</th><th>Start</th><th>End</th><th>Arguments</th></tr>");
                var red = "tomato";
                var green = "lightgreen";
                foreach (var procPlan in group)
                {
                    var diffInExecution = procPlan.TestProcExecution - aveExecutionOfMethods;
                    string executionTime = procPlan.TestProcExecution.ToString()??"";
                    string diff = diffInExecution.ToString()??"";
                    if (procPlan.TestProcStart >= lastRunStart && procPlan.TestProcEnd <= lastRunEnd) report.AppendFormat($"<tr><td style=\"background-color: {green}; text-align: right;\">{executionTime}</td><td style=\"background-color: {green}; text-align: right;\">{diff}</td><td style=\"background-color: {green};\">{procPlan.TestProcStart}</td><td style=\"background-color: {green};\">{procPlan.TestProcEnd}</td><td style=\"background-color: {green}; text-align: right;\">{procPlan.Arguments}</td></tr>");
                    else if (diffInExecution > fivePercent) report.AppendFormat($"<tr><td style=\"text-align: right;\">{executionTime}</td><td style=\"background-color: {red}; text-align: right;\">{diff}</td><td>{procPlan.TestProcStart}</td><td>{procPlan.TestProcEnd}</td><td style=\"text-align: right;\">{procPlan.Arguments}</td></tr>");
                    else report.AppendFormat($"<tr><td style=\"text-align: right;\">{executionTime}</td><td style=\"text-align: right;\">{diff}</td><td>{procPlan.TestProcStart}</td><td>{procPlan.TestProcEnd}</td><td style=\"text-align: right;\">{procPlan.Arguments}</td></tr>");
                }
                report.AppendLine("</table>"); 
            }
            return report.ToString();
        }


        public static string CreateHTMLPerformanceComparisonReport(List<TargetTestReport.TestReportStepPlan> listOfSteps, DateTime lastRunStart, DateTime lastRunEnd)
        {
            var stepsGroupedByName = listOfSteps.GroupBy(TestReportStepPlan => TestReportStepPlan.TestStepName);
            var report = new StringBuilder();
            string title = "Performance Comparison";
            string heading1 = "SBU Performance Comparison Step Test Results";
            report = CreateHeader(report, title, heading1);

            //  The body   
            report.AppendLine("<body>");         
            report.AppendLine($"<h5>Date\\Time of Report: {DateTime.UtcNow.ToString()}</h5>");  
            report.AppendLine($"<h6>Last run: {lastRunStart} till {lastRunEnd}</h6>");  

            foreach (var group in stepsGroupedByName)
            {
                report.AppendLine($"<h2>{group.Key}</h2>");  
                
                var aveExecutionOfStep = GetAverageExecutionOfTestReportStepPlan(group);
                report.AppendLine($"<h3>Average Execution for this step is {aveExecutionOfStep.ToString()} ticks</h3>");  
                var fivePercent = aveExecutionOfStep / 5;
                report.AppendLine($"<h4>Highlighting Step Execution with difference from Average  {fivePercent.ToString()} ticks</h4>");
                report.AppendLine(@"<table><tr><th>Execution (ticks)</th><th>Difference (ticks)</th><th>Start</th><th>End</th></tr>");
                var red = "tomato";
                var green = "lightgreen";
                foreach (var testReportStepPlan in group)
                {
                    var diffInExecution = testReportStepPlan.TestStepExecution - aveExecutionOfStep;
                    string executionTime = testReportStepPlan.TestStepExecution.ToString()??"";
                    string diff = diffInExecution.ToString()??"";
                    if (testReportStepPlan.TestStepStart >= lastRunStart && testReportStepPlan.TestStepEnd <= lastRunEnd) report.AppendFormat($"<tr><td style=\"background-color: {green}; text-align: right;\">{executionTime}</td><td style=\"background-color: {green}; text-align: right;\">{diff}</td><td style=\"background-color: {green};\">{testReportStepPlan.TestStepStart}</td><td style=\"background-color: {green};\">{testReportStepPlan.TestStepEnd}</td></tr>");
                    else if (diffInExecution > fivePercent) report.AppendFormat($"<tr><td style=\"text-align: right;\">{executionTime}</td><td style=\"background-color: {red}; text-align: right;\">{diff}</td><td>{testReportStepPlan.TestStepStart}</td><td>{testReportStepPlan.TestStepEnd}</td></tr>");
                    else report.AppendFormat($"<tr><td style=\"text-align: right;\">{executionTime}</td><td style=\"text-align: right;\">{diff}</td><td>{testReportStepPlan.TestStepStart}</td><td>{testReportStepPlan.TestStepEnd}</td></tr>");
                }
                report.AppendLine("</table>"); 
            }
            return report.ToString();
        }
        

        public static string CreateHTMLPerformanceReport(TargetTestReport.TestReportTiming testReport)
        {
            var report = new StringBuilder();
            string title = "Automated Test Result Report";
            string heading1 = "All SBU Feature Test Results";
            report = CreateHeader(report, title, heading1);

            //  The body   
            report.AppendLine("<body>");            

            // Test Plan Summary Table
            foreach (var testPlan in testReport.TestPlans)
            {
                if (testPlan.EPOCHNumber != null)
                {
                    var epochDate = EPOCHControl.GetDateTimeFromEPOCH(testPlan.EPOCHNumber);
                    report.AppendLine($"<h1>Feature: {testPlan.FeatureName} {epochDate}</h1>");  
                }
                report.AppendLine("<table>");  
                report.AppendFormat("<tr><th>This Test Plan Start</th><td>{0}</td></tr>", testPlan.TestPlanStart);
                report.AppendFormat("<tr><th>All Test Plans End</th><td>{0}</td></tr>", testPlan.TestPlanEnd);
                report.AppendFormat("<tr><th>Feature Name</th><td>{0}</td></tr>", testPlan.FeatureName);
                report.AppendFormat("<tr><th>Test Plan ID</th><td>{0}</td></tr>", testPlan.EPOCHNumber); 
                report.AppendLine("</table>");
                
                // Scenarios table
                int scenarioCount = 0;
                report.AppendLine("<h2>Scenarios</h2>");
                report.AppendLine(@"<table><tr><th>Name</th><th>Start</th><th>End</th><th>Duration</th></tr>");
                foreach (var scenarioPlan in testPlan.ScenarioPlans)
                {
                    report.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td style=\"text-align: right;\">{3} ticks</td></tr>", scenarioPlan.TestScenarioName, scenarioPlan.TestScenarioStart, scenarioPlan.TestScenarioEnd, scenarioPlan.TestScenarioExecution);
                    scenarioCount++;
                }
                report.AppendLine("</table>");  
                
                // Steps table 
                report.AppendLine("<h3>Scenario Steps</h3>");  
                int stepCount = 0;
                for (int counter = 0; counter < scenarioCount; counter++)
                {
                    var scenarioName = testPlan.ScenarioPlans[counter].TestScenarioName;
                    report.AppendLine($"<h3>{scenarioName}</h3>");
                    report.AppendLine(@"<table><tr><th>Name</th><th>Start</th><th>End</th><th>Duration</th><th>Duration (s)</th></tr>");
                    long stepExecutionCounter = 0;
                    DateTime firstStepStart = DateTime.Now;
                    DateTime lastStepEnded = DateTime.Now;
                    foreach (var step in testPlan.ScenarioPlans[counter].StepPlans)
                    {
                        if (stepCount == 0) firstStepStart = step.TestStepStart??DateTime.Now;
                        report.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td style=\"text-align: right;\">{3}</td></tr>",step.TestStepName, step.TestStepStart, step.TestStepEnd, step.TestStepExecution); 
                        stepCount ++;
                        long stepTime = step.TestStepExecution??0;
                        stepExecutionCounter += stepTime;
                        lastStepEnded = step.TestStepEnd??DateTime.Now;
                    }
                    double totalSeconds = Math.Round((double)stepExecutionCounter / 10000000, 2);
                    if (totalSeconds < 0) totalSeconds *= -1;
                    report.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td style=\"text-align: right;\">{4}</td></tr>",".........TOTAL Duration for All Steps", firstStepStart, lastStepEnded, stepExecutionCounter, totalSeconds); 
                    report.AppendLine("</table>");
                }
            }
            report.AppendLine("</body></html>");            
            return report.ToString();
        }

        private static StringBuilder CreateKey(StringBuilder report)
        {
            // Key for Cosine Similarity Scores
            report.AppendLine("<table class=\"key\" style=\"font-size: 12px;\">");
            report.AppendLine("<tr>");
            report.AppendLine("<th>Similarity Range</th>");
            report.AppendLine("<th>Colour</th>");
            report.AppendLine("</tr>");

            report.AppendLine("<tr>");
            report.AppendLine("<td>Perfect Similarity (1.00)</td>");
            report.AppendLine("<td style=\"background-color: lime;\"></td>");
            report.AppendLine("</tr>");

            report.AppendLine("<tr>");
            report.AppendLine("<td>High Similarity (0.75 - 0.99)</td>");
            report.AppendLine("<td style=\"background-color: lightgreen;\"></td>");
            report.AppendLine("</tr>");

            report.AppendLine("<tr>");
            report.AppendLine("<td>Moderate Similarity (0.50 - 0.74)</td>");
            report.AppendLine("<td style=\"background-color: white;\"></td>");
            report.AppendLine("</tr>");

            report.AppendLine("<tr>");
            report.AppendLine("<td>Low Similarity (0.25 - 0.49)</td>");
            report.AppendLine("<td style=\"background-color: yellow;\"></td>");
            report.AppendLine("</tr>");

            report.AppendLine("<tr>");
            report.AppendLine("<td>Very Low Similarity (0.00 - 0.24)</td>");
            report.AppendLine("<td style=\"background-color: tomato;\"></td>");
            report.AppendLine("</tr>");

            report.AppendLine("</table>");

            return report;
        }

    }

}