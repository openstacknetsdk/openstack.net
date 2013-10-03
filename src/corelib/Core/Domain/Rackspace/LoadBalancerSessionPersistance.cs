using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerSessionPersistance
    {
        [JsonProperty("persistenceType")]
        public string Type { get; set; }

    }
}