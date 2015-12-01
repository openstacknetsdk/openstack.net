using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_2
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "keypair")]
    public class KeyPairDefinition : v2_1.KeyPairDefinition
    {
        /// <summary />
        public KeyPairDefinition(string name) : base(name)
        {
            Name = name;
        }

        /// <summary />
        [JsonProperty("type")]
        public KeyPairType? Type { get; set; }
    }
}