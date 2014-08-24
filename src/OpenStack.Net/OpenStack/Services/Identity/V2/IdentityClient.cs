namespace OpenStack.Services.Identity.V2
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
    using Rackspace.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides the default implementation of the OpenStack Identity Service V2, with the core operations
    /// defined in the <see cref="IIdentityService"/> interface.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class IdentityClient : BaseIdentityClient, IIdentityService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityClient"/> class with the specified fixed base address.
        /// </summary>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public IdentityClient(Uri baseAddress)
            : base(baseAddress)
        {
        }

        /// <inheritdoc/>
        public Task<ListExtensionsApiCall> PrepareListExtensionsAsync(CancellationToken cancellationToken)
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
                                JObject extensionsObject = responseObject["extensions"] as JObject;
                                if (extensionsObject == null)
                                    return null;

                                JArray extensionsArray = extensionsObject["values"] as JArray;
                                if (extensionsArray == null)
                                    return null;

                                IList<Extension> list = extensionsArray.ToObject<Extension[]>();
                                // according to the available documentation, this call does not appear to be paginated
                                Func<CancellationToken, Task<ReadOnlyCollectionPage<Extension>>> getNextPageAsync = null;

                                ReadOnlyCollectionPage<Extension> results = new BasicReadOnlyCollectionPage<Extension>(list, getNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListExtensionsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public Task<GetExtensionApiCall> PrepareGetExtensionAsync(ExtensionAlias alias, CancellationToken cancellationToken)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            UriTemplate template = new UriTemplate("v2.0/extensions/{alias}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "alias", alias.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetExtensionApiCall(CreateJsonApiCall<ExtensionResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public Task<AuthenticateApiCall> PrepareAuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v2.0/tokens/");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AuthenticateApiCall(CreateJsonApiCall<AccessResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public Task<ListTenantsApiCall> PrepareListTenantsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
