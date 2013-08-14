namespace net.openstack.Core.Domain
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class IdentityToken
    {
        [JsonProperty("expires")]
        public string Expires { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tenant")]
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
