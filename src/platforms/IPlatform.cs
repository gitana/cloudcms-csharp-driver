using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS.Repositories;
using CloudCMS.Documents;

namespace CloudCMS.Platforms
{
    public interface IPlatform : IDocument
    {
        Task<List<IRepository>> ListRepositoriesAsync();

        Task<IRepository> ReadRepositoryAsync(string repositoryId);

        Task<IRepository> CreateRepositoryAsync(JObject obj = null);
    }
}