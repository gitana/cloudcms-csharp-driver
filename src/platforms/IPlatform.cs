using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS;

namespace CloudCMS
{
    public interface IPlatform : IDatastore, IReferenceable
    {
        Task<List<IRepository>> ListRepositoriesAsync();

        Task<IRepository> ReadRepositoryAsync(string repositoryId);

        Task<IRepository> CreateRepositoryAsync(JObject obj = null);
    }
}