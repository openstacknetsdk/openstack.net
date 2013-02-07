using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Providers.Rackspace.Objects.Request;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class PasswordCredencialResponse
    {
        [DataMember(Name = "passwordCredentials")]
        public PasswordCredencial PasswordCredencial { get; set; }
    }
}
