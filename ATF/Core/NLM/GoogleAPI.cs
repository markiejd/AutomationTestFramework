
// using System.Diagnostics;
// using Core.Configuration;
// using Core.Logging;
// using Google.Cloud.Language.V1;


// namespace Core.Google
// {
//     public static class GoogleAPI
//     {

//         private static readonly string NLP = "google";

//         public static AnnotateTextResponse? GetAnnotateTextResponse(string inputText)
//         {
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;
//             DebugOutput.OutputMethod("GetAnnotateTextResponse", $"{inputText}");
//             try
//             {                
//                 var client = LanguageServiceClient.Create();
//                 var response = client.AnnotateText(new Document()
//                 {
//                     Content = inputText,
//                     Type = Document.Types.Type.PlainText
//                 }, new AnnotateTextRequest.Types.Features() { ExtractSyntax = true });
//                 return response;
//             }
//             catch
//             {
//                 DebugOutput.Log($"FAiled to use GetAnnotateTextResponse  {inputText}");
//                 return null;

//             }
//         }

//         public static ClassifyTextResponse? GetClassifyTextResponse(string inputText)
//         {
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;
//             DebugOutput.OutputMethod("GetClassifyTextResponse", $"{inputText}");
//             try
//             {      
//                 var client = LanguageServiceClient.Create();
//                 var response = client.ClassifyText(new Document()
//                 {
//                     Content = inputText,
//                     Type = Document.Types.Type.PlainText
//                 });
//                 return response;
//             }
//             catch
//             {
//                 DebugOutput.Log($"FAiled to use GetClassifyTextResponse  {inputText}");
//                 return null;
//             }
//         }

//         public static AnalyzeSentimentResponse? GetAnalyzeSentimentResponse(string inputText)
//         {
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;
//             DebugOutput.OutputMethod("GetAnalyzeSentimentResponse", $"{inputText}");
//             try
//             {
//                 var client = LanguageServiceClient.Create();
//                 var response = client.AnalyzeSentiment(new Document()
//                 {
//                     Content = inputText,
//                     Type = Document.Types.Type.PlainText
//                 });
//                 return response;
//             }
//             catch
//             {
//                 DebugOutput.Log($"FAiled to use GetAnalyzeSentimentResponse  {inputText}");
//                 return null;
//             }
//         }

//         public static AnalyzeEntitiesResponse? GetAnalyzeEntitiesResponse(string inputText)
//         {
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;
//             DebugOutput.OutputMethod("GetAnalyzeEntitiesResponse", $"{inputText}");
//             try
//             {
//                 var client = LanguageServiceClient.Create();
//                 var response = client.AnalyzeEntities(new Document()
//                 {
//                     Content = inputText,
//                     Type = Document.Types.Type.PlainText
//                 });
//                 return response;
//             }
//             catch
//             {
//                 DebugOutput.Log($"Failed to GetAnalyzeEntitiesResponse {inputText}");
//                 return null;
//             }
//         }

//         public static AnalyzeSyntaxResponse? GetAnalyzeSyntaxResponse(string inputText)
//         {
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;
//             DebugOutput.OutputMethod("GetAnalyzeSyntaxResponse", $"{inputText}");
//             try
//             {
//                 // Initialize the Natural Language client
//                 var client = LanguageServiceClient.Create();
//                 // Create a document with plain text content
//                 var document = new Document
//                 {
//                     Content = inputText,
//                     Type = Document.Types.Type.PlainText
//                 };

//                 // Analyze the syntax (part-of-speech) of the document
//                 var response = client.AnalyzeSyntax(document);
//                 return response;    
//             }
//             catch
//             {
//                 DebugOutput.Log($"we have had an issue with GOOGLE API GetAnalyzeSyntaxResponse {inputText}");
//                 return null;
//             }
//         }

//         public static AnalyzeSyntaxResponse? GetPartOfSpeach(string inputText)
//         {
//             DebugOutput.OutputMethod("GetPartOfSpeach", $"{inputText}");
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;
//             var response = GetAnalyzeSyntaxResponse(inputText);
//             if (response == null) return null;

//             DebugOutput.Log($"We have this many tokens {response.Tokens.Count}");

//             // Print the part-of-speech tags for each token
//             int tokenCount = 0;
//             foreach (var token in response.Tokens)
//             {
//                 tokenCount++;
//                 DebugOutput.Log($"Token: {token.Text.Content}, POS: {token.PartOfSpeech.Tag}");
//             }
//             return response;
//         }


//         public static double? GetComputeCosineSimilarity(string text1, string text2)
//         {
//             DebugOutput.OutputMethod("GetComputeCosineSimilarity", $"{text1} AND {text2}");
//             if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() != NLP) return null;

//             var response1 = GetAnalyzeSyntaxResponse(text1);
//             var response2 = GetAnalyzeSyntaxResponse(text2);
//             if (response1 == null || response2 == null) return null;

//             var tokens1 = new HashSet<string>();
//             var tokens2 = new HashSet<string>();

//             foreach (var token in response1.Tokens)
//             {
//                 tokens1.Add(token.Text.Content.ToLower());
//             }

//             foreach (var token in response2.Tokens)
//             {
//                 tokens2.Add(token.Text.Content.ToLower());
//             }

//             var intersection = new HashSet<string>(tokens1);
//             intersection.IntersectWith(tokens2);

//             double numerator = intersection.Count;
//             double denominator = Math.Sqrt(tokens1.Count * tokens2.Count);

//             return numerator / denominator;
//         }

//     }
// }
