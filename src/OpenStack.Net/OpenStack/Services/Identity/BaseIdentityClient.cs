namespace OpenStack.Services.Identity
{
    using System;
    using System.Collections.Generic;
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

    /// <summary>
    /// This class provides the default implementation of the <see cref="IBaseIdentityService"/>. It may be extended as
    /// part of implementing a specific version of the Identity Service, or used directly for obtaining information
    /// about versions of the Identity Services available at a specific endpoint address.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class BaseIdentityClient : ServiceClient, IBaseIdentityService
    {
        /// <summary>
        /// The absolute base URI of the Identity Service endpoint.
        /// </summary>
        private readonly Uri _baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIdentityClient"/> class with the specified fixed base
        /// address.
        /// </summary>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public BaseIdentityClient(Uri baseAddress)
            : base(new PassThroughAuthenticationService(baseAddress), null)
        {
            _baseAddress = baseAddress;
        }

        /// <summary>
        /// Gets the absolute base URI of the Identity Service endpoint.
        /// </summary>
        /// <value>
        /// The absolute base URI of the Identity Service endpoint.
        /// </value>
        protected Uri BaseAddress
        {
            get
            {
                return _baseAddress;
            }
        }

        /// <inheritdoc/>
        public virtual Task<ListApiVersionsApiCall> PrepareListApiVersionsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate(string.Empty);
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
                                JObject versionsObject = responseObject["versions"] as JObject;
                                if (versionsObject == null)
                                    return null;

                                JArray versionsArray = versionsObject["values"] as JArray;
                                if (versionsArray == null)
                                    return null;

                                IList<ApiVersion> list = versionsArray.ToObject<ApiVersion[]>();
                                // according to the available documentation, this call does not appear to be paginated
                                Func<CancellationToken, Task<ReadOnlyCollectionPage<ApiVersion>>> getNextPageAsync = null;

                                ReadOnlyCollectionPage<ApiVersion> results = new BasicReadOnlyCollectionPage<ApiVersion>(list, getNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListApiVersionsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetApiVersionApiCall> PrepareGetApiVersionAsync(ApiVersionId apiVersionId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("{version_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "version_id", apiVersionId.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetApiVersionApiCall(CreateJsonApiCall<ApiVersionResponse>(task.Result)));
        }

        /// <inheritdoc/>
        protected override Task<Uri> GetBaseUriAsyncImpl(CancellationToken cancellationToken)
        {
            return CompletedTask.FromResult(_baseAddress);
        }
    }
}
