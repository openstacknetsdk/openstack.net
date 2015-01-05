namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rackspace.Threading;

    /// <summary>
    /// This class defines an HTTP API call where the response body is a JSON-serialized
    /// representation of an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <note type="caller">
    /// If the content type of the HTTP response is not acceptable according to the <c>Accept</c>
    /// header of the request, the body is assumed to not be valid JSON and the default
    /// value for type <typeparamref name="T"/> is return as the result of the call. If
    /// necessary code can use the <see cref="HttpApiCall.IsAcceptable(HttpResponseMessage)"/>
    /// method to verify that deserialization was properly performed.
    /// </note>
    /// </remarks>
    /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class JsonHttpApiCall<T> : HttpApiCall<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHttpApiCall{T}"/> class with
        /// the specified <see cref="HttpClient"/>, request message, and completion option.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// </exception>
        public JsonHttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption)
            : base(httpClient, requestMessage, completionOption)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHttpApiCall{T}"/> class with
        /// the specified <see cref="HttpClient"/>, request message, completion option, and
        /// user-defined function to implement the response validation behavior.
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
        public JsonHttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> validate)
            : base(httpClient, requestMessage, completionOption, validate)
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation first verifies that the content type of the response is acceptable
        /// according to the <c>Accept</c> header of the request. If acceptable, the body is assumed to be
        /// in JSON syntax and is deserialized using <see cref="JsonConvert.DeserializeObject{T}(string)"/>.
        /// Otherwise, the method returns the default value of type <typeparamref name="T"/>.
        /// </remarks>
        protected override Task<T> DeserializeResultImplAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            bool acceptable = HttpApiCall.IsAcceptable(response);
            return response.Content.ReadAsStringAsync()
                .Select(task => acceptable ? JsonConvert.DeserializeObject<T>(task.Result) : default(T));
        }
    }
}
