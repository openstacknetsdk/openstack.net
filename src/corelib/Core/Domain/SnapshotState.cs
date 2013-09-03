namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents the state of a block storage snapshot.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known snapshot states,
    /// with added support for unknown states returned by a server extension.
    /// </remarks>
    public sealed class SnapshotState : IEquatable<SnapshotState>
    {
        private static readonly ConcurrentDictionary<string, SnapshotState> _states =
            new ConcurrentDictionary<string, SnapshotState>(StringComparer.OrdinalIgnoreCase);
        private static readonly SnapshotState _creating = FromName("CREATING");
        private static readonly SnapshotState _available = FromName("AVAILABLE");
        private static readonly SnapshotState _attaching = FromName("ATTACHING");
        private static readonly SnapshotState _inUse = FromName("IN-USE");
        private static readonly SnapshotState _deleting = FromName("DELETING");
        private static readonly SnapshotState _error = FromName("ERROR");
        private static readonly SnapshotState _errorDeleting = FromName("ERROR_DELETING");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotState"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private SnapshotState(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="SnapshotState"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static SnapshotState FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _states.GetOrAdd(name, i => new SnapshotState(i));
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating the snapshot is being created.
        /// </summary>
        public static SnapshotState Creating
        {
            get
            {
                return _creating;
            }
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating the snapshot is ready to be attached to an instance.
        /// </summary>
        public static SnapshotState Available
        {
            get
            {
                return _available;
            }
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating the snapshot is attaching to an instance.
        /// </summary>
        public static SnapshotState Attaching
        {
            get
            {
                return _attaching;
            }
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating the snapshot is attached to an instance.
        /// </summary>
        public static SnapshotState InUse
        {
            get
            {
                return _inUse;
            }
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating the snapshot is being deleted.
        /// </summary>
        public static SnapshotState Deleting
        {
            get
            {
                return _deleting;
            }
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating there has been some error with the snapshot.
        /// </summary>
        public static SnapshotState Error
        {
            get
            {
                return _error;
            }
        }

        /// <summary>
        /// Gets a <see cref="SnapshotState"/> indicating an error occurred while deleting the snapshot.
        /// </summary>
        public static SnapshotState ErrorDeleting
        {
            get
            {
                return _errorDeleting;
            }
        }

        /// <summary>
        /// Gets the canonical name of this snapshot state.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(SnapshotState other)
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
