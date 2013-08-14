namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class FlavorDetails : Flavor
    {
        [JsonProperty("OS-FLV-DISABLED:disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("disk")] 
        public int DiskSizeInGB { get; set; }

        [JsonProperty("ram")]
        public int RAMInMB { get; set; }

        //"rxtx_factor": 2.0,
 
        //"swap": 512, 

        [JsonProperty("vcpus")]
        public int VirtualCPUCount { get; set; }
    }
}