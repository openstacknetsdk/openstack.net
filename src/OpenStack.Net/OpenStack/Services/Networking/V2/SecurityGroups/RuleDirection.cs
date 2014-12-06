namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the direction of a <see cref="SecurityGroupRule"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known directions,
    /// with added support for unknown directions supported by a server extension.
    /// </remarks>
    /// <seealso cref="SecurityGroupRuleData.Direction"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(RuleDirection.Converter))]
    public sealed class RuleDirection : ExtensibleEnum<RuleDirection>
    {
        private static readonly ConcurrentDictionary<string, RuleDirection> _types =
            new ConcurrentDictionary<string, RuleDirection>(StringComparer.OrdinalIgnoreCase);
        private static readonly RuleDirection _ingress = FromName("ingress");
        private static readonly RuleDirection _egress = FromName("egress");

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDirection"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private RuleDirection(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="RuleDirection"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="RuleDirection"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static RuleDirection FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new RuleDirection(i));
        }

        /// <summary>
        /// Gets a <see cref="RuleDirection"/> representing a security group rule that is applied to incoming (ingress) traffic.
        /// </summary>
        public static RuleDirection Ingress
        {
            get
            {
                return _ingress;
            }
        }

        /// <summary>
        /// Gets a <see cref="RuleDirection"/> representing a security group rule that is applied to outgoing (egress) traffic.
        /// </summary>
        public static RuleDirection Egress
        {
            get
            {
                return _ingress;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="RuleDirection"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override RuleDirection FromName(string name)
            {
                return RuleDirection.FromName(name);
            }
        }
    }
}
