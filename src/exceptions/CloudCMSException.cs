using System;

namespace CloudCMS
{
    public class CloudCMSException : SystemException
    {
        public CloudCMSException(string message) : base(message)
        {
        }
    }
}