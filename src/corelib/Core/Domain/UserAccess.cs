namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserAccess
    {
        [JsonProperty("token")]
        public IdentityToken Token { get; set; }

        [JsonProperty("user")]
        public UserDetails User { get; set; }

        [JsonProperty("serviceCatalog")]
        public ServiceCatalog[] ServiceCatalog { get; set; }
    }
}