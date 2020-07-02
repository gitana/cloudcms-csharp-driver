using System.Collections.Generic;
using System.Threading.Tasks;
using CloudCMS.support;
using Newtonsoft.Json.Linq;

namespace CloudCMS.Nodes
{
    public interface INode : IBaseNode
    {
        // Associations
        Task<List<IAssociation>> AssociationsAsync();
        Task<List<IAssociation>> AssociationsAsync(JObject pagination);
        Task<List<IAssociation>> AssociationsAsync(Direction direction);
        Task<List<IAssociation>> AssociationsAsync(QName associationTypeQName);
        Task<List<IAssociation>> AssociationsAsync(QName associationTypeQName, Direction direction, JObject pagination);

        Task<Association> AssociateAsync(INode targetNode, QName associationTypeQName);
        Task<Association> AssociateAsync(INode targetNode, QName associationTypeQName, JObject data);
        Task<Association> AssociateAsync(INode otherNode, QName associationTypeQName, Directionality directionality);
        Task<Association> AssociateAsync(INode otherNode, QName associationTypeQName, Directionality directionality, JObject data);

        Task UnassociateAsync(INode targetNode, QName associationTypeQName);
        Task UnassociateAsync(INode targetNode, QName associationTypeQName, Directionality directionality);
        
        
        // traversal
        // filefolder tree
        // children
        // translations
        // relatives


    }
}