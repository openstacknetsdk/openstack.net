namespace OpenStack.Services.Networking.V2
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
    using OpenStack.Security.Authentication;
    using Rackspace.Net;
    using Rackspace.Threading;
    using Extension = OpenStack.Services.Identity.V2.Extension;
    using ExtensionAlias = OpenStack.Services.Identity.V2.ExtensionAlias;
    using ExtensionResponse = OpenStack.Services.Identity.V2.ExtensionResponse;
    using GetExtensionApiCall = OpenStack.Services.Identity.V2.GetExtensionApiCall;
    using Link = OpenStack.Services.Identity.Link;
    using ListExtensionsApiCall = OpenStack.Services.Identity.V2.ListExtensionsApiCall;

    /// <summary>
    /// This class provides a default implementation of <see cref="INetworkingService"/> suitable for connecting
    /// to OpenStack-compatible installations of the Networking Service V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class NetworkingClient : ServiceClient, INetworkingService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkingClient"/> class with the specified
        /// authentication service, default region, and value indicating whether an internal or public
        /// endpoint should be used for communicating with the service.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests
        /// made to this service.</param>
        /// <param name="defaultRegion">The preferred region for the service. Unless otherwise specified for a
        /// specific client, derived service clients will not use a default region if this value is
        /// <see langword="null"/> (i.e. only region-less or "global" service endpoints will be considered
        /// acceptable).</param>
        /// <param name="internalUrl"><see langword="true"/> to access the service over a local network;
        /// otherwise, <see langword="false"/> to access the service over a public network (the
        /// Internet).</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="authenticationService"/> is <see langword="null"/>.
        /// </exception>
        public NetworkingClient(IAuthenticationService authenticationService, string defaultRegion, bool internalUrl)
            : base(authenticationService, defaultRegion, internalUrl)
        {
        }

        #region API Version

        /// <inheritdoc/>
        public virtual Task<ListApiVersionsApiCall> PrepareListApiVersionsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("/");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<ApiVersion>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                if (string.IsNullOrEmpty(innerTask.Result))
                                    return null;

                                JObject responseObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray versionsArray = responseObject["versions"] as JArray;
                                if (versionsArray == null)
                                    return null;

                                IList<ApiVersion> list = versionsArray.ToObject<ApiVersion[]>();
                                // according to the available documentation, this call does not appear to be paginated
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<ApiVersion>>>> prepareGetNextPageAsync = null;

                                ReadOnlyCollectionPage<ApiVersion> results = new BasicReadOnlyCollectionPage<ApiVersion>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListApiVersionsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetApiDetailsApiCall> PrepareGetApiDetailsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate(string.Empty);
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetApiDetailsApiCall(CreateJsonApiCall<ApiDetails>(task.Result)));
        }

        #endregion

        #region Networks

        /// <inheritdoc/>
        public virtual Task<AddNetworkApiCall> PrepareAddNetworkAsync(NetworkRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("networks");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddNetworkApiCall(CreateJsonApiCall<NetworkResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListNetworksApiCall> PrepareListNetworksAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("networks");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Network>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                if (string.IsNullOrEmpty(innerTask.Result))
                                    return null;

                                JObject responseObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray networksArray = responseObject["networks"] as JArray;
                                if (networksArray == null)
                                    return null;

                                IList<Network> list = networksArray.ToObject<Network[]>();
                                // according to the available documentation, this call does not appear to be paginated
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<Network>>>> prepareGetNextPageAsync = null;

                                ReadOnlyCollectionPage<Network> results = new BasicReadOnlyCollectionPage<Network>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListNetworksApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetNetworkApiCall> PrepareGetNetworkAsync(NetworkId networkId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("networks/{network_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "network_id", networkId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetNetworkApiCall(CreateJsonApiCall<NetworkResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<UpdateNetworkApiCall> PrepareUpdateNetworkAsync(NetworkId networkId, NetworkRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("networks/{network_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "network_id", networkId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateNetworkApiCall(CreateJsonApiCall<NetworkResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<RemoveNetworkApiCall> PrepareRemoveNetworkAsync(NetworkId networkId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("networks/{network_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "network_id", networkId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveNetworkApiCall(CreateBasicApiCall(task.Result)));
        }

        #endregion

        #region Subnets

        /// <inheritdoc/>
        public virtual Task<AddSubnetApiCall> PrepareAddSubnetAsync(SubnetRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("subnets");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddSubnetApiCall(CreateJsonApiCall<SubnetResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListSubnetsApiCall> PrepareListSubnetsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("subnets");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Subnet>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                if (string.IsNullOrEmpty(innerTask.Result))
                                    return null;

                                JObject responseObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray subnetsArray = responseObject["subnets"] as JArray;
                                if (subnetsArray == null)
                                    return null;

                                IList<Subnet> list = subnetsArray.ToObject<Subnet[]>();
                                // according to the available documentation, this call does not appear to be paginated
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<Subnet>>>> prepareGetNextPageAsync = null;

                                ReadOnlyCollectionPage<Subnet> results = new BasicReadOnlyCollectionPage<Subnet>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListSubnetsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetSubnetApiCall> PrepareGetSubnetAsync(SubnetId subnetId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("subnets/{subnet_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "subnet_id", subnetId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetSubnetApiCall(CreateJsonApiCall<SubnetResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<UpdateSubnetApiCall> PrepareUpdateSubnetAsync(SubnetId subnetId, SubnetRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("subnets/{subnet_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "subnet_id", subnetId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateSubnetApiCall(CreateJsonApiCall<SubnetResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<RemoveSubnetApiCall> PrepareRemoveSubnetAsync(SubnetId subnetId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("subnets/{subnet_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "subnet_id", subnetId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveSubnetApiCall(CreateBasicApiCall(task.Result)));
        }

        #endregion

        #region Ports

        /// <inheritdoc/>
        public virtual Task<AddPortApiCall> PrepareAddPortAsync(PortRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("ports");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddPortApiCall(CreateJsonApiCall<PortResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListPortsApiCall> PrepareListPortsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("ports");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Port>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                if (string.IsNullOrEmpty(innerTask.Result))
                                    return null;

                                JObject responseObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray portsArray = responseObject["ports"] as JArray;
                                if (portsArray == null)
                                    return null;

                                IList<Port> list = portsArray.ToObject<Port[]>();
                                // according to the available documentation, this call does not appear to be paginated
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<Port>>>> prepareGetNextPageAsync = null;

                                ReadOnlyCollectionPage<Port> results = new BasicReadOnlyCollectionPage<Port>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListPortsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetPortApiCall> PrepareGetPortAsync(PortId portId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("ports/{port_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "port_id", portId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetPortApiCall(CreateJsonApiCall<PortResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<UpdatePortApiCall> PrepareUpdatePortAsync(PortId portId, PortRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("ports/{port_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "port_id", portId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdatePortApiCall(CreateJsonApiCall<PortResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<RemovePortApiCall> PrepareRemovePortAsync(PortId portId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("ports/{port_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "port_id", portId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemovePortApiCall(CreateBasicApiCall(task.Result)));
        }

        #endregion

        #region Extensions

        /// <inheritdoc/>
        public virtual Task<ListExtensionsApiCall> PrepareListExtensionsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("extensions");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Extension>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                if (string.IsNullOrEmpty(innerTask.Result))
                                    return null;

                                JObject responseObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray extensionsArray = responseObject["extensions"] as JArray;
                                if (extensionsArray == null)
                                    return null;

                                IList<Extension> list = extensionsArray.ToObject<Extension[]>();
                                // http://docs.openstack.org/api/openstack-identity-service/2.0/content/Paginated_Collections-d1e325.html
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<Extension>>>> prepareGetNextPageAsync = null;
                                JArray extensionsLinksArray = responseObject["extensions_links"] as JArray;
                                if (extensionsLinksArray != null)
                                {
                                    IList<Link> extensionsLinks = extensionsLinksArray.ToObject<Link[]>();
                                    Link nextLink = extensionsLinks.FirstOrDefault(i => string.Equals("next", i.Relation, StringComparison.OrdinalIgnoreCase));
                                    if (nextLink != null)
                                    {
                                        prepareGetNextPageAsync =
                                            nextCancellationToken =>
                                            {
                                                return PrepareListExtensionsAsync(nextCancellationToken)
                                                    .WithUri(nextLink.Target)
                                                    .Select(_ => _.Result.AsHttpApiCall());
                                            };
                                    }
                                }

                                ReadOnlyCollectionPage<Extension> results = new BasicReadOnlyCollectionPage<Extension>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListExtensionsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetExtensionApiCall> PrepareGetExtensionAsync(ExtensionAlias alias, CancellationToken cancellationToken)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            UriTemplate template = new UriTemplate("extensions/{alias}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "alias", alias.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetExtensionApiCall(CreateJsonApiCall<ExtensionResponse>(task.Result)));
        }

        #endregion

        /// <inheritdoc/>
        public virtual TExtension GetServiceExtension<TExtension>(ServiceExtensionDefinition<INetworkingService, TExtension> definition)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            return definition.CreateDefaultInstance(this, this);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>This method calls <see cref="IAuthenticationService.GetBaseAddressAsync"/> to obtain a URI for the
        /// type <c>network</c>. The preferred name is not specified.</para>
        /// </remarks>
        protected override Task<Uri> GetBaseUriAsyncImpl(CancellationToken cancellationToken)
        {
            const string serviceType = "network";
            const string serviceName = null;
            return AuthenticationService.GetBaseAddressAsync(serviceType, serviceName, DefaultRegion, InternalUrl, cancellationToken);
        }
    }
}
