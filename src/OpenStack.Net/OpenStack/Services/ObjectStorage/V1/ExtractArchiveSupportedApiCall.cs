namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Immutable;
    using System.Net.Http;
    using System.Threading;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to determine if the Extract Archive extension is supported by a
    /// particular Object Storage Service endpoint.
    /// </summary>
    /// <remarks>
    /// <para>This API call is part of the <see cref="PredefinedObjectStorageExtensions.ExtractArchive">Extract
    /// Archive</see> extension to the OpenStack Object Storage Service V1.</para>
    /// </remarks>
    /// <seealso cref="IExtractArchiveExtension.PrepareExtractArchiveSupportedAsync"/>
    /// <seealso cref="ExtractArchiveExtensions.SupportsExtractArchiveAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ExtractArchiveSupportedApiCall : DelegatingHttpApiCall<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveSupportedApiCall"/> class with the behavior
        /// provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ExtractArchiveSupportedApiCall(IHttpApiCall<bool> httpApiCall)
            : base(httpApiCall)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveSupportedApiCall"/> class using the default
        /// behavior of checking for the <c>bulk_upload</c> property in the result of a
        /// <see cref="GetObjectStorageInfoApiCall"/> API call.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="GetObjectStorageInfoApiCall"/> which provides information about
        /// the Object Storage Service installation.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ExtractArchiveSupportedApiCall(GetObjectStorageInfoApiCall httpApiCall)
            : this(WrapGetObjectStorageInfoCall(httpApiCall))
        {
        }

        /// <summary>
        /// This method examines the result of a <see cref="GetObjectStorageInfoApiCall"/> HTTP API call to determine if
        /// the Extract Archive extension is reported as supported.
        /// </summary>
        /// <remarks>
        /// <para>This method determines that the Extract Archive operation is supported solely based on the presence of
        /// a <c>bulk_upload</c> key in the <paramref name="result"/> dictionary.</para>
        /// </remarks>
        /// <param name="response">The HTTP response to sending the <see cref="GetObjectStorageInfoApiCall"/> API
        /// call.</param>
        /// <param name="result">The deserialized result from the <see cref="GetObjectStorageInfoApiCall"/> API
        /// call.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><see langword="true"/> if the Object Storage Service reports that the Extract Archive operation is
        /// supported; otherwise, <see langword="false"/>.</returns>
        protected static bool SelectResult(HttpResponseMessage response, ImmutableDictionary<string, JToken> result, CancellationToken cancellationToken)
        {
            return result != null && result.ContainsKey("bulk_upload");
        }

        /// <summary>
        /// Create an <see cref="IHttpApiCall{T}"/> instance providing the behavior for the
        /// <see cref="ExtractArchiveSupportedApiCall"/> API call by applying the <see cref="SelectResult"/>
        /// transformation to the result of a <see cref="GetObjectStorageInfoApiCall"/> API call.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="GetObjectStorageInfoApiCall"/> which provides information about
        /// the Object Storage Service installation.</param>
        /// <returns>An <see cref="IHttpApiCall{T}"/> providing the behavior for the
        /// <see cref="ExtractArchiveSupportedApiCall"/> API call.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        protected static IHttpApiCall<bool> WrapGetObjectStorageInfoCall(GetObjectStorageInfoApiCall httpApiCall)
        {
            return new TransformHttpApiCall<ImmutableDictionary<string, JToken>, bool>(httpApiCall, SelectResult);
        }
    }
}
