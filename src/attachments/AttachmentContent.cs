using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CloudCMS
{
    public class AttachmentContent : HttpContent
    {
        private HttpContent _content;
        private long _length;
        
        public AttachmentContent(byte[] bytes, string mimetype)
        {
            _content = new ByteArrayContent(bytes);
            this.Headers.ContentType = new MediaTypeHeaderValue(mimetype);

            _length = bytes.Length;
        }

        public AttachmentContent(Stream stream, string mimetype)
        {
            _content = new StreamContent(stream);
            this.Headers.ContentType = new MediaTypeHeaderValue(mimetype);

            _length = stream.Length;
        }
        
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            byte[] contentBytes = await _content.ReadAsByteArrayAsync();
            await stream.WriteAsync(contentBytes);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _length;
            return true;
        }
    }
}