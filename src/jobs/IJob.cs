using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public interface IJob : IDocument
    {
        string Type { get; }
        string State { get; }
        Task WaitForCompletion();
        Task KillAsync();

    }
}