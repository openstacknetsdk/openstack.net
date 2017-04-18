using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.Metadata {

	/// <summary>
	/// Allow to define the Metadata value of enum configuration item.
	/// </summary>
	internal class MetadataEnumValueAttribute
		: Attribute
	{

		private readonly string _metadataValue;

		public MetadataEnumValueAttribute(string metadataValue)
		{
			this._metadataValue = metadataValue;
		}

		/// <summary>
		/// The metadata value
		/// </summary>
		public string MetadataValue
		{
			get { return this._metadataValue; }
		}

		/// <summary>
		/// Returns the MetadataEnumValue attribute by the Enum value.
		/// </summary>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static MetadataEnumValueAttribute GetAttribute(Enum enumValue)
		{
			var metadataAttrib = typeof(MetadataEnumValueAttribute);
			var enumType = enumValue.GetType();
			var enumValueType = enumType.GetMember(enumValue.ToString()).First();

			var enumValueAttr = enumValueType.GetCustomAttributes(metadataAttrib, false)
				.First();

			return enumValueAttr as MetadataEnumValueAttribute;
		}

	}
}
