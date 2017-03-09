using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using Flurl.Http.Content;
using net.openstack.Core;
using OpenStack.ObjectStorage.v1.Metadata;
using OpenStack.ObjectStorage.v1.Serialization;
using OpenStack.Serialization;

// ReSharper disable once CheckNamespace
namespace Flurl.Http
{
    /// <summary>
    /// Represents a prepared Flurl request which can be executed at a later time.
    /// </summary>
    /// <exclude />
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
		/// Apply default timeout to current Request
		/// </summary>
		/// <param name="defaultTimeout">The timeout to apply.</param>
		/// <returns></returns>
	    public PreparedRequest SettingTimeout(TimeSpan defaultTimeout)
		{
			Settings.DefaultTimeout = defaultTimeout;
		    return this;
	    }


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
        /// Prepares the client to send a DELETE request
        /// </summary>
        public PreparedRequest PrepareHead(CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Head;
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
            Content = new CapturedJsonContent(Settings.JsonSerializer.Serialize(data));
            CancellationToken = cancellationToken;
            return this;
        }
		
        /// <summary>
        /// Prepares the client to send a POST request containing json
        /// </summary>
        public PreparedRequest PreparePostJson(object data, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Post;
            Content = new CapturedJsonContent(Settings.JsonSerializer.Serialize(data));
            CancellationToken = cancellationToken;
            return this;
        }
		
        /// <summary>
        /// Prepares the client to send a POST request containing form
        /// </summary>
        public PreparedRequest PreparePostForm(IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Post;

			// var multipartFormDataContent = new MultipartFormDataContent();
			//multipartFormDataContent.Add(new StringContent(pairValue), pair.Key.ToLowerInvariant().Replace('-', '_'));
	        /*foreach (var pair in formData)
	        {
		        foreach (var pairValue in pair.Value)
		        {
			        multipartFormDataContent.Add(new StringContent(pairValue), pair.Key.ToLowerInvariant().Replace('-', '_'));
		        }
	        }
			Content = multipartFormDataContent;*/
			// this.PostUrlEncodedAsync(formData, cancellationToken);
			Content = new CapturedUrlEncodedContent(
				Settings.UrlEncodedSerializer.Serialize(
					formData.SelectMany(
						pair => pair.Value,
						(pair, pairVal) => new KeyValuePair<string, string>(pair.Key, pairVal)
					)
				)
			);

            CancellationToken = cancellationToken;
            return this;
        }

		/// <summary>
		/// Prepares the client to send a POST request containing data in content Header
		/// </summary>
		/// <param name="formData"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public PreparedRequest PreparePostContentHeader(IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Post;

			Content = new System.Net.Http.StringContent("");
	        Content.Headers.Remove("Content-Type");
			foreach (var pair in formData)
	        {
		        foreach (var pairValue in pair.Value)
		        {
			        Content.Headers.Add(pair.Key, pairValue);
		        }
	        }

            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Prepares the client to send a POST request containing data in header
        /// </summary>
        public PreparedRequest PreparePostHeader(IEnumerable<IMetadata> data, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Post;
	        foreach (var keyValuePair in data)
	        {
		        this.HttpClient.DefaultRequestHeaders.Add(keyValuePair.MetadataKey, keyValuePair.MetadataValue);
	        }
            CancellationToken = cancellationToken;
            return this;
        }



        /// <summary>
        /// Prepares the client to send a PUT request containing json
        /// </summary>
        public PreparedRequest PreparePutJson(object data, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Put;
            Content = new CapturedJsonContent(Settings.JsonSerializer.Serialize(data));
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Prepares the client to send a PUT request containing data in stream
        /// </summary>
        public PreparedRequest PreparePutStream(System.IO.Stream dataStream, CancellationToken cancellationToken = default(CancellationToken))
        {
            Verb = HttpMethod.Put;
            Content = new CapturedStreamContent(dataStream);
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

    /// <summary />
    public static class PreparedRequestExtensions
    {
        /// <summary>
        /// Allow a specific set of HTTP status codes.
        /// </summary>
        /// <param name="request">The prepared request.</param>
        /// <param name="statusCodes">The allowed status codes.</param>
        /// <returns></returns>
        public static PreparedRequest AllowHttpStatus(this PreparedRequest request, params HttpStatusCode[] statusCodes)
        {
            return (PreparedRequest)((FlurlClient)request).AllowHttpStatus(statusCodes);
        }
    }
}