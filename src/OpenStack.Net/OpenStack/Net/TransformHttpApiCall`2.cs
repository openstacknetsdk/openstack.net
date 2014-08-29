namespace OpenStack.Net
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// Provides the base class for an HTTP API call that delegates behavior to another <see cref="IHttpApiCall{T}"/>
    /// instance, and performs a transformation operation on the result of the underlying implementation.
    /// </summary>
    /// <remarks>
    /// <para>This class can be used to simplify the creation of API-specific implementations of
    /// <see cref="IHttpApiCall{T}"/> which do not interpret the result in the same manner as the original call.</para>
    /// </remarks>
    /// <typeparam name="TSource">The class modeling the response returned by the underlying call.</typeparam>
    /// <typeparam name="TResult">The class modeling the transformed response returned by the current call.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class TransformHttpApiCall<TSource, TResult> : IHttpApiCall<TResult>
    {
        /// <summary>
        /// This is the backing field for the <see cref="HttpApiCall"/> property.
        /// </summary>
        private readonly IHttpApiCall<TSource> _httpApiCall;

        /// <summary>
        /// This is the backing field for the <see cref="Selector"/> property.
        /// </summary>
        private readonly Func<HttpResponseMessage, TSource, CancellationToken, TResult> _selector;

        /// <inheritdoc/>
        public event EventHandler<HttpRequestEventArgs> BeforeAsyncWebRequest;

        /// <inheritdoc/>
        public event EventHandler<HttpResponseEventArgs> AfterAsyncWebResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformHttpApiCall{TSource, TResult}"/> class with the
        /// behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <param name="selector">The function which selects the result for this API call from the result provided by
        /// the underlying <paramref name="httpApiCall"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="httpApiCall"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="selector"/> is <see langword="null"/>.</para>
        /// </exception>
        public TransformHttpApiCall(IHttpApiCall<TSource> httpApiCall, Func<HttpResponseMessage, TSource, CancellationToken, TResult> selector)
        {
            if (httpApiCall == null)
                throw new ArgumentNullException("httpApiCall");
            if (selector == null)
                throw new ArgumentNullException("selector");

            _httpApiCall = httpApiCall;
            _selector = selector;
            _httpApiCall.BeforeAsyncWebRequest += HandleBeforeAsyncWebRequest;
            _httpApiCall.AfterAsyncWebResponse += HandleAfterAsyncWebResponse;
        }

        /// <inheritdoc/>
        public HttpCompletionOption CompletionOption
        {
            get
            {
                return _httpApiCall.CompletionOption;
            }

            set
            {
                _httpApiCall.CompletionOption = value;
            }
        }

        /// <inheritdoc/>
        public HttpClient HttpClient
        {
            get
            {
                return _httpApiCall.HttpClient;
            }

            set
            {
                _httpApiCall.HttpClient = value;
            }
        }

        /// <inheritdoc/>
        public HttpRequestMessage RequestMessage
        {
            get
            {
                return _httpApiCall.RequestMessage;
            }

            set
            {
                _httpApiCall.RequestMessage = value;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="IHttpApiCall{T}"/> instance which provides the
        /// behavior for this API call.
        /// </summary>
        /// <value>The underlying <see cref="IHttpApiCall{T}"/> for this instance.</value>
        protected IHttpApiCall<TSource> HttpApiCall
        {
            get
            {
                return _httpApiCall;
            }
        }

        /// <summary>
        /// Gets the function which transforms the result of the underlying <see cref="HttpApiCall"/> into an instance
        /// of <typeparamref name="TResult"/> which is the response to the current call.
        /// </summary>
        protected Func<HttpResponseMessage, TSource, CancellationToken, TResult> Selector
        {
            get
            {
                return _selector;
            }
        }

        /// <inheritdoc/>
        public virtual Task<Tuple<HttpResponseMessage, TResult>> SendAsync(CancellationToken cancellationToken)
        {
            return HttpApiCall.SendAsync(cancellationToken)
                .Select(task => Tuple.Create(task.Result.Item1, Selector(task.Result.Item1, task.Result.Item2, cancellationToken)));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the managed and unmanaged resources used by this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if this method is was called from <see cref="Dispose()"/>; otherwise, <see langword="false"/> if this method was called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _httpApiCall.Dispose();
        }

        /// <summary>
        /// This method provides the handling for the <see cref="IHttpApiRequest.BeforeAsyncWebRequest"/> event
        /// sent by the underlying <see cref="HttpApiCall"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method sends the <see cref="BeforeAsyncWebRequest"/>
        /// event for the current instance.
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void HandleBeforeAsyncWebRequest(object sender, HttpRequestEventArgs e)
        {
            var handler = BeforeAsyncWebRequest;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// This method provides the handling for the <see cref="IHttpApiRequest.AfterAsyncWebResponse"/> event
        /// sent by the underlying <see cref="HttpApiCall"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method sends the <see cref="AfterAsyncWebResponse"/>
        /// event for the current instance.
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void HandleAfterAsyncWebResponse(object sender, HttpResponseEventArgs e)
        {
            var handler = AfterAsyncWebResponse;
            if (handler != null)
                handler(this, e);
        }
    }
}
