using System;
using System.Collections.Concurrent;
using net.openstack.Core.Domain.Converters;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    /// <summary>
    /// Represents the state of a Load Balancer.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known load balancer states,
    /// with added support for unknown states returned by a load balancer extension.
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof (LoadBalancerState.Converter))]
    public sealed class LoadBalancerState : IEquatable<LoadBalancerState>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerState> _states =
            new ConcurrentDictionary<string, LoadBalancerState>(StringComparer.OrdinalIgnoreCase);

        private static readonly LoadBalancerState _active = FromName("ACTIVE");
        private static readonly LoadBalancerState _build = FromName("BUILD");
        private static readonly LoadBalancerState _deleted = FromName("DELETED");
        private static readonly LoadBalancerState _pendingDelete = FromName("PENDING_DELETE");
        private static readonly LoadBalancerState _pendingUpdate = FromName("PENDING_UPDATE");
        private static readonly LoadBalancerState _error = FromName("ERROR");
        private static readonly LoadBalancerState _unknown = FromName("UNKNOWN");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerState"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private LoadBalancerState(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerState"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerState FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _states.GetOrAdd(name, i => new LoadBalancerState(i));
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> representing a load balancer which is active and ready to use.
        /// </summary>
        public static LoadBalancerState Active
        {
            get { return _active; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> representing a load balancer currently building.
        /// </summary>
        public static LoadBalancerState Build
        {
            get { return _build; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> representing a load balancer which has been deleted.
        /// </summary>
        public static LoadBalancerState Deleted
        {
            get { return _deleted; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> representing a load balancer which has been marked for deletion.
        /// </summary>
        public static LoadBalancerState PendingDelete
        {
            get { return _pendingDelete; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> representing a load balancer which is pending an update.
        /// </summary>
        public static LoadBalancerState PendingUpdate
        {
            get { return _pendingUpdate; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> representing a load balancer which failed to perform
        /// an operation and is now in an error state.
        /// </summary>
        public static LoadBalancerState Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerState"/> for a load balancer that is currently in an unknown state.
        /// </summary>
        public static LoadBalancerState Unknown
        {
            get { return _unknown; }
        }

        /// <summary>
        /// Gets the canonical name of this load balancer state.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public bool Equals(LoadBalancerState other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerState"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<LoadBalancerState>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(LoadBalancerState obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override LoadBalancerState ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}