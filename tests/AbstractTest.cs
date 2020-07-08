using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using CloudCMS;

namespace CloudCMS.Tests
{
    public class PlatformFixture : IDisposable
    {
        public PlatformFixture()
        {
            SetupAsync().Wait();
        }

        public virtual void Dispose()
        {
            
        }
        private async Task SetupAsync()
        {
            using (StreamReader file = File.OpenText("gitana.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject json = (JObject)JToken.ReadFrom(reader);
                // Login as admin for testing
                json["username"] = "admin";
                json["password"] = "admin";
                ConnectionConfig config = json.ToObject<ConnectionConfig>();
                
                Platform = await CloudCMSDriver.ConnectAsync(config);
            }
        }

        public IPlatform Platform { get; private set; }
    }

    public abstract class AbstractTest<T> : IClassFixture<T> where T: PlatformFixture
    {
        protected T Fixture;

        public AbstractTest(T fixture)
        {
            this.Fixture = fixture;
        }
    }
}