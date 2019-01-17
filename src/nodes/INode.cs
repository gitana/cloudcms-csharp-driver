using CloudCMS.Repositories;
using CloudCMS.Branches;

namespace CloudCMS.Nodes
{
    interface INode : IRepositoryDocument
    {
        IBranch Branch { get; }

        string BranchId { get; }

    }
}