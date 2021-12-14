using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public abstract class AbstractPlatformDocument : AbstractDocument, IPlatformDocument
    {
        public IPlatform Platform { get; }

        protected AbstractPlatformDocument(IPlatform platform, JObject obj) : base(platform.Driver, obj)
        {
            Platform = platform;
        }
        
        public abstract Reference Ref { get; }

        public abstract string TypeId { get; }
    }
}