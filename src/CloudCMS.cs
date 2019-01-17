using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
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
            Driver driver = new Driver();
            return await driver.ConnectAsync(config);
        }
    }
}