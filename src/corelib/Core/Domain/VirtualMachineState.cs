﻿namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core.Domain.Converters;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the virtual machine (VM) state of a server.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known virtual machine states,
    /// with added support for unknown states returned by a server extension.
    ///
    /// <note>
    /// This property is defined by the Rackspace-specific Extended Status Extension
    /// to the OpenStack Compute API. The API does not regulate the status values,
    /// so it is possible that values can be added, removed, or renamed.
    /// </note>
    /// </remarks>
    /// <seealso cref="Server.VMState"/>
    /// <seealso href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_extensions.html#vm_state">OS-EXT-STS:vm_state (Rackspace Next Generation Cloud Servers Developer Guide - API v2)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(VirtualMachineState.Converter))]
    public sealed class VirtualMachineState : IEquatable<VirtualMachineState>
    {
        private static readonly ConcurrentDictionary<string, VirtualMachineState> _states =
            new ConcurrentDictionary<string, VirtualMachineState>(StringComparer.OrdinalIgnoreCase);
        private static readonly VirtualMachineState _active = FromName("ACTIVE");
        private static readonly VirtualMachineState _build = FromName("BUILD");
        private static readonly VirtualMachineState _deleted = FromName("DELETED");
        private static readonly VirtualMachineState _error = FromName("ERROR");
        private static readonly VirtualMachineState _paused = FromName("PAUSED");
        private static readonly VirtualMachineState _rescued = FromName("RESCUED");
        private static readonly VirtualMachineState _resized = FromName("RESIZED");
        private static readonly VirtualMachineState _softDeleted = FromName("SOFT_DELETED");
        private static readonly VirtualMachineState _stopped = FromName("STOPPED");
        private static readonly VirtualMachineState _suspended = FromName("SUSPENDED");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMachineState"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private VirtualMachineState(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="VirtualMachineState"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static VirtualMachineState FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _states.GetOrAdd(name, i => new VirtualMachineState(i));
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Build
        {
            get
            {
                return _build;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Deleted
        {
            get
            {
                return _deleted;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Error
        {
            get
            {
                return _error;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Paused
        {
            get
            {
                return _paused;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Rescued
        {
            get
            {
                return _rescued;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Resized
        {
            get
            {
                return _resized;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState SoftDeleted
        {
            get
            {
                return _softDeleted;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Stopped
        {
            get
            {
                return _stopped;
            }
        }

        /// <summary>
        /// Gets a <see cref="VirtualMachineState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static VirtualMachineState Suspended
        {
            get
            {
                return _suspended;
            }
        }

        /// <summary>
        /// Gets the canonical name of this virtual machine state.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(VirtualMachineState other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="VirtualMachineState"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<VirtualMachineState>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(VirtualMachineState obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override VirtualMachineState ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
