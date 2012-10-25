using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.corelib
{
    public class IdentityToken
    {
        public string Expires { get; set; }

        public string Id { get; set; }

        public bool IsExpired()
        {
            if (string.IsNullOrWhiteSpace(Expires))
                return true;

            var exp = DateTime.Parse(Expires);

            return exp.ToUniversalTime().CompareTo(DateTime.UtcNow) <= 0;
        }
    }
}
