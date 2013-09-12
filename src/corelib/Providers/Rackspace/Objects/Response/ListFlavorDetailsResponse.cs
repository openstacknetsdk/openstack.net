namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListFlavorDetailsResponse
    {
        [JsonProperty("flavors")]
        public FlavorDetails[] Flavors { get; private set; }
    }
}
