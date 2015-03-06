namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="Member"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="Member.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(MemberStatus.Converter))]
    public sealed class MemberStatus : ExtensibleEnum<MemberStatus>
    {
        private static readonly ConcurrentDictionary<string, MemberStatus> _types =
            new ConcurrentDictionary<string, MemberStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly MemberStatus _active = FromName("ACTIVE");

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private MemberStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="MemberStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="MemberStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static MemberStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new MemberStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="MemberStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static MemberStatus Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="MemberStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override MemberStatus FromName(string name)
            {
                return MemberStatus.FromName(name);
            }
        }
    }
}
