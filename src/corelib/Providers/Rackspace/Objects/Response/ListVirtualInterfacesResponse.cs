namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListVirtualInterfacesResponse
    {
        [JsonProperty("virtual_interfaces")]
        public IEnumerable<VirtualInterface> VirtualInterfaces { get; set; }
    }
}
