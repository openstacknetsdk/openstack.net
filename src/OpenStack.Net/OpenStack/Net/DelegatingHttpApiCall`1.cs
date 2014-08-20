namespace OpenStack.Net
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the base class for an HTTP API call that delegates behavior to
    /// another <see cref="IHttpApiCall{T}"/> instance.
    /// </summary>
    /// <remarks>
    /// This class can be used to simplify the creation of API-specific implementations
    /// of <see cref="IHttpApiCall{T}"/> while using the functionality of general
    /// classes such as <see cref="JsonHttpApiCall{T}"/> or <see cref="Net.HttpApiCall"/>.
    /// </remarks>
    /// <typeparam name="T">The class modeling the response returned by the call.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class DelegatingHttpApiCall<T> : IHttpApiCall<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="HttpApiCall"/> property.
        /// </summary>
        private readonly IHttpApiCall<T> _httpApiCall;

        /// <inheritdoc/>
        public event EventHandler<HttpRequestEventArgs> BeforeAsyncWebRequest;

        /// <inheritdoc/>
        public event EventHandler<HttpResponseEventArgs> AfterAsyncWebResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingHttpApiCall{T}"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public DelegatingHttpApiCall(IHttpApiCall<T> httpApiCall)
        {
            if (httpApiCall == null)
                throw new ArgumentNullException("httpApiCall");

            _httpApiCall = httpApiCall;
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
        protected IHttpApiCall<T> HttpApiCall
        {
            get
            {
                return _httpApiCall;
            }
        }

        /// <inheritdoc/>
        public Task<Tuple<HttpResponseMessage, T>> SendAsync(CancellationToken cancellationToken)
        {
            return _httpApiCall.SendAsync(cancellationToken);
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
