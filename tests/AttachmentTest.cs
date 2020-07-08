using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Xunit;

namespace CloudCMS.Tests
{
    public class AttachmentTest : AbstractWithRepositoryTest
    {
        public AttachmentTest(RepositoryFixture fixture) : base(fixture)
        {
        }

        public byte[] ReadStreamBytes(Stream sourceStream)
        {
            using var memoryStream = new MemoryStream();
            sourceStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        [Fact]
        public async void TestAttachmentBytes()
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            Stream cloudcmsStream = assembly.GetManifestResourceStream("CloudCMS.res.cloudcms.png");
            byte[] cloudcmsImage = ReadStreamBytes(cloudcmsStream);
            
            Stream headphonesStream = assembly.GetManifestResourceStream("CloudCMS.res.headphones.png");
            byte[] headphonesImage = ReadStreamBytes(headphonesStream);
            
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");
            INode node = (INode) await branch.CreateNodeAsync();
            
            await node.UploadAttachmentAsync("default", cloudcmsImage, "image/png", "myImage");
            
            List<IAttachment> attachments = await node.ListAttachments();
            IAttachment attachment = attachments[0];
            Assert.Equal("default", attachment.Id);
            Assert.Equal("myImage", attachment.Filename);
            Assert.Equal("image/png", attachment.ContentType);
            Assert.True(attachment.Length > 0);
            Assert.NotNull(attachment.ObjectId);
            
            byte[] download = await node.DownloadAttachmentBytesAsync();
            Assert.NotEmpty(download);
            byte[] downloadCopy = await attachment.DownloadAsync();
            Assert.Equal(download.Length, downloadCopy.Length);
            
            Dictionary<string, AttachmentContent> attachmentContents = new Dictionary<string, AttachmentContent>();
            attachmentContents.Add("another", new AttachmentContent(headphonesImage, "image/png"));
            await node.UploadAttachmentsAsync(attachmentContents);

            attachments = await node.ListAttachments();
            Assert.Equal(2, attachments.Count);

            await node.DeleteAttachmentAsync("default");
            attachments = await node.ListAttachments();
            Assert.Single(attachments);
            
            attachment = attachments[0];
            Assert.Equal("another", attachment.Id);
            Assert.Equal("another", attachment.Filename);
            Assert.Equal("image/png", attachment.ContentType);
            Assert.True(attachment.Length > 0);
            Assert.NotNull(attachment.ObjectId);
        }

        [Fact]
        public async void TestAttachmentStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            Stream cloudcmsStream = assembly.GetManifestResourceStream("CloudCMS.res.cloudcms.png");
            Stream headphonesStream = assembly.GetManifestResourceStream("CloudCMS.res.headphones.png");
            
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");
            INode node = (INode) await branch.CreateNodeAsync();
            
            await node.UploadAttachmentAsync("default", cloudcmsStream, "image/png", "myImage");
            
            List<IAttachment> attachments = await node.ListAttachments();
            IAttachment attachment = attachments[0];
            Assert.Equal("default", attachment.Id);
            Assert.Equal("myImage", attachment.Filename);
            Assert.Equal("image/png", attachment.ContentType);
            Assert.True(attachment.Length > 0);
            Assert.NotNull(attachment.ObjectId);
            
            Stream download = await node.DownloadAttachmentAsync();
            Assert.True(download.Length > 0);
            Stream downloadCopy = await attachment.StreamAsync();
            Assert.Equal(download.Length, downloadCopy.Length);
            
            Dictionary<string, AttachmentContent> attachmentContents = new Dictionary<string, AttachmentContent>();
            attachmentContents.Add("another", new AttachmentContent(headphonesStream, "image/png"));
            await node.UploadAttachmentsAsync(attachmentContents);
            
            attachments = await node.ListAttachments();
            Assert.Equal(2, attachments.Count);

            await node.DeleteAttachmentAsync("default");
            attachments = await node.ListAttachments();
            Assert.Single(attachments);
            
            attachment = attachments[0];
            Assert.Equal("another", attachment.Id);
            Assert.Equal("another", attachment.Filename);
            Assert.Equal("image/png", attachment.ContentType);
            Assert.True(attachment.Length > 0);
            Assert.NotNull(attachment.ObjectId);
        }
    }
}