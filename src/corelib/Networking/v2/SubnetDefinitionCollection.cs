using System.Collections.Generic;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Represents a collection of subnet definition resources of the <see cref="NetworkingService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "subnets")]
    internal class SubnetDefinitionCollection : List<SubnetCreateDefinition>
    {
        public SubnetDefinitionCollection(IEnumerable<SubnetCreateDefinition> subnets) : base(subnets)
        {
        }
    }
}