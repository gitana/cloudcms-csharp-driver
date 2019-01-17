using CloudCMS.Documents;

namespace CloudCMS.Repositories
{
    public interface IRepositoryDocument : IDocument
    {
        IRepository Repository { get; }

        string RepositoryId { get; }
    }
}