using Newtonsoft.Json.Linq;
using Xunit;

namespace CloudCMS.Tests
{
    public class GraphqlTest : AbstractWithRepositoryTest
    {
        public GraphqlTest(RepositoryFixture fixture) : base(fixture)
        {
            
        }

        [Fact]
        public async void TestGraphql()
        {
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");

            string schema = await branch.GraphqlSchemaAsync();
            Assert.NotNull(schema);

            string query = "query { n_nodes { title } }";
            JObject result = await branch.GraphqlQueryAsync(query);
            Assert.NotNull(result);
        }
    }
}