using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata {

	/// <summary>
	/// The `Content-Type` header metadata.
	/// </summary>
	public class ContentTypeContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{

		/// <summary>
		/// Create new instance
		/// </summary>
		public ContentTypeContainerObjectMetadata() : base("Content-Type")
		{
			
		}
		
		/// <summary>
		/// Get or set content-type of Object
		/// </summary>
		public string ContentType
		{
			get { return parseValue(this.MetadataValue); }
			set { this.MetadataValue = serializeValue(value); }
		}

		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		private static string serializeValue(string value)
		{
			return value ?? "";
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="value"></param>
		private static string parseValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			return value;
		}
	}
}
