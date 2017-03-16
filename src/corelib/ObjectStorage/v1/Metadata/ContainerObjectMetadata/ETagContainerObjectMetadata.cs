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
	public class ETagContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{

		/// <summary>
		/// Create new instance
		/// </summary>
		public ETagContainerObjectMetadata() : base("ETag", false)
		{
			
		}
		
		/// <summary>
		/// Get or set ETag of Object
		/// </summary>
		public string ETag
		{
			get { return MetadataConverter.ParseStringSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeStringValue(value); }
		}
		
	}
}
