namespace OpenStack.Services.Networking.V2.Layer3
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILayer3Extension
    {
        Task<Layer3SupportedApiCall> PrepareLayer3SupportedAsync(CancellationToken cancellationToken);

        Task<AddRouterApiCall> PrepareAddRouterAsync(RouterRequest request, CancellationToken cancellationToken);

        Task<ListRoutersApiCall> PrepareListRoutersAsync(CancellationToken cancellationToken);

        Task<GetRouterApiCall> PrepareGetRouterAsync(RouterId routerId, CancellationToken cancellationToken);

        Task<UpdateRouterApiCall> PrepareUpdateRouterAsync(RouterId routerId, RouterRequest request, CancellationToken cancellationToken);

        Task<RemoveRouterApiCall> PrepareRemoveRouterAsync(RouterId routerId, CancellationToken cancellationToken);

        Task<AddRouterInterfaceApiCall> PrepareAddRouterInterfaceAsync(RouterId routerId, AddRouterInterfaceRequest request, CancellationToken cancellationToken);

        Task<RemoveRouterInterfaceApiCall> PrepareRemoveRouterInterfaceAsync(RouterId routerId, RemoveRouterInterfaceRequest request, CancellationToken cancellationToken);

        Task<AddFloatingIpApiCall> PrepareAddFloatingIpAsync(FloatingIpRequest request, CancellationToken cancellationToken);

        Task<ListFloatingIpsApiCall> PrepareListFloatingIpsAsync(CancellationToken cancellationToken);

        Task<GetFloatingIpApiCall> PrepareGetFloatingIpAsync(FloatingIpId floatingIpId, CancellationToken cancellationToken);

        Task<UpdateFloatingIpApiCall> PrepareUpdateFloatingIpAsync(FloatingIpId floatingIpId, FloatingIpRequest request, CancellationToken cancellationToken);

        Task<RemoveFloatingIpApiCall> PrepareRemoveFloatingIpAsync(FloatingIpId floatingIpId, CancellationToken cancellationToken);
    }
}
