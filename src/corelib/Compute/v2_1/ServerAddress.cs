using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Networking;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ServerAddress : IHaveExtraData
    {
        /// <summary />
        [JsonProperty("addr")]
        public string IP { get; set; }

        /// <summary />
        [JsonProperty("version")]
        public IPVersion Version { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-IPS-MAC:mac_addr")]
        public string MAC { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-IPS:type")]
        public AddressType Type { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}