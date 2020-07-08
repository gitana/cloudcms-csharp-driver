using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudCMS;
using System.Net.Http;

namespace CloudCMS
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
            IRepository repository = null;
            try
            {
                JObject response = await Driver.GetAsync(uri);
                repository = new Repository(Driver, response);

            }
            catch (CloudCMSRequestException)
            {
                repository = null;
            }

            return repository;
        }

        public async Task<IRepository> CreateRepositoryAsync(JObject obj = null)
        {
            string uri = this.URI + "/repositories/";
            if (obj == null)
            {
                obj = new JObject();
            }
            HttpContent content = new StringContent(obj.ToString());
            JObject repsonse = await Driver.PostAsync(uri, null, content);

            string repositoryId = repsonse["_doc"].ToString();
            return await ReadRepositoryAsync(repositoryId);
        }

        public override string TypeId
        {
            get
            {
                return "platform";
            }
        }

        public override Reference Ref
        {
            get
            {
                return Reference.create(TypeId, Id);
            }
        }
    }
}