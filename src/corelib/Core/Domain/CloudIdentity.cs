namespace net.openstack.Core.Domain
{
    /// <summary>
    /// Represents a set of credentials used for accessing a cloud account. Individual providers
    /// may impose restrictions on the values allowed for individual properties.
    /// </summary>
    public class CloudIdentity
    {
        /// <summary>
        /// Gets or sets the username for this identity.
        /// </summary>
        /// <value>
        /// The username for this identity. The value may be <c>null</c> if the particular
        /// provider supports authenticating without a username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for this identity.
        /// </summary>
        /// <remarks>
        /// The <see cref="CloudIdentity"/> class represents <em>credentials</em> (as opposed
        /// to an <em>account</em>), so any changes made to this property value will not be
        /// reflected in the account.
        /// </remarks>
        /// <value>
        /// A password to use when authenticating this identity, or <c>null</c> if authentication
        /// should be performed by different method (e.g. with a <see cref="APIKey"/>).
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the API key for this identity.
        /// </summary>
        /// <remarks>
        /// The <see cref="CloudIdentity"/> class represents <em>credentials</em> (as opposed
        /// to an <em>account</em>), so any changes made to this property value will not be
        /// reflected in the account.
        /// </remarks>
        /// <value>
        /// An API key to use when authenticating this identity, or <c>null</c> if authentication
        /// should be performed by different method (e.g. with a <see cref="Password"/>).
        /// </value>
        public string APIKey { get; set; }

        /// <summary>
        /// Gets or sets the project (tenant) name for this identity.
        /// </summary>
        /// <remarks>
        /// The <see cref="CloudIdentity"/> class represents <em>credentials</em> (as opposed
        /// to an <em>account</em>), so any changes made to this property value will not be
        /// reflected in the account.
        /// </remarks>
        /// <value>
        /// The name of the project for this identity. The value may be <c>null</c> if the particular
        /// provider supports authenticating without a project.
        /// </value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the default region to use.
        /// </summary>
        /// <remarks>
        /// The <see cref="CloudIdentity"/> class represents <em>credentials</em> (as opposed
        /// to an <em>account</em>), so any changes made to this property value will not be
        /// reflected in the account.
        /// </remarks>
        /// <value>
        /// The name of the region to use by this identity. The value may be <c>null</c> if no
        /// default region shall be set from here.
        /// </value>
        public string DefaultRegion { get; set; }
    }
}
