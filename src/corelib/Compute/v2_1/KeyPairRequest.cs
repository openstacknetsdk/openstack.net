using Newtonsoft.Json;
using OpenStack.Compute.v2_1.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverter(typeof(KeyPairConverter))]
    public class KeyPairRequest
    {
        /// <summary />
        public KeyPairRequest(string name)
        {
            Name = name;
        }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}