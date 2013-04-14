using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class CloudNetworkResponse
    {
        [DataMember(Name = "network")]
        public CloudNetwork Network { get; set; }
    }
}
