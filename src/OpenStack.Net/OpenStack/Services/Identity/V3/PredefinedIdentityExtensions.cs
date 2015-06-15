namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;
    using IOAuthExtension = OAuth.IOAuthExtension;
    using OAuthExtension = OAuth.OAuthExtension;

    /// <summary>
    /// This class provides definitions for extensions to the Identity Service V3 which are defined by OpenStack.
    /// </summary>
    /// <remarks>
    /// <para>The service extension definitions provided here may be passed to
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/> to get an instance of the service
    /// extension.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class PredefinedIdentityExtensions
    {
        /// <summary>
        /// Gets a service definition for the OAth extension to the OpenStack Identity Service.
        /// </summary>
        /// <value>
        /// A <see cref="IdentityServiceExtensionDefinition{TExtension}"/> representing the OAuth extension.
        /// </value>
        /// <seealso cref="IOAuthExtension"/>
        /// <seealso cref="OAuthExtension"/>
        public static IdentityServiceExtensionDefinition<IOAuthExtension> OAuth
        {
            get
            {
                return OAuthExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="OAuth"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class OAuthExtensionDefinition : IdentityServiceExtensionDefinition<IOAuthExtension>
        {
            /// <summary>
            /// A singleton instance of the OAuth service extension definition.
            /// </summary>
            public static readonly OAuthExtensionDefinition Instance = new OAuthExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="OAuthExtensionDefinition"/> class.
            /// </summary>
            private OAuthExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "OAuth";
                }
            }

            /// <inheritdoc/>
            public override IOAuthExtension CreateDefaultInstance(IIdentityService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new OAuthExtension(service, httpApiCallFactory);
            }
        }
    }
}
