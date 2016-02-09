using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Extensions;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using OpenStack.Authentication;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Builds requests to the Compute API which can be further customized and then executed.
    /// <para>Intended for custom implementations.</para>
    /// </summary>
    /// <exclude />
    /// <seealso href="http://developer.openstack.org/api-ref-compute-v2.1.html">OpenStack Compute API v2.1 Overview</seealso>
    public class ComputeApi
    {
        /// <summary />
        protected readonly IAuthenticationProvider AuthenticationProvider;

        /// <summary>
        /// The Nova microversion header key
        /// </summary>
        public const string MicroversionHeader = "X-OpenStack-Nova-API-Version";

        /// <summary />
        protected readonly ServiceEndpoint Endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeApi"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for the desired compute provider.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        public ComputeApi(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl)
            : this(serviceType, authenticationProvider, region, useInternalUrl, "2.1")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeApi"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for the desired compute provider.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        /// <param name="region">The region.</param>
        /// <param name="microversion">The requested API microversion.</param>
        protected ComputeApi(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl, string microversion)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (authenticationProvider == null)
                throw new ArgumentNullException("authenticationProvider");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentException("region cannot be null or empty", "region");

            AuthenticationProvider = authenticationProvider;
            Endpoint = new ServiceEndpoint(serviceType, authenticationProvider, region, useInternalUrl, microversion, MicroversionHeader);
        }

        #region Servers

        /// <summary />
        public virtual async Task<T> GetServerAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetServerRequest(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetServerRequest(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"servers/{serverId}", cancellationToken);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildCreateServerRequest(object server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareCreateResourceRequest("servers", server, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateServerAsync<T>(object server, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildCreateServerRequest(server, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the server to reach the specified status.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TServer> WaitForServerStatusAsync<TServer, TStatus>(string serverId, TStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TServer : IServiceResource
            where TStatus : ResourceStatus
        {
            Func<Task<TServer>> getServer = async () => await GetServerAsync<TServer>(serverId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(serverId, status, getServer, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the server to reach the specified status.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TServer> WaitForServerStatusAsync<TServer, TStatus>(string serverId, IEnumerable<TStatus> status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TServer : IServiceResource
            where TStatus : ResourceStatus
        {
            Func<Task<TServer>> getServer = async () => await GetServerAsync<TServer>(serverId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(serverId, status, getServer, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the server to be deleted.
        /// <para>Treats a 404 NotFound exception as confirmation that it is deleted.</para>
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="deletedStatus">The deleted status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public Task WaitUntilServerIsDeletedAsync<TServer, TStatus>(string serverId, TStatus deletedStatus = null, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TServer : IServiceResource
            where TStatus : ResourceStatus
        {
            deletedStatus = deletedStatus ?? StringEnumeration.FromDisplayName<TStatus>("DELETED");
            Func<Task<dynamic>> getServer = async () => await GetServerAsync<TServer>(serverId, cancellationToken);
            return Endpoint.WaitUntilDeletedAsync(serverId, deletedStatus, getServer, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListServerSummariesAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IEnumerable<IServiceResource>
        {
            Url initialRequestUrl = await BuildListServerSummariesUrl(queryString, cancellationToken);
            return await Endpoint.GetResourcePageAsync<TPage>(initialRequestUrl, cancellationToken)
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual async Task<Url> BuildListServerSummariesUrl(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("servers")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary />
        public virtual async Task<TPage> ListServersAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IEnumerable<IServiceResource>
        {
            Url initialRequestUrl = await BuildListServersUrl(queryString, cancellationToken);
            return await Endpoint.GetResourcePageAsync<TPage>(initialRequestUrl, cancellationToken)
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual async Task<Url> BuildListServersUrl(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("servers/detail")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildUpdateServerRequest(string serverId, object server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareUpdateResourceRequest($"servers/{serverId}", server, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> UpdateServerAsync<T>(string serverId, object server, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildUpdateServerRequest(serverId, server, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task DeleteServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteServerRequest(serverId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteServerRequest(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareDeleteResourceRequest($"servers/{serverId}", cancellationToken);
        }

        /// <summary>
        /// Waits for an image to reach the specified state.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="status">The image status.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TImage> WaitForImageStatusAsync<TImage, TStatus>(string imageId, TStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TImage : IServiceResource
            where TStatus : ResourceStatus
        {
            Func<Task<TImage>> getImage = async () => await GetImageAsync<TImage>(imageId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(imageId, status, getImage, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the image to be deleted.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="deletedStatus">The deleted status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public Task WaitUntilImageIsDeletedAsync<TImage, TStatus>(string imageId, TStatus deletedStatus, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TImage : IServiceResource
            where TStatus : ResourceStatus
        {
            deletedStatus = deletedStatus ?? StringEnumeration.FromDisplayName<TStatus>("DELETED");
            Func<Task<dynamic>> getImage = async () => await GetServerAsync<TImage>(imageId, cancellationToken);
            return Endpoint.WaitUntilDeletedAsync(imageId, deletedStatus, getImage, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> SnapshotServerAsync<T>(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var response = await BuildServerActionRequest(serverId, request, cancellationToken).SendAsync();
            Identifier imageId = response.Headers.Location.Segments.Last(); // grab id off the end of the url, e.g. http://172.29.236.100:9292/images/baaab9b9-3635-429e-9969-2899a7cf2d97
            return await GetImageAsync<T>(imageId, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task StartServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new StartServerRequest();
            return BuildServerActionRequest(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task StopServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new StopServerRequest();
            return BuildServerActionRequest(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task RebootServerAsync<TRequest>(string serverId, TRequest request = null, CancellationToken cancellationToken = default(CancellationToken))
            where TRequest : class, new()
        {
            request = request ?? new TRequest();
            return BuildServerActionRequest(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task EvacuateServerAsync(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildServerActionRequest(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<T> GetVncConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getVNCConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<T> GetSpiceConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getSPICEConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<T> GetSerialConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getSerialConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<T> GetRdpConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getRDPConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual async Task<string> GetConsoleOutputAsync(string serverId, int length = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getConsoleOutput': {{ 'length': '{length}' }} }}");
            dynamic result = await BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson();

            return result.output;
        }

        /// <summary />
        public virtual async Task<string> RescueServerAsync(string serverId, object request = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            request = request ?? new Dictionary<string, object> { ["rescue"] = null };
            dynamic result = await BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson();

            return result.adminPass;
        }

        /// <summary />
        public virtual Task UnrescueServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            object request = new Dictionary<string, object> { ["unrescue"] = null };
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync();
        }

        /// <summary />
        public virtual Task ResizeServerAsync(string serverId, string flavorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            object request = new
            {
                resize = new
                {
                    flavorRef = flavorId
                }
            };
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync();
        }

        /// <summary />
        public virtual Task ConfirmResizeServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            object request = new Dictionary<string, object> { ["confirmResize"] = null };
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync();
        }

        /// <summary />
        public virtual Task CancelResizeServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            object request = new Dictionary<string, object> { ["revertResize"] = null };
            return BuildServerActionRequest(serverId, request, cancellationToken)
                .SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildServerActionRequest(string serverId, object requestBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            if (requestBody == null)
                throw new ArgumentNullException("requestBody");

            var request = await Endpoint.PrepareRequest($"servers/{serverId}/action", cancellationToken);
            return request.PreparePostJson(requestBody, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListServerActionsAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListServerActionsRequest(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListServerActionsRequest(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"servers/{serverId}/os-instance-actions", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> GetServerActionAsync<T>(string serverId, string actionId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetServerActionRequest(serverId, actionId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetServerActionRequest(string serverId, string actionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            if (serverId == null)
                throw new ArgumentNullException("actionId");

            return Endpoint.PrepareGetResourceRequest($"servers/{serverId}/os-instance-actions/{actionId}", cancellationToken);
        }

        #endregion

        #region Flavors
        /// <summary />
        public virtual async Task<T> GetFlavorAsync<T>(string flavorId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetFlavorRequest(flavorId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetFlavorRequest(string flavorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"flavors/{flavorId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListFlavorSummariesAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListFlavorSummariesRequest(cancellationToken)
                 .SendAsync()
                 .ReceiveJson<T>()
                 .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListFlavorSummariesRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareListResourcesRequest("flavors", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListFlavorsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListFlavorsRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListFlavorsRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareListResourcesRequest("flavors/detail", cancellationToken);
        }

        #endregion

        #region Images
        /// <summary />
        public virtual async Task<T> GetImageAsync<T>(string imageId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetImageRequest(imageId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetImageRequest(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"images/{imageId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> GetImageMetadataAsync<T>(string imageId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IChildResource
        {
            return await BuildGetImageMetadataRequest(imageId, cancellationToken)
                 .SendAsync()
                 .ReceiveJson<T>()
                 .PropogateOwner(this)
                 .SetParent(imageId);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetImageMetadataRequest(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"images/{imageId}/metadata", cancellationToken);
        }

        /// <summary />
        public virtual async Task<string> GetImageMetadataItemAsync(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            dynamic result = await BuildGetImageMetadataItemRequest(imageId, key, cancellationToken)
                .SendAsync()
                .ReceiveJson();

            var meta = (IDictionary<string, object>)result.meta;
            return meta[key]?.ToString();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetImageMetadataItemRequest(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"images/{imageId}/metadata/{key}", cancellationToken);
        }

        /// <summary />
        public virtual Task CreateImageMetadataAsync(string imageId, string key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildCreateImageMetadataRequest(imageId, key, value, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildCreateImageMetadataRequest(string imageId, string key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var imageMetadata = new
            {
                meta = new Dictionary<string, string>
                {
                    [key] = value
                }
            };

            PreparedRequest request = await Endpoint.PrepareRequest($"images/{imageId}/metadata/{key}", cancellationToken);
            return request.PreparePutJson(imageMetadata, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListImageSummariesAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IEnumerable<IServiceResource>
        {
            Url initialRequestUrl = await BuildListImageSummariesRequest(queryString, cancellationToken);
            return await Endpoint.GetResourcePageAsync<TPage>(initialRequestUrl, cancellationToken)
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual async Task<Url> BuildListImageSummariesRequest(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("images")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary />
        public virtual async Task<TPage> ListImagesAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IEnumerable<IServiceResource>
        {
            Url initialRequestUrl = await BuildListImagesRequest(queryString, cancellationToken);
            return await Endpoint.GetResourcePageAsync<TPage>(initialRequestUrl, cancellationToken)
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual async Task<Url> BuildListImagesRequest(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("images/detail")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary /> // this keeps existing, but omitted values
        public virtual async Task<T> UpdateImageMetadataAsync<T>(string imageId, object metadata, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildUpdateImageMetadataRequest(imageId, metadata, overwrite, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildUpdateImageMetadataRequest(string imageId, object metadata, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            PreparedRequest request = await Endpoint.PrepareRequest($"images/{imageId}/metadata", cancellationToken);

            if (overwrite)
                return request.PreparePutJson(metadata, cancellationToken);

            return request.PreparePostJson(metadata, cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteImageAsync(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteImageRequest(imageId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteImageRequest(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareDeleteResourceRequest($"images/{imageId}", cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteImageMetadataAsync(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteImageMetadataRequest(imageId, key, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteImageMetadataRequest(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (imageId == null)
                throw new ArgumentNullException("imageId");

            if (key == null)
                throw new ArgumentNullException("key");

            return Endpoint.PrepareDeleteResourceRequest($"images/{imageId}/metadata/{key}", cancellationToken);
        }
        #endregion

        #region IP Addresses

        /// <summary />
        public virtual async Task<IList<T>> GetServerAddressAsync<T>(string serverId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await BuildGetServerAddressRequest(serverId, key, cancellationToken)
                .SendAsync()
                .ReceiveJson<IDictionary<string, IList<T>>>();

            return result[key];
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetServerAddressRequest(string serverid, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"servers/{serverid}/ips/{key}", cancellationToken);
        }

        /// <summary />
        public virtual Task<T> ListServerAddressesAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildListServerAddressesRequest(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListServerAddressesRequest(string serverid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"servers/{serverid}/ips", cancellationToken);
        }

        #endregion

        #region Server Volumes
        /// <summary />
        public virtual async Task<T> GetServerVolumeAsync<T>(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IChildResource
        {
            return await BuildGetServerVolumeRequest(serverId, volumeId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this)
                .SetParent(serverId);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetServerVolumeRequest(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            if (volumeId == null)
                throw new ArgumentNullException("volumeId");

            return Endpoint.PrepareGetResourceRequest($"servers/{serverId}/os-volume_attachments/{volumeId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListServerVolumesAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IChildResource>
        {
            return await BuidListServerVolumesRequest(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this)
                .SetParentOnChildren(serverId);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuidListServerVolumesRequest(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            return Endpoint.PrepareGetResourceRequest($"servers/{serverId}/os-volume_attachments", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> AttachVolumeAsync<T>(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
            where T : IChildResource
        {
            return await BuildAttachVolumeRequest(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this)
                .SetParent(serverId);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildAttachVolumeRequest(string serverId, object serverVolume, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            return Endpoint.PrepareCreateResourceRequest($"servers/{serverId}/os-volume_attachments", serverVolume, cancellationToken);
        }

        /// <summary />
        public virtual Task DetachVolumeAsync(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDetachVolumeRequest(serverId, volumeId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDetachVolumeRequest(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            if (volumeId == null)
                throw new ArgumentNullException("volumeId");

            return Endpoint.PrepareDeleteResourceRequest($"servers/{serverId}/os-volume_attachments/{volumeId}", cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <summary />
        public virtual async Task<T> GetKeyPairAsync<T>(string keypairName, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            if (keypairName == null)
                throw new ArgumentNullException("keypairName");

            return await BuildGetKeyPairRequest(keypairName, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetKeyPairRequest(string keypairName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest($"os-keypairs/{keypairName}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateKeyPairAsync<T>(object keypair, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            if (keypair == null)
                throw new ArgumentNullException("keypair");

            return await BuildCreateKeyPairRequest(keypair, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildCreateKeyPairRequest(object keypair, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareCreateResourceRequest("os-keypairs", keypair, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListKeyPairsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListKeyPairsRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListKeyPairsRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("os-keypairs", cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteKeyPairAsync(string keypairName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (keypairName == null)
                throw new ArgumentNullException("keypairName");

            return BuildDeleteKeyPairRequest(keypairName, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteKeyPairRequest(string keypairName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareDeleteResourceRequest($"os-keypairs/{keypairName}", cancellationToken);
        }
        #endregion

        #region Security Groups

        /// <summary />
        public virtual async Task<T> GetSecurityGroupAsync<T>(string securityGroupId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetSecurityGroupsRequest(securityGroupId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetSecurityGroupsRequest(string securityGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (securityGroupId == null)
                throw new ArgumentNullException("securityGroupId");

            return Endpoint.PrepareGetResourceRequest($"os-security-groups/{securityGroupId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateSecurityGroupAsync<T>(object securityGroup, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildCreateSecurityGroupRequest(securityGroup, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildCreateSecurityGroupRequest(object securityGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (securityGroup == null)
                throw new ArgumentNullException("securityGroup");

            return Endpoint.PrepareCreateResourceRequest("os-security-groups", securityGroup, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateSecurityGroupRuleAsync<T>(object rule, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildCreateSecurityGroupRuleRequest(rule, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildCreateSecurityGroupRuleRequest(object rule, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (rule == null)
                throw new ArgumentNullException("rule");

            return Endpoint.PrepareCreateResourceRequest("os-security-group-rules", rule, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListSecurityGroupsAsync<T>(string serverId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListSecurityGroupsRequest(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListSecurityGroupsRequest(string serverId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = serverId == null ? "os-security-groups" : $"servers/{serverId}/os-security-groups";
            return Endpoint.PrepareGetResourceRequest(path, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> UpdateSecurityGroupAsync<T>(string securityGroupId, object securityGroup, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildUpdateSecurityGroupRequest(securityGroupId, securityGroup, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildUpdateSecurityGroupRequest(string securityGroupId, object securityGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (securityGroupId == null)
                throw new ArgumentNullException("securityGroupId");

            if (securityGroup == null)
                throw new ArgumentNullException("securityGroup");

            return Endpoint.PrepareUpdateResourceRequest($"os-security-groups/{securityGroupId}", securityGroup, cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteSecurityGroupAsync(string securityGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteSecurityGroupRequest(securityGroupId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteSecurityGroupRequest(string securityGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (securityGroupId == null)
                throw new ArgumentNullException("securityGroupId");

            return Endpoint.PrepareDeleteResourceRequest($"os-security-groups/{securityGroupId}", cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteSecurityGroupRuleAsync(string ruleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteSecurityGroupRuleRequest(ruleId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteSecurityGroupRuleRequest(string ruleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ruleId == null)
                throw new ArgumentNullException("ruleId");

            return Endpoint.PrepareDeleteResourceRequest($"os-security-group-rules/{ruleId}", cancellationToken);
        }

        #endregion

        #region Server Groups

        /// <summary />
        public virtual async Task<T> GetServerGroupAsync<T>(string serverGroupId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetServerGroupsRequest(serverGroupId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetServerGroupsRequest(string serverGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverGroupId == null)
                throw new ArgumentNullException("serverGroupId");

            return Endpoint.PrepareGetResourceRequest($"os-server-groups/{serverGroupId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateServerGroupAsync<T>(object serverGroup, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildCreateServerGroupRequest(serverGroup, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildCreateServerGroupRequest(object serverGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverGroup == null)
                throw new ArgumentNullException("serverGroup");

            return Endpoint.PrepareCreateResourceRequest("os-server-groups", serverGroup, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListServerGroupsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListServerGroupsRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListServerGroupsRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("os-server-groups", cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteServerGroupAsync(string serverGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteServerGroupRequest(serverGroupId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteServerGroupRequest(string serverGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverGroupId == null)
                throw new ArgumentNullException("serverGroupId");

            return Endpoint.PrepareDeleteResourceRequest($"os-server-groups/{serverGroupId}", cancellationToken);
        }

        #endregion

        #region Volumes

        /// <summary />
        public virtual async Task<T> GetVolumeAsync<T>(string volumeId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetVolumeRequest(volumeId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetVolumeRequest(string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (volumeId == null)
                throw new ArgumentNullException("volumeId");

            return Endpoint.PrepareGetResourceRequest($"os-volumes/{volumeId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> GetVolumeTypeAsync<T>(string volumeTypeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await BuildGetVolumeTypeRequest(volumeTypeId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetVolumeTypeRequest(string volumeTypeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (volumeTypeId == null)
                throw new ArgumentNullException("volumeTypeId");

            return Endpoint.PrepareGetResourceRequest($"os-volume-types/{volumeTypeId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> GetVolumeSnapshotAsync<T>(string snapshotId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildGetVolumeSnapshotRequest(snapshotId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetVolumeSnapshotRequest(string snapshotId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (snapshotId == null)
                throw new ArgumentNullException("snapshotId");

            return Endpoint.PrepareGetResourceRequest($"os-snapshots/{snapshotId}", cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateVolumeAsync<T>(object volume, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildCreateVolumeRequest(volume, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildCreateVolumeRequest(object volume, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (volume == null)
                throw new ArgumentNullException("volume");

            return Endpoint.PrepareCreateResourceRequest("os-volumes", volume, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> SnapshotVolumeAsync<T>(object snapshot, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            return await BuildCreateVolumeSnapshotRequest(snapshot, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwner(this);
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildCreateVolumeSnapshotRequest(object snapshot, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(snapshot == null)
                throw new ArgumentNullException("snapshot");

            return await Endpoint.PrepareCreateResourceRequest("os-snapshots", snapshot, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListVolumesAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListVolumesRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListVolumesRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("os-volumes", cancellationToken);
        }

        ///// <summary />
        //public virtual async Task<T> ListVolumeTypesAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return await BuildListVolumeTypesRequest(cancellationToken)
        //        .SendAsync()
        //        .ReceiveJson<T>();
        //}

        ///// <summary />
        //public virtual Task<PreparedRequest> BuildListVolumeTypesRequest(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return Endpoint.PrepareGetResourceRequest("os-volume-types", cancellationToken);
        //}

        /// <summary />
        public virtual async Task<T> ListVolumeSnapshotsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            return await BuildListVolumeSnapshotsRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>()
                .PropogateOwnerToChildren(this);
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildListVolumeSnapshotsRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("os-snapshots", cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteVolumeAsync(string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteVolumeRequest(volumeId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteVolumeRequest(string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (volumeId == null)
                throw new ArgumentNullException("volumeId");

            return Endpoint.PrepareDeleteResourceRequest($"os-volumes/{volumeId}", cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteVolumeSnapshotAsync(string snapshotId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteVolumeSnapshotRequest(snapshotId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildDeleteVolumeSnapshotRequest(string snapshotId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (snapshotId == null)
                throw new ArgumentNullException("snapshotId");

            return Endpoint.PrepareDeleteResourceRequest($"os-snapshots/{snapshotId}", cancellationToken);
        }

        /// <summary>
        /// Waits for the volume to reach the specified status.
        /// </summary>
        /// <param name="volumeId">The volume identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TVolume> WaitForVolumeStatusAsync<TVolume, TStatus>(string volumeId, TStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TVolume : IServiceResource
            where TStatus : ResourceStatus
        {
            if(volumeId == null)
                throw new ArgumentNullException("volumeId");

            if(status == null)
                throw new ArgumentNullException("status");

            Func<Task<TVolume>> getVolume = async () => await GetVolumeAsync<TVolume>(volumeId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(volumeId, status, getVolume, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the volume snapshot to reach the specified status.
        /// </summary>
        /// <param name="snapshotId">The snapshot identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TSnapshot> WaitForVolumeSnapshotStatusAsync<TSnapshot, TStatus>(string snapshotId, TStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TSnapshot : IServiceResource
            where TStatus : ResourceStatus
        {
            if (snapshotId == null)
                throw new ArgumentNullException("snapshotId");

            if (status == null)
                throw new ArgumentNullException("status");

            Func<Task<TSnapshot>> getSnapshot = async () => await GetVolumeSnapshotAsync<TSnapshot>(snapshotId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(snapshotId, status, getSnapshot, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the volume to reach the specified status.
        /// </summary>
        /// <param name="volumeId">The volume identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TVolume> WaitForVolumeStatusAsync<TVolume, TStatus>(string volumeId, IEnumerable<TStatus> status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TVolume : IServiceResource
            where TStatus : ResourceStatus
        {
            if (volumeId == null)
                throw new ArgumentNullException("volumeId");

            if (status == null)
                throw new ArgumentNullException("status");

            Func<Task<TVolume>> getVolume = async () => await GetVolumeAsync<TVolume>(volumeId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(volumeId, status, getVolume, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the volume snapshot to reach the specified status.
        /// </summary>
        /// <param name="snapshotId">The snapshot identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<TSnapshot> WaitForVolumeSnapshotStatusAsync<TSnapshot, TStatus>(string snapshotId, IEnumerable<TStatus> status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TSnapshot : IServiceResource
            where TStatus : ResourceStatus
        {
            if (snapshotId == null)
                throw new ArgumentNullException("snapshotId");

            if (status == null)
                throw new ArgumentNullException("status");

            Func<Task<TSnapshot>> getSnapshot = async () => await GetVolumeSnapshotAsync<TSnapshot>(snapshotId, cancellationToken);
            return await Endpoint.WaitForStatusAsync(snapshotId, status, getSnapshot, refreshDelay, timeout, progress, cancellationToken)
                .PropogateOwner(this);
        }

        /// <summary>
        /// Waits for the volume to be deleted.
        /// <para>Treats a 404 NotFound exception as confirmation that it is deleted.</para>
        /// </summary>
        /// <param name="volumeId">The volume identifier.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public Task WaitUntilVolumeIsDeletedAsync<TVolume, TStatus>(string volumeId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TVolume : IServiceResource
            where TStatus : ResourceStatus
        {
            if (volumeId == null)
                throw new ArgumentNullException("volumeId");
            
            Func<Task<dynamic>> getVolume = async () => await GetVolumeAsync<TVolume>(volumeId, cancellationToken);
            return Endpoint.WaitUntilDeletedAsync<TStatus>(volumeId, getVolume, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <summary>
        /// Waits for the volume snapshot to be deleted.
        /// <para>Treats a 404 NotFound exception as confirmation that it is deleted.</para>
        /// </summary>
        /// <param name="snapshotId">The snapshot identifier.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public Task WaitUntilVolumeSnapshotIsDeletedAsync<TVolume, TStatus>(string snapshotId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TVolume : IServiceResource
            where TStatus : ResourceStatus
        {
            if (snapshotId == null)
                throw new ArgumentNullException("snapshotId");

            Func<Task<dynamic>> getSnapshot = async () => await GetVolumeSnapshotAsync<TVolume>(snapshotId, cancellationToken);
            return Endpoint.WaitUntilDeletedAsync<TStatus>(snapshotId, getSnapshot, refreshDelay, timeout, progress, cancellationToken);
        }
        #endregion

        #region Compute Service
        /// <summary />
        public virtual Task<T> GetLimitsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildGetLimitsRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetLimitsRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("limits", cancellationToken);
        }

        /// <summary />
        public virtual Task<T> GetCurrentQuotasAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildGetCurrentQuotasRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetCurrentQuotasRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("os-quota-sets/details", cancellationToken);
        }

        /// <summary />
        public virtual Task<T> GetDefaultQuotasAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildGetDefaultQuotasRequest(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual Task<PreparedRequest> BuildGetDefaultQuotasRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Endpoint.PrepareGetResourceRequest("os-quota-sets/defaults", cancellationToken);
        }
        #endregion
    }
}
