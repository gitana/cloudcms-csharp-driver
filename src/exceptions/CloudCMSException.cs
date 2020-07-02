using System;

namespace CloudCMS.Exceptions
{
    public class CloudCMSException : SystemException
    {
        public CloudCMSException(string message) : base(message)
        {
        }
    }
}