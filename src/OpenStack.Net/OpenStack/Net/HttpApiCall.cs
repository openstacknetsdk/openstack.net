namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides an implementation of <see cref="HttpApiCall{T}"/> returns
    /// the response body as a <see cref="string"/>.
    /// </summary>
    /// <remarks>
    /// This class supports both calls that return a text body that do not require special
    /// deserialization, as well as calls that do not return a response body at all. The
    /// latter is simply treated as an empty string.
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class HttpApiCall : HttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpApiCall"/> class
        /// with the specified <see cref="HttpClient"/>, request message, and
        /// completion option.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// </exception>
        public HttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption)
            : base(httpClient, requestMessage, completionOption)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpApiCall"/> class
        /// with the specified <see cref="HttpClient"/>, request message,
        /// completion option, and validation behavior.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <param name="validate">A user-defined function to validate the HTTP response, or <see langword="null"/> to use the default validation behavior.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// </exception>
        public HttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> validate)
            : base(httpClient, requestMessage, completionOption, validate)
        {
        }

        /// <summary>
        /// Determines if the body of an HTTP response is acceptable according to the <c>Accept</c>
        /// header sent in the HTTP request.
        /// </summary>
        /// <remarks>
        /// This method may be used by API calls that deserialize the response to avoid deserialization
        /// exceptions in cases where the response does not match any of the supported content types.
        /// If the HTTP request did not specify an <c>Accept</c> header, this method simply returns
        /// <see langword="true"/>.
        /// </remarks>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> representing the HTTP response.</param>
        /// <returns><see langword="true"/> if the content type of the response is acceptable according to the <c>Accept</c> header of the request; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="responseMessage"/> is <see langword="null"/>.</exception>
        public static bool IsAcceptable(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
                throw new ArgumentNullException("responseMessage");

            bool acceptable = true;
            if (responseMessage.RequestMessage != null)
            {
                HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> acceptHeaders = responseMessage.RequestMessage.Headers.Accept;
                if (acceptHeaders.Count > 0)
                {
                    MediaTypeHeaderValue contentType = responseMessage.Content.Headers.ContentType;
                    acceptable = false;
                    foreach (var acceptHeader in acceptHeaders)
                    {
                        if (IsAcceptable(acceptHeader, contentType))
                        {
                            acceptable = true;
                            break;
                        }
                    }
                }
            }

            return acceptable;
        }

        /// <summary>
        /// Determines if a content type is acceptable according to a specific <c>Accept</c> header value.
        /// </summary>
        /// <remarks>
        /// HTTP requests may specify multiple acceptable content types. For a more general test
        /// of the acceptability of an HTTP response according to its request, see
        /// <see cref="IsAcceptable(HttpResponseMessage)"/>.
        /// </remarks>
        /// <param name="acceptHeader">An <c>Accept</c> header value from an HTTP request.</param>
        /// <param name="contentType">The <c>Content-Type</c> header value from an HTTP response.</param>
        /// <returns><see langword="true"/> if the <paramref name="contentType"/> is acceptable according to the <paramref name="acceptHeader"/> value; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="acceptHeader"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="contentType"/> is <see langword="null"/>.</para>
        /// </exception>
        public static bool IsAcceptable(MediaTypeHeaderValue acceptHeader, MediaTypeHeaderValue contentType)
        {
            if (acceptHeader == null)
                throw new ArgumentNullException("acceptHeader");
            if (string.IsNullOrEmpty(acceptHeader.MediaType))
                throw new ArgumentException("acceptHeader.MediaType cannot be empty");

            // if the response doesn't include a Content-Type header, assume not acceptable
            if (contentType == null || string.IsNullOrEmpty(contentType.MediaType))
                return false;

            // handle wildcards
            if (acceptHeader.MediaType == "*/*")
                return true;

            if (acceptHeader.MediaType.EndsWith("/*"))
            {
                // include the slash, but not the *
                string acceptType = acceptHeader.MediaType.Substring(0, acceptHeader.MediaType.Length - 1);

                if (contentType.MediaType.Length < acceptType.Length)
                    return false;

                // case-insensitive "starts with"
                return StringComparer.OrdinalIgnoreCase.Equals(contentType.MediaType.Substring(0, acceptType.Length), acceptType);
            }

            // accept header without wildcards
            return StringComparer.OrdinalIgnoreCase.Equals(contentType.MediaType, acceptHeader.MediaType);
        }

        /// <inheritdoc/>
        protected override Task<string> DeserializeResultImplAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.Content == null)
                return CompletedTask.FromResult(default(string));

            return response.Content.ReadAsStringAsync();
        }
    }
}
