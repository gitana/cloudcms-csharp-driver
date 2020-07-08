using System.Threading.Tasks;
using CloudCMS;

namespace CloudCMS
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