using System;
using System.Collections.Generic;
using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;
using Console = OpenStack.Compute.v2_1.Console;

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

        /// <inheritdoc cref="ComputeService.WaitUntilServerIsActiveAsync" />
        public static Server WaitUntilServerIsActive(this ComputeService service, Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            return service.WaitUntilServerIsActiveAsync(serverId, refreshDelay, timeout, progress).ForceSynchronous();
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

        /// <inheritdoc cref="ComputeService.GetVncConsoleAync" />
        public static Console GetVncConsole(this ComputeService service, Identifier serverId, ConsoleType type)
        {
            return service.GetVncConsoleAync(serverId, type).ForceSynchronous();
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

        /// <inheritdoc cref="ComputeService.WaitUntilImageIsActiveAsync" />
        public static void WaitUntilImageIsActive(this ComputeService service, Identifier imageId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            service.WaitUntilImageIsActiveAsync(imageId, refreshDelay, timeout, progress).ForceSynchronous();
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

        #region KeyPairs
        /// <inheritdoc cref="ComputeService.CreateKeyPairAsync" />
        public static KeyPair CreateKeyPair(this ComputeService service, string name)
        {
            return service.CreateKeyPairAsync(name).ForceSynchronous();
        }
        #endregion
    }
}
