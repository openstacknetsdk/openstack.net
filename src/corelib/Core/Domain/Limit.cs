using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Limit
    {
        [DataMember(Name = "resource")] 
        public ResourceField Resource;

        [DataMember(Name = "rate")] 
        public RateField Rate;
    }
}
