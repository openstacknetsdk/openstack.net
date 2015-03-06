namespace OpenStack.Services.Networking.V2.Layer3
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

    public class Layer3Extension : ServiceExtension<INetworkingService>, ILayer3Extension
    {
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("router");

        public Layer3Extension(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        public Task<Layer3SupportedApiCall> PrepareLayer3SupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareListExtensionsAsync(cancellationToken)
                .Select(task => new Layer3SupportedApiCall(task.Result));
        }

        public Task<AddRouterApiCall> PrepareAddRouterAsync(RouterRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddRouterApiCall(factory.CreateJsonApiCall<RouterResponse>(task.Result)));
        }

        public Task<ListRoutersApiCall> PrepareListRoutersAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Router>>> deserializeResult =
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

                                JToken routersToken = jsonObject["routers"];
                                if (routersToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                Router[] routers = routersToken.ToObject<Router[]>();
                                ReadOnlyCollectionPage<Router> result = new BasicReadOnlyCollectionPage<Router>(routers, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListRoutersApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetRouterApiCall> PrepareGetRouterAsync(RouterId routerId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers/{router_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "router_id", routerId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetRouterApiCall(factory.CreateJsonApiCall<RouterResponse>(task.Result)));
        }

        public Task<UpdateRouterApiCall> PrepareUpdateRouterAsync(RouterId routerId, RouterRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers/{router_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "router_id", routerId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateRouterApiCall(factory.CreateJsonApiCall<RouterResponse>(task.Result)));
        }

        public Task<RemoveRouterApiCall> PrepareRemoveRouterAsync(RouterId routerId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers/{router_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "router_id", routerId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveRouterApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddRouterInterfaceApiCall> PrepareAddRouterInterfaceAsync(RouterId routerId, AddRouterInterfaceRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers/{router_id}/add_router_interface");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "router_id", routerId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new AddRouterInterfaceApiCall(factory.CreateJsonApiCall<AddRouterInterfaceResponse>(task.Result)));
        }

        public Task<RemoveRouterInterfaceApiCall> PrepareRemoveRouterInterfaceAsync(RouterId routerId, RemoveRouterInterfaceRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("routers/{router_id}/remove_router_interface");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "router_id", routerId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new RemoveRouterInterfaceApiCall(factory.CreateJsonApiCall<RemoveRouterInterfaceResponse>(task.Result)));
        }

        public Task<AddFloatingIpApiCall> PrepareAddFloatingIpAsync(FloatingIpRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("floatingips");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddFloatingIpApiCall(factory.CreateJsonApiCall<FloatingIpResponse>(task.Result)));
        }

        public Task<ListFloatingIpsApiCall> PrepareListFloatingIpsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("floatingips");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<FloatingIp>>> deserializeResult =
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

                                JToken floatingIpsToken = jsonObject["floatingips"];
                                if (floatingIpsToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                FloatingIp[] floatingIps = floatingIpsToken.ToObject<FloatingIp[]>();
                                ReadOnlyCollectionPage<FloatingIp> result = new BasicReadOnlyCollectionPage<FloatingIp>(floatingIps, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListFloatingIpsApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetFloatingIpApiCall> PrepareGetFloatingIpAsync(FloatingIpId floatingIpId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("floatingips/{floatingip_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "floatingip_id", floatingIpId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetFloatingIpApiCall(factory.CreateJsonApiCall<FloatingIpResponse>(task.Result)));
        }

        public Task<UpdateFloatingIpApiCall> PrepareUpdateFloatingIpAsync(FloatingIpId floatingIpId, FloatingIpRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("floatingips/{floatingip_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "floatingip_id", floatingIpId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateFloatingIpApiCall(factory.CreateJsonApiCall<FloatingIpResponse>(task.Result)));
        }

        public Task<RemoveFloatingIpApiCall> PrepareRemoveFloatingIpAsync(FloatingIpId floatingIpId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("floatingips/{floatingip_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "floatingip_id", floatingIpId.Value }
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveFloatingIpApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }
    }
}
