namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateServerResponse
    {
        [JsonProperty("server")]
        public NewServer Server { get; private set; }
    }
}
