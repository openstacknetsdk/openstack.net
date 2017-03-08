using System.Collections.Generic;
using OpenStack.ObjectStorage.v1.Metadata;
using OpenStack.ObjectStorage.v1.Metadata.ContainerMetadata;
using OpenStack.Serialization;

namespace OpenStack.ObjectStorage.v1.Serialization
{
    /// <summary>
    /// Represents a collection of Container Metadata.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <exclude />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "containerMetadataList")]
    public class ContainerMetadataCollection : List<IContainerMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerMetadataCollection"/> class.
        /// </summary>
        public ContainerMetadataCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerMetadataCollection"/> class.
        /// </summary>
        /// <param name="containerMetadataList">The list of metadata.</param>
        public ContainerMetadataCollection(IEnumerable<IContainerMetadata> containerMetadataList) : base(containerMetadataList)
        {
        }
    }
}