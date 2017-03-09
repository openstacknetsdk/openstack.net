using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.ObjectStorage.v1.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata {

	/// <summary>
	/// Base implementation of generic Metadata
	/// </summary>
	public abstract class MetadataBase : IMetadata {

		/// <summary>
		/// Create new <see cref="MetadataBase"/>.
		/// </summary>
		/// <param name="metadataKey"></param>
		/// <param name="allowMultiValue"></param>
		public MetadataBase(string metadataKey, bool allowMultiValue)
		{
			this.MetadataKey = metadataKey;
			this.AllowMultiValue = allowMultiValue;
		}

		/// <summary>
		/// Get metadata Key
		/// </summary>
		[JsonProperty]
		public string MetadataKey { get; }

		/// <summary>
		/// If True <see cref="MetadataValue"/> can contains multi value, otherwise one only.
		/// </summary>
		public bool AllowMultiValue { get; }

		/// <summary>
		/// Get or set value
		/// </summary>
		[JsonProperty]
		public string[] MetadataValue { get; set; }

		/// <summary>
		/// Convert Metadata to standard KeyValuePair structure
		/// </summary>
		/// <returns></returns>
		public KeyValuePair<string, IEnumerable<string>> ToKeyValuePair()
		{
			return new KeyValuePair<string, IEnumerable<string>>(this.MetadataKey, this.MetadataValue);
		}
		
	}
}
