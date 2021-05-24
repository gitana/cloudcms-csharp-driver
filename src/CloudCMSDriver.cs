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
using CloudCMS;

namespace CloudCMS
{
    public class CloudCMSDriver : ICloudCMSDriver
    {
        // Static connection methods

        public static async Task<IPlatform> ConnectAsync(string configPath)
        {
            using (StreamReader file = File.OpenText(configPath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject json = (JObject)JToken.ReadFrom(reader);
                ConnectionConfig config = json.ToObject<ConnectionConfig>();
                
                return await ConnectAsync(config);
            }
        }

        public static async Task<IPlatform> ConnectAsync(IDictionary<string, string> configDict)
        {
            JObject configObject = JObject.FromObject(configDict);
            return await ConnectAsync(configObject);
        }

        public static async Task<IPlatform> ConnectAsync(JObject configObject)
        {
            ConnectionConfig config = configObject.ToObject<ConnectionConfig>();
            return await ConnectAsync(config);
        }

        public static async Task<IPlatform> ConnectAsync(ConnectionConfig config)
        {
            CloudCMSDriver driver = new CloudCMSDriver();
            return await driver.doConnectAsync(config);
        }

        // Instance methods

        private static string TOKEN_URI = "/oauth/token";

        private ConnectionConfig Config;
        private OAuthToken token;

        protected CloudCMSDriver()
        {
            
        }

        private async Task<IPlatform> doConnectAsync(ConnectionConfig config)
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

        public async Task<Stream> DownloadAsync(string uri)
        {
            HttpResponseMessage response = await _requestAsync(uri, HttpMethod.Get);
            return await response.Content.ReadAsStreamAsync();
        }
        
        public async Task<byte[]> DownloadBytesAsync(string uri)
        {
            HttpResponseMessage response = await _requestAsync(uri, HttpMethod.Get);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task UploadAsync(string uri, byte[] bytes, string mimetype, IDictionary<string, string> paramMap=null)
        {
            paramMap ??= new Dictionary<string, string>();
            HttpContent content = GenerateUploadContent(bytes, mimetype);
            await _requestAsync(uri, HttpMethod.Post, paramMap, content);
        }

        public async Task UploadAsync(string uri, Stream stream, string mimetype, IDictionary<string, string> paramMap=null)
        {
            paramMap ??= new Dictionary<string, string>();
            HttpContent content = GenerateUploadContent(stream, mimetype);
            await _requestAsync(uri, HttpMethod.Post, paramMap, content);
        }

        public async Task UploadAsync(string uri, IDictionary<string, string> paramMap, IDictionary<string, AttachmentContent> payloads)
        {
            HttpContent content = GenerateUploadContent(payloads);
            await _requestAsync(uri, HttpMethod.Post, paramMap, content);
        }
        
        private HttpContent GenerateUploadContent(byte[] bytes, string mimetype)
        {
            return new AttachmentContent(bytes, mimetype);
        }
        
        private HttpContent GenerateUploadContent(Stream stream, string mimetype)
        {
            return new AttachmentContent(stream, mimetype);

        }

        private HttpContent GenerateUploadContent(IDictionary<string, AttachmentContent> payloads)
        {
            MultipartFormDataContent multi = new MultipartFormDataContent();
            foreach (var kv in payloads)
            {
                string filename = kv.Key;
                AttachmentContent payload = kv.Value;
                
                multi.Add(payload, filename, filename);
            }

            return multi;
        }


        public async Task<JObject> RequestAsync(string uri, HttpMethod method, IDictionary<string, string> queryParams = null, HttpContent body = null)
        {
            HttpResponseMessage response = await _requestAsync(uri, method, queryParams, body);
            string responseString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseString);
        }
        
        public async Task<string> RequestStringAsync(string uri, HttpMethod method, IDictionary<string, string> queryParams = null, HttpContent body = null)
        {
            HttpResponseMessage response = await _requestAsync(uri, method, queryParams, body);
            string responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        private async Task<HttpResponseMessage> _requestAsync(string uri, HttpMethod method, IDictionary<string, string> queryParams = null, HttpContent body = null)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = BuildUrl(uri, queryParams);

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
                if (!response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    throw new CloudCMSRequestException(responseString);
                }
                
                return response;
            }
        }

        private string BuildUrl(string uri, IDictionary<string, string> queryParams)
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
                foreach (var kvp in queryParams)
                {
                    query[kvp.Key] = kvp.Value;
                }
            }

            uriBuilder.Query = query.ToString() ?? "";

            return uriBuilder.ToString();
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

                token = JsonConvert.DeserializeObject<OAuthToken>(responseBody);
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

                token = JsonConvert.DeserializeObject<OAuthToken>(responseBody);
                token.expireTime = DateTime.UtcNow.AddSeconds(token.expires_in);
            }
        }
    }
    

}