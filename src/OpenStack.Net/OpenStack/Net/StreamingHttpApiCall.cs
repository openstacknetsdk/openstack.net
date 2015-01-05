namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class defines an HTTP API call where the response body is returned as a
    /// <see cref="Stream"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class StreamingHttpApiCall : HttpApiCall<Stream>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingHttpApiCall"/> class with
        /// the specified <see cref="HttpClient"/> and request message.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// </exception>
        public StreamingHttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage)
            : base(httpClient, requestMessage, HttpCompletionOption.ResponseHeadersRead)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingHttpApiCall"/> class with
        /// the specified <see cref="HttpClient"/>, request message, and user-defined function
        /// to implement the response validation behavior.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <param name="validate">A user-defined function to validate the HTTP response, or <see langword="null"/> to use the default validation behavior.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// </exception>
        public StreamingHttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> validate)
            : base(httpClient, requestMessage, HttpCompletionOption.ResponseHeadersRead, validate)
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation returns the result of <see cref="HttpContent.ReadAsStreamAsync"/>.
        /// The content type header of the response is not tested for acceptability.
        /// </remarks>
        protected override Task<Stream> DeserializeResultImplAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return response.Content.ReadAsStreamAsync();
        }
    }
}
