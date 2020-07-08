using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CloudCMS;


namespace CloudCMS
{
    public class Node : BaseNode, INode
    {
        public Node(IBranch branch, JObject obj) : base(branch, obj)
        {
        }

        public async Task<List<IAssociation>> AssociationsAsync()
        {
            return await AssociationsAsync(null, Direction.ANY, null);
        }

        public async Task<List<IAssociation>> AssociationsAsync(JObject pagination)
        {
            return await AssociationsAsync(null, Direction.ANY, pagination);
        }

        public async Task<List<IAssociation>> AssociationsAsync(Direction direction)
        {
            return await AssociationsAsync(null, direction, null);
        }

        public async Task<List<IAssociation>> AssociationsAsync(QName associationTypeQName)
        {
            return await AssociationsAsync(associationTypeQName, Direction.ANY, null);
        }

        public async Task<List<IAssociation>> AssociationsAsync(QName associationTypeQName, Direction direction, JObject pagination=null)
        {
            if (pagination == null)
            {
                pagination = new JObject();
            }

            string uri = URI + "/associations";
            
            IDictionary<string, string> queryParams = pagination.ToObject<Dictionary<string, string>>();
            queryParams.Add("direction", direction.ToString());

            if (associationTypeQName != null)
            {
                queryParams.Add("type", associationTypeQName.ToString());
            }

            JObject response = await Driver.GetAsync(uri, queryParams);
            JArray associationArray = (JArray) response.SelectToken("rows");
            List<IAssociation> associations = AssociationUtil.AssociationList(associationArray, Branch);

            return associations;
        }

        public async Task<Association> AssociateAsync(INode targetNode, QName associationTypeQName)
        {
            return await AssociateAsync(targetNode, associationTypeQName, Directionality.DIRECTED, null);
        }

        public async Task<Association> AssociateAsync(INode targetNode, QName associationTypeQName, JObject data)
        {
            return await AssociateAsync(targetNode, associationTypeQName, Directionality.DIRECTED, data);
        }

        public async Task<Association> AssociateAsync(INode otherNode, QName associationTypeQName, Directionality directionality)
        {
            return await AssociateAsync(otherNode, associationTypeQName, directionality, null);
        }

        public async Task<Association> AssociateAsync(INode otherNode, QName associationTypeQName, Directionality directionality, JObject data)
        {
            if (data == null)
            {
                data = new JObject();
            }

            string uri = URI + "/associate";
            
            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("node", otherNode.Id);
            queryParams.Add("type", associationTypeQName.ToString());
            if (!directionality.Equals(Directionality.DIRECTED))
            {
                queryParams.Add("directionality", directionality.ToString());
            }

            JObject response1 = await Driver.PostAsync(uri, queryParams);
            string associationId = (string)response1["_doc"];
            
            // Read back association object
            JObject response2 = await Driver.GetAsync(Branch.URI + "/nodes/" + associationId);
            Association association = new Association(Branch, response2);

            return association;
        }

        public Task UnassociateAsync(INode targetNode, QName associationTypeQName)
        {
            return UnassociateAsync(targetNode, associationTypeQName, Directionality.DIRECTED);
        }

        public Task UnassociateAsync(INode targetNode, QName associationTypeQName, Directionality directionality)
        {
            string uri = URI + "/unassociate";
            
            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("node", targetNode.Id);
            queryParams.Add("type", associationTypeQName.ToString());
            if (!directionality.Equals(Directionality.DIRECTED))
            {
                queryParams.Add("directionality", directionality.ToString());
            }
            
            return Driver.PostAsync(uri, queryParams);
        }

        public Task<JObject> FileFolderTreeAsync()
        {
            return FileFolderTreeAsync(null);
        }

        public Task<JObject> FileFolderTreeAsync(string basePath)
        {
            return FileFolderTreeAsync(null, null);
        }

        public Task<JObject> FileFolderTreeAsync(string basePath, string leafPath)
        {
            List<string> paths = new List<string>();
            if (leafPath != null)
            {
                paths.Add(leafPath);
            }

            return FileFolderTreeAsync(basePath, -1, paths, true, false, null);

        }

        public Task<JObject> FileFolderTreeAsync(string basePath, int depth, List<string> leafPaths, bool includeProperties, bool containersOnly,
            JObject query = null)
        {
            string uri = URI + "/tree";
            
            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            if (basePath != null)
            {
                queryParams.Add("base", basePath);
            }

            if (leafPaths != null && leafPaths.Count > 0)
            {
                string leafPathsParam = "";
                foreach (string leafPath in leafPaths)
                {
                    leafPathsParam += leafPath;
                    leafPathsParam += ",";
                }
                
                leafPathsParam.TrimEnd(',');
                queryParams.Add("leaf", leafPathsParam);
            }

            if (depth > -1)
            {
                queryParams.Add("depth", depth.ToString());
            }

            if (includeProperties)
            {
                queryParams.Add("properties", true.ToString());
            }

            if (containersOnly)
            {
                queryParams.Add("containers", true.ToString());
            }

            return Driver.PostAsync(uri, queryParams);
        }

        public async Task<List<IBaseNode>> ListChildrenAsync(JObject pagination = null)
        {
            pagination ??= new JObject();
            
            IDictionary<string, string> queryParams =  pagination.ToObject<Dictionary<string, string>>();

            string uri = URI + "/children";

            JObject response = await Driver.GetAsync(uri, queryParams);

            return NodeUtil.NodeList(response, Branch);
        }

        public async Task<List<IBaseNode>> ListRelativesAsync(QName type, Direction direction, JObject pagination = null)
        {
            string uri = URI + "/relatives";
            pagination ??= new JObject();
            
            Dictionary<string, string> queryParams = pagination.ToObject<Dictionary<string, string>>();
            queryParams.Add("type", type.ToString());
            queryParams.Add("direction", direction.ToString());

            JObject response = await Driver.GetAsync(uri, queryParams);
            return NodeUtil.NodeList(response, Branch);
        }

        public async Task<List<IBaseNode>> QueryRelativesAsync(QName type, Direction direction, JObject query, JObject pagination = null)
        {
            pagination ??= new JObject();
            string uri = URI + "/relatives/query";
            
            Dictionary<string, string> queryParams = pagination.ToObject<Dictionary<string, string>>();
            queryParams.Add("type", type.ToString());
            queryParams.Add("direction", direction.ToString());
            
            HttpContent content = new StringContent(query.ToString());
            JObject response = await Driver.PostAsync(uri, queryParams, content);

            return NodeUtil.NodeList(response, Branch);
        }

        public async Task<TraversalResults> TraverseAsync(JObject traverse)
        {
            JObject config = new JObject();
            config.Add("traverse", traverse);

            string uri = URI + "/traverse";
            HttpContent content = new StringContent(config.ToString());

            JObject response = await Driver.PostAsync(uri, null, content);
            return TraversalResults.Parse(response, Branch);
        }
        
        public async Task<INode> CreateTranslationAsync(string locale, string edition, JObject obj)
        {
            string uri1 = URI + "/i18n";

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("edition", edition);
            queryParams.Add("locale", locale);
            
            HttpContent content = new StringContent(obj.ToString());
            JObject response1 = await Driver.PostAsync(uri1, queryParams, content);
            string nodeId = response1.GetValue("_doc").ToString();

            return (INode) await Branch.ReadNodeAsync(nodeId);
        }

        public async Task<List<string>> GetTranslationEditionsAsync()
        {
            string uri = URI + "/i18n/editions";
            JObject response = await Driver.GetAsync(uri);
            JArray array = (JArray) response.GetValue("editions");
            
            List<string> editions = new List<string>();
            foreach (var editionToken in array)
            {
                editions.Add(editionToken.ToString());
            }

            return editions;
        }

        public async Task<List<string>> GetTranslationLocales(string edition)
        {
            string uri = URI + "/i18n/locales";
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("edition", edition);
            
            JObject response = await Driver.GetAsync(uri, queryParams);    
            JArray array = (JArray) response.GetValue("locales");
            
            List<string> locales = new List<string>();
            foreach (var localeToken in array)
            {
                locales.Add(localeToken.ToString());
            }

            return locales;
        }
        
        public async Task<INode> ReadTranslationAsync(string locale, string edition=null)
        {
            string uri = URI + "/i18n";
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("locale", locale);
            if (edition != null)
            {
                queryParams.Add("edition", edition);
            }

            JObject response = await Driver.GetAsync(uri, queryParams);
            return (INode) NodeUtil.Node(response, Branch);
        }

        public override string TypeId
        {
            get
            {
                return "node";
            }
        }
    }
}