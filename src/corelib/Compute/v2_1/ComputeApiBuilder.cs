using System;
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

        #region Servers

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
