namespace net.openstack.Providers.Rackspace.Objects.Databases
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the status of a database backup.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses returned by a server extension.
    /// </remarks>
    /// <seealso cref="Backup.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(BackupStatus.Converter))]
    public sealed class BackupStatus : ExtensibleEnum<BackupStatus>
    {
        private static readonly ConcurrentDictionary<string, BackupStatus> _types =
            new ConcurrentDictionary<string, BackupStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly BackupStatus _new = FromName("NEW");
        private static readonly BackupStatus _completed = FromName("COMPLETED");

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private BackupStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="BackupStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static BackupStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new BackupStatus(i));
        }

        /// <summary>
        /// Gets a <see cref="BackupStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static BackupStatus New
        {
            get
            {
                return _new;
            }
        }

        /// <summary>
        /// Gets a <see cref="BackupStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static BackupStatus Completed
        {
            get
            {
                return _completed;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="BackupStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override BackupStatus FromName(string name)
            {
                return BackupStatus.FromName(name);
            }
        }
    }
}
