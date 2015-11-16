using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Compute.v2_2.Serialization;

namespace OpenStack.Compute.v2_2
{
    /// <inheritdoc />
    public class ComputeService
    {
        private readonly ComputeApiBuilder _computeApiBuilder;

        /// <inheritdoc />
        public ComputeService(IAuthenticationProvider authenticationProvider, string region)
        {
            _computeApiBuilder = new ComputeApiBuilder(ServiceType.Compute, authenticationProvider, region);
        }

        #region Servers
        /// <inheritdoc cref="v2_1.ComputeApiBuilder.ListServersAsync{TPage,TItem}(string,int?,CancellationToken)" />
        public virtual Task<IPage<ServerReference>> ListServersAsync(Identifier startServerId = null, int? pageSize = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.ListServersAsync<ServerCollection, ServerReference>(startServerId, pageSize, cancellationToken);
        }

        /// <summary />
        public virtual Task<Console> GetVncConsoleAync(Identifier serverId, ConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.GetVncConsoleAsync<Console>(serverId, type, cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <summary />
        public virtual Task<KeyPair> CreateKeyPairAsync(string name, KeyPairType? type = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var keyPair = new KeyPairDefinition(name)
            {
                Type = type
            };
            return _computeApiBuilder.CreateKeyPairAsync<KeyPair>(keyPair, cancellationToken);
        }

        #endregion
    }
}
