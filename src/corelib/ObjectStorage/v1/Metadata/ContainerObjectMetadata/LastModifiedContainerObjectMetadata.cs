using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata {

	/// <summary>
	/// The `X-Timestamp` header metadata.
	/// </summary>
	public class LastModifiedContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{
		
		/// <summary>
		/// Create new instance
		/// </summary>
		public LastModifiedContainerObjectMetadata() : base("Last-Modified", false)
		{
			
		}
		
		/// <summary>
		/// Get or set LastModified of Object
		/// </summary>
		public DateTime LastModified
		{
			get { return MetadataConverter.ParseDateTimeSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeDateTimeValue(value); }
		}
		
	}
}
