namespace OpenStack.Net
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// This class represents a call to an HTTP API. It provides clients the opportunity to inspect
    /// and/or modify the <see cref="RequestMessage"/> prior to sending the request, without losing
    /// the ability to obtain a strongly-typed result from <see cref="SendAsync"/>.
    /// </summary>
    /// <typeparam name="T">The class modeling the response returned by the call.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class HttpApiCall<T> : IHttpApiCall<T>, IDisposable
    {
        private static readonly Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> DefaultResponseValidator =
            (task, cancellationToken) => task;

        /// <summary>
        /// This is the backing field for the <see cref="HttpClient"/> property.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// This is the backing field for the <see cref="RequestMessage"/> property.
        /// </summary>
        private HttpRequestMessage _requestMessage;

        /// <summary>
        /// This is the backing field for the <see cref="CompletionOption"/> property.
        /// </summary>
        private HttpCompletionOption _completionOption;

        /// <summary>
        /// This is the backing field for the <see cref="ValidateCallback"/> property.
        /// </summary>
        private Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> _validate;

        /// <summary>
        /// This field is set by <see cref="Dispose(bool)"/> and supports the <see cref="ThrowIfDisposed"/> method.
        /// </summary>
        private bool _disposed;

        /// <inheritdoc/>
        public event EventHandler<HttpRequestEventArgs> BeforeAsyncWebRequest;

        /// <inheritdoc/>
        public event EventHandler<HttpResponseEventArgs> AfterAsyncWebResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpApiCall{T}"/> class with
        /// the specified <see cref="HttpClient"/>, request message and completion option.
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
            : this(httpClient, requestMessage, completionOption, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpApiCall{T}"/> class with
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
        public HttpApiCall(HttpClient httpClient, HttpRequestMessage requestMessage, HttpCompletionOption completionOption, Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> validate)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");
            if (requestMessage == null)
                throw new ArgumentNullException("requestMessage");

            _httpClient = httpClient;
            _requestMessage = requestMessage;
            _completionOption = completionOption;
            _validate = validate ?? DefaultResponseValidator;
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">If the current instance has been disposed.</exception>
        public HttpClient HttpClient
        {
            get
            {
                ThrowIfDisposed();
                return _httpClient;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                ThrowIfDisposed();

                _httpClient = value;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">If the current instance has been disposed.</exception>
        public HttpRequestMessage RequestMessage
        {
            get
            {
                ThrowIfDisposed();
                return _requestMessage;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                ThrowIfDisposed();

                _requestMessage = value;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">If the current instance has been disposed.</exception>
        public HttpCompletionOption CompletionOption
        {
            get
            {
                ThrowIfDisposed();
                return _completionOption;
            }

            set
            {
                ThrowIfDisposed();
                _completionOption = value;
            }
        }

        /// <summary>
        /// Gets or sets the function to use for validating an HTTP response to the call.
        /// </summary>
        /// <remarks>
        /// If this property is set to <see langword="null"/>, a default validation function
        /// will be used instead.
        /// </remarks>
        /// <value>The function to use for validating an HTTP response to the call.</value>
        /// <exception cref="ObjectDisposedException">If the current instance has been disposed.</exception>
        protected Func<Task<HttpResponseMessage>, CancellationToken, Task<HttpResponseMessage>> ValidateCallback
        {
            get
            {
                ThrowIfDisposed();
                return _validate;
            }

            set
            {
                ThrowIfDisposed();
                _validate = value ?? DefaultResponseValidator;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation calls <see cref="SendImplAsync"/> to send the HTTP request, the validates the response
        /// using <see cref="ValidateCallback"/>, and finally deserializes the result to an instance of
        /// <typeparamref name="T"/> using <see cref="DeserializeResultImplAsync"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">If the current instance has been disposed.</exception>
        public Task<Tuple<HttpResponseMessage, T>> SendAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return SendImplAsync(cancellationToken)
                .Then(task => ValidateCallback(task, cancellationToken))
                .Then(
                    task =>
                    {
                        return DeserializeResultImplAsync(task.Result, cancellationToken)
                            .Select(innerTask => Tuple.Create(task.Result, innerTask.Result));
                    });
        }

        /// <summary>
        /// Invokes the <see cref="BeforeAsyncWebRequest"/> event for the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <see langword="null"/>.</exception>
        protected virtual void OnBeforeAsyncWebRequest(HttpRequestMessage request)
        {
            var handler = BeforeAsyncWebRequest;
            if (handler != null)
                handler(this, new HttpRequestEventArgs(request));
        }

        /// <summary>
        /// Invokes the <see cref="AfterAsyncWebResponse"/> event for the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="response"/> is <see langword="null"/>.</exception>
        protected virtual void OnAfterAsyncWebResponse(Task<HttpResponseMessage> response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            var handler = AfterAsyncWebResponse;
            if (handler != null)
                handler(this, new HttpResponseEventArgs(response));
        }

        /// <summary>
        /// This method provides the behavior for sending an HTTP request.
        /// </summary>
        /// <remarks>
        /// The default implementation first calls <see cref="OnBeforeAsyncWebRequest"/>. It
        /// then uses <see cref="HttpClient"/> to send the request, followed by calling
        ///  <see cref="OnAfterAsyncWebResponse"/> to notify any listeners of the response.
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will
        /// contain an <see cref="HttpResponseMessage"/> object describing the HTTP response.
        /// </returns>
        protected virtual Task<HttpResponseMessage> SendImplAsync(CancellationToken cancellationToken)
        {
            OnBeforeAsyncWebRequest(RequestMessage);
            return HttpClient.SendAsync(RequestMessage, CompletionOption, cancellationToken)
                .Finally(task => OnAfterAsyncWebResponse(task));
        }

        /// <summary>
        /// This method implements the deserialization behavior for HTTP responses.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> describing HTTP response.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will
        /// contain the result of the API call as an instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="response"/> is <see langword="null"/>.</exception>
        protected abstract Task<T> DeserializeResultImplAsync(HttpResponseMessage response, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the managed and unmanaged resources owned by this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if this method is called from <see cref="Dispose()"/>; otherwise, <see langword="false"/> if this method is called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _requestMessage.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Throw an <see cref="ObjectDisposedException"/> if the current instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the current instance has been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
