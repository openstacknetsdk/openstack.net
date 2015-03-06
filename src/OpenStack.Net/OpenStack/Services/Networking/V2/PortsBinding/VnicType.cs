namespace OpenStack.Services.Networking.V2.PortsBinding
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents a vnic type.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known vnic types,
    /// with added support for unknown types supported by a server extension.
    /// </remarks>
    /// <seealso cref="PortsBindingExtensions.GetBindingVnicType"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(VnicType.Converter))]
    public sealed class VnicType : ExtensibleEnum<VnicType>
    {
        private static readonly ConcurrentDictionary<string, VnicType> _types =
            new ConcurrentDictionary<string, VnicType>(StringComparer.OrdinalIgnoreCase);
        private static readonly VnicType _normal = FromName("normal");
        private static readonly VnicType _direct = FromName("direct");
        private static readonly VnicType _macvtap = FromName("macvtap");

        /// <summary>
        /// Initializes a new instance of the <see cref="VnicType"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private VnicType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="VnicType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="VnicType"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static VnicType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new VnicType(i));
        }

        /// <summary>
        /// Gets an <see cref="VnicType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static VnicType Normal
        {
            get
            {
                return _normal;
            }
        }

        /// <summary>
        /// Gets an <see cref="VnicType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static VnicType Direct
        {
            get
            {
                return _direct;
            }
        }

        /// <summary>
        /// Gets an <see cref="VnicType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static VnicType MacVTap
        {
            get
            {
                return _macvtap;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="VnicType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override VnicType FromName(string name)
            {
                return VnicType.FromName(name);
            }
        }
    }
}
