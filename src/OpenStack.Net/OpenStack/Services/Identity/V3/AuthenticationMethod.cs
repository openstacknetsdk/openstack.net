namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents an authentication method in the <see cref="IIdentityService"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known authentication methods,
    /// with added support for unknown methods supported by a server extension.
    /// </remarks>
    /// <seealso cref="DatabaseInstance.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(AuthenticationMethod.Converter))]
    public sealed class AuthenticationMethod : ExtensibleEnum<AuthenticationMethod>
    {
        private static readonly ConcurrentDictionary<string, AuthenticationMethod> _types =
            new ConcurrentDictionary<string, AuthenticationMethod>(StringComparer.OrdinalIgnoreCase);
        private static readonly AuthenticationMethod _password = FromName("password");
        private static readonly AuthenticationMethod _token = FromName("token");

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationMethod"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private AuthenticationMethod(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="AuthenticationMethod"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="AuthenticationMethod"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static AuthenticationMethod FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new AuthenticationMethod(i));
        }

        /// <summary>
        /// Gets an <see cref="AuthenticationMethod"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static AuthenticationMethod Password
        {
            get
            {
                return _password;
            }
        }

        /// <summary>
        /// Gets an <see cref="AuthenticationMethod"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static AuthenticationMethod Token
        {
            get
            {
                return _token;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="AuthenticationMethod"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override AuthenticationMethod FromName(string name)
            {
                return AuthenticationMethod.FromName(name);
            }
        }
    }
}
