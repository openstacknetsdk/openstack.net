using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class UpdateUserRequest
    {
        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}
