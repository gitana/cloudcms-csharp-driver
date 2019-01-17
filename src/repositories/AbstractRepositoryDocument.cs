using Newtonsoft.Json.Linq;
using CloudCMS.Documents;

namespace CloudCMS.Repositories
{
    abstract class AbstractRepositoryDocument : AbstractDocument,
                                                IRepositoryDocument
    {
        public IRepository Repository { get; }

        public string RepositoryId { get; 
        }
        protected AbstractRepositoryDocument(IRepository repository, JObject obj)
            : base(repository.Driver, obj)
        {
            this.Repository = repository;
            this.RepositoryId = repository.Id;
        }
    }
}