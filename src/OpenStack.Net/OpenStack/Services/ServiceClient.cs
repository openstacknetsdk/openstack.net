namespace OpenStack.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using OpenStack.Net;
    using OpenStack.Threading;
    using Rackspace.Net;
    using Rackspace.Threading;
    using CancellationToken = System.Threading.CancellationToken;
    using Encoding = System.Text.Encoding;
    using IAuthenticationService = Security.Authentication.IAuthenticationService;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class provides a common implementation infrastructure for service clients in the OpenStack.Net SDK.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class ServiceClient : IHttpApiCallFactory, IHttpService
    {
        /// <summary>
        /// The <see cref="IAuthenticationService"/> to use for authenticating requests to this service.
        /// </summary>
        private readonly IAuthenticationService _authenticationService;

#if !PORTABLE
        /// <summary>
        /// This is the backing field for the <see cref="ConnectionLimit"/> property.
        /// </summary>
        private int? _connectionLimit;
#endif

        /// <summary>
        /// This is the backing field for the <see cref="DefaultRegion"/> property.
        /// </summary>
        private string _defaultRegion;

        /// <summary>
        /// This is the backing field for the <see cref="InternalUrl"/> property.
        /// </summary>
        private bool _internalUrl;

        /// <summary>
        /// This is the backing field for the <see cref="BackoffPolicy"/> property.
        /// </summary>
        private IBackoffPolicy _backoffPolicy;

        /// <summary>
        /// This is the backing field for the <see cref="HttpClient"/> property.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// This field caches the base URI used for accessing the service.
        /// </summary>
        /// <seealso cref="GetBaseUriAsync"/>
        private Uri _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClient"/> class using the specified authentication
        /// service and default region, and the default <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="defaultRegion">The preferred region for the service. Unless otherwise specified for a specific
        /// client, derived service clients will not use a default region if this value is <see langword="null"/> (i.e.
        /// only region-less or "global" service endpoints will be considered acceptable).</param>
        /// <param name="internalUrl">
        /// <para><see langword="true"/> to access the service using an endpoint on the local network.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> to access the service over a public network (the Internet).</para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="authenticationService"/> is <see langword="null"/>.
        /// </exception>
        protected ServiceClient(IAuthenticationService authenticationService, string defaultRegion, bool internalUrl)
        {
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");

            _authenticationService = authenticationService;
            _defaultRegion = defaultRegion;
            _internalUrl = internalUrl;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// This event is fired immediately before sending an asynchronous web request.
        /// </summary>
        public event EventHandler<HttpRequestEventArgs> BeforeAsyncWebRequest;

        /// <summary>
        /// This event is fired when the result of an asynchronous web request is received.
        /// </summary>
        public event EventHandler<HttpResponseEventArgs> AfterAsyncWebResponse;

#if !PORTABLE
        /// <summary>
        /// Gets or sets the maximum number of connections allowed on the <see cref="ServicePoint"/> objects used for
        /// requests. If the value is <see langword="null"/>, the connection limit value for the
        /// <see cref="ServicePoint"/> object is not altered.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/> is less than or equal to 0.
        /// </exception>
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
#endif

        /// <summary>
        /// Gets the default region for this provider instance, if one was specified.
        /// </summary>
        /// <value>
        /// The default region to use for API calls; or <see langword="null"/> to use the default region (if any)
        /// provided by the <see cref="IAuthenticationService"/> instance.
        /// </value>
        public string DefaultRegion
        {
            get
            {
                return _defaultRegion;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use a local network connection or a public network connection for
        /// accessing the service.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>Individual services may require a particular value be used for this property.</para>
        /// </note>
        /// </remarks>
        /// <value>
        /// <para><see langword="true"/> to access the service using an endpoint on the local network.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> to access the service over a public network (the Internet).</para>
        /// </value>
        public bool InternalUrl
        {
            get
            {
                return _internalUrl;
            }
        }

        /// <summary>
        /// Gets or sets the back-off policy to use for polling operations.
        /// </summary>
        /// <remarks>
        /// If this value is set to <see langword="null"/>, the default back-off policy for the current provider will be
        /// used.
        /// </remarks>
        /// <value>
        /// A <see cref="IBackoffPolicy"/> instance implementing the back-off policy for polling operations on the
        /// service.
        /// </value>
        public IBackoffPolicy BackoffPolicy
        {
            get
            {
                return _backoffPolicy ?? DefaultBackoffPolicy;
            }

            set
            {
                _backoffPolicy = value;
            }
        }

        /// <summary>
        /// Gets the default HTTP response validation method for requests sent to this service.
        /// </summary>
        /// <remarks>
        /// This property is intended to support extension methods which prepare <see cref="IHttpApiCall{T}"/> instances
        /// which are not supported by the default client. The validation behavior itself may be customized by
        /// overriding <see cref="ValidateResultImplAsync"/> in a particular service client.
        /// </remarks>
        public Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> DefaultResponseValidator
        {
            get
            {
                return ValidateResultImplAsync;
            }
        }

        /// <summary>
        /// Gets the authentication service to use for authenticating requests made to this service.
        /// </summary>
        /// <value>
        /// The authentication service to use for authenticating requests made to this service.
        /// </value>
        protected IAuthenticationService AuthenticationService
        {
            get
            {
                return _authenticationService;
            }
        }

        /// <summary>
        /// Gets the default back-off policy for the current provider.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="OpenStack.Threading.BackoffPolicy.Default"/>. This property
        /// may be overridden to change the default back-off policy.
        /// </remarks>
        protected virtual IBackoffPolicy DefaultBackoffPolicy
        {
            get
            {
                return OpenStack.Threading.BackoffPolicy.Default;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Net.Http.HttpClient"/> to use for sending requests to the server.
        /// </summary>
        /// <value>
        /// The <see cref="System.Net.Http.HttpClient"/> to use for sending HTTP requests.
        /// </value>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <see langword="null"/>.</exception>
        public HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _httpClient = value;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> if this method was called from <see cref="Dispose()"/>.
        /// <para>-or-</para>
        /// <para><see langword="false"/> if this method was called from a finalizer.</para>
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }
        }

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpRequestMessage"/> for use
        /// in asynchronous HTTP API calls.
        /// </summary>
        /// <remarks>
        /// <para>The input to the continuation function is a completed task which computes the base address for use in
        /// binding the URI templates for HTTP API calls. The continuation function calls
        /// <see cref="PrepareRequestImpl"/> to create and prepare the resulting <see cref="HttpRequestMessage"/>,
        /// followed by using the <see cref="AuthenticationService"/> to authenticate the request.</para>
        /// </remarks>
        /// <typeparam name="T">The type used for parameter values in binding the <see cref="UriTemplate"/>.</typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task created by the returned
        /// delegate will observe.</param>
        /// <param name="uriTransform">An optional transformation to apply to the bound URI for the request. If this
        /// value is <see langword="null"/>, the result of binding the <paramref name="template"/> with
        /// <paramref name="parameters"/> will be used as the absolute request URI.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpRequestMessage"/>
        /// following the completion of a task that obtains the base URI for a service endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        public Func<Task<Uri>, Task<HttpRequestMessage>> PrepareRequestAsyncFunc<T>(HttpMethod method, UriTemplate template, IDictionary<string, T> parameters, CancellationToken cancellationToken, Func<Uri, Uri> uriTransform)
        {
            var prepared = PrepareRequestAsyncFunc(method, template, parameters, cancellationToken);
            if (uriTransform == null)
                return prepared;

            return
                task =>
                {
                    return prepared(task).Select(
                        innerTask =>
                        {
                            innerTask.Result.RequestUri = uriTransform(innerTask.Result.RequestUri);
                            return innerTask.Result;
                        });
                };
        }

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpRequestMessage"/> for use
        /// in asynchronous HTTP API calls.
        /// </summary>
        /// <remarks>
        /// <para>The input to the continuation function is a completed task which computes the base address for use in
        /// binding the URI templates for HTTP API calls. The continuation function calls
        /// <see cref="PrepareRequestImpl"/> to create and prepare the resulting <see cref="HttpRequestMessage"/>,
        /// followed by using the <see cref="AuthenticationService"/> to authenticate the request.</para>
        /// </remarks>
        /// <typeparam name="T">The type used for parameter values in binding the <see cref="UriTemplate"/>.</typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task created by the returned
        /// delegate will observe.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpRequestMessage"/>
        /// following the completion of a task that obtains the base URI for a service endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        public Func<Task<Uri>, Task<HttpRequestMessage>> PrepareRequestAsyncFunc<T>(HttpMethod method, UriTemplate template, IDictionary<string, T> parameters, CancellationToken cancellationToken)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            return
                task =>
                {
                    Uri baseUri = task.Result;
                    HttpRequestMessage request = PrepareRequestImpl(method, template, baseUri, parameters);

                    Func<Task<HttpRequestMessage>, Task<HttpRequestMessage>> authenticateRequest =
                        task2 => AuthenticationService.AuthenticateRequestAsync(task2.Result, cancellationToken).Select(_ => task2.Result);
                    return CompletedTask.FromResult(request).Then(authenticateRequest);
                };
        }

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpRequestMessage"/> for use
        /// in asynchronous HTTP API calls.
        /// </summary>
        /// <remarks>
        /// <para>The input to the continuation function is a completed task which computes the base address for use in
        /// binding the URI templates for HTTP API calls. The continuation function calls
        /// <see cref="PrepareRequestImpl"/> to create and prepare the resulting <see cref="HttpRequestMessage"/>,
        /// followed by calling <see cref="EncodeRequestBodyImpl"/> to prepare the
        /// <see cref="HttpRequestMessage.Content"/> of the request and using the <see cref="AuthenticationService"/> to
        /// authenticate the request.</para>
        /// </remarks>
        /// <typeparam name="T">The type used for parameter values in binding the <see cref="UriTemplate"/>.</typeparam>
        /// <typeparam name="TBody">The type modeling the body of the request.</typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <param name="body">An object modeling the body of the web request.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task created by the returned
        /// delegate will observe.</param>
        /// <param name="uriTransform">An optional transformation to apply to the bound URI for the request. If this
        /// value is <see langword="null"/>, the result of binding the <paramref name="template"/> with
        /// <paramref name="parameters"/> will be used as the absolute request URI.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpRequestMessage"/>
        /// following the completion of a task that obtains the base URI for a service endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        public Func<Task<Uri>, Task<HttpRequestMessage>> PrepareRequestAsyncFunc<T, TBody>(HttpMethod method, UriTemplate template, IDictionary<string, T> parameters, TBody body, CancellationToken cancellationToken, Func<Uri, Uri> uriTransform)
        {
            var prepared = PrepareRequestAsyncFunc(method, template, parameters, body, cancellationToken);
            if (uriTransform == null)
                return prepared;

            return
                task =>
                {
                    return prepared(task).Select(
                        innerTask =>
                        {
                            innerTask.Result.RequestUri = uriTransform(innerTask.Result.RequestUri);
                            return innerTask.Result;
                        });
                };
        }

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpRequestMessage"/> for use
        /// in asynchronous HTTP API calls.
        /// </summary>
        /// <remarks>
        /// <para>The input to the continuation function is a completed task which computes the base address for use in
        /// binding the URI templates for HTTP API calls. The continuation function calls
        /// <see cref="PrepareRequestImpl"/> to create and prepare the resulting <see cref="HttpRequestMessage"/>,
        /// followed by calling <see cref="EncodeRequestBodyImpl"/> to prepare the
        /// <see cref="HttpRequestMessage.Content"/> of the request and using the <see cref="AuthenticationService"/> to
        /// authenticate the request.</para>
        /// </remarks>
        /// <typeparam name="T">The type used for parameter values in binding the <see cref="UriTemplate"/>.</typeparam>
        /// <typeparam name="TBody">The type modeling the body of the request.</typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <param name="body">An object modeling the body of the web request.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task created by the returned
        /// delegate will observe.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpRequestMessage"/>
        /// following the completion of a task that obtains the base URI for a service endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        public Func<Task<Uri>, Task<HttpRequestMessage>> PrepareRequestAsyncFunc<T, TBody>(HttpMethod method, UriTemplate template, IDictionary<string, T> parameters, TBody body, CancellationToken cancellationToken)
        {
            return
                task =>
                {
                    Uri baseUri = task.Result;
                    HttpRequestMessage request = PrepareRequestImpl(method, template, baseUri, parameters);
                    request.Content = EncodeRequestBodyImpl(request, body);

                    Func<Task<HttpRequestMessage>, Task<HttpRequestMessage>> authenticateRequest =
                        task2 => AuthenticationService.AuthenticateRequestAsync(task2.Result, cancellationToken).Select(_ => task2.Result);
                    return CompletedTask.FromResult(request).Then(authenticateRequest);
                };
        }

        /// <summary>
        /// Encode the body of a request, and update the <see cref="HttpRequestMessage"/> properties as necessary to
        /// support the encoded body.
        /// </summary>
        /// <remarks>
        /// The default implementation uses <see cref="JsonConvert"/> to convert <paramref name="body"/> to JSON
        /// notation, and then uses <see cref="Encoding.UTF8"/> to encode the text. The
        /// <see cref="HttpContentHeaders.ContentType"/> and <see cref="HttpContentHeaders.ContentLength"/> properties
        /// of the request are updated to reflect the JSON content, and the <see cref="HttpContent"/> object is
        /// returned.
        /// </remarks>
        /// <typeparam name="TBody">The type modeling the body of the request.</typeparam>
        /// <param name="request">The <see cref="HttpRequestMessage"/> object for the request.</param>
        /// <param name="body">The object modeling the body of the request.</param>
        /// <returns>An <see cref="HttpContent"/> object representing the body of the
        /// <see cref="HttpRequestMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <see langword="null"/>.</exception>
        protected virtual HttpContent EncodeRequestBodyImpl<TBody>(HttpRequestMessage request, TBody body)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string bodyText = JsonConvert.SerializeObject(body);
            byte[] encodedBody = Encoding.UTF8.GetBytes(bodyText);
            ByteArrayContent content = new ByteArrayContent(encodedBody);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };

            content.Headers.ContentLength = encodedBody.Length;

            return content;
        }

        /// <summary>
        /// Creates and prepares an <see cref="HttpRequestMessage"/> for an asynchronous HTTP API call.
        /// </summary>
        /// <remarks>
        /// <para>The base implementation sets the following properties of the web request.</para>
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
        /// <description><see cref="HttpRequestHeaders.Accept"/></description>
        /// <description><c>application/json</c></description>
        /// </item>
        /// <item>
        /// <description><see cref="HttpRequestHeaders.UserAgent"/></description>
        /// <description>
        /// A <see cref="ProductInfoHeaderValue"/> constructed from the <see cref="AssemblyProductAttribute"/> and
        /// <see cref="AssemblyInformationalVersionAttribute"/> values
        /// </description>
        /// </item>
        /// <item>
        /// <description><see cref="P:System.Net.ServicePoint.ConnectionLimit"/></description>
        /// <description><see cref="P:OpenStack.Services.ServiceClient.ConnectionLimit"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <typeparam name="T">The type used for parameter values in binding the <see cref="UriTemplate"/>.</typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="baseUri">The base URI to use for binding the URI template.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <returns>An <see cref="HttpRequestMessage"/> to use for making the asynchronous HTTP API call.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="template"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="baseUri"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="baseUri"/> is not an absolute URI.</exception>
        protected virtual HttpRequestMessage PrepareRequestImpl<T>(HttpMethod method, UriTemplate template, Uri baseUri, IDictionary<string, T> parameters)
        {
            Uri boundUri = template.BindByName(baseUri, parameters);

            HttpRequestMessage request = new HttpRequestMessage(method, boundUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(AssemblyInfo.AssemblyProduct, AssemblyInfo.AssemblyInformationalVersion));
#if !PORTABLE
            if (ConnectionLimit.HasValue)
            {
                ServicePoint servicePoint = ServicePointManager.FindServicePoint(boundUri);
                servicePoint.ConnectionLimit = ConnectionLimit.Value;
            }
#endif

            return request;
        }

        /// <summary>
        /// Gets the base absolute URI to use for making asynchronous HTTP API calls to this service.
        /// </summary>
        /// <remarks>
        /// <para>This method returns a cached base address if one is available. If no cached address is available,
        /// <see cref="GetBaseUriAsyncImpl"/> is called to obtain the <see cref="Uri"/>. The result is then cached for
        /// future use before returning.</para>
        ///
        /// <note type="inherit">
        /// Most service client implementations will override <see cref="GetBaseUriAsyncImpl"/> instead of this method.
        /// This method should only be overridden in order to alter the base URI caching strategy provided in the
        /// default implementation.
        /// </note>
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain a <see cref="Uri"/> representing the base
        /// absolute URI for the service.
        /// </returns>
        public virtual Task<Uri> GetBaseUriAsync(CancellationToken cancellationToken)
        {
            if (_baseUri != null)
            {
                return CompletedTask.FromResult(_baseUri);
            }

            return GetBaseUriAsyncImpl(cancellationToken)
                .Select(
                    task =>
                    {
                        _baseUri = task.Result;
                        return task.Result;
                    });
        }

        /// <summary>
        /// Gets the base absolute URI to use for making asynchronous HTTP API calls to this service.
        /// </summary>
        /// <remarks>
        /// This method provides the implementation for <see cref="GetBaseUriAsync"/>, and should not be called
        /// directly.
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain a <see cref="Uri"/> representing the base
        /// absolute URI for the service.
        /// </returns>
        protected abstract Task<Uri> GetBaseUriAsyncImpl(CancellationToken cancellationToken);

        /// <summary>
        /// Invokes the <see cref="BeforeAsyncWebRequest"/> event for the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The web request.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <see langword="null"/>.</exception>
        protected virtual void OnBeforeAsyncWebRequest(HttpRequestMessage request)
        {
            var handler = BeforeAsyncWebRequest;
            if (handler != null)
                handler(this, new HttpRequestEventArgs(request));
        }

        /// <summary>
        /// Invokes the <see cref="AfterAsyncWebResponse"/> event for the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="response"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void OnAfterAsyncWebResponse(Task<HttpResponseMessage> response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            var handler = AfterAsyncWebResponse;
            if (handler != null)
                handler(this, new HttpResponseEventArgs(response));
        }

        /// <summary>
        /// Gets the response from an asynchronous web request, with the body of the response (if any) returned as a
        /// string.
        /// </summary>
        /// <remarks>
        /// The default implementation calls <see cref="RequestResourceImplAsync"/> to perform the HTTP request,
        /// followed by <see cref="ReadResultImpl"/> to obtain the body of the response as a string.
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A continuation function delegate which takes an asynchronously prepared <see cref="HttpRequestMessage"/>,
        /// sends the request, and returns the resulting body of the operation, if any, as a string.
        /// </returns>
        protected virtual Func<Task<HttpRequestMessage>, Task<string>> GetResponseAsyncFunc(CancellationToken cancellationToken)
        {
            Func<Task<HttpRequestMessage>, Task<HttpResponseMessage>> requestResource =
                task => RequestResourceImplAsync(task, cancellationToken);

            Func<Task<HttpResponseMessage>, Task<Tuple<HttpResponseMessage, string>>> readResult =
                task => ReadResultImpl(task, cancellationToken);

            Func<Task<Tuple<HttpResponseMessage, string>>, string> parseResult =
                task => task.Result.Item2;

            Func<Task<HttpRequestMessage>, Task<string>> result =
                task =>
                {
                    return task.Then(requestResource)
                        .Then(readResult)
                        .Select(parseResult);
                };

            return result;
        }

        /// <summary>
        /// Gets the response from an asynchronous web request, with the body of the response (if any) returned as an
        /// object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type for the response object.</typeparam>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="parseResult">
        /// A continuation function delegate which parses the body of the <see cref="HttpResponseMessage"/>, as an
        /// asynchronous operation. The parameter to the function is a tuple containing the
        /// <see cref="HttpResponseMessage"/> as well as the raw content of the body as a string. It returns a
        /// <see cref="Task"/> representing the asynchronous operation; when the task is complete, the
        /// <see cref="Task{TResult}.Result"/> property contains an object of type <typeparamref name="T"/>. If this
        /// value is <see langword="null"/>, the conversion will be performed by
        /// <see cref="ParseJsonResultImplAsync{T}"/>.
        /// </param>
        /// <returns>
        /// A continuation function delegate which takes an asynchronously prepared <see cref="HttpRequestMessage"/>,
        /// sends the request, and returns the resulting body of the operation, if any, as an instance of type
        /// <typeparamref name="T"/>.
        /// </returns>
        protected virtual Func<Task<HttpRequestMessage>, Task<T>> GetResponseAsyncFunc<T>(CancellationToken cancellationToken, Func<Task<Tuple<HttpResponseMessage, string>>, Task<T>> parseResult = null)
        {
            Func<Task<HttpRequestMessage>, Task<HttpResponseMessage>> requestResource =
                task => RequestResourceImplAsync(task, cancellationToken);

            Func<Task<HttpResponseMessage>, Task<Tuple<HttpResponseMessage, string>>> readResult =
                task => ReadResultImpl(task, cancellationToken);

            if (parseResult == null)
            {
                parseResult = task => ParseJsonResultImplAsync<T>(task, cancellationToken);
            }

            Func<Task<HttpRequestMessage>, Task<T>> result =
                task =>
                {
                    return task.Then(requestResource)
                        .Then(readResult)
                        .Then(parseResult);
                };

            return result;
        }

        /// <summary>
        /// This method calls <see cref="OnBeforeAsyncWebRequest"/> and then asynchronously gets the response to the web
        /// request.
        /// </summary>
        /// <remarks>
        /// <para>This method is the first step of implementing <see cref="GetResponseAsyncFunc"/> and
        /// <see cref="GetResponseAsyncFunc{T}"/>.</para>
        /// <para>The default implementation uses the <see cref="HttpCompletionOption.ResponseContentRead"/> completion
        /// option. Service client implementations may override this method to use a different completion option if
        /// necessary.</para>
        /// </remarks>
        /// <param name="task">A task which created and prepared the <see cref="HttpRequestMessage"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation. When the task completes
        /// successfully, the <see cref="Task{TResult}.Result"/> property contains an instance of
        /// <see cref="HttpResponseMessage"/> representing the response to the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        protected virtual Task<HttpResponseMessage> RequestResourceImplAsync(Task<HttpRequestMessage> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            OnBeforeAsyncWebRequest(task.Result);
            return _httpClient.SendAsync(task.Result, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        /// <summary>
        /// This method verifies that an HTTP request was successful, and reads the complete body of the response as a
        /// string.
        /// </summary>
        /// <remarks>
        /// In the default implementation, the validation is performed by <see cref="ValidateResultImplAsync"/>, and the
        /// read operation is performed by <see cref="HttpContent.ReadAsStringAsync"/>.
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> object representing the asynchronous operation to get the
        /// <see cref="HttpResponseMessage"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains a tuple; the first element
        /// is the <see cref="HttpResponseMessage"/> obtained from <paramref name="task"/>, and the second element is
        /// the complete body of the response as a string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> provided by the antecedent <paramref name="task"/>
        /// indicates that the request failed.
        /// </exception>
        protected virtual Task<Tuple<HttpResponseMessage, string>> ReadResultImpl(Task<HttpResponseMessage> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            HttpResponseMessage response = task.Result;
            OnAfterAsyncWebResponse(task);
            return ValidateResultImplAsync(task, cancellationToken)
                .Then(innerTask => innerTask.Result.Content.ReadAsStringAsync())
                .Select(innerTask => Tuple.Create(response, innerTask.Result));
        }

        /// <summary>
        /// This method validates the response to an asynchronous web request was successful.
        /// </summary>
        /// <remarks>
        /// <para>The default implementation determines if the call was successful by checking the
        /// <see cref="HttpResponseMessage.IsSuccessStatusCode"/> property of the result message. This validation is
        /// performed synchronously, and upon success the method simply returns <paramref name="task"/>, which has
        /// already completed.</para>
        ///
        /// <note type="implement">
        /// Most overriding implementations will not need to transform the resulting <see cref="HttpResponseMessage"/>,
        /// and may simply return <paramref name="task"/> for efficiency. Another option in this case is to perform
        /// custom validation first, and if that validation succeeds return the result of the base implementation.
        /// </note>
        /// </remarks>
        /// <param name="task">The antecedent task, which provides the <see cref="HttpResponseMessage"/> from the
        /// HTTP request.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation completes
        /// successfully, the <see cref="Task{TResult}.Result"/> property will contain the
        /// <see cref="HttpResponseMessage"/> providing information about the result of the call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> provided by the antecedent <paramref name="task"/> indicates that
        /// the request failed.
        /// </exception>
        protected virtual Task<HttpResponseMessage> ValidateResultImplAsync(Task<HttpResponseMessage> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            if (task.Result.IsSuccessStatusCode)
                return task;

            throw new HttpWebException(task.Result);
        }

        /// <summary>
        /// Provides a default object parser for <see cref="GetResponseAsyncFunc{T}"/> which converts the body of an
        /// <see cref="HttpResponseMessage"/> to an object of type <typeparamref name="T"/> by calling
        /// <see cref="JsonConvert.DeserializeObject{T}(string)"/>
        /// </summary>
        /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
        /// <param name="task">A <see cref="Task"/> object representing the asynchronous operation to get the
        /// <see cref="HttpResponseMessage"/>, and the body of the response as a string.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation completes
        /// successfully, the <see cref="Task{TResult}.Result"/> property will contain an object of type
        /// <typeparamref name="T"/> representing the deserialized body of the response.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        protected virtual Task<T> ParseJsonResultImplAsync<T>(Task<Tuple<HttpResponseMessage, string>> task, CancellationToken cancellationToken)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return CompletedTask.Default.Select(_ => JsonConvert.DeserializeObject<T>(task.Result.Item2));
        }

        /// <summary>
        /// Create an HTTP API call where the response body is a JSON-serialized representation of an instance of
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API
        /// call.</param>
        /// <returns>An HTTP API call which deserializes the JSON response body to obtain the strongly-typed result of
        /// the call.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> returned by the call indicates that the request failed.
        /// </exception>
        protected virtual HttpApiCall<T> CreateJsonApiCall<T>(HttpRequestMessage requestMessage)
        {
            var result = new JsonHttpApiCall<T>(HttpClient, requestMessage, HttpCompletionOption.ResponseContentRead, ValidateResultImplAsync);
            return RegisterApiCall(result);
        }

        /// <summary>
        /// Create an HTTP API call which returns a <see cref="Stream"/> for reading the response body.
        /// </summary>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API
        /// call.</param>
        /// <returns>An HTTP API call which returns a <see cref="Stream"/> for reading the response body.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> returned by the call indicates that the request failed.
        /// </exception>
        protected virtual HttpApiCall<Stream> CreateStreamingApiCall(HttpRequestMessage requestMessage)
        {
            var result = new StreamingHttpApiCall(HttpClient, requestMessage, ValidateResultImplAsync);
            return RegisterApiCall(result);
        }

        /// <summary>
        /// Create an HTTP API call where the response body is a treated as a simple string.
        /// </summary>
        /// <remarks>
        /// This method can be used for API calls that do not return a body. In those cases, the response is treated as
        /// an empty string.
        /// </remarks>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API
        /// call.</param>
        /// <returns>An HTTP API call which returns the response body as a string.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> returned by the call indicates that the request failed.
        /// </exception>
        protected HttpApiCall CreateBasicApiCall(HttpRequestMessage requestMessage)
        {
            return CreateBasicApiCall(requestMessage, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Create an HTTP API call where the response body is a treated as a simple string.
        /// </summary>
        /// <remarks>
        /// This method can be used for API calls that do not return a body. In those cases, the response is treated as
        /// an empty string.
        /// </remarks>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API
        /// call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP
        /// request. The default value is <see cref="HttpCompletionOption.ResponseContentRead"/>.</param>
        /// <returns>An HTTP API call which returns the response body as a string.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="completionOption"/> is not a valid <see cref="HttpCompletionOption"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> returned by the call indicates that the request failed.
        /// </exception>
        protected virtual HttpApiCall CreateBasicApiCall(HttpRequestMessage requestMessage, HttpCompletionOption completionOption)
        {
            var result = new HttpApiCall(HttpClient, requestMessage, completionOption, ValidateResultImplAsync);
            return RegisterApiCall(result);
        }

        /// <summary>
        /// Create an HTTP API call where the deserialization logic is provided as a user-defined function.
        /// </summary>
        /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API
        /// call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP
        /// request.</param>
        /// <param name="deserializeResult">A user-defined function to perform deserialization of the HTTP
        /// response.</param>
        /// <returns>An HTTP API call which deserializes the response to an instance of <typeparamref name="T"/> using
        /// the user-defined function <paramref name="deserializeResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="deserializeResult"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="completionOption"/> is not a valid <see cref="HttpCompletionOption"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If the <see cref="HttpResponseMessage"/> returned by the call indicates that the request failed.
        /// </exception>
        protected virtual HttpApiCall<T> CreateCustomApiCall<T>(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<HttpResponseMessage, CancellationToken, Task<T>> deserializeResult)
        {
            var result = new CustomHttpApiCall<T>(HttpClient, requestMessage, completionOption, ValidateResultImplAsync, deserializeResult);
            return RegisterApiCall(result);
        }

        /// <summary>
        /// Associates an HTTP API request with the current service client.
        /// </summary>
        /// <remarks>
        /// The base implementation adds event handlers to the <see cref="IHttpApiRequest.BeforeAsyncWebRequest"/>
        /// and <see cref="IHttpApiRequest.AfterAsyncWebResponse"/> events to ensure the
        /// <see cref="ServiceClient.OnBeforeAsyncWebRequest"/> and <see cref="ServiceClient.OnAfterAsyncWebResponse"/>
        /// methods are invoked at the correct time.
        /// </remarks>
        /// <typeparam name="T">The specific (static) type of the HTTP API request.</typeparam>
        /// <param name="call">The HTTP API request instance.</param>
        /// <returns>The input argument <paramref name="call"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="call"/> is <see langword="null"/>.</exception>
        protected virtual T RegisterApiCall<T>(T call)
            where T : IHttpApiRequest
        {
            if (call == null)
                throw new ArgumentNullException("call");

            call.BeforeAsyncWebRequest += (sender, e) => OnBeforeAsyncWebRequest(e.Request);
            call.AfterAsyncWebResponse += (sender, e) => OnAfterAsyncWebResponse(e.Response);
            return call;
        }

        #region IHttpApiCallFactory Members

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation calls <see cref="CreateJsonApiCall{T}"/> to process the request.
        /// </remarks>
        HttpApiCall<T> IHttpApiCallFactory.CreateJsonApiCall<T>(HttpRequestMessage requestMessage)
        {
            return CreateJsonApiCall<T>(requestMessage);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation calls <see cref="CreateStreamingApiCall"/> to process the request.
        /// </remarks>
        HttpApiCall<Stream> IHttpApiCallFactory.CreateStreamingApiCall(HttpRequestMessage requestMessage)
        {
            return CreateStreamingApiCall(requestMessage);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation calls <see cref="CreateBasicApiCall(HttpRequestMessage, HttpCompletionOption)"/> to
        /// process the request.
        /// </remarks>
        HttpApiCall IHttpApiCallFactory.CreateBasicApiCall(HttpRequestMessage requestMessage, HttpCompletionOption completionOption)
        {
            return CreateBasicApiCall(requestMessage, completionOption);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation calls <see cref="CreateCustomApiCall{T}"/> to process the request.
        /// </remarks>
        HttpApiCall<T> IHttpApiCallFactory.CreateCustomApiCall<T>(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<HttpResponseMessage, CancellationToken, Task<T>> deserializeResult)
        {
            return CreateCustomApiCall<T>(requestMessage, completionOption, deserializeResult);
        }

        #endregion
    }
}
