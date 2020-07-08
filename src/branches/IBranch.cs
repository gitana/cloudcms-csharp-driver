using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS;

namespace CloudCMS
{
    public interface IBranch : IRepositoryDocument
    {
        bool IsMaster();

        Task<IBaseNode> ReadNodeAsync(string nodeId);

        Task<List<IBaseNode>> QueryNodesAsync(JObject query, JObject pagination = null);

        Task<List<IBaseNode>> FindNodesAsync(JObject config, JObject pagination = null);

        Task<IBaseNode> CreateNodeAsync();
        Task<IBaseNode> CreateNodeAsync(JObject nodeObj, JObject options = null);

        Task<INode> RootNodeAsync();
        
        // Definitions
        
    }
}