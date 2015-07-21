using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// The default implementation of the OpenStack Networking API.
    /// <para>Intended for custom implementations.</para>
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack Networking API v2 Reference</seealso>
    public class NetworkingApiBuilder
    {
        /// <summary />
        protected readonly IAuthenticationProvider AuthenticationProvider;

        /// <summary />
        protected readonly ServiceUrlBuilder UrlBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkingService"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for the desired networking provider.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public NetworkingApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region)
        {
            if(serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (authenticationProvider == null)
                throw new ArgumentNullException("authenticationProvider");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentException("region cannot be null or empty", "region");

            AuthenticationProvider = authenticationProvider;
            UrlBuilder = new ServiceUrlBuilder(serviceType, authenticationProvider, region);
        }

        /// <summary>
        /// Lists all networks associated with the account.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A collection of network resources associated with the account.
        /// </returns>
        public async Task<PreparedRequest> ListNetworksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);
 
            return endpoint
                .AppendPathSegments("networks")
                .Authenticate(AuthenticationProvider)
                .PrepareGet(cancellationToken);
        }
        
        /// <summary>
        /// Gets the specified network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The network associated with the specified identifier.
        /// </returns>
        public virtual async Task<PreparedRequest> GetNetworkAsync(string networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(AuthenticationProvider)
                .PrepareGet(cancellationToken);
        }

        /// <summary>
        /// Creates a network.
        /// </summary>
        /// <param name="network">The network definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created network.
        /// </returns>
        public virtual async Task<PreparedRequest> CreateNetworkAsync(NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("networks")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(network, cancellationToken);
        }

        /// <summary>
        /// Bulk creates multiple networks.
        /// </summary>
        /// <param name="networks">The network definitions.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created networks.
        /// </returns>
        public virtual async Task<PreparedRequest> CreateNetworksAsync(IEnumerable<NetworkDefinition> networks, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("networks")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(networks, cancellationToken);
        }

        /// <summary>
        /// Updates the specified network.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="network">The updated network definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated network.
        /// </returns>
        public virtual async Task<PreparedRequest> UpdateNetworkAsync(string networkId, NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(AuthenticationProvider)
                .PreparePutJson(network, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public virtual async Task<PreparedRequest> DeleteNetworkAsync(string networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(AuthenticationProvider)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
        }
    }
}
