using System;
using System.Threading.Tasks;
using CloudCMS.Branches;
using CloudCMS.support;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace CloudCMS.Nodes
{
    public class Association : BaseNode, IAssociation
    {
        public Association(IBranch branch, JObject obj) : base(branch, obj)
        {
        }

        public QName SourceNodeTypeQName
        {
            get => QName.create(GetString("source_type"));
            set => SetString("source_type", value.ToString());
        }
        
        public QName TargetNodeTypeQName
        {
            get => QName.create(GetString("target_type"));
            set => SetString("target_type", value.ToString());
        }
        
        public string SourceNodeId
        {
            get => GetString("source");
            set => SetString("source", value);
        }
        
        public string TargetNodeId
        {
            get => GetString("target");
            set => SetString("target", value);
        }
        
        public Directionality Directionality
        {
            get
            {
                Directionality directionality;
                Enum.TryParse(GetString("directionality"), out directionality);
                return directionality;
            }
            set
            {
                SetString("directionality", value.ToString());
            }
        }

        public Task<INode> ReadSourceNodeAsync()
        {
            return this.Branch.ReadNodeAsync(this.SourceNodeId);
        }
        
        public Task<INode> ReadTargetNodeAsync()
        {
            return this.Branch.ReadNodeAsync(this.TargetNodeId);
        }
    }
}