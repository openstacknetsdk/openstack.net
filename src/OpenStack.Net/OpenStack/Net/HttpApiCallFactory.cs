namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class provides a default implementation of <see cref="IHttpApiCallFactory"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class HttpApiCallFactory : IHttpApiCallFactory
    {
        /// <summary>
        /// This is the backing field for the <see cref="HttpClient"/> property.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpApiCallFactory"/> class
        /// using a new instance of <see cref="System.Net.Http.HttpClient"/> as the HTTP client.
        /// </summary>
        public HttpApiCallFactory()
            : this(new HttpClient())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpApiCallFactory"/> class
        /// with the specified <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> instance to use when creating API calls.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="httpClient"/> is <see langword="null"/>.</exception>
        public HttpApiCallFactory(HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> to use when creating API
        /// calls from this factory.
        /// </summary>
        /// <value>The <see cref="System.Net.Http.HttpClient"/> to use when creating API
        /// calls from this factory.</value>
        protected HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation constructs an instance of <see cref="JsonHttpApiCall{T}"/>
        /// using the <see cref="HttpCompletionOption.ResponseContentRead"/> completion option.
        /// </remarks>
        public virtual HttpApiCall<T> CreateJsonApiCall<T>(HttpRequestMessage requestMessage)
        {
            return new JsonHttpApiCall<T>(HttpClient, requestMessage, HttpCompletionOption.ResponseContentRead);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation constructs an instance of <see cref="StreamingHttpApiCall"/>.
        /// </remarks>
        public virtual HttpApiCall<Stream> CreateStreamingApiCall(HttpRequestMessage requestMessage)
        {
            return new StreamingHttpApiCall(HttpClient, requestMessage);
        }

        /// <summary>
        /// Creates an HTTP API call where the response body is a treated as a simple string.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to calling <see cref="CreateBasicApiCall(HttpRequestMessage, HttpCompletionOption)"/>
        /// with the completion option set to <see cref="HttpCompletionOption.ResponseContentRead"/>.
        /// </remarks>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API call.</param>
        /// <returns>An HTTP API call which returns the response body as a string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="requestMessage"/> is <see langword="null"/>.</exception>
        public HttpApiCall CreateBasicApiCall(HttpRequestMessage requestMessage)
        {
            return CreateBasicApiCall(requestMessage, HttpCompletionOption.ResponseContentRead);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation constructs an instance of <see cref="HttpApiCall"/>.
        /// </remarks>
        public virtual HttpApiCall CreateBasicApiCall(HttpRequestMessage requestMessage, HttpCompletionOption completionOption)
        {
            return new HttpApiCall(HttpClient, requestMessage, completionOption);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation constructs an instance of <see cref="CustomHttpApiCall{T}"/>.
        /// </remarks>
        public HttpApiCall<T> CreateCustomApiCall<T>(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<HttpResponseMessage, CancellationToken, Task<T>> deserializeResult)
        {
            return new CustomHttpApiCall<T>(HttpClient, requestMessage, completionOption, deserializeResult);
        }
    }
}
