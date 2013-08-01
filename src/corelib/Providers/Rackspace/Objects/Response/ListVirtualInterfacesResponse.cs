using System.Collections.Generic;
using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListVirtualInterfacesResponse
    {
        [DataMember(Name = "virtual_interfaces")]
        public IEnumerable<VirtualInterface> VirtualInterfaces { get; set; }
    }
}
