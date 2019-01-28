using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using CloudCMS.Repositories;

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
            Repository = await Platform.CreateRepositoryAsync();
        }

        public new void Dispose()
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