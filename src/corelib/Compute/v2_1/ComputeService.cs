using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// The OpenStack Compute Service.
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-compute-v2.1.html">OpenStack Compute API v2.1 Overview</seealso>
    public class ComputeService
    {
        /// <summary />
        private readonly ComputeApiBuilder _computeApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public ComputeService(IAuthenticationProvider authenticationProvider, string region)
        {
            _computeApi = new ComputeApiBuilder(ServiceType.Compute, authenticationProvider, region);
        }
    
        #region Servers
        /// <inheritdoc cref="ComputeApiBuilder.ListServersAsync{TPage,TItem}(IQueryStringBuilder,CancellationToken)" />
        public virtual Task<IPage<ServerReference>> ListServersAsync(ListServersOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.ListServersAsync<ServerCollection, ServerReference>(options, cancellationToken);
        }
        
        /// <inheritdoc cref="ComputeApiBuilder.GetVncConsoleAsync{T}" />
        public virtual Task<Console> GetVncConsoleAync(Identifier serverId, ConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetVncConsoleAsync<Console>(serverId, type, cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <inheritdoc cref="ComputeApiBuilder.CreateKeyPairAsync{T}" />
        public virtual Task<KeyPair> CreateKeyPairAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var keyPair = new KeyPairDefinition(name);
            return _computeApi.CreateKeyPairAsync<KeyPair>(keyPair, cancellationToken);
        }

        #endregion

    }
}
