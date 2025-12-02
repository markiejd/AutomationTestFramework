// using System.Diagnostics;
// using Core.Configuration;
// using Core.FileIO;
// using Core.Logging;
// using DocumentFormat.OpenXml.Packaging;
// using DocumentFormat.OpenXml.Wordprocessing;

// namespace Core.Transformations
// {
//     public static class MicrosoftWord
//     {

//         public static List<string>? GetTextParagraphsFromWordDocument(string fullFilePathAndName)
//         {
//             using (WordprocessingDocument myDocument = WordprocessingDocument.Open(fullFilePathAndName, true))
//             {
//                 if (myDocument.MainDocumentPart != null)
//                 {
//                     if (myDocument.MainDocumentPart.Document != null)
//                     {
//                         if (myDocument.MainDocumentPart.Document.Body != null)
//                         {
//                             Body body = myDocument.MainDocumentPart.Document.Body;
//                             DebugOutput.Log($"We have the body");
//                             int counter = 1;
//                             var paragraphText = new List<string>();
//                             foreach (Paragraph paragraph in body.Elements<Paragraph>())
//                             {
//                                 DebugOutput.Log($"we are in paragraph {counter}");
//                                 var content = paragraph.InnerText;
//                                 paragraphText.Add(content);
//                                 counter++;
//                             }
//                             return paragraphText;
//                         }
//                     }
//                 }
//             }
//             return null;            
//         }
        
//         public static string? GetTextFromWordDocument(string fullFilePathAndName)
//         {
//             using (WordprocessingDocument myDocument = WordprocessingDocument.Open(fullFilePathAndName, true))
//             {
//                 if (myDocument.MainDocumentPart != null)
//                 {
//                     if (myDocument.MainDocumentPart.Document != null)
//                     {
//                         if (myDocument.MainDocumentPart.Document.Body != null)
//                         {
//                             Body body = myDocument.MainDocumentPart.Document.Body;
//                             DebugOutput.Log($"We have the body");
//                             int counter = 1;
//                             string content = "";
//                             foreach (Paragraph paragraph in body.Elements<Paragraph>())
//                             {
//                                 DebugOutput.Log($"we are in paragraph {counter}");
//                                 content = content + paragraph.InnerText;
//                                 content = content + "\n\r";
//                                 counter++;
//                             }
//                             return content;
//                         }
//                     }
//                 }
//             }
//             return null;
//         }


//     }
// }