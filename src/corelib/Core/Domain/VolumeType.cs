using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class VolumeType
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
