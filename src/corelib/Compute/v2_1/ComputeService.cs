using System;
using System.Collections.Generic;
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

        /// <inheritdoc cref="ComputeApiBuilder.GetServerAsync{T}" />
        public Task<Server> GetServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetServerAsync<Server>(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateServerAsync{TPage}" />
        public Task<Server> CreateServerAsync(ServerCreateDefinition server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateServerAsync<Server>(server, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilServerIsActiveAsync" />
        public Task<Server> WaitUntilServerIsActiveAsync(Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitUntilServerIsActiveAsync(serverId, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServersAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<ServerReference>> ListServersAsync(ListServersOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServersAsync<ServerReferenceCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerDetailsAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<Server>> ListServerDetailsAsync(ListServersOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerDetailsAsync<ServerCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.UpdateServerAsync{TPage}" />
        public Task<Server> UpdateServerAsync(Identifier serverid, ServerUpdateDefinition server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.UpdateServerAsync<Server>(serverid, server, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteServerAsync" />
        public Task DeleteServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilServerIsDeletedAsync" />
        public Task WaitUntilServerIsDeletedAsync(Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitUntilServerIsDeletedAsync(serverId, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateSnapshotAsync{T}" />
        public Task<Image> CreateSnapshotAsync(Identifier serverId, SnapshotServerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateSnapshotAsync<Image>(serverId, request, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.StartServerAsync" />
        public Task StartServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.StartServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetVncConsoleAsync{T}" />
        public virtual Task<Console> GetVncConsoleAync(Identifier serverId, ConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetVncConsoleAsync<Console>(serverId, type, cancellationToken);
        }
        #endregion

        #region Flavors

        /// <inheritdoc cref="ComputeApiBuilder.GetFlavorAsync{T}" />
        public Task<Flavor> GetFlavorAsync(Identifier flavorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetFlavorAsync<Flavor>(flavorId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListFlavorsAsync{T}" />
        public async Task<IEnumerable<FlavorReference>> ListFlavorsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListFlavorsAsync<FlavorReferenceCollection>(cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListFlavorDetailsAsync{T}" />
        public async Task<IEnumerable<Flavor>> ListFlavorDetailsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListFlavorDetailsAsync<FlavorCollection>(cancellationToken);
        }

        #endregion

        #region Images

        /// <inheritdoc cref="ComputeApiBuilder.GetImageAsync{T}" />
        public Task<Image> GetImageAsync(Identifier imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetImageAsync<Image>(imageId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetImageMetadataAsync{T}" />
        public Task<ImageMetadata> GetImageMetadataAsync(Identifier imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetImageMetadataAsync<ImageMetadata>(imageId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetImageMetadataItemAsync" />
        public Task<string> GetImageMetadataItemAsync(Identifier imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetImageMetadataItemAsync(imageId, key, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilImageIsActiveAsync" />
        public Task<Image> WaitUntilImageIsActiveAsync(Identifier imageId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitUntilImageIsActiveAsync(imageId, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateImagMetadataAsync" />
        public Task CreateImagMetadataAsync(Identifier imageId, string key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateImagMetadataAsync(imageId, key, value, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListImagesAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<ImageReference>> ListImagesAsync(ImageListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListImagesAsync<ImageReferenceCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListImageDetailsAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<Image>> ListImageDetailsAsync(ImageListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListImageDetailsAsync<ImageCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.UpdateImageMetadataAsync{T}" />
        public Task<ImageMetadata> UpdateImageMetadataAsync(Identifier imageId, ImageMetadata metadata, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.UpdateImageMetadataAsync<ImageMetadata>(imageId, metadata, overwrite, cancellationToken);
        }
        
        /// <inheritdoc cref="ComputeApiBuilder.DeleteImageAsync" />
        public Task DeleteImageAsync(Identifier imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteImageAsync(imageId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteImageMetadataAsync" />
        public Task DeleteImageMetadataAsync(Identifier imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteImageMetadataAsync(imageId, key, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilImageIsDeletedAsync" />
        public Task WaitUntilImageIsDeletedAsync(Identifier imageId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitUntilImageIsDeletedAsync(imageId, refreshDelay, timeout, progress, cancellationToken);
        }
        #endregion

        #region IP Addresses

        /// <inheritdoc cref="ComputeApiBuilder.GetServerAddressAsync{T}" />
        public Task<IList<ServerAddress>> GetServerAddressAsync(Identifier serverId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetServerAddressAsync<ServerAddress>(serverId, key, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerAddressesAsync{T}" />
        public async Task<IDictionary<string, IList<ServerAddress>>> ListServerAddressesAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerAddressesAsync<ServerAddressCollection>(serverId, cancellationToken);
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
