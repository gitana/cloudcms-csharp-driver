using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS.Repositories;
using CloudCMS.Nodes;

namespace CloudCMS.Branches
{
    public interface IBranch : IRepositoryDocument
    {
        Task<INode> ReadNodeAsync(string nodeId);

        Task<List<INode>> QueryNodesAsync(JObject query, JObject pagination = null);

        Task<List<INode>> FindNodesAsync(JObject config, JObject pagination = null);

        Task<string> CreateNodeAsync(JObject nodeObj, JObject options = null);
    }
}