namespace Rackspace.Security.Authentication
{
    using System;
    using System.Collections.Immutable;
    using Newtonsoft.Json.Linq;
    using OpenStack.Services.Identity.V2;

    /// <summary>
    /// This class provides utility methods for creating <see cref="AuthenticationRequest"/> instances suitable for
    /// authenticating with the Rackspace Cloud Identity service.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class RackspaceAuthentication
    {
        /// <summary>
        /// Create an <see cref="AuthenticationRequest"/> using the specified username and API key as credentials.
        /// </summary>
        /// <param name="username">The account username.</param>
        /// <param name="apiKey">The account API key.</param>
        /// <returns>
        /// <para>An <see cref="AuthenticationRequest"/> instance containing the specified credentials, which is
        /// typically used for constructing an instance of <see cref="RackspaceAuthenticationService"/>.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="username"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>If <paramref name="username"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is empty.</para>
        /// </exception>
        public static AuthenticationRequest ApiKey(string username, string apiKey)
        {
            if (username == null)
                throw new ArgumentNullException("username");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username cannot be empty");
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("apiKey cannot be empty");

            var extensionData = ImmutableDictionary<string, JToken>.Empty
                .Add("RAX-KSKEY:apiKeyCredentials", new JObject(
                    new JProperty("username", username),
                    new JProperty("apiKey", apiKey)));
            AuthenticationData authenticationData = new AuthenticationData(extensionData);

            AuthenticationRequest authenticationRequest = new AuthenticationRequest(authenticationData);
            return authenticationRequest;
        }

        /// <summary>
        /// Create an <see cref="AuthenticationRequest"/> using the specified username and password as credentials.
        /// </summary>
        /// <remarks>
        /// <note type="warning">
        /// For improved security, clients are encouraged to use API key credentials instead of a password whenever
        /// possible.
        /// </note>
        /// </remarks>
        /// <param name="username">The account username.</param>
        /// <param name="password">The account password.</param>
        /// <returns>
        /// <para>An <see cref="AuthenticationRequest"/> instance containing the specified credentials, which is
        /// typically used for constructing an instance of <see cref="RackspaceAuthenticationService"/>.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="username"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>If <paramref name="username"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is empty.</para>
        /// </exception>
        public static AuthenticationRequest Password(string username, string password)
        {
            if (username == null)
                throw new ArgumentNullException("username");
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username cannot be empty");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password cannot be empty");

            PasswordCredentials passwordCredentials = new PasswordCredentials(username, password);
            AuthenticationData authenticationData = new AuthenticationData(passwordCredentials);
            AuthenticationRequest authenticationRequest = new AuthenticationRequest(authenticationData);
            return authenticationRequest;
        }
    }
}
