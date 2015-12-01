using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Attempts to convert a string to a <see cref="StringEnumeration"/> using the following resolution strategy:
    /// <para>1. Use StringEnumConverter.</para>
    /// <para>2. Use null if the property is nullable.</para>
    /// <para>3. Use the Unknown enum value.</para>
    /// <para>4. Use the first enum value.</para>
    /// </summary>
    /// <seealso href="http://stackoverflow.com/a/22755077/808818"/>
    /// <exclude />
    public class StringEnumerationConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return GetType(objectType).IsSubclassOf(typeof(StringEnumeration));
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type type = GetType(objectType);

            string displayName = serializer.Deserialize<string>(reader);
            var result = StringEnumeration.FromDisplayName(type, displayName);

            if(IsNullableType(objectType))
                return result;

            IEnumerable<StringEnumeration> availableValues = StringEnumeration.GetAll(type);
            result = availableValues.FirstOrDefault(n => string.Equals(n.DisplayName, "Unknown", StringComparison.OrdinalIgnoreCase)) ?? availableValues.First();
            return result;
        }

        /// <summary />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumeration = (StringEnumeration)value;
            serializer.Serialize(writer, enumeration.DisplayName);
        }

        /// <summary />
        private static Type GetType(Type objectType)
        {
            return IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
        }

        private static bool IsNullableType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}