using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CloudCMS;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public abstract class BaseNode : AbstractRepositoryDocument, IBaseNode
    {
        public IBranch Branch { get; }

        public string BranchId { get; }

        public QName TypeQName
        {
            get
            {
                string type = GetString("_type");
                return QName.create(type);
            }
        }

        public QName QName
        {
            get
            {
                string q = GetString("_qname");
                return CloudCMS.QName.create(q);
            }
        }

        protected BaseNode(IBranch branch, JObject obj)
            :base(branch.Repository, obj)
        {
            this.Branch = branch;
            this.BranchId = branch.Id;
        }
        
        public override string URI
        {
            get
            {
                return Branch.URI + "/nodes/" + this.Id;
            }
        }

        public Task RefreshAsync()
        {
            string uri = URI + "/refresh";
            return Driver.PostAsync(uri);
        }

        public List<string> GetFeatureIds()
        {
            List<string> featureIds = new List<string>();
            JObject featuresObj = (JObject) Data.GetValue("_features");
            if (featuresObj != null)
            {
                foreach (var kv in featuresObj)
                {
                    string fieldName = kv.Key;
                    featureIds.Add(fieldName);
                }
            }

            return featureIds;
        }

        public JObject GetFeature(string featureId)
        {
            JObject feature = null;
            
            JObject featuresObj = (JObject) Data.GetValue("_features");
            if (featuresObj != null)
            {
                if (featuresObj.ContainsKey(featureId))
                {
                    feature = (JObject) featuresObj.GetValue(featureId);
                }
            }

            return feature;
        }

        public bool HasFeature(string featureId)
        {
            bool hasFeature = false;
            
            JObject featuresObj = (JObject) Data.GetValue("_features");
            if (featuresObj != null)
            {
                if (featuresObj.ContainsKey(featureId))
                {
                    hasFeature = true;
                }
            }

            return hasFeature;
        }

        public async Task AddFeatureAsync(string featureId, JObject featureConfig)
        {
            string uri = URI + "/features/" + featureId;
            HttpContent content = new StringContent(featureConfig.ToString());
            
            await Driver.PostAsync(uri, null, content);
            await ReloadAsync();
        }

        public async Task RemoveFeatureAsync(string featureId)
        {
            string uri = URI + "/features/" + featureId;

            await Driver.DeleteAsync(uri);
            await ReloadAsync();
        }

        public async Task<IBaseNode> ReadVersionAsync(string changesetId, JObject options = null)
        {
            string uri = URI + "/versions/" + changesetId;
            
            IDictionary<string, string> queryParams = null;
            if (options != null)
            {
                queryParams = options.ToObject<Dictionary<string, string>>();
            }
            
            JObject response = await Driver.GetAsync(uri, queryParams);
            IBaseNode node = NodeUtil.Node(response, Branch);

            return node;
        }

        public async Task<List<IBaseNode>> ListVersionsAsync(JObject options = null, JObject pagination = null)
        {
            string uri = URI + "/versions/";

            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            if (options != null)
            {
                queryParams.AddAll(options.ToObject<Dictionary<string, string>>());
            }

            if (pagination != null)
            {
                queryParams.AddAll(pagination.ToObject<Dictionary<string, string>>());
            }

            JObject response = await Driver.GetAsync(uri, queryParams);
            JArray nodeArray = (JArray) response.SelectToken("rows");
            List<IBaseNode> nodes = NodeUtil.NodeList(nodeArray, Branch);

            return nodes;
        }

        public async Task<IBaseNode> RestoreVersionAsync(string changesetId)
        {
            string uri = URI + "/versions/" + changesetId + "/restore";
            JObject response = await Driver.PostAsync(uri, new Dictionary<string, string>(), new JObject());
            IBaseNode node = NodeUtil.Node(response, Branch);

            return node;
        }

        public async Task ChangeQNameAsync(QName newQName)
        {
            string uri = URI + "/change_qname";
            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("qname", newQName.ToString());

            await Driver.PostAsync(uri, queryParams, new JObject());
        }

        public string GetString(string field)
        {
            return this.Data[field] != null ? (string) this.Data[field] : null;
        }

        public void SetString(string field, string value)
        {
            this.Data[field] = value;
        }

        public Task UploadAttachmentAsync(byte[] bytes, string mimeType)
        {
            return UploadAttachmentAsync(null, bytes, mimeType);
        }

        public async Task UploadAttachmentAsync(string attachmentId, byte[] bytes, string mimeType, string filename = null)
        {
            attachmentId ??= "default";
            string uri = URI + "/attachments/" + attachmentId;

            Dictionary<string, string> paramMap = new Dictionary<string, string>();
            if (filename != null)
            {
                paramMap.Add("filename", filename);
            }
            await Driver.UploadAsync(uri, bytes, mimeType, paramMap);
        }

        public Task UploadAttachmentAsync(Stream stream, string mimeType)
        {
            return UploadAttachmentAsync(null, stream, mimeType);
        }

        public async Task UploadAttachmentAsync(string attachmentId, Stream stream, string mimeType, string filename = null)
        {
            attachmentId ??= "default";
            string uri = URI + "/attachments/" + attachmentId;

            Dictionary<string, string> paramMap = new Dictionary<string, string>();
            if (filename != null)
            {
                paramMap.Add("filename", filename);
            }
            await Driver.UploadAsync(uri, stream, mimeType, paramMap);
        }


        /**
         * Upload Multiple attachments. Takes a map with atthachmentIds as keys and httpcontent as values.
         * The HttpContent should be of type ByteArrayContent or StreamContent
         */
        public async Task UploadAttachmentsAsync(IDictionary<string, AttachmentContent> attachments)
        {
            IDictionary<string, string> paramMap = new Dictionary<string, string>();
            await UploadAttachmentsAsync(paramMap, attachments);
        }

        /**
         * Upload Multiple attachments. Takes a map with atthachmentIds as keys and httpcontent as values.
         * The HttpContent should be of type ByteArrayContent or StreamContent
         */
        public async Task UploadAttachmentsAsync(IDictionary<string, string> paramMap, IDictionary<string, AttachmentContent> attachments)
        {
            string uri = URI + "/attachments";
            await Driver.UploadAsync(uri, paramMap, attachments);
        }

        public Task<Stream> DownloadAttachmentAsync()
        {
            return DownloadAttachmentAsync(null);
        }

        public async Task<Stream> DownloadAttachmentAsync(string attachmentId)
        {
            if (attachmentId == null)
            {
                attachmentId = "default";
            }

            string uri = GetDownloadUri(attachmentId);
            return await Driver.DownloadAsync(uri);
        }

        public Task<byte[]> DownloadAttachmentBytesAsync()
        {
            return DownloadAttachmentBytesAsync(null);
        }
        
        public async Task<byte[]> DownloadAttachmentBytesAsync(string attachmentId)
        {
            attachmentId ??= "default";
            
            string uri = GetDownloadUri(attachmentId);
            return await Driver.DownloadBytesAsync(uri);
        }

        public async Task<List<IAttachment>> ListAttachments()
        {
            string uri = URI + "/attachments";
            JObject response = await Driver.GetAsync(uri);

            return AttachmentUtil.AttachmentList(this, response);
        }

        public string GetDownloadUri()
        {
            return GetDownloadUri(null);
        }

        public string GetDownloadUri(string attachmentId)
        {
            attachmentId ??= "default";
            return URI + "/attachments/" + attachmentId;
        }

        public Task DeleteAttachmentAsync()
        {
            return DeleteAttachmentAsync(null);
        }

        public async Task DeleteAttachmentAsync(string attachmentId)
        {
            attachmentId ??= "default";
            string uri = GetDownloadUri(attachmentId);

            await Driver.DeleteAsync(uri);
        }

        public override Reference Ref
        {
            get
            {
                return Reference.create(TypeId, PlatformId, RepositoryId, BranchId, Id);
            }
        }
    }
}