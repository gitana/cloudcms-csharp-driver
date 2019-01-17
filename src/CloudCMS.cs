using System.Threading.Tasks;
using System.IO;
using CloudCMS.Platforms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CloudCMS
{
    class CloudCMS
    {
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

        public static async Task<IPlatform> ConnectAsync(ConnectionConfig config)
        {
            Driver driver = new Driver();
            return await driver.ConnectAsync(config);
        }
    }
}