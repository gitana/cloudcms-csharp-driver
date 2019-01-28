using Xunit;

namespace CloudCMS.Tests
{
    public class PlatformTest : AbstractTest<PlatformFixture>
    {
        public PlatformTest(PlatformFixture platformFixture) : base(platformFixture)
        {

        }

        [Fact]
        public void TestPlatforms()
        {
            Assert.Equal("Root Platform", Fixture.Platform.Data.GetValue("title"));
            Assert.Equal("", Fixture.Platform.URI);
        }
    }
}