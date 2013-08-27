using System.Collections.Generic;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ServerAddresses : Dictionary<string, AddressDetails[]>
    {
        [IgnoreDataMember]
        public AddressDetails[] Private { get { return ContainsKey("private") ? this["private"] : null; } }

        [IgnoreDataMember]
        public AddressDetails[] Public { get { return ContainsKey("public") ? this["public"] : null; } }
    }
}