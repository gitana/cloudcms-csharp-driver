using System.Threading.Tasks;
using CloudCMS.Branches;
using CloudCMS.Repositories;

namespace CloudCMS.Nodes
{
    public interface IBaseNode : IRepositoryDocument
    {
        IBranch Branch { get; }

        string BranchId { get; }

        Task RefreshAsync();

        string GetString(string field);
        void SetString(string field, string value);


    }
}