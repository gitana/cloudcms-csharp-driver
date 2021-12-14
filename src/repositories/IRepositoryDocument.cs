using CloudCMS;

namespace CloudCMS
{
    public interface IRepositoryDocument : IPlatformDocument
    {
        IRepository Repository { get; }

        string RepositoryId { get; }
        
        
        string PlatformId { get; }
    }
}