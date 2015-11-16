using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "keypair")]
    public class KeyPairDefinition
    {
        /// <summary />
        public KeyPairDefinition(string name)
        {
            Name = name;
        }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }
    }
}