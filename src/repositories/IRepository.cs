using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public interface IRepository : IPlatformDocument, IDatastore
    {
        public string PlatformId { get; }

        Task<List<IBranch>> ListBranchesAsync();
        Task<List<IBranch>> QueryBranchesAsync(JObject query, JObject pagination = null);

        Task<IBranch> ReadBranchAsync(string branchId);
        Task<IBranch> MasterAsync();
        
        // Releases

        Task<List<IRelease>> ListReleasesAsync();
        Task<List<IRelease>> QueryReleasesAsync(JObject query, JObject pagination = null);
        Task<IRelease> ReadReleaseAsync(string releaseId);
        Task<IJob> StartCreateReleaseAsync(JObject obj, string sourceReleaseId = null);
    }
}