using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Compute.v2_2.Serialization;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_2
{
    /// <summary />
    public class ComputeService
    {
        private readonly ComputeApiBuilder _computeApiBuilder;

        /// <summary />
        public ComputeService(IAuthenticationProvider authenticationProvider, string region)
        {
            _computeApiBuilder = new ComputeApiBuilder(ServiceType.Compute, authenticationProvider, region);
        }

        #region Servers
        /// <inheritdoc cref="v2_1.ComputeApiBuilder.ListServersAsync{T}(IQueryStringBuilder,CancellationToken)" />
        public virtual async Task<IPage<ServerReference>> ListServersAsync(ListServersOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApiBuilder.ListServersAsync<ServerCollection>(options, cancellationToken);
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
