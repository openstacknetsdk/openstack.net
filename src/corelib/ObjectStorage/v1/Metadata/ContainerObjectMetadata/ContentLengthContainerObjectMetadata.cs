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
		public ContentLengthContainerObjectMetadata() : base("Content-Length")
		{
			
		}
		
		/// <summary>
		/// Get or set content-length of Object
		/// </summary>
		public long ContentLength
		{
			get { return parseValue(this.MetadataValue); }
			set { this.MetadataValue = serializeValue(value); }
		}

		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		private static string serializeValue(long value)
		{
			return value.ToString("0");
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="value"></param>
		private static long parseValue(string value)
		{
			return System.Convert.ToInt64(value);
		}
	}
}
