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
		public ContentTypeContainerObjectMetadata() : base("Content-Type", false)
		{
			
		}
		
		/// <summary>
		/// Get or set content-type of Object
		/// </summary>
		public string ContentType
		{
			get { return MetadataConverter.ParseStringSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeStringValue(value); }
		}
		
	}
}
