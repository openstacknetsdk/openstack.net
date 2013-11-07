using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CancellationToken = System.Threading.CancellationToken;
using Encoding = System.Text.Encoding;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Adds common functionality for all Rackspace Providers.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public abstract class ProviderBase<TProvider>
        where TProvider : class
    {
        /// <summary>
        /// The <see cref="IIdentityProvider"/> to use for authenticating requests to this provider.
        /// </summary>
        protected readonly IIdentityProvider IdentityProvider;

        /// <summary>
        /// The REST service implementation to use for requests sent from this provider.
        /// </summary>
        protected readonly IRestService RestService;

        /// <summary>
        /// The default identity to use when none is specified in the specific request.
        /// </summary>
        protected readonly CloudIdentity DefaultIdentity;

        /// <summary>
        /// The validator to use for determining whether a particular HTTP status code represents
        /// a success or failure.
        /// </summary>
        protected readonly IHttpResponseCodeValidator ResponseCodeValidator;

        /// <summary>
        /// This is the backing field for <see cref="ConnectionLimit"/>.
        /// </summary>
        private int? _connectionLimit;

        /// <summary>
        /// This is the backing field for <see cref="DefaultRegion"/>.
        /// </summary>
        private string _defaultRegion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderBase{TProvider}"/> class using
        /// the specified default identity, default region, identity provider, and REST service
        /// implementation, and the default HTTP response code validator.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="defaultRegion">The default region to use for calls that do not explicitly specify a region. If this value is <c>null</c>, the default region for the user will be used; otherwise if the service uses region-specific endpoints all calls must specify an explicit region.</param>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created using <paramref name="defaultIdentity"/> as the default identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        protected ProviderBase(CloudIdentity defaultIdentity, string defaultRegion, IIdentityProvider identityProvider, IRestService restService)
            : this(defaultIdentity, defaultRegion, identityProvider, restService, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderBase{TProvider}"/> class
        /// using the specified values.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="defaultRegion">The default region to use for calls that do not explicitly specify a region. If this value is <c>null</c>, the default region for the user will be used; otherwise if the service uses region-specific endpoints all calls must specify an explicit region.</param>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created using <paramref name="defaultIdentity"/> as the default identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="httpStatusCodeValidator">The HTTP status code validator to use. If this value is <c>null</c>, the provider will use <see cref="HttpResponseCodeValidator.Default"/>.</param>
        protected ProviderBase(CloudIdentity defaultIdentity, string defaultRegion,  IIdentityProvider identityProvider, IRestService restService, IHttpResponseCodeValidator httpStatusCodeValidator)
        {
            DefaultIdentity = defaultIdentity;
            _defaultRegion = defaultRegion;
            IdentityProvider = identityProvider ?? this as IIdentityProvider ?? new CloudIdentityProvider(defaultIdentity);
            RestService = restService ?? new JsonRestServices();
            ResponseCodeValidator = httpStatusCodeValidator ?? HttpResponseCodeValidator.Default;
        }

        /// <summary>
        /// This event is fired immediately before sending an asynchronous web request.
        /// </summary>
        /// <preliminary/>
        public event EventHandler<WebRequestEventArgs> BeforeAsyncWebRequest;

        /// <summary>
        /// This event is fired when the result of an asynchronous web request is received.
        /// </summary>
        /// <preliminary/>
        public event EventHandler<WebResponseEventArgs> AfterAsyncWebResponse;

        /// <summary>
        /// Gets or sets the maximum number of connections allowed on the <see cref="ServicePoint"/>
        /// objects used for requests. If the value is <c>null</c>, the connection limit value for the
        /// <see cref="ServicePoint"/> object is not altered.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is less than or equal to 0.</exception>
        public int? ConnectionLimit
        {
            get
            {
                return _connectionLimit;
            }

            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                _connectionLimit = value;
            }
        }

        /// <summary>
        /// Gets the default region for this provider instance, if one was specified.
        /// </summary>
        /// <value>
        /// The default region to use for API calls where an explicit region is not specified in the call;
        /// or <c>null</c> to use the default region associated with the identity making the call.
        /// </value>
        public string DefaultRegion
        {
            get
            {
                return _defaultRegion;
            }
        }

        /// <summary>
        /// Execute a REST request with an <see cref="object"/> body and strongly-typed result.
        /// </summary>
        /// <remarks>
        /// If the request fails due to an authorization failure, i.e. the <see cref="Response.StatusCode"/> is <see cref="HttpStatusCode.Unauthorized"/>,
        /// the request is attempted a second time.
        ///
        /// <para>This method calls <see cref="CheckResponse"/>, which results in a <see cref="ResponseException"/> if the request fails.</para>
        ///
        /// <para>This method uses <see cref="IRestService.Execute{T}(Uri, HttpMethod, string, Dictionary{string, string}, Dictionary{string, string}, RequestSettings)"/> to handle the underlying REST request(s).</para>
        /// </remarks>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="absoluteUri">The absolute URI for the request.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
        /// </param>
        /// <param name="queryStringParameter">
        /// A collection of parameters to add to the query string portion of the request
        /// URI. This parameter is optional. If the value is <c>null</c>, no parameters are
        /// added to the query string.
        /// </param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. This parameter
        /// is optional. If the value is <c>null</c>, no custom headers are added to the HTTP
        /// request.
        /// </param>
        /// <param name="isRetry"><c>true</c> if this request is retrying a previously failed operation; otherwise, <c>false</c>.</param>
        /// <param name="isTokenRequest"><c>true</c> if this is an authentication request; otherwise, <c>false</c>. Authentication requests do not perform separate authentication prior to the call.</param>
        /// <param name="settings">
        /// The settings to use for the request. This parameter is optional. If the value
        /// is <c>null</c>, <see cref="BuildDefaultRequestSettings"/> will be called to
        /// provide the settings.
        /// </param>
        /// <returns>
        /// Returns a <see cref="Response{T}"/> object containing the HTTP status code,
        /// headers, body, and strongly-typed data from the REST response.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="absoluteUri"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="method"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, bool isTokenRequest = false, RequestSettings settings = null)
        {
            if (absoluteUri == null)
                throw new ArgumentNullException("absoluteUri");
            CheckIdentity(identity);

            return ExecuteRESTRequest<Response<T>>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, isTokenRequest, settings, RestService.Execute<T>);
        }

        /// <summary>
        /// Execute a REST request with an <see cref="object"/> body and basic result (text or no content).
        /// </summary>
        /// <remarks>
        /// If the request fails due to an authorization failure, i.e. the <see cref="Response.StatusCode"/> is <see cref="HttpStatusCode.Unauthorized"/>,
        /// the request is attempted a second time.
        ///
        /// <para>This method calls <see cref="CheckResponse"/>, which results in a <see cref="ResponseException"/> if the request fails.</para>
        ///
        /// <para>This method uses <see cref="IRestService.Execute(Uri, HttpMethod, string, Dictionary{string, string}, Dictionary{string, string}, RequestSettings)"/> to handle the underlying REST request(s).</para>
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="absoluteUri">The absolute URI for the request.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
        /// </param>
        /// <param name="queryStringParameter">
        /// A collection of parameters to add to the query string portion of the request
        /// URI. This parameter is optional. If the value is <c>null</c>, no parameters are
        /// added to the query string.
        /// </param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. This parameter
        /// is optional. If the value is <c>null</c>, no custom headers are added to the HTTP
        /// request.
        /// </param>
        /// <param name="isRetry"><c>true</c> if this request is retrying a previously failed operation; otherwise, <c>false</c>.</param>
        /// <param name="isTokenRequest"><c>true</c> if this is an authentication request; otherwise, <c>false</c>. Authentication requests do not perform separate authentication prior to the call.</param>
        /// <param name="settings">
        /// The settings to use for the request. This parameter is optional. If the value
        /// is <c>null</c>, <see cref="BuildDefaultRequestSettings"/> will be called to
        /// provide the settings.
        /// </param>
        /// <returns>
        /// Returns a <see cref="Response"/> object containing the HTTP status code,
        /// headers, and body from the REST response.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="absoluteUri"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="method"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, bool isTokenRequest = false, RequestSettings settings = null)
        {
            if (absoluteUri == null)
                throw new ArgumentNullException("absoluteUri");
            CheckIdentity(identity);

            return ExecuteRESTRequest<Response>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, isTokenRequest, settings, RestService.Execute);
        }

        /// <summary>
        /// Execute a REST request with an <see cref="object"/> body and user-defined callback function
        /// for constructing the resulting <see cref="Response"/> object.
        /// </summary>
        /// <remarks>
        /// If the request fails due to an authorization failure, i.e. the <see cref="Response.StatusCode"/> is <see cref="HttpStatusCode.Unauthorized"/>,
        /// the request is attempted a second time.
        ///
        /// <para>This method calls <see cref="CheckResponse"/>, which results in a <see cref="ResponseException"/> if the request fails.</para>
        ///
        /// <para>This method uses <see cref="IRestService.Execute(Uri, HttpMethod, Func{HttpWebResponse, bool, Response}, string, Dictionary{string, string}, Dictionary{string, string}, RequestSettings)"/> to handle the underlying REST request(s).</para>
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="absoluteUri">The absolute URI for the request.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="buildResponseCallback">
        /// A user-specified function used to construct the resulting <see cref="Response"/>
        /// object from the <see cref="HttpWebResponse"/> and a Boolean value specifying
        /// whether or not a <see cref="WebException"/> was thrown during the request. If
        /// this value is <c>null</c>, this method is equivalent to calling
        /// <see cref="ExecuteRESTRequest(CloudIdentity, Uri, HttpMethod, object, Dictionary{string, string}, Dictionary{string, string}, bool, bool, RequestSettings)"/>.
        /// </param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
        /// </param>
        /// <param name="queryStringParameter">
        /// A collection of parameters to add to the query string portion of the request
        /// URI. This parameter is optional. If the value is <c>null</c>, no parameters are
        /// added to the query string.
        /// </param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. This parameter
        /// is optional. If the value is <c>null</c>, no custom headers are added to the HTTP
        /// request.
        /// </param>
        /// <param name="isRetry"><c>true</c> if this request is retrying a previously failed operation; otherwise, <c>false</c>.</param>
        /// <param name="isTokenRequest"><c>true</c> if this is an authentication request; otherwise, <c>false</c>. Authentication requests do not perform separate authentication prior to the call.</param>
        /// <param name="settings">
        /// The settings to use for the request. This parameter is optional. If the value
        /// is <c>null</c>, <see cref="BuildDefaultRequestSettings"/> will be called to
        /// provide the settings.
        /// </param>
        /// <returns>A <see cref="Response"/> object containing the result of the REST call.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="absoluteUri"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="method"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, Func<HttpWebResponse, bool, Response> buildResponseCallback, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, bool isTokenRequest = false, RequestSettings settings = null)
        {
            if (absoluteUri == null)
                throw new ArgumentNullException("absoluteUri");
            CheckIdentity(identity);

            return ExecuteRESTRequest<Response>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, isTokenRequest, settings,
                (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => RestService.Execute(uri, requestMethod, buildResponseCallback, requestBody, requestHeaders, requestQueryParams, requestSettings));
        }

        /// <summary>
        /// Execute a REST request, using a callback method to deserialize the result into a <see cref="Response"/> or <see cref="Response{T}"/> object.
        /// </summary>
        /// <remarks>
        /// If the request fails due to an authorization failure, i.e. the <see cref="Response.StatusCode"/> is <see cref="HttpStatusCode.Unauthorized"/>,
        /// the request is attempted a second time.
        ///
        /// <para>This method calls <see cref="CheckResponse"/>, which results in a <see cref="ResponseException"/> if the request fails.</para>
        /// </remarks>
        /// <typeparam name="T">The <see cref="Response"/> type used for representing the response to the REST call.</typeparam>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="absoluteUri">The absolute URI for the request.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
        /// </param>
        /// <param name="queryStringParameter">
        /// A collection of parameters to add to the query string portion of the request
        /// URI. This parameter is optional. If the value is <c>null</c>, no parameters are
        /// added to the query string.
        /// </param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. This parameter
        /// is optional. If the value is <c>null</c>, no custom headers are added to the HTTP
        /// request.
        /// </param>
        /// <param name="isRetry"><c>true</c> if this request is retrying a previously failed operation; otherwise, <c>false</c>.</param>
        /// <param name="isTokenRequest"><c>true</c> if this is an authentication request; otherwise, <c>false</c>. Authentication requests do not perform separate authentication prior to the call.</param>
        /// <param name="requestSettings">
        /// The settings to use for the request. This parameter is optional. If the value
        /// is <c>null</c>, <see cref="BuildDefaultRequestSettings"/> will be called to
        /// provide the settings.
        /// </param>
        /// <param name="callback">A callback function that prepares and executes the HTTP request, and returns the deserialized result as an object of type <typeparamref name="T"/>.</param>
        /// <returns>A response object of type <typeparamref name="T"/> containing the result of the REST call.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="absoluteUri"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="method"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        private T ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body, Dictionary<string, string> queryStringParameter, Dictionary<string, string> headers, bool isRetry, bool isTokenRequest, RequestSettings requestSettings,
            Func<Uri, HttpMethod, string, Dictionary<string, string>, Dictionary<string, string>, RequestSettings, T> callback) where T : Response
        {
            if (absoluteUri == null)
                throw new ArgumentNullException("absoluteUri");
            CheckIdentity(identity);

            identity = GetDefaultIdentity(identity);

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            if (headers == null)
                headers = new Dictionary<string, string>();

            if (!isTokenRequest)
                headers["X-Auth-Token"] = IdentityProvider.GetToken(identity, isRetry).Id;

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else if (body is string)
                    bodyStr = body as string;
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            if (string.IsNullOrEmpty(requestSettings.UserAgent))
                requestSettings.UserAgent = UserAgentGenerator.UserAgent;

            var response = callback(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == HttpStatusCode.Unauthorized && !isRetry && !isTokenRequest)
            {
                return ExecuteRESTRequest<T>(identity, absoluteUri, method, body, queryStringParameter, headers, true, isTokenRequest, requestSettings, callback);
            }

            CheckResponse(response);

            return response;
        }

        /// <summary>
        /// Executes a streaming REST request with a <see cref="Stream"/> and basic result (text or no content).
        /// </summary>
        /// <remarks>
        /// If the request fails due to an authorization failure, i.e. the <see cref="Response.StatusCode"/> is <see cref="HttpStatusCode.Unauthorized"/>,
        /// the request is attempted a second time.
        ///
        /// <para>This method uses an HTTP request timeout of 4 hours.</para>
        ///
        /// <para>This method calls <see cref="CheckResponse"/>, which results in a <see cref="ResponseException"/> if the request fails.</para>
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="absoluteUri">The absolute URI for the request.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="stream">A stream providing the body of the request.</param>
        /// <param name="chunkSize">The size of the buffer used for copying data from <paramref name="stream"/> to the HTTP request stream.</param>
        /// <param name="maxReadLength">The maximum number of bytes to send with the request. This parameter is optional. If the value is 0, the request will include all data from <paramref name="stream"/>.</param>
        /// <param name="queryStringParameter">
        /// A collection of parameters to add to the query string portion of the request
        /// URI. This parameter is optional. If the value is <c>null</c>, no parameters are
        /// added to the query string.
        /// </param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. This parameter
        /// is optional. If the value is <c>null</c>, no custom headers are added to the HTTP
        /// request.
        /// </param>
        /// <param name="isRetry"><c>true</c> if this request is retrying a previously failed operation; otherwise, <c>false</c>.</param>
        /// <param name="requestSettings">
        /// The settings to use for the request. This parameter is optional. If the value
        /// is <c>null</c>, <see cref="BuildDefaultRequestSettings"/> will be called to
        /// provide the settings.
        /// </param>
        /// <param name="progressUpdated">A user-defined callback function for reporting progress of the send operation. This parameter is optional. If the value is <c>null</c>, the method does not report progress updates to the caller.</param>
        /// <returns>
        /// Returns a <see cref="Response"/> object containing the HTTP status code,
        /// headers, and body from the REST response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="absoluteUri"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="chunkSize"/> is less than or equal to 0.
        /// <para>-or-</para>
        /// <para>If <paramref name="maxReadLength"/> is less than 0.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="method"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected Response StreamRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, Stream stream, int chunkSize, long maxReadLength = 0, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, RequestSettings requestSettings = null, Action<long> progressUpdated = null)
        {
            if (absoluteUri == null)
                throw new ArgumentNullException("absoluteUri");
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException("chunkSize");
            if (maxReadLength < 0)
                throw new ArgumentOutOfRangeException("maxReadLength");
            CheckIdentity(identity);

            identity = GetDefaultIdentity(identity);

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            requestSettings.Timeout = TimeSpan.FromMilliseconds(14400000); // Need to pass this in.

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers["X-Auth-Token"] = IdentityProvider.GetToken(identity, isRetry).Id;

            if (string.IsNullOrEmpty(requestSettings.UserAgent))
                requestSettings.UserAgent = UserAgentGenerator.UserAgent;

            var response = RestService.Stream(absoluteUri, method, stream, chunkSize, maxReadLength, headers, queryStringParameter, requestSettings, progressUpdated);

            // on errors try again 1 time.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (!isRetry)
                {
                    return StreamRESTRequest(identity, absoluteUri, method, stream, chunkSize, maxReadLength, queryStringParameter, headers, isRetry, requestSettings, progressUpdated);
                }
            }

            CheckResponse(response);

            return response;
        }

        /// <summary>
        /// Gets the default <see cref="RequestSettings"/> object to use for REST requests sent by this provider.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a <see cref="JsonRequestSettings"/> object initialized with the following values.
        ///
        /// <list type="bullet">
        /// <item>The <see cref="RequestSettings.RetryCount"/> is 2.</item>
        /// <item>The <see cref="RequestSettings.RetryDelay"/> is 200 milliseconds.</item>
        /// <item>The <see cref="RequestSettings.Non200SuccessCodes"/> contains <see cref="HttpStatusCode.Unauthorized"/> and <see cref="HttpStatusCode.Conflict"/>, along with the values in <paramref name="non200SuccessCodes"/> (if any).</item>
        /// <item>The <see cref="RequestSettings.UserAgent"/> is set to <see cref="UserAgentGenerator.UserAgent"/>.</item>
        /// <item>Other properties are set to the default values for <see cref="JsonRequestSettings"/>.</item>
        /// </list>
        ///
        /// <note type="implement">The caller may directly modify the object returned by this call, so a new object should be returned each time this method is called.</note>
        /// </remarks>
        /// <param name="non200SuccessCodes">A collection of non-200 HTTP status codes to consider as "success" codes for the request. This value may be <c>null</c> or an empty collection to use the default value.</param>
        /// <returns>A <see cref="RequestSettings"/> object containing the default settings to use for a REST request sent by this provider.</returns>
        protected virtual RequestSettings BuildDefaultRequestSettings(IEnumerable<HttpStatusCode> non200SuccessCodes = null)
        {
            var non200SuccessCodesAggregate = new List<HttpStatusCode>{ HttpStatusCode.Unauthorized, HttpStatusCode.Conflict };
            if(non200SuccessCodes != null)
                non200SuccessCodesAggregate.AddRange(non200SuccessCodes);

            return new JsonRequestSettings
            {
                RetryCount = 2,
                RetryDelay = TimeSpan.FromMilliseconds(200),
                Non200SuccessCodes = non200SuccessCodesAggregate,
                UserAgent = UserAgentGenerator.UserAgent,
                ConnectionLimit = ConnectionLimit,
            };
        }

        /// <summary>
        /// Gets the <see cref="Endpoint"/> associated with the specified service in the user's service catalog.
        /// </summary>
        /// <remarks>
        /// The endpoint returned by this method may not be an exact match for all arguments to this method.
        /// This method filters the service catalog in the following order to locate an acceptable endpoint.
        /// If more than one acceptable endpoint remains after all filters are applied, it is unspecified
        /// which one is returned by this method.
        ///
        /// <list type="number">
        /// <item>This method only considers services which match the specified <paramref name="serviceType"/>.</item>
        /// <item>This method attempts to filter the remaining items to those matching <paramref name="serviceName"/>. If <paramref name="serviceName"/> is <c>null</c>, or if no services match the specified name, <em>this argument is ignored</em>.</item>
        /// <item>This method attempts to filter the remaining items to those matching <paramref name="region"/>. If <paramref name="region"/> is <c>null</c>, the user's default region is used. If no services match the specified region, <em>this argument is ignored</em>.</item>
        /// <item>If the <paramref name="region"/> argument is ignored as a result of the previous rule, this method filters the remaining items to only include region-independent endpoints, i.e. endpoints where <see cref="Endpoint.Region"/> is <c>null</c> or empty.</item>
        /// </list>
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="serviceType">The service type (see <see cref="ServiceCatalog.Type"/>).</param>
        /// <param name="serviceName">The preferred name of the service (see <see cref="ServiceCatalog.Name"/>).</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>An <see cref="Endpoint"/> object containing the details of the requested service.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c>, the service does not provide a region-independent endpoint, and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected Endpoint GetServiceEndpoint(CloudIdentity identity, string serviceType, string serviceName, string region)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (string.IsNullOrEmpty(serviceType))
                throw new ArgumentException("serviceType cannot be empty");
            CheckIdentity(identity);

            identity = GetDefaultIdentity(identity);

            var userAccess = IdentityProvider.GetUserAccess(identity);

            if (userAccess == null || userAccess.ServiceCatalog == null)
                throw new UserAuthenticationException("Unable to authenticate user and retrieve authorized service endpoints.");

            IEnumerable<ServiceCatalog> services = userAccess.ServiceCatalog.Where(sc => string.Equals(sc.Type, serviceType, StringComparison.OrdinalIgnoreCase));

            if (serviceName != null)
            {
                IEnumerable<ServiceCatalog> namedServices = services.Where(sc => string.Equals(sc.Name, serviceName, StringComparison.OrdinalIgnoreCase));
                if (namedServices.Any())
                    services = namedServices;
            }

            IEnumerable<Tuple<ServiceCatalog, Endpoint>> endpoints =
                services.SelectMany(service => service.Endpoints.Select(endpoint => Tuple.Create(service, endpoint)));

            string effectiveRegion = region;
            if (string.IsNullOrEmpty(effectiveRegion))
            {
                if (!string.IsNullOrEmpty(DefaultRegion))
                    effectiveRegion = DefaultRegion;
                else if (!string.IsNullOrEmpty(userAccess.User.DefaultRegion))
                    effectiveRegion = userAccess.User.DefaultRegion;
            }

            IEnumerable<Tuple<ServiceCatalog, Endpoint>> regionEndpoints =
                endpoints.Where(i => string.Equals(i.Item2.Region ?? string.Empty, effectiveRegion ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (regionEndpoints.Any())
                endpoints = regionEndpoints;
            else
                endpoints = endpoints.Where(i => string.IsNullOrEmpty(i.Item2.Region));

            if (effectiveRegion == null && !endpoints.Any())
                throw new NoDefaultRegionSetException("No region was provided, the service does not provide a region-independent endpoint, and there is no default region set for the user's account.");

            Tuple<ServiceCatalog, Endpoint> serviceEndpoint = endpoints.FirstOrDefault();
            if (serviceEndpoint == null)
                throw new UserAuthorizationException("The user does not have access to the requested service or region.");

            return serviceEndpoint.Item2;
        }

        /// <summary>
        /// Gets the <see cref="Endpoint.PublicURL"/> for the <see cref="Endpoint"/> associated with the
        /// specified service in the user's service catalog.
        /// </summary>
        /// <remarks>
        /// For details on how endpoint resolution is performed, see <see cref="GetServiceEndpoint"/>.
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="serviceType">The service type (see <see cref="ServiceCatalog.Type"/>).</param>
        /// <param name="serviceName">The preferred name of the service (see <see cref="ServiceCatalog.Name"/>).</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>The <see cref="Endpoint.PublicURL"/> value for the <see cref="Endpoint"/> object containing the details of the requested service.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected virtual string GetPublicServiceEndpoint(CloudIdentity identity, string serviceType, string serviceName, string region)
        {
            var endpoint = GetServiceEndpoint(identity, serviceType, serviceName, region);

            return endpoint.PublicURL;
        }

        /// <summary>
        /// Gets the <see cref="Endpoint.InternalURL"/> for the <see cref="Endpoint"/> associated with the
        /// specified service in the user's service catalog.
        /// </summary>
        /// <remarks>
        /// For details on how endpoint resolution is performed, see <see cref="GetServiceEndpoint"/>.
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="serviceType">The service type (see <see cref="ServiceCatalog.Type"/>).</param>
        /// <param name="serviceName">The preferred name of the service (see <see cref="ServiceCatalog.Name"/>).</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>The <see cref="Endpoint.InternalURL"/> value for the <see cref="Endpoint"/> object containing the details of the requested service.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected virtual string GetInternalServiceEndpoint(CloudIdentity identity, string serviceType, string serviceName, string region)
        {
            var endpoint = GetServiceEndpoint(identity, serviceType, serviceName, region);

            return endpoint.InternalURL;
        }

        internal void CheckResponse(Response response)
        {
            ResponseCodeValidator.Validate(response);
        }

        /// <summary>
        /// Sets the <see cref="ProviderStateBase{TProvider}.Provider"/>, <see cref="ProviderStateBase{TProvider}.Region"/>,
        /// and <see cref="ProviderStateBase{TProvider}.Identity"/> properties of a collection of object to values
        /// matching the request parameters used when the objects were created.
        /// </summary>
        /// <typeparam name="T">The type of the provider-aware object to initialize.</typeparam>
        /// <param name="input">The collection of provider-aware objects.</param>
        /// <param name="region">The region used for the request that created this object.</param>
        /// <param name="identity">The identity used for the request that created this object.</param>
        /// <returns>This method returns an enumerable collection containing the objects in <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the current provider object is not an instance of <typeparamref name="TProvider"/>.</exception>
        protected IEnumerable<T> BuildCloudServersProviderAwareObject<T>(IEnumerable<T> input, string region, CloudIdentity identity)
            where T : ProviderStateBase<TProvider>
        {
            if (input == null)
                throw new ArgumentNullException("input");

            foreach (T obj in input)
            {
                if (obj == null)
                    continue;

                BuildCloudServersProviderAwareObject(obj, region, identity);
            }

            return input;
        }

        /// <summary>
        /// Sets the <see cref="ProviderStateBase{TProvider}.Provider"/>, <see cref="ProviderStateBase{TProvider}.Region"/>,
        /// and <see cref="ProviderStateBase{TProvider}.Identity"/> properties of an object to values
        /// matching the request parameters used when the object was created.
        /// </summary>
        /// <typeparam name="T">The type of the provider-aware object to initialize.</typeparam>
        /// <param name="input">The provider-aware object.</param>
        /// <param name="region">The region used for the request that created this object.</param>
        /// <param name="identity">The identity used for the request that created this object.</param>
        /// <returns>This method returns the <paramref name="input"/> argument.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the current provider object is not an instance of <typeparamref name="TProvider"/>.</exception>
        protected T BuildCloudServersProviderAwareObject<T>(T input, string region, CloudIdentity identity)
            where T : ProviderStateBase<TProvider>
        {
            if (input == null)
                throw new ArgumentNullException("input");

            TProvider provider = this as TProvider;
            if (provider == null)
                throw new InvalidOperationException(string.Format("The current provider type {0} does not implement the provider type {1} required for the provider-aware object.", GetType(), typeof(TProvider)));

            input.Provider = provider;
            input.Region = region;
            input.Identity = identity;
            return input;
        }

        /// <summary>
        /// Gets the effective cloud identity to use for a request based on the <paramref name="identity"/>
        /// argument and the configured default identity.
        /// </summary>
        /// <param name="identity">The explicitly specified identity for the request, or <c>null</c> if no identity was specified for the request.</param>
        /// <returns>The effective identity to use for the request, or <c>null</c> if <paramref name="identity"/> is <c>null</c> and no default identity is available.</returns>
        protected CloudIdentity GetDefaultIdentity(CloudIdentity identity)
        {
            if (identity != null)
                return identity;

            if (DefaultIdentity != null)
                return DefaultIdentity;

            return IdentityProvider.DefaultIdentity;
        }

        /// <summary>
        /// Creates a new dictionary from <paramref name="optionalParameters"/> with all
        /// entries with null or empty values removed.
        /// </summary>
        /// <param name="optionalParameters">The dictionary of optional parameters.</param>
        /// <returns>
        /// Returns a new dictionary created from <paramref name="optionalParameters"/> with all
        /// entries with <c>null</c> or empty values removed. If <paramref name="optionalParameters"/>
        /// is <c>null</c>, or if the resulting dictionary is empty, this method returns <c>null</c>.
        /// </returns>
        protected Dictionary<string, string> BuildOptionalParameterList(Dictionary<string, string> optionalParameters)
        {
            if (optionalParameters == null)
                return null;

            var paramList = optionalParameters.Where(optionalParameter => !string.IsNullOrEmpty(optionalParameter.Value)).ToDictionary(optionalParameter => optionalParameter.Key, optionalParameter => optionalParameter.Value, optionalParameters.Comparer);
            if (!paramList.Any())
                return null;

            return paramList;
        }

        /// <summary>
        /// Ensures that an identity is available for a request.
        /// </summary>
        /// <param name="identity">The explicitly specified identity for the request, or <c>null</c> if the request should use the default identity for the provider.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the request.</exception>
        protected virtual void CheckIdentity(CloudIdentity identity)
        {
            if (GetDefaultIdentity(identity) == null)
                throw new InvalidOperationException("No identity was specified for the request, and no default is available for the provider.");
        }

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpWebRequest"/> for use
        /// in asynchronous REST API calls. The input to the continuation function is a completed task which
        /// computes an <see cref="IdentityToken"/> for an authenticated user and a base URI for use in binding
        /// the URI templates for REST API calls. The continuation function calls <see cref="PrepareRequestImpl"/>
        /// to create and prepare the resulting <see cref="HttpWebRequest"/>.
        /// </summary>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to <see cref="UriTemplate.BindByName(Uri, IDictionary{string, string})"/>.</param>
        /// <param name="uriTransform">An optional transformation to apply to the bound URI for the request. If this value is <c>null</c>, the result of binding the <paramref name="template"/> with <paramref name="parameters"/> will be used as the absolute request URI.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpWebRequest"/> following the completion of a task that obtains an <see cref="IdentityToken"/> and the base URI for a service.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <c>null</c>.</para>
        /// </exception>
        /// <preliminary/>
        protected Func<Task<Tuple<IdentityToken, Uri>>, HttpWebRequest> PrepareRequestAsyncFunc(HttpMethod method, UriTemplate template, IDictionary<string, string> parameters, Func<Uri, Uri> uriTransform = null)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            return
                task =>
                {
                    Uri baseUri = task.Result.Item2;
                    return PrepareRequestImpl(method, task.Result.Item1, template, baseUri, parameters, uriTransform);
                };
        }

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpWebRequest"/> for use
        /// in asynchronous REST API calls. The input to the continuation function is a completed task which
        /// computes an <see cref="IdentityToken"/> for an authenticated user and a base URI for use in binding
        /// the URI templates for REST API calls. The continuation function calls <see cref="PrepareRequestImpl"/>
        /// to create and prepare the resulting <see cref="HttpWebRequest"/>, and then asynchronously obtains
        /// the request stream for the request and writes the specified <paramref name="body"/> in JSON notation.
        /// </summary>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to <see cref="UriTemplate.BindByName(Uri, IDictionary{string, string})"/>.</param>
        /// <param name="body">A object modeling the body of the web request. The object is serialized in JSON notation for inclusion in the request.</param>
        /// <param name="uriTransform">An optional transformation to apply to the bound URI for the request. If this value is <c>null</c>, the result of binding the <paramref name="template"/> with <paramref name="parameters"/> will be used as the absolute request URI.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpWebRequest"/> following the completion of a task that obtains an <see cref="IdentityToken"/> and the base URI for a service.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <c>null</c>.</para>
        /// </exception>
        /// <preliminary/>
        protected Func<Task<Tuple<IdentityToken, Uri>>, Task<HttpWebRequest>> PrepareRequestAsyncFunc<TBody>(HttpMethod method, UriTemplate template, IDictionary<string, string> parameters, TBody body, Func<Uri, Uri> uriTransform = null)
        {
            return
                task =>
                {
                    Uri baseUri = task.Result.Item2;
                    HttpWebRequest request = PrepareRequestImpl(method, task.Result.Item1, template, baseUri, parameters, uriTransform);

                    string bodyText = JsonConvert.SerializeObject(body);
                    byte[] encodedBody = Encoding.UTF8.GetBytes(bodyText);
                    request.ContentType = new ContentType() { MediaType = JsonRequestSettings.JsonContentType, CharSet = "UTF-8" }.ToString();
                    request.ContentLength = encodedBody.Length;

                    Task<Stream> streamTask = Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream(null, null), request.EndGetRequestStream);
                    return
                        streamTask.ContinueWith(subTask =>
                        {
                            using (Stream stream = subTask.Result)
                            {
                                stream.Write(encodedBody, 0, encodedBody.Length);
                            }

                            return request;
                        });
                };
        }

        /// <summary>
        /// Creates and prepares an <see cref="HttpWebRequest"/> for an asynchronous REST API call.
        /// </summary>
        /// <remarks>
        /// The base implementation sets the following properties of the web request.
        ///
        /// <list type="table">
        /// <listheader>
        /// <term>Property</term>
        /// <term>Value</term>
        /// </listheader>
        /// <item>
        /// <description><see cref="WebRequest.Method"/></description>
        /// <description><paramref name="method"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="HttpWebRequest.Accept"/></description>
        /// <description><see cref="JsonRequestSettings.JsonContentType"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WebRequest.Headers"/><literal>["X-Auth-Token"]</literal></description>
        /// <description><see name="IdentityToken.Id"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="HttpWebRequest.UserAgent"/></description>
        /// <description><see cref="UserAgentGenerator.UserAgent"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WebRequest.Timeout"/></description>
        /// <description>14400 seconds (4 hours)</description>
        /// </item>
        /// <item>
        /// <description><see cref="ServicePoint.ConnectionLimit"/></description>
        /// <description><see cref="ConnectionLimit"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="identityToken">The <see cref="IdentityToken"/> to use for making an authenticated REST API call.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="baseUri">The base URI to use for binding the URI template.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to <see cref="UriTemplate.BindByName(Uri, IDictionary{string, string})"/>.</param>
        /// <param name="uriTransform">An optional transformation to apply to the bound URI for the request. If this value is <c>null</c>, the result of binding the <paramref name="template"/> with <paramref name="parameters"/> will be used as the absolute request URI.</param>
        /// <returns>An <see cref="HttpWebRequest"/> to use for making the asynchronous REST API call.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="identityToken"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="template"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="baseUri"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="baseUri"/> is not an absolute URI.</exception>
        /// <preliminary/>
        protected virtual HttpWebRequest PrepareRequestImpl(HttpMethod method, IdentityToken identityToken, UriTemplate template, Uri baseUri, IDictionary<string, string> parameters, Func<Uri, Uri> uriTransform)
        {
            Uri boundUri = template.BindByName(baseUri, parameters);
            if (uriTransform != null)
                boundUri = uriTransform(boundUri);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(boundUri);
            request.Method = method.ToString().ToUpperInvariant();
            request.Accept = JsonRequestSettings.JsonContentType;
            request.Headers["X-Auth-Token"] = identityToken.Id;
            request.UserAgent = UserAgentGenerator.UserAgent;
            request.Timeout = (int)TimeSpan.FromSeconds(14400).TotalMilliseconds;
            if (ConnectionLimit.HasValue)
                request.ServicePoint.ConnectionLimit = ConnectionLimit.Value;

            return request;
        }

        /// <summary>
        /// Gets the base absolute URI to use for making asynchronous REST API calls to this service.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will contain
        /// a <see cref="Uri"/> representing the base absolute URI for the service.
        /// </returns>
        /// <preliminary/>
        protected virtual Task<Uri> GetBaseUriAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Authenticate with the identity service prior to making an asynchronous REST API call.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will contain
        /// a tuple containing the authentication information. The first element of the tuple is
        /// an <see cref="IdentityToken"/> for the authenticated user, and the second element is
        /// a base absolute <see cref="Uri"/> the service should use for making authenticated
        /// asynchronous web requests.
        /// </returns>
        /// <preliminary/>
        protected virtual Task<Tuple<IdentityToken, Uri>> AuthenticateServiceAsync(CancellationToken cancellationToken)
        {
            Task<IdentityToken> authenticate = Task.Factory.StartNew(() => IdentityProvider.GetToken(GetDefaultIdentity(null)));

            Func<Task<IdentityToken>, Task<Tuple<IdentityToken, Uri>>> getBaseUri =
                task =>
                {
                    Task[] tasks = { task, GetBaseUriAsync(cancellationToken) };
                    return Task.Factory.ContinueWhenAll(tasks,
                        ts =>
                        {
                            Task<IdentityToken> first = (Task<IdentityToken>)ts[0];
                            Task<Uri> second = (Task<Uri>)ts[1];
                            return Tuple.Create(first.Result, second.Result);
                        });
                };

            return authenticate.ContinueWith(getBaseUri).Unwrap();
        }

        /// <summary>
        /// Invokes the <see cref="BeforeAsyncWebRequest"/> event for the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The web request.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <c>null</c>.</exception>
        /// <preliminary/>
        protected virtual void OnBeforeAsyncWebRequest(HttpWebRequest request)
        {
            var handler = BeforeAsyncWebRequest;
            if (handler != null)
                handler(this, new WebRequestEventArgs(request));
        }

        /// <summary>
        /// Invokes the <see cref="AfterAsyncWebResponse"/> event for the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="response"/> is <c>null</c>.</exception>
        /// <preliminary/>
        protected virtual void OnAfterAsyncWebResponse(HttpWebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            var handler = AfterAsyncWebResponse;
            if (handler != null)
                handler(this, new WebResponseEventArgs(response));
        }

        /// <summary>
        /// Gets the response from an asynchronous web request, with the body of the response (if any) returned as a string.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A continuation function delegate which takes an asynchronously prepared <see cref="HttpWebRequest"/>
        /// and returns the resulting body of the operation, if any, as a string.
        /// </returns>
        /// <preliminary/>
        protected virtual Func<Task<HttpWebRequest>, Task<string>> GetResponseAsyncFunc(CancellationToken cancellationToken)
        {
            Func<Task<HttpWebRequest>, Task<WebResponse>> requestResource =
                task => RequestResourceImplAsync(task, cancellationToken);

            Func<Task<WebResponse>, Tuple<HttpWebResponse, string>> readResult =
                task => ReadResultImpl(task, cancellationToken);

            Func<Task<Tuple<HttpWebResponse, string>>, string> parseResult =
                task => task.Result.Item2;

            Func<Task<HttpWebRequest>, Task<string>> result =
                task =>
                {
                    return task.ContinueWith(requestResource).Unwrap()
                        .ContinueWith(readResult)
                        .ContinueWith(parseResult);
                };

            return result;
        }

        /// <summary>
        /// Gets the response from an asynchronous web request, with the body of the response (if any) returned as an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type for the response object.</typeparam>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="parseResult">
        /// A continuation function delegate which parses the body of the <see cref="HttpWebResponse"/>
        /// and returns an object of type <typeparamref name="T"/>, as an asynchronous operation. If
        /// this value is <c>null</c>, the conversion will be performed by calling <see cref="ParseJsonResultImplAsync{T}"/>.
        /// </param>
        /// <returns>
        /// A continuation function delegate which takes an asynchronously prepared <see cref="HttpWebRequest"/>
        /// and returns the resulting body of the operation, if any, as an instance of type <typeparamref name="T"/>.
        /// </returns>
        /// <preliminary/>
        protected virtual Func<Task<HttpWebRequest>, Task<T>> GetResponseAsyncFunc<T>(CancellationToken cancellationToken, Func<Task<Tuple<HttpWebResponse, string>>, Task<T>> parseResult = null)
        {
            Func<Task<HttpWebRequest>, Task<WebResponse>> requestResource =
                task => RequestResourceImplAsync(task, cancellationToken);

            Func<Task<WebResponse>, Tuple<HttpWebResponse, string>> readResult =
                task => ReadResultImpl(task, cancellationToken);

            if (parseResult == null)
            {
                parseResult = task => ParseJsonResultImplAsync<T>(task, cancellationToken);
            }

            Func<Task<HttpWebRequest>, Task<T>> result =
                task =>
                {
                    return task.ContinueWith(requestResource).Unwrap()
                        .ContinueWith(readResult)
                        .ContinueWith(parseResult).Unwrap();
                };

            return result;
        }

        /// <summary>
        /// This method calls <see cref="OnBeforeAsyncWebRequest"/> and then asynchronously gets the response
        /// to the web request.
        /// </summary>
        /// <remarks>
        /// This method is the first step of implementing <see cref="GetResponseAsyncFunc"/> and <see cref="GetResponseAsyncFunc{T}"/>.
        /// </remarks>
        /// <param name="task">A task which created and prepared the <see cref="HttpWebRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <c>null</c>.</exception>
        /// <preliminary/>
        protected virtual Task<WebResponse> RequestResourceImplAsync(Task<HttpWebRequest> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            OnBeforeAsyncWebRequest(task.Result);
            return task.Result.GetResponseAsync(cancellationToken);
        }

        /// <summary>
        /// This method reads the complete body of an asynchronous <see cref="WebResponse"/> as a string.
        /// </summary>
        /// <param name="task">A <see cref="Task"/> object representing the asynchronous operation to get the <see cref="WebResponse"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Tuple{T1, T2}"/> object. The first element of the tuple contains the
        /// <see cref="WebResponse"/> provided by <paramref name="task"/> as an <see cref="HttpWebResponse"/>.
        /// The second element of the tuple contains the complete body of the response as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <c>null</c>.</exception>
        /// <preliminary/>
        protected virtual Tuple<HttpWebResponse, string> ReadResultImpl(Task<WebResponse> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            HttpWebResponse response;
            WebException webException = null;
            if (task.IsFaulted)
            {
                webException = task.Exception.Flatten().InnerException as WebException;
                if (webException == null)
                    task.PropagateExceptions();

                response = webException.Response as HttpWebResponse;
                if (response == null)
                    task.PropagateExceptions();
            }
            else
            {
                response = (HttpWebResponse)task.Result;
            }

            OnAfterAsyncWebResponse(response);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string body = reader.ReadToEnd();
                if (task.IsFaulted)
                {
                    if (!string.IsNullOrEmpty(body))
                    {
                        WebExceptionStatus webExceptionStatus = webException != null ? webException.Status : WebExceptionStatus.UnknownError;
                        WebResponse webResponse = webException != null ? webException.Response : null;
                        throw new WebException(body, task.Exception, webExceptionStatus, webResponse);
                    }

                    task.PropagateExceptions();
                }

                return Tuple.Create(response, body);
            }
        }

        /// <summary>
        /// Provides a default object parser for <see cref="GetResponseAsyncFunc{T}"/> which converts the
        /// body of an <see cref="HttpWebResponse"/> to an object of type <typeparamref name="T"/> by calling
        /// <see cref="JsonConvert.DeserializeObject{T}(String)"/>
        /// </summary>
        /// <typeparam name="T">The type for the response object.</typeparam>
        /// <param name="task">A <see cref="Task"/> object representing the asynchronous operation to get the <see cref="WebResponse"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will contain an
        /// object of type <typeparamref name="T"/> representing the serialized body of the response.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <c>null</c>.</exception>
        /// <preliminary/>
        protected virtual Task<T> ParseJsonResultImplAsync<T>(Task<Tuple<HttpWebResponse, string>> task, CancellationToken cancellationToken)
        {
#if NET35
            return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(task.Result.Item2));
#else
            return JsonConvert.DeserializeObjectAsync<T>(task.Result.Item2);
#endif
        }
    }
}
