using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CloudCMS.Tests
{
    public class ReleaseTest : AbstractWithRepositoryTest
    {
        public ReleaseTest(RepositoryFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void TestReleases()
        {
            IRepository repository = Fixture.Repository;

            JObject releaseObj = new JObject();
            releaseObj.Add("title", "blah");
            IJob releaseJob = await repository.StartCreateReleaseAsync(releaseObj);
            await releaseJob.WaitForCompletion();

            List<IRelease> releases = await repository.ListReleasesAsync();
            Assert.Single(releases);
            IRelease firstRelease = releases[0];

            IRelease release = await repository.ReadReleaseAsync(firstRelease.Id);
            Assert.NotNull(release);

            List<IRelease> queriedReleases = await repository.QueryReleasesAsync(new JObject());
            Assert.Single(queriedReleases);

        }
    }
}