using System.Collections.Generic;
using CloudCMS.Branches;
using Newtonsoft.Json.Linq;

namespace CloudCMS.Nodes
{
    public class AssociationUtil
    {
        public static List<IAssociation> AssociationList(JArray nodeArray, IBranch branch)
        {
            List<IAssociation> associations = new List<IAssociation>();
            foreach(var nodeJson in nodeArray)
            {
                JObject associationObj = (JObject) nodeJson;
                IAssociation association = new Association(branch, associationObj);
                associations.Add(association);
            }

            return associations;
        }
    }
}