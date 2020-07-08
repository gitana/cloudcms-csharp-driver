using System;

namespace CloudCMS
{
    public class OAuthException : SystemException
    {
        public OAuthException(string message) : base(message)
        {
            
        }
    }
}