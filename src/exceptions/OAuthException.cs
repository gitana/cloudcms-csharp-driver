using System;

namespace CloudCMS.Exceptions
{
    public class OAuthException : SystemException
    {
        public OAuthException(string message) : base(message)
        {
            
        }
    }
}