using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class FlavorDetails : Flavor
    {
        [DataMember(Name ="OS-FLV-DISABLED:disabled")]
        public bool Disabled { get; set; }

        [DataMember(Name ="disk")] 
        public int DiskSizeInGB { get; set; }

        [DataMember(Name = "ram")]
        public int RAMInMB { get; set; }

        //"rxtx_factor": 2.0,
 
        //"swap": 512, 

        [DataMember(Name ="vcpus")]
        public int VirtualCPUCount { get; set; }
    }
}