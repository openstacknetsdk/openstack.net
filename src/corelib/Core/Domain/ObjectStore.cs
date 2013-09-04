namespace net.openstack.Core.Domain
{
    using net.openstack.Core.Providers;

    /// <summary>
    /// Represents the result of various Object Storage operations.
    /// </summary>
    /// <seealso cref="IObjectStorageProvider"/>
    public enum ObjectStore
    {
        /// <summary>
        /// <placeholder>What should we do with this?</placeholder>
        /// </summary>
        Unknown,

        /// <summary>
        /// The container was created.
        /// </summary>
        /// <seealso cref="IObjectStorageProvider.CreateContainer"/>
        ContainerCreated,

        /// <summary>
        /// The container already exists.
        /// </summary>
        /// <seealso cref="IObjectStorageProvider.CreateContainer"/>
        ContainerExists,
    }
}
