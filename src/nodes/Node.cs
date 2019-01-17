using Newtonsoft.Json.Linq;
using CloudCMS.Documents;
using CloudCMS.Repositories;
using CloudCMS.Branches;


namespace CloudCMS.Nodes
{
    class Node : AbstractRepositoryDocument,
                INode
    {
        public IBranch Branch { get; }

        public string BranchId { get; }

        public Node(IBranch branch, JObject obj)
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
    }
}