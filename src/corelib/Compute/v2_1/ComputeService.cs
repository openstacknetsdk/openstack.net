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
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        public ComputeService(IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
        {
            _computeApi = new ComputeApiBuilder(ServiceType.Compute, authenticationProvider, region, useInternalUrl);
        }

        #region Servers

        /// <inheritdoc cref="ComputeApiBuilder.GetServerAsync{T}" />
        public Task<Server> GetServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetServerAsync<Server>(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateServerAsync{T}" />
        public Task<Server> CreateServerAsync(ServerCreateDefinition server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateServerAsync<Server>(server, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitForServerStatusAsync{TServer,TStatus}(string,TStatus,TimeSpan?,TimeSpan?,IProgress{bool},CancellationToken)" />
        public Task<Server> WaitForServerStatusAsync(Identifier serverId, ServerStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitForServerStatusAsync<Server, ServerStatus>(serverId, status, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitForServerStatusAsync{TServer,TStatus}(string, IEnumerable{TStatus},TimeSpan?,TimeSpan?,IProgress{bool},CancellationToken)" />
        public Task<Server> WaitForServerStatusAsync(Identifier serverId, IEnumerable<ServerStatus> statuses, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitForServerStatusAsync<Server, ServerStatus>(serverId, statuses, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerReferencesAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<ServerReference>> ListServerReferencesAsync(ServerListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerReferencesAsync<ServerReferenceCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServersAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<Server>> ListServersAsync(ServerListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServersAsync<ServerCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.UpdateServerAsync{T}" />
        public Task<Server> UpdateServerAsync(Identifier serverid, ServerUpdateDefinition server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.UpdateServerAsync<Server>(serverid, server, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteServerAsync" />
        public Task DeleteServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilServerIsDeletedAsync{TServer,TStatus}" />
        public Task WaitUntilServerIsDeletedAsync(Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitUntilServerIsDeletedAsync<Server, ServerStatus>(serverId, null, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.SnapshotServerAsync{T}" />
        public Task<Image> SnapshotServerAsync(Identifier serverId, SnapshotServerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.SnapshotServerAsync<Image>(serverId, request, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.StartServerAsync" />
        public Task StartServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.StartServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.StopServerAsync" />
        public Task StopServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.StopServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.RebootServerAsync{TRequest}" />
        public Task RebootServerAsync(Identifier serverId, RebootServerRequest request = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.RebootServerAsync(serverId, request, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.EvacuateServerAsync" />
        public Task EvacuateServerAsync(Identifier serverId, EvacuateServerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.EvacuateServerAsync(serverId, request, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetVncConsoleAsync{T}" />
        public virtual Task<RemoteConsole> GetVncConsoleAync(Identifier serverId, RemoteConsoleType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetVncConsoleAsync<RemoteConsole>(serverId, type, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetSpiceConsoleAsync{T}" />
        public virtual Task<RemoteConsole> GetSpiceConsoleAync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetSpiceConsoleAsync<RemoteConsole>(serverId, RemoteConsoleType.SpiceHtml5, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetSerialConsoleAsync{T}" />
        public virtual Task<RemoteConsole> GetSerialConsoleAync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetSerialConsoleAsync<RemoteConsole>(serverId, RemoteConsoleType.Serial, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetRdpConsoleAsync{T}" />
        public virtual Task<RemoteConsole> GetRdpConsoleAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetRdpConsoleAsync<RemoteConsole>(serverId, RemoteConsoleType.RdpHtml5, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetConsoleOutputAsync" />
        public virtual Task<string> GetConsoleOutputAsync(Identifier serverId, int length = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetConsoleOutputAsync(serverId, length, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.RescueServerAsync" />
        public virtual Task<string> RescueServerAsync(Identifier serverId, RescueServerRequest request = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.RescueServerAsync(serverId, request, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.UnrescueServerAsync" />
        public virtual Task UnrescueServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.UnrescueServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ResizeServerAsync" />
        public virtual Task ResizeServerAsync(Identifier serverId, Identifier flavorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.ResizeServerAsync(serverId, flavorId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ConfirmResizeServerAsync" />
        public virtual Task ConfirmResizeServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.ConfirmResizeServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CancelResizeServerAsync" />
        public virtual Task CancelResizeServerAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CancelResizeServerAsync(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerActionsAsync{T}" />
        public virtual async Task<IEnumerable<ServerActionReference>> ListServerActionsAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerActionsAsync<ServerActionReferenceCollection>(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerActionsAsync{T}" />
        public virtual Task<ServerAction> GetServerActionAsync(Identifier serverId, Identifier actionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetServerActionAsync<ServerAction>(serverId, actionId, cancellationToken);
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

        /// <inheritdoc cref="ComputeApiBuilder.WaitForImageStatusAsync{TImage,TStatus}" />
        public Task<Image> WaitForImageStatusAsync(Identifier imageId, ImageStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitForImageStatusAsync<Image, ImageStatus>(imageId, status, refreshDelay, timeout, progress, cancellationToken);
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

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilImageIsDeletedAsync{TImage,TStatus}" />
        public Task WaitUntilImageIsDeletedAsync(Identifier imageId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.WaitUntilImageIsDeletedAsync<Image, ImageStatus>(imageId, null, refreshDelay, timeout, progress, cancellationToken);
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

        #region Server Volumes
        /// <inheritdoc cref="ComputeApiBuilder.GetServerVolumeAsync{T}" />
        public async Task<ServerVolume> GetServerVolumeAsync(Identifier serverId, Identifier volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.GetServerVolumeAsync<ServerVolume>(serverId, volumeId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerVolumesAsync{T}" />
        public async Task<IEnumerable<ServerVolume>> ListServerVolumesAsync(Identifier serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerVolumesAsync<ServerVolumeCollection>(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.AttachVolumeAsync{T}" />
        public Task<ServerVolume> AttachVolumeAsync(Identifier serverId, ServerVolumeDefinition volume, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.AttachVolumeAsync<ServerVolume>(serverId, volume, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DetachVolumeAsync" />
        public Task DetachVolumeAsync(Identifier serverId, Identifier volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DetachVolumeAsync(serverId, volumeId, cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <inheritdoc cref="ComputeApiBuilder.GetKeyPairAsync{T}" />
        public virtual Task<KeyPair> GetKeyPairAsync(string keypairName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetKeyPairAsync<KeyPair>(keypairName, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateKeyPairAsync{T}" />
        public virtual Task<KeyPairResponse> CreateKeyPairAsync(KeyPairRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateKeyPairAsync<KeyPairResponse>(request, cancellationToken);
        }

        /// <summary />
        public virtual Task<KeyPairSummary> ImportKeyPairAsync(KeyPairDefinition keypair, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateKeyPairAsync<KeyPairSummary>(keypair, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListKeyPairsAsync{T}" />
        public virtual async Task<IEnumerable<KeyPairSummary>> ListKeyPairsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListKeyPairsAsync<KeyPairSummaryCollection>(cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteKeyPairAsync(string keypairName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteKeyPairAsync(keypairName, cancellationToken);
        }
        #endregion

        #region Security Groups

        /// <inheritdoc cref="ComputeApiBuilder.GetSecurityGroupAsync{T}" />
        public Task<SecurityGroup> GetSecurityGroupAsync(Identifier securityGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetSecurityGroupAsync<SecurityGroup>(securityGroupId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateSecurityGroupAsync{T}" />
        public Task<SecurityGroup> CreateSecurityGroupAsync(SecurityGroupDefinition securityGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateSecurityGroupAsync<SecurityGroup>(securityGroup, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListSecurityGroupsAsync{T}" />
        public async Task<IEnumerable<SecurityGroup>> ListSecurityGroupsAsync(Identifier serverId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListSecurityGroupsAsync<SecurityGroupCollection>(serverId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.UpdateSecurityGroupAsync{T}" />
        public Task<SecurityGroup> UpdateSecurityGroupAsync(Identifier securityGroupId, SecurityGroupDefinition securityGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.UpdateSecurityGroupAsync<SecurityGroup>(securityGroupId, securityGroup, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteSecurityGroupAsync" />
        public Task DeleteSecurityGroupAsync(Identifier securityGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteSecurityGroupAsync(securityGroupId, cancellationToken);
        }

        #endregion

        #region Sever Groups

        /// <inheritdoc cref="ComputeApiBuilder.GetServerGroupAsync{T}" />
        public Task<ServerGroup> GetServerGroupAsync(Identifier serverGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetServerGroupAsync<ServerGroup>(serverGroupId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateServerGroupAsync{T}" />
        public Task<ServerGroup> CreateServerGroupAsync(ServerGroupDefinition serverGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateServerGroupAsync<ServerGroup>(serverGroup, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListServerGroupsAsync{T}" />
        public async Task<IEnumerable<ServerGroup>> ListServerGroupsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServerGroupsAsync<ServerGroupCollection>(cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteServerGroupAsync" />
        public Task DeleteServerGroupAsync(Identifier serverGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteServerGroupAsync(serverGroupId, cancellationToken);
        }

        #endregion

        #region Volumes

        /// <inheritdoc cref="ComputeApiBuilder.GetVolumeAsync{T}" />
        public Task<Volume> GetVolumeAsync(Identifier volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetVolumeAsync<Volume>(volumeId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetVolumeTypeAsync{T}" />
        public Task<VolumeType> GetVolumeTypeAsync(Identifier volumeTypeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetVolumeTypeAsync<VolumeType>(volumeTypeId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetVolumeSnapshotAsync{T}" />
        public Task<VolumeSnapshot> GetVolumeSnapshotAsync(Identifier snapshotId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetVolumeSnapshotAsync<VolumeSnapshot>(snapshotId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateVolumeAsync{T}" />
        public Task<Volume> CreateVolumeAsync(VolumeDefinition volume, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.CreateVolumeAsync<Volume>(volume, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.SnapshotVolumeAsync{T}" />
        public Task<VolumeSnapshot> SnapshotVolumeAsync(VolumeSnapshotDefinition snapshot, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.SnapshotVolumeAsync<VolumeSnapshot>(snapshot, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.ListVolumesAsync{T}" />
        public async Task<IEnumerable<Volume>> ListVolumesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListVolumesAsync<VolumeCollection>(cancellationToken);
        }

        ///// <inheritdoc cref="ComputeApiBuilder.ListVolumeTypesAsync{T}" />
        //public async Task<IEnumerable<VolumeType>> ListVolumeTypesAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return await _computeApi.ListVolumeTypesAsync<VolumeTypeCollection>(cancellationToken);
        //}

        /// <inheritdoc cref="ComputeApiBuilder.ListVolumeSnapshotsAsync{T}" />
        public async Task<IEnumerable<VolumeSnapshot>> ListVolumeSnapshotsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListVolumeSnapshotsAsync<VolumeSnapshotCollection>(cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteVolumeAsync" />
        public Task DeleteVolumeAsync(Identifier volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteVolumeAsync(volumeId, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteVolumeSnapshotAsync" />
        public Task DeleteVolumeSnapshotAsync(Identifier snapshotId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.DeleteVolumeSnapshotAsync(snapshotId, cancellationToken);
        }

        #endregion

        #region Compute Service
        /// <inheritdoc cref="ComputeApiBuilder.GetLimitsAsync{T}" />
        public Task<ServiceLimits> GetLimitsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetLimitsAsync<ServiceLimits>(cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetCurrentQuotasAsync{T}" />
        public Task<ServiceQuotas> GetCurrentQuotasAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetCurrentQuotasAsync<ServiceQuotas>(cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.GetDefaultQuotasAsync{T}" />
        public Task<ServiceQuotas> GetDefaultQuotasAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _computeApi.GetDefaultQuotasAsync<ServiceQuotas>(cancellationToken);
        }
        #endregion
    }
}
