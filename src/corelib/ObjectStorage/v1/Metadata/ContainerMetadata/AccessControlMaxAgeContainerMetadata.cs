using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerMetadata {

	/// <summary>
	/// The `Access-Control-Max-Age` header metadata.
	/// </summary>
	public class AccessControlMaxAgeContainerMetadata : MetadataBase, IContainerMetadata {

		/// <summary>
		/// Create new instance
		/// </summary>
		public AccessControlMaxAgeContainerMetadata() : base("Access-Control-Max-Age")
		{
			
		}
		
		/// <summary>
		/// Get or set the max age of container objects, in seconds.
		/// </summary>
		public long MaxAgeSeconds
		{
			get { return parseValue(this.Value); }
			set { this.Value = serializeValue(value); }
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
