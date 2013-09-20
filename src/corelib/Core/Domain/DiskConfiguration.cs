namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core.Domain.Converters;
    using Newtonsoft.Json;

    /// <summary>
    /// This enumeration is part of the <see href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_extensions.html#diskconfig_attribute"><newTerm>disk configuration extension</newTerm></see>,
    /// which adds at attribute to images and servers to control how the disk is partitioned when
    /// servers are created, rebuilt, or resized.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known disk configurations,
    /// with added support for unknown types returned by a server extension.
    /// </remarks>
    /// <seealso href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_extensions.html#diskconfig_attribute">Disk Configuration Extension (Rackspace Next Generation Cloud Servers Developer Guide - API v2)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(DiskConfiguration.Converter))]
    public sealed class DiskConfiguration : IEquatable<DiskConfiguration>
    {
        private static readonly ConcurrentDictionary<string, DiskConfiguration> _types =
            new ConcurrentDictionary<string, DiskConfiguration>(StringComparer.OrdinalIgnoreCase);
        private static readonly DiskConfiguration _auto = FromName("AUTO");
        private static readonly DiskConfiguration _manual = FromName("MANUAL");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskConfiguration"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private DiskConfiguration(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="DiskConfiguration"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static DiskConfiguration FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new DiskConfiguration(i));
        }

        /// <summary>
        /// Gets a <see cref="DiskConfiguration"/> representing automatic configuration.
        /// </summary>
        /// <remarks>
        /// The server is built with a single partition the size of the target flavor disk. The
        /// file system is automatically adjusted to fit the entire partition. This keeps things
        /// simple and automated. <see cref="Auto"/> is valid only for images and servers with a
        /// single partition that use the EXT3 file system. This is the default setting for
        /// applicable Rackspace base images.
        /// </remarks>
        public static DiskConfiguration Auto
        {
            get
            {
                return _auto;
            }
        }

        /// <summary>
        /// Gets a <see cref="DiskConfiguration"/> manual configuration.
        /// </summary>
        /// <remarks>
        /// The server is built using whatever partition scheme and file system is in the source
        /// image. If the target flavor disk is larger, the remaining disk space is left
        /// unpartitioned. This enables images to have non-EXT3 file systems, multiple partitions,
        /// and so on, and enables you to manage the disk configuration.
        /// </remarks>
        public static DiskConfiguration Manual
        {
            get
            {
                return _manual;
            }
        }

        /// <summary>
        /// Gets the canonical name of this disk configuration.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(DiskConfiguration other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="DiskConfiguration"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<DiskConfiguration>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(DiskConfiguration obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override DiskConfiguration ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
