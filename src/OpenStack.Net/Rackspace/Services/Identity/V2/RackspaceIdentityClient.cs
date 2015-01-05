namespace Rackspace.Services.Identity.V2
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
    using OpenStack.Services.Identity;
    using OpenStack.Services.Identity.V2;
    using Rackspace.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class extends the default <see cref="IdentityClient"/> implementation of OpenStack Identity Service V2 to
    /// address vendor-specific differences in various responses to HTTP API calls sent to the service.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RackspaceIdentityClient : IdentityClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RackspaceIdentityClient"/> class with the specified fixed base address.
        /// </summary>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public RackspaceIdentityClient(Uri baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RackspaceIdentityClient"/> class with the specified fixed base address.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public RackspaceIdentityClient(IAuthenticationService authenticationService, Uri baseAddress)
            : base(authenticationService, baseAddress)
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Rackspace sends back the <c>media-types</c> property value wrapped in an additional JSON object, as
        /// opposed to the array OpenStack uses. This method overrides the behavior of the base OpenStack client to
        /// replace the unexpected structure with the correct layout prior to deserialization.</para>
        /// <note type="note">
        /// <para>If required, the original response is available via the <see cref="HttpResponseMessage.Content"/>
        /// property of the response.</para>
        /// </note>
        /// </remarks>
        public override Task<GetApiVersionApiCall> PrepareGetApiVersionAsync(ApiVersionId apiVersionId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("{version_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "version_id", apiVersionId.Value } };

            Func<HttpResponseMessage, CancellationToken, Task<ApiVersionResponse>> deserializeResult =
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
                                if (responseObject == null)
                                    return null;

                                JObject versionObject = responseObject["version"] as JObject;
                                if (versionObject == null)
                                    return responseObject.ToObject<ApiVersionResponse>();

                                JToken mediaTypesToken = versionObject["media-types"];
                                if (mediaTypesToken is JArray)
                                    return responseObject.ToObject<ApiVersionResponse>();

                                JArray mediaTypesArray = mediaTypesToken["values"] as JArray;
                                if (mediaTypesArray == null)
                                    versionObject.Remove("media-types");
                                else
                                    versionObject["media-types"] = mediaTypesArray;

                                return responseObject.ToObject<ApiVersionResponse>();
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetApiVersionApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public override Task<ListExtensionsApiCall> PrepareListExtensionsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v2.0/extensions");
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

                                // Unlike OpenStack, Rackspace does not wrap the array into a child object 'extensions.values'
                                JArray extensionsArray = responseObject["extensions"] as JArray;
                                if (extensionsArray == null)
                                    return null;

                                IList<Extension> list = extensionsArray.ToObject<Extension[]>();
                                // http://docs.openstack.org/api/openstack-identity-service/2.0/content/Paginated_Collections-d1e325.html
                                Func<CancellationToken, Task<ReadOnlyCollectionPage<Extension>>> getNextPageAsync = null;
                                JArray extensionsLinksArray = responseObject["extensions_links"] as JArray;
                                if (extensionsLinksArray != null)
                                {
                                    IList<Link> extensionsLinks = extensionsLinksArray.ToObject<Link[]>();
                                    Link nextLink = extensionsLinks.FirstOrDefault(i => string.Equals("next", i.Relation, StringComparison.OrdinalIgnoreCase));
                                    if (nextLink != null)
                                    {
                                        getNextPageAsync =
                                            nextCancellationToken =>
                                            {
                                                return PrepareListExtensionsAsync(nextCancellationToken)
                                                    .WithUri(nextLink.Target)
                                                    .Then(nextApiCall => nextApiCall.Result.SendAsync(nextCancellationToken))
                                                    .Select(nextApiCallResult => nextApiCallResult.Result.Item2);
                                            };
                                    }
                                }

                                ReadOnlyCollectionPage<Extension> results = new BasicReadOnlyCollectionPage<Extension>(list, getNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListExtensionsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }
    }
}
