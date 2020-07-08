using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudCMS
{
    public interface IAttachable
    {
        Task UploadAttachmentAsync(byte[] bytes, string mimeType);
        Task UploadAttachmentAsync(string attachmentId, byte[] bytes, string mimeType, string filename=null);
        
        Task UploadAttachmentAsync(Stream stream, string mimeType);
        Task UploadAttachmentAsync(string attachmentId, Stream stream, string mimeType, string filename=null);

        Task UploadAttachmentsAsync(IDictionary<string, AttachmentContent> attachments);
        Task UploadAttachmentsAsync(IDictionary<string, string> paramMap, IDictionary<string, AttachmentContent> attachments);

        Task<Stream> DownloadAttachmentAsync();
        Task<Stream> DownloadAttachmentAsync(string attachmentId);
        Task<byte[]> DownloadAttachmentBytesAsync();
        Task<byte[]> DownloadAttachmentBytesAsync(string attachmentId);

        Task<List<IAttachment>> ListAttachments();

        string GetDownloadUri();
        string GetDownloadUri(string attachmentId);

        Task DeleteAttachmentAsync();
        Task DeleteAttachmentAsync(string attachmentId);
    }
}