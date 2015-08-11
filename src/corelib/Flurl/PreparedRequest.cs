using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Content;

// ReSharper disable once CheckNamespace
namespace Flurl.Http
{
    /// <summary>
    /// Represents a prepared Flurl request which can be executed at a later time.
    /// </summary>
    public class PreparedRequest : FlurlClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedRequest"/> class.
        /// </summary>
        public PreparedRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedRequest"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public PreparedRequest(string url) : base(url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedRequest"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public PreparedRequest(Url url) : base(url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedRequest"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="autoDispose">Specifies if the request should be automatically disposed.</param>
        public PreparedRequest(string url, bool autoDispose) : base(url, autoDispose)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedRequest"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="autoDispose">Specifies if the request should be automatically disposed.</param>
        public PreparedRequest(Url url, bool autoDispose) : base(url, autoDispose)
        {
        }

        /// <summary>
        /// The HTTP verb which will be used in the request.
        /// </summary>
        public HttpMethod Verb { get; protected set; }

        /// <summary>
        /// The HTTP content which will be used in the request.
        /// </summary>
        public HttpContent Content { get; protected set; }

        /// <summary>
        /// The optional canellation token which will be used in the request, defaults to None.
        /// </summary>
        public CancellationToken CancellationToken { get; protected set; }

        /// <summary>
        /// Prepares the client to send a DELETE request
        /// </summary>
        public PreparedRequest PrepareDelete(CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Delete;
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Prepares the client to send a GET request
        /// </summary>
        public PreparedRequest PrepareGet(CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Get;
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Prepares the client to send a PATCH request containing json
        /// </summary>
        public PreparedRequest PreparePatchJson(object data, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = new HttpMethod("PATCH");
            Content = new CapturedJsonContent(data);
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Prepares the client to send a POST request containing json
        /// </summary>
        public PreparedRequest PreparePostJson(object data, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Post;
            Content = new CapturedJsonContent(data);
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Prepares the client to send a PUT request containing json
        /// </summary>
        public PreparedRequest PreparePutJson(object data, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Put;
            Content = new CapturedJsonContent(data);
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Executes the built request
        /// </summary>
        public Task<HttpResponseMessage> SendAsync()
        {
            if(Verb == null)
                throw new InvalidOperationException("Unable to execute request as nothing has been built yet.");

            return SendAsync(Verb, Content, CancellationToken);
        }
    }
}