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
	public class ExpiresContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{
		
		/// <summary>
		/// Create new instance
		/// </summary>
		public ExpiresContainerObjectMetadata() : base("Expires", false)
		{
			
		}
		
		/// <summary>
		/// Get or set Expire date of Object
		/// </summary>
		public DateTimeOffset ExpireDate
		{
			get { return MetadataConverter.ParseDateTimeOffsetSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeDateTimeOffsetValue(value); }
		}
		
	}
}
