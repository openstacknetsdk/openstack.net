using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Compute.v2_1;
using OpenStack.Compute.v2_2.Serialization;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_2
{
    /// <summary />
    public class ComputeService
    {
        private readonly ComputeApiBuilder _computeApiBuilder;

        /// <summary />
        public ComputeService(IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl)
        {
            _computeApiBuilder = new ComputeApiBuilder(ServiceType.Compute, authenticationProvider, region, useInternalUrl);
        }

        #region Servers
        /// <inheritdoc cref="v2_1.ComputeApiBuilder.ListServerSummariesAsync{TPage}" />
        public virtual async Task<IPage<ServerReference>> ListServerReferencesAsync(ServerListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApiBuilder.ListServerSummariesAsync<ServerCollection>(options, cancellationToken);
        }

        /// <summary />
        public virtual Task<RemoteConsole> GetVncConsoleAync(Identifier serverId, RemoteConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.GetVncConsoleAsync<RemoteConsole>(serverId, type, cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <summary />
        public virtual Task<KeyPair> ImportKeyPairAsync(KeyPairDefinition keypair, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.CreateKeyPairAsync<KeyPair>(keypair, cancellationToken);
        }

        #endregion
    }
}
