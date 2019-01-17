using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS.Repositories;
using CloudCMS.Documents;

namespace CloudCMS.Platforms
{
    interface IPlatform : IDocument
    {
        Task<List<IRepository>> ListRepositoriesAsync();

        Task<IRepository> ReadRepositoryAsync(string repositoryId);
    }
}