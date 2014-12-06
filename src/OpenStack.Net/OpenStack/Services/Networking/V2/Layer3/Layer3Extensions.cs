namespace OpenStack.Services.Networking.V2.Layer3
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Networking.V2.Quotas;
    using Rackspace.Threading;

    public static class Layer3Extensions
    {
        public static Task<bool> SupportsLayer3Async(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareLayer3SupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Router> AddRouterAsync(this INetworkingService service, RouterData routerData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareAddRouterAsync(new RouterRequest(routerData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Router));
        }

        public static Task<ReadOnlyCollectionPage<Router>> ListRoutersAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareListRoutersAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Router> GetRouterAsync(this INetworkingService service, RouterId routerId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareGetRouterAsync(routerId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Router));
        }

        public static Task<Router> UpdateRouterAsync(this INetworkingService service, RouterId routerId, RouterData routerData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateRouterAsync(routerId, new RouterRequest(routerData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Router));
        }

        public static Task RemoveRouterAsync(this INetworkingService service, RouterId routerId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveRouterAsync(routerId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<AddRouterInterfaceResponse> AddRouterInterfaceAsync(this INetworkingService service, RouterId routerId, SubnetId subnetId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareAddRouterInterfaceAsync(routerId, new AddRouterInterfaceRequest(subnetId), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<RemoveRouterInterfaceResponse> RemoveRouterInterfaceAsync(this INetworkingService service, RouterId routerId, SubnetId subnetId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveRouterInterfaceAsync(routerId, new RemoveRouterInterfaceRequest(subnetId), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<FloatingIp> AddFloatingIpAsync(this INetworkingService service, FloatingIpData floatingIpData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareAddFloatingIpAsync(new FloatingIpRequest(floatingIpData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.FloatingIp));
        }

        public static Task<ReadOnlyCollectionPage<FloatingIp>> ListFloatingIpsAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareListFloatingIpsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<FloatingIp> GetFloatingIpAsync(this INetworkingService service, FloatingIpId floatingIpId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareGetFloatingIpAsync(floatingIpId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.FloatingIp));
        }

        public static Task<FloatingIp> UpdateFloatingIpAsync(this INetworkingService service, FloatingIpId floatingIpId, FloatingIpData floatingIpData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateFloatingIpAsync(floatingIpId, new FloatingIpRequest(floatingIpData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.FloatingIp));
        }

        public static Task RemoveFloatingIpAsync(this INetworkingService service, FloatingIpId floatingIpId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILayer3Extension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Layer3);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveFloatingIpAsync(floatingIpId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static int? GetRouterQuota(this QuotaData quotaData)
        {
            JToken value;
            if (!quotaData.ExtensionData.TryGetValue("router", out value) || value == null)
                return null;

            return value.ToObject<int?>();
        }

        public static QuotaData WithRouterQuota(this QuotaData quotaData, int routerQuota)
        {
            var extensionData = quotaData.ExtensionData.SetItem("router", JToken.FromObject(routerQuota));
            return quotaData.WithExtensionData(extensionData);
        }

        public static int? GetFloatingIpQuota(this QuotaData quotaData)
        {
            JToken value;
            if (!quotaData.ExtensionData.TryGetValue("floatingip", out value) || value == null)
                return null;

            return value.ToObject<int?>();
        }

        public static QuotaData WithFloatingIpQuota(this QuotaData quotaData, int routerQuota)
        {
            var extensionData = quotaData.ExtensionData.SetItem("floatingip", JToken.FromObject(routerQuota));
            return quotaData.WithExtensionData(extensionData);
        }
    }
}
