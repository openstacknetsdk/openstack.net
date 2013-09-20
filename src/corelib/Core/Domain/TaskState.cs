﻿namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core.Domain.Converters;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the task status of a server.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known task states,
    /// with added support for unknown states returned by a server extension.
    ///
    /// <note>
    /// This property is defined by the Rackspace-specific Extended Status Extension
    /// to the OpenStack Compute API. The API does not regulate the status values,
    /// so it is possible that values can be added, removed, or renamed.
    /// </note>
    /// </remarks>
    /// <seealso cref="Server.TaskState"/>
    /// <seealso href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_extensions.html#task_state">OS-EXT-STS:task_state (Rackspace Next Generation Cloud Servers Developer Guide - API v2)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(TaskState.Converter))]
    public sealed class TaskState : IEquatable<TaskState>
    {
        private static readonly ConcurrentDictionary<string, TaskState> _states =
            new ConcurrentDictionary<string, TaskState>(StringComparer.OrdinalIgnoreCase);
        private static readonly TaskState _blockDeviceMapping = FromName("block_device_mapping");
        private static readonly TaskState _deleting = FromName("deleting");
        private static readonly TaskState _imageSnapshot = FromName("image_snapshot");
        private static readonly TaskState _imageBackup = FromName("image_backup");
        private static readonly TaskState _migrating = FromName("migrating");
        private static readonly TaskState _networking = FromName("networking");
        private static readonly TaskState _pausing = FromName("pausing");
        private static readonly TaskState _poweringOff = FromName("powering_off");
        private static readonly TaskState _poweringOn = FromName("powering_on");
        private static readonly TaskState _rebooting = FromName("rebooting");
        private static readonly TaskState _rebootingHard = FromName("rebooting_hard");
        private static readonly TaskState _rebuilding = FromName("rebuilding");
        private static readonly TaskState _rebuildBlockDeviceMapping = FromName("rebuild_block_device_mapping");
        private static readonly TaskState _rebuildSpawning = FromName("rebuild_spawning");
        private static readonly TaskState _rescuing = FromName("rescuing");
        private static readonly TaskState _resizeConfirming = FromName("resize_confirming");
        private static readonly TaskState _resizeFinish = FromName("resize_finish");
        private static readonly TaskState _resizeMigrated = FromName("resize_migrated");
        private static readonly TaskState _resizeMigrating = FromName("resize_migrating");
        private static readonly TaskState _resizePrep = FromName("resize_prep");
        private static readonly TaskState _resizeReverting = FromName("resize_reverting");
        private static readonly TaskState _resuming = FromName("resuming");
        private static readonly TaskState _scheduling = FromName("scheduling");
        private static readonly TaskState _spawning = FromName("spawning");
        private static readonly TaskState _starting = FromName("starting");
        private static readonly TaskState _stopping = FromName("stopping");
        private static readonly TaskState _suspending = FromName("suspending");
        private static readonly TaskState _unpausing = FromName("unpausing");
        private static readonly TaskState _unrescuing = FromName("unrescuing");
        private static readonly TaskState _updatingPassword = FromName("updating_password");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskState"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private TaskState(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="TaskState"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static TaskState FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _states.GetOrAdd(name, i => new TaskState(i));
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState BlockDeviceMapping
        {
            get
            {
                return _blockDeviceMapping;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Deleting
        {
            get
            {
                return _deleting;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ImageSnapshot
        {
            get
            {
                return _imageSnapshot;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ImageBackup
        {
            get
            {
                return _imageBackup;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Migrating
        {
            get
            {
                return _migrating;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Networking
        {
            get
            {
                return _networking;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Pausing
        {
            get
            {
                return _pausing;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState PoweringOff
        {
            get
            {
                return _poweringOff;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState PoweringOn
        {
            get
            {
                return _poweringOn;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Rebooting
        {
            get
            {
                return _rebooting;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState RebootingHard
        {
            get
            {
                return _rebootingHard;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Rebuilding
        {
            get
            {
                return _rebuilding;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState RebuildBlockDeviceMapping
        {
            get
            {
                return _rebuildBlockDeviceMapping;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState RebuildSpawning
        {
            get
            {
                return _rebuildSpawning;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Rescuing
        {
            get
            {
                return _rescuing;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ResizeConfirming
        {
            get
            {
                return _resizeConfirming;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ResizeFinish
        {
            get
            {
                return _resizeFinish;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ResizeMigrated
        {
            get
            {
                return _resizeMigrated;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ResizeMigrating
        {
            get
            {
                return _resizeMigrating;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ResizePrep
        {
            get
            {
                return _resizePrep;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState ResizeReverting
        {
            get
            {
                return _resizeReverting;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Resuming
        {
            get
            {
                return _resuming;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Scheduling
        {
            get
            {
                return _scheduling;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Spawning
        {
            get
            {
                return _spawning;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Starting
        {
            get
            {
                return _starting;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Stopping
        {
            get
            {
                return _stopping;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Suspending
        {
            get
            {
                return _suspending;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Unpausing
        {
            get
            {
                return _unpausing;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState Unrescuing
        {
            get
            {
                return _unrescuing;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskState"/> instance representing <placeholder>description</placeholder>.
        /// </summary>
        public static TaskState UpdatingPassword
        {
            get
            {
                return _updatingPassword;
            }
        }

        /// <summary>
        /// Gets the canonical name of this task state.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(TaskState other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="TaskState"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<TaskState>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(TaskState obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override TaskState ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
