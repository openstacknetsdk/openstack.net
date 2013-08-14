namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the response to a user authentication.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_authenticate_v2.0_tokens_.html">Authenticate (OpenStack Identity Service API v2.0 Reference)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    public class UserAccess
    {
        /// <summary>
        /// Gets the <see cref="IdentityToken"/> which allows providers to make authenticated
        /// calls to API methods.
        /// </summary>
        /// <remarks>
        /// The specific manner in which the token is used is provider-specific. Some implementations
        /// pass the token's <see cref="IdentityToken.Id"/> as an HTTP header when requesting a
        /// resource.
        /// </remarks>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_authenticate_v2.0_tokens_.html">Authenticate (OpenStack Identity Service API v2.0 Reference)</seealso>
        [JsonProperty("token")]
        public IdentityToken Token { get; private set; }

        /// <summary>
        /// Gets the details for the authenticated user, such as the username and roles.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_authenticate_v2.0_tokens_.html">Authenticate (OpenStack Identity Service API v2.0 Reference)</seealso>
        [JsonProperty("user")]
        public UserDetails User { get; private set; }

        /// <summary>
        /// Gets the services which may be accessed by this user.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_authenticate_v2.0_tokens_.html">Authenticate (OpenStack Identity Service API v2.0 Reference)</seealso>
        [JsonProperty("serviceCatalog")]
        public ServiceCatalog[] ServiceCatalog { get; private set; }
    }
}