namespace net.openstack.Core.Domain
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the authentication token used for making authenticated calls to
    /// multiple APIs.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class IdentityToken
    {
        /// <summary>
        /// Gets the token expiration time in the format originally returned by the
        /// authentication response.
        /// </summary>
        /// <seealso cref="IIdentityProvider.GetToken"/>
        [JsonProperty("expires")]
        public string Expires { get; set; }

        /// <summary>
        /// Gets the token ID which can be used to make authenticated API calls.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets a <see cref="Tenant"/> object containing the name and ID of the
        /// tenant (or account) for the authenticated credentials.
        /// </summary>
        [JsonProperty("tenant")]
        public Tenant Tenant { get; set; }

        /// <summary>
        /// Gets whether or not the token has expired. This method simply checks
        /// the <see cref="Expires"/> property against the current system time.
        /// If the <see cref="Expires"/> value is missing or not in a recognized
        /// format, the token is assumed to have expired.
        /// </summary>
        /// <returns><c>true</c> if the token has expired; otherwise, <c>false</c>.</returns>
        public bool IsExpired()
        {
            if (string.IsNullOrWhiteSpace(Expires))
                return true;

            DateTime expiration;
            if (!DateTime.TryParse(Expires, out expiration))
                return true;

            return expiration.ToUniversalTime() <= DateTime.UtcNow;
        }
    }
}
