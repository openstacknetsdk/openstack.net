using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Provides the same serialization capabilities as json.net with some additions:
    /// <para>* Ensures that empty enumerables are not serialized.</para>
    /// </summary>
    public class OpenStackContractResolver : DefaultContractResolver
    {
        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            DoNotSerializeEmptyLists(property);

            return property;
        }

        private static void DoNotSerializeEmptyLists(JsonProperty property)
        {
            if (IsEnumerable(property))
            {
                property.ShouldSerialize = containerInstance =>
                {
                    var propertyValue = property.DeclaringType.GetProperty(property.UnderlyingName).GetValue(containerInstance);
                    return propertyValue != null && ((IEnumerable) propertyValue).OfType<object>().Any();
                };
            }
        }

        /// <inheritdoc/>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            var converter =  base.ResolveContractConverter(objectType);

            var jsonConverterAttr = objectType.GetCustomAttribute<JsonConverterWithConstructorAttribute>(inherit:true);
            if (jsonConverterAttr != null)
                return jsonConverterAttr.CreateJsonConverterInstance();

            return converter;
        }

        /// <summary>
        /// Check if a property implements IEnumerable and IEnumerable&lt;&gt;
        /// </summary>
        private static bool IsEnumerable(JsonProperty property)
        {
            var interfaces = property.PropertyType.GetInterfaces();
            return interfaces.Any(i => i == typeof(IEnumerable));
        }
    }
}
