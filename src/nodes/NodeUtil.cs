using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS;

namespace CloudCMS
{
    public class NodeUtil
    {
        public static List<IBaseNode> NodeList(JObject response, IBranch branch)
        {
            JArray nodeArray = (JArray) response.SelectToken("rows");
            return NodeList(nodeArray, branch);
        }
        public static List<IBaseNode> NodeList(JArray nodeArray, IBranch branch)
        {
            List<IBaseNode> nodes = new List<IBaseNode>();
            foreach(var nodeJson in nodeArray)
            {
                JObject nodeObj = (JObject) nodeJson;
                IBaseNode node = Node(nodeObj, branch);
                nodes.Add(node);
            }

            return nodes;
        }

        public static IBaseNode Node(JObject nodeObj, IBranch branch)
        {
            bool isAssociation = false;
            if (nodeObj.ContainsKey("_is_association"))
            {
                isAssociation = ((JValue)nodeObj.GetValue("_is_association")).ToObject<bool>();
            }

            if (isAssociation)
            {
                return new Association(branch, nodeObj);
            }
            else
            {
                return new Node(branch, nodeObj);
            }
        }
    }
}