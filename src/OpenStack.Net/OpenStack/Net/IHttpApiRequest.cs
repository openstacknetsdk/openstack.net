namespace OpenStack.Net
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// This interface defines the basic definition of an HTTP request along with
    /// the behavior for sending the request.
    /// </summary>
    /// <preliminary/>
    public interface IHttpApiRequest : IDisposable
    {
        /// <summary>
        /// This event is fired immediately before sending an asynchronous web request.
        /// </summary>
        event EventHandler<HttpRequestEventArgs> BeforeAsyncWebRequest;

        /// <summary>
        /// This event is fired when the result of an asynchronous web request is received.
        /// </summary>
        event EventHandler<HttpResponseEventArgs> AfterAsyncWebResponse;

        /// <summary>
        /// Gets or sets the <see cref="HttpClient"/> to use for sending the HTTP request.
        /// </summary>
        /// <value>The <see cref="HttpClient"/> to use for sending the HTTP request.</value>
        HttpClient HttpClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="HttpRequestMessage"/> representing the HTTP request.
        /// </summary>
        /// <value>The <see cref="HttpRequestMessage"/> representing the HTTP request.</value>
        HttpRequestMessage RequestMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="HttpCompletionOption"/> to use when sending the HTTP request.
        /// </summary>
        /// <value>The <see cref="HttpCompletionOption"/> to use when sending the HTTP request.</value>
        HttpCompletionOption CompletionOption
        {
            get;
            set;
        }
    }
}
