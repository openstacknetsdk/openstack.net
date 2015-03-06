namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using Rackspace.Threading;

    public static class LoadBalancerExtensions
    {
        public static Task<bool> SupportsLoadBalancerAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareLoadBalancerSupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<VirtualAddress> AddVirtualAddressAsync(this INetworkingService service, VirtualAddressData virtualAddressData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareAddVirtualAddressAsync(new VirtualAddressRequest(virtualAddressData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.VirtualAddress));
        }

        public static Task<ReadOnlyCollectionPage<VirtualAddress>> ListVirtualAddressesAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                    () => extension.PrepareListVirtualAddressesAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<VirtualAddress> GetVirtualAddressAsync(this INetworkingService service, VirtualAddressId virtualAddressId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareGetVirtualAddressAsync(virtualAddressId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.VirtualAddress));
        }

        public static Task<VirtualAddress> UpdateVirtualAddressAsync(this INetworkingService service, VirtualAddressId virtualAddressId, VirtualAddressData virtualAddressData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateVirtualAddressAsync(virtualAddressId, new VirtualAddressRequest(virtualAddressData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.VirtualAddress));
        }

        public static Task RemoveVirtualAddressAsync(this INetworkingService service, VirtualAddressId virtualAddressId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveVirtualAddressAsync(virtualAddressId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<HealthMonitor> AddHealthMonitorAsync(this INetworkingService service, HealthMonitorData healthMonitorData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareAddHealthMonitorAsync(new HealthMonitorRequest(healthMonitorData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.HealthMonitor));
        }

        public static Task<ReadOnlyCollectionPage<HealthMonitor>> ListHealthMonitorsAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareListHealthMonitorsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<HealthMonitor> GetHealthMonitorAsync(this INetworkingService service, HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareGetHealthMonitorAsync(healthMonitorId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.HealthMonitor));
        }

        public static Task<HealthMonitor> UpdateHealthMonitorAsync(this INetworkingService service, HealthMonitorId healthMonitorId, HealthMonitorData healthMonitorData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateHealthMonitorAsync(healthMonitorId, new HealthMonitorRequest(healthMonitorData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.HealthMonitor));
        }

        public static Task RemoveHealthMonitorAsync(this INetworkingService service, HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveHealthMonitorAsync(healthMonitorId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<Pool> AddPoolAsync(this INetworkingService service, PoolData poolData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareAddPoolAsync(new PoolRequest(poolData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Pool));
        }

        public static Task<ReadOnlyCollectionPage<Pool>> ListPoolsAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareListPoolsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Pool> GetPoolAsync(this INetworkingService service, PoolId poolId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareGetPoolAsync(poolId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Pool));
        }

        public static Task<Pool> UpdatePoolAsync(this INetworkingService service, PoolId poolId, PoolData poolData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareUpdatePoolAsync(poolId, new PoolRequest(poolData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Pool));
        }

        public static Task RemovePoolAsync(this INetworkingService service, PoolId poolId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareRemovePoolAsync(poolId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<HealthMonitor> AddPoolHealthMonitorAsync(this INetworkingService service, PoolId poolId, HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareAddPoolHealthMonitorAsync(poolId, new PoolHealthMonitorRequest(new PoolHealthMonitorRequest.HealthMonitorData(healthMonitorId)), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.HealthMonitor));
        }

        public static Task RemovePoolHealthMonitorAsync(this INetworkingService service, PoolId poolId, HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareRemovePoolHealthMonitorAsync(poolId, healthMonitorId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<Member> AddMemberAsync(this INetworkingService service, MemberData memberData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareAddMemberAsync(new MemberRequest(memberData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Member));
        }

        public static Task<ReadOnlyCollectionPage<Member>> ListMembersAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareListMembersAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Member> GetMemberAsync(this INetworkingService service, MemberId memberId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareGetMemberAsync(memberId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Member));
        }

        public static Task<Member> UpdateMemberAsync(this INetworkingService service, MemberId memberId, MemberData memberData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateMemberAsync(memberId, new MemberRequest(memberData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Member));
        }

        public static Task RemoveMemberAsync(this INetworkingService service, MemberId memberId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ILoadBalancerExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.LoadBalancer);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveMemberAsync(memberId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }
    }
}
