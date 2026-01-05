using AppXAPI;
using Core.FileIO;
using Core.Logging;
using Core.Transformations;
using Core.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Holds the last HTTP response returned by APIUtil. Intended for diagnostics only.
    /// </summary>
    public static class APIResponse
    {
        /// <summary>
        /// The most recent HttpResponseMessage captured by APIUtil methods.
        /// </summary>
        public static HttpResponseMessage? fullResponse { get; set; }
    }

    /// <summary>
    /// General-purpose HTTP helper methods used across the framework.
    /// </summary>
    public class APIUtil
    {

        private static HttpResponseMessage SetResponse(HttpResponseMessage response)
        {
            APIResponse.fullResponse = response;
            return response;
        }


        //////////   GET
        ///
        
        /// <summary>
        /// Executes a simple GET request. If successful, saves the response body to a file named after apiName.
        /// </summary>
        /// <param name="url">The endpoint to call.</param>
        /// <param name="apiName">Friendly name used to persist the response to disk for debugging.</param>
        public static async Task<HttpResponseMessage> Get(string url, string apiName = "unknownAPI")
        {
            DebugOutput.OutputMethod($"APIUtil - Get", $" {url} {apiName}  ");
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Use configured short timeout (useful for negative tests)
                    var timeout = TargetConfiguration.Configuration.NegativeTimeout;
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                    try
                    {
                        response = await client.GetAsync(url);
                        var statusCode = (int)response.StatusCode;
                        DebugOutput.Log($"Status Code of {statusCode} received");
                        APIResponse.fullResponse = response;
                        if (response.IsSuccessStatusCode)
                        {
                            // Avoid issuing a second GET; read the body from the existing response.
                            var fullJson = await response.Content.ReadAsStringAsync();
                            WriteAPIJsonToFile(fullJson, apiName);
                            return SetResponse(response);
                        }
                        else
                        {
                            DebugOutput.Log($"Failure code received from API {statusCode}");
                        }
                    }
                    catch (TaskCanceledException e)
                    {
                        DebugOutput.Log($"Request timed out: {e.Message}");
                        return SetResponse(response);
                    }
                    catch
                    {
                        DebugOutput.Log($"Issue here with API {apiName}");
                        return SetResponse(response);
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Issue USING httpClient");
            }
            return SetResponse(response);
        }
        

        /// <summary>
        /// Retrieves a specific cookie value from an HTTP response by sending a GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL endpoint to send the GET request to. Defaults to "http://".</param>
        /// <param name="cookieTitle">The name of the cookie to extract from the Set-Cookie header. Defaults to "JSESSIONID".</param>
        /// <returns>The cookie value string if found and successfully parsed; otherwise, null.</returns>
        /// <remarks>
        /// This method sends a GET request and inspects the Set-Cookie response headers to locate the requested cookie.
        /// The cookie value is cleaned/normalized using FixJSessionCookie before being returned.
        /// </remarks>
        public static  async Task<string?> GetCookie(string url = "http://", string cookieTitle = "JSESSIONID")
        {
            // Log the method entry with parameters for debugging
            DebugOutput.OutputMethod($"APIUtil - GetCookie", $" {url} {cookieTitle} ");
            
            // Initialize an empty response object to hold the HTTP response
            HttpResponseMessage response = new HttpResponseMessage();
            
            try
            {
                // Create an HttpClient with a default handler for making the HTTP request
                using (HttpClient client = new HttpClient(new HttpClientHandler() {  }))
                {
                    try
                    {
                        // Send GET request to the specified URL, reading only headers initially for performance
                        response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        
                        // Check if the response status code indicates failure (not 2xx)
                        if (!response.IsSuccessStatusCode)
                        {
                            DebugOutput.Log($"Failed code returned! returning null");
                            return null;
                        }
                        
                        // Store the response globally for diagnostic purposes
                        APIResponse.fullResponse = response;
                        DebugOutput.Log($"CODE = {response.StatusCode}");

                        // Attempt to extract the Set-Cookie header from the response headers
                        if (response.Headers.TryGetValues("Set-Cookie", out var setCookieValues))
                        {
                            // Search for the first cookie in the Set-Cookie header that matches the requested cookie name (case-insensitive)
                            var cookie = setCookieValues.FirstOrDefault(x => x.StartsWith(cookieTitle, StringComparison.OrdinalIgnoreCase));
                            
                            // If a matching cookie was found, process and return it
                            if (!string.IsNullOrEmpty(cookie))
                            {
                                // Clean up the cookie value by removing extraneous data (e.g., path, domain)
                                cookie = FixJSessionCookie(cookie);
                                DebugOutput.Log($"Cookie {cookieTitle} = {cookie}");
                                return cookie;
                            }
                        }
                        
                        // Log when the cookie header exists but the specific cookie wasn't found
                        DebugOutput.Log($"Failed to populate cookie");
                    }
                    catch
                    {
                        // Catch any exceptions during the GET request or cookie extraction
                        DebugOutput.Log($"Failed when trying to get {cookieTitle}");
                    }
                    
                    // Return null if cookie extraction failed
                    return null;
                }
            }
            catch
            {
                // Catch any exceptions during HttpClient creation or usage
                DebugOutput.Log($"Failed when trying to use HTTPCLIENT for {cookieTitle} on {url}");
            }
            
            // Return null if the entire operation failed
            return null;
        }
        
        /// <summary>
        /// Retrieves raw data from a Jira endpoint using an access token.
        /// </summary>
        /// <param name="API_Access_Token">Bearer token used for Authorization header.</param>
        /// <param name="apiUrl">The full Jira API URL to call (defaults to "https://").</param>
        /// <returns>Response body as string on success; "Error: {status}" or "Exception: {message}" on failure.</returns>
        public static string GetJiraData(string API_Access_Token = "", string apiUrl = "https://")
        {
            // NOTE: Uses HttpWebRequest for legacy compatibility. Prefer HttpClient for new code.
            try
            {
                #pragma warning disable SYSLIB0014 // Type or member is obsolete
                // Create a legacy HttpWebRequest - kept for backwards compatibility with existing callers.
                var request = (HttpWebRequest)WebRequest.Create(apiUrl);
                #pragma warning restore SYSLIB0014 // Type or member is obsolete
                request.Method = "GET";
                // Add the Authorization header using the provided bearer token.
                request.Headers.Add("Authorization", "Bearer " + API_Access_Token);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Read the response stream to the end and return the payload as a UTF8 string.
                        using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        // Non-success HTTP status code - return a readable error string.
                        return $"Error: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                // Surface exceptions as a prefixed string to avoid throwing from legacy callers.
                return $"Exception: {ex.Message}";
            }
        }

        
        /// <summary>
        /// Attempts to sign on to the configured API server and returns the JSESSIONID
        /// issued for the authenticated session. The method will retrieve an initial
        /// JSESSIONID, fall back to configured application credentials when none are
        /// provided, post the credentials to the server's j_security_check endpoint,
        /// and return the session id if the POST succeeds.
        /// </summary>
        /// <param name="serverURL">Optional server logon URL. If empty, the configured APIServer + "/logon" is used.</param>
        /// <param name="userName">Optional username for logon. Falls back to AppUserName from configuration when empty.</param>
        /// <param name="password">Optional password for logon. Falls back to AppUserPassword from configuration when empty.</param>
        /// <returns>JSESSIONID string on success; otherwise null.</returns>
        public static async Task<string?> GetRedirectionURL(string serverURL = "", string userName = "", string password = "")
        {
            DebugOutput.OutputMethod($"APIUtil - GetRedirectionURL", $" {serverURL} {userName} {password}");
            HttpResponseMessage response = new HttpResponseMessage();
            string url = "";
            if (serverURL == "")
            {
                // Build default logon URL from configuration when none supplied
                url = VariableConfiguration.Configuration.APIServer + @"/logon";
                DebugOutput.Log($"URL set to {url}");
            }

            // Get the initial JSESSIONID value from the server (may include attributes)
            var JSESSIONID = await GetJsessionId(url);
            if (JSESSIONID == null) return null;

            // Normalize cookie value by removing path/domain suffixes if present
            JSESSIONID = JSESSIONID.Replace("; Path=/", "");
            DebugOutput.Log($"Got to here {JSESSIONID}");

            var x = VariableConfiguration.Configuration;

            // Fallback to configured application credentials when none are provided
            if (userName == "")
            {
                userName = x.AppUserName;
            }
            if (password == "")
            {
                password = x.AppUserPassword;
            }
            string content = "j_username=" + userName + "&j_password=" + password;

            var newUrl = x.APIServer;
            // var url = "http://adsmk07-web01.dev.adte/atlascm";
            var baseAddress = new Uri(newUrl);
            var cookieContainer = new CookieContainer();
            DebugOutput.Log($"cookie built");
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                DebugOutput.Log($"handler built");
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    DebugOutput.Log($"client built");

                    // Place the retrieved JSESSIONID into the handler's cookie container
                    cookieContainer.Add(baseAddress, new Cookie("JSESSIONID", JSESSIONID));
                    DebugOutput.Log($"cookie added");

                    // Post credentials to the server's j_security_check endpoint to complete authentication
                    var newUrl1 = url + "/j_security_check";

                    response = await client.PostAsync(newUrl1, new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"));
                    APIResponse.fullResponse = response;    

                    DebugOutput.Log($"CODE = {(int)response.StatusCode}");

                    // If login succeeded, return the normalized JSESSIONID; otherwise fall through to null
                    if (response.IsSuccessStatusCode) return JSESSIONID;
                }       
            }    
            return null;        
        }
        
        /// <summary>
        /// Performs an HTTP GET using a supplied JSESSIONID value by placing it into a CookieContainer
        /// for the request's base address. Returns the HttpResponseMessage and stores it in APIResponse.fullResponse
        /// for diagnostics.
        /// </summary>
        public static async Task<HttpResponseMessage> GetResponseWithJsession(string url, string JSESSIONID)
        {
            DebugOutput.OutputMethod($"APIUtil - GetResponseWithJsession", $" {url} {JSESSIONID} ");
            HttpResponseMessage response = new HttpResponseMessage();
            var cookieContainer = new CookieContainer();
            var baseAddress = new Uri(url);
            // Add the JSESSIONID cookie to the container so the request is sent with the provided session id.
            cookieContainer.Add(baseAddress, new Cookie("JSESSIONID", JSESSIONID));
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                DebugOutput.Log($"handler built");
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    DebugOutput.Log($"client built");
                    // Cookie already added to the container above; no need to add again.

                    // Execute the GET request (reading headers first for efficiency), store the response for diagnostics, and return it.
                    response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    APIResponse.fullResponse = response;    
                    return response;
                }       
            }    
        }

        
        public static async Task<HttpResponseMessage> GetResponseWithAccessToken(string accessToken = "", string apiUrl = "https://")
        {
            DebugOutput.OutputMethod($"APIUtil - GetStringWithAccessToken", $" {accessToken} {apiUrl}");
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;

            // If an explicit access token is provided, prefer it over file/env token.
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            DebugOutput.Log($"Sending GET to {apiUrl} with token auth");
            response = await client.GetAsync(apiUrl);
            APIResponse.fullResponse = response;
            DebugOutput.Log($"{response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                DebugOutput.Log($"Failure code received from API {response.StatusCode}");
                return response;
            }
        }

        
        public static async Task<HttpResponseMessage> GetResponseWithApplicationAuth(string url, string apiName = "unknownAPI", string userName = "", string userPassword = "")
        {
            DebugOutput.OutputMethod($"APIUtil - GetResponseWithApplicationAuth", $" {url} {apiName}  '{userName}' '{userPassword}'");
            HttpResponseMessage response = new HttpResponseMessage();
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{userName}:{userPassword}")));

            response = await client.GetAsync(url);
            APIResponse.fullResponse = response;    
            var statusCode = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                // Avoid a second GET by reading the content from the initial response
                var fullJson = await response.Content.ReadAsStringAsync();
                WriteAPIJsonToFile(fullJson, apiName);
                return SetResponse(response);
            }
            else
            {
                DebugOutput.Log($"Failure code received from API {statusCode}");
            }
            DebugOutput.Log($"{response.StatusCode}");
            return response;
        }

        

        public static async Task<string?> GetStringWithAppAuth(string url, string apiName = "unknownAPI", string userName = "", string userPassword = "")
        {
            DebugOutput.OutputMethod($"APIUtil - GetStringWithAppAuth", $" {url} {apiName} {userName} {userPassword} ");
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("basic", userName, userPassword);
            if (client == null) return null;

            response = await client.GetAsync(url);
            APIResponse.fullResponse = response;    
            DebugOutput.Log($"{response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                // Read content from the response we just fetched
                var fullJson = await response.Content.ReadAsStringAsync();
                return fullJson;
            }
            else
            {
                DebugOutput.Log($"Failure code received from API {response.StatusCode}");
                return null;
            }
        }


        public static async Task<string?> GetStringWithWindowsAuth(string url, string apiName = "unknownAPI", string userName = "", string userPassword = "")
        {
            DebugOutput.OutputMethod($"APIUtil - GetStringWithWindowsAuth", $" {url} {apiName} {userName} {userPassword} ");
            var credentials = new NetworkCredential( userName, userPassword );
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
                {
                    try
                    {
                        response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        var statusCode = (int)response.StatusCode;
                        APIResponse.fullResponse = response;    
                        DebugOutput.Log($"Status Code of {statusCode} received");
                        if (response.IsSuccessStatusCode)
                        {
                            var fullJson = await response.Content.ReadAsStringAsync();
                            return fullJson;
                        }
                        else
                        {
                            DebugOutput.Log($"Failure code received from API {statusCode}");
                            return null;
                        }
                    }
                    catch
                    {
                        DebugOutput.Log($"Issue here with API {apiName}");
                        return null;
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Issue USING httpClient");
            }
            DebugOutput.Log($"DONE and not good!");
            return null;
        }


        public static async Task<string?> GetStringWithJsession(string url, string JSESSIONID)
        {
            DebugOutput.OutputMethod($"APIUtil - GetStringWithJsession", $" {url} {JSESSIONID} ");
            HttpResponseMessage response = new HttpResponseMessage();
            var cookieContainer = new CookieContainer();
            var baseAddress = new Uri(url);
            cookieContainer.Add(baseAddress, new Cookie("JSESSIONID", JSESSIONID));
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                DebugOutput.Log($"handler built");
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    DebugOutput.Log($"client built");
                    // Cookie already present in cookie container; avoid adding a duplicate.

                    response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    var statusCode = (int)response.StatusCode;
                    DebugOutput.Log($"Status Code of {statusCode} received");
                    APIResponse.fullResponse = response;
                    if (response.IsSuccessStatusCode)
                    {
                        var fullJson = await response.Content.ReadAsStringAsync();
                        DebugOutput.Log($"FULL RESPONSE = {APIResponse.fullResponse.Headers}"); 
                        return fullJson;
                    }
                    else
                    {
                        DebugOutput.Log($"Failure code received from API {statusCode}");
                        return null;
                    }
                }       
            }    
        }
        
        
        public static async Task<string?> GetStringWithAccessToken(string accessToken = "", string apiUrl = "", string type = "token", string email = "", string domain = "")
        {
            DebugOutput.Log($"APIUtil - GetStringWithAccessToken {accessToken} {apiUrl}");
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient(type, email, domain);
            if (client == null) return null;            
            DebugOutput.Log($"GetStringWithAccessToken PRIOR");
            response = await client.GetAsync(apiUrl);
            DebugOutput.Log($"GetStringWithAccessToken POST");
            APIResponse.fullResponse = response;    
            DebugOutput.Log($"GetStringWithAccessToken status code {(int)response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                DebugOutput.Log($"Have a rsepnse of {response.StatusCode.ToString()}");
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    var fullJson = await reader.ReadToEndAsync();
                    DebugOutput.Log($"JSON GOTTEN = {fullJson}");
                    return fullJson;
                }
            }
            else
            {
                DebugOutput.Log($"Failure code received from API {response.StatusCode}");
                return null;
            }
        }

        private static HttpClient? GetHttpClient(string authorisation = "", string userName = "", string userPassword = "")
        {
            // Creates an HttpClient configured with a requested auth scheme. For token-based flows
            // we try file on disk first, then fall back to an environment variable.
            var client = new HttpClient();
            switch(authorisation.ToLower())
            {
                case "2fa":
                {
                    DebugOutput.Log($"TOKEN TO BE SET AS 2FA!");
                    var jiraName = TargetConfiguration.Configuration.JiraName;
                    var fileName = $"jiratoken-{jiraName}.txt";
                    var fullFileNameAndPath = $"C:/Drivers/{fileName}";
                    string? token;
                    if (!FileUtils.OSFileCheck(fullFileNameAndPath))
                    {
                        DebugOutput.Log($"Token file not found at {fullFileNameAndPath} maybe I'm a pipeline agent?");
                        token = Environment.GetEnvironmentVariable("JIRA_CLOUD_API_TOKEN");
                    }
                    else
                    {
                        token = FileUtils.OSGetAllTextInFile(fullFileNameAndPath);   
                    }
                    DebugOutput.Log($"Setting token to {token}");
                    var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{token}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    DebugOutput.Log($"Returning 2FA Client!");
                    break;
                    
                }
                case "cloud":
                {
                    DebugOutput.Log($"TOKEN TO BE SET AS cloud!");
                    var jiraName = TargetConfiguration.Configuration.JiraName;
                    var fileName = $"jiratoken-{jiraName}.txt";
                    var fullFileNameAndPath = $"C:/Drivers/{fileName}";
                    string? token;
                    if (FileUtils.OSFileCheck(fullFileNameAndPath))
                    {
                        token = FileUtils.OSGetAllTextInFile(fullFileNameAndPath);   
                    }
                    else
                    {
                        DebugOutput.Log($"Token file not found at {fullFileNameAndPath} maybe I'm a pipeline agent?");
                        token = Environment.GetEnvironmentVariable("JIRA_CLOUD_API_TOKEN");
                    }
                    DebugOutput.Log($"Setting token to {token}");
                    var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{token}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    DebugOutput.Log($"Returning 2FA Client!");
                    break;                    
                }
                case "token":
                {
                    DebugOutput.Log($"TOKEN TO BE SET AS TOKEN!");
                    var jiraName = TargetConfiguration.Configuration.JiraName;
                    var fileName = $"jiratoken-{jiraName}.txt";
                    var fullFileNameAndPath = $"C:/Drivers/{fileName}";
                    string? token;
                    if (!FileUtils.OSFileCheck(fullFileNameAndPath))
                    {
                        DebugOutput.Log($"Unable to find the file {fullFileNameAndPath} maybe I'm a pipeline agent?");
                        token = System.Environment.GetEnvironmentVariable("JIRA_CLOUD_API_TOKEN");
                    }
                    else
                    {
                        token = FileUtils.OSGetAllTextInFile(fullFileNameAndPath);                            
                    }
                    DebugOutput.Log($"Setting token to {token}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    DebugOutput.Log($"Returning TOEKN Client!");
                    break;
                }
                case "basic":
                {
                    DebugOutput.Log($"No Token!");
                    if (userName == "" || userPassword == "") return client;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{userName}:{userPassword}")));
                    DebugOutput.Log($"Returning BASIC Client!");
                    break;
                }
                default: return client;
            }
            return client;
        }
        
        public static async Task<string?> GetJsessionId(string url = "http://adsmk07-web01.dev.adte/atlascm/logon", string cookieTitle = "JSESSIONID")
        {
            DebugOutput.OutputMethod($"APIUtil - GetJsessionId", $" {url} {cookieTitle} ");
            var cookieValue = await GetCookie(url, "JSESSIONID");
            if (cookieValue != null) return cookieValue;
            DebugOutput.Log($"GetJsessionID is returning null!");
            return null;
        }


        public static async Task<HttpResponseMessage> GetWithWindowsAuth(string url, string apiName = "unknownAPI", string userName = "", string userPassword = "")
        {
            DebugOutput.OutputMethod($"APIUtil - GetWithWindowsAuth", $" {url} {apiName}  '{userName}' '{userPassword}'");
            var credentials = new NetworkCredential( userName, userPassword );
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
                {
                    try
                    {
                        response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        var statusCode = (int)response.StatusCode;
                        DebugOutput.Log($"Status Code of {statusCode} received");
                        APIResponse.fullResponse = response;    
                        if (response.IsSuccessStatusCode)
                        {
                            var fullJson = await response.Content.ReadAsStringAsync();
                            WriteAPIJsonToFile(fullJson, apiName);
                            return SetResponse(response);
                        }
                        else
                        {
                            DebugOutput.Log($"Failure code received from API {statusCode}");
                        }
                    }
                    catch
                    {
                        DebugOutput.Log($"Issue here with API {apiName}");
                        return SetResponse(response);
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Issue USING httpClient");
            }
            DebugOutput.Log($"DONE and not good!");
            return SetResponse(response);
        }



        ////////// PATCH
        ///      

        /// <summary>
        /// Sends a PATCH request with JSON payload. Optionally wraps payload in an array for APIs expecting batches.
        /// </summary>
        /// <remarks>Parameter name addSquareBrckets kept for backward compatibility.</remarks>
        public static async Task<HttpResponseMessage> Patch(string url, string jSonText, string apiName = "default", bool addSquareBrckets = true)
        {
            DebugOutput.OutputMethod($"APIUtil - Patch", $" {url} {apiName}");
            if (addSquareBrckets)
            {
                jSonText = "[" + jSonText + "]";
            }   
            HttpResponseMessage response = new HttpResponseMessage();
            DebugOutput.Log($"JSON = {jSonText}");
            using (HttpClient client = new HttpClient(new HttpClientHandler()))
            try
            {
                response = await client.PatchAsync(url, new StringContent(jSonText, Encoding.UTF8, "application/json"));
                var statusCode = (int)response.StatusCode;
                DebugOutput.Log($"Status Code of {statusCode} received");
                APIResponse.fullResponse = response;
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"We are DONE Sucessfully!");
                    return SetResponse(response);
                }
                else
                {
                    DebugOutput.Log($"Failure code received from API {statusCode}");
                }
            }
            catch
            {  
                DebugOutput.Log($"Failed and caught fire here!");
            }
            return SetResponse(response);
        }


        public static async Task<HttpResponseMessage> PatchWithWindowsAuth(string url, string jSonText, string apiName = "default", string userName = "", string userPassword = "", bool addSquareBrckets = true)
        {
            DebugOutput.OutputMethod($"APIUtil - PatchWithWindowsAuth", $" {url} {apiName} {userName} {userPassword}");
            if (addSquareBrckets)
            {
                jSonText = "[" + jSonText + "]";
            }   
            HttpResponseMessage response = new HttpResponseMessage();
            DebugOutput.Log($"JSON = {jSonText}");
            var credentials = new NetworkCredential( userName, userPassword );
            using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
            try
            {
                response = await client.PatchAsync(url, new StringContent(jSonText, Encoding.UTF8, "application/json"));
                var statusCode = (int)response.StatusCode;
                DebugOutput.Log($"Status Code of {statusCode} received");
                APIResponse.fullResponse = response;
                if (response.IsSuccessStatusCode)
                {
                    DebugOutput.Log($"We are DONE Sucessfully!");
                    return SetResponse(response);
                }
                else
                {
                    DebugOutput.Log($"Failure code received from API {statusCode}");
                }
            }
            catch
            {  
                DebugOutput.Log($"Failed and caught fire here!");
            }
            return SetResponse(response);

        }

        ////////// POST
        ///
        

        public static async Task<HttpResponseMessage> Post(string url, string jSonText, string apiName = "default", bool addSquareBrckets = true)
        {
            DebugOutput.OutputMethod($"APIUtil - Post", $" {url} {apiName}");
            if (addSquareBrckets)
            {
                jSonText = "[" + jSonText + "]";
            }   
            DebugOutput.Log($"JSON = {jSonText}");
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler()))
                {
                    response = await client.PostAsync(url, new StringContent(jSonText, Encoding.UTF8, "application/json"));
                    var statusCode = (int)response.StatusCode;
                    DebugOutput.Log($"Status Code of {statusCode} received");
                    APIResponse.fullResponse = response;    
                    if (response.IsSuccessStatusCode)
                    {                        
                        return SetResponse(response);
                    }
                    else
                    {
                        DebugOutput.Log($"Failure code received from API {statusCode}");
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Failed when posting Json!");
            }
            return SetResponse(response);
        }

        
        public static async Task<HttpResponseMessage> PostJsonWithToken(string url, string jsonString)
        {
            DebugOutput.OutputMethod($"APIUtil - PostJsonWithToken", $" {url} {jsonString}");
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PostAsync(url, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostContentWithToken(string url, ByteArrayContent content)
        {
            DebugOutput.Log($"APIUtil - PostContentWithToken {url} {content} ByteArrayContent");
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PostAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostContentWithToken(string url, StringContent content)
        {
            DebugOutput.Log($"APIUtil - PostContentWithToken {url} {content} StringContent");
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PostAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }
        
        public static async Task<bool> PostTrigger(string IngestionType = "BL002")
        {
            var webUrl = VariableConfiguration.Configuration.URL;  //https://ca-ui-firefly-qa2-3411132db.happyriver-d3981a85.uksouth.azurecontainerapps.io
            webUrl = webUrl.Replace("-ui-", "-http-"); 
            var extensionUrl = "/httphandler/api/dataio";
            string url = webUrl + extensionUrl;
            var fileName = IngestionType + ".json";
            string filePath = "./AppTargets/Resources/Variables/" + fileName;
            string contentType = "application/json";

            DebugOutput.OutputMethod("PostTrigger", $"{url} {filePath} {contentType}");
            // does the json file exist in filePath
            if (!File.Exists(filePath))
            {
                DebugOutput.Log($"File {filePath} does not exist.");
                return false;
            }

            try
            {
                // Read the JSON content from the file
                string jsonData = File.ReadAllText(filePath);
                using (HttpClient client = new HttpClient())
                {
                    // Set the Content-Type header
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                    // Create the StringContent with the JSON data
                    var content = new StringContent(jsonData, Encoding.UTF8, contentType);
                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    // Get the HTTP status code as a string
                    string statusCode = ((int)response.StatusCode).ToString();
                    if (response.IsSuccessStatusCode)
                    {
                        DebugOutput.Log($"POST request successful. Status Code: {statusCode}");
                        return true;
                    }
                    else
                    {
                        DebugOutput.Log($"POST request failed. Status Code: {statusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"Error in PostDataIO: {ex.Message}");
                return false;
            }
        }

        public static async Task<HttpResponseMessage> PostZipFileWithWindowsAuth(string url, string zipFilePath, string issueKey, string apiName = "default", string userName = "", string userPassword = "")
        {
            DebugOutput.OutputMethod("PostZipFileWithWindowsAuth", $"{url} {zipFilePath} {issueKey}");
            var credentials = new NetworkCredential(userName, userPassword);
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{userName}:{userPassword}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    // Create a new multipart form data content
                    using (var multipartFormDataContent = new MultipartFormDataContent())
                    {
                        // Add the ZIP file as a file attachment
                        var fileContent = new ByteArrayContent(File.ReadAllBytes(zipFilePath));
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
                        multipartFormDataContent.Add(fileContent, "file", Path.GetFileName(zipFilePath));
                        // Add the required Jira header
                        multipartFormDataContent.Headers.Add("X-Atlassian-Token", "no-check");

                        // Build the URL for the Jira issue attachment API endpoint
                        string attachmentUrl = $"{url}/rest/api/2/issue/{issueKey}/attachments";

                        // Send the POST request to upload the attachment (use the attachmentUrl - bugfix)
                        response = await client.PostAsync(attachmentUrl, multipartFormDataContent);
                        if (response.IsSuccessStatusCode) DebugOutput.Log($"PASSED AND SENT!");
                        else DebugOutput.Log($"FAiled and not sent!");
                        return SetResponse(response);
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Failed when posting Json!");
            }
            return SetResponse(response);
        }


        public static async Task<HttpResponseMessage> PostWithWindowsAuth(string url, string jSonText, string apiName = "default", string userName = "", string userPassword = "", bool addSquareBrckets = true)
        {
            DebugOutput.OutputMethod($"APIUtil - PostWithWindowsAuth", $" {url} {apiName} {userName} {userPassword}");
            if (addSquareBrckets)
            {
                jSonText = "[" + jSonText + "]";
            }   
            DebugOutput.Log($"JSON = {jSonText}");
            var credentials = new NetworkCredential( userName, userPassword );
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
                {
                    response = await client.PostAsync(url, new StringContent(jSonText, Encoding.UTF8, "application/json"));
                    var statusCode = (int)response.StatusCode;
                    APIResponse.fullResponse = response;    
                    DebugOutput.Log($"Status Code of {statusCode} received");
                    if (response.IsSuccessStatusCode)
                    {                        
                        return SetResponse(response);
                    }
                    else
                    {
                        DebugOutput.Log($"Failure code received from API {statusCode}");
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Failed when posting Json!");
            }
            return SetResponse(response);
        }

        
        public static async Task<HttpResponseMessage> PostZipFileWithToken(string url, string zipFilePath)
        {
            DebugOutput.Log($"APIUtil - PostFileWithToken {url} {zipFilePath} "); 
            HttpResponseMessage response = new HttpResponseMessage();
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue/" + "" + "/attachments";
            try
            {
                var client= GetHttpClient("token");
                if (client == null) return response;
                DebugOutput.Log($"We have the token CLIENT!");
                using (var multipartFormDataContent = new MultipartFormDataContent())
                {
                    // Add the ZIP file as a file attachment
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(zipFilePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
                    multipartFormDataContent.Add(fileContent, "file", Path.GetFileName(zipFilePath));
                
                    // Add the required Jira header
                    multipartFormDataContent.Headers.Add("X-Atlassian-Token", "no-check");

                    // Send the POST request to upload the attachment
                    DebugOutput.Log($"ABOUT TO SEND {zipFilePath}");
                    response = await client.PostAsync(url, multipartFormDataContent);
                    DebugOutput.Log($"Response status code is {(int)response.StatusCode}");
                    return response;
                }
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"FAILED with exeception {ex}");
            }
            return response;
        }


        public static async Task<HttpResponseMessage> PostHttpRequestMessage(HttpRequestMessage content)
        {
            DebugOutput.Log($"APIUtil - PostHttpRequestMessage {content}  StringContent"); 
            HttpResponseMessage response = new HttpResponseMessage();
            if (content == null) return response;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    response = await client.SendAsync(content);
                    return response;
                }
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }


        public static async Task<HttpResponseMessage> PostContentWith2FA(string url, System.Net.Http.MultipartFormDataContent content, string userName = "mark.duffy@castlewater.co.uk")
        {
            DebugOutput.Log($"APIUtil - PostContentWith2FA {url} {content}  System.Net.Http.StreamContent"); 
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("cloud", userName);
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PostAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }

        
        public static async Task<HttpResponseMessage> PostContentWithToken(string url, System.Net.Http.MultipartFormDataContent content)
        {
            DebugOutput.Log($"APIUtil - PostContentWithToken {url} {content}  System.Net.Http.StreamContent"); 
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PostAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }            
        }

        public static async Task<HttpResponseMessage> PostContentWithToken(string url, System.Net.Http.StreamContent content)
        {
            DebugOutput.Log($"APIUtil - PostContentWithToken {url} {content}  System.Net.Http.StreamContent"); 
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PostAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }



        ////////// PUT
        ///
        
        
        public static async Task<HttpResponseMessage> PutContentWithToken(string url, StringContent content, string type = "token", string email = "", string domain = "")
        {
            DebugOutput.Log($"APIUtil - PutContentWithToken {url} {content}");
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue";
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient(type, email, domain);
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PutAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }


        public static async Task<HttpResponseMessage> PutStringWithToken(string url = "", string text = "")
        {
            DebugOutput.OutputMethod($"APIUtil - PutStringWithToken", $" {url} {text}");
            // NOTE: Jira comment endpoints typically expect { "body": "..." }. Adjust as needed for the target API.
            var body = new { fields = new { description = text } };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            if (url == "") url = "https://proactionuk.ent.cgi.com/jira/rest/api/2/issue/SDIACSC-280/comment";

            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;
            try
            {
                DebugOutput.Log($"Awaiting repsonse! ");
                response = await client.PutAsync(url, content);
                DebugOutput.Log($"Done response {(int)response.StatusCode}");
                return response;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Thrown {e}");
                return response;
            }
        }

        
        public static async Task<HttpResponseMessage> PutResponseWithToken(string url, string apiName = "unknownAPI", string token = "", string jsonString = "")
        {
            DebugOutput.OutputMethod($"APIUtil - PutResponseWithToken", $" {url} {apiName}");
            DebugOutput.Log($"json - {jsonString}");
            HttpResponseMessage response = new HttpResponseMessage();
            var client= GetHttpClient("token");
            if (client == null) return response;

            response = await client.PutAsync(url, new StringContent(jsonString, Encoding.UTF8, "application/json"));
            APIResponse.fullResponse = response;    
            var statusCode = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                DebugOutput.Log($"Successfully PUT");
                return SetResponse(response);
            }
            else
            {
                DebugOutput.Log($"Failure code received from API {statusCode}");
            }
            DebugOutput.Log($"{response.StatusCode}");
            return response;

        }

        public static async Task<HttpResponseMessage> PutResponseWithApplicationAuth(string url, string apiName = "unknownAPI", string userName = "", string userPassword = "", string jsonString = "")
        {
            DebugOutput.OutputMethod($"APIUtil - PutResponseWithApplicationAuth", $" {url} {apiName}  '{userName}' '{userPassword}'");
            DebugOutput.Log($"json - {jsonString}");
            HttpResponseMessage response = new HttpResponseMessage();
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{userName}:{userPassword}")));

            response = await client.PutAsync(url, new StringContent(jsonString, Encoding.UTF8, "application/json"));
            APIResponse.fullResponse = response;    
            var statusCode = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                DebugOutput.Log($"Successfully PUT");
                return SetResponse(response);
            }
            else
            {
                DebugOutput.Log($"Failure code received from API {statusCode}");
            }
            DebugOutput.Log($"{response.StatusCode}");
            return response;
        }

        ////////// WAIT 
        ///

        public static async Task<HttpResponseMessage> WaitForAPICompletion(string url, string completionCheck, string userName = "", string userPassword = "")
        {
            DebugOutput.OutputMethod($"APIUtil - WaitForAPICompletion", $" {url} {completionCheck} {userName} {userPassword}");
            var credentials = new NetworkCredential( userName, userPassword );            
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
                {
                    try
                    {
                        response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        var statusCode = (int)response.StatusCode;
                        APIResponse.fullResponse = response;    
                        DebugOutput.Log($"Status Code of {statusCode} received");
                        if (response.IsSuccessStatusCode)
                        {
                            DebugOutput.Log($"We have a success code - but is it completed?");
                            int waitTimer = 500;      // milliseconds between polls
                            int fullTimer = 10000;     // overall polling window in milliseconds
                            for (int x = 0; x < fullTimer; x = x + waitTimer)
                            {
                                var fullJson = await client.GetStringAsync(url);
                                DebugOutput.Log($"FULL JSON = {fullJson}");
                                if (fullJson.Contains("ProcessedJournal"))
                                {
                                    DebugOutput.Log($"DONE!");
                                    return SetResponse(response);
                                } 
                                DebugOutput.Log($"NOT DONE YET! {x}");
                                // Don't block the thread inside async code
                                await Task.Delay(waitTimer).ConfigureAwait(false);
                            }
                            return SetResponse(response);
                        }
                        else
                        {
                            DebugOutput.Log($"Failure code received from API {statusCode}");
                        }
                    }
                    catch
                    {
                        DebugOutput.Log($"Issue here with CHECK API");
                        return SetResponse(response);
                    }
                }
            }
            catch
            {
                DebugOutput.Log($"Issue USING httpClient");
                return SetResponse(response);
            }
            DebugOutput.Log($"DONE!");
            return SetResponse(response);
        }




        //////////   PRIVATE AREA


        private static string FixJSessionCookie(string JSESSION)
        {
            DebugOutput.OutputMethod($"APIUtil - FixJSessionCookie", $" {JSESSION} ");
            // Normalize a Set-Cookie value down to the cookie value only
            JSESSION = JSESSION.Replace("JSESSIONID=","");
            var listOfString = StringValues.BreakUpByDelimitedToList(JSESSION,";");
            DebugOutput.Log($"Returning {listOfString[0]}");
            return listOfString[0];
        }

        public static async Task<bool> MyAsyncMethod(long firstTick, long secondTick, string query = "")
        {
            var asyncProc = "MyAsyncMethod";
            var asyncMethodId = firstTick;

            // Perform some synchronous setup
            var x = VariableConfiguration.Configuration;

            await Task.Run(() => 
            {
                // Long-running synchronous below here 
                if (query == "" || query == "x")
                {
                    var userName = x.JiraUserName;
                    var userPassword = x.JiraUserPassword;
                    var url = x.JiraServer + x.JiraAPI + @"2/project";
                    var response = APIUtil.GetResponseWithApplicationAuth(url, "JiraResponse", userName, userPassword);
                }
                // Long-running synchronous above here 
            });
            var howManyTicksToEndAsync = DateTime.Now.Ticks - firstTick;
            var endTick = DateTime.Now.Ticks;
            TargetAsyncReport.NewAsyncReportDataRun(firstTick, secondTick, endTick, asyncProc);


            // More synchronous work can happen here after awaiting
            return true;
        }       
        

        private static bool WriteAPIJsonToFile(string text, string apiName)
        {
            DebugOutput.OutputMethod($"APIUtil - WriteAPIJsonToFile", $" {apiName}  ");
            DebugOutput.Log($"json -  {text}");
            try
            {
                var fileName = $"{apiName}.json";
                var directory = "\\AppXAPI\\APIOutFiles\\";
                var fullFileName = directory + fileName;
                if (!FileUtils.FileDeletion(fullFileName))
                {
                    DebugOutput.Log($"FAIL");
                    return false;
                }
                if (!FileUtils.FilePopulate(fullFileName, text))
                {
                    DebugOutput.Log($"FAIL to create file");
                    return false;
                }
            }
            catch
            {
                DebugOutput.Log($"Also a problem in or around the files");
                return false;
            }
            return true;
        }
    }
}
