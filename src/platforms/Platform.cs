using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS.Repositories;
using CloudCMS.Documents;

namespace CloudCMS.Platforms
{
    class Platform : AbstractDocument,
                     IPlatform
    {
        public Platform(CloudCMSDriver driver, JObject obj) : base(driver, obj)
        {

        }

        public override string URI
        {
            get
            {
                return "";
            }
        }

        public async Task<List<IRepository>> ListRepositoriesAsync()
        {
            string uri = this.URI + "/repositories";
            JObject response = await Driver.GetAsync(uri);

            List<IRepository> repositories = new List<IRepository>();
            JArray rows = (JArray) response.SelectToken("rows");
            foreach(var row in rows)
            {
                JObject repositoryObj = (JObject) row;
                IRepository repository = new Repository(Driver, repositoryObj);
                repositories.Add(repository);
            }

            return repositories;
        }

        public async Task<IRepository> ReadRepositoryAsync(string repositoryId)
        {
            string uri = this.URI + "/repositories/" + repositoryId;
            JObject response = await Driver.GetAsync(uri);

            IRepository repository = new Repository(Driver, response);
            return repository;
        }
    }
}