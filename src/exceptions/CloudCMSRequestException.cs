using System.Net.Http;

namespace CloudCMS
{
    public class CloudCMSRequestException : CloudCMSException
    {
        public CloudCMSRequestException(string message) : base(message)
        {
            
        }
    }
}