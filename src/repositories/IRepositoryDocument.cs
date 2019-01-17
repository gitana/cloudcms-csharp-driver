using CloudCMS.Documents;

namespace CloudCMS.Repositories
{
    interface IRepositoryDocument : IDocument
    {
        IRepository Repository { get; }

        string RepositoryId { get; }
    }
}