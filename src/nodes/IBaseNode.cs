using System.Collections.Generic;
using System.Threading.Tasks;
using CloudCMS;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public interface IBaseNode : IRepositoryDocument, IAttachable
    {
        IBranch Branch { get; }

        string BranchId { get; }
        
        QName TypeQName { get; }
        QName QName { get; }

        Task RefreshAsync();
        
        // Features
        List<string> GetFeatureIds();
        JObject GetFeature(string featureId);
        bool HasFeature(string featureId);
        Task AddFeatureAsync(string featureId, JObject featureConfig);
        Task RemoveFeatureAsync(string featureId);
        
        // Versions
        Task<IBaseNode> ReadVersionAsync(string changesetId, JObject options = null);
        Task<List<IBaseNode>> ListVersionsAsync(JObject options = null, JObject pagination = null);
        Task<IBaseNode> RestoreVersionAsync(string changesetId);

        Task ChangeQNameAsync(QName newQName);

        
        string GetString(string field);
        void SetString(string field, string value);


    }
}