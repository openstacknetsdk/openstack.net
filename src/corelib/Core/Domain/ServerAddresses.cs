using System.Collections.Generic;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ServerAddresses : Dictionary<string, AddressDetails[]>
    {
        [IgnoreDataMember]
        public AddressDetails[] Private { get { return this["private"]; } }

        [IgnoreDataMember]
        public AddressDetails[] Public { get { return this["public"]; } }
    }
}