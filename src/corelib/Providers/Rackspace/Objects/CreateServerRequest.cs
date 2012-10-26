using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    public class CreateServerRequest
    {
        public string Name { get; set; }

        public string ImageName { get; set; }

        public string Flavor { get; set; }

        public string DiskConfig { get; set; }

        public List<MetaData> Metadata { get; set; }

        public string FriendlyName { get; set; }

        public CreateServerRequest()
        {
            Metadata = new List<MetaData>();
        }
    }
}
