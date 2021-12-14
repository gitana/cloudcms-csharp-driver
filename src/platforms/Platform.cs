using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS;
using System.Net.Http;
using System.Threading;

namespace CloudCMS
{
    class Platform : AbstractDocument,
                     IPlatform
    {
        public Platform(ICloudCMSDriver driver, JObject obj) : base(driver, obj)
        {

        }

        public override string URI
        {
            get
            {
                return "";
            }
        }

        public async Task<List<IRepository>> ListRepositoriesAsync(JObject pagination = null)
        {
            string uri = this.URI + "/repositories";
            
            IDictionary<string, string> queryParams = null;
            if (pagination != null)
            {
                queryParams = pagination.ToObject<Dictionary<string, string>>();
            }
            
            JObject response = await Driver.GetAsync(uri, queryParams);

            List<IRepository> repositories = new List<IRepository>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject repositoryObj = (JObject) row;
                IRepository repository = new Repository(this, repositoryObj);
                repositories.Add(repository);
            }

            return repositories;
        }

        public async Task<IRepository> ReadRepositoryAsync(string repositoryId)
        {
            string uri = this.URI + "/repositories/" + repositoryId;
            IRepository repository = null;
            try
            {
                JObject response = await Driver.GetAsync(uri);
                repository = new Repository(this, response);

            }
            catch (CloudCMSRequestException)
            {
                repository = null;
            }

            return repository;
        }

        public async Task<IRepository> CreateRepositoryAsync(JObject obj = null)
        {
            string uri = this.URI + "/repositories/";
            if (obj == null)
            {
                obj = new JObject();
            }
            HttpContent content = new StringContent(obj.ToString());
            JObject repsonse = await Driver.PostAsync(uri, null, content);

            string repositoryId = repsonse["_doc"].ToString();
            return await ReadRepositoryAsync(repositoryId);
        }

        public async Task<IProject> ReadProjectAsync(string projectId)
        {
            string uri = URI + "/projects/" + projectId;
            JObject response = await Driver.GetAsync(uri);

            return new Project(this, response);
        }

        public async Task<IJob> StartCreateProjectAsync(JObject obj)
        {
            string uri = URI + "/projects/start";
            JObject response = await Driver.PostAsync(uri, null, obj);
            string jobId = response.GetValue("_doc").ToString();

            return await ReadJobAsync(jobId);
        }

        public async Task<List<IProject>> ListProjectsAsync(JObject pagination = null)
        {
            string uri = URI + "/projects";
            
            IDictionary<string, string> queryParams = null;
            if (pagination != null)
            {
                queryParams = pagination.ToObject<Dictionary<string, string>>();
            }

            JObject response = await Driver.GetAsync(uri, queryParams);
            
            List<IProject> projects = new List<IProject>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject projectObj = (JObject) row;
                IProject project = new Project(this, projectObj);
                projects.Add(project);
            }

            return projects;
        }

        public async Task<List<IProject>> QueryProjectsAsync(JObject query, JObject pagination = null)
        {
            string uri = URI + "/projects/query";
            
            IDictionary<string, string> queryParams = null;
            if (pagination != null)
            {
                queryParams = pagination.ToObject<Dictionary<string, string>>();
            }

            JObject response = await Driver.PostAsync(uri, queryParams, query);
            
            List<IProject> projects = new List<IProject>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject projectObj = (JObject) row;
                IProject project = new Project(this, projectObj);
                projects.Add(project);
            }

            return projects;
        }

        public async Task<IJob> ReadJobAsync(string jobId)
        {
            string uri = URI + "/jobs/" + jobId;
            JObject response = await Driver.GetAsync(uri);
            return new Job(Driver, response);
        }

        public async Task<List<IJob>> QueryJobsAsync(JObject query, JObject pagination = null)
        {
            string uri = URI + "/jobs/query";
            IDictionary<string, string> queryParams = null;
            if (pagination != null)
            {
                queryParams = pagination.ToObject<Dictionary<string, string>>();
            }

            JObject response = await Driver.PostAsync(uri, queryParams, query);
            JArray jobsArray = (JArray) response.SelectToken("rows");

            List<IJob> result = new List<IJob>();
            foreach (JToken tok in jobsArray)
            {
                JObject obj = (JObject)tok;
                result.Add(new Job(Driver, obj));
            }

            return result;
        }

        public string TypeId
        {
            get
            {
                return "platform";
            }
        }

        public Reference Ref
        {
            get
            {
                return Reference.create(TypeId, Id);
            }
        }
    }
}