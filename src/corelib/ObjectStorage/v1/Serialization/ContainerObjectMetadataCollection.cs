using System.Collections.Generic;
using System.Linq;
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

		/// <summary>
		/// Convert list to standard KeyValuePair enumerator
		/// </summary>
		/// <returns></returns>
	    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ToKeyValuePairs()
	    {
		    return this.Select(item => item.ToKeyValuePair());
	    }
    }
}