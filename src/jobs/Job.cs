using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CloudCMS
{
    public class Job : AbstractDocument, IJob
    {
        public Job(ICloudCMSDriver driver, JObject data) : base(driver, data)
        {
            
        }
        
        public string Type => Data.GetValue("type").ToString();
        public string State => Data.GetValue("state").ToString();

        public async Task WaitForCompletion()
        {
            // Use with caution
            while (true)
            {
                await ReloadAsync();
                if ("FINISHED".Equals(State))
                {
                    return;
                }
                else if ("ERROR".Equals(State))
                {
                    throw new CloudCMSException("Job failed: " + Id);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
        
        public async Task KillAsync()
        {
            string uri = URI + "/kill";
            await Driver.PostAsync(uri);
        }

        public override string URI => "/jobs/" + Id;
    }
}