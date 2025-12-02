
using Core.Configuration;
using Core.Logging;
using Newtonsoft.Json;


namespace Core.NLM
{

    // (int questionNumber, string question, List<Entity>? entites, Sentiment? sentiment, List<Category>? categories, List<string>? sentances, List<Core.NLM.Sentiment>? sentanceSentiment, List<List<Core.NLM.Category>>? sentanceCategories)
    public class AnalysedAnswer
    {
        public int? QuestionNumber { get; set; }
        public string? Question { get; set; }        
        // Original Question will be the same as Question unless the Question being asked is a Varient
        public string? OriginalQuestion { get; set; }
        public List<Entity>? QuestionEntities { get; set; }
        
        public string? AIAnswer { get; set; }
        public List<Entity>? AIEntities { get; set; }
        public Sentiment? AISentiment { get; set; }
        public List<Category>? AICategories { get; set; }     
        public List<string>? AISentances { get; set; }   
        public List<Core.NLM.Sentiment>? AISentanceSentiment { get; set; }
        public List<List<Core.NLM.Category>>? AISentanceCategories { get; set; }

        public List<string>? ExpectedSentances { get; set; }
        public List<Entity>? ExpectedEntities { get; set; }
        public Sentiment? ExpectedSentiment { get; set; }
        public List<Category>? ExpectedCategories { get; set; }
        public string? ExpectedAnswer { get; set;}
        public List<Core.NLM.Sentiment>? ExpectedSentanceSentiment { get; set; }
        public List<List<Core.NLM.Category>>? ExpectedSentanceCategories { get; set; }
        public DateTime? TimeOfTest { get; set; }

        public CompareMeasurements? Measurements { get; set;}
    }
    
    public class Mention
    {
        public string? Content { get; set; }
        public int? Offset { get; set; }
    }

    public class Entity
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Salience { get; set; }
        public List<Mention>? Mention { get; set; }
    }

    public class Sentiment
    {
        public float? Score { get; set; }
        public float? Magnitude { get; set; }
    }

    public class Category
    {
        public string? Name { get; set; }
        public float? Confidence { get; set; }        
    }

    public class CompareMeasurements
    {        
        public double? CosineSimilarity { get; set; }
        public int? LevenshteinDistance { get; set; }
        public double? JaccardSimilarity { get; set; }
    }




    public static class Analyse
    {

        public static string Hello()
        {
            return "hello";
        }        

        /// <summary>
        /// Json Strings of Analyse Make into Model
        /// </summary>
        /// <param name="listOfNLTKModels"></param>
        /// <returns></returns>
        public static List<AnalysedAnswer>? AddJsonStringToListOfAnalusedAnswersSupplied(string jsonString, List<AnalysedAnswer>? currentListOfAnalysesAnswers)
        {
            DebugOutput.OutputMethod("AddJsonStringToListOfAnalusedAnswersSupplied","");
            var newListOfAnalysesAnswers = new List<AnalysedAnswer>();
            if (currentListOfAnalysesAnswers != null) 
            {
                DebugOutput.Log($"We have a previous version of analysed answers");
                newListOfAnalysesAnswers = currentListOfAnalysesAnswers;
            }
            // var firstChar = jsonString.First();
            // if (firstChar.ToString() == "[")
            // {
            //     jsonString = jsonString.Substring(1, jsonString.Length - 2);
            // }
            // convert Json into AnalysedAnswers
            try
            {
                var singleAnswerModel = JsonConvert.DeserializeObject<List<AnalysedAnswer>>(jsonString);
                if (singleAnswerModel == null) return null;
                foreach (var answer in singleAnswerModel)
                {
                    newListOfAnalysesAnswers.Add(answer);
                }
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Something went wrong converting json string to model {ex} {jsonString}");
                return null;
            }
            DebugOutput.Log($"Returning additional model onto list of models!");
            return newListOfAnalysesAnswers;
        }

        public static List<AnalysedAnswer> ConvertNLTKtoAnalysedAnswer(List<Core.NLTK.NLTKAnalysis> listOfNLTKModels)
        {
            var listOfnewAnalysisModels = new List<AnalysedAnswer>();
            foreach (var model in listOfNLTKModels)
            {
                var newAnalysisModel = new AnalysedAnswer();
                newAnalysisModel.Question = model.question;
                newAnalysisModel.OriginalQuestion = model.originalQuestion;
                newAnalysisModel.TimeOfTest = model.TimeOfTest;
                newAnalysisModel.QuestionNumber = model.questionNumber;
                // newAnalysisModel.QuestionEntities = model.QuestionEntities;
                newAnalysisModel.AIAnswer =  model.aiAnswer;
                newAnalysisModel.ExpectedAnswer = model.expectedAnswer;
                var newMeasurement = new CompareMeasurements();
                if (model.comparisonResults != null) newMeasurement.LevenshteinDistance = model.comparisonResults.levenshteinDistance;
                if (model.comparisonResults != null) newMeasurement.JaccardSimilarity = model.comparisonResults.jaccardSimilarity;
                if (model.comparisonResults != null) newMeasurement.CosineSimilarity = model.comparisonResults.cosineSimilarity;
                newAnalysisModel.Measurements = newMeasurement;
                // if (model.comparisonResults != null) newMeasurement. = model.comparisonResults.sentimentDifference;
                var newAISentiment = new Sentiment();
                if (model.comparisonResults != null) newAISentiment.Score = model.comparisonResults.aiAnswerSentiment;
                if (model.comparisonResults != null) newAISentiment.Magnitude = model.comparisonResults.aiMagnitude;
                newAnalysisModel.AISentiment = newAISentiment;
                var newExpectedSentiment = new Sentiment();
                if (model.comparisonResults != null)newExpectedSentiment.Score = model.comparisonResults.expectedAnswerSentiment;
                if (model.comparisonResults != null)newExpectedSentiment.Magnitude = model.comparisonResults.expectedMagnitude;
                newAnalysisModel.ExpectedSentiment = newExpectedSentiment;

                if (model.QuestionEntities != null && model.QuestionEntitiesType != null)newAnalysisModel.QuestionEntities = ConvertEntities(model.QuestionEntities, model.QuestionEntitiesType);
                if (model.AIEntities != null && model.AIEntitiesType != null)newAnalysisModel.AIEntities = ConvertEntities(model.AIEntities, model.AIEntitiesType);
                if (model.ExpectedEntities != null && model.ExpectedEntitiesType != null)newAnalysisModel.AIEntities = ConvertEntities(model.ExpectedEntities, model.ExpectedEntitiesType);

                newAnalysisModel.AICategories = ConvertCategory(model.AICategory, model.AICategoryScore);

                //AI Sentences
                if (model.sentenceResults != null && model.sentenceResults.AISentences != null)
                {
                    DebugOutput.Log($"AI Sentences count: {model.sentenceResults.AISentences.Count}");
                    newAnalysisModel.AISentances = model.sentenceResults.AISentences;
                }
                if (model.sentenceResults != null && model.sentenceResults.AISentencesSentiment != null) newAnalysisModel.AISentanceSentiment = ExtractSentiments(model.sentenceResults.AISentencesSentiment);
                if (model.sentenceResults != null && model.sentenceResults.AISentencesCategory != null) newAnalysisModel.AISentanceCategories = ExtractCategories(model.sentenceResults.AISentencesCategory);

                //Expected Sentences
                if (model.sentenceResults != null)newAnalysisModel.ExpectedSentances = model.sentenceResults.expectedSentances;
                if (model.sentenceResults != null && model.sentenceResults.expectedSentencesSentiment != null) newAnalysisModel.ExpectedSentanceSentiment = ExtractSentiments(model.sentenceResults.expectedSentencesSentiment);
                if (model.sentenceResults != null && model.sentenceResults.expectedSentencesCategory != null) newAnalysisModel.ExpectedSentanceCategories = ExtractCategories(model.sentenceResults.expectedSentencesCategory);


                listOfnewAnalysisModels.Add(newAnalysisModel);
            }
            return listOfnewAnalysisModels;
        }

        public static List<Entity>? ConvertEntities(List<string> entities, List<string> types)
        {
            if (entities == null && types == null)
                return null;

            var entityList = new List<Entity>();

            if (entities != null && types != null)
            {
                var entityTuples = entities.Zip(types, (name, type) => (name, type));
                entityList.AddRange(entityTuples.Select(entity =>
                    new Entity
                    {
                        Name = entity.name,
                        Type = entity.type,
                        Salience = null,
                        Mention = null
                    }));
            }
            else if (entities != null)
            {
                entityList.AddRange(entities.Select(entity =>
                    new Entity
                    {
                        Name = entity,
                        Type = null,
                        Salience = null,
                        Mention = null
                    }));
            }
            else if (types != null)
            {
                entityList.AddRange(types.Select(entity =>
                    new Entity
                    {
                        Name = null,
                        Type = entity,
                        Salience = null,
                        Mention = null
                    }));
            }

            return entityList;
        }

        public static List<Category>? ConvertCategory(string category, float score)
        {
            if (category == null)
                return null;

            var categoryList = new List<Category>();
            var categoryNew = new Category();

            categoryNew.Name = category;
            categoryNew.Confidence = score;

            categoryList.Add(categoryNew);

            return categoryList;
        }

        public static List<Sentiment> ExtractSentiments(List<float> sentimentScores)
            {
                List<Sentiment> sentiments = new List<Sentiment>();

                if (sentimentScores != null)
                {
                    foreach (var score in sentimentScores)
                    {
                        sentiments.Add(new Sentiment
                        {
                            Score = score
                        });
                    }
                }

                return sentiments;
            }

        public static List<List<Category>> ExtractCategories(List<string> categoryNamesLists)
        {
            List<List<Category>> categoriesLists = new List<List<Category>>();

            if (categoryNamesLists != null)
            {
                List<Category> categories = new List<Category>();
                foreach (var name in categoryNamesLists)
                {
                    categories.Add(new Category
                    {
                        Name = name
                    });
                }
                categoriesLists.Add(categories);
            }

            return categoriesLists;
        }



        public static double GetJaccardSimilarity(string paragraph1, string paragraph2)
        {
            // Tokenize paragraphs into sets of words (you can adjust this based on your needs)
            var words1 = new HashSet<string>(paragraph1.Split());
            var words2 = new HashSet<string>(paragraph2.Split());

            // Calculate intersection and union
            var intersection = words1.Intersect(words2).Count();
            var union = words1.Union(words2).Count();

            // Compute Jaccard similarity
            double jaccardIndex = (double)intersection / union;

            return jaccardIndex;
        }
        
        public static int GetLevenshteinDistance(string sentence1, string sentence2)
        {
            int m = sentence1.Length;
            int n = sentence2.Length;
            // Initialize a 2D array to store distances
            int[,] dp = new int[m + 1, n + 1];
            // Initialize the first row and column
            for (int i = 0; i <= m; i++)
                dp[i, 0] = i;
            for (int j = 0; j <= n; j++)
                dp[0, j] = j;
            // Fill in the rest of the matrix
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = (sentence1[i - 1] == sentence2[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }
            // The final value in the matrix represents the Levenshtein Distance
            return dp[m, n];
        }


        public static string? GetJsonFomrListOfAnalysedAnswers(List<AnalysedAnswer> analysedAnswers)
        {
            DebugOutput.Log($"GetJsonFromAnalysedAnswer ");
            string jsonString = System.Text.Json.JsonSerializer.Serialize(analysedAnswers);
            return jsonString;
        }


        public static string? GetJsonFromAnalysedAnswer(AnalysedAnswer analysedAnswer)
        {
            DebugOutput.Log($"GetJsonFromAnalysedAnswer ");
            string jsonString = System.Text.Json.JsonSerializer.Serialize(analysedAnswer);
            return jsonString;
        }

        public static double? GetCosineSimilarity(string text1, string text2)
        {
            DebugOutput.OutputMethod("GetCosineSimilarity", $"{text1} AND {text2}");
            if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() == "google")
            {
                // return GoogleAPI.GetComputeCosineSimilarity(text1, text2);
                return null;
            }
            return null;
        }


        public static List<Entity>? GetEntities(int questionNumber, string text)
        {
            DebugOutput.Log($"AnalyzeEntities {questionNumber} '{text}'");

            var entities = new List<Entity>();

            if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() == "google")
            {
                return null;
                // var response = GoogleAPI.GetAnalyzeEntitiesResponse(text);
                // if (response == null) return null;
                // foreach (var entity in response.Entities)
                // {
                //     var tempEntity = new Entity();
                //     DebugOutput.Log($"Name: {entity.Name}");
                //     tempEntity.Name = entity.Name;
                //     DebugOutput.Log($"Type: {entity.Type}");
                //     tempEntity.Type = entity.Type.ToString();
                //     DebugOutput.Log($"Salience: {entity.Salience}");
                //     tempEntity.Salience = entity.Salience.ToString();
                //     DebugOutput.Log("Mentions:");
                //     var mentions = new List<Mention>();
                //     foreach (var mention in entity.Mentions)
                //     {
                //         var tempMention = new Mention();
                //         DebugOutput.Log($"  Text: {mention.Text.Content}");
                //         tempMention.Content = mention.Text.Content;
                //         DebugOutput.Log($"  Offset: {mention.Text.BeginOffset}");
                //         tempMention.Offset = mention.Text.BeginOffset;
                //         mentions.Add(tempMention);
                //     }
                //     DebugOutput.Log("-".PadRight(20, '-'));
                //     entities.Add(tempEntity);
                // }
                // return entities;
            }
            return null;
        }

        
        public static Sentiment? GetSentiment(int questionNumber, string text)
        {
            DebugOutput.Log($"AnalyzeSentimentFromText {questionNumber} '{text}'");
            
            if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() == "google")
            {
                return null;
                // var response = GoogleAPI.GetAnalyzeSentimentResponse(text);
                // if (response == null) return null;
                // var sentiment = response.DocumentSentiment;
                // DebugOutput.Log($"Sentiment score: {sentiment.Score}, Sentiment magnitude: {sentiment.Magnitude} ");
                // var returnSentiment = new Sentiment();
                // returnSentiment.Score = sentiment.Score;
                // returnSentiment.Magnitude = sentiment.Magnitude;
                // return returnSentiment;
            }
            return null;
        }

        
        public static List<Category>? GetCategories(int questionNumber, string text)
        {
            DebugOutput.Log($"AnalyzeCategories {questionNumber} '{text}'");
            if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() == "google")
            {                
                try
                {       
                    return null;
                    // var response = GoogleAPI.GetClassifyTextResponse(text);
                    // if (response == null) return null;

                    // int counter = 1;

                    // if (response.Categories.Count < 1) DebugOutput.Log($"No response categories");

                    // var returnListOfCategories = new List<Category>();

                    // foreach (var category in response.Categories)
                    // {
                    //     var tempCategory = new Category();
                    //     DebugOutput.Log($"{counter} Category: {category.Name}, Confidence: {category.Confidence}");
                    //     tempCategory.Name = category.Name;
                    //     tempCategory.Confidence = category.Confidence;
                    //     returnListOfCategories.Add(tempCategory);
                    // }
                    // return returnListOfCategories;
                }
                catch
                {
                    DebugOutput.Log($"Could be too few words to do this!");
                    return null;
                }
            }
            return null;
        }

        
        public static List<string>? GetSentances(int questionNumber, string text)
        {
            DebugOutput.Log($"AnalyzeSyntax {questionNumber} '{text}'");
            if (TargetAIChatBotConfiguration.Configuration.AnswerNLP.ToLower() == "google")
            {
                return null;
                // var response = GoogleAPI.GetAnnotateTextResponse(text);
                // if (response == null) return null;

                // var returnSentances = new List<string>();

                // foreach (var sentence in response.Sentences)
                // {
                //     Console.WriteLine($"Sentence: {sentence.Text.Content}");
                //     returnSentances.Add(sentence.Text.Content);
                // }
                // return returnSentances;
            }
            return null;
        }


    }
}