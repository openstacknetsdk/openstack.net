using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
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

        /// <inheritdoc cref="ComputeApiBuilder.GetServerAsync{T}" />
        public async Task<Server> GetServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.GetServerAsync<Server>(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateServerAsync{TPage}" />
        public async Task<Server> CreateServerAsync(ServerCreateDefinition server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.CreateServerAsync<Server>(server, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilServerIsActiveAsync" />
        public async Task<Server> WaitUntilServerIsActiveAsync(Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.WaitUntilServerIsActiveAsync(serverId, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServersAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public virtual async Task<IPage<ServerReference>> ListServersAsync(ListServersOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServersAsync<ServerReferenceCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerDetailsAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public virtual async Task<IPage<Server>> ListServerDetailsAsync(ListServersOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerDetailsAsync<ServerCollection>(options, cancellationToken);
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
