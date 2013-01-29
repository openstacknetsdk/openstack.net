using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class UserCredentialResponse
    {
        [DataMember(Name = "RAX-KSKEY:apiKeyCredentials")]
        public UserCredential UserCredential { get; set; }
    }
}
