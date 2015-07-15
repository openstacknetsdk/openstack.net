using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Provides the same serialization capabilities as json.net with some additions:
    /// <para>* Ensures that when an enumerable property is deserialized, it is never null and is always an empty collection.</para>
    /// <para>* Handles adding/unwrapping superfluous root containers.</para>
    /// </summary>
    internal class OpenStackContractResolver : DefaultContractResolver
    {
        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            IValueProvider provider = base.CreateMemberValueProvider(member);

            if (member.MemberType != MemberTypes.Property)
                return provider;

            Type propType = ((PropertyInfo)member).PropertyType;
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return new EmptyEnumerableValueProvider(provider, propType);
            }

            return provider;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            var converter =  base.ResolveContractConverter(objectType);

            var jsonConverterAttr = objectType.GetCustomAttribute<JsonConverterWithConstructorAttribute>(inherit:true);
            if (jsonConverterAttr != null)
                return jsonConverterAttr.CreateJsonConverterInstance();

            return converter;
        }

        private class EmptyEnumerableValueProvider : IValueProvider
        {
            private readonly IValueProvider _innerProvider;
            private readonly object _emptyEnumerable;

            public EmptyEnumerableValueProvider(IValueProvider innerProvider, Type enumerableType)
            {
                _innerProvider = innerProvider;
                Type t = enumerableType.GetGenericArguments()[0];
                _emptyEnumerable = Array.CreateInstance(t, 0);
            }

            public void SetValue(object target, object value)
            {
                _innerProvider.SetValue(target, value ?? _emptyEnumerable);
            }

            public object GetValue(object target)
            {
                return _innerProvider.GetValue(target) ?? _emptyEnumerable;
            }
        }
    }
}
