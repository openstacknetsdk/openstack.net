namespace OpenStack.Services.Identity.V2
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

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityClient"/> class with the specified fixed base address.
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
        public IdentityClient(IAuthenticationService authenticationService, Uri baseAddress)
            : base(CreateAuthenticationService(authenticationService, baseAddress), baseAddress)
        {
        }

        /// <summary>
        /// Create an authentication service instance which distinguishes between the authenticated and unauthenticated
        /// HTTP API calls in the Identity Service.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <returns>The authentication service to use for this client.</returns>
        private static IdentityClientAuthenticationService CreateAuthenticationService(IAuthenticationService authenticationService, Uri baseAddress)
        {
            IdentityClientAuthenticationService result = authenticationService as IdentityClientAuthenticationService;
            if (result != null)
                return result;

            return new IdentityClientAuthenticationService(baseAddress, authenticationService, new PassThroughAuthenticationService(baseAddress));
        }

        /// <inheritdoc/>
        public virtual Task<ListExtensionsApiCall> PrepareListExtensionsAsync(CancellationToken cancellationToken)
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

        /// <inheritdoc/>
        public virtual Task<GetExtensionApiCall> PrepareGetExtensionAsync(ExtensionAlias alias, CancellationToken cancellationToken)
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
        public virtual Task<AuthenticateApiCall> PrepareAuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v2.0/tokens/");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AuthenticateApiCall(CreateJsonApiCall<AccessResponse>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListTenantsApiCall> PrepareListTenantsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v2.0/tenants");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Tenant>>> deserializeResult =
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
                                JArray tenantsArray = responseObject["tenants"] as JArray;
                                if (tenantsArray == null)
                                    return null;

                                IList<Tenant> list = tenantsArray.ToObject<Tenant[]>();
                                // http://docs.openstack.org/api/openstack-identity-service/2.0/content/Paginated_Collections-d1e325.html
                                Func<CancellationToken, Task<ReadOnlyCollectionPage<Tenant>>> getNextPageAsync = null;
                                JArray tenantsLinksArray = responseObject["tenants_links"] as JArray;
                                if (tenantsLinksArray != null)
                                {
                                    IList<Link> tenantsLinks = tenantsLinksArray.ToObject<Link[]>();
                                    Link nextLink = tenantsLinks.FirstOrDefault(i => string.Equals("next", i.Relation, StringComparison.OrdinalIgnoreCase));
                                    if (nextLink != null)
                                    {
                                        getNextPageAsync =
                                            nextCancellationToken =>
                                            {
                                                return PrepareListTenantsAsync(nextCancellationToken)
                                                    .WithUri(nextLink.Target)
                                                    .Then(nextApiCall => nextApiCall.Result.SendAsync(nextCancellationToken))
                                                    .Select(nextApiCallResult => nextApiCallResult.Result.Item2);
                                            };
                                    }
                                }

                                ReadOnlyCollectionPage<Tenant> results = new BasicReadOnlyCollectionPage<Tenant>(list, getNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListTenantsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual TExtension GetServiceExtension<TExtension>(ServiceExtensionDefinition<IIdentityService, TExtension> definition)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            return definition.CreateDefaultInstance(this);
        }

        /// <summary>
        /// This class extends the support of <see cref="IdentityClientAuthenticationService"/> to include support
        /// for <see cref="IdentityClient"/> calls.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        protected class IdentityClientAuthenticationService : BaseIdentityClientAuthenticationService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IdentityClientAuthenticationService"/> class with the
            /// specified delegate authentication service implementations to use for authenticating calls made from a
            /// client.
            /// </summary>
            /// <inheritdoc/>
            public IdentityClientAuthenticationService(Uri baseAddress, IAuthenticationService authenticatedCallsService, IAuthenticationService unauthenticatedCallsService)
                : base(baseAddress, authenticatedCallsService, unauthenticatedCallsService)
            {
            }

            /// <inheritdoc/>
            /// <remarks>
            /// <para>The <see cref="IdentityClientAuthenticationService"/> implementation of this method treats the
            /// unauthenticated calls inherited from <see cref="BaseIdentityClient"/> along with
            /// <see cref="AuthenticateApiCall"/>, <see cref="ListExtensionsApiCall"/>, and
            /// <see cref="GetExtensionApiCall"/> as unauthenticated. All other calls are treated as
            /// authenticated.</para>
            /// </remarks>
            protected override bool? IsAuthenticatedCall(HttpRequestMessage requestMessage)
            {
                if (requestMessage == null)
                    throw new ArgumentNullException("requestMessage");

                bool? baseResult = base.IsAuthenticatedCall(requestMessage);
                if (baseResult.HasValue)
                    return baseResult;

                // normalize the request URI
                Uri relativeUri = BaseAddress.MakeRelativeUri(requestMessage.RequestUri);
                if (relativeUri.IsAbsoluteUri)
                    return null;

                Uri normalizedUri = new Uri(new Uri("http://localhost"), relativeUri);
                string[] segments = normalizedUri.GetSegments();
                if (segments.Length < 3)
                {
                    // these are handled by the base client, if at all
                    return null;
                }

                // if the second segment isn't "v2.0/", then it's not a known call to this service
                if (!string.Equals("v2.0/", segments[1], StringComparison.Ordinal))
                {
                    return null;
                }

                // authenticate isn't authenticated
                if (string.Equals(segments[2], "tokens", StringComparison.Ordinal)
                    || string.Equals(segments[2], "tokens/", StringComparison.Ordinal))
                {
                    if (requestMessage.Method == HttpMethod.Post && segments.Length == 3)
                    {
                        return false;
                    }
                }

                // list extensions and get extension aren't authenticated
                if (string.Equals(segments[2], "extensions", StringComparison.Ordinal)
                    || string.Equals(segments[2], "extensions/", StringComparison.Ordinal))
                {
                    if (requestMessage.Method == HttpMethod.Get && segments.Length <= 4)
                        return false;
                }

                // all other calls are assumed to be authenticated
                return true;
            }
        }
    }
}
