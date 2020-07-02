using System.Threading.Tasks;
using CloudCMS.support;

namespace CloudCMS.Nodes
{
    public interface IAssociation : IBaseNode
    {
        QName SourceNodeTypeQName { get; set; }
        string SourceNodeId { get; set; }
        
        QName TargetNodeTypeQName { get; set; }
        string TargetNodeId { get; set; }
        
        Directionality Directionality { get; set; }
        
        Task<INode> ReadSourceNodeAsync();
        Task<INode> ReadTargetNodeAsync();
    }
}