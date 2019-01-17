using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS.Branches;
using CloudCMS.Documents;

namespace CloudCMS.Repositories
{
    class Repository : AbstractDocument,
                        IRepository
    {
        public string PlatformId { get; }

        public Repository(Driver driver, JObject obj) : base(driver, obj)
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

        public async Task<IBranch> ReadBranchAsync(string branchId)
        {
            string uri = this.URI + "/branches/" + branchId;
            JObject response = await Driver.GetAsync(uri);

            IBranch branch = new Branch(this, response);
            return branch;
        }
    }
}