namespace OpenStack.Services.Identity.V2
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of the request used for authenticating with the OpenStack Identity
    /// Service V2.
    /// </summary>
    /// <remarks>
    /// <para>The representation used for authentication credentials frequently varies among vendors. When connecting to
    /// a vendor which uses a non-standard representation for the credentials, use the
    /// <see cref="AuthenticationRequest(AuthenticationData, JProperty[])"/> or
    /// <see cref="AuthenticationRequest(AuthenticationData, ImmutableDictionary{string, JToken})"/> constructor to manually
    /// specify the complete set of properties for the JSON representation of the required credentials.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticationRequest : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="AuthenticationData"/> property.
        /// </summary>
        [JsonProperty("auth", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AuthenticationData _auth;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AuthenticationRequest()
        {
        }

        /// <inheritdoc cref="AuthenticationRequest(AuthenticationData, ImmutableDictionary{string, JToken})"/>
        /// <exception cref="ArgumentException">
        /// If <paramref name="extensionData"/> contains any <see langword="null"/> values.
        /// </exception>
        public AuthenticationRequest(AuthenticationData auth, params JProperty[] extensionData)
            : base(extensionData)
        {
            _auth = auth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationRequest"/> class
        /// with the specified extension data.
        /// </summary>
        /// <param name="auth">An <see cref="AuthenticationData"/> which specifies the value for the <c>auth</c>
        /// property in the JSON representation of an authentication request in the OpenStack Identity Service
        /// V2.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <remarks>
        /// <para>
        /// The default authentication request places credentials within an <c>auth</c> property in the JSON
        /// representation. The following block shows an example representation.
        /// </para>
        /// <code language="none">
        /// {
        ///   "auth" : {AuthenticationData...},
        ///   extensionData...
        /// }
        /// </code>
        /// <para>
        /// To specify credentials for a vendor which uses a non-standard form of the request which does not include the
        /// authentication credentials within an <c>auth</c> property in the JSON representation of the request, pass
        /// <see langword="null"/> for the <paramref name="auth"/> argument and use the <paramref name="extensionData"/>
        /// argument to specify the complete set of properties for the authentication request.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="extensionData"/> is <see langword="null"/>.
        /// </exception>
        public AuthenticationRequest(AuthenticationData auth, ImmutableDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _auth = auth;
        }

        /// <summary>
        /// Gets the authentication data for the <c>auth</c> property in the JSON body of the authentication request.
        /// </summary>
        /// <value>
        /// <para>The authentication data for the <c>auth</c> property in the JSON body of the authentication
        /// request.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public AuthenticationData AuthenticationData
        {
            get
            {
                return _auth;
            }
        }
    }
}
