using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class KeyPairResponse : KeyPairSummary
    {
        /// <summary />
        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }
    }
}