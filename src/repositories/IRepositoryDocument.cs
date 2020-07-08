using CloudCMS;

namespace CloudCMS
{
    public interface IRepositoryDocument : IDocument
    {
        IRepository Repository { get; }

        string RepositoryId { get; }
        
        
        string PlatformId { get; }
    }
}