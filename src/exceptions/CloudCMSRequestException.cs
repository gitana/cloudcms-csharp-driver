using System;
using System.Net.Http;

namespace CloudCMS.Exceptions
{
    class CloudCMSRequestException : SystemException
    {
        public CloudCMSRequestException(string message) : base(message)
        {
            
        }
    }
}