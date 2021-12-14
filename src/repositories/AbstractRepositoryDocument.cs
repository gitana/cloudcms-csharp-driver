using Newtonsoft.Json.Linq;
using CloudCMS;

namespace CloudCMS
{
    public abstract class AbstractRepositoryDocument : AbstractPlatformDocument,
                                                IRepositoryDocument
    {
        public IRepository Repository { get; }
        public string RepositoryId { get; }
        
        public string PlatformId { get; }

        protected AbstractRepositoryDocument(IRepository repository, JObject obj)
            : base(repository.Platform, obj)
        {
            this.Repository = repository;
            this.RepositoryId = repository.Id;

            this.PlatformId = repository.PlatformId;
        }
    }
}