using Xunit;
using System.Collections.Generic;
using CloudCMS.Repositories;
using CloudCMS.Branches;

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

            IBranch branch = await repository.ReadBranchAsync("master");
            Assert.Equal("/repositories/" + repository.Id + "/branches/" + branch.Id, branch.URI);
            Assert.True(branch.IsMaster());

            branch = await repository.ReadBranchAsync("I'm not real");
            Assert.Null(branch);
        }
    }
}