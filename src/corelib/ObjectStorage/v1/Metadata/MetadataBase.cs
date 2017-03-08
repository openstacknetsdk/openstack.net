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
	public abstract class MetadataBase : IMetadata, ISerializedKeyValuePair {

		/// <summary>
		/// Create new <see cref="MetadataBase"/>.
		/// </summary>
		/// <param name="metadataKey"></param>
		public MetadataBase(string metadataKey)
		{
			this.MetadataKey = metadataKey;
			
		}

		/// <summary>
		/// Get metadata Key
		/// </summary>
		[JsonProperty]
		public string MetadataKey { get; }

		/// <summary>
		/// Get or set value
		/// </summary>
		[JsonProperty]
		public string MetadataValue { get; set; }

		string ISerializedKeyValuePair.Key
		{
			get { return this.MetadataKey; }
			set { throw new InvalidOperationException(); }
		}

		string ISerializedKeyValuePair.Value
		{
			get { return this.MetadataValue; }
			set { this.MetadataValue = value; }
		}
	}
}
