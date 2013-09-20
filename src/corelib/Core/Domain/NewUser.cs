﻿namespace net.openstack.Core.Domain
{
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the JSON result of an Add User operation.
    /// </summary>
    /// <seealso cref="IIdentityProvider.AddUser"/>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_addUser_v2.0_users_.html">Add User (OpenStack Identity Service API v2.0 Reference)</seealso>
    /// <seealso href="http://docs.rackspace.com/auth/api/v2.0/auth-client-devguide/content/POST_addUser_v2.0_users_.html">Add User (Rackspace Cloud Identity Client Developer Guide - API v2.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    public class NewUser
    {
        /// <summary>
        /// Gets the generated password for the new user.
        /// <note type="warning">The value of this property is not defined by OpenStack, and may not be consistent across vendors.</note>
        /// </summary>
        /// <value>The generated password for the new user, or <c>null</c> if the Add User request included a password.</value>
        [JsonProperty("OS-KSADM:password")]
        public string Password { get; internal set; }

        /// <summary>
        /// Gets the ID for the new user.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the username of the new user.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; private set; }

        /// <summary>
        /// Gets the email address of the new user.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the user is enabled.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewUser"/> class with the specified
        /// username, email address, password, and value indicating whether or not the user
        /// is initially enabled.
        /// </summary>
        /// <param name="username">The username of the new user (see <see cref="Username"/>).</param>
        /// <param name="email">The email address of the new user (see <see cref="Email"/>).</param>
        /// <param name="password">The password for the new user (see <see cref="Password"/>).</param>
        /// <param name="enabled"><c>true</c> if the user is initially enabled; otherwise, <c>false</c> (see <see cref="Enabled"/>).</param>
        public NewUser(string username, string email, string password = null, bool enabled = true)
        {
            Username = username;
            Email = email;
            Password = password;
            Enabled = enabled;
        }
    }
}
