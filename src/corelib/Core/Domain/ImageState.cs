namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core.Domain.Converters;
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the state of a compute image.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known image states,
    /// with added support for unknown states returned by an image extension.
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(ImageState.Converter))]
    public sealed class ImageState : IEquatable<ImageState>
    {
        private static readonly ConcurrentDictionary<string, ImageState> _states =
            new ConcurrentDictionary<string, ImageState>(StringComparer.OrdinalIgnoreCase);
        private static readonly ImageState _active = FromName("ACTIVE");
        private static readonly ImageState _saving = FromName("SAVING");
        private static readonly ImageState _deleted = FromName("DELETED");
        private static readonly ImageState _error = FromName("ERROR");
        private static readonly ImageState _unknown = FromName("UNKNOWN");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageState"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private ImageState(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="ImageState"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ImageState FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _states.GetOrAdd(name, i => new ImageState(i));
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which is active and ready to use.
        /// </summary>
        public static ImageState Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently being saved.
        /// </summary>
        public static ImageState Saving
        {
            get
            {
                return _saving;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which has been deleted.
        /// </summary>
        /// <remarks>
        /// By default, the <see cref="IComputeProvider.ListImages"/> operation does not return
        /// images which have been deleted. To list deleted images, call
        /// <see cref="IComputeProvider.ListImages"/> specifying the <c>changesSince</c>
        /// parameter.
        /// </remarks>
        public static ImageState Deleted
        {
            get
            {
                return _deleted;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which failed to perform
        /// an operation and is now in an error state.
        /// </summary>
        public static ImageState Error
        {
            get
            {
                return _error;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> for an image that is currently in an unknown state.
        /// </summary>
        public static ImageState Unknown
        {
            get
            {
                return _unknown;
            }
        }

        /// <summary>
        /// Gets the canonical name of this image state.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(ImageState other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ImageState"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<ImageState>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(ImageState obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override ImageState ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
