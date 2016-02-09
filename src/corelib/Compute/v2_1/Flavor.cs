using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "flavor")]
    public class Flavor : FlavorSummary
    {
        /// <summary />
        [JsonProperty("disk")]
        public int DiskSize { get; set; }

        /// <summary />
        [JsonProperty("ram")]
        public int MemorySize { get; set; }

        /// <summary />
        [JsonProperty("swap")]
        public int? SwapSize { get; set; }

        /// <summary />
        [JsonProperty("vcpus")]
        public int VirtualCPUs { get; set; }

        /// <summary />
        [JsonProperty("rxtx_factor")]
        public double? BandwidthCap { get; set; }

        /// <summary />
        [JsonProperty("OS-FLV-EXT-DATA:ephemeral")]
        public int? EphemeralDiskSize { get; set; }
    }
}