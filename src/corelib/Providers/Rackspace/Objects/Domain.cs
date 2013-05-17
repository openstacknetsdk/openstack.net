using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects
{
    [DataContract]
    public class Domain
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}