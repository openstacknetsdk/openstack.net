namespace net.openstack.Providers.Rackspace.Objects
{
    /// <summary>
    /// Represents a particular Rackspace entity where a user's account is located.
    /// </summary>
    public enum CloudInstance
    {
        /// <summary>
        /// The Rackspace cloud for US-based accounts.
        /// </summary>
        US,

        /// <summary>
        /// The Rackspace cloud for UK-based accounts.
        /// </summary>
        UK,

        /// <summary>
        /// The default Rackspace cloud, which is currently equal to <see cref="US"/>.
        /// </summary>
        Default = US,
    }
}
