using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Represents a provider for the OpenStack (Neutron) Networking Service.
    /// </summary>
    /// <preliminary/>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack (Neutron) Networking API v2 Reference</seealso>
    public interface INetworkingService
    {
        /// <summary>
        /// Lists all networks associated with the account.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="FlurlHttpException">If the service call returns a bad <see cref="HttpStatusCode"/>.</exception>
        /// <returns>
        /// A collection of <see cref="Network" /> resources associated with the acccount./>
        /// </returns>
        Task<IEnumerable<Network>> ListNetworksAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the specified network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="networkId"/> is <see langword="null"/>.</exception>
        /// <exception cref="FlurlHttpException">If the service call returns a bad <see cref="HttpStatusCode"/>.</exception>
        /// <returns>
        /// The network associated with the specified identifer.
        /// </returns>
        Task<Network> GetNetworkAsync(string networkId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a network.
        /// </summary>
        /// <param name="network">The network definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="network"/> is <see langword="null"/>.</exception>
        /// <exception cref="FlurlHttpException">If the service call returns a bad <see cref="HttpStatusCode"/>.</exception>
        /// <returns>
        /// The created network.
        /// </returns>
        Task<Network> CreateNetworkAsync(NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Bulk creates multiple networks.
        /// </summary>
        /// <param name="networks">The network definitions.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="networks"/> is <see langword="null"/>.</exception>
        /// <exception cref="FlurlHttpException">If the service call returns a bad <see cref="HttpStatusCode"/>.</exception>
        /// <returns>
        /// The created networks.
        /// </returns>
        Task<IEnumerable<Network>> CreateNetworksAsync(IEnumerable<NetworkDefinition> networks, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the specified network.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="network">The updated network definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="network"/> is <see langword="null"/>.</exception>
        /// <exception cref="FlurlHttpException">If the service call returns a bad <see cref="HttpStatusCode"/>.</exception>
        /// <returns>
        /// The updated network.
        /// </returns>
        Task<Network> UpdateNetworkAsync(string networkId, NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the specified network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="networkId"/> is <see langword="null"/>.</exception>
        /// <exception cref="FlurlHttpException">If the service call returns a bad <see cref="HttpStatusCode"/>.</exception>
        Task DeleteNetworkAsync(string networkId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
