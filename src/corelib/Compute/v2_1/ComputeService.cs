using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Compute.v2_1.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// The OpenStack Compute Service.
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-compute-v2.1.html">OpenStack Compute API v2.1 Overview</seealso>
    public class ComputeService
    {
        private readonly ComputeApiBuilder _computeApiBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public ComputeService(IAuthenticationProvider authenticationProvider, string region)
        {
            _computeApiBuilder = new ComputeApiBuilder(ServiceType.Networking, authenticationProvider, region);
        }

        #region Servers
        /// <inheritdoc cref="ComputeApiBuilder.ListServersAsync{TPage,TItem}(string,int?,CancellationToken)" />
        public virtual Task<IPage<ServerReference>> ListServersAsync(Identifier startServerId = null, int? pageSize = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.ListServersAsync<ServerCollection, ServerReference>(startServerId, pageSize, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetVncConsoleAsync{T}" />
        public virtual Task<Console> GetVncConsoleAync(Identifier serverId, ConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.GetVncConsoleAsync<Console>(serverId, type, cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <inheritdoc cref="ComputeApiBuilder.CreateKeyPairAsync{T}" />
        public virtual Task<KeyPair> CreateKeyPairAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var keyPair = new KeyPairDefinition(name);
            return _computeApiBuilder.CreateKeyPairAsync<KeyPair>(keyPair, cancellationToken);
        }

        #endregion

    }
}
