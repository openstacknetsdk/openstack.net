namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="Service"/> resource in the Content Delivery Service.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses, with added support for unknown statuses
    /// supported by a server extension.
    /// </remarks>
    /// <seealso cref="Service.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ServiceStatus.Converter))]
    public sealed class ServiceStatus : ExtensibleEnum<ServiceStatus>
    {
        private static readonly ConcurrentDictionary<string, ServiceStatus> _values =
            new ConcurrentDictionary<string, ServiceStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly ServiceStatus _createInProgress = FromName("create_in_progress");
        private static readonly ServiceStatus _deployed = FromName("deployed");
        private static readonly ServiceStatus _updateInProgress = FromName("update_in_progress");
        private static readonly ServiceStatus _deleteInProgress = FromName("delete_in_progress");
        private static readonly ServiceStatus _failed = FromName("failed");

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private ServiceStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="ServiceStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="ServiceStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ServiceStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _values.GetOrAdd(name, i => new ServiceStatus(i));
        }

        /// <summary>
        /// Gets a <see cref="ServiceStatus"/> representing a service which is currently being created or deployed.
        /// </summary>
        /// <value>
        /// A <see cref="ServiceStatus"/> representing a service which is currently being created or deployed.
        /// </value>
        public static ServiceStatus CreateInProgress
        {
            get
            {
                return _createInProgress;
            }
        }

        /// <summary>
        /// Gets a <see cref="ServiceStatus"/> representing a service which has been deployed and is ready to use.
        /// </summary>
        /// <value>
        /// A <see cref="ServiceStatus"/> representing a service which has been deployed and is ready to use.
        /// </value>
        public static ServiceStatus Deployed
        {
            get
            {
                return _deployed;
            }
        }

        /// <summary>
        /// Gets a <see cref="ServiceStatus"/> representing a service which is currently being updated.
        /// </summary>
        /// <value>
        /// A <see cref="ServiceStatus"/> representing a service which is currently being updated.
        /// </value>
        public static ServiceStatus UpdateInProgress
        {
            get
            {
                return _updateInProgress;
            }
        }

        /// <summary>
        /// Gets a <see cref="ServiceStatus"/> representing a service which is currently being deleted.
        /// </summary>
        /// <value>
        /// A <see cref="ServiceStatus"/> representing a service which is currently being deleted.
        /// </value>
        public static ServiceStatus DeleteInProgress
        {
            get
            {
                return _deleteInProgress;
            }
        }

        /// <summary>
        /// Gets a <see cref="ServiceStatus"/> representing a service which failed to complete the previous operation.
        /// </summary>
        /// <remarks>
        /// <para>Detailed information about errors is available in the <see cref="Service.Errors"/> property.</para>
        /// </remarks>
        /// <value>
        /// A <see cref="ServiceStatus"/> representing a service which failed to complete the previous operation.
        /// </value>
        public static ServiceStatus Failed
        {
            get
            {
                return _failed;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ServiceStatus"/> objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ServiceStatus FromName(string name)
            {
                return ServiceStatus.FromName(name);
            }
        }
    }
}
