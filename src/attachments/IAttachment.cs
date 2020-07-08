using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CloudCMS
{
    public interface IAttachment
    {
        string Id { get; }
        string ObjectId { get; }
        long Length { get; }
        string Filename { get; }
        string ContentType { get; }
        Task<Stream> StreamAsync();
        Task<byte[]> DownloadAsync();
    }
}