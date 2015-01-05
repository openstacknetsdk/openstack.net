namespace OpenStack.Services.Identity.V2
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;
    using OpenStack.Security.Authentication;

    /// <summary>
    /// This class models the JSON representation of an authentication token provided by the OpenStack Identity Service
    /// V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Token : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private TokenId _id;

#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="IssuedAt"/> property.
        /// </summary>
        [JsonProperty("issued_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _issuedAt;

        /// <summary>
        /// This is the backing field for the <see cref="ExpiresAt"/> property.
        /// </summary>
        [JsonProperty("expires", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _expires;

        /// <summary>
        /// This is the backing field for the <see cref="Tenant"/> property.
        /// </summary>
        [JsonProperty("tenant", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Tenant _tenant;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Token()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified ID.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used for authentication requests which specified a <see cref="Token"/>
        /// as the credentials.</para>
        /// </remarks>
        /// <param name="id">The unique ID of the token.</param>
        public Token(TokenId id)
        {
            _id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified ID and extension data.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used for authentication requests which specified a <see cref="Token"/>
        /// as the credentials.</para>
        /// </remarks>
        /// <param name="id">The unique ID of the token.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="extensionData"/> contains any <see langword="null"/> values.</exception>
        public Token(TokenId id, params JProperty[] extensionData)
            : base(extensionData)
        {
            _id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified ID and extension data.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is typically used for authentication requests which specified a <see cref="Token"/>
        /// as the credentials.</para>
        /// </remarks>
        /// <param name="id">The unique ID of the token.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        public Token(TokenId id, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _id = id;
        }

        /// <summary>
        /// Gets the unique ID of the token.
        /// </summary>
        /// <remarks>
        /// <para>This value is used by <see cref="IAuthenticationService"/> implementation for the OpenStack Identity
        /// Service V2 for authenticating HTTP API calls to the service.</para>
        /// </remarks>
        /// <value>
        /// <para>The unique ID of the authentication token.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public TokenId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets a timestamp indicating when the current authentication token was originally issued.
        /// </summary>
        /// <value>
        /// <para>A timestamp indicating when the current authentication token was originally issued.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public DateTimeOffset? IssuedAt
        {
            get
            {
                return _issuedAt;
            }
        }

        /// <summary>
        /// Gets a timestamp indicating when the current authentication token is set to expire.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>Authentication tokens may be invalidated at any time. Client applications should be prepared for calls
        /// to return an error due to an authentication token expiring, and retry the call if necessary.
        /// <placeholder>TODO: Explain the process for forcing an <see cref="IAuthenticationService"/> instance to
        /// invalidate a cached authentication token.</placeholder>
        /// </para>
        /// </note>
        /// </remarks>
        /// <value>
        /// <para>A timestamp indicating when the current authentication token is set to expire.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public DateTimeOffset? ExpiresAt
        {
            get
            {
                return _expires;
            }
        }

        /// <summary>
        /// Gets the tenant associated with the current authentication token.
        /// </summary>
        /// <value>
        /// <para>The tenant associated with the current authentication token.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Tenant Tenant
        {
            get
            {
                return _tenant;
            }
        }
    }
}
