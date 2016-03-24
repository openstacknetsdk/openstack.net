using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenStack.Networking.v2.Serialization;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Networking.v2.Layer3
{
    /// <summary>
    /// Exposes functionality from the Level3 networking extension
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2-ext.html#layer3-ext"/>
    public static class NetworkingService_Layer3_Extensions
    {
        #region Floating IPs
        /// <inheritdoc cref="NetworkingApiBuilder.GetFloatingIPAsync{T}" />
        public static Task<FloatingIP> GetFloatingIPAsync(this NetworkingService service, Identifier floatingIPId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return service._networkingApiBuilder.GetFloatingIPAsync<FloatingIP>(floatingIPId, cancellationToken);
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateFloatingIPAsync{T}" />
        public static Task<FloatingIP> CreateFloatingIPAsync(this NetworkingService service, FloatingIPCreateDefinition floatingIP, CancellationToken cancellationToken = default(CancellationToken))
        {
            return service._networkingApiBuilder.CreateFloatingIPAsync<FloatingIP>(floatingIP, cancellationToken);
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdateFloatingIPAsync{T}" />
        public static Task<FloatingIP> UpdateFloatingIPAsync(this NetworkingService service, FloatingIPUpdateDefinition floatingIP, CancellationToken cancellationToken = default(CancellationToken))
        {
            return service._networkingApiBuilder.CreateFloatingIPAsync<FloatingIP>(floatingIP, cancellationToken);
        }

        /// <inheritdoc cref="NetworkingApiBuilder.ListFloatingIPsAsync{T}" />
        public static async Task<IEnumerable<FloatingIP>> ListFloatingIPAsync(this NetworkingService service, FloatingIPListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await service._networkingApiBuilder.ListFloatingIPsAsync<FloatingIPCollection>(options, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeleteFloatingIPAsync" />
        public static Task DeleteFloatingIPAsync(this NetworkingService service, Identifier floatingIPId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return service._networkingApiBuilder.DeleteFloatingIPAsync(floatingIPId, cancellationToken);
        }
        #endregion
    }
}

namespace OpenStack.Networking.v2.Layer3.Synchronous
{
    /// <summary>
    /// Exposes synchronous extension methods for the Level3 networking extension
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2-ext.html#layer3-ext"/>
    public static class NetworkingService_Layer3_Extensions
    {
        #region Floating IPs
        /// <inheritdoc cref="NetworkingApiBuilder.GetFloatingIPAsync{T}" />
        public static FloatingIP GetFloatingIP(this NetworkingService service, Identifier floatingIPId)
        {
            return service._networkingApiBuilder.GetFloatingIPAsync<FloatingIP>(floatingIPId).ForceSynchronous();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateFloatingIPAsync{T}" />
        public static FloatingIP CreateFloatingIP(this NetworkingService service, FloatingIPCreateDefinition floatingIP)
        {
            return service._networkingApiBuilder.CreateFloatingIPAsync<FloatingIP>(floatingIP).ForceSynchronous();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdateFloatingIPAsync{T}" />
        public static FloatingIP UpdateFloatingIP(this NetworkingService service, FloatingIPUpdateDefinition floatingIP)
        {
            return service._networkingApiBuilder.CreateFloatingIPAsync<FloatingIP>(floatingIP).ForceSynchronous();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.ListFloatingIPsAsync{T}" />
        public static IEnumerable<FloatingIP> ListFloatingIPs(this NetworkingService service, FloatingIPListOptions options = null)
        {
            return service._networkingApiBuilder.ListFloatingIPsAsync<FloatingIPCollection>(options).ForceSynchronous();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeleteFloatingIPAsync" />
        public static void DeleteFloatingIP(this NetworkingService service, Identifier floatingIPId)
        {
            service._networkingApiBuilder.DeleteFloatingIPAsync(floatingIPId).ForceSynchronous();
        }
        #endregion
    }
}
