using System.Collections.Generic;
using OpenStack.ObjectStorage.v1.Metadata;
using OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata;
using OpenStack.Serialization;

namespace OpenStack.ObjectStorage.v1.Serialization
{
    /// <summary>
    /// Represents a collection of Container Object Metadata.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <exclude />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "metadataList")]
    public class ContainerObjectMetadataCollection : List<IContainerObjectMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerObjectMetadataCollection"/> class.
        /// </summary>
        public ContainerObjectMetadataCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerObjectMetadataCollection"/> class.
        /// </summary>
        /// <param name="metadataList">The list of metadata.</param>
        public ContainerObjectMetadataCollection(IEnumerable<IContainerObjectMetadata> metadataList) : base(metadataList)
        {
        }
    }
}