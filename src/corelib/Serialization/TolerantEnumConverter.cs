using System;
using System.Linq;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Attempts to convert a string to an enum using the following resolution strategy:
    /// <para>1. Use the name of the enum.</para>
    /// <para>2. Use null if the property is nullable.</para>
    /// <para>3. Use the Unknown enum value.</para>
    /// <para>4. Use the first enum value.</para>
    /// </summary>
    /// <seealso href="http://stackoverflow.com/a/22755077/808818"/>
    public class TolerantEnumConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            Type type = IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            return type.IsEnum;
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);
            Type enumType = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            string[] names = Enum.GetNames(enumType);

            string enumText = reader.Value.ToString();

            if (!string.IsNullOrEmpty(enumText))
            {
                string match = names.FirstOrDefault(n => string.Equals(n, enumText, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    return Enum.Parse(enumType, match);
            }

            if (!isNullable)
            {
                string defaultName = names.FirstOrDefault(n => string.Equals(n, "Unknown", StringComparison.OrdinalIgnoreCase));
                if (defaultName == null)
                    defaultName = names.First();

                return Enum.Parse(enumType, defaultName);
            }

            return null;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        private bool IsNullableType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}