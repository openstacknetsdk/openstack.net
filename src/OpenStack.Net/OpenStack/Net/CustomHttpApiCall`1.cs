namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class provides an implementation of <see cref="HttpApiCall{T}"/> that implements
    /// <see cref="DeserializeResultImplAsync"/> by calling a user-defined delegate.
    /// </summary>
    /// <typeparam name="T">The class modeling the response returned by the call.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class CustomHttpApiCall<T> : HttpApiCall<T>
    {
        private readonly Func<HttpResponseMessage, CancellationToken, Task<T>> _deserializeResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpApiCall{T}"/> class with
        /// the specified <see cref="HttpClient"/>, request message, completion option, and
        /// user-defined function to implement the response deserialization behavior.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <param name="deserializeResult">A user-defined function to perform deserialization of the HTTP response.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="deserializeResult"/> is <see langword="null"/>.</para>
        /// </exception>
        public CustomHttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<HttpResponseMessage, CancellationToken, Task<T>> deserializeResult)
            : base(httpClient, requestMessage, completionOption)
        {
            if (deserializeResult == null)
                throw new ArgumentNullException("deserializeResult");

            _deserializeResult = deserializeResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpApiCall{T}"/> class with
        /// the specified <see cref="HttpClient"/>, request message, completion option, and
        /// user-defined functions to implement the response validation and deserialization
        /// behavior.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for HTTP requests.</param>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <param name="validate">A user-defined function to validate the HTTP response, or <see langword="null"/> to use the default validation behavior.</param>
        /// <param name="deserializeResult">A user-defined function to perform deserialization of the HTTP response.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpClient"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="requestMessage"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="deserializeResult"/> is <see langword="null"/>.</para>
        /// </exception>
        public CustomHttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> validate, Func<HttpResponseMessage, CancellationToken, Task<T>> deserializeResult)
            : base(httpClient, requestMessage, completionOption, validate)
        {
            if (deserializeResult == null)
                throw new ArgumentNullException("deserializeResult");

            _deserializeResult = deserializeResult;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The default implementation calls the provided function to perform deserialization.
        /// The content type header of the response is not tested for acceptability before calling
        /// the deserialization method.
        /// </remarks>
        protected override Task<T> DeserializeResultImplAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return _deserializeResult(response, cancellationToken);
        }
    }
}
