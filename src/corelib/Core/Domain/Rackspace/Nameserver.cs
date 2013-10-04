using System.Runtime.Serialization;

namespace net.openstack.Core.Domain.Rackspace
{
    [DataContract]
    public class Nameserver
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
