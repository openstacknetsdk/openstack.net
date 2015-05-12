namespace OpenStack.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Net;

    /// <summary>
    /// This is the base interface for a service API exposed over HTTP. It defines the essential methods required for
    /// applications to write extension methods to support arbitrary custom features of the service, as opposed to
    /// requiring applications derived from an actual implementation.
    /// </summary>
    /// <preliminary/>
    public interface IHttpService : IDisposable
    {
        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpRequestMessage"/> for use in
        /// asynchronous HTTP API calls. The input to the continuation function is a completed task which provides the
        /// base absolute URI for the service for use in binding the URI templates for HTTP API calls, and the
        /// <see cref="CancellationToken"/> which the task should observe.
        /// </summary>
        /// <typeparam name="T">
        /// The type used to represent replacement parameters for the URI Template expansion process.
        /// </typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpRequestMessage"/>
        /// following the completion of a task that obtains the base URI for a service.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="method"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="template"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        Func<Task<Uri>, Task<HttpRequestMessage>> PrepareRequestAsyncFunc<T>(HttpMethod method, UriTemplate template, IDictionary<string, T> parameters, CancellationToken cancellationToken, string mediaType);

        /// <summary>
        /// Creates a task continuation function responsible for creating an <see cref="HttpRequestMessage"/> for use in
        /// asynchronous HTTP API calls. The input to the continuation function is a completed task which provides the
        /// base absolute URI for the service for use in binding the URI templates for HTTP API calls, and the
        /// <see cref="CancellationToken"/> which the task should observe.
        /// </summary>
        /// <typeparam name="T">
        /// The type used to represent replacement parameters for the URI Template expansion process.
        /// </typeparam>
        /// <typeparam name="TBody">The type modeling the body of the request.</typeparam>
        /// <param name="method">The <see cref="HttpMethod"/> to use for the request.</param>
        /// <param name="template">The <see cref="UriTemplate"/> for the target URI.</param>
        /// <param name="parameters">A collection of parameters for binding the URI template in a call to
        /// <see cref="UriTemplate.BindByName{T}(Uri, IDictionary{string, T})"/>.</param>
        /// <param name="body">A object modeling the body of the request. The service client implementation serializes
        /// this object during the request preparation into an instance of <see cref="HttpContent"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A task continuation delegate which can be used to create an <see cref="HttpRequestMessage"/>
        /// following the completion of a task that obtains the base URI for a service.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="method"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="template"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameters"/> is <see langword="null"/>.</para>
        /// </exception>
        Func<Task<Uri>, Task<HttpRequestMessage>> PrepareRequestAsyncFunc<T, TBody>(HttpMethod method, UriTemplate template, IDictionary<string, T> parameters, TBody body, CancellationToken cancellationToken, string mediaType);

        /// <summary>
        /// Gets the base absolute URI to use for making asynchronous HTTP API calls to this service.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain a <see cref="Uri"/> representing the base
        /// absolute URI for the service.
        /// </returns>
        Task<Uri> GetBaseUriAsync(CancellationToken cancellationToken);
    }
}
