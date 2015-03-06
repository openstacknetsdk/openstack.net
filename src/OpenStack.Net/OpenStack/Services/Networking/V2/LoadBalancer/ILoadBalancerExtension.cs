namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILoadBalancerExtension
    {
        Task<LoadBalancerSupportedApiCall> PrepareLoadBalancerSupportedAsync(CancellationToken cancellationToken);

        Task<AddVirtualAddressApiCall> PrepareAddVirtualAddressAsync(VirtualAddressRequest request, CancellationToken cancellationToken);

        Task<ListVirtualAddressesApiCall> PrepareListVirtualAddressesAsync(CancellationToken cancellationToken);

        Task<GetVirtualAddressApiCall> PrepareGetVirtualAddressAsync(VirtualAddressId virtualAddressId, CancellationToken cancellationToken);

        Task<UpdateVirtualAddressApiCall> PrepareUpdateVirtualAddressAsync(VirtualAddressId virtualAddressId, VirtualAddressRequest request, CancellationToken cancellationToken);

        Task<RemoveVirtualAddressApiCall> PrepareRemoveVirtualAddressAsync(VirtualAddressId virtualAddressId, CancellationToken cancellationToken);

        Task<AddHealthMonitorApiCall> PrepareAddHealthMonitorAsync(HealthMonitorRequest request, CancellationToken cancellationToken);

        Task<ListHealthMonitorsApiCall> PrepareListHealthMonitorsAsync(CancellationToken cancellationToken);

        Task<GetHealthMonitorApiCall> PrepareGetHealthMonitorAsync(HealthMonitorId healthMonitorId, CancellationToken cancellationToken);

        Task<UpdateHealthMonitorApiCall> PrepareUpdateHealthMonitorAsync(HealthMonitorId healthMonitorId, HealthMonitorRequest request, CancellationToken cancellationToken);

        Task<RemoveHealthMonitorApiCall> PrepareRemoveHealthMonitorAsync(HealthMonitorId healthMonitorId, CancellationToken cancellationToken);

        Task<AddPoolApiCall> PrepareAddPoolAsync(PoolRequest request, CancellationToken cancellationToken);

        Task<ListPoolsApiCall> PrepareListPoolsAsync(CancellationToken cancellationToken);

        Task<GetPoolApiCall> PrepareGetPoolAsync(PoolId poolId, CancellationToken cancellationToken);

        Task<UpdatePoolApiCall> PrepareUpdatePoolAsync(PoolId poolId, PoolRequest request, CancellationToken cancellationToken);

        Task<RemovePoolApiCall> PrepareRemovePoolAsync(PoolId poolId, CancellationToken cancellationToken);

        Task<AddPoolHealthMonitorApiCall> PrepareAddPoolHealthMonitorAsync(PoolId poolId, PoolHealthMonitorRequest request, CancellationToken cancellationToken);

        Task<RemovePoolHealthMonitorApiCall> PrepareRemovePoolHealthMonitorAsync(PoolId poolId, HealthMonitorId healthMonitorId, CancellationToken cancellationToken);

        Task<AddMemberApiCall> PrepareAddMemberAsync(MemberRequest request, CancellationToken cancellationToken);

        Task<ListMembersApiCall> PrepareListMembersAsync(CancellationToken cancellationToken);

        Task<GetMemberApiCall> PrepareGetMemberAsync(MemberId memberId, CancellationToken cancellationToken);

        Task<UpdateMemberApiCall> PrepareUpdateMemberAsync(MemberId memberId, MemberRequest request, CancellationToken cancellationToken);

        Task<RemoveMemberApiCall> PrepareRemoveMemberAsync(MemberId memberId, CancellationToken cancellationToken);
    }
}
