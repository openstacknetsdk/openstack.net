using System.Collections.Generic;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Represents a collection of network definition resources of the <see cref="NetworkingService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverterWithConstructor(typeof (RootWrapperConverter), "networks")]
    internal class NetworkDefinitionCollection : List<NetworkDefinition>
    {
        public NetworkDefinitionCollection(IEnumerable<NetworkDefinition> networks) : base(networks)
        {
        }
    }
}