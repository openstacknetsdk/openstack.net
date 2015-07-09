using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Provides the same serialization capabilities as json.net but ensures that when an enumerable property is deserialized, it is never null and is always an empty collection
    /// This makes it safe for users to iterate without checking for null
    /// </summary>
    internal class EmptyEnumerableResolver : DefaultContractResolver
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
