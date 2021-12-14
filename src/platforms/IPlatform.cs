using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS;

namespace CloudCMS
{
    public interface IPlatform : IDatastore, IReferenceable, ITypedID
    {
        Task<List<IRepository>> ListRepositoriesAsync(JObject pagination = null);

        Task<IRepository> ReadRepositoryAsync(string repositoryId);

        Task<IRepository> CreateRepositoryAsync(JObject obj = null);
        
        // Projects
        Task<IProject> ReadProjectAsync(string projectId);
        Task<IJob> StartCreateProjectAsync(JObject obj);
        Task<List<IProject>> ListProjectsAsync(JObject pagination = null);
        Task<List<IProject>> QueryProjectsAsync(JObject query, JObject pagination = null);

        // Jobs 
        Task<IJob> ReadJobAsync(string jobId);
        Task<List<IJob>> QueryJobsAsync(JObject query, JObject pagination = null);
    }
}