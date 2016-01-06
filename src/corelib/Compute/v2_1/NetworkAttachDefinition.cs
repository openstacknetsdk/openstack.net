using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class NetworkAttachDefinition
    {
        /// <summary />
        [JsonProperty("uuid")]
        public Identifier NetworkId { get; set; }

        /// <summary />
        [JsonProperty("port")]
        public Identifier PortId { get; set; }

        /// <summary />
        [JsonProperty("fixed_ip")]
        public string IPAddress { get; set; }
    }
}