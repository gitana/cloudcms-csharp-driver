using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CloudCMS;
using Newtonsoft.Json.Linq;

namespace CloudCMS
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
        
        // File Folder
        Task<JObject> FileFolderTreeAsync();
        Task<JObject> FileFolderTreeAsync(string basePath);

        Task<JObject> FileFolderTreeAsync(string basePath, string leafPath);
        Task<JObject> FileFolderTreeAsync(string basePath, int depth, List<string> leafPaths, bool includeProperties, bool containersOnly, JObject query=null);

        Task<List<IBaseNode>> ListChildrenAsync(JObject pagination=null);
        Task<List<IBaseNode>> ListRelativesAsync(QName type, Direction direction, JObject pagination=null);
        Task<List<IBaseNode>> QueryRelativesAsync(QName type, Direction direction, JObject query, JObject pagination=null);

        // Traverse
        Task<TraversalResults> TraverseAsync(JObject traverse);

        // Translations
        Task<INode> CreateTranslationAsync(string locale, string edition, JObject obj);
        Task<List<string>> GetTranslationEditionsAsync();
        Task<List<string>> GetTranslationLocales(string edition);
        Task<INode> ReadTranslationAsync(string locale, string edition);
    }
}