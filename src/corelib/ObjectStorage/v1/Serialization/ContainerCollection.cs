using System.Collections.Generic;
using OpenStack.Serialization;

namespace OpenStack.ObjectStorage.v1.Serialization
{
    /// <summary>
    /// Represents a collection of Object resources returned by the <see cref="ObjectStorage"/>.
    /// <para>Intended for custom implementations and stubbing responses in unit tests.</para>
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <exclude />
    public class ContainerCollection : List<Container>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerCollection"/> class.
        /// </summary>
        public ContainerCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerCollection"/> class.
        /// </summary>
        /// <param name="containers">The containers.</param>
        public ContainerCollection(IEnumerable<Container> containers) : base(containers)
        {
        }
    }
}