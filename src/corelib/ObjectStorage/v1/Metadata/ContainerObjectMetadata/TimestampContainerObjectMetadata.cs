using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata {

	/// <summary>
	/// The `X-Timestamp` header metadata.
	/// </summary>
	public class TimestampContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{
		
		/// <summary>
		/// Create new instance
		/// </summary>
		public TimestampContainerObjectMetadata() : base("X-Timestamp", false)
		{
			
		}

		/// <summary>
		/// Get or set Timestamp of Object
		/// </summary>
		public double Timestamp
		{
			get { return MetadataConverter.ParseDoubleSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeDoubleValue(value); }
		}

		/// <summary>
		/// Get or set Timestamp of Object in DateTime format
		/// </summary>
		public DateTimeOffset TimestampDate
		{
			get { return MetadataConverter.ParseTimestampSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeTimestampValue(value); }
		}
		
		
	}
}
