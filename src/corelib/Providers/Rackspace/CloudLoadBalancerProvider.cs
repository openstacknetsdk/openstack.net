namespace net.openstack.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using JSIStudios.SimpleRESTServices.Client;
    using net.openstack.Core;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers.Request;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers.Response;
    using net.openstack.Providers.Rackspace.Validators;
    using CancellationToken = System.Threading.CancellationToken;
    using JsonRestServices = JSIStudios.SimpleRESTServices.Client.Json.JsonRestServices;

    /// <summary>
    /// Provides an implementation of <see cref="ILoadBalancerService"/> for operating
    /// with Rackspace's Cloud Load Balancers product.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Overview-d1e82.html">Rackspace Cloud Load Balancers Developer Guide - API v1.0</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class CloudLoadBalancerProvider : ProviderBase<ILoadBalancerService>, ILoadBalancerService
    {
        /// <summary>
        /// This field caches the base URI used for accessing the Cloud Load Balancers service.
        /// </summary>
        /// <seealso cref="GetBaseUriAsync"/>
        private Uri _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudLoadBalancerProvider"/> class with
        /// the specified values.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="defaultRegion">The default region to use for calls that do not explicitly specify a region. If this value is <c>null</c>, the default region for the user will be used; otherwise if the service uses region-specific endpoints all calls must specify an explicit region.</param>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created using <paramref name="defaultIdentity"/> as the default identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        public CloudLoadBalancerProvider(CloudIdentity defaultIdentity, string defaultRegion, IIdentityProvider identityProvider, IRestService restService)
            : base(defaultIdentity, defaultRegion, identityProvider, restService, HttpResponseCodeValidator.Default)
        {
        }

        #region ILoadBalancerService Members

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancer>> ListLoadBalancersAsync(LoadBalancerId markerId, int? limit, CancellationToken cancellationToken)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");

            UriTemplate template = new UriTemplate("/loadbalancers?markerId={markerId}&limit={limit}");
            var parameters = new Dictionary<string, string>();
            if (markerId != null)
                parameters.Add("markerId", markerId.Value);
            if (limit != null)
                parameters.Add("limit", limit.ToString());

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancersResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancersResponse>(cancellationToken);

            Func<Task<ListLoadBalancersResponse>, IEnumerable<LoadBalancer>> resultSelector =
                task => (task.Result != null ? task.Result.LoadBalancers : null) ?? Enumerable.Empty<LoadBalancer>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<LoadBalancer> GetLoadBalancerAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value } };
            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerResponse>(cancellationToken);

            Func<Task<GetLoadBalancerResponse>, LoadBalancer> resultSelector =
                task => task.Result.LoadBalancer;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<LoadBalancer> CreateLoadBalancerAsync(LoadBalancerConfiguration configuration, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            UriTemplate template = new UriTemplate("/loadbalancers");
            var parameters = new Dictionary<string, string>();

            CreateLoadBalancerRequest requestBody = new CreateLoadBalancerRequest(configuration);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.POST, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerResponse>(cancellationToken);

            Func<Task<GetLoadBalancerResponse>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    LoadBalancer loadBalancer = task.Result.LoadBalancer;
                    if (loadBalancer != null && completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancer.Id, LoadBalancerStatus.Build, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(loadBalancer);
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task UpdateLoadBalancerAsync(LoadBalancerId loadBalancerId, LoadBalancerUpdate configuration, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value } };

            UpdateLoadBalancerRequest requestBody = new UpdateLoadBalancerRequest(configuration);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc<string>(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveLoadBalancerAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value } };
            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingDelete, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveLoadBalancerRangeAsync(IEnumerable<LoadBalancerId> loadBalancerIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer[]> progress)
        {
            if (loadBalancerIds == null)
                throw new ArgumentNullException("loadBalancerIds");

            return RemoveLoadBalancerRangeAsync(loadBalancerIds.ToArray(), completionOption, cancellationToken, progress);
        }

        /// <summary>
        /// Removes one or more load balancers.
        /// </summary>
        /// <param name="loadBalancerIds">The IDs of load balancers to remove. These is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="completionOption">Specifies when the <see cref="Task"/> representing the asynchronous server operation should be considered complete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications, if <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. If
        /// <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>,
        /// the task will not be considered complete until all of the load balancers
        /// transition out of the <see cref="LoadBalancerStatus.PendingDelete"/> state.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="loadBalancerIds"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="loadBalancerIds"/> contains any <c>null</c> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="completionOption"/> is not a valid <see cref="AsyncCompletionOption"/>.</para>
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Remove_Load_Balancer-d1e2093.html">Remove Load Balancer (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task RemoveLoadBalancerRangeAsync(LoadBalancerId[] loadBalancerIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer[]> progress)
        {
            if (loadBalancerIds == null)
                throw new ArgumentNullException("loadBalancerIds");
            if (loadBalancerIds.Contains(null))
                throw new ArgumentException("loadBalancerIds cannot contain any null values", "loadBalancerIds");

            if (loadBalancerIds.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask();
            }
            else if (loadBalancerIds.Length == 1)
            {
                IProgress<LoadBalancer> wrapper = null;
                if (progress != null)
                    wrapper = new ArrayElementProgressWrapper<LoadBalancer>(progress);

                return RemoveLoadBalancerAsync(loadBalancerIds[0], completionOption, cancellationToken, wrapper);
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers?id={id}");
                var parameters = new Dictionary<string, string> { { "id", string.Join(",", Array.ConvertAll(loadBalancerIds, i => i.Value)) } };

                Func<Uri, Uri> uriTransform =
                    uri =>
                    {
                        string path = uri.GetLeftPart(UriPartial.Path);
                        string query = uri.Query.Replace(",", "&id=").Replace("%2c", "&id=");
                        return new Uri(path + query);
                    };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters, uriTransform);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                Func<Task<string>, Task<LoadBalancer[]>> resultSelector =
                    task =>
                    {
                        task.PropagateExceptions();
                        if (completionOption == AsyncCompletionOption.RequestCompleted)
                            return WaitForLoadBalancersToLeaveStateAsync(loadBalancerIds, LoadBalancerStatus.PendingDelete, cancellationToken, progress);

                        return InternalTaskExtensions.CompletedTask(default(LoadBalancer[]));
                    };

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap()
                    .ContinueWith(resultSelector).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task<string> GetErrorPageAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/errorpage");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerErrorPageResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerErrorPageResponse>(cancellationToken);

            Func<Task<GetLoadBalancerErrorPageResponse>, string> resultSelector =
                task => task.Result != null ? task.Result.Content : null;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task SetErrorPageAsync(LoadBalancerId loadBalancerId, string content, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (content == null)
                throw new ArgumentNullException("content");
            if (string.IsNullOrEmpty(content))
                throw new ArgumentException("content cannot be empty");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/errorpage");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            SetLoadBalancerErrorPageRequest requestBody = new SetLoadBalancerErrorPageRequest(content);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveErrorPageAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/errorpage");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<LoadBalancerStatistics> GetStatisticsAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/stats");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<LoadBalancerStatistics>> requestResource =
                GetResponseAsyncFunc<LoadBalancerStatistics>(cancellationToken);

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Node>> ListNodesAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value } };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancerNodesResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancerNodesResponse>(cancellationToken);

            Func<Task<ListLoadBalancerNodesResponse>, IEnumerable<Node>> resultSelector =
                task => (task.Result != null ? task.Result.Nodes : null) ?? Enumerable.Empty<Node>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<Node> GetNodeAsync(LoadBalancerId loadBalancerId, NodeId nodeId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value }, { "nodeId", nodeId.Value } };
            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerNodeResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerNodeResponse>(cancellationToken);

            Func<Task<GetLoadBalancerNodeResponse>, Node> resultSelector =
                task => task.Result.Node;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<Node> AddNodeAsync(LoadBalancerId loadBalancerId, NodeConfiguration nodeConfiguration, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (nodeConfiguration == null)
                throw new ArgumentNullException("nodeConfiguration");

            Func<Task<IEnumerable<Node>>, Node> resultSelector =
                task => task.Result.Single();

            return
                AddNodeRangeAsync(loadBalancerId, new[] { nodeConfiguration }, completionOption, cancellationToken, progress)
                    .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Node>> AddNodeRangeAsync(LoadBalancerId loadBalancerId, IEnumerable<NodeConfiguration> nodeConfigurations, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (nodeConfigurations == null)
                throw new ArgumentNullException("nodeConfigurations");

            return AddNodeRangeAsync(loadBalancerId, nodeConfigurations.ToArray(), completionOption, cancellationToken, progress);
        }

        /// <summary>
        /// Add one or more nodes to a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="nodeConfigurations">A collection of <see cref="NodeConfiguration"/> objects describing the load balancer nodes to add.</param>
        /// <param name="completionOption">Specifies when the <see cref="Task"/> representing the asynchronous server operation should be considered complete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications, if <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation
        /// completes, the <see cref="Task{TResult}.Result"/> property will contain a collection of
        /// <see cref="Node"/> objects describing the new load balancer nodes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="nodeConfigurations"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeConfigurations"/> contains any <c>null</c> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="completionOption"/> is not a valid <see cref="AsyncCompletionOption"/>.</para>
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Add_Nodes-d1e2379.html">Add Nodes (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task<IEnumerable<Node>> AddNodeRangeAsync(LoadBalancerId loadBalancerId, NodeConfiguration[] nodeConfigurations, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeConfigurations == null)
                throw new ArgumentNullException("nodeConfigurations");
            if (nodeConfigurations.Contains(null))
                throw new ArgumentException("nodeConfigurations cannot contain any null values", "nodeConfigurations");

            if (nodeConfigurations.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask(Enumerable.Empty<Node>());
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                };

                AddNodesRequest requestBody = new AddNodesRequest(nodeConfigurations);
                Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.POST, template, parameters, requestBody);

                Func<Task<HttpWebRequest>, Task<ListLoadBalancerNodesResponse>> requestResource =
                    GetResponseAsyncFunc<ListLoadBalancerNodesResponse>(cancellationToken);

                Func<Task<ListLoadBalancerNodesResponse>, Task<IEnumerable<Node>>> resultSelector =
                    task =>
                    {
                        task.PropagateExceptions();
                        if (completionOption == AsyncCompletionOption.RequestCompleted)
                        {
                            return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress).ContinueWith(
                                t =>
                                {
                                    t.PropagateExceptions();
                                    return task.Result.Nodes.AsEnumerable();
                                });
                        }

                        return InternalTaskExtensions.CompletedTask(task.Result.Nodes.AsEnumerable());
                    };

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest).Unwrap()
                    .ContinueWith(requestResource).Unwrap()
                    .ContinueWith(resultSelector).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task UpdateNodeAsync(LoadBalancerId loadBalancerId, NodeId nodeId, NodeUpdate configuration, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value }, { "nodeId", nodeId.Value } };

            UpdateLoadBalancerNodeRequest requestBody = new UpdateLoadBalancerNodeRequest(configuration);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc<string>(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveNodeAsync(LoadBalancerId loadBalancerId, NodeId nodeId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value }, { "nodeId", nodeId.Value } };
            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveNodeRangeAsync(LoadBalancerId loadBalancerId, IEnumerable<NodeId> nodeIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (nodeIds == null)
                throw new ArgumentNullException("nodeIds");

            return RemoveNodeRangeAsync(loadBalancerId, nodeIds.ToArray(), completionOption, cancellationToken, progress);
        }

        /// <summary>
        /// Remove one or more nodes from a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="nodeIds">The load balancer node IDs of nodes to remove. These are obtained from <see cref="Node.Id">Node.Id</see>.</param>
        /// <param name="completionOption">Specifies when the <see cref="Task"/> representing the asynchronous server operation should be considered complete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications, if <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="nodeIds"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeIds"/> contains any <c>null</c> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="completionOption"/> is not a valid <see cref="AsyncCompletionOption"/>.</para>
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Remove_Nodes-d1e2675.html">Remove Nodes (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task RemoveNodeRangeAsync(LoadBalancerId loadBalancerId, NodeId[] nodeIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeIds == null)
                throw new ArgumentNullException("nodeIds");
            if (nodeIds.Contains(null))
                throw new ArgumentException("nodeIds cannot contain any null values", "nodeIds");

            if (nodeIds.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask();
            }
            else if (nodeIds.Length == 1)
            {
                return RemoveNodeAsync(loadBalancerId, nodeIds[0], completionOption, cancellationToken, progress);
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes?id={id}");
                var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value }, { "id", string.Join(",", Array.ConvertAll(nodeIds, i => i.Value)) } };

                Func<Uri, Uri> uriTransform =
                    uri =>
                    {
                        string path = uri.GetLeftPart(UriPartial.Path);
                        string query = uri.Query.Replace(",", "&id=").Replace("%2c", "&id=");
                        return new Uri(path + query);
                    };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters, uriTransform);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                Func<Task<string>, Task<LoadBalancer>> resultSelector =
                    task =>
                    {
                        task.PropagateExceptions();
                        if (completionOption == AsyncCompletionOption.RequestCompleted)
                            return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                        return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                    };

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap()
                    .ContinueWith(resultSelector).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<NodeServiceEvent>> ListNodeServiceEventsAsync(LoadBalancerId loadBalancerId, NodeServiceEventId markerId, int? limit, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/events");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value } };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListNodeServiceEventsResponse>> requestResource =
                GetResponseAsyncFunc<ListNodeServiceEventsResponse>(cancellationToken);

            Func<Task<ListNodeServiceEventsResponse>, IEnumerable<NodeServiceEvent>> resultSelector =
                task => (task.Result != null ? task.Result.NodeServiceEvents : null) ?? Enumerable.Empty<NodeServiceEvent>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerVirtualAddress>> ListVirtualAddressesAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/virtualips");
            var parameters = new Dictionary<string, string> { { "loadBalancerId", loadBalancerId.Value } };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListVirtualAddressesResponse>> requestResource =
                GetResponseAsyncFunc<ListVirtualAddressesResponse>(cancellationToken);

            Func<Task<ListVirtualAddressesResponse>, IEnumerable<LoadBalancerVirtualAddress>> resultSelector =
                task => (task.Result != null ? task.Result.VirtualAddresses : null) ?? Enumerable.Empty<LoadBalancerVirtualAddress>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<LoadBalancerVirtualAddress> AddVirtualAddressAsync(LoadBalancerId loadBalancerId, LoadBalancerVirtualAddressType type, AddressFamily addressFamily, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (type == null)
                throw new ArgumentNullException("type");
            if (addressFamily != AddressFamily.InterNetwork && addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException("Unsupported address family.", "addressFamily");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/virtualips");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
            };

            LoadBalancerVirtualAddress requestBody = new LoadBalancerVirtualAddress(type, addressFamily);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.POST, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<LoadBalancerVirtualAddress>> requestResource =
                GetResponseAsyncFunc<LoadBalancerVirtualAddress>(cancellationToken);

            Func<Task<LoadBalancerVirtualAddress>, Task<LoadBalancerVirtualAddress>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                    {
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress).ContinueWith(
                            t =>
                            {
                                t.PropagateExceptions();
                                return task.Result;
                            });
                    }

                    return InternalTaskExtensions.CompletedTask(task.Result);
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveVirtualAddressAsync(LoadBalancerId loadBalancerId, VirtualAddressId virtualAddressId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/virtualips/{virtualipId}");
            var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "virtualipId", virtualAddressId.Value }
                };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveVirtualAddressRangeAsync(LoadBalancerId loadBalancerId, IEnumerable<VirtualAddressId> virtualAddressIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (virtualAddressIds == null)
                throw new ArgumentNullException("virtualAddressIds");

            return RemoveVirtualAddressRangeAsync(loadBalancerId, virtualAddressIds.ToArray(), completionOption, cancellationToken, progress);
        }

        /// <summary>
        /// Remove a collection of virtual addresses associated with a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="virtualAddressIds">The virtual address IDs. These are obtained from <see cref="LoadBalancerVirtualAddress.Id">LoadBalancerVirtualAddress.Id</see>.</param>
        /// <param name="completionOption">Specifies when the <see cref="Task"/> representing the asynchronous server operation should be considered complete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications, if <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. If <paramref name="completionOption"/> is
        /// <see cref="AsyncCompletionOption.RequestCompleted"/>, the task will not be considered complete until
        /// the load balancer transitions out of the <see cref="LoadBalancerStatus.PendingUpdate"/> state.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="virtualAddressIds"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="virtualAddressIds"/> contains any <c>null</c> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="completionOption"/> is not a valid <see cref="AsyncCompletionOption"/>.</para>
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Remove_Virtual_IP-d1e2919.html">Remove Virtual IP (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task RemoveVirtualAddressRangeAsync(LoadBalancerId loadBalancerId, VirtualAddressId[] virtualAddressIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (virtualAddressIds == null)
                throw new ArgumentNullException("metadataIds");
            if (virtualAddressIds.Contains(null))
                throw new ArgumentException("virtualAddressIds cannot contain any null values", "virtualAddressIds");

            if (virtualAddressIds.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask();
            }
            else if (virtualAddressIds.Length == 1)
            {
                return RemoveVirtualAddressAsync(loadBalancerId, virtualAddressIds[0], completionOption, cancellationToken, progress);
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/virtualips?id={id}");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "id", string.Join(",", Array.ConvertAll(virtualAddressIds, i => i.Value)) }
                };

                Func<Uri, Uri> uriTransform =
                    uri =>
                    {
                        string path = uri.GetLeftPart(UriPartial.Path);
                        string query = uri.Query.Replace(",", "&id=").Replace("%2c", "&id=");
                        return new Uri(path + query);
                    };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters, uriTransform);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                Func<Task<string>, Task<LoadBalancer>> resultSelector =
                    task =>
                    {
                        task.PropagateExceptions();
                        if (completionOption == AsyncCompletionOption.RequestCompleted)
                            return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                        return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                    };

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap()
                    .ContinueWith(resultSelector).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> ListAllowedDomainsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("/loadbalancers/alloweddomains");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListAllowedDomainsResponse>> requestResource =
                GetResponseAsyncFunc<ListAllowedDomainsResponse>(cancellationToken);

            Func<Task<ListAllowedDomainsResponse>, IEnumerable<string>> resultSelector =
                task => task.Result.AllowedDomains;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancer>> ListBillableLoadBalancersAsync(DateTimeOffset? startTime, DateTimeOffset? endTime, int? offset, int? limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerUsage>> ListAccountLevelUsageAsync(DateTimeOffset? startTime, DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerUsage>> ListHistoricalUsageAsync(LoadBalancerId loadBalancerId, DateTimeOffset? startTime, DateTimeOffset? endTime, CancellationToken cancellationToken1)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerUsage>> ListCurrentUsageAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<NetworkItem>> ListAccessListAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/accesslist");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetAccessListResponse>> requestResource =
                GetResponseAsyncFunc<GetAccessListResponse>(cancellationToken);

            Func<Task<GetAccessListResponse>, IEnumerable<NetworkItem>> resultSelector =
                task => task.Result.AccessList;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task CreateAccessListAsync(LoadBalancerId loadBalancerId, NetworkItem networkItem, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (networkItem == null)
                throw new ArgumentNullException("networkItem");

            return CreateAccessListAsync(loadBalancerId, new[] { networkItem }, completionOption, cancellationToken, progress);
        }

        /// <inheritdoc/>
        public Task CreateAccessListAsync(LoadBalancerId loadBalancerId, IEnumerable<NetworkItem> networkItems, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (networkItems == null)
                throw new ArgumentNullException("networkItems");

            return CreateAccessListAsync(loadBalancerId, networkItems.ToArray(), completionOption, cancellationToken, progress);
        }

        /// <summary>
        /// Add a collection of network items to the access list for a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="networkItems">A collection of <see cref="NetworkItem"/> objects describing the network items to add to the load balancer's access list.</param>
        /// <param name="completionOption">Specifies when the <see cref="Task"/> representing the asynchronous server operation should be considered complete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications, if <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation
        /// completes, the <see cref="Task{TResult}.Result"/> property will contain a collection of
        /// <see cref="NetworkItem"/> objects describing the network items added to the access list
        /// for the load balancer.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="networkItems"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="networkItems"/> contains any <c>null</c> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="completionOption"/> is not a valid <see cref="AsyncCompletionOption"/>.</para>
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Manage_Access_Lists-d1e3187.html">Manage Access Lists (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task CreateAccessListAsync(LoadBalancerId loadBalancerId, NetworkItem[] networkItems, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (networkItems == null)
                throw new ArgumentNullException("networkItems");
            if (networkItems.Contains(null))
                throw new ArgumentException("networkItems cannot contain any null values");

            if (networkItems.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask(Enumerable.Empty<NetworkItem>());
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/accesslist");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                };

                CreateAccessListRequest requestBody = new CreateAccessListRequest(networkItems);
                Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.POST, template, parameters, requestBody);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                Func<Task<string>, Task<LoadBalancer>> resultSelector =
                    task =>
                    {
                        task.PropagateExceptions();
                        if (completionOption == AsyncCompletionOption.RequestCompleted)
                            return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                        return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                    };

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest).Unwrap()
                    .ContinueWith(requestResource).Unwrap()
                    .ContinueWith(resultSelector).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task RemoveAccessListAsync(LoadBalancerId loadBalancerId, NetworkItemId networkItemId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (networkItemId == null)
                throw new ArgumentNullException("networkItemId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/accesslist/{networkItemId}");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "networkItemId", networkItemId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveAccessListRangeAsync(LoadBalancerId loadBalancerId, IEnumerable<NetworkItemId> networkItemIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (networkItemIds == null)
                throw new ArgumentNullException("networkItemIds");

            return RemoveAccessListRangeAsync(loadBalancerId, networkItemIds.ToArray(), completionOption, cancellationToken, progress);
        }

        /// <summary>
        /// Remove a collection of network items from the access list of a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="networkItemIds">The network item IDs. These are obtained from <see cref="NetworkItem.Id"/>.</param>
        /// <param name="completionOption">Specifies when the <see cref="Task"/> representing the asynchronous server operation should be considered complete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications, if <paramref name="completionOption"/> is <see cref="AsyncCompletionOption.RequestCompleted"/>. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="networkItemIds"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="networkItemIds"/> contains any <c>null</c> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="completionOption"/> is not a valid <see cref="AsyncCompletionOption"/>.</para>
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Manage_Access_Lists-d1e3187.html">Manage Access Lists (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task RemoveAccessListRangeAsync(LoadBalancerId loadBalancerId, NetworkItemId[] networkItemIds, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (networkItemIds == null)
                throw new ArgumentNullException("networkItemIds");
            if (networkItemIds.Contains(null))
                throw new ArgumentException("networkItemIds cannot contain any null values", "networkItemIds");

            if (networkItemIds.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask();
            }
            else if (networkItemIds.Length == 1)
            {
                return RemoveAccessListAsync(loadBalancerId, networkItemIds[0], completionOption, cancellationToken, progress);
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/accesslist?id={id}");
                var parameters = new Dictionary<string, string>
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "id", string.Join(",", Array.ConvertAll(networkItemIds, i => i.Value)) }
                };

                Func<Uri, Uri> uriTransform =
                    uri =>
                    {
                        string path = uri.GetLeftPart(UriPartial.Path);
                        string query = uri.Query.Replace(",", "&id=").Replace("%2c", "&id=");
                        return new Uri(path + query);
                    };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters, uriTransform);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                Func<Task<string>, Task<LoadBalancer>> resultSelector =
                    task =>
                    {
                        task.PropagateExceptions();
                        if (completionOption == AsyncCompletionOption.RequestCompleted)
                            return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                        return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                    };

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap()
                    .ContinueWith(resultSelector).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task ClearAccessListAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/accesslist");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<HealthMonitor> GetHealthMonitorAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SetHealthMonitorAsync(LoadBalancerId loadBalancerId, HealthMonitor monitor, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveHealthMonitorAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<SessionPersistence> GetSessionPersistenceAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/sessionpersistence");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<SessionPersistence>> requestResource =
                GetResponseAsyncFunc<SessionPersistence>(cancellationToken);

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap();
        }

        /// <inheritdoc/>
        public Task SetSessionPersistenceAsync(LoadBalancerId loadBalancerId, SessionPersistence sessionPersistence, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (sessionPersistence == null)
                throw new ArgumentNullException("sessionPersistence");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/sessionpersistence");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, sessionPersistence);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveSessionPersistenceAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/sessionpersistence");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<bool> GetConnectionLoggingAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/connectionlogging");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerConnectionLoggingResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerConnectionLoggingResponse>(cancellationToken);

            Func<Task<GetLoadBalancerConnectionLoggingResponse>, bool> resultSelector =
                task => task.Result != null ? task.Result.Enabled ?? false : false;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task SetConnectionLoggingAsync(LoadBalancerId loadBalancerId, bool enabled, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/connectionlogging");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            SetLoadBalancerConnectionLoggingRequest request = new SetLoadBalancerConnectionLoggingRequest(enabled);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, request);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<ConnectionThrottles> ListThrottlesAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/connectionthrottle");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancerThrottlesResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancerThrottlesResponse>(cancellationToken);

            Func<Task<ListLoadBalancerThrottlesResponse>, ConnectionThrottles> resultSelector =
                task => task.Result.Throttles;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task UpdateThrottlesAsync(LoadBalancerId loadBalancerId, ConnectionThrottles throttleConfiguration, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (throttleConfiguration == null)
                throw new ArgumentNullException("throttleConfiguration");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/connectionthrottle");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, throttleConfiguration);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveThrottlesAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/connectionthrottle");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<bool> GetContentCachingAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/contentcaching");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerContentCachingResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerContentCachingResponse>(cancellationToken);

            Func<Task<GetLoadBalancerContentCachingResponse>, bool> resultSelector =
                task => task.Result != null ? task.Result.Enabled ?? false : false;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task SetContentCachingAsync(LoadBalancerId loadBalancerId, bool enabled, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/contentcaching");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            SetLoadBalancerContentCachingRequest request = new SetLoadBalancerContentCachingRequest(enabled);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, request);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancingProtocol>> ListProtocolsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("/loadbalancers/protocols");
            var parameters = new Dictionary<string, string>();

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancingProtocolsResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancingProtocolsResponse>(cancellationToken);

            Func<Task<ListLoadBalancingProtocolsResponse>, IEnumerable<LoadBalancingProtocol>> resultSelector =
                task => (task.Result != null ? task.Result.Protocols : null) ?? Enumerable.Empty<LoadBalancingProtocol>();

            // authenticate -> request resource -> check result -> parse result -> cache result -> return
            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancingAlgorithm>> ListAlgorithmsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("/loadbalancers/algorithms");
            var parameters = new Dictionary<string, string>();

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancingAlgorithmsResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancingAlgorithmsResponse>(cancellationToken);

            Func<Task<ListLoadBalancingAlgorithmsResponse>, IEnumerable<LoadBalancingAlgorithm>> resultSelector =
                task => (task.Result != null ? task.Result.Algorithms : null) ?? Enumerable.Empty<LoadBalancingAlgorithm>();

            // authenticate -> request resource -> check result -> parse result -> cache result -> return
            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest, cancellationToken)
                .ContinueWith(requestResource, cancellationToken).Unwrap()
                .ContinueWith(resultSelector, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<LoadBalancerSslConfiguration> GetSslConfigurationAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/ssltermination");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerSslConfigurationResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerSslConfigurationResponse>(cancellationToken);

            Func<Task<GetLoadBalancerSslConfigurationResponse>, LoadBalancerSslConfiguration> resultSelector =
                task => task.Result != null ? task.Result.SslConfiguration : null;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task UpdateSslConfigurationAsync(LoadBalancerId loadBalancerId, LoadBalancerSslConfiguration configuration, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/ssltermination");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            LoadBalancerSslConfiguration requestBody = configuration;
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveSslConfigurationAsync(LoadBalancerId loadBalancerId, AsyncCompletionOption completionOption, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/ssltermination");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value }
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            Func<Task<string>, Task<LoadBalancer>> resultSelector =
                task =>
                {
                    task.PropagateExceptions();
                    if (completionOption == AsyncCompletionOption.RequestCompleted)
                        return WaitForLoadBalancerToLeaveStateAsync(loadBalancerId, LoadBalancerStatus.PendingUpdate, cancellationToken, progress);

                    return InternalTaskExtensions.CompletedTask(default(LoadBalancer));
                };

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector).Unwrap();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerMetadataItem>> ListLoadBalancerMetadataAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/metadata");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancerMetadataResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancerMetadataResponse>(cancellationToken);

            Func<Task<ListLoadBalancerMetadataResponse>, IEnumerable<LoadBalancerMetadataItem>> resultSelector =
                task => (task.Result != null ? task.Result.Metadata : null) ?? Enumerable.Empty<LoadBalancerMetadataItem>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<LoadBalancerMetadataItem> GetLoadBalancerMetadataItemAsync(LoadBalancerId loadBalancerId, MetadataId metadataId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (metadataId == null)
                throw new ArgumentNullException("metadataId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/metadata/{metaId}");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "metaId", metadataId.Value },
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerMetadataItemResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerMetadataItemResponse>(cancellationToken);

            Func<Task<GetLoadBalancerMetadataItemResponse>, LoadBalancerMetadataItem> resultSelector =
                task => task.Result != null ? task.Result.MetadataItem : null;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerMetadataItem>> ListNodeMetadataAsync(LoadBalancerId loadBalancerId, NodeId nodeId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}/metadata");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "nodeId", nodeId.Value },
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancerMetadataResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancerMetadataResponse>(cancellationToken);

            Func<Task<ListLoadBalancerMetadataResponse>, IEnumerable<LoadBalancerMetadataItem>> resultSelector =
                task => (task.Result != null ? task.Result.Metadata : null) ?? Enumerable.Empty<LoadBalancerMetadataItem>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<LoadBalancerMetadataItem> GetNodeMetadataItemAsync(LoadBalancerId loadBalancerId, NodeId nodeId, MetadataId metadataId, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");
            if (metadataId == null)
                throw new ArgumentNullException("metadataId");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}/metadata/{metaId}");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "nodeId", nodeId.Value },
                { "metaId", metadataId.Value },
            };

            Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.GET, template, parameters);

            Func<Task<HttpWebRequest>, Task<GetLoadBalancerMetadataItemResponse>> requestResource =
                GetResponseAsyncFunc<GetLoadBalancerMetadataItemResponse>(cancellationToken);

            Func<Task<GetLoadBalancerMetadataItemResponse>, LoadBalancerMetadataItem> resultSelector =
                task => task.Result != null ? task.Result.MetadataItem : null;

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest)
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerMetadataItem>> AddLoadBalancerMetadataAsync(LoadBalancerId loadBalancerId, IEnumerable<KeyValuePair<string, string>> metadata, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/metadata");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
            };

            AddLoadBalancerMetadataRequest requestBody = new AddLoadBalancerMetadataRequest(metadata);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.POST, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancerMetadataResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancerMetadataResponse>(cancellationToken);

            Func<Task<ListLoadBalancerMetadataResponse>, IEnumerable<LoadBalancerMetadataItem>> resultSelector =
                task => (task.Result != null ? task.Result.Metadata : null) ?? Enumerable.Empty<LoadBalancerMetadataItem>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<LoadBalancerMetadataItem>> AddNodeMetadataAsync(LoadBalancerId loadBalancerId, NodeId nodeId, IEnumerable<KeyValuePair<string, string>> metadata, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}/metadata");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "nodeId", nodeId.Value },
            };

            AddLoadBalancerMetadataRequest requestBody = new AddLoadBalancerMetadataRequest(metadata);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.POST, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<ListLoadBalancerMetadataResponse>> requestResource =
                GetResponseAsyncFunc<ListLoadBalancerMetadataResponse>(cancellationToken);

            Func<Task<ListLoadBalancerMetadataResponse>, IEnumerable<LoadBalancerMetadataItem>> resultSelector =
                task => (task.Result != null ? task.Result.Metadata : null) ?? Enumerable.Empty<LoadBalancerMetadataItem>();

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap()
                .ContinueWith(resultSelector);
        }

        /// <inheritdoc/>
        public Task UpdateLoadBalancerMetadataItemAsync(LoadBalancerId loadBalancerId, MetadataId metadataId, string value, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (metadataId == null)
                throw new ArgumentNullException("metadataId");
            if (value == null)
                throw new ArgumentNullException("value");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/metadata/{metaId}");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "metaId", metadataId.Value }
            };

            UpdateLoadBalancerMetadataItemRequest requestBody = new UpdateLoadBalancerMetadataItemRequest(value);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap();
        }

        /// <inheritdoc/>
        public Task UpdateNodeMetadataItemAsync(LoadBalancerId loadBalancerId, NodeId nodeId, MetadataId metadataId, string value, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");
            if (metadataId == null)
                throw new ArgumentNullException("metadataId");
            if (value == null)
                throw new ArgumentNullException("value");

            UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}/metadata/{metaId}");
            var parameters = new Dictionary<string, string>()
            {
                { "loadBalancerId", loadBalancerId.Value },
                { "nodeId", nodeId.Value },
                { "metaId", metadataId.Value }
            };

            UpdateLoadBalancerMetadataItemRequest requestBody = new UpdateLoadBalancerMetadataItemRequest(value);
            Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> prepareRequest =
                PrepareRequestAsyncFunc(HttpMethod.PUT, template, parameters, requestBody);

            Func<Task<HttpWebRequest>, Task<string>> requestResource =
                GetResponseAsyncFunc(cancellationToken);

            return AuthenticateServiceAsync(cancellationToken)
                .ContinueWith(prepareRequest).Unwrap()
                .ContinueWith(requestResource).Unwrap();
        }

        /// <inheritdoc/>
        public Task RemoveLoadBalancerMetadataItemAsync(LoadBalancerId loadBalancerId, IEnumerable<MetadataId> metadataIds, CancellationToken cancellationToken)
        {
            if (metadataIds == null)
                throw new ArgumentNullException("metadataIds");

            return RemoveLoadBalancerMetadataItemAsync(loadBalancerId, metadataIds.ToArray(), cancellationToken);
        }

        /// <summary>
        /// Removes one or more metadata items associated with a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="metadataIds">The metadata item IDs. These are obtained from <see cref="LoadBalancerMetadataItem.Id">LoadBalancerMetadataItem.Id</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="metadataIds"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="metadataIds"/> contains any <c>null</c> values.
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Remove_Metadata-d1e2675.html">Remove Metadata (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task RemoveLoadBalancerMetadataItemAsync(LoadBalancerId loadBalancerId, MetadataId[] metadataIds, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (metadataIds == null)
                throw new ArgumentNullException("metadataIds");
            if (metadataIds.Contains(null))
                throw new ArgumentException("metadataIds cannot contain any null values", "metadataIds");

            if (metadataIds.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask();
            }
            else if (metadataIds.Length == 1)
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/metadata/{metaId}");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "metaId", metadataIds[0].Value }
                };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap();
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/metadata?id={id}");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "id", string.Join(",", Array.ConvertAll(metadataIds, i => i.Value)) }
                };

                Func<Uri, Uri> uriTransform =
                    uri =>
                    {
                        string path = uri.GetLeftPart(UriPartial.Path);
                        string query = uri.Query.Replace(",", "&id=").Replace("%2c", "&id=");
                        return new Uri(path + query);
                    };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters, uriTransform);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap();
            }
        }

        /// <inheritdoc/>
        public Task RemoveNodeMetadataItemAsync(LoadBalancerId loadBalancerId, NodeId nodeId, IEnumerable<MetadataId> metadataIds, CancellationToken cancellationToken)
        {
            if (metadataIds == null)
                throw new ArgumentNullException("metadataIds");

            return RemoveNodeMetadataItemAsync(loadBalancerId, nodeId, metadataIds.ToArray(), cancellationToken);
        }

        /// <summary>
        /// Removes one or more metadata items associated with a load balancer node.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="nodeId">The load balancer node ID. This is obtained from <see cref="Node.Id">Node.Id</see>.</param>
        /// <param name="metadataIds">The metadata item IDs. These are obtained from <see cref="LoadBalancerMetadataItem.Id">LoadBalancerMetadataItem.Id</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="nodeId"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadataIds"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="metadataIds"/> contains any <c>null</c> values.
        /// </exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Remove_Metadata-d1e2675.html">Remove Metadata (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        public Task RemoveNodeMetadataItemAsync(LoadBalancerId loadBalancerId, NodeId nodeId, MetadataId[] metadataIds, CancellationToken cancellationToken)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");
            if (metadataIds == null)
                throw new ArgumentNullException("metadataIds");
            if (metadataIds.Contains(null))
                throw new ArgumentException("metadataIds cannot contain any null values", "metadataIds");

            if (metadataIds.Length == 0)
            {
                return InternalTaskExtensions.CompletedTask();
            }
            else if (metadataIds.Length == 1)
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}/metadata/{metaId}");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "nodeId", nodeId.Value },
                    { "metaId", metadataIds[0].Value }
                };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap();
            }
            else
            {
                UriTemplate template = new UriTemplate("/loadbalancers/{loadBalancerId}/nodes/{nodeId}/metadata?id={id}");
                var parameters = new Dictionary<string, string>()
                {
                    { "loadBalancerId", loadBalancerId.Value },
                    { "nodeId", nodeId.Value },
                    { "id", string.Join(",", Array.ConvertAll(metadataIds, i => i.Value)) }
                };

                Func<Uri, Uri> uriTransform =
                    uri =>
                    {
                        string path = uri.GetLeftPart(UriPartial.Path);
                        string query = uri.Query.Replace(",", "&id=").Replace("%2c", "&id=");
                        return new Uri(path + query);
                    };

                Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> prepareRequest =
                    PrepareRequestAsyncFunc(HttpMethod.DELETE, template, parameters, uriTransform);

                Func<Task<HttpWebRequest>, Task<string>> requestResource =
                    GetResponseAsyncFunc(cancellationToken);

                return AuthenticateServiceAsync(cancellationToken)
                    .ContinueWith(prepareRequest)
                    .ContinueWith(requestResource).Unwrap();
            }
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="Task"/> that will complete after a load balancer leaves a particular state.
        /// </summary>
        /// <remarks>
        /// The task is considered complete as soon as a call to <see cref="ILoadBalancerService.GetLoadBalancerAsync"/>
        /// indicates that the load balancer is not in the state specified by <paramref name="state"/>. The method
        /// does not perform any other checks related to the initial or final state of the load balancer.
        /// </remarks>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="state">A <see cref="LoadBalancerStatus"/> representing the state the load balancer should <em>not</em> be in at the end of the wait operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will contain a
        /// <see cref="LoadBalancer"/> object representing the load balancer. In addition, the load
        /// <see cref="LoadBalancer.Status"/> property of the load balancer will <em>not</em> be
        /// equal to <paramref name="state"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="state"/> is <c>null</c>.</para>
        /// </exception>
        protected Task<LoadBalancer> WaitForLoadBalancerToLeaveStateAsync(LoadBalancerId loadBalancerId, LoadBalancerStatus state, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (state == null)
                throw new ArgumentNullException("state");

            TaskCompletionSource<LoadBalancer> taskCompletionSource = new TaskCompletionSource<LoadBalancer>();
            Func<Task<LoadBalancer>> pollLoadBalancer = () => PollLoadBalancerStateAsync(loadBalancerId, cancellationToken, progress);

            Task<LoadBalancer> currentTask = pollLoadBalancer();
            Action<Task<LoadBalancer>> continuation = null;
            continuation =
                previousTask =>
                {
                    if (previousTask.Status != TaskStatus.RanToCompletion)
                    {
                        taskCompletionSource.SetFromTask(previousTask);
                        return;
                    }

                    LoadBalancer result = previousTask.Result;
                    if (result == null || result.Status != state)
                    {
                        // finished waiting
                        taskCompletionSource.SetResult(result);
                        return;
                    }

                    // reschedule
                    currentTask = Task.Factory.StartNewDelayed((int)TimeSpan.FromSeconds(1).TotalMilliseconds, cancellationToken).ContinueWith(
                        task =>
                        {
                            task.PropagateExceptions();
                            return pollLoadBalancer();
                        }).Unwrap();
                    currentTask.ContinueWith(continuation);
                };
            currentTask.ContinueWith(continuation);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Asynchronously poll the current state of a load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The load balancer ID. This is obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When
        /// the task completes successfully, the <see cref="Task{TResult}.Result"/>
        /// property will contain a <see cref="LoadBalancer"/> object containing the
        /// updated state information for the load balancer.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="loadBalancerId"/> is <c>null</c>.</exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        private Task<LoadBalancer> PollLoadBalancerStateAsync(LoadBalancerId loadBalancerId, CancellationToken cancellationToken, IProgress<LoadBalancer> progress)
        {
            Task<LoadBalancer> chain = GetLoadBalancerAsync(loadBalancerId, cancellationToken);
            chain = chain.ContinueWith(
                task =>
                {
                    if (task.Result == null || task.Result.Id != loadBalancerId)
                        throw new InvalidOperationException("Could not obtain status for load balancer");

                    return task.Result;
                }, TaskContinuationOptions.ExecuteSynchronously);

            if (progress != null)
            {
                chain = chain.ContinueWith(
                    task =>
                    {
                        progress.Report(task.Result);
                        return task.Result;
                    }, TaskContinuationOptions.ExecuteSynchronously);
            }

            return chain;
        }

        /// <summary>
        /// Creates a <see cref="Task"/> that will complete after a group of load balancers all leave a particular state.
        /// </summary>
        /// <remarks>
        /// The task is considered complete as soon as calls to <see cref="ILoadBalancerService.GetLoadBalancerAsync"/>
        /// indicates that none of the load balancers are in the state specified by <paramref name="state"/>. The method
        /// does not perform any other checks related to the initial or final state of the load balancers.
        /// </remarks>
        /// <param name="loadBalancerIds">The load balancer IDs. These are obtained from <see cref="LoadBalancer.Id">LoadBalancer.Id</see>.</param>
        /// <param name="state">A <see cref="LoadBalancerStatus"/> representing the state the load balancers should <em>not</em> be in at the end of the wait operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <c>null</c>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will contain a
        /// collection of <see cref="LoadBalancer"/> objects representing the load balancers. In
        /// addition, the load <see cref="LoadBalancer.Status"/> property of the load balancer will
        /// <em>not</em> be equal to <paramref name="state"/> for <em>any</em> of the load balancer
        /// instances.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="loadBalancerIds"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="state"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="loadBalancerIds"/> contains any <c>null</c> values.</exception>
        protected Task<LoadBalancer[]> WaitForLoadBalancersToLeaveStateAsync(LoadBalancerId[] loadBalancerIds, LoadBalancerStatus state, CancellationToken cancellationToken, IProgress<LoadBalancer[]> progress)
        {
            if (loadBalancerIds == null)
                throw new ArgumentNullException("loadBalancerIds");
            if (state == null)
                throw new ArgumentNullException("state");
            if (loadBalancerIds.Contains(null))
                throw new ArgumentException("loadBalancerIds cannot contain any null values");

            TaskCompletionSource<LoadBalancer[]> taskCompletionSource = new TaskCompletionSource<LoadBalancer[]>();
            Func<Task<LoadBalancer[]>> pollLoadBalancers =
                () =>
                {
                    Task<LoadBalancer>[] tasks = Array.ConvertAll(
                        loadBalancerIds,
                        loadBalancerId =>
                        {
                            return PollLoadBalancerStateAsync(loadBalancerId, cancellationToken, null);
                        });

                    return Task.Factory.WhenAll(tasks).ContinueWith(
                        completedTasks =>
                        {
                            LoadBalancer[] loadBalancers = Array.ConvertAll(completedTasks.Result, completedTask => completedTask.Result);
                            if (progress != null)
                                progress.Report(loadBalancers);

                            return loadBalancers;
                        });
                };

            Task<LoadBalancer[]> currentTask = pollLoadBalancers();
            Action<Task<LoadBalancer[]>> continuation = null;
            continuation =
                previousTask =>
                {
                    if (previousTask.Status != TaskStatus.RanToCompletion)
                    {
                        taskCompletionSource.SetFromTask(previousTask);
                        return;
                    }

                    LoadBalancer[] results = previousTask.Result;
                    if (results.All(result => result == null || result.Status == state))
                    {
                        // finished waiting
                        taskCompletionSource.SetResult(results);
                        return;
                    }

                    // reschedule
                    currentTask = Task.Factory.StartNewDelayed((int)TimeSpan.FromSeconds(1).TotalMilliseconds, cancellationToken).ContinueWith(
                        task =>
                        {
                            task.PropagateExceptions();
                            return pollLoadBalancers();
                        }).Unwrap();
                    currentTask.ContinueWith(continuation);
                };
            currentTask.ContinueWith(continuation);

            return taskCompletionSource.Task;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method returns a cached base address if one is available. If no cached address is
        /// available, <see cref="ProviderBase{TProvider}.GetServiceEndpoint"/> is called to obtain
        /// an <see cref="Endpoint"/> with the type <c>rax:load-balancer</c> and preferred type <c>cloudLoadBalancers</c>.
        /// </remarks>
        protected override Task<Uri> GetBaseUriAsync(CancellationToken cancellationToken)
        {
            if (_baseUri != null)
            {
                return InternalTaskExtensions.CompletedTask(_baseUri);
            }

            return Task.Factory.StartNew(
                () =>
                {
                    Endpoint endpoint = GetServiceEndpoint(null, "rax:load-balancer", "cloudLoadBalancers", null);
                    _baseUri = new Uri(endpoint.PublicURL);
                    return _baseUri;
                });
        }

        /// <summary>
        /// This class provides a wrapper implementation of <see cref="IProgress{T}"/> which
        /// wraps a single progress report values into a single-element array.
        /// </summary>
        /// <typeparam name="T">The type of progress update value.</typeparam>
        private class ArrayElementProgressWrapper<T> : IProgress<T>
        {
            /// <summary>
            /// The delegate progress handler to dispatch progress reports to.
            /// </summary>
            private readonly IProgress<T[]> _delegate;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayElementProgressWrapper{T}"/> class
            /// that dispatches progress reports to the specified delegate. The reported progress
            /// values are wrapped in a single-element array.
            /// </summary>
            /// <param name="delegate">The delegate to dispatch progress reports to.</param>
            /// <exception cref="ArgumentNullException">If <paramref name="delegate"/> is <c>null</c>.</exception>
            public ArrayElementProgressWrapper(IProgress<T[]> @delegate)
            {
                if (@delegate == null)
                    throw new ArgumentNullException("delegate");

                _delegate = @delegate;
            }

            /// <inheritdoc/>
            public void Report(T value)
            {
                _delegate.Report(new T[] { value });
            }
        }
    }
}
