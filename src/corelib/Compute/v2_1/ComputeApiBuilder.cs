using System;
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

        private void SetOwner(IServiceResource<ComputeApiBuilder> resource)
        {
            resource.Owner = this;
        }

        #region Servers

        /// <summary />
        public virtual async Task<T> GetServerAsync<T>(string serverId, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource<ComputeApiBuilder>
        {
            PreparedRequest request = await BuildGetServerAsync(serverId, cancellationToken);
            var result = await request.SendAsync().ReceiveJson<T>();
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
                .PrepareGet(cancellationToken);
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildCreateServerAsync(object server, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return endpoint
                .AppendPathSegments("servers")
                .Authenticate(AuthenticationProvider)
                .PreparePostJson(server, cancellationToken);
        }

        /// <summary />
        public virtual async Task<T> CreateServerAsync<T>(object server, CancellationToken cancellationToken = default(CancellationToken))
            where T : IServiceResource<ComputeApiBuilder>
        {
            PreparedRequest request = await BuildCreateServerAsync(server, cancellationToken);
            var result = await request.SendAsync().ReceiveJson<T>();
            SetOwner(result);
            return result;
        }

        /// <summary>
        /// Waits for the server to become active.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<Server> WaitUntilServerIsActiveAsync(string serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(serverId))
                throw new ArgumentNullException("serverId");

            refreshDelay = refreshDelay ?? TimeSpan.FromSeconds(5);
            timeout = timeout ?? TimeSpan.FromMinutes(5);

            using (var timeoutSource = new CancellationTokenSource(timeout.Value))
            using (var rootCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token))
            {
                while (true)
                {
                    Server server = await GetServerAsync<Server>(serverId, cancellationToken).ConfigureAwait(false);
                    if (server.Status == ServerStatus.Error)
                        throw new ComputeOperationFailedException("TODO");

                    bool complete = server.Status == ServerStatus.Active;

                    progress?.Report(complete);

                    if (complete)
                        return server;

                    try
                    {
                        await Task.Delay(refreshDelay.Value, rootCancellationToken.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (timeoutSource.IsCancellationRequested)
                            throw new TimeoutException($"The requested timeout of {timeout.Value.TotalSeconds} seconds has been reached while waiting for the server ({serverId}) to become active.", ex);

                        throw;
                    }
                }
            }
        }

        /// <summary />
        public virtual async Task<TPage>  ListServersAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>
        {
            Url initialRequestUrl = await BuildListServersUrlAsync(queryString, cancellationToken);
            return await ListServersAsync<TPage>(initialRequestUrl, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListServersAsync<TPage>(Url url, CancellationToken cancellationToken)
            where TPage : IPageBuilder<TPage>
        {
            var results = await url
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken)
                .SendAsync()
                .ReceiveJson<TPage>();

            results.SetNextPageHandler(ListServersAsync<TPage>);

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
            where TPage : IPageBuilder<TPage>
        {
            Url initialRequestUrl = await BuildListServerDetailsUrlAsync(queryString, cancellationToken);
            return await ListServersAsync<TPage>(initialRequestUrl, cancellationToken);
        }

        /// <summary />
        public virtual async Task<TPage> ListServerDetailsAsync<TPage>(Url url, CancellationToken cancellationToken)
            where TPage : IPageBuilder<TPage>
        {
            var results = await url
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PrepareGet(cancellationToken)
                .SendAsync()
                .ReceiveJson<TPage>();

            results.SetNextPageHandler(ListServerDetailsAsync<TPage>);

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
        public virtual async Task<T> GetVncConsoleAsync<T>(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            PreparedRequest request = await BuildGetVncConsoleRequestAsync(serverId, type, cancellationToken);
            return await request.SendAsync().ReceiveJson<T>();
        }

        /// <summary />
        public virtual async Task<PreparedRequest> BuildGetVncConsoleRequestAsync(string serverId, object type, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await UrlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            var request = JObject.Parse($"{{ 'os-getVNCConsole': {{ 'type': '{type}' }} }}");
            return endpoint
                .AppendPathSegments("servers", serverId, "action")
                .Authenticate(AuthenticationProvider)
                .SetMicroversion(this)
                .PreparePostJson(request, cancellationToken);
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
