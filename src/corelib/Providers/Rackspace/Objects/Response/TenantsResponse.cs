namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class TenantsResponse
    {
        [JsonProperty("tenants")]
        public Tenant[] Tenants { get; private set; }
    }
}
