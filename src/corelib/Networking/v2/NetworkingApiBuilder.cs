using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;
using OpenStack.Networking.v2.Serialization;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Builds requests to the Networking API which can be further customized and then executed.
    /// <para>Intended for custom implementations.</para>
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack Networking API v2 Reference</seealso>
    public class NetworkingApiBuilder
    {
        /// <summary />
        protected readonly IAuthenticationProvider AuthenticationProvider;

        /// <summary />
        protected readonly ServiceEndpoint Endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkingApiBuilder"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for the desired networking provider.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        public NetworkingApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl)
        {
            if(serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (authenticationProvider == null)
                throw new ArgumentNullException("authenticationProvider");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentException("region cannot be null or empty", "region");

            AuthenticationProvider = authenticationProvider;
            Endpoint = new ServiceEndpoint(serviceType, authenticationProvider, region, useInternalUrl);
        }

        #region Networks
        /// <summary>
        /// Lists all networks associated with the account.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A collection of network resources associated with the account.
        /// </returns>
        public async Task<PreparedRequest> ListNetworksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
 
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
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

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
        public virtual async Task<PreparedRequest> CreateNetworkAsync(object network, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

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
        public virtual async Task<PreparedRequest> CreateNetworksAsync(IEnumerable<object> networks, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("networks")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(new NetworkDefinitionCollection(networks), cancellationToken);
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
        public virtual async Task<PreparedRequest> UpdateNetworkAsync(string networkId, object network, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

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
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("networks", networkId)
                .Authenticate(AuthenticationProvider)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
        }
        #endregion

        #region Subnets

        /// <summary>
        /// Lists all subnets associated with the account.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A collection of subnet resources associated with the account.
        /// </returns>
        public virtual async Task<PreparedRequest> ListSubnetsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("subnets")
                .Authenticate(AuthenticationProvider)
                .PrepareGet(cancellationToken);
        }

        /// <summary>
        /// Creates a subnet.
        /// </summary>
        /// <param name="subnet">The subnet definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created subnet.
        /// </returns>
        public virtual async Task<PreparedRequest> CreateSubnetAsync(object subnet, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("subnets")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(subnet, cancellationToken);
        }

        /// <summary>
        /// Bulk creates multiple subnets.
        /// </summary>
        /// <param name="subnets">The subnet definitions.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created subnets.
        /// </returns>
        public virtual async Task<PreparedRequest> CreateSubnetsAsync(IEnumerable<object> subnets, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("subnets")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(new SubnetDefinitionCollection(subnets), cancellationToken);
        }

        /// <summary>
        /// Gets the specified subnet.
        /// </summary>
        /// <param name="subnetId">The subnet identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The subnet associated with the specified identifier.
        /// </returns>
        public virtual async Task<PreparedRequest> GetSubnetAsync(string subnetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("subnets", subnetId)
                .Authenticate(AuthenticationProvider)
                .PrepareGet(cancellationToken);
        }

        /// <summary>
        /// Updates the specified subnet.
        /// </summary>
        /// <param name="subnetId"></param>
        /// <param name="subnet">The updated subnet definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated subnet.
        /// </returns>
        public virtual async Task<PreparedRequest> UpdateSubnetAsync(string subnetId, object subnet, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("subnets", subnetId)
                .Authenticate(AuthenticationProvider)
                .PreparePutJson(subnet, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified subnet.
        /// </summary>
        /// <param name="subnetId">The subnet identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public virtual async Task<PreparedRequest> DeleteSubnetAsync(string subnetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("subnets", subnetId)
                .Authenticate(AuthenticationProvider)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
        }
        #endregion

        #region Ports
        /// <summary>
        /// Lists all ports associated with the account.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A collection of port resources associated with the account.
        /// </returns>
        public virtual async Task<PreparedRequest> ListPortsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("ports")
                .Authenticate(AuthenticationProvider)
                .PrepareGet(cancellationToken);
        }

        /// <summary>
        /// Creates a port.
        /// </summary>
        /// <param name="port">The port definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created port.
        /// </returns>
        public virtual async Task<PreparedRequest> CreatePortAsync(object port, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("ports")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(port, cancellationToken);
        }

        /// <summary>
        /// Bulk creates multiple ports.
        /// </summary>
        /// <param name="ports">The port definitions.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created subnets.
        /// </returns>
        public virtual async Task<PreparedRequest> CreatePortsAsync(IEnumerable<object> ports, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("ports")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(new PortDefinitionCollection(ports), cancellationToken);
        }

        /// <summary>
        /// Gets the specified port.
        /// </summary>
        /// <param name="portId">The port identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The port associated with the specified identifier.
        /// </returns>
        public virtual async Task<PreparedRequest> GetPortAsync(string portId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("ports", portId)
                .Authenticate(AuthenticationProvider)
                .PrepareGet(cancellationToken);
        }

        /// <summary>
        /// Updates the specified port.
        /// </summary>
        /// <param name="portId">The port identifier.</param>
        /// <param name="port">The updated port definition.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated port.
        /// </returns>
        public virtual async Task<PreparedRequest> UpdatePortAsync(string portId, object port, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("ports", portId)
                .Authenticate(AuthenticationProvider)
                .PreparePutJson(port, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified port.
        /// </summary>
        /// <param name="portId">The port identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public virtual async Task<PreparedRequest> DeletePortAsync(string portId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("ports", portId)
                .Authenticate(AuthenticationProvider)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
        }
        #endregion
    }
}
