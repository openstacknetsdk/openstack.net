using System;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    [Serializable]
    public class MetaDataResponse
    {
        public Metadata Metadata { get; set; }
    }
}