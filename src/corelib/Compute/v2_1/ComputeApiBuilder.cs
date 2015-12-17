using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Extensions;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using OpenStack.Authentication;
using OpenStack.Extensions;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Builds requests to the Compute API which can be further customized and then executed.
    /// <para>Intended for custom implementations.</para>
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-compute-v2.1.html">OpenStack Compute API v2.1 Overview</seealso>
    public class ComputeApiBuilder : ISupportMicroversions
    {
        /// <summary />
        protected readonly IAuthenticationProvider AuthenticationProvider;

        /// <summary />
        protected readonly ServiceUrlBuilder UrlBuilder;

        /// <summary />
        public ComputeApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region)
            : this(serviceType, authenticationProvider, region, "2.1")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeApiBuilder"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for the desired compute provider.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="microversion">The requested microversion.</param>
        protected ComputeApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, string microversion)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (authenticationProvider == null)
                throw new ArgumentNullException("authenticationProvider");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentException("region cannot be null or empty", "region");

            AuthenticationProvider = authenticationProvider;
            UrlBuilder = new ServiceUrlBuilder(serviceType, authenticationProvider, region);
            Microversion = microversion;
        }

        /// <summary />
        string ISupportMicroversions.MicroversionHeader => "X-OpenStack-Nova-API-Version";

        /// <summary />
        public string Microversion { get; }

        private void SetOwner(IServiceResource resource)
        {
            resource.PropogateOwner(this);
        }

        #region Servers

        /// <summary />
        public virtual async Task<T> GetServerAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildGetServerAsync(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments($"servers/{serverId}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildCreateServerAsync(object server, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("servers")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePostJson(server, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateServerAsync<T>(object server, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildCreateServerAsync(server, cancellationToken).SendAsync().ReceiveJson<T>();
            SetOwner(result);
            return result;
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
            Func<Task<dynamic>> getServer = async () => await GetServerAsync<TServer>(serverId, cancellationToken);
            return await ApiHelper.WaitForStatusAsync(serverId, status, getServer, refreshDelay, timeout, progress, cancellationToken);
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
            Func<Task<dynamic>> getServer = async () => await GetServerAsync<TServer>(serverId, cancellationToken);
            return await ApiHelper.WaitForStatusAsync(serverId, status, getServer, refreshDelay, timeout, progress, cancellationToken);
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
        public async Task WaitUntilServerIsDeletedAsync<TServer, TStatus>(string serverId, TStatus deletedStatus = null, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TServer : IServiceResource
            where TStatus : ResourceStatus
        {
            deletedStatus = deletedStatus ?? StringEnumeration.FromDisplayName<TStatus>("DELETED");
            Func<Task<dynamic>> getServer = async () => await GetServerAsync<TServer>(serverId, cancellationToken);
            await ApiHelper.WaitUntilDeletedAsync(serverId, deletedStatus, getServer, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage>  ListServersAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            Url initialRequestUrl = await BuildListServersUrlAsync(queryString, cancellationToken);
            return await ListServersAsync<TPage>(initialRequestUrl, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListServersAsync<TPage>(Url url, CancellationToken cancellationToken)
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            var results = await url
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken)
                .SendAsync()
                .ReceiveJson<TPage>();

            results.SetNextPageHandler(ListServersAsync<TPage>);
            SetOwner(results);

            return results;
        }

        /// <summary />
        public virtual async Task<Url> BuildListServersUrlAsync(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("servers")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary />
        public virtual async Task<TPage> ListServerDetailsAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            Url initialRequestUrl = await BuildListServerDetailsUrlAsync(queryString, cancellationToken);
            return await ListServersAsync<TPage>(initialRequestUrl, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListServerDetailsAsync<TPage>(Url url, CancellationToken cancellationToken)
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            var results = await url
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken)
                .SendAsync()
                .ReceiveJson<TPage>();

            results.SetNextPageHandler(ListServerDetailsAsync<TPage>);
            SetOwner(results);
            return results;
        }

        /// <summary />
        public virtual async Task<Url> BuildListServerDetailsUrlAsync(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("servers/detail")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildUpdateServerAsync(string serverId, object server, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments($"servers/{serverId}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePutJson(server, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> UpdateServerAsync<T>(string serverId, object server, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildUpdateServerAsync(serverId, server, cancellationToken).SendAsync().ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual Task DeleteServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteServerAsync(serverId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildDeleteServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("servers", serverId)
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
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
            Func<Task<dynamic>> getServer = async () => await GetImageAsync<TImage>(imageId, cancellationToken);
            return await ApiHelper.WaitForStatusAsync(imageId, status, getServer, refreshDelay, timeout, progress, cancellationToken);
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
        public async Task WaitUntilImageIsDeletedAsync<TImage, TStatus>(string imageId, TStatus deletedStatus, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TImage : IServiceResource
            where TStatus : ResourceStatus
        {
            deletedStatus = deletedStatus ?? StringEnumeration.FromDisplayName<TStatus>("DELETED");
            Func<Task<dynamic>> getServer = async () => await GetServerAsync<TImage>(imageId, cancellationToken);
            await ApiHelper.WaitUntilDeletedAsync(imageId, deletedStatus, getServer, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateSnapshotAsync<T>(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var response = await BuildServerActionAsync(serverId, request, cancellationToken).SendAsync();
            Identifier imageId = response.Headers.Location.Segments.Last(); // grab id off the end of the url, e.g. http://172.29.236.100:9292/images/baaab9b9-3635-429e-9969-2899a7cf2d97
            return await GetImageAsync<T>(imageId, cancellationToken);
        }

        /// <summary />
        public virtual Task StartServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new StartServerRequest();
            return BuildServerActionAsync(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task StopServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new StopServerRequest();
            return BuildServerActionAsync(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task RebootServerAsync<TRequest>(string serverId, TRequest request = null, CancellationToken cancellationToken = default(CancellationToken))
            where TRequest : class, new()
        {
            request = request ?? new TRequest();
            return BuildServerActionAsync(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task EvacuateServerAsync(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildServerActionAsync(serverId, request, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual Task<T> GetVncConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getVNCConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }
        
        /// <summary />
        public virtual Task<T> GetSpiceConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getSPICEConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }
        
        /// <summary />
        public virtual Task<T> GetSerialConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getSerialConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }
        
        /// <summary />
        public virtual Task<T> GetRdpConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getRDPConsole': {{ 'type': '{type}' }} }}");
            return BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }
        
        /// <summary />
        public virtual async Task<string> GetConsoleOutputAsync(string serverId, int length = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = JObject.Parse($"{{ 'os-getConsoleOutput': {{ 'length': '{length}' }} }}");
            dynamic result = await BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson();

            return result.output;
        }
        
        /// <summary />
        public virtual async Task<string> RescueServerAsync(string serverId, object request = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            request = request ?? new Dictionary<string, object> {["rescue"] = null};
            dynamic result = await BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson();

            return result.adminPass;
        }

        /// <summary />
        public virtual Task UnrescueServerAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            object request = new Dictionary<string, object> {["unrescue"] = null };
            return BuildServerActionAsync(serverId, request, cancellationToken)
                .SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildServerActionAsync(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("servers", serverId, "action")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePostJson(request, cancellationToken);
        }
        #endregion

        #region Flavors
        /// <summary />
        public virtual async Task<T> GetFlavorAsync<T>(string flavorId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildGetFlavorAsync(flavorId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetFlavorAsync(string flavorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments($"flavors/{flavorId}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListFlavorsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildListFlavorsAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildListFlavorsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("flavors")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListFlavorDetailsAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildListFlavorDetailsAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildListFlavorDetailsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("flavors/detail")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        #endregion

        #region Images
        /// <summary />
        public virtual async Task<T> GetImageAsync<T>(string imageId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildGetImageAsync(imageId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetImageAsync(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments($"images/{imageId}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> GetImageMetadataAsync<T>(string imageId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IChildResource
        {
            var result = await BuildGetImageMetadataAsync(imageId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            result.PropogateOwner(this);
            result.SetParent(imageId);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetImageMetadataAsync(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments($"images/{imageId}/metadata")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<string> GetImageMetadataItemAsync(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            dynamic result = await BuildGetImageMetadataItemAsync(imageId, key, cancellationToken)
                .SendAsync()
                .ReceiveJson();

            var meta = (IDictionary<string, object>)result.meta;
            return meta[key]?.ToString();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetImageMetadataItemAsync(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments($"images/{imageId}/metadata/{key}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual Task CreateImagMetadataAsync(string imageId, string key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildCreateImagMetadataAsync(imageId, key, value, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildCreateImagMetadataAsync(string imageId, string key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            var request = new
            {
                meta = new Dictionary<string, string>
                {
                    [key] = value
                }
            };

            return endpoint
                .AppendPathSegment($"images/{imageId}/metadata/{key}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePutJson(request, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListImagesAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            Url initialRequestUrl = await BuildListImagesUrlAsync(queryString, cancellationToken);
            return await ListImagesAsync<TPage>(initialRequestUrl, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListImagesAsync<TPage>(Url url, CancellationToken cancellationToken)
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            var results = await url
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken)
                .SendAsync()
                .ReceiveJson<TPage>();

            results.SetNextPageHandler(ListImagesAsync<TPage>);
            SetOwner(results);

            return results;
        }

        /// <summary />
        public virtual async Task<Url> BuildListImagesUrlAsync(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("images")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary />
        public virtual async Task<TPage> ListImageDetailsAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            Url initialRequestUrl = await BuildListImageDetailsUrlAsync(queryString, cancellationToken);
            return await ListImagesAsync<TPage>(initialRequestUrl, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListImageDetailsAsync<TPage>(Url url, CancellationToken cancellationToken)
            where TPage : IPageBuilder<TPage>, IServiceResource
        {
            var results = await url
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken)
                .SendAsync()
                .ReceiveJson<TPage>();

            results.SetNextPageHandler(ListImageDetailsAsync<TPage>);
            SetOwner(results);
            return results;
        }

        /// <summary />
        public virtual async Task<Url> BuildListImageDetailsUrlAsync(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("images/detail")
                .SetQueryParams(queryString?.Build());
        }

        /// <summary /> // this keeps existing, but omitted values
        public virtual async Task<T> UpdateImageMetadataAsync<T>(string imageId, object metadata, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            var result = await BuildUpdateImageMetadataAsync(imageId, metadata, overwrite, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildUpdateImageMetadataAsync(string imageId, object metadata, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            PreparedRequest request = endpoint
                .AppendPathSegments($"images/{imageId}/metadata")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this);

            if (overwrite)
                return request.PreparePutJson(metadata, cancellationToken);

            return request.PreparePostJson(metadata, cancellationToken);
        }

        /// <summary />
        public virtual Task DeleteImageAsync(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteImageAsync(imageId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildDeleteImageAsync(string imageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (imageId == null)
                throw new ArgumentNullException("imageId");

            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("images", imageId)
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
        }

        /// <summary />
        public virtual Task DeleteImageMetadataAsync(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDeleteImageMetadataAsync(imageId, key, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildDeleteImageMetadataAsync(string imageId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (imageId == null)
                throw new ArgumentNullException("imageId");

            if(key == null)
                throw new ArgumentNullException("key");

            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return (PreparedRequest)endpoint
                .AppendPathSegments("images", imageId, "metadata", key)
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareDelete(cancellationToken)
                .AllowHttpStatus(HttpStatusCode.NotFound);
        }
        #endregion

        #region IP Addresses

        /// <summary />
        public virtual async Task<IList<T>> GetServerAddressAsync<T>(string serverId, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await BuildGetServerAddressAsync(serverId, key, cancellationToken)
                .SendAsync()
                .ReceiveJson<IDictionary<string, IList<T>>>();

            return result[key];
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetServerAddressAsync(string serverid, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment($"servers/{serverid}/ips/{key}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual Task<T> ListServerAddressesAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildListServerAddressesAsync(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildListServerAddressesAsync(string serverid, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment($"servers/{serverid}/ips")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        #endregion

        #region Volumes
        /// <summary />
        public virtual async Task<T> GetServerVolumeAsync<T>(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IChildResource
        {
            var result = await BuildGetServerVolumeAsync(serverId, volumeId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            result.PropogateOwner(this);
            result.SetParent(serverId);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetServerVolumeAsync(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment($"servers/{serverId}/os-volume_attachments/{volumeId}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> ListServerVolumesAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IEnumerable<IServiceResource>
        {
            var result = await BuidListServerVolumesAsync(serverId, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            result.PropogateOwner(this);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuidListServerVolumesAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment($"servers/{serverId}/os-volume_attachments")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> AttachVolumeAsync<T>(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource
        {
            T result = await BuildAttachVolumeAsync(serverId, request, cancellationToken)
                .SendAsync()
                .ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildAttachVolumeAsync(string serverId, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment($"servers/{serverId}/os-volume_attachments")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePostJson(request, cancellationToken);
        }

        /// <summary />
        public virtual Task DetachVolumeAsync(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildDetachVolumeAsync(serverId, volumeId, cancellationToken).SendAsync();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildDetachVolumeAsync(string serverId, string volumeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (serverId == null)
                throw new ArgumentNullException("serverId");

            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment($"servers/{serverId}/os-volume_attachments/{volumeId}")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareDelete(cancellationToken);
        }
        #endregion

        #region Keypairs

        /// <summary />
        public virtual async Task<T> CreateKeyPairAsync<T>(object keypair, CancellationToken cancellationToken = default(CancellationToken))
        {
            PreparedRequest request = await BuildCreateKeyPairRequestAsync(keypair, cancellationToken);
            return await request.SendAsync().ReceiveJson<T>();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildCreateKeyPairRequestAsync(object keypair, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegment("os-keypairs")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePostJson(keypair, cancellationToken);
        }

        #endregion
    }
}
