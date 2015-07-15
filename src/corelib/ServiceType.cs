using OpenStack.Authentication;

namespace OpenStack
{
    /// <summary>
    /// Types of services supported by <see cref="IAuthenticationProvider"/>.
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// The Content Delivery Network (CDN) service
        /// </summary>
        ContentDeliveryNetwork,

        /// <summary>
        /// The Networking service
        /// </summary>
        Networking
    }
}
