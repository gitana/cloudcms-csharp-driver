using System.Threading.Tasks;
using CloudCMS.Branches;
using CloudCMS.Repositories;
using Newtonsoft.Json.Linq;

namespace CloudCMS.Nodes
{
    public abstract class BaseNode : AbstractRepositoryDocument, IBaseNode
    {
        public IBranch Branch { get; }

        public string BranchId { get; }

        protected BaseNode(IBranch branch, JObject obj)
            :base(branch.Repository, obj)
        {
            this.Branch = branch;
            this.BranchId = branch.Id;
        }
        
        public override string URI
        {
            get
            {
                return Branch.URI + "/nodes/" + this.Id;
            }
        }

        public Task RefreshAsync()
        {
            string uri = URI + "/refresh";
            return Driver.PostAsync(uri);
        }

        public string GetString(string field)
        {
            return this.Data[field] != null ? (string) this.Data[field] : null;
        }

        public void SetString(string field, string value)
        {
            this.Data[field] = value;
        }
    }
}