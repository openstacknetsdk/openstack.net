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
    public class ContainerItemCollection : List<IContainerItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItemCollection"/> class.
        /// </summary>
        public ContainerItemCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItemCollection"/> class.
        /// </summary>
        /// <param name="containerObjects">The container objects.</param>
        public ContainerItemCollection(IEnumerable<ContainerObject> containerObjects) : base(containerObjects)
        {
        }
    }
}