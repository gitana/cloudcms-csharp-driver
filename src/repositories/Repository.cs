using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS;

namespace CloudCMS
{
    class Repository : AbstractPlatformDocument,
                        IRepository
    {
        public string PlatformId { get; }

        public Repository(IPlatform platform, JObject obj) : base(platform, obj)
        {
            this.PlatformId = obj.SelectToken("platformId").ToString();
        }

        public override string URI
        {
            get 
            {
                return "/repositories/" + Id;
            }
        }

        public async Task<List<IBranch>> ListBranchesAsync()
        {
            string uri = this.URI + "/branches";
            JObject response = await Driver.GetAsync(uri);

            List<IBranch> branches = new List<IBranch>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject branchObj = (JObject) row;
                IBranch branch = new Branch(this, branchObj);
                branches.Add(branch);
            }

            return branches;
        }

        public async Task<List<IBranch>> QueryBranchesAsync(JObject query, JObject pagination = null)
        {
            string uri = URI + "/branches/query";
            
            IDictionary<string, string> queryParams = null;
            if (pagination != null)
            {
                queryParams = pagination.ToObject<Dictionary<string, string>>();
            }

            JObject response = await Driver.PostAsync(uri, queryParams, query);
            List<IBranch> branches = new List<IBranch>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject branchObj = (JObject) row;
                IBranch branch = new Branch(this, branchObj);
                branches.Add(branch);
            }

            return branches;
        }

        public async Task<IBranch> ReadBranchAsync(string branchId)
        {
            string uri = this.URI + "/branches/" + branchId;
            IBranch branch = null;
            try
            {
                JObject response = await Driver.GetAsync(uri);
                branch = new Branch(this, response);
            }
            catch (CloudCMSRequestException)
            {
                branch = null;
            }
            return branch;
        }

        public Task<IBranch> MasterAsync()
        {
            return ReadBranchAsync("master");
        }

        public async Task<List<IRelease>> ListReleasesAsync()
        {
            string uri = URI + "/releases";
            
            JObject response = await Driver.GetAsync(uri);
            
            List<IRelease> releases = new List<IRelease>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject releaseObj = (JObject) row;
                IRelease release = new Release(this, releaseObj);
                releases.Add(release);
            }

            return releases;
        }

        public async  Task<List<IRelease>> QueryReleasesAsync(JObject query, JObject pagination = null)
        {
            string uri = URI + "/releases/query";
            
            IDictionary<string, string> queryParams = null;
            if (pagination != null)
            {
                queryParams = pagination.ToObject<Dictionary<string, string>>();
            }

            JObject response = await Driver.PostAsync(uri, queryParams, query);
            
            List<IRelease> releases = new List<IRelease>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject releaseObj = (JObject) row;
                IRelease release = new Release(this, releaseObj);
                releases.Add(release);
            }

            return releases;
        }

        public async Task<IRelease> ReadReleaseAsync(string releaseId)
        {
            string uri = this.URI + "/releases/" + releaseId;
            
            JObject response = await Driver.GetAsync(uri);
            IRelease release = new Release(this, response);

            return release;
        }

        public async Task<IJob> StartCreateReleaseAsync(JObject obj, string sourceReleaseId = null)
        {
            string uri = URI + "/releases/create/start";
            
            IDictionary<string, string> queryParams = null;
            if (sourceReleaseId != null)
            {
                queryParams.Add("sourceId", sourceReleaseId);
            }

            JObject response = await Driver.PostAsync(uri, queryParams, obj);
            string jobId = response.GetValue("_doc").ToString();
            
            return await Platform.ReadJobAsync(jobId);
        }


        public override string TypeId
        {
            get
            {
                return "repository";
            }
        }

        public override Reference Ref
        {
            get
            {
                return Reference.create(TypeId, PlatformId, Id);
            }
        }
    }
}