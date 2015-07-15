using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using OpenStack.Authentication;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// The default provider for the OpenStack Networking Service.
    /// </summary>
    /// <preliminary/>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2-ext.html">OpenStack Networking API v2 Reference</seealso>
    public class NetworkingService : INetworkingService
    {
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ServiceUrlBuilder _urlBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkingService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public NetworkingService(IAuthenticationProvider authenticationProvider, string region)
        {
            if (authenticationProvider == null)
                throw new ArgumentNullException("authenticationProvider");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentException("region cannot be null or empty", "region");

            _authenticationProvider = authenticationProvider;
            _urlBuilder = new ServiceUrlBuilder(ServiceType.Networking, authenticationProvider, region);
        }

        public async Task<IEnumerable<Network>> ListNetworksAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            string endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("networks")
                .Authenticate(_authenticationProvider)
                .GetJsonAsync<NetworkCollection>(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Network> GetNetworkAsync(string networkId, CancellationToken cancellationToken = new CancellationToken())
        {
            string endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(_authenticationProvider)
                .GetJsonAsync<Network>(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Network> CreateNetworkAsync(NetworkDefinition network, CancellationToken cancellationToken = new CancellationToken())
        {
            string endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("networks")
                .Authenticate(_authenticationProvider)
                .PostJsonAsync(network, cancellationToken)
                .ReceiveJson<Network>()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Network>> CreateNetworksAsync(IEnumerable<NetworkDefinition> networks, CancellationToken cancellationToken = new CancellationToken())
        {
            string endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("networks")
                .Authenticate(_authenticationProvider)
                .PostJsonAsync(new NetworkDefinitionCollection(networks), cancellationToken)
                .ReceiveJson<NetworkCollection>()
                .ConfigureAwait(false);
        }

        public async Task<Network> UpdateNetworkAsync(string networkId, NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(_authenticationProvider)
                .PutJsonAsync(network, cancellationToken)
                .ReceiveJson<Network>()
                .ConfigureAwait(false);
        }

        public async Task DeleteNetworkAsync(string networkId, CancellationToken cancellationToken = new CancellationToken())
        {
            string endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            await endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(_authenticationProvider)
                .AllowHttpStatus(HttpStatusCode.NotFound)
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
