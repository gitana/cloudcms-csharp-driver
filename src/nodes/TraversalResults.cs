using System.Collections.Generic;
using CloudCMS;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public class TraversalResults
    {
        public List<INode> Nodes { get; }
        public List<IAssociation> Associations { get; }
        
        protected TraversalResults()
        {
            Nodes = new List<INode>();
            Associations = new List<IAssociation>();
        }

        public static TraversalResults Parse(JObject response, IBranch branch)
        {
            TraversalResults results = new TraversalResults();

            JObject nodesObj = (JObject) response.GetValue("nodes");
            foreach (var kv in nodesObj)
            {
                string field = kv.Key;
                JObject nodeObj = (JObject) kv.Value;

                INode node = (INode) NodeUtil.Node(nodeObj, branch);
                results.Nodes.Add(node);
            }

            JObject associationsObj = (JObject) response.GetValue("associations");
            foreach (var kv in associationsObj)
            {
                string field = kv.Key;
                JObject associationObj = (JObject) kv.Value;

                IAssociation association = (IAssociation) NodeUtil.Node(associationObj, branch);
                results.Associations.Add(association);
            }

            return results;
        }
    }
}