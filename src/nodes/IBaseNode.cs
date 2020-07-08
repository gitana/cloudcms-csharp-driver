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

        
        string GetString(string field);
        void SetString(string field, string value);


    }
}