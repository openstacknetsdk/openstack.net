using System;
using System.Collections.Generic;
using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for a <see cref="ComputeService"/> instance.
    /// </summary>
    public static class ComputeServiceExtensions_v2_1
    {
        #region Servers
        /// <inheritdoc cref="ComputeService.GetServerAsync" />
        public static Server GetServer(this ComputeService service, Identifier serverId)
        {
            return service.GetServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CreateServerAsync" />
        public static Server CreateServer(this ComputeService service, ServerCreateDefinition server)
        {
            return service.CreateServerAsync(server).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.WaitForServerStatusAsync(Identifier,ServerStatus,TimeSpan?,TimeSpan?,IProgress{bool},System.Threading.CancellationToken)" />
        public static Server WaitForServerStatus(this ComputeService service, Identifier serverId, ServerStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            return service.WaitForServerStatusAsync(serverId, status, refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.WaitForServerStatusAsync(Identifier,IEnumerable{ServerStatus},TimeSpan?,TimeSpan?,IProgress{bool},System.Threading.CancellationToken)" />
        public static Server WaitForServerStatus(this ComputeService service, Identifier serverId, IEnumerable<ServerStatus> statuses, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            return service.WaitForServerStatusAsync(serverId, statuses, refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServersAsync" />
        public static IPage<ServerReference> ListServers(this ComputeService service, ListServersOptions options = null)
        {
            return service.ListServersAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServerDetailsAsync" />
        public static IPage<Server> ListServerDetails(this ComputeService service, ListServersOptions options = null)
        {
            return service.ListServerDetailsAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.UpdateServerAsync" />
        public static Server UpdateServer(this ComputeService service, Identifier serverid, ServerUpdateDefinition server)
        {
            return service.UpdateServerAsync(serverid, server).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.DeleteServerAsync" />
        public static void DeleteServer(this ComputeService service, Identifier serverId)
        {
            service.DeleteServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.WaitUntilServerIsDeletedAsync" />
        public static void WaitUntilServerIsDeleted(this ComputeService service, Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            service.WaitUntilServerIsDeletedAsync(serverId, refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CreateSnapshotAsync" />
        public static Image CreateSnapshot(this ComputeService service, Identifier serverId, SnapshotServerRequest request)
        {
            return service.CreateSnapshotAsync(serverId, request).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.StartServerAsync" />
        public static void StartServer(this ComputeService service, Identifier serverId)
        {
            service.StartServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.StopServerAsync" />
        public static void StopServer(this ComputeService service, Identifier serverId)
        {
            service.StopServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.RebootServerAsync" />
        public static void RebootServer(this ComputeService service, Identifier serverId, RebootServerRequest request = null)
        {
            service.RebootServerAsync(serverId, request).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.EvacuateServerAsync" />
        public static void EvacuateServer(this ComputeService service, Identifier serverId, EvacuateServerRequest request)
        {
            service.EvacuateServerAsync(serverId, request).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetVncConsoleAync" />
        public static RemoteConsole GetVncConsole(this ComputeService service, Identifier serverId, RemoteConsoleType type)
        {
            return service.GetVncConsoleAync(serverId, type).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetSpiceConsoleAync" />
        public static RemoteConsole GetSpiceConsole(this ComputeService service, Identifier serverId)
        {
            return service.GetSpiceConsoleAync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetSpiceConsoleAync" />
        public static RemoteConsole GetSerialConsole(this ComputeService service, Identifier serverId)
        {
            return service.GetSerialConsoleAync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetRdpConsoleAsync" />
        public static RemoteConsole GetRdpConsole(this ComputeService service, Identifier serverId)
        {
            return service.GetRdpConsoleAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetConsoleOutputAsync" />
        public static string GetConsoleOutput(this ComputeService service, Identifier serverId, int length = -1)
        {
            return service.GetConsoleOutputAsync(serverId, length).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.RescueServerAsync" />
        public static string RescueServer(this ComputeService service, Identifier serverId, RescueServerRequest request = null)
        {
            return service.RescueServerAsync(serverId, request).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.UnrescueServerAsync" />
        public static void UnrescueServer(this ComputeService service, Identifier serverId)
        {
            service.UnrescueServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ResizeServerAsync" />
        public static void ResizeServer(this ComputeService service, Identifier serverId, Identifier flavorId)
        {
            service.ResizeServerAsync(serverId, flavorId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ConfirmResizeServerAsync" />
        public static void ConfirmResizeServer(this ComputeService service, Identifier serverId)
        {
            service.ConfirmResizeServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CancelResizeServerAsync" />
        public static void CancelResizeServer(this ComputeService service, Identifier serverId)
        {
            service.CancelResizeServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServerActionsAsync" />
        public static IEnumerable<ServerActionReference> ListServerActions(this ComputeService service, Identifier serverId)
        {
            return service.ListServerActionsAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetServerActionAsync" />
        public static ServerAction GetServerAction(this ComputeService service, Identifier serverId, Identifier actionId)
        {
            return service.GetServerActionAsync(serverId, actionId).ForceSynchronous();
        }
        #endregion

        #region Flavors
        /// <inheritdoc cref="ComputeService.GetFlavorAsync" />
        public static Flavor GetFlavor(this ComputeService service, Identifier flavorId)
        {
            return service.GetFlavorAsync(flavorId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListFlavorsAsync" />
        public static IEnumerable<FlavorReference> ListFlavors(this ComputeService service)
        {
            return service.ListFlavorsAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListFlavorDetailsAsync" />
        public static IEnumerable<Flavor> ListFlavorDetails(this ComputeService service)
        {
            return service.ListFlavorDetailsAsync().ForceSynchronous();
        }
        #endregion

        #region Images
        /// <inheritdoc cref="ComputeService.GetImageAsync" />
        public static Image GetImage(this ComputeService service, Identifier imageId)
        {
            return service.GetImageAsync(imageId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetImageAsync" />
        public static ImageMetadata GetImageMetadata(this ComputeService service, Identifier imageId)
        {
            return service.GetImageMetadataAsync(imageId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetImageMetadataItemAsync" />
        public static string GetImageMetadataItem(this ComputeService service, Identifier imageId, string key)
        {
            return service.GetImageMetadataItemAsync(imageId, key).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.WaitForImageStatusAsync" />
        public static void WaitForImageStatus(this ComputeService service, Identifier imageId, ImageStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            service.WaitForImageStatusAsync(imageId, status, refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CreateImagMetadataAsync" />
        public static void CreateImagMetadata(this ComputeService service, Identifier imageId, string key, string value)
        {
            service.CreateImagMetadataAsync(imageId, key, value).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListImagesAsync" />
        public static IPage<ImageReference> ListImages(this ComputeService service, ImageListOptions options = null)
        {
            return service.ListImagesAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListImageDetailsAsync" />
        public static IPage<Image> ListImageDetails(this ComputeService service, ImageListOptions options = null)
        {
            return service.ListImageDetailsAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.UpdateImageMetadataAsync" />
        public static ImageMetadata UpdateImageMetadata(this ComputeService service, Identifier imageId, ImageMetadata metadata, bool overwrite = false)
        {
            return service.UpdateImageMetadataAsync(imageId, metadata, overwrite).ForceSynchronous();
        }
        
        /// <inheritdoc cref="ComputeService.DeleteImageAsync" />
        public static void DeleteImage(this ComputeService service, Identifier imageId)
        {
            service.DeleteImageAsync(imageId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.DeleteImageMetadataAsync" />
        public static void DeleteImageMetadata(this ComputeService service, Identifier imageId, string key)
        {
            service.DeleteImageMetadataAsync(imageId, key).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.WaitUntilImageIsDeletedAsync" />
        public static void WaitUntilImageIsDeleted(this ComputeService service, Identifier imageId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            service.WaitUntilImageIsDeletedAsync(imageId, refreshDelay, timeout, progress).ForceSynchronous();
        }
        #endregion

        #region IP Addresses

        /// <inheritdoc cref="ComputeService.GetServerAddressAsync" />
        public static IList<ServerAddress> GetServerAddress(this ComputeService service, Identifier serverId, string key)
        {
            return service.GetServerAddressAsync(serverId, key).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServerAddressesAsync" />
        public static IDictionary<string, IList<ServerAddress>> ListServerAddresses(this ComputeService service, Identifier serverId)
        {
            return service.ListServerAddressesAsync(serverId).ForceSynchronous();
        }

        #endregion

        #region Volumes
        /// <inheritdoc cref="ComputeService.GetServerVolumeAsync" />
        public static ServerVolume GetServerVolume(this ComputeService service, Identifier serverId, Identifier volumeId)
        {
            return service.GetServerVolumeAsync(serverId, volumeId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServerVolumesAsync" />
        public static IEnumerable<ServerVolume> ListServerVolumes(this ComputeService service, Identifier serverId)
        {
            return service.ListServerVolumesAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.AttachVolumeAsync" />
        public static void AttachVolume(this ComputeService service, Identifier serverId, ServerVolumeDefinition volume)
        {
            service.AttachVolumeAsync(serverId, volume).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.DetachVolumeAsync" />
        public static void DetachVolume(this ComputeService service, Identifier serverId, Identifier volumeId)
        {
            service.DetachVolumeAsync(serverId, volumeId).ForceSynchronous();
        }
        #endregion

        #region KeyPairs
        /// <inheritdoc cref="ComputeService.CreateKeyPairAsync" />
        public static KeyPair CreateKeyPair(this ComputeService service, string name)
        {
            return service.CreateKeyPairAsync(name).ForceSynchronous();
        }
        #endregion

        #region Security Groups

        /// <inheritdoc cref="ComputeService.GetSecurityGroupAsync" />
        public static SecurityGroup GetSecurityGroup(this ComputeService service, Identifier securityGroupId)
        {
            return service.GetSecurityGroupAsync(securityGroupId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CreateSecurityGroupAsync" />
        public static SecurityGroup CreateSecurityGroup(this ComputeService service, SecurityGroupDefinition securityGroup)
        {
            return service.CreateSecurityGroupAsync(securityGroup).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListSecurityGroupsAsync" />
        public static IEnumerable<SecurityGroup> ListSecurityGroups(this ComputeService service, Identifier serverId = null)
        {
            return service.ListSecurityGroupsAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.UpdateSecurityGroupAsync" />
        public static SecurityGroup UpdateSecurityGroup(this ComputeService service, Identifier securityGroupid, SecurityGroupDefinition securityGroup)
        {
            return service.UpdateSecurityGroupAsync(securityGroupid, securityGroup).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.DeleteSecurityGroupAsync" />
        public static void DeleteSecurityGroup(this ComputeService service, Identifier securityGroupId)
        {
            service.DeleteSecurityGroupAsync(securityGroupId).ForceSynchronous();
        }

        #endregion

        #region Compute Service
        /// <inheritdoc cref="ComputeService.GetLimitsAsync" />
        public static ServiceLimits GetLimits(this ComputeService service)
        {
            return service.GetLimitsAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetCurrentQuotasAsync" />
        public static ServiceQuotas GetCurrentQuotas(this ComputeService service)
        {
            return service.GetCurrentQuotasAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetDefaultQuotasAsync" />
        public static ServiceQuotas GetDefaultQuotas(this ComputeService service)
        {
            return service.GetDefaultQuotasAsync().ForceSynchronous();
        }
        #endregion
    }
}
