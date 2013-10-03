using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AllowedDomain
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
