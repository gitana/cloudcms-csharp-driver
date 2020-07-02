using System.Net.Http;

namespace CloudCMS.Exceptions
{
    public class CloudCMSRequestException : CloudCMSException
    {
        public CloudCMSRequestException(string message) : base(message)
        {
            
        }
    }
}