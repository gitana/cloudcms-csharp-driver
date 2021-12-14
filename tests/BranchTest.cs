using Xunit;
using System.Collections.Generic;
using CloudCMS;
using Newtonsoft.Json.Linq;

namespace CloudCMS.Tests
{
    public class BranchTest : AbstractWithRepositoryTest
    {
        public BranchTest(RepositoryFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public async void TestBranches()
        {
            IRepository repository = Fixture.Repository;
            List<IBranch> branches = await repository.ListBranchesAsync();
            Assert.True(branches.Count > 0);

            List<IBranch> queriedBranches = await repository.QueryBranchesAsync(new JObject());
            Assert.NotEmpty(queriedBranches);

            IBranch branch = await repository.ReadBranchAsync("master");
            string expectedRef = "branch://" + repository.PlatformId + "/" + repository.Id + "/" + branch.Id;
            Assert.Equal("/repositories/" + repository.Id + "/branches/" + branch.Id, branch.URI);
            Assert.True(branch.IsMaster());
            Assert.Equal(expectedRef, branch.Ref.Ref);

            branch = await repository.ReadBranchAsync("I'm not real");
            Assert.Null(branch);
        }
    }
}