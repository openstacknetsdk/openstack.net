using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata {

	/// <summary>
	/// The `Cache-Control` header metadata.
	/// </summary>
	public class CacheControlContainerObjectMetadata : MetadataBase, IContainerObjectMetadata
	{
		
		/// <summary>
		/// Create new instance
		/// </summary>
		public CacheControlContainerObjectMetadata() : base("Cache-Control", false)
		{	
		}

		/// <summary>
		/// Get or set the Cacheability vale
		/// </summary>
		public CacheabilityEnum Cacheability
		{
			get
			{
				var metaValue = new CacheControlValue(this.MetadataValue);
				return metaValue.Cacheability;
			}
			set
			{
				var metaValue = new CacheControlValue(this.MetadataValue);
				metaValue.Cacheability = value;
				this.MetadataValue = metaValue.ToMetadata();
			}
		}

		/// <summary>
		/// Get or set the max age of cached item
		/// </summary>
		public TimeSpan MaxAge
		{
			get
			{
				var metaValue = new CacheControlValue(this.MetadataValue);
				return metaValue.MaxAge;
			}
			set
			{
				var metaValue = new CacheControlValue(this.MetadataValue);
				metaValue.MaxAge = value;
				this.MetadataValue = metaValue.ToMetadata();
			}
		}

		private static bool tryGetCacheability(string metadataPart, out CacheabilityEnum value)
		{
			var part = metadataPart.Trim();

			var enumValues = Enum.GetValues(typeof(CacheabilityEnum)).Cast<CacheabilityEnum>();
			foreach (var enumValue in enumValues)
			{
				var enumValueAttr = MetadataEnumValueAttribute.GetAttribute(enumValue);
				if (enumValueAttr.MetadataValue.Equals(part, StringComparison.InvariantCultureIgnoreCase))
				{
					value = enumValue;
					return true;
				}
			}

			value = CacheabilityEnum.NoCache;
			return false;
		}

		private static bool tryGetMaxAge(string metadataPart, out TimeSpan value)
		{
			var re = new Regex(@"\s*max-age=(?<time>[0-9]+)\s*");
			var match = re.Match(metadataPart);
			if (match.Success == false)
			{
				value = TimeSpan.Zero;
				return false;
			}

			value = new TimeSpan(0, 0, 0, int.Parse(match.Groups["time"].Value, System.Globalization.CultureInfo.InvariantCulture), 0);
			return true;
		}


		internal class CacheControlValue
		{
			public CacheControlValue(string[] metadataValue)
			{
				if (metadataValue == null) return;

				var metadataValueItem = metadataValue.FirstOrDefault();
				if (string.IsNullOrEmpty(metadataValueItem)) return;

				var parts = metadataValueItem.Split(',');
				foreach (var part in parts)
				{
					// Check for `Cacheability`
					CacheabilityEnum cache;
					if (tryGetCacheability(part, out cache))
					{
						this.Cacheability = cache;
						continue;
					}

					// Check for `MaxAge`
					TimeSpan age;
					if (tryGetMaxAge(part, out age))
					{
						this.MaxAge = age;
						continue;
					}
				}
			}

			public CacheabilityEnum Cacheability { get; set; }

			public TimeSpan MaxAge { get; set; }

			public string[] ToMetadata()
			{
				var sb = new StringBuilder();
				sb.Append(MetadataEnumValueAttribute.GetAttribute(this.Cacheability).MetadataValue);

				if (this.Cacheability != CacheabilityEnum.NoCache)
				{
					sb.Append(", max-age=");
					sb.Append(this.MaxAge.TotalSeconds.ToString("0"));
				}

				return new []{ sb.ToString() };
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public enum CacheabilityEnum
		{
			/// <summary>
			/// Indicates that the response may be cached by any cache.
			/// </summary>
			[MetadataEnumValue("public")]
			PublicCache = 1,

			/// <summary>
			/// Indicates that the response is intended for a single user and must not be stored by a shared cache. A private cache may store the response.
			/// </summary>
			[MetadataEnumValue("private")]
			PrivateCache = 2,

			/// <summary>
			/// Forces caches to submit the request to the origin server for validation before releasing a cached copy.
			/// </summary>
			[MetadataEnumValue("no-cache")]
			NoCache = 3
		}

	}
}
