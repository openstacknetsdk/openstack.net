namespace OpenStack.Net
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface represents a complete HTTP API, which extends <see cref="IHttpApiRequest"/>
    /// with the ability to send the request and obtain a strongly-typed representation of the
    /// response.
    /// </summary>
    /// <typeparam name="T">The type modeling the response to the API call.</typeparam>
    /// <preliminary/>
    public interface IHttpApiCall<T> : IHttpApiRequest
    {
        /// <summary>
        /// Sends the HTTP request and obtains the strongly-typed response as an
        /// asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// a tuple; the first element is the <see cref="HttpResponseMessage"/> resulting
        /// from sending the HTTP request and the second element is the deserialized
        /// representation of the response to the API call.
        /// </returns>
        /// <exception cref="HttpWebException">If the server response to the HTTP request indicates an error occurred.</exception>
        /// <exception cref="WebException">If an error occurs while sending the HTTP request.</exception>
        Task<Tuple<HttpResponseMessage, T>> SendAsync(CancellationToken cancellationToken);
    }
}
