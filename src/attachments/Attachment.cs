using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public class Attachment : IAttachment
    {
        public string Id { get; }
        public string ObjectId { get; }
        public long Length { get; }
        public string Filename { get; }
        public string ContentType { get; }

        private IAttachable _attachable;
        
        public Attachment(IAttachable attachable, JObject obj)
        {
            Id = obj.GetValue("attachmentId").ToString();
            ObjectId = obj.GetValue("objectId").ToString();
            Length = obj.GetValue("length").ToObject<long>();
            ContentType = obj.GetValue("contentType").ToString();
            Filename = obj.GetValue("filename").ToString();

            _attachable = attachable;
        }

        public Task<Stream> StreamAsync()
        {
            return _attachable.DownloadAttachmentAsync(Id);
        }

        public Task<byte[]> DownloadAsync()
        {
            return _attachable.DownloadAttachmentBytesAsync(Id);
        }
    }
}