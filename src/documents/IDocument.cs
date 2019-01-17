using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CloudCMS.Documents
{
    interface IDocument
    {
        string Id { get; set; }

        JObject Data { get; set; }

        Driver Driver { get; }

        string URI { get; }

        Task ReloadAsync();

        Task DeleteAsync();

        Task UpdateAsync();
    }
}