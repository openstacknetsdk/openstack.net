using System;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    [Serializable]
    public class CreateServerResponse
    {
        public NewServer Server { get; set; }
    }
}
