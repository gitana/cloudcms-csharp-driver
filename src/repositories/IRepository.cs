using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS;

namespace CloudCMS
{
    public interface IRepository : IDatastore, IReferenceable
    {
        public string PlatformId { get; }

        Task<List<IBranch>> ListBranchesAsync();

        Task<IBranch> ReadBranchAsync(string branchId);
        Task<IBranch> MasterAsync();
    }
}