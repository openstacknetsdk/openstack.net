namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents an image type.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known image types,
    /// with added support for unknown types returned by a server extension.
    /// </remarks>
    public sealed class ImageType : IEquatable<ImageType>
    {
        private static readonly ConcurrentDictionary<string, ImageType> _types =
            new ConcurrentDictionary<string, ImageType>(StringComparer.OrdinalIgnoreCase);
        private static readonly ImageType _base = FromName("BASE");
        private static readonly ImageType _snapshot = FromName("SNAPSHOT");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageType"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private ImageType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="ImageType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ImageType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new ImageType(i));
        }

        /// <summary>
        /// Gets an <see cref="ImageType"/> representing a base image.
        /// </summary>
        public static ImageType Base
        {
            get
            {
                return _base;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageType"/> representing an image created as a snapshot.
        /// </summary>
        public static ImageType Snapshot
        {
            get
            {
                return _snapshot;
            }
        }

        /// <summary>
        /// Gets the canonical name of this image type.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(ImageType other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
