using System;
using System.Net.Http;

namespace CloudCMS.Exceptions
{
    public class CloudCMSRequestException : SystemException
    {
        public CloudCMSRequestException(string message) : base(message)
        {
            
        }
    }
}