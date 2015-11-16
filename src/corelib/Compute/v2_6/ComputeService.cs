using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Compute.v2_6.Serialization;

namespace OpenStack.Compute.v2_6
{
    /// <inheritdoc />
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

        /// <summary />
        public virtual Task<IPage<ServerReference>> ListServersAsync(Identifier startServerId = null, int? pageSize = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.ListServersAsync<ServerCollection, ServerReference>(startServerId, pageSize, cancellationToken);
        }

        /// <summary />
        public virtual Task<Console> GetConsoleAsync(Identifier serverId, ConsoleProtocol protocol, ConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApiBuilder.GetConsoleAsync<Console>(serverId, protocol, type, cancellationToken);
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
