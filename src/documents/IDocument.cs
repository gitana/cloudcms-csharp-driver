using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CloudCMS
{
    public interface IDocument : ITypedID, IReferenceable
    {
        string Id { get; set; }

        JObject Data { get; set; }

        CloudCMSDriver Driver { get; }

        string URI { get; }

        Task ReloadAsync();

        Task DeleteAsync();

        Task UpdateAsync();
    }
}