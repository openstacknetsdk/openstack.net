namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListFlavorsResponse
    {
        [JsonProperty("flavors")]
        public Flavor[] Flavors { get; private set; }
    }
}
