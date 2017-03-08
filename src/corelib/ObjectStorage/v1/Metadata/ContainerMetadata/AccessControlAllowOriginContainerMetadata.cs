using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerMetadata {

	/// <summary>
	/// The `Access-Control-Allow-Origin` header metadata.
	/// </summary>
	public class AccessControlAllowOriginContainerMetadata : MetadataBase, IContainerMetadata
	{

		/// <summary>
		/// Create new instance
		/// </summary>
		public AccessControlAllowOriginContainerMetadata() : base("Access-Control-Allow-Origin")
		{
			
		}
		
		/// <summary>
		/// Get or set list of allowed origins. Example: [ "http://web.com", "https://web.com" ]
		/// </summary>
		public string[] Origins
		{
			get { return parseValue(this.MetadataValue); }
			set { this.MetadataValue = serializeValue(value); }
		}

		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		private static string serializeValue(string[] value)
		{
			if (value == null) return "";

			return string.Join(" ", value);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="value"></param>
		private static string[] parseValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			return value.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
