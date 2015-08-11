using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Represents the definition of a network resource of the <see cref="NetworkingService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "network")]
    public class NetworkDefinition
    {
        /// <summary>
        /// The network name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}