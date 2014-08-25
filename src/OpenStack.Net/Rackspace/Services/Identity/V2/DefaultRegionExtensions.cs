namespace Rackspace.Services.Identity.V2
{
    using System;
    using Newtonsoft.Json.Linq;
    using OpenStack.Services.Identity.V2;

    /// <summary>
    /// Provides extension methods for accessing the default region properties of authenticated users in the Rackspace
    /// Cloud Identity product.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class DefaultRegionExtensions
    {
        /// <summary>
        /// The name of the default region property as it appears in the JSON representation.
        /// </summary>
        private const string DefaultRegionProperty = "RAX-AUTH:defaultRegion";

        /// <summary>
        /// Gets the default region for an authenticated user.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object for a user authenticated with Rackspace Cloud
        /// Identity.</param>
        /// <returns>
        /// <para>The default region for the user.</para>
        /// <token>NullIfNotIncluded</token>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="user"/> is <see langword="null"/>.</exception>
        public static string GetDefaultRegion(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            JToken value;
            if (!user.ExtensionData.TryGetValue(DefaultRegionProperty, out value))
                return null;

            return value.ToObject<string>();
        }
    }
}
