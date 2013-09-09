using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    /// <summary>
    /// Extends the <see cref="CloudIdentity"/> class by adding support for specifying
    /// a <see cref="CloudInstance"/> and <see cref="Domain"/> for the identity.
    /// </summary>
    public class RackspaceCloudIdentity : CloudIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RackspaceCloudIdentity"/> class
        /// with the default values.
        /// </summary>
        public RackspaceCloudIdentity()
        {
            CloudInstance = CloudInstance.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RackspaceCloudIdentity"/> class
        /// from the given <see cref="CloudIdentity"/> instance and the default
        /// <see cref="CloudInstance"/>.
        /// </summary>
        /// <param name="cloudIdentity">The generic cloud identity.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="cloudIdentity"/> is <c>null</c>.</exception>
        public RackspaceCloudIdentity(CloudIdentity cloudIdentity) : this()
        {
            this.Username = cloudIdentity.Username;
            this.Password = cloudIdentity.Password;
            this.APIKey = cloudIdentity.APIKey;
        }

        /// <summary>
        /// Gets or sets the <see cref="CloudInstance"/> for this account.
        /// </summary>
        /// <remarks>
        /// The <see cref="RackspaceCloudIdentity"/> class represents <em>credentials</em> (as opposed
        /// to an <em>account</em>), so any changes made to this property value will not be
        /// reflected in the account.
        ///
        /// <para>The default value is <see cref="net.openstack.Providers.Rackspace.Objects.CloudInstance.Default"/>.</para>
        /// </remarks>
        public CloudInstance CloudInstance { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Domain"/> for this account.
        /// </summary>
        /// <remarks>
        /// The <see cref="RackspaceCloudIdentity"/> class represents <em>credentials</em> (as opposed
        /// to an <em>account</em>), so any changes made to this property value will not be
        /// reflected in the account.
        /// </remarks>
        public Domain Domain { get; set; }
    }

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
