using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    public class UpdateMetadataRequest
    {
        [DataMember]
        public Metadata Metadata { get; set; }
    }
}
