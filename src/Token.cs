using System;

namespace CloudCMS
{
    class Token
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }

        public DateTime expireTime { get; set; }
    }
}