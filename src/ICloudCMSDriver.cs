using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public interface ICloudCMSDriver
    {
        Task<JObject> RequestAsync(string uri, HttpMethod method, IDictionary<string, string> queryParams = null, HttpContent body = null);
        Task<JObject> GetAsync(string uri, IDictionary<string, string> queryParams = null);
        Task<JObject> PostAsync(string uri, IDictionary<string, string> queryParams = null, HttpContent body = null);
        Task<JObject> PutAsync(string uri, IDictionary<string, string> queryParams = null, HttpContent body = null);
        Task<JObject> DeleteAsync(string uri, IDictionary<string, string> queryParams = null);
        Task<Stream> DownloadAsync(string uri);
        Task<byte[]> DownloadBytesAsync(string uri);
        Task UploadAsync(string uri, byte[] bytes, string mimetype, IDictionary<string, string> paramMap = null);
        Task UploadAsync(string uri, Stream stream, string mimetype, IDictionary<string, string> paramMap = null);
        Task UploadAsync(string uri, IDictionary<string, string> paramMap, IDictionary<string, AttachmentContent> payloads);

    }
}