namespace OpenStack.Services.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Net;
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
        /// <remarks>
        /// <para>This constructor initializes the identity client using the
        /// <see cref="PassThroughAuthenticationService"/>, which does not perform any authentication operations on HTTP
        /// API calls made through the client. For authenticating some (or all) requests made by this service, use the
        /// <see cref="BaseIdentityClient(IAuthenticationService, Uri)"/> constructor instead.</para>
        /// </remarks>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public BaseIdentityClient(Uri baseAddress)
            : this(new PassThroughAuthenticationService(baseAddress), baseAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIdentityClient"/> class with the specified fixed base
        /// address.
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
        protected BaseIdentityClient(IAuthenticationService authenticationService, Uri baseAddress)
            : base(CreateAuthenticationService(authenticationService, baseAddress), null)
        {
            _baseAddress = baseAddress;
        }

        /// <summary>
        /// Create an authentication service instance which distinguishes between the authenticated and unauthenticated
        /// HTTP API calls in the Identity Service.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <returns>The authentication service to use for this client.</returns>
        private static BaseIdentityClientAuthenticationService CreateAuthenticationService(IAuthenticationService authenticationService, Uri baseAddress)
        {
            BaseIdentityClientAuthenticationService result = authenticationService as BaseIdentityClientAuthenticationService;
            if (result != null)
                return result;

            return new BaseIdentityClientAuthenticationService(baseAddress, authenticationService, new PassThroughAuthenticationService(baseAddress));
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

        /// <inheritdoc/>
        /// <remarks>
        /// <para>This method extends the base implementation by treating a response of
        /// <see cref="HttpStatusCode.MultipleChoices"/> as successful for the <see cref="ListApiVersionsApiCall"/> API
        /// call.</para>
        /// </remarks>
        protected override Task<HttpResponseMessage> ValidateResultImplAsync(Task<HttpResponseMessage> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            // Handle the Multiple Choices status for the List API Versions call
            if (task.Result.RequestMessage.RequestUri.Equals(BaseAddress) && task.Result.StatusCode == HttpStatusCode.MultipleChoices)
                return task;

            return base.ValidateResultImplAsync(task, cancellationToken);
        }

        /// <summary>
        /// This class provides support for using <see cref="DelegatingPartialAuthenticationService"/> in Identity
        /// Service clients derived from <see cref="BaseIdentityClient"/>.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        protected class BaseIdentityClientAuthenticationService : DelegatingPartialAuthenticationService
        {
            /// <summary>
            /// The absolute base URI of the Identity Service endpoint.
            /// </summary>
            private readonly Uri _baseAddress;

            /// <summary>
            /// Initializes a new instance of the <see cref="BaseIdentityClientAuthenticationService"/> class with the
            /// specified delegate authentication service implementations to use for authenticating calls made from a
            /// client.
            /// </summary>
            /// <param name="baseAddress">The base address of the Identity Service.</param>
            /// <param name="authenticatedCallsService">
            /// The authentication service to use for authenticated HTTP API calls.
            /// </param>
            /// <param name="unauthenticatedCallsService">
            /// The authentication service to use for unauthenticated HTTP API calls.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// <para>If <paramref name="baseAddress"/> is <see langword="null"/>.</para>
            /// <para>-or-</para>
            /// <para>If <paramref name="authenticatedCallsService"/> is <see langword="null"/>.</para>
            /// <para>-or-</para>
            /// <para>If <paramref name="unauthenticatedCallsService"/> is <see langword="null"/>.</para>
            /// </exception>
            public BaseIdentityClientAuthenticationService(Uri baseAddress, IAuthenticationService authenticatedCallsService, IAuthenticationService unauthenticatedCallsService)
                : base(authenticatedCallsService, unauthenticatedCallsService)
            {
                if (baseAddress == null)
                    throw new ArgumentNullException("baseAddress");

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
            /// <remarks>
            /// <para>The base implementation implements this method by returning false for the HTTP API calls
            /// associated with <see cref="ListApiVersionsApiCall"/> and <see cref="GetApiVersionApiCall"/>, and
            /// otherwise returns <see langword="null"/>.</para>
            /// </remarks>
            protected override bool? IsAuthenticatedCall(HttpRequestMessage requestMessage)
            {
                if (requestMessage == null)
                    throw new ArgumentNullException("requestMessage");

                // normalize the request URI
                Uri relativeUri = BaseAddress.MakeRelativeUri(requestMessage.RequestUri);
                if (relativeUri.IsAbsoluteUri)
                    return null;

                Uri normalizedUri = new Uri(new Uri("http://localhost"), relativeUri);

                // the only unauthenticated calls are / and /{apiVersion}
                string[] segments = normalizedUri.GetSegments();
                if (segments.Length <= 2)
                    return false;

                return null;
            }
        }
    }
}
