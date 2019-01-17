using System;

namespace CloudCMS.Exceptions
{
    class OAuthException : SystemException
    {
        public OAuthException(string message) : base(message)
        {
            
        }
    }
}