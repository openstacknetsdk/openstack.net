using System;

namespace net.openstack.Core.Domain
{
    public class IdentityToken
    {
        public string Expires { get; set; }

        public string Id { get; set; }

        public Tenant Tenant { get; set; }

        public bool IsExpired()
        {
            if (string.IsNullOrWhiteSpace(Expires))
                return true;

            var exp = DateTime.Parse(Expires);

            return exp.ToUniversalTime().CompareTo(DateTime.UtcNow) <= 0;
        }
    }
}
