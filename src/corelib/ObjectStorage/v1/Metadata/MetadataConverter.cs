using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.Metadata {

	/// <summary>
	/// Library of converters to/from metadata value
	/// </summary>
	public static class MetadataConverter {

		#region String converter

		/// <summary>
		/// Serialize the string value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string[] SerializeStringValue(string value)
		{
			if (value == null) return null;
			
			return new []{ value };
		}

		/// <summary>
		/// Serialize the string value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string[] SerializeStringValue(IEnumerable<string> value)
		{
			if (value == null) return null;

			return value.ToArray();
		}
		
		/// <summary>
		/// Parse the serialized value
		/// </summary>
		/// <param name="serializedValue"></param>
		/// <returns></returns>
		public static string ParseStringSingleValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;
			
			return serializedValue.FirstOrDefault();
		}

		/// <summary>
		/// Parse the serialized value
		/// </summary>
		/// <param name="serializedValue"></param>
		/// <returns></returns>
		public static string[] ParseStringMultiValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;

			return serializedValue.ToArray();
		}

		#endregion

		#region Long converter
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeLongValue(long value)
		{
			return new [] { value.ToString("0", CultureInfo.InvariantCulture) };
		}
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeLongValue(IEnumerable<long> value)
		{
			if (value == null) return null;

			return value.Select(item => item.ToString("0", CultureInfo.InvariantCulture)).ToArray();
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static long ParseLongSingleValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return 0;

			var firstValue = serializedValue.FirstOrDefault();
			if (firstValue == null) return 0;

			return long.Parse(firstValue, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static long[] ParseLongMultiValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;

			return serializedValue.Select(item => long.Parse(item, CultureInfo.InvariantCulture)).ToArray();
		}

		#endregion
		
		#region Double converter
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeDoubleValue(double value)
		{
			return new [] { value.ToString("R", CultureInfo.InvariantCulture) };
		}
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeDoubleValue(IEnumerable<double> value)
		{
			if (value == null) return null;

			return value.Select(item => item.ToString("R", CultureInfo.InvariantCulture)).ToArray();
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static double ParseDoubleSingleValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return 0;

			var firstValue = serializedValue.FirstOrDefault();
			if (firstValue == null) return 0;

			return double.Parse(firstValue, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static double[] ParseDoubleMultiValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;

			return serializedValue.Select(item => double.Parse(item, CultureInfo.InvariantCulture)).ToArray();
		}

		#endregion
		
		#region DateTime converter
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeDateTimeValue(DateTime value)
		{
			return new [] { value.ToString(CultureInfo.InvariantCulture.DateTimeFormat) };
		}
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeDateTimeValue(IEnumerable<DateTime> value)
		{
			if (value == null) return null;

			return value.Select(item => item.ToString(CultureInfo.InvariantCulture.DateTimeFormat)).ToArray();
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static DateTime ParseDateTimeSingleValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return DateTime.MinValue;

			var firstValue = serializedValue.FirstOrDefault();
			if (firstValue == null) return DateTime.MinValue;

			return DateTime.Parse(firstValue,
				System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat,
				DateTimeStyles.AssumeUniversal
			);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static DateTime[] ParseDateTimeMultiValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;

			return serializedValue
				.Select(item => DateTime.Parse(item,
					System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat,
					DateTimeStyles.AssumeUniversal
				))
				.ToArray();
		}

		#endregion
		
		#region DateTimeOffset converter
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeDateTimeOffsetValue(DateTimeOffset value)
		{
			return new [] { value.ToString("r", CultureInfo.InvariantCulture.DateTimeFormat) };
		}
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeDateTimeOffsetValue(IEnumerable<DateTimeOffset> value)
		{
			if (value == null) return null;

			return value.Select(item => item.ToString(CultureInfo.InvariantCulture.DateTimeFormat)).ToArray();
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static DateTimeOffset ParseDateTimeOffsetSingleValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return DateTimeOffset.MinValue;

			var firstValue = serializedValue.FirstOrDefault();
			if (firstValue == null) return DateTimeOffset.MinValue;

			return DateTimeOffset.Parse(firstValue,
				System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat,
				DateTimeStyles.AssumeUniversal
			);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static DateTimeOffset[] ParseDateTimeOffsetMultiValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;

			return serializedValue
				.Select(item => DateTimeOffset.Parse(item,
					System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat,
					DateTimeStyles.AssumeUniversal
				))
				.ToArray();
		}

		#endregion
		
		#region Timestamp converter
		
		private static readonly DateTimeOffset zeroDayUnixTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, new TimeSpan(0));

		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeTimestampValue(DateTimeOffset value)
		{
			return new [] { value.Subtract(zeroDayUnixTime).TotalSeconds.ToString("R", CultureInfo.InvariantCulture.DateTimeFormat) };
		}
		
		/// <summary>
		/// Serialize value to Metadata
		/// </summary>
		/// <returns></returns>
		public static string[] SerializeTimestampValue(IEnumerable<DateTimeOffset> value)
		{
			if (value == null) return null;

			return value.Select(item => item.Subtract(zeroDayUnixTime).TotalSeconds.ToString("R", CultureInfo.InvariantCulture.DateTimeFormat)).ToArray();
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static DateTimeOffset ParseTimestampSingleValue(IEnumerable<string> serializedValue)
		{
			var doubleValue = ParseDoubleSingleValue(serializedValue);

			return zeroDayUnixTime.AddSeconds(doubleValue);
		}

		/// <summary>
		/// Parse value from Metadata
		/// </summary>
		/// <param name="serializedValue"></param>
		public static DateTimeOffset[] ParseTimestampMultiValue(IEnumerable<string> serializedValue)
		{
			if (serializedValue == null) return null;

			return ParseDoubleMultiValue(serializedValue)
				.Select(item => zeroDayUnixTime.AddSeconds(item))
				.ToArray();
		}

		#endregion
	}
}
