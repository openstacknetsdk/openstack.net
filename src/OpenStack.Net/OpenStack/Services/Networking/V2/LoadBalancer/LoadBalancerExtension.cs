namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.Net;
    using Rackspace.Net;
    using Rackspace.Threading;
    using ExtensionAlias = Identity.V2.ExtensionAlias;

    public class LoadBalancerExtension : ServiceExtension<INetworkingService>, ILoadBalancerExtension
    {
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("lbaas");

        public LoadBalancerExtension(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        public Task<LoadBalancerSupportedApiCall> PrepareLoadBalancerSupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareListExtensionsAsync(cancellationToken)
                .Select(task => new LoadBalancerSupportedApiCall(task.Result));
        }

        public Task<AddVirtualAddressApiCall> PrepareAddVirtualAddressAsync(VirtualAddressRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/vips");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddVirtualAddressApiCall(factory.CreateJsonApiCall<VirtualAddressResponse>(task.Result)));
        }

        public Task<ListVirtualAddressesApiCall> PrepareListVirtualAddressesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/vips");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<VirtualAddress>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            content =>
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content.Result);
                                if (jsonObject == null)
                                    return null;

                                JToken virtualAddressesToken = jsonObject["vips"];
                                if (virtualAddressesToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                VirtualAddress[] virtualAddresses = virtualAddressesToken.ToObject<VirtualAddress[]>();
                                ReadOnlyCollectionPage<VirtualAddress> result = new BasicReadOnlyCollectionPage<VirtualAddress>(virtualAddresses, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListVirtualAddressesApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetVirtualAddressApiCall> PrepareGetVirtualAddressAsync(VirtualAddressId virtualAddressId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/vips/{vip_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "vip_id", virtualAddressId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetVirtualAddressApiCall(factory.CreateJsonApiCall<VirtualAddressResponse>(task.Result)));
        }

        public Task<UpdateVirtualAddressApiCall> PrepareUpdateVirtualAddressAsync(VirtualAddressId virtualAddressId, VirtualAddressRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/vips/{vip_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "vip_id", virtualAddressId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateVirtualAddressApiCall(factory.CreateJsonApiCall<VirtualAddressResponse>(task.Result)));
        }

        public Task<RemoveVirtualAddressApiCall> PrepareRemoveVirtualAddressAsync(VirtualAddressId virtualAddressId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/vips/{vip_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "vip_id", virtualAddressId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveVirtualAddressApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddHealthMonitorApiCall> PrepareAddHealthMonitorAsync(HealthMonitorRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/healthmonitors");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddHealthMonitorApiCall(factory.CreateJsonApiCall<HealthMonitorResponse>(task.Result)));
        }

        public Task<ListHealthMonitorsApiCall> PrepareListHealthMonitorsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/healthmonitors");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<HealthMonitor>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            content =>
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content.Result);
                                if (jsonObject == null)
                                    return null;

                                JToken healthMonitorsToken = jsonObject["health_monitors"];
                                if (healthMonitorsToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                HealthMonitor[] healthMonitors = healthMonitorsToken.ToObject<HealthMonitor[]>();
                                ReadOnlyCollectionPage<HealthMonitor> result = new BasicReadOnlyCollectionPage<HealthMonitor>(healthMonitors, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListHealthMonitorsApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetHealthMonitorApiCall> PrepareGetHealthMonitorAsync(HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/healthmonitors/{health_monitor_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "health_monitor_id", healthMonitorId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetHealthMonitorApiCall(factory.CreateJsonApiCall<HealthMonitorResponse>(task.Result)));
        }

        public Task<UpdateHealthMonitorApiCall> PrepareUpdateHealthMonitorAsync(HealthMonitorId healthMonitorId, HealthMonitorRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/healthmonitors/{health_monitor_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "health_monitor_id", healthMonitorId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateHealthMonitorApiCall(factory.CreateJsonApiCall<HealthMonitorResponse>(task.Result)));
        }

        public Task<RemoveHealthMonitorApiCall> PrepareRemoveHealthMonitorAsync(HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/healthmonitors/{health_monitor_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "health_monitor_id", healthMonitorId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveHealthMonitorApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddPoolApiCall> PrepareAddPoolAsync(PoolRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddPoolApiCall(factory.CreateJsonApiCall<PoolResponse>(task.Result)));
        }

        public Task<ListPoolsApiCall> PrepareListPoolsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Pool>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            content =>
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content.Result);
                                if (jsonObject == null)
                                    return null;

                                JToken poolsToken = jsonObject["pools"];
                                if (poolsToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                Pool[] pools = poolsToken.ToObject<Pool[]>();
                                ReadOnlyCollectionPage<Pool> result = new BasicReadOnlyCollectionPage<Pool>(pools, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListPoolsApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetPoolApiCall> PrepareGetPoolAsync(PoolId poolId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools/{pool_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "pool_id", poolId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetPoolApiCall(factory.CreateJsonApiCall<PoolResponse>(task.Result)));
        }

        public Task<UpdatePoolApiCall> PrepareUpdatePoolAsync(PoolId poolId, PoolRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools/{pool_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "pool_id", poolId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdatePoolApiCall(factory.CreateJsonApiCall<PoolResponse>(task.Result)));
        }

        public Task<RemovePoolApiCall> PrepareRemovePoolAsync(PoolId poolId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools/{pool_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "pool_id", poolId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemovePoolApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddPoolHealthMonitorApiCall> PrepareAddPoolHealthMonitorAsync(PoolId poolId, PoolHealthMonitorRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools/{pool_id}/health_monitors");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "pool_id", poolId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddPoolHealthMonitorApiCall(factory.CreateJsonApiCall<HealthMonitorResponse>(task.Result)));
        }

        public Task<RemovePoolHealthMonitorApiCall> PrepareRemovePoolHealthMonitorAsync(PoolId poolId, HealthMonitorId healthMonitorId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/pools/{pool_id}/health_monitors/{health_monitor_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "pool_id", poolId.Value }, { "health_monitor_id", healthMonitorId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemovePoolHealthMonitorApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddMemberApiCall> PrepareAddMemberAsync(MemberRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/members");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddMemberApiCall(factory.CreateJsonApiCall<MemberResponse>(task.Result)));
        }

        public Task<ListMembersApiCall> PrepareListMembersAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/members");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Member>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            content =>
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content.Result);
                                if (jsonObject == null)
                                    return null;

                                JToken membersToken = jsonObject["members"];
                                if (membersToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                Member[] members = membersToken.ToObject<Member[]>();
                                ReadOnlyCollectionPage<Member> result = new BasicReadOnlyCollectionPage<Member>(members, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListMembersApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetMemberApiCall> PrepareGetMemberAsync(MemberId memberId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/members/{member_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "member_id", memberId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetMemberApiCall(factory.CreateJsonApiCall<MemberResponse>(task.Result)));
        }

        public Task<UpdateMemberApiCall> PrepareUpdateMemberAsync(MemberId memberId, MemberRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/members/{member_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "member_id", memberId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateMemberApiCall(factory.CreateJsonApiCall<MemberResponse>(task.Result)));
        }

        public Task<RemoveMemberApiCall> PrepareRemoveMemberAsync(MemberId memberId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("lb/members/{member_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "member_id", memberId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveMemberApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }
    }
}
