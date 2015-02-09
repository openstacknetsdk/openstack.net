namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using Newtonsoft.Json;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to upload and extract an archive in the Object Storage Service, creating
    /// or replacing objects with the contents of the archive.
    /// </summary>
    /// <remarks>
    /// <para>This API call is part of the <see cref="PredefinedObjectStorageExtensions.ExtractArchive">Extract
    /// Archive</see> extension to the OpenStack Object Storage Service V1.</para>
    /// </remarks>
    /// <seealso cref="O:OpenStack.Services.ObjectStorage.V1.IExtractArchiveExtension.PrepareExtractArchiveAsync"/>
    /// <seealso cref="O:OpenStack.Services.ObjectStorage.V1.ExtractArchiveExtensions.ExtractArchiveAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ExtractArchiveApiCall : DelegatingHttpApiCall<ExtractArchiveResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveApiCall"/> class with the behavior provided by
        /// another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ExtractArchiveApiCall(IHttpApiCall<ExtractArchiveResponse> httpApiCall)
            : base(httpApiCall)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveApiCall"/> class using the default behavior of
        /// modifying a <see cref="CopyObjectApiCall"/> to produce the desired behavior.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="CreateObjectApiCall"/> which creates the archive object in the
        /// Object Storage Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ExtractArchiveApiCall(CreateObjectApiCall httpApiCall)
            : this(WrapCreateObjectCall(httpApiCall))
        {
        }

        /// <summary>
        /// Deserialize the body of the response to the HTTP API call as a JSON-encoded
        /// <see cref="ExtractArchiveResponse"/> instance.
        /// </summary>
        /// <param name="response">The raw response to the HTTP API call.</param>
        /// <param name="result">The original content of the response as a string.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>An <see cref="ExtractArchiveResponse"/> instance representing the result of the HTTP API call.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/>, if the <see cref="HttpContentHeaders.ContentType"/> header of the response is
        /// not acceptable according to the <see cref="HttpRequestHeaders.Accept"/> header of the request.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="response"/> is <see langword="null"/>.
        /// </exception>
        protected static ExtractArchiveResponse SelectResult(HttpResponseMessage response, string result, CancellationToken cancellationToken)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (!OpenStack.Net.HttpApiCall.IsAcceptable(response))
                return null;

            return JsonConvert.DeserializeObject<ExtractArchiveResponse>(result);
        }

        /// <summary>
        /// Create an <see cref="IHttpApiCall{T}"/> instance providing the behavior for the
        /// <see cref="ExtractArchiveApiCall"/> API call by applying the <see cref="SelectResult"/> transformation to
        /// the result of a <see cref="CreateObjectApiCall"/> API call.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="CreateObjectApiCall"/> which creates the archive object in the
        /// Object Storage Service.</param>
        /// <returns>An <see cref="IHttpApiCall{T}"/> providing the behavior for the
        /// <see cref="ExtractArchiveApiCall"/> API call.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        protected static IHttpApiCall<ExtractArchiveResponse> WrapCreateObjectCall(CreateObjectApiCall httpApiCall)
        {
            return new TransformHttpApiCall<string, ExtractArchiveResponse>(httpApiCall, SelectResult);
        }
    }
}
