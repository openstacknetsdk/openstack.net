namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// This class defines general extension methods for implementations of <see cref="IHttpApiRequest"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class HttpApiRequestExtensions
    {
        /// <summary>
        /// Replaces the <see cref="HttpRequestMessage.RequestUri"/> for an HTTP API request.
        /// </summary>
        /// <typeparam name="T">The specific type of HTTP API request.</typeparam>
        /// <param name="apiRequest">The prepared HTTP API request.</param>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>Returns the input argument <paramref name="apiRequest"/>, which was modified to use the specified
        /// <paramref name="uri"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="apiRequest"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="uri"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API request has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API request has already been sent.</exception>
        public static T WithUri<T>(this T apiRequest, Uri uri)
            where T : IHttpApiRequest
        {
            if (apiRequest == null)
                throw new ArgumentNullException("apiRequest");
            if (uri == null)
                throw new ArgumentNullException("uri");

            apiRequest.RequestMessage.RequestUri = uri;
            return apiRequest;
        }

        /// <summary>
        /// Replaces the <see cref="HttpRequestMessage.RequestUri"/> for an HTTP API request.
        /// </summary>
        /// <remarks>
        /// <para>This method simplifies the use of <see cref="WithUri{T}(T, Uri)"/> in scenarios where
        /// <see langword="async/await"/> are not used for the preparation of the HTTP API request.</para>
        /// </remarks>
        /// <typeparam name="T">The specific type of HTTP API request.</typeparam>
        /// <param name="apiRequest">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP
        /// API request.</param>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input task
        /// <paramref name="apiRequest"/>, which was modified according to the specified
        /// <paramref name="uri"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="apiRequest"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="uri"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API request has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API request has already been sent.</exception>
        public static Task<T> WithUri<T>(this Task<T> apiRequest, Uri uri)
            where T : IHttpApiRequest
        {
            return apiRequest.Select(task => task.Result.WithUri(uri));
        }
    }
}
