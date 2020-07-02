using CloudCMS.Exceptions;
using Newtonsoft.Json.Linq;

namespace CloudCMS.support
{
    public class QName
    {
        public string Namespace { get; }
        public string Name { get; }

        private QName(string nameSpace, string name)
        {
            this.Namespace = nameSpace;
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Namespace + ":" + this.Name;
        }

        public static QName create(string nameSpace, string name)
        {
            if (!string.IsNullOrEmpty(nameSpace)) {
                if (!string.IsNullOrEmpty(name)) {
                    return new QName(nameSpace, name);
                } else {
                    throw new InvalidQNameException("Invalid name");
                }
            } else {
                throw new InvalidQNameException("Invalid namespace");
            }
        }

        public static QName create(string qname)
        {
            if (string.IsNullOrEmpty(qname))
            {
                throw new InvalidQNameException("QName cannot be null");
            }
            else
            {
                string nameSpace = null;
                string localName = null;
                int x = qname.IndexOf(":");
                if (x > -1) {
                    nameSpace = qname.Substring(0, x);
                    localName = qname.Substring(x + 1);
                    return new QName(nameSpace, localName);
                } else {
                    throw new InvalidQNameException("Could not locate namespace and local name partition for qname: " + qname);
                }
            }
        }
    }
}