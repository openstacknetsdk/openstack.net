using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.ObjectStorage.v1.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata {

	/// <summary>
	/// Generic metadata
	/// </summary>
	public interface IMetadata : ISerializedKeyValuePair {

		/// <summary>
		/// Header key of Metatag
		/// </summary>
		string MetadataKey { get; }

		/// <summary>
		/// Value of Metadata
		/// </summary>
		string MetadataValue { get; set; }

	}
}
