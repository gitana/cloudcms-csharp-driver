using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS.Branches;

namespace CloudCMS.Nodes
{
    class NodeUtil
    {
        public static List<INode> NodeList(JArray nodeArray, IBranch branch)
        {
            List<INode> nodes = new List<INode>();
            foreach(var nodeJson in nodeArray)
            {
                JObject nodeObj = (JObject) nodeJson;
                INode node = new Node(branch, nodeObj);
                nodes.Add(node);
            }

            return nodes;
        }
    }
}