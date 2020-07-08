using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

namespace CloudCMS
{
    public class Reference
    {
        public string TypeId { get; }
        public List<string> Identifiers { get; }
        public string Ref { get; }
        
        private static Regex ALPHA_ONLY = new Regex(@"^[a-zA-Z-]*");

        private Reference(string typeId, List<string> identifiers)
        {
            TypeId = typeId;
            Identifiers = new List<string>();
            Identifiers.AddRange(identifiers);
            Ref = TypeId + "://" + String.Join('/', identifiers);
        }

        public static bool IsReference(string reference)
        {
            bool isReference = false;
            int x = reference.IndexOf("://");
            if (x > -1)
            {
                string typeId = reference.Substring(0, x);
                if (ALPHA_ONLY.IsMatch(typeId))
                {
                    isReference = true;
                    if ("http".Equals(typeId, StringComparison.OrdinalIgnoreCase) || "https".Equals(typeId, StringComparison.OrdinalIgnoreCase)) {
                        isReference = false;
                    }
                }
            }

            return isReference;
        }

        public static Reference create(string reference)
        {
            if (!IsReference(reference))
            {
                throw new CloudCMSException("Invalid reference: " + reference);
            }
            
            int x = reference.IndexOf("://");
            string typeId = reference.Substring(0, x);
            string idString = reference.Substring(x + 3);
            string[] ids = idString.Split('/');
            
            return new Reference(typeId, ids.ToList());
        }

        public static Reference create(string typeId, List<string> identifiers)
        {
            return new Reference(typeId, identifiers);
        }

        public static Reference create(string typeId, string id)
        {
            return new Reference(typeId, new List<string>(){ id });
        }

        public static Reference create(string typeId, params string[] identifiers)
        {
            return new Reference(typeId, identifiers.ToList());
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Reference reference)
            {
                return this.Ref.Equals(reference.Ref);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return Ref;
        }
    }
}