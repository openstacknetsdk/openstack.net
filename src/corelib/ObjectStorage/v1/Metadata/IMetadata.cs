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
	public interface IMetadata /*: ISerializedKeyValuePair*/ {

		/// <summary>
		/// Header key of Metatag
		/// </summary>
		string MetadataKey { get; }

		/// <summary>
		/// Value of Metadata
		/// </summary>
		string[] MetadataValue { get; set; }

		/// <summary>
		/// If True <see cref="MetadataValue"/> can contains multi value, otherwise one only.
		/// </summary>
		bool AllowMultiValue { get; }

		/// <summary>
		/// Convert Metadata to standard KeyValuePair structure
		/// </summary>
		/// <returns></returns>
		KeyValuePair<string, IEnumerable<string>> ToKeyValuePair();
	}
}
