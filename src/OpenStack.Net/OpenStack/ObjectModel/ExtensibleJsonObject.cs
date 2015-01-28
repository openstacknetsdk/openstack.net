namespace OpenStack.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;

    /// <summary>
    /// This is the abstract base class for types modeling the JSON representation of a resource
    /// which may be extended by specific providers or updated in future releases of a core
    /// service.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ExtensibleJsonObject
    {
        /// <summary>
        /// Gets an immutable dictionary representing empty extension data.
        /// </summary>
        protected static readonly ImmutableDictionary<string, JToken> EmptyExtensionData =
            ImmutableDictionary<string, JToken>.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="ExtensionData"/> property.
        /// </summary>
        private ImmutableDictionary<string, JToken> _extensionData = ImmutableDictionary<string, JToken>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibleJsonObject"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ExtensibleJsonObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibleJsonObject"/> class
        /// with the specified extension data.
        /// </summary>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        protected ExtensibleJsonObject(ImmutableDictionary<string, JToken> extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            _extensionData = extensionData.WithComparers(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets a map of object properties which did not map to another field or property
        /// during JSON deserialization. The keys of the map represent the property names,
        /// and the values are <see cref="JToken"/> instances containing the parsed JSON
        /// values.
        /// </summary>
        /// <value>
        /// <para>A collection of object properties which did not map to another field or property
        /// during JSON deserialization.</para>
        /// <para>-or-</para>
        /// <para>An empty dictionary if the object did not contain any unmapped properties.</para>
        /// </value>
        public ImmutableDictionary<string, JToken> ExtensionData
        {
            get
            {
                return _extensionData;
            }
        }

        /// <summary>
        /// This property exposes the <see cref="_extensionData"/> field to Json.NET as a mutable dictionary so it can
        /// be updated during JSON deserialization.
        /// </summary>
        [JsonExtensionData]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExtensionDataDictionary ExtensionDataWrapper
        {
            get
            {
                // This can never return null or Json.NET will attempt to set the value.
                return new ExtensionDataDictionary(this);
            }

            set
            {
                // This setter must exist or Json.NET will not recognize the extension data. It cannot be used because
                // Json.NET will bypass the getter, resulting in a lost update.
                throw new NotSupportedException("Attempted to set the extension data wrapper. See issue openstacknetsdk/openstack.net#419.");
            }
        }

        /// <summary>
        /// Gets an <see cref="ExtensibleJsonObject"/> with the same type and properties from the current object and the
        /// specified extension data.
        /// </summary>
        /// <remarks>
        /// <para>This method provides the implementation for
        /// <see cref="O:OpenStack.ObjectModel.ExtensibleJsonObjectExtensions.WithExtensionData``2"/>.</para>
        /// <note type="implement">
        /// <para>This method is only intended to be overridden in cases where <see cref="object.MemberwiseClone"/>
        /// cannot be used to clone the current instance. Any override should ensure that the return value has the same
        /// type as the current instance, or throw a <see cref="NotSupportedException"/>.</para>
        /// </note>
        /// </remarks>
        /// <param name="extensionData">The new extension data for the object.</param>
        /// <returns>
        /// An <see cref="ExtensibleJsonObject"/> which represents the current object with the specified extension data.
        /// If <paramref name="extensionData"/> is the same as the existing <see cref="ExtensionData"/> for the current
        /// object, the method may return the same instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="extensionData"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If a new object cannot be created from the current object in order to set the extension data.
        /// </exception>
        protected internal virtual ExtensibleJsonObject WithExtensionDataImpl(ImmutableDictionary<string, JToken> extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            if (extensionData == _extensionData)
                return this;

            ExtensibleJsonObject result = (ExtensibleJsonObject)MemberwiseClone();
            result._extensionData = extensionData.WithComparers(StringComparer.Ordinal);
            return result;
        }

        /// <summary>
        /// Converts an object to a <see cref="JToken"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="JToken.FromObject(object)"/>, this method supports <see langword="null"/> values.
        /// </para>
        /// </remarks>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// <para>The result of calling <see cref="JToken.FromObject(object)"/> on the input object.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="obj"/> is <see langword="null"/>.</para>
        /// </returns>
        private static JToken ToJToken(object obj)
        {
            if (obj == null)
                return null;

            return JToken.FromObject(obj);
        }

        /// <summary>
        /// This class exposes the immutable dictionary used by this class to Json.NET as a mutable dictionary.
        /// </summary>
        /// <remarks>
        /// <para>Adding values to the underlying dictionary requires converting the value to a <see cref="JToken"/> by
        /// calling <see cref="ToJToken(object)"/>. Reading values does not require the inverse because the serializer
        /// in Json.NET has no trouble handling <see cref="JToken"/> values as input.</para>
        /// </remarks>
        /// <seealso cref="ExtensionDataWrapper"/>
        private sealed class ExtensionDataDictionary : IDictionary<string, object>
        {
            /// <summary>
            /// The backing extensible JSON object.
            /// </summary>
            private readonly ExtensibleJsonObject _underlying;

            [JsonConstructor]
            private ExtensionDataDictionary()
            {
                // This constructor must exist or Json.NET will not be able to set the extension data. It cannot be used
                // because Json.NET will not set the required _underlying field.
                throw new NotSupportedException("Attempted to create the extension data wrapper with its underlying object. See issue openstacknetsdk/openstack.net#419.");
            }

            public ExtensionDataDictionary(ExtensibleJsonObject extensibleJsonObject)
            {
                if (extensibleJsonObject == null)
                    throw new ArgumentNullException("extensibleJsonObject");

                _underlying = extensibleJsonObject;
            }

            public object this[string key]
            {
                get
                {
                    return _underlying.ExtensionData[key];
                }

                set
                {
                    _underlying._extensionData = _underlying._extensionData.SetItem(key, ToJToken(value));
                }
            }

            public int Count
            {
                get
                {
                    return _underlying.ExtensionData.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    return ((IDictionary<string, JToken>)_underlying.ExtensionData).Keys;
                }
            }

            public ICollection<object> Values
            {
                get
                {
                    return new ExtensionDataValues(((IDictionary<string, JToken>)_underlying.ExtensionData).Values);
                }
            }

            public void Add(KeyValuePair<string, object> item)
            {
                _underlying._extensionData = _underlying._extensionData.SetItem(item.Key, ToJToken(item.Value));
            }

            public void Add(string key, object value)
            {
                _underlying._extensionData = _underlying._extensionData.Add(key, ToJToken(value));
            }

            public void Clear()
            {
                _underlying._extensionData = _underlying._extensionData.Clear();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                return _underlying.ExtensionData.Contains(new KeyValuePair<string, JToken>(item.Key, ToJToken(item.Value)));
            }

            public bool ContainsKey(string key)
            {
                return _underlying.ExtensionData.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                IDictionary<string, object> intermediate = new Dictionary<string, object>(_underlying.ExtensionData.ToDictionary(i => i.Key, i => (object)i.Value));
                intermediate.CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return _underlying.ExtensionData.Select(i => new KeyValuePair<string, object>(i.Key, i.Value)).GetEnumerator();
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                IDictionary<string, JToken> extensionData = _underlying._extensionData;
                if (extensionData == null)
                    return false;

                return extensionData.Remove(new KeyValuePair<string, JToken>(item.Key, ToJToken(item.Value)));
            }

            public bool Remove(string key)
            {
                var extensionData = _underlying._extensionData;
                _underlying._extensionData = extensionData.Remove(key);
                return _underlying._extensionData != extensionData;
            }

            public bool TryGetValue(string key, out object value)
            {
                JToken intermediate;
                bool result = _underlying.ExtensionData.TryGetValue(key, out intermediate);
                value = intermediate;
                return result;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// This class exposes the immutable dictionary used by this class to Json.NET as a mutable dictionary.
        /// </summary>
        /// <seealso cref="ExtensionDataWrapper"/>
        private class ExtensionDataValues : ICollection<object>
        {
            private readonly ICollection<JToken> _values;

            public ExtensionDataValues(ICollection<JToken> values)
            {
                if (values == null)
                    throw new ArgumentNullException("values");

                _values = values;
            }

            public int Count
            {
                get
                {
                    return _values.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _values.IsReadOnly;
                }
            }

            public void Add(object item)
            {
                _values.Add(ToJToken(item));
            }

            public void Clear()
            {
                _values.Clear();
            }

            public bool Contains(object item)
            {
                return _values.Contains(ToJToken(item));
            }

            public void CopyTo(object[] array, int arrayIndex)
            {
                ICollection<object> intermediate = _values.ToArray();
                intermediate.CopyTo(array, arrayIndex);
            }

            public IEnumerator<object> GetEnumerator()
            {
                return _values.Cast<object>().GetEnumerator();
            }

            public bool Remove(object item)
            {
                return _values.Remove(ToJToken(item));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
