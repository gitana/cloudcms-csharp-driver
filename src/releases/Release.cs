using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public class Release : AbstractRepositoryDocument, IRelease
    {
        public Release(IRepository repository, JObject obj) : base(repository, obj)
        {
        }

        public override string URI => Repository.URI + "/releases/" + Id;
        public override string TypeId => "release";
        public override Reference Ref => Reference.create(TypeId, Repository.PlatformId, RepositoryId, Id);
    }
}