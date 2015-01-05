namespace OpenStack.Services.Identity.V2
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of the credentials included in the <c>auth</c> property of an
    /// authentication request to the OpenStack Identity Service V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticationData : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="TenantName"/> property.
        /// </summary>
        [JsonProperty("tenantName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _tenantName;

        /// <summary>
        /// This is the backing field for the <see cref="TenantId"/> property.
        /// </summary>
        [JsonProperty("tenantId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _tenantId;

        /// <summary>
        /// This is the backing field for the <see cref="PasswordCredentials"/> property.
        /// </summary>
        [JsonProperty("passwordCredentials", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PasswordCredentials _passwordCredentials;

        /// <summary>
        /// This is the backing field for the <see cref="Token"/> property.
        /// </summary>
        [JsonProperty("token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Token _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AuthenticationData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class
        /// with the specified extension data.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used when authenticating with vendor-specific credentials that do not
        /// use any of the properties which are present in the typical OpenStack authentication scenarios.</para>
        /// </remarks>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="extensionData"/> contains any <see langword="null"/> values.</exception>
        public AuthenticationData(params JProperty[] extensionData)
            : this(default(string), default(ProjectId), default(PasswordCredentials), default(Token), extensionData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class
        /// with the specified extension data.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used when authenticating with vendor-specific credentials that do not
        /// use any of the properties which are present in the typical OpenStack authentication scenarios.</para>
        /// </remarks>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        public AuthenticationData(IDictionary<string, JToken> extensionData)
            : this(default(string), default(ProjectId), default(PasswordCredentials), default(Token), extensionData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified credentials.
        /// </summary>
        /// <param name="passwordCredentials">The credentials to use for authentication.</param>
        public AuthenticationData(PasswordCredentials passwordCredentials)
            : this(default(string), default(ProjectId), passwordCredentials, default(Token))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified token.
        /// </summary>
        /// <param name="token">The <see cref="V2.Token"/> to use for authentication.</param>
        public AuthenticationData(Token token)
            : this(default(string), default(ProjectId), default(PasswordCredentials), token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified tenant name and
        /// credentials.
        /// </summary>
        /// <param name="tenantName">The tenant name.</param>
        /// <param name="passwordCredentials">The credentials to use for authentication.</param>
        public AuthenticationData(string tenantName, PasswordCredentials passwordCredentials)
            : this(tenantName, default(ProjectId), passwordCredentials, default(Token))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified tenant name and
        /// token.
        /// </summary>
        /// <param name="tenantName">The tenant name.</param>
        /// <param name="token">The <see cref="V2.Token"/> to use for authentication.</param>
        public AuthenticationData(string tenantName, Token token)
            : this(tenantName, default(ProjectId), default(PasswordCredentials), token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified tenant ID and
        /// credentials.
        /// </summary>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="passwordCredentials">The credentials to use for authentication.</param>
        public AuthenticationData(ProjectId tenantId, PasswordCredentials passwordCredentials)
            : this(default(string), tenantId, passwordCredentials, default(Token))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified tenant ID and
        /// token.
        /// </summary>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="token">The <see cref="V2.Token"/> to use for authentication.</param>
        public AuthenticationData(ProjectId tenantId, Token token)
            : this(default(string), tenantId, default(PasswordCredentials), token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified values and
        /// extension data.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used when authenticating with vendor-specific credentials that require
        /// additional properties which are not present in the typical OpenStack authentication scenarios.</para>
        /// </remarks>
        /// <param name="tenantName">The tenant name.</param>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="passwordCredentials">The credentials to use for authentication.</param>
        /// <param name="token">The <see cref="V2.Token"/> to use for authentication.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="extensionData"/> contains any <see langword="null"/> values.</exception>
        public AuthenticationData(string tenantName, ProjectId tenantId, PasswordCredentials passwordCredentials, Token token, params JProperty[] extensionData)
            : base(extensionData)
        {
            _tenantName = tenantName;
            _tenantId = tenantId;
            _passwordCredentials = passwordCredentials;
            _token = token;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationData"/> class with the specified values and
        /// extension data.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used when authenticating with vendor-specific credentials that require
        /// additional properties which are not present in the typical OpenStack authentication scenarios.</para>
        /// </remarks>
        /// <param name="tenantName">The tenant name.</param>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="passwordCredentials">The credentials to use for authentication.</param>
        /// <param name="token">The <see cref="V2.Token"/> to use for authentication.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        public AuthenticationData(string tenantName, ProjectId tenantId, PasswordCredentials passwordCredentials, Token token, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _tenantName = tenantName;
            _tenantId = tenantId;
            _passwordCredentials = passwordCredentials;
            _token = token;
        }

        /// <summary>
        /// Gets the tenant name included in the authentication data.
        /// </summary>
        /// <value>
        /// <para>The tenant name included in the authentication data.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string TenantName
        {
            get
            {
                return _tenantName;
            }
        }

        /// <summary>
        /// Gets the tenant ID included in the authentication data.
        /// </summary>
        /// <value>
        /// <para>The tenant ID included in the authentication data.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ProjectId TenantId
        {
            get
            {
                return _tenantId;
            }
        }

        /// <summary>
        /// Gets the password credentials included in the authentication data.
        /// </summary>
        /// <value>
        /// <para>The password credentials included in the authentication data.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public PasswordCredentials PasswordCredentials
        {
            get
            {
                return _passwordCredentials;
            }
        }

        /// <summary>
        /// Gets the access token included in the authentication data.
        /// </summary>
        /// <value>
        /// <para>The access token included in the authentication data.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Token Token
        {
            get
            {
                return _token;
            }
        }
    }
}
