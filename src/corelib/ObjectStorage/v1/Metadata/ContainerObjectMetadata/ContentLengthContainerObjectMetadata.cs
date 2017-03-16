using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata {

	/// <summary>
	/// The `Content-Length` header metadata.
	/// </summary>
	public class ContentLengthContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{

		/// <summary>
		/// Create new instance
		/// </summary>
		public ContentLengthContainerObjectMetadata() : base("Content-Length", false)
		{
			
		}
		
		/// <summary>
		/// Get or set content-length of Object
		/// </summary>
		public long ContentLength
		{
			get { return MetadataConverter.ParseLongSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeLongValue(value); }
		}
		
	}
}
