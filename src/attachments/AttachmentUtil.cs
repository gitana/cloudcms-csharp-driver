using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace CloudCMS
{
    public class AttachmentUtil
    {
        public static List<IAttachment> AttachmentList(IAttachable attachable, JObject response)
        {
            JArray nodeArray = (JArray) response.SelectToken("rows");
            return AttachmentList(attachable, nodeArray);
        }
        public static List<IAttachment> AttachmentList(IAttachable attachable, JArray attachmentArray)
        {
            List<IAttachment> attachments = new List<IAttachment>();
            foreach(var attachmentJson in attachmentArray)
            {
                JObject attachmentObj = (JObject) attachmentJson;
                IAttachment attachment = Attachment(attachable, attachmentObj);
                attachments.Add(attachment);
            }

            return attachments;
        }
        
        public static IAttachment Attachment(IAttachable attachable, JObject attachmentObj)
        {
            return new Attachment(attachable, attachmentObj);
        }
    }
}