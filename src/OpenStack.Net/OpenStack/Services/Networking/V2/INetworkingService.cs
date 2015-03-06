namespace OpenStack.Services.Networking.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using ExtensionAlias = Identity.V2.ExtensionAlias;
    using GetExtensionApiCall = Identity.V2.GetExtensionApiCall;
    using ListExtensionsApiCall = Identity.V2.ListExtensionsApiCall;

    public interface INetworkingService : IHttpService, IExtensibleService<INetworkingService>
    {
        #region API Version

        Task<ListApiVersionsApiCall> PrepareListApiVersionsAsync(CancellationToken cancellationToken);

        Task<GetApiDetailsApiCall> PrepareGetApiDetailsAsync(CancellationToken cancellationToken);

        #endregion

        #region Networks

        Task<AddNetworkApiCall> PrepareAddNetworkAsync(NetworkRequest request, CancellationToken cancellationToken);

        Task<ListNetworksApiCall> PrepareListNetworksAsync(CancellationToken cancellationToken);

        Task<GetNetworkApiCall> PrepareGetNetworkAsync(NetworkId networkId, CancellationToken cancellationToken);

        Task<UpdateNetworkApiCall> PrepareUpdateNetworkAsync(NetworkId networkId, NetworkRequest request, CancellationToken cancellationToken);

        Task<RemoveNetworkApiCall> PrepareRemoveNetworkAsync(NetworkId networkId, CancellationToken cancellationToken);

        #endregion

        #region Subnets

        Task<AddSubnetApiCall> PrepareAddSubnetAsync(SubnetRequest request, CancellationToken cancellationToken);

        Task<ListSubnetsApiCall> PrepareListSubnetsAsync(CancellationToken cancellationToken);

        Task<GetSubnetApiCall> PrepareGetSubnetAsync(SubnetId subnetId, CancellationToken cancellationToken);

        Task<UpdateSubnetApiCall> PrepareUpdateSubnetAsync(SubnetId subnetId, SubnetRequest request, CancellationToken cancellationToken);

        Task<RemoveSubnetApiCall> PrepareRemoveSubnetAsync(SubnetId subnetId, CancellationToken cancellationToken);

        #endregion

        #region Ports

        Task<AddPortApiCall> PrepareAddPortAsync(PortRequest request, CancellationToken cancellationToken);

        Task<ListPortsApiCall> PrepareListPortsAsync(CancellationToken cancellationToken);

        Task<GetPortApiCall> PrepareGetPortAsync(PortId portId, CancellationToken cancellationToken);

        Task<UpdatePortApiCall> PrepareUpdatePortAsync(PortId portId, PortRequest request, CancellationToken cancellationToken);

        Task<RemovePortApiCall> PrepareRemovePortAsync(PortId portId, CancellationToken cancellationToken);

        #endregion

        #region Extensions

        Task<ListExtensionsApiCall> PrepareListExtensionsAsync(CancellationToken cancellationToken);

        Task<GetExtensionApiCall> PrepareGetExtensionAsync(ExtensionAlias alias, CancellationToken cancellationToken);

        #endregion
    }
}
