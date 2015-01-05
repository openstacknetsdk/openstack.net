namespace OpenStack.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.ObjectModel;

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
        /// An empty, and thus immutable, value which is the default return value
        /// for <see cref="ExtensionData"/> when the backing field is <see langword="null"/>.
        /// </summary>
        protected static readonly ReadOnlyDictionary<string, JToken> EmptyExtensionData =
            new ReadOnlyDictionary<string, JToken>(new Dictionary<string, JToken>());

        /// <summary>
        /// This is the backing field for the <see cref="ExtensionData"/> property.
        /// </summary>
        [JsonExtensionData]
        private Dictionary<string, JToken> _extensionData;

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
        protected ExtensibleJsonObject(IDictionary<string, JToken> extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            if (extensionData.Count > 0)
                _extensionData = new Dictionary<string, JToken>(extensionData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibleJsonObject"/> class
        /// with the specified extension data.
        /// </summary>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="extensionData"/> contains any <see langword="null"/> values.</exception>
        protected ExtensibleJsonObject(IEnumerable<JProperty> extensionData)
            : this(extensionData.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibleJsonObject"/> class
        /// with the specified extension data.
        /// </summary>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="extensionData"/> contains any <see langword="null"/> values.</exception>
        protected ExtensibleJsonObject(params JProperty[] extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            if (extensionData.Length > 0)
            {
                _extensionData = new Dictionary<string, JToken>();
                foreach (JProperty property in extensionData)
                {
                    if (property == null)
                        throw new ArgumentException("extensionData cannot contain any null values");

                    _extensionData[property.Name] = property.Value;
                }
            }
        }

        /// <summary>
        /// Gets a map of object properties which did not map to another field or property
        /// during JSON deserialization. The keys of the map represent the property names,
        /// and the values are <see cref="JToken"/> instances containing the parsed JSON
        /// values.
        /// </summary>
        /// <value>
        /// A collection of object properties which did not map to another field or property
        /// during JSON deserialization.
        /// <para>-or-</para>
        /// <para>An empty dictionary if the object did not contain any unmapped properties.</para>
        /// </value>
        public ReadOnlyDictionary<string, JToken> ExtensionData
        {
            get
            {
                if (_extensionData == null)
                    return EmptyExtensionData;

                return new ReadOnlyDictionary<string, JToken>(_extensionData);
            }
        }
    }
}
