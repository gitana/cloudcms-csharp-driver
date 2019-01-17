using System;
using System.IO;
using System.Text;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CloudCMS.Platforms;
using CloudCMS.Exceptions;

namespace CloudCMS
{
    class Driver
    {
        private static string TOKEN_URI = "/oauth/token";

        private ConnectionConfig Config;
        private Token token;

        public Driver()
        {
            
        }

        public async Task<IPlatform> ConnectAsync(ConnectionConfig config)
        {
            if (config.clientKey == null)
            {
                throw new OAuthException("Missing required config property clientKey");
            }
            else if (config.clientSecret == null)
            {
                throw new OAuthException("Missing required config property clientSecret");
            }
            else if (config.username == null)
            {
                throw new OAuthException("Missing required config property username");                
            }
            else if (config.password == null)
            {
                throw new OAuthException("Missing required config property password");                
            }
            else if (config.baseURL == null)
            {
                throw new OAuthException("Missing required config property baseURL");                
            }
            
            this.Config = config;
            await GetTokenAsync();
            return await ReadPlatformAsync();
        }

        public async Task<IPlatform> ReadPlatformAsync()
        {
            JObject response = await GetAsync("");
            Platform platform = new Platform(this, response);
            return platform;
        }

        public async Task<JObject> GetAsync(string uri, IDictionary<string, string> queryParams = null)
        {
            HttpMethod method = HttpMethod.Get;
            return await RequestAsync(uri, method, queryParams);
        }

        public async Task<JObject> PostAsync(string uri, IDictionary<string, string> queryParams = null, HttpContent body = null)
        {
            HttpMethod method = HttpMethod.Post;
            return await RequestAsync(uri, method, queryParams, body);
        }

        public async Task<JObject> PutAsync(string uri, IDictionary<string, string> queryParams = null, HttpContent body = null)
        {
            HttpMethod method = HttpMethod.Put;
            return await RequestAsync(uri, method, queryParams, body);
        }

        public async Task<JObject> DeleteAsync(string uri, IDictionary<string, string> queryParams = null)
        {
            HttpMethod method = HttpMethod.Delete;
            return await RequestAsync(uri, method, queryParams);
        }

        public async Task<JObject> RequestAsync(string uri, HttpMethod method, IDictionary<string, string> queryParams = null, HttpContent body = null)
        {
            using (HttpClient client = new HttpClient())
            {
                var uriBuilder = new UriBuilder(Config.baseURL + uri);

                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                // Add "full" parameter unless already set
                if (query["full"] == null)
                {
                    query["full"] = "true";
                }
                // Add all params to query string
                if (queryParams != null)
                {
                    foreach(var kvp in queryParams)
                    {
                        query[kvp.Key] = kvp.Value;
                    }
                }
                uriBuilder.Query = query.ToString();

                string url = uriBuilder.ToString();
            
                HttpRequestMessage request =  new HttpRequestMessage(method, url);
                if (body != null)
                {
                    request.Content = body;
                }

                // Apply OAuth token or refresh if necessary
                if (DateTime.UtcNow >= token.expireTime)
                {
                    await RefreshTokenAsync();
                }
                AuthenticationHeaderValue auth = new AuthenticationHeaderValue(token.token_type, token.access_token);
                client.DefaultRequestHeaders.Authorization = auth;

                HttpResponseMessage response = await client.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new CloudCMSRequestException(responseString);
                }

                return JObject.Parse(responseString);
            }
        }

        private async Task GetTokenAsync()
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<KeyValuePair<string,string>> requestData = new List<KeyValuePair<string,string>>();
                requestData.Add(new KeyValuePair<string,string>("grant_type", "password"));
                requestData.Add(new KeyValuePair<string,string>("client_id", Config.clientKey));
                requestData.Add(new KeyValuePair<string,string>("client_secret", Config.clientSecret));
                requestData.Add(new KeyValuePair<string,string>("username", Config.username));
                requestData.Add(new KeyValuePair<string,string>("password", Config.password));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                HttpResponseMessage response = await client.PostAsync(Config.baseURL + TOKEN_URI, requestBody);
                string responseBody = await response.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<Token>(responseBody);
                token.expireTime = DateTime.UtcNow.AddSeconds(token.expires_in);
            }
        }

        private async Task RefreshTokenAsync()
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<KeyValuePair<string,string>> requestData = new List<KeyValuePair<string,string>>();
                requestData.Add(new KeyValuePair<string,string>("grant_type", "refresh_token"));
                requestData.Add(new KeyValuePair<string,string>("client_id", Config.clientKey));
                requestData.Add(new KeyValuePair<string,string>("client_secret", Config.clientSecret));
                requestData.Add(new KeyValuePair<string,string>("refresh_token", token.refresh_token));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                HttpResponseMessage response = await client.PostAsync(Config.baseURL + TOKEN_URI, requestBody);
                string responseBody = await response.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<Token>(responseBody);
                token.expireTime = DateTime.UtcNow.AddSeconds(token.expires_in);
            }
        }
    }
    

}