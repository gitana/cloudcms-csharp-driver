using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public class Project : AbstractPlatformDocument, IProject
    {
        public Project(IPlatform platform, JObject obj) : base(platform, obj)
        {
        }

        public override string URI => "/projects/" + Id;
        public override string TypeId => "project";
        public override Reference Ref => Reference.create(TypeId, Platform.Id, Id);
    }
}