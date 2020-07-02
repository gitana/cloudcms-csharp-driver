using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CloudCMS.Documents;
using CloudCMS.Repositories;
using CloudCMS.Branches;
using CloudCMS.support;


namespace CloudCMS.Nodes
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
    }
}