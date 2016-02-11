using Newtonsoft.Json;
using OpenStack.Compute.v2_1.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Defines a key pair which can be imported into the compute service.
    /// </summary>
    [JsonConverter(typeof(KeyPairConverter))]
    public class KeyPairDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPairDefinition"/> class.
        /// </summary>
        /// <param name="name">The key pair name.</param>
        /// <param name="publicKey">The public ssh key to import.</param>
        public KeyPairDefinition(string name, string publicKey)
        {
            Name = name;
            PublicKey = publicKey;
        }

        /// <summary>
        /// The key pair name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The public ssh key to import.
        /// </summary>
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }
    }
}