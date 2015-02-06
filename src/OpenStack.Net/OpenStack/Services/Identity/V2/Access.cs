namespace OpenStack.Services.Identity.V2
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of the access information returned by an HTTP API authentication
    /// request to the OpenStack Identity Service V2.
    /// </summary>
    /// <seealso cref="IIdentityService.PrepareAuthenticateAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Access : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Token"/> property.
        /// </summary>
        [JsonProperty("token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Token _token;

        /// <summary>
        /// This is the backing field for the <see cref="ServiceCatalog"/> property.
        /// </summary>
        [JsonProperty("serviceCatalog", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceCatalogEntry> _serviceCatalog;

        /// <summary>
        /// This is the backing field for the <see cref="User"/> property.
        /// </summary>
        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private User _user;

        /// <summary>
        /// This is the backing field for the <see cref="Metadata"/> property.
        /// </summary>
        [JsonProperty("metadata", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, JToken> _metadata;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Access"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Access()
        {
        }

        /// <summary>
        /// Gets the authentication token used for authenticating HTTP API calls to services described in the
        /// <see cref="ServiceCatalog"/>.
        /// </summary>
        /// <value>
        /// <para>The authentication token used for authenticating HTTP API calls to services described in the
        /// <see cref="ServiceCatalog"/>.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Token Token
        {
            get
            {
                return _token;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ServiceCatalogEntry"/> objects describing the services, regions, and
        /// endpoints available to an authenticated user.
        /// </summary>
        /// <value>
        /// <para>A collection of <see cref="ServiceCatalogEntry"/> objects describing the services, regions, and
        /// endpoints available to an authenticated user.</para>
        /// <token>DefaultArrayIfNotIncluded</token>
        /// </value>
        public ImmutableArray<ServiceCatalogEntry> ServiceCatalog
        {
            get
            {
                return _serviceCatalog;
            }
        }

        /// <summary>
        /// Gets a <see cref="V2.User"/> object containing information about the authenticated user.
        /// </summary>
        /// <value>
        /// <para>A <see cref="V2.User"/> object containing information about the authenticated user.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public User User
        {
            get
            {
                return _user;
            }
        }

        /// <summary>
        /// Gets a collection of additional vendor-specific metadata related to the authentication information.
        /// </summary>
        /// <value>
        /// <para>A collection of additional vendor-specific metadata related to the authentication information.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ImmutableDictionary<string, JToken> Metadata
        {
            get
            {
                return _metadata;
            }
        }
    }
}
