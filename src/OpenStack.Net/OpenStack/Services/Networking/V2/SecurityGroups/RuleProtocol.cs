namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the protocol of a <see cref="SecurityGroupRule"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known protocols,
    /// with added support for unknown protocols supported by a server extension.
    /// </remarks>
    /// <seealso cref="SecurityGroupRuleData.Protocol"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(RuleProtocol.Converter))]
    public sealed class RuleProtocol : ExtensibleEnum<RuleProtocol>
    {
        private static readonly ConcurrentDictionary<string, RuleProtocol> _types =
            new ConcurrentDictionary<string, RuleProtocol>(StringComparer.OrdinalIgnoreCase);
        private static readonly RuleProtocol _tcp = FromName("tcp");
        private static readonly RuleProtocol _udp = FromName("udp");
        private static readonly RuleProtocol _icmp = FromName("icmp");

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProtocol"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private RuleProtocol(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="RuleProtocol"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="RuleProtocol"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static RuleProtocol FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new RuleProtocol(i));
        }

        /// <summary>
        /// Gets an <see cref="RuleProtocol"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static RuleProtocol Tcp
        {
            get
            {
                return _tcp;
            }
        }

        /// <summary>
        /// Gets an <see cref="RuleProtocol"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static RuleProtocol Udp
        {
            get
            {
                return _udp;
            }
        }

        /// <summary>
        /// Gets an <see cref="RuleProtocol"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static RuleProtocol Icmp
        {
            get
            {
                return _icmp;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="RuleProtocol"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override RuleProtocol FromName(string name)
            {
                return RuleProtocol.FromName(name);
            }
        }
    }
}
