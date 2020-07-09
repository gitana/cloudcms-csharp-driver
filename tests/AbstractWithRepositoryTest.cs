using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using CloudCMS;

namespace CloudCMS.Tests
{
    public class RepositoryFixture : PlatformFixture
    {
        public RepositoryFixture() : base()
        {
            SetupAsync().Wait();
        }

        private async Task SetupAsync()
        {
            JObject obj = new JObject();
            obj["title"] = "C# Driver Test Repository";
            Repository = await Platform.CreateRepositoryAsync(obj);
        }

        
        public override void Dispose()
        {
            Repository.DeleteAsync().Wait();
        }

        public IRepository Repository { get; private set; }
    }

    public abstract class AbstractWithRepositoryTest : AbstractTest<RepositoryFixture>
    {
        public AbstractWithRepositoryTest(RepositoryFixture fixture) : base(fixture)
        {

        }
    }
}