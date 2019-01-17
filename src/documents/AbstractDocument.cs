using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace CloudCMS.Documents
{
    public abstract class AbstractDocument : IDocument
    {
        public string Id { get; set; }

        public JObject Data { get; set; }

        public abstract string URI { get; }

        public CloudCMSDriver Driver { get; }

        protected AbstractDocument(CloudCMSDriver driver, JObject obj)
        {
            this.Driver = driver;
            this.Id = obj.GetValue("_doc").ToString();
            this.Data = obj;
        }

        public async Task ReloadAsync()
        {
            JObject data = await Driver.GetAsync(URI);
            this.Data = data;
        }

        public async Task DeleteAsync()
        {
            await Driver.DeleteAsync(URI);
        }

        public async Task UpdateAsync()
        {
            HttpContent content = new StringContent(Data.ToString());
            await Driver.PutAsync(URI, body: content);
        }
    }
}