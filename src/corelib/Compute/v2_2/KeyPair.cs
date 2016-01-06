using Newtonsoft.Json;

namespace OpenStack.Compute.v2_2
{
    /// <summary />
    public class KeyPair : v2_1.KeyPair
    {
        /// <summary />
        [JsonProperty("type")]
        public KeyPairType Type { get; set; }
    }
}