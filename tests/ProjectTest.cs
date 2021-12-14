using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CloudCMS.Tests
{
    public class ProjectTest : AbstractWithRepositoryTest
    {
        public ProjectTest(RepositoryFixture fixture) : base(fixture)
        {
        }
        
        [Fact]
        public async void TestProjects()
        {
            IPlatform platform = Fixture.Platform;
            string title = "Test Project" + Environment.TickCount;
            JObject projectObj = new JObject(new JProperty("title", title));

            IJob job = await platform.StartCreateProjectAsync(projectObj);
            await job.WaitForCompletion();

            string projectId = job.Data["created-project-id"].ToString();
            IProject project = await platform.ReadProjectAsync(projectId);
            Assert.Equal(title, project.Data["title"].ToString());

            List<IProject> projects = await platform.ListProjectsAsync(new JObject(new JProperty("limit", 5)));
            Assert.True(projects.Count > 0);
            
            List<IProject> queriedProjects = await platform.QueryProjectsAsync(new JObject(), new JObject(new JProperty("limit", 5)));
            Assert.True(queriedProjects.Count > 0);
        }
    }
}