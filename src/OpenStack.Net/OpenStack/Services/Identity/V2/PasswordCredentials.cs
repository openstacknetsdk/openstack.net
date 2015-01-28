namespace OpenStack.Services.Identity.V2
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of the credentials used for username/password authentication with the
    /// OpenStack Identity Service V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class PasswordCredentials : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Username"/> property.
        /// </summary>
        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _username;

        /// <summary>
        /// This is the backing field for the <see cref="Password"/> property.
        /// </summary>
        [JsonProperty("password", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCredentials"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PasswordCredentials()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCredentials"/> class with the specified credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public PasswordCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCredentials"/> class with the specified credentials and
        /// extension data.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="extensionData"/> contains any <see langword="null"/> values.</exception>
        public PasswordCredentials(string username, string password, params JProperty[] extensionData)
            : base(extensionData)
        {
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCredentials"/> class with the specified credentials and
        /// extension data.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="extensionData">The extension data.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="extensionData"/> is <see langword="null"/>.</exception>
        public PasswordCredentials(string username, string password, ImmutableDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Gets the username in the authentication credentials.
        /// </summary>
        /// <value>
        /// <para>The username in the authentication credentials.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Username
        {
            get
            {
                return _username;
            }
        }

        /// <summary>
        /// Gets the password in the authentication credentials.
        /// </summary>
        /// <value>
        /// <para>The password in the authentication credentials.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Password
        {
            get
            {
                return _password;
            }
        }
    }
}
