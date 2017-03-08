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

		private static readonly DateTime zeroDayUnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		/// <summary>
		/// Create new instance
		/// </summary>
		public TimestampContainerObjectMetadata() : base("X-Timestamp")
		{
			
		}
		
		/// <summary>
		/// Get or set LastUpdate of Object
		/// </summary>
		public DateTime LastUpdate
		{
			get { return parseValue(this.Value); }
			set { this.Value = serializeValue(value); }
		}

		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		private static string serializeValue(DateTime value)
		{
			return value.Subtract(zeroDayUnixTime).TotalDays.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="value"></param>
		private static DateTime parseValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return zeroDayUnixTime;
			}

			var valueDays = System.Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);

			return zeroDayUnixTime.AddDays(valueDays);
		}
	}
}
