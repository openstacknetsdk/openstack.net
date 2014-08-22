namespace OpenStack.Net
{
    using System;
    using System.Threading.Tasks;
    using CancellationToken = System.Threading.CancellationToken;
    using HttpCompletionOption = System.Net.Http.HttpCompletionOption;
    using HttpRequestMessage = System.Net.Http.HttpRequestMessage;
    using HttpResponseMessage = System.Net.Http.HttpResponseMessage;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This interface provides methods to create instances of <see cref="IHttpApiCall{T}"/>
    /// for several common cases.
    /// </summary>
    /// <remarks>
    /// Extension methods which extend the base functionality of a service interface may use
    /// this factory to create API call instances, allowing the service client to customize
    /// aspects of the request and add event handlers for the events in
    /// <see cref="IHttpApiRequest"/>.
    /// </remarks>
    /// <preliminary/>
    public interface IHttpApiCallFactory
    {
        /// <summary>
        /// Create an HTTP API call where the response body is a JSON-serialized representation
        /// of an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API call.</param>
        /// <returns>An HTTP API call which deserializes the JSON response body to obtain the strongly-typed result of the call.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="requestMessage"/> is <see langword="null"/>.</exception>
        HttpApiCall<T> CreateJsonApiCall<T>(HttpRequestMessage requestMessage);

        /// <summary>
        /// Create an HTTP API call which returns a <see cref="Stream"/> for reading the response body.
        /// </summary>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API call.</param>
        /// <returns>An HTTP API call which returns a <see cref="Stream"/> for reading the response body.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="requestMessage"/> is <see langword="null"/>.</exception>
        HttpApiCall<Stream> CreateStreamingApiCall(HttpRequestMessage requestMessage);

        /// <summary>
        /// Create an HTTP API call where the response body is a treated as a simple string.
        /// </summary>
        /// <remarks>
        /// This method can be used for API calls that do not return a body. In those cases,
        /// the response is treated as an empty string.
        /// </remarks>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <returns>An HTTP API call which returns the response body as a string.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="completionOption"/> is not a valid <see cref="HttpCompletionOption"/>.
        /// </exception>
        HttpApiCall CreateBasicApiCall(HttpRequestMessage requestMessage, HttpCompletionOption completionOption);

        /// <summary>
        /// Create an HTTP API call where the deserialization logic is provided as a user-defined function.
        /// </summary>
        /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the request for the API call.</param>
        /// <param name="completionOption">The <see cref="HttpCompletionOption"/> to specify when sending the HTTP request.</param>
        /// <param name="deserializeResult">A user-defined function to perform deserialization of the HTTP response.</param>
        /// <returns>An HTTP API call which deserializes the response to an instance of <typeparamref name="T"/> using a user-defined function.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="deserializeResult"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="completionOption"/> is not a valid <see cref="HttpCompletionOption"/>.
        /// </exception>
        HttpApiCall<T> CreateCustomApiCall<T>(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<HttpResponseMessage, CancellationToken, Task<T>> deserializeResult);
    }
}
