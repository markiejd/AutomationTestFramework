using System;
using System.IO;
using Newtonsoft.Json;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;
using AppXAPI;

namespace Core
{
    /// <summary>
    /// Simple GraphQL envelope used for requests/responses.
    /// Property names are bound to JSON names expected by GraphQL.
    /// </summary>
    public class GraphQL
    {
        /// <summary>
        /// GraphQL query or mutation string.
        /// </summary>
        [JsonProperty("query")] // keep wire name stable
        public string? query { get; set; }

        /// <summary>
        /// Optional data payload returned by GraphQL responses we care about.
        /// </summary>
        [JsonProperty("data")] // keep wire name stable
        public Data? data { get; set; }
    }

    /// <summary>
    /// Subset of response data we currently deserialize.
    /// Expand as needed when supporting more operations.
    /// </summary>
    public class Data
    {
        [JsonProperty("getTest")] public GetTest? getTest { get; set; }
    }

    public class GetTest
    {
        [JsonProperty("gherkin")] public string? gherkin { get; set; }
    }

    /// <summary>
    /// Utilities for composing GraphQL payloads and handling XRAY Cloud auth tokens.
    /// </summary>
    public static class GraphQLUtil
    {
        // Persisted filenames for XRAY Cloud auth. Keep constants centralized.
        private const string XrayCloudTokenFileName = "xraycloudtoken.txt";
        private const string XrayCloudClientFileName = "xrayclouduser.json";

        /// <summary>
        /// Build a GraphQL JSON payload safely, ensuring proper escaping.
        /// </summary>
        public static string GetGraphQLQuery(string query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            // Serialize an anonymous object so quotes and special chars are escaped correctly.
            // Produces: {"query":"..."}
            return JsonConvert.SerializeObject(new { query });
        }

        /// <summary>
        /// Serialize a GraphQL model to JSON.
        /// </summary>
        public static string GetGraphQLObject(GraphQL model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            // Formatting.None by default for compact payloads.
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// Deserialize JSON into a GraphQL model. Returns null on failure.
        /// </summary>
        public static GraphQL? GetGraphQLModel(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                return JsonConvert.DeserializeObject<GraphQL>(json);
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Failed to deserialize GraphQL JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieve the current XRAY Cloud JWT token from disk or environment.
        /// Env var fallback: ATF_XRAY_CLOUD_TOKEN
        /// </summary>
        public static string? GetCurrentJWTTokenForXRAY()
        {
            DebugOutput.OutputMethod("GetCurrentJWTTokenForXRAY");

            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;

            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;

            DebugOutput.Log($"Looking for {jiraName}");
            if (jiraDetails.Authorization?.ToLowerInvariant() != "cloud") return null;

            if (TargetJiraConfiguration.Configuration.UserDetailsLocation == null) return null;
            var directory = TargetJiraConfiguration.Configuration.UserDetailsLocation;

            // Build full path safely regardless of trailing slashes.
            var fullFileNameAndPath = Path.Combine(directory, XrayCloudTokenFileName);

            if (!FileUtils.OSFileCheck(fullFileNameAndPath))
            {
                DebugOutput.Log("Token file does not exist. Trying environment variable ATF_XRAY_CLOUD_TOKEN.");

                var envToken = Environment.GetEnvironmentVariable("ATF_XRAY_CLOUD_TOKEN");
                if (!string.IsNullOrWhiteSpace(envToken))
                {
                    DebugOutput.Log("Found token in environment.");
                    return envToken.Trim();
                }

                DebugOutput.Log("No token found in environment.");
                return null;
            }

            DebugOutput.Log("Token file exists.");
            var token = FileUtils.OSGetFileContentsAsString(fullFileNameAndPath)?.Trim();
            return string.IsNullOrWhiteSpace(token) ? null : token;
        }

        /// <summary>
        /// Request a new XRAY Cloud JWT using client credentials and persist it.
        /// Reads client JSON from file (xrayclouduser.json) or env var ATF_XRAY_CLOUD_CLIENT.
        /// </summary>
        public static string? GetNewJWTTokenForXRAY()
        {
            DebugOutput.OutputMethod("GetNewJWTTokenForXRAY");

            var jiraName = TargetConfiguration.Configuration.JiraName;
            if (jiraName == null) return null;

            var jiraDetails = TargetJiraConfiguration.GetJiraConfigurationByName(jiraName);
            if (jiraDetails == null) return null;

            DebugOutput.Log($"Looking for {jiraName}");
            if (jiraDetails.Authorization?.ToLowerInvariant() != "cloud") return null;

            if (TargetJiraConfiguration.Configuration.UserDetailsLocation == null) return null;
            var directory = TargetJiraConfiguration.Configuration.UserDetailsLocation;

            // Prefer local client json file, fall back to environment variable.
            var clientJsonPath = Path.Combine(directory, XrayCloudClientFileName);
            string? jsonContent;
            if (FileUtils.OSFileCheck(clientJsonPath))
            {
                DebugOutput.Log("Client credentials file exists.");
                jsonContent = FileUtils.OSGetFileContentsAsString(clientJsonPath);
                if (string.IsNullOrWhiteSpace(jsonContent)) return null;
            }
            else
            {
                DebugOutput.Log("Client credentials file not found. Checking ATF_XRAY_CLOUD_CLIENT env var.");
                jsonContent = Environment.GetEnvironmentVariable("ATF_XRAY_CLOUD_CLIENT");
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    DebugOutput.Log("No client credentials found in environment.");
                    return null;
                }
            }

            const string AUTH_URL = "https://xray.cloud.getxray.app/api/v2/authenticate";

            // Note: .Result is used to remain synchronous with existing APIUtil pattern.
            // Consider introducing async all the way up to avoid potential deadlocks.
            var result = APIUtil.Post(AUTH_URL, jsonContent, "XRAY", false).Result;
            if (result == null) return null;

            DebugOutput.Log("Auth response received.");

            // The XRAY auth endpoint returns the token as a JSON string (e.g., "eyJ..."), so strip quotes.
            var tokenRaw = result.Content.ReadAsStringAsync().Result;
            DebugOutput.Log("Raw token payload received from XRAY auth endpoint.");

            if (string.IsNullOrWhiteSpace(tokenRaw)) return null;

            var token = tokenRaw.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(token)) return null;

            // Persist token to disk for reuse.
            var tokenPath = Path.Combine(directory, XrayCloudTokenFileName);
            if (FileUtils.OSFileCheck(tokenPath))
            {
                // Remove old token file to avoid stale content.
                if (!FileUtils.OSFileDeletion(tokenPath)) return null;
            }

            DebugOutput.Log("Persisting new XRAY token to disk.");
            if (!FileUtils.OSFileCreationAndPopulation(directory, XrayCloudTokenFileName, token)) return null;

            return token;
        }
    }
}