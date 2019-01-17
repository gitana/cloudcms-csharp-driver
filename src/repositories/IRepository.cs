using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS.Branches;
using CloudCMS.Documents;

namespace CloudCMS.Repositories
{
    interface IRepository : IDocument
    {
        Task<List<IBranch>> ListBranchesAsync();

        Task<IBranch> ReadBranchAsync(string branchId);
    }
}